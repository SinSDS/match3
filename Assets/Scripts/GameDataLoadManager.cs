using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class GameDataLoadManager : MonoSingleton<GameDataLoadManager>
{
	private bool waitOpenPopupNewLevel;

	public int LobbyLoadedLevelBallCount;

	public bool allTableReload;

	public void StartLoadData()
	{
		NetworkCheckTableVersion();
	}

	public void MoveToLobbyScene()
	{
		double num = PlayerPrefs.GetInt("LastLoginTime", 0);
		if (num > 0.0)
		{
			DateTime oldDateTime = Utils.UnixTimeStampToDateTime(num);
			if (AppEventCommonParameters.IsDifferentDay(oldDateTime))
			{
				MonoSingleton<AppEventManager>.Instance.SendAppEventDailyPlayerStatus();
			}
		}
		PlayerPrefs.SetInt("LastLoginTime", Utils.ConvertToTimestamp(DateTime.Now));
		MonoSingleton<UIManager>.Instance.HideLoading();
		if (MonoSingleton<SceneControlManager>.Instance.CurrentSceneType == SceneType.Title)
		{
			if (MonoSingleton<PlayerDataManager>.Instance.IsFirstAppInstall)
			{
				MonoSingleton<AppEventManager>.Instance.SendAppEventPlayerSessionStart(-1, AppEventManager.SessionStartedAs.First_Launch);
			}
			else
			{
				string sessionEndDateTimeString = PlayerDataManager.GetSessionEndDateTimeString();
				if (sessionEndDateTimeString == string.Empty)
				{
					string sessionStartDateTimeString = PlayerDataManager.GetSessionStartDateTimeString();
					if (sessionStartDateTimeString == string.Empty)
					{
						MonoSingleton<AppEventManager>.Instance.SendAppEventPlayerSessionStart(-100, AppEventManager.SessionStartedAs.Relaunched);
					}
					else if ((int)(DateTime.Now - Convert.ToDateTime(sessionStartDateTimeString)).TotalMinutes >= MonoSingleton<ServerDataTable>.Instance.SessionTimeMinute)
					{
						MonoSingleton<AppEventManager>.Instance.SendAppEventPlayerSessionStart((int)(DateTime.Now - Convert.ToDateTime(sessionStartDateTimeString)).TotalHours, AppEventManager.SessionStartedAs.Relaunched);
					}
				}
				else
				{
					int num2 = (int)(DateTime.Now - Convert.ToDateTime(sessionEndDateTimeString)).TotalMinutes;
					if (num2 < 0)
					{
						MonoSingleton<AppEventManager>.Instance.SendAppEventPlayerSessionStart(-101, AppEventManager.SessionStartedAs.Relaunched);
					}
					else if (num2 >= MonoSingleton<ServerDataTable>.Instance.SessionTimeMinute)
					{
						MonoSingleton<AppEventManager>.Instance.SendAppEventPlayerSessionStart((int)(DateTime.Now - Convert.ToDateTime(sessionEndDateTimeString)).TotalHours, AppEventManager.SessionStartedAs.Relaunched);
					}
				}
			}
			MonoSingleton<AppEventManager>.Instance.SendAppEventPlayerADSource();
		}
		PlayerDataManager.SetSessionStartDateTimeString();
		MonoSingleton<SceneControlManager>.Instance.LoadScene(SceneType.Lobby, SceneChangeEffect.Color);
	}

	private void NetworkCheckTableVersion()
	{
		NetRequestLoadGameTable.RequestCheckTableVersion(WaitNetworkCheckTableVersion);
	}

	private void WaitNetworkCheckTableVersion(ResPacketCheckTableVersion res)
	{
		ResPacketCheckTableVersion resPacketCheckTableVersion = LocalDataManager.GetTableVersion();
		MonoSingleton<ServerDataTable>.Instance.InitTable();
		if (resPacketCheckTableVersion == null)
		{
			resPacketCheckTableVersion = new ResPacketCheckTableVersion();
		}
		bool flag = false;
		int @int = PlayerPrefs.GetInt("LastVer", 0);
		PlayerPrefs.SetInt("LastVer", GlobalSetting.ConfigData.AppVersionNumber);
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		stringBuilder.Append("\"");
		if (res != null && res.m_DATA_VERSION != null)
		{
			for (int i = 0; i < res.m_DATA_VERSION.Length; i++)
			{
				bool flag2 = true;
				if (!allTableReload && resPacketCheckTableVersion != null && resPacketCheckTableVersion.m_DATA_VERSION != null)
				{
					int num2 = -1;
					for (int j = 0; j < resPacketCheckTableVersion.m_DATA_VERSION.Length; j++)
					{
						if (res.m_DATA_VERSION[i].table_name == resPacketCheckTableVersion.m_DATA_VERSION[j].table_name)
						{
							num2 = j;
							break;
						}
					}
					if (num2 != -1 && res.m_DATA_VERSION[i].table_version <= resPacketCheckTableVersion.m_DATA_VERSION[num2].table_version && LocalDataManager.ExistLocalTableFile(resPacketCheckTableVersion.m_DATA_VERSION[num2].table_name))
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					if (num > 0)
					{
						stringBuilder.Append(",");
					}
					num++;
					stringBuilder.Append(res.m_DATA_VERSION[i].table_name);
					flag = true;
				}
			}
			stringBuilder.Append("\"");
		}
		if (flag)
		{
			LocalDataManager.SaveTableVersion(res);
			NetRequestLoadGameTable.Request(WaitNetworkLoadSpecCommonTable, stringBuilder.ToString());
		}
	}

	private void WaitNetworkLoadSpecCommonTable(ResPacketLoadGameTable res)
	{
		int mAX_LEVEL = ServerDataTable.MAX_LEVEL;
		MonoSingleton<ServerDataTable>.Instance.SetTable(res);
		MonoSingleton<ServerDataTable>.Instance.LoadTableFromLocalFile();
		waitOpenPopupNewLevel = false;
		if (MonoSingleton<PlayerDataManager>.Instance.AllLevelCleared && ServerDataTable.MAX_LEVEL > MonoSingleton<PlayerDataManager>.Instance.CurrentLevelNo)
		{
			MonoSingleton<PlayerDataManager>.Instance.AllLevelCleared = false;
			MonoSingleton<PlayerDataManager>.Instance.CurrentLevelNo++;
			MonoSingleton<PlayerDataManager>.Instance.SaveCurrentLevel();
			waitOpenPopupNewLevel = true;
		}
		else if (ServerDataTable.MAX_LEVEL > mAX_LEVEL && MonoSingleton<PlayerDataManager>.Instance.CurrentLevelNo >= mAX_LEVEL - 20)
		{
			waitOpenPopupNewLevel = true;
		}
		StartCoroutine(waitLobbyScene());
	}

	private IEnumerator waitLobbyScene()
	{
		while (MonoSingleton<SceneControlManager>.Instance.CurrentSceneType != SceneType.Lobby)
		{
			yield return null;
		}
		if (MonoSingleton<SceneControlManager>.Instance.CurrentSceneType == SceneType.Lobby)
		{
			yield return null;
			MonoSingleton<LobbyManager>.Instance.OnEventRecvLevelData();
			if (waitOpenPopupNewLevel)
			{
				MonoSingleton<PopupManager>.Instance.OpenCommonPopup(PopupType.PopupCommonInfo, "Enjoy", "New Level Updated.", OnEventCloseNewLevelPopup, OnEventCloseNewLevelPopup);
				waitOpenPopupNewLevel = false;
			}
		}
	}

	private void OnEventCloseNewLevelPopup()
	{
		if (LobbyLoadedLevelBallCount < ServerDataTable.MAX_LEVEL)
		{
			MonoSingleton<PopupManager>.Instance.CloseAllPopup();
			MonoSingleton<SceneControlManager>.Instance.RemoveCurrentScene();
			MonoSingleton<SceneControlManager>.Instance.LoadScene(SceneType.Lobby);
		}
	}
}
