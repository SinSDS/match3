using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : Popup
{
	public GameObject buttonHome;

	public GameObject buttonQuit;

	public Text DebugBuildVersion;

	public GameObject ObjGroupOption;

	public Toggle ToggleSoundBGMButton;

	public Toggle ToggleSoundEffectButton;

	public Text deviceID;

	public override void Start()
	{
		base.Start();
	}

	public void SetPopup(UIOptionButton.OptionMenuType type)
	{
		deviceID.text = PlayerDataManager.GetDeviceID();
		if ((bool)DebugBuildVersion)
		{
			DebugBuildVersion.transform.parent.gameObject.SetActive(value: true);
			DebugBuildVersion.text = "Ver : " + GlobalSetting.ConfigData.AppVersion + "-" + BuildNumber.GetJenkinsBuildVersion() + "." + BuildNumber.GetGitLashHash();
		}
		if (type == UIOptionButton.OptionMenuType.Lobby)
		{
			buttonHome.SetActive(value: true);
			buttonQuit.SetActive(value: false);
			ObjGroupOption.transform.localPosition = Vector3.zero;
		}
		else
		{
			buttonHome.SetActive(value: false);
			buttonQuit.SetActive(value: true);
			ObjGroupOption.transform.localPosition = Vector3.zero;
		}
	}

	public void Update()
	{
	}

	public override void OnEnable()
	{
		base.OnEnable();
		ToggleSoundBGMButton.isOn = !MonoSingleton<PlayerDataManager>.Instance.IsOnSoundBGM;
		ToggleSoundEffectButton.isOn = !MonoSingleton<PlayerDataManager>.Instance.IsOnSoundEffect;
	}

	public void OnToggleSoundBGMButton(bool changed)
	{
		SoundSFX.Play(SFXIndex.ButtonClick);
		MonoSingleton<PlayerDataManager>.Instance.IsOnSoundBGM = !changed;
		if (MonoSingleton<PlayerDataManager>.Instance.IsOnSoundBGM)
		{
			SoundManager.SetVolumeMusic(1f);
		}
		else
		{
			SoundManager.SetVolumeMusic(0f);
		}
		MonoSingleton<PlayerDataManager>.Instance.SaveOptionSound();
	}

	public void OnToggleSoundEffectButton(bool changed)
	{
		MonoSingleton<PlayerDataManager>.Instance.IsOnSoundEffect = !changed;
		SoundManager.Instance.offTheSFX = changed;
		if (!MonoSingleton<PlayerDataManager>.Instance.IsOnSoundEffect)
		{
			SoundManager.StopSFX();
		}
		MonoSingleton<PlayerDataManager>.Instance.SaveOptionSound();
		SoundSFX.Play(SFXIndex.ButtonClick);
	}

	public void OnPressButtonFacebook()
	{
		SoundSFX.Play(SFXIndex.ButtonClick);
		if (MonoSingleton<FacebookManager>.Instance.IsLogin)
		{
			MonoSingleton<FacebookManager>.Instance.CallFBLogout();
			OnEventClose();
		}
		else if (MonoSingleton<IRVManager>.Instance.CurrentNetStatus == InternetReachabilityVerifier.Status.Offline)
		{
			MonoSingleton<PopupManager>.Instance.Open(PopupType.PopupConnectionLost);
		}
		else
		{
			MonoSingleton<UIManager>.Instance.ShowLoading();
			MonoSingleton<FacebookManager>.Instance.CallFBLogin(AppEventManager.FacebookLoginFromWhere.Option_Popup);
		}
	}

	public void OnPressTitle()
	{
		SoundSFX.Play(SFXIndex.ButtonClick);
		Application.Quit();
	}

	public void OnPressGameQuit()
	{
		SoundSFX.Play(SFXIndex.ButtonClick);
		if (!GameMain.main.isFirstBoardSetting && MonoSingleton<SceneControlManager>.Instance.CurrentSceneType == SceneType.Game)
		{
			GameMain.main.OnPressButtonExit();
		}
	}

	public void OnPressGPGSRanking()
	{
	}

	public void OnPressGPGSAchievement()
	{
	}

	public void OnPressGPGSLoginOrLogout()
	{
	}

	public void OnPressCopyToClipboard()
	{
		string textToClipboard = $"DeviceID : {PlayerDataManager.GetDeviceID()}";
        //@TODO NATIVE
        //AndroidNativeFunctions.SetTextToClipboard(textToClipboard);
		//AndroidNativeFunctions.ShowToast("Copied to clipboard.");
	}

	public void OnPressHelpShiftFAQ()
	{
		Application.OpenURL(MonoSingleton<ServerDataTable>.Instance.faqCsUrl);
		if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
		{
		}
	}
}
