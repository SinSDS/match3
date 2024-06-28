using System.Collections;
using UnityEngine;

public class LobbyManager : MonoSingleton<LobbyManager>
{
	private bool isFirstLoad = true;

	private bool isInit;

	public static bool afterFailed;

	public override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		if (!PlayerPrefs.HasKey("FirstInstalledVersion"))
		{
			PlayerPrefs.SetString("FirstInstalledVersion", GlobalSetting.ConfigData.AppVersion);
		}
	}

	private void OnDisable()
	{
	}

	private void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine(OnEnableCoroutine());
	}

	public void OnEventRecvLevelData()
	{
	}

	private void OnDestroy()
	{
	}

	private IEnumerator OnEnableCoroutine()
	{
		isInit = true;
		isFirstLoad = false;
		yield return null;
	}

	public void OnPressStartLevel()
	{
		SoundSFX.Play(SFXIndex.ButtonClick);
		MonoSingleton<PlayerDataManager>.Instance.lastPlayedLevel = MonoSingleton<PlayerDataManager>.Instance.CurrentLevelNo;
		MapData.main = new MapData(MonoSingleton<PlayerDataManager>.Instance.CurrentLevelNo);
		MonoSingleton<SceneControlManager>.Instance.LoadScene(SceneType.Game, SceneChangeEffect.Color);
		GameMain.CompleteGameStart();
	}

	private void Update()
	{
		if (isInit)
		{
		}
	}

	private IEnumerator processStageClearEffect()
	{
		yield return null;
		yield return null;
	}
}
