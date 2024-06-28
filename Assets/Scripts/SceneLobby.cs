using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class SceneLobby : SceneClass
{
	public RawImage ImageCookappsCrossPromotionButtonIcon;

	public ScrollRect LevelBallScrollRect;

	public GameObject BaseEpisodeListItem;

	public GameObject ObjNoAdsButton;

	public RectTransform RTLevelBallScrollPanel;

	private List<GameObject> listObj = new List<GameObject>();

	private void SetLevelBall(GameObject obj, int level)
	{
		if (!(obj == null))
		{
			try
			{
				obj.name = level.ToString();
				obj.transform.Find("TextLevel").GetComponent<Text>().text = level.ToString();
				if (level < MonoSingleton<PlayerDataManager>.Instance.CurrentLevelNo)
				{
					obj.transform.Find("Current").gameObject.SetActive(value: false);
					obj.transform.Find("Disabled").gameObject.SetActive(value: false);
				}
				else if (level == MonoSingleton<PlayerDataManager>.Instance.CurrentLevelNo)
				{
					obj.transform.Find("Current").gameObject.SetActive(value: true);
					obj.transform.Find("Disabled").gameObject.SetActive(value: false);
				}
				else
				{
					obj.transform.Find("Current").gameObject.SetActive(value: false);
					obj.transform.Find("Disabled").gameObject.SetActive(value: true);
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		if (MonoSingleton<ServerDataTable>.Instance.EnableCookappsCrossPromotion)
		{
			CM_main.instance.callAppInfo("JewelB", "all", NetCommonData.GetStoreMarket());
		}
		ObjNoAdsButton.SetActive(value: false);
		SetLevelBallScrollView(fullScreen: false);
	}

	private void SetLevelBallScrollView(bool fullScreen)
	{
		Vector2 offsetMin = RTLevelBallScrollPanel.offsetMin;
		if (fullScreen)
		{
			offsetMin.y = 0f;
		}
		else
		{
			offsetMin.y = 96f;
		}
		RTLevelBallScrollPanel.offsetMin = offsetMin;
	}

	private void OnEnable()
	{
		RefreshCrossPromotionIcon();
	}

	private void LoadLevelBallList()
	{
		RemoveLevelBall();
		GameObject gameObject = BaseEpisodeListItem;
		MonoSingleton<GameDataLoadManager>.Instance.LobbyLoadedLevelBallCount = ServerDataTable.MAX_LEVEL;
		for (int i = 0; i < ServerDataTable.MAX_LEVEL / 20; i++)
		{
			if (i > 0)
			{
				gameObject = UnityEngine.Object.Instantiate(BaseEpisodeListItem);
				gameObject.transform.SetParent(LevelBallScrollRect.content.transform, worldPositionStays: false);
			}
			listObj.Add(gameObject);
			gameObject.GetComponent<UIEpisodeItemList>().SetData(i + 1);
		}
	}

	public override IEnumerator OnSceneShow()
	{
		Application.targetFrameRate = GlobalSetting.FPS;
		MonoSingleton<UIManager>.Instance.SetCoinCurrencyMenuLayer(isPopupOverLayer: false);
		SoundManager.PlayConnection("Lobby");
		LoadLevelBallList();
		yield return null;
		listObj[(MonoSingleton<PlayerDataManager>.Instance.CurrentLevelNo - 1) / 20].GetComponent<UIEpisodeItemList>().OnPressButton(doNotTween: true);
		Canvas.ForceUpdateCanvases();
		LevelBallScrollRect.content.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, (MonoSingleton<PlayerDataManager>.Instance.CurrentLevelNo - 1) / 20 * 142);
		RefreshCrossPromotionIcon();
		MonoSingleton<PlayerDataManager>.Instance.LoadLastDailyBonusDate();
		if (AppEventCommonParameters.IsDifferentDay(MonoSingleton<PlayerDataManager>.Instance.lastRecvDailyBonusDateTime))
		{
			MonoSingleton<PopupManager>.Instance.Open(PopupType.PopupEventDailySpinReward);
		}
#if ENABLE_IAP
		else if (!PeriodEventData.CheckAndOpenPopup() && MonoSingleton<PlayerDataManager>.Instance.PayCount == 0 && MonoSingleton<PlayerDataManager>.Instance.CurrentLevelNo >= 50 && MonoSingleton<IAPManager>.Instance.GetProduct(5) != null)
		{
			MonoSingleton<PopupManager>.Instance.Open(PopupType.PopupStarterPack);
		}
#endif

    }

	public void RefreshCrossPromotionIcon()
	{
		if (MonoSingleton<ServerDataTable>.Instance.EnableCookappsCrossPromotion && CM_main.instance.isCrossPromotionSuceess && (bool)CM_main.instance.CM_SmallIcon)
		{
			if ((bool)ImageCookappsCrossPromotionButtonIcon)
			{
				ImageCookappsCrossPromotionButtonIcon.transform.parent.gameObject.SetActive(value: true);
			}
			ImageCookappsCrossPromotionButtonIcon.texture = CM_main.instance.CM_SmallIcon;
		}
		else if ((bool)ImageCookappsCrossPromotionButtonIcon)
		{
			ImageCookappsCrossPromotionButtonIcon.transform.parent.gameObject.SetActive(value: false);
		}
	}

	public override void OnSceneHideStart()
	{
	}

	private void RemoveLevelBall()
	{
		if (listObj == null)
		{
			return;
		}
		for (int i = 0; i < listObj.Count; i++)
		{
			if ((bool)listObj[i])
			{
				UnityEngine.Object.Destroy(listObj[i]);
			}
		}
		listObj.Clear();
	}

	public override void OnSceneHideEnd()
	{
		MonoSingleton<UIManager>.Instance.HideCoinCurrentMenuLayer();
	}

	public void OnPressEpisodeListItem(GameObject obj)
	{
		int result = 0;
		int.TryParse(obj.name, out result);
		if (result <= 0)
		{
		}
	}

	public void OnPressCookappsPromotion()
	{
		MonoSingleton<PopupManager>.Instance.Open(PopupType.PopupCookappsCrossPromotion);
	}

	public void OnPressNoAds()
	{
	}

	private void OnPurchaseSuccessNoAds(PurchaseEventArgs e = null)
	{
		MonoSingleton<UIManager>.Instance.HideLoading();
		ObjNoAdsButton.SetActive(value: false);
		SetLevelBallScrollView(fullScreen: true);
    
    }

	private void OnPurchaseFailedNoAds(PurchaseFailureReason reason = PurchaseFailureReason.Unknown)
	{
		MonoSingleton<UIManager>.Instance.HideLoading();
	}

	public void OnPressDailySpin()
	{
		MonoSingleton<PopupManager>.Instance.Open(PopupType.PopupEventDailySpinReward);
	}
}
