using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
	public delegate void EventCancelBooster();

	public UIModalLoading modalLoading;

	public Camera CoinCurrencyCamera;

	private Button ButtonAddCoin;

	[HideInInspector]
	public Sprite spriteBGMaskPortrait;

	public bool IsModalLoading;

	public Sprite[] SpriteRewardItemType;

	public Sprite[] SpriteAdRewardItemType;

	[Header("Localize Font")]
	public Font FontDefault;

	public GameObject ObjDoubleShopCoinAlarm;

	private bool IsAppPause;

	public static string AdvertisingID = string.Empty;

	private bool IsInit;

	private readonly Dictionary<int, int> localPushDic = new Dictionary<int, int>();

	[HideInInspector]
	public bool IsSoonUpdatedApp;

	public GameObject PrefabGetCoinEffect1;

	public GameObject PrefabGetCoinEffect2;

	public GameObject PrefabGetCoinEffect3;

	[HideInInspector]
	public MarketVersionChecker VersionChecker;

	public static bool holdOnUpdateCoin;

	private DateTime appPauseLastTime = DateTime.Now;

	private EventSystem currentEventSystem;

	public Sprite sprLock;

	private bool isWaitingToUpdateForEffect;

	public event EventCancelBooster eventCancelBooster;

	public override void Awake()
	{
		base.Awake();
#if APPSFLYER_SDK
        AppsFlyer.setAppsFlyerKey("rpPWfG9Nbc8v2Rk7fYm783");
		AppsFlyer.setAppID("com.cookapps.playgrounds.ff.jewelblast");
		AppsFlyer.init("rpPWfG9Nbc8v2Rk7fYm783", "AppsFlyerTrackerCallbacks");
#endif
		Application.RequestAdvertisingIdentifierAsync(delegate(string advertisingId, bool trackingEnabled, string error)
		{
			AdvertisingID = advertisingId;
		});
	}

	private IEnumerator Start()
	{
		IsInit = true;
		ShowDoubleShopCoinAlarm(MonoSingleton<PlayerDataManager>.Instance.EnabledDoubleShopCoin);
		yield return new WaitForSeconds(1f);
		LocalPush.Init_Notification();
		LocalPush.DeleteAllNotification();
		UpdateStoreVersionCheck();
	}

	private void OnApplicationPause(bool pause)
	{
		if (!IsInit)
		{
			return;
		}
		if (pause)
		{
			AppEventManager.m_TempBox.remotePushAs = AppEventManager.SessionStartedAs.Resumed;
			appPauseLastTime = DateTime.UtcNow;
			IsAppPause = true;
			RegisterLocalNotification();
		}
		else if (IsAppPause)
		{
			PlayerDataManager.SetSessionStartDateTimeString();
			if ((DateTime.UtcNow - appPauseLastTime).TotalMinutes >= (double)MonoSingleton<ServerDataTable>.Instance.SessionTimeMinute)
			{
				MonoSingleton<PlayerDataManager>.Instance.IsFirstSession = false;
				MonoSingleton<AppEventManager>.Instance.SendAppEventPlayerSessionStart((int)(DateTime.UtcNow - appPauseLastTime).TotalHours, AppEventManager.SessionStartedAs.Resumed);
				MonoSingleton<AppEventManager>.Instance.SendAppEventPlayerADSource();
				PlayerDataManager.SetSessionEndDateTimeString();
			}
			IsAppPause = false;
			NetRequestResume.Request();
			LocalPush.DeleteAllNotification();
		}
	}

	public void ShowDoubleShopCoinAlarm(bool show)
	{
		if ((bool)ObjDoubleShopCoinAlarm)
		{
			ObjDoubleShopCoinAlarm.SetActive(show);
		}
	}

	private void OnApplicationFocus(bool isFocus)
	{
	}

	private void OnApplicationQuit()
	{
		if (!IsAppPause)
		{
			RegisterLocalNotification();
		}
		PlayerDataManager.SetSessionEndDateTimeString();
		PlayerPrefs.Save();
	}

	private static int getSDKInt()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build$VERSION"))
		{
			return androidJavaClass.GetStatic<int>("SDK_INT");
		}
	}

	private string GetLocalPushDesc(int index)
	{
		bool flag = false;
		if (Application.platform == RuntimePlatform.Android)
		{
			flag = true;
		}
		string result = string.Empty;
		if (!flag)
		{
			switch (index)
			{
			case 1:
				result = "\ud83c\udf6c \ud83c\udf6c Need some sweets? \ud83d\ude03 It`s Sweet Road time! \ud83d\udc93";
				break;
			case 2:
				result = "\ud83c\udf6d \ud83d\ude0d \ud83c\udf6d Delicious candies await you\nHead into the game now";
				break;
			case 3:
				result = "The Sweetest \ud83c\udf6c Match-3 Game Ever! PLAY NOW \ud83d\ude03";
				break;
			case 4:
				result = "Ready to CRUSH \ud83d\udc4a candies? PLAY NOW\ud83c\udf6c \ud83d\ude0d \ud83c\udf6c";
				break;
			case 6:
				result = "❤\ufe0f LIFE restored! You will win this time \ud83d\udc4d";
				break;
			case 7:
				result = "❤\ufe0f Your Friend just sent you a LIFE! ❤\ufe0f Come back & PLAY~!";
				break;
			case 8:
				result = "Your LIFE is FULL \ud83d\udc4d now! ❤\ufe0f ❤\ufe0f ❤\ufe0f ❤\ufe0f ❤\ufe0f";
				break;
			case 13:
				result = "⏰ 1 Hour Left! Road to Candy House is closing.";
				break;
			case 14:
				result = "⏰ 6 Hours, FINAL Chance to get to Candy House!";
				break;
			}
			if (index >= 100 && index < 200)
			{
				result = "⏰ Sweet Time Again! ⏰ Come back to Sweet Road and have some fun \ud83d\ude03";
			}
			else if (index >= 200 && index < 300)
			{
				result = "LUNCH BREAK! \ud83c\udf74 PLAY Yummy & Sweet game NOW \ud83c\udfae";
			}
			else if (index >= 300 && index < 400)
			{
				result = "Happy meal \ud83c\udf74with the Family & PLAY Sweet Road!\ud83c\udfae";
			}
			else if (index >= 400 && index < 500)
			{
				result = "Time to relax ☕\ufe0f with sweet game.PLAY NOW!";
			}
			else if ((index >= 500 && index < 600) || index == 12)
			{
				result = "Check your today's fortune!";
			}
		}
		else
		{
			switch (index)
			{
			case 1:
				result = "Need some sweets? :D It`s Sweet Road time";
				break;
			case 2:
				result = "Delicious candies await you. Head into the game now :)";
				break;
			case 3:
				result = "The sweetest match-3 game ever! PLAY NOW :D";
				break;
			case 4:
				result = "Ready to crush candies? :v PLAY NOW";
				break;
			case 6:
				result = "Your life just restored! You will win this time";
				break;
			case 7:
				result = "Your friend just sent you a life! Come back and PLAY!";
				break;
			case 8:
				result = "Now you have FULL LIFE again!";
				break;
			case 13:
				result = "1 Hour Left! Road to Candy House is closing.";
				break;
			case 14:
				result = "6 Hours, FINAL Chance to get to Candy House!";
				break;
			}
			if (index >= 100 && index < 200)
			{
				result = "The sweetest time. Spend a little time and have some fun ;)";
			}
			else if (index >= 200 && index < 300)
			{
				result = "Have lunch with SWEETEST puzzle game! Play Now!";
			}
			else if (index >= 300 && index < 400)
			{
				result = "Ready to crush some candies? :v PLAY NOW!";
			}
			else if (index >= 400 && index < 500)
			{
				result = "Time to relax with sweet game. PLAY NOW!";
			}
			else if ((index >= 500 && index < 600) || index == 12)
			{
				result = "Check your today's fortune!";
			}
		}
		return result;
	}

	public void RegisterLocalNotification()
	{
		LocalPush.DeleteAllNotification();
	}

	public void Update()
	{
		if (Application.platform != RuntimePlatform.Android || !Input.GetKeyUp(KeyCode.Escape) || IsModalLoading)
		{
			return;
		}
		if (MonoSingleton<PopupManager>.Instance.CurrentPopupType != PopupType.PopupNone && MonoSingleton<PopupManager>.Instance.CurrentPopup != null)
		{
			if (!MonoSingleton<PopupManager>.Instance.CurrentPopup.DisableBackKey)
			{
				MonoSingleton<PopupManager>.Instance.CurrentPopup.OnEventClose();
			}
		}
		else if (!MonoSingleton<SceneControlManager>.Instance.IsLoadingScene)
		{
			if (MonoSingleton<SceneControlManager>.Instance.CurrentSceneType == SceneType.Lobby)
			{
				Application.Quit();
			}
			else if (MonoSingleton<SceneControlManager>.Instance.CurrentSceneType == SceneType.Game && GameMain.main != null && !GameMain.main.isBonusTime && !GameMain.main.outOfGame)
			{
				GameMain.main.OnPressButtonExit();
			}
		}
	}

	public void FixedUpdate()
	{
	}

	private void UpdateStoreVersionCheck()
	{
		GameObject gameObject = new GameObject();
		VersionChecker = gameObject.AddComponent<MarketVersionChecker>();
		VersionChecker.Initialize();
	}

    // @TODO: Check at here
	public void ShowLoading()
	{
		//if (!IsModalLoading)
		//{
		//	IsModalLoading = true;
		//	modalLoading.FadeIn();
		//}
	}

	public void HideLoading()
	{
		//if (IsModalLoading)
		//{
		//	IsModalLoading = false;
		//	modalLoading.FadeOut();
		//}
	}

	public void SetCoinCurrencyMenuLayer(bool isPopupOverLayer)
	{
		if (MonoSingleton<SceneControlManager>.Instance.OldSceneType != SceneType.MapTool)
		{
			if (!CoinCurrencyCamera.gameObject.transform.parent.gameObject.activeSelf)
			{
				CoinCurrencyCamera.gameObject.transform.parent.gameObject.SetActive(value: true);
			}
			if (isPopupOverLayer)
			{
				CoinCurrencyCamera.depth = 13f;
			}
			else
			{
				CoinCurrencyCamera.depth = 1f;
			}
		}
	}

	public void HideCoinCurrentMenuLayer()
	{
		CoinCurrencyCamera.transform.parent.gameObject.SetActive(value: false);
	}

	public void OnPopupShopCoin(Button addButton)
	{
		SoundSFX.Play(SFXIndex.ButtonClick);
		if (MonoSingleton<IRVManager>.Instance.CurrentNetStatus != InternetReachabilityVerifier.Status.NetVerified
            //|| !MonoSingleton<IAPManager>.Instance.IsStoreInitialized
            )
		{
			MonoSingleton<PopupManager>.Instance.Open(PopupType.PopupConnectionLost);
		}
		else if (MonoSingleton<PopupManager>.Instance.CurrentPopupType != PopupType.PopupShopCoin && MonoSingleton<PopupManager>.Instance.CurrentPopupType != PopupType.PopupCoinDoubleShop && MonoSingleton<PopupManager>.Instance.CurrentPopupType != PopupType.PopupCoinShop2)
		{
			AppEventManager.m_TempBox.coinCategory = AppEventManager.CoinCategory.Null;
			if (MonoSingleton<PlayerDataManager>.Instance.EnabledDoubleShopCoin)
			{
				AppEventManager.m_TempBox.adAccessedBy = AppEventManager.AdAccessedBy.Coin_Store_Popup;
				MonoSingleton<PopupManager>.Instance.Open(PopupType.PopupCoinDoubleShop, enableBackCloseButton: true, OffPopupShopCoin);
			}
			else
			{
				AppEventManager.m_TempBox.adAccessedBy = AppEventManager.AdAccessedBy.Coin_Store_Popup;
				MonoSingleton<PopupManager>.Instance.Open(PopupType.PopupShopCoin, enableBackCloseButton: true, OffPopupShopCoin);
			}
			ButtonAddCoin = addButton;
			ButtonAddCoin.interactable = false;
		}
	}

	public void OffPopupShopCoin()
	{
		if ((bool)ButtonAddCoin)
		{
			ButtonAddCoin.interactable = true;
		}
	}

	public void DisableEventSystem()
	{
		if ((bool)EventSystem.current)
		{
			currentEventSystem = EventSystem.current;
			EventSystem.current.enabled = false;
		}
	}

	public void EnableEventSystem()
	{
		if ((bool)currentEventSystem)
		{
			currentEventSystem.enabled = true;
		}
	}

	public void CancelBooster()
	{
		if (this.eventCancelBooster != null)
		{
			this.eventCancelBooster();
		}
	}

	public Font GetLocalizeFont()
	{
		return FontDefault;
	}

	public Sprite GetLockSprite()
	{
		return sprLock;
	}

	public Sprite GetServerRewardItemTypeSprite(ServerItemIndex itemIndex)
	{
		try
		{
			return SpriteRewardItemType[(int)(itemIndex - 1)];
		}
		catch (Exception)
		{
		}
		return null;
	}

	public static bool IsNotchScreen()
	{
		return false;
	}

	public void ShowGetCoinEffect(Transform parentTransform, Vector2 offset, Action callBack, int getCoin)
	{
		if (getCoin == 0)
		{
			callBack?.Invoke();
		}
		else
		{
			StartCoroutine(waitShowGetCoinEffect(parentTransform, offset, callBack, getCoin));
		}
	}

	private IEnumerator waitShowGetCoinEffect(Transform parentTransform, Vector2 offset, Action callBack, int getCoin)
	{
		EventSystem currentEventSystem = EventSystem.current;
		if ((bool)currentEventSystem)
		{
			currentEventSystem.enabled = false;
		}
		SetCoinCurrencyMenuLayer(isPopupOverLayer: true);
		isWaitingToUpdateForEffect = true;
		int numOfCoinEffect = (getCoin / 10 <= 0) ? 1 : Mathf.Min(getCoin / 10, 10);
		GameObject objCoinLightEffect = UnityEngine.Object.Instantiate(PrefabGetCoinEffect1);
		objCoinLightEffect.transform.SetParent(parentTransform, worldPositionStays: false);
		objCoinLightEffect.transform.localPosition = offset;
		yield return new WaitForSeconds(0.817f);
		for (int i = 0; i < numOfCoinEffect; i++)
		{
			StartCoroutine(waitShowGetCoinEffect2(parentTransform, offset, 1f));
			yield return new WaitForSeconds(0.2f);
		}
		UnityEngine.Object.Destroy(objCoinLightEffect);
		yield return new WaitForSeconds(1f);
		if ((bool)currentEventSystem)
		{
			currentEventSystem.enabled = true;
		}
		SetCoinCurrencyMenuLayer(isPopupOverLayer: false);
		isWaitingToUpdateForEffect = false;
		callBack?.Invoke();
	}

	private IEnumerator waitShowGetCoinEffect2(Transform parentTransform, Vector2 offset, float duration)
	{
		Button coinButton = CoinCurrencyCamera.transform.parent.GetComponentInChildren<Button>();
		GameObject objCoinEffect = UnityEngine.Object.Instantiate(PrefabGetCoinEffect2);
		objCoinEffect.transform.SetParent(parentTransform, worldPositionStays: false);
		objCoinEffect.transform.localPosition = offset;
		Vector2 coinPosition = MonoSingleton<PopupManager>.Instance.PopupCamera.WorldToViewportPoint(objCoinEffect.transform.position);
		objCoinEffect.transform.SetParent(coinButton.transform, worldPositionStays: false);
		objCoinEffect.transform.position = CoinCurrencyCamera.ViewportToWorldPoint(coinPosition);
		SoundSFX.Play(SFXIndex.GetCoinStart);
		float elapse_time = 0f;
		Vector3 startPos = objCoinEffect.transform.localPosition;
		Vector3 targetPos = new Vector3(-108f, 4f, 0f);
		while (elapse_time < 1f)
		{
			Vector3 pointPos = new Vector3(-108f, startPos.y - 50f, 0f);
			objCoinEffect.transform.localPosition = Utils.Bezier(elapse_time, startPos, pointPos, targetPos);
			elapse_time += Time.deltaTime;
			yield return null;
		}
		UnityEngine.Object.Destroy(objCoinEffect);
		SoundSFX.Play(SFXIndex.GetCoinEnd);
		GameObject objCoinEndEffect = UnityEngine.Object.Instantiate(PrefabGetCoinEffect3);
		objCoinEndEffect.transform.SetParent(coinButton.transform, worldPositionStays: false);
		objCoinEndEffect.transform.localPosition = new Vector3(-108f, 4f, -2f);
		holdOnUpdateCoin = false;
		yield return new WaitForSeconds(0.5f);
		UnityEngine.Object.Destroy(objCoinEndEffect);
	}
}
