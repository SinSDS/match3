using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class PopupShopCoin : Popup, IAppEventPurchaseFunnel
{
	public enum SaleType
	{
		Normal,
		BestValue,
		MostPopular
	}

	public enum CoinShopType
	{
		Regular,
		PushOpened,
		DoubleCoin,
		StepDoubleCoin,
		CoinSaleEvent
	}

	public static bool isNewUser;

	private AppEventManager.AdCompletedStepReached adCompletedStepReached;

	private int buyingItemIndex = -1;

	private int buyingItemListIndex;

	private readonly List<int> listCoinItemIndex = new List<int>();

	public List<GameObject> listItem = new List<GameObject>();

	private AppEventManager.PurchaseReachedStep purchaseReachedStep;

	private readonly float[] viewMainNewUserPosY = new float[5]
	{
		206f,
		106f,
		6f,
		-91.3f,
		-196f
	};

	private readonly float[] viewMainPosY = new float[5]
	{
		210f,
		120f,
		30f,
		-70f,
		-193f
	};

	private PurchaseEventArgs purchaseEvent;

	public CoinShopType coinShopType;

	private AppEventManager.PurchaseTypeOfPopup purchaseTypeOfPopup;

	public Image ImageBackBlocking;

	private int stepDoubleCoinItemIndex;

	public void SendAppEventPurchaseFunnel(int beforeCoin, AppEventManager.PurchaseReachedStep step)
	{
		MonoSingleton<AppEventManager>.Instance.SendAppEventPurchaseFunnelConversionRate(purchaseTypeOfPopup, step, beforeCoin, buyingItemListIndex, GetPriceDollar(), (int)(DateTime.Now - PopupOpenDateTime).TotalSeconds);
	}

	public override void Start()
	{
		if ((bool)ImageBackBlocking)
		{
			ImageBackBlocking.gameObject.SetActive(value: true);
			ImageBackBlocking.color = new Color(0f, 0f, 0f, 0f);
			ImageBackBlocking.DOFade(0.5f, 0.5f);
		}
		base.Start();
		if (coinShopType == CoinShopType.DoubleCoin)
		{
			purchaseTypeOfPopup = AppEventManager.PurchaseTypeOfPopup.Double_Coin_Store;
		}
		purchaseReachedStep = AppEventManager.PurchaseReachedStep.PopupOpened;
		InitViewMain();
		SetViewMainNewUser();
		MonoSingleton<AppEventManager>.Instance.SendAppEventPurchaseFunnelPopupShown(purchaseTypeOfPopup);
		MonoSingleton<UIManager>.Instance.SetCoinCurrencyMenuLayer(isPopupOverLayer: true);
		if (coinShopType == CoinShopType.Regular)
		{
			SetAdCompletedStepReached(AppEventManager.AdCompletedStepReached.Popup_Opened);
		}
	}

	public void SendAppEventAdWatchingFunnel(int beforeCoin, AppEventManager.AdCompletedStepReached step)
	{
		adCompletedStepReached = step;
		MonoSingleton<AppEventManager>.Instance.SendAppEventAdWatchingFunnel(step, (int)(DateTime.Now - AppEventManager.m_TempBox.PurchaseFunnelStepElapsedTime).TotalSeconds, 0, beforeCoin, AdRewardType.Coin, 1, AppEventManager.AdAccessedBy.Coin_Store_Popup);
	}

	public void SetAdCompletedStepReached(AppEventManager.AdCompletedStepReached step)
	{
		adCompletedStepReached = step;
	}

	private void InitViewMain()
	{
		listCoinItemIndex.Clear();
		int num = 0;
		string[] array = new string[5]
		{
			"Text_X2",
			"Text_X2_02",
			"Text_X2_03",
			"Text_X2_04",
			"Text_X2_05"
		};
		foreach (PacketDataSpecCoin value in MonoSingleton<ServerDataTable>.Instance.m_dicTableCoin.Values)
		{
			if (value.view != 0)
			{
				GameObject gameObject = listItem[num];
				listCoinItemIndex.Add(value.lid);
				gameObject.transform.Find("Text_Price2").GetComponent<Text>().text = value.coin.ToString();
				Text componentInChildren = gameObject.transform.Find("Button_Buy").GetComponentInChildren<Text>();
				componentInChildren.text = "USD " + value.price;
				if (coinShopType == CoinShopType.DoubleCoin)
				{
					gameObject.transform.Find(array[num]).GetComponent<Text>().text = (value.coin * 2).ToString();
				}
				else if (value.rate == 0f)
				{
					gameObject.transform.Find("Tween").Find("GroupRateMore").gameObject.SetActive(value: false);
				}
				else
				{
					Text component = gameObject.transform.Find("Tween").Find("GroupRateMore").Find("Text_RatePercent")
						.GetComponent<Text>();
					component.text = value.rate + "%";
				}
#if ENABLE_IAP
				if (MonoSingleton<IAPManager>.Instance.m_Controller != null)
				{
					Product product = MonoSingleton<IAPManager>.Instance.GetProduct(num);
					if (product != null)
					{
						componentInChildren.text = MonoSingleton<IAPManager>.Instance.GetPrettyLocalizedPriceString(product.metadata.localizedPriceString);
					}
					if (!product.availableToPurchase)
					{
						gameObject.transform.Find("Button_Buy").GetComponent<Button>().interactable = false;
					}
				}
#endif
				switch (value.mark)
				{
				case 0:
					gameObject.transform.Find("ListBg_MostPopular").gameObject.SetActive(value: false);
					gameObject.transform.Find("ListBg_BestValue").gameObject.SetActive(value: false);
					gameObject.transform.Find("Img_MostPopular").gameObject.SetActive(value: false);
					gameObject.transform.Find("Img_BestValue").gameObject.SetActive(value: false);
					break;
				case 1:
					gameObject.transform.Find("ListBg_MostPopular").gameObject.SetActive(value: true);
					gameObject.transform.Find("ListBg_BestValue").gameObject.SetActive(value: false);
					gameObject.transform.Find("Img_MostPopular").gameObject.SetActive(value: true);
					gameObject.transform.Find("Img_BestValue").gameObject.SetActive(value: false);
					break;
				case 2:
					gameObject.transform.Find("ListBg_MostPopular").gameObject.SetActive(value: false);
					gameObject.transform.Find("ListBg_BestValue").gameObject.SetActive(value: true);
					gameObject.transform.Find("Img_MostPopular").gameObject.SetActive(value: false);
					gameObject.transform.Find("Img_BestValue").gameObject.SetActive(value: true);
					break;
				}
				if (coinShopType == CoinShopType.Regular)
				{
					GameObject gameObject2 = gameObject.transform.Find("Tween").gameObject;
					gameObject2.transform.Find("GroupRateMore").gameObject.SetActive(value: true);
					GameObject gameObject3 = gameObject2.transform.Find("GroupRateMore").gameObject;
					Text component2 = gameObject3.transform.Find("Text_RatePercent").GetComponent<Text>();
					if (value.rate == 0f)
					{
						gameObject3.SetActive(value: false);
					}
					else
					{
						component2.text = $"{value.rate}%";
					}
				}
				num++;
			}
		}
	}

	public void Update()
	{
	}

	private void SetViewMainNewUser()
	{
		if (coinShopType != 0 && coinShopType != CoinShopType.StepDoubleCoin)
		{
			return;
		}
		for (int i = 0; i < listItem.Count; i++)
		{
			if (i == 5)
			{
				listItem[i].SetActive(value: false);
				continue;
			}
			listItem[i].SetActive(value: true);
			Vector3 localPosition = listItem[i].transform.localPosition;
			listItem[i].transform.localPosition = new Vector3(localPosition.x, viewMainPosY[i], localPosition.z);
		}
	}

	public void OnPressPurchaseButton(int orderIndex)
	{
#if ENABLE_IAP
		if (orderIndex != 5 && orderIndex < listCoinItemIndex.Count && MonoSingleton<IAPManager>.Instance.BuyProduct(orderIndex, OnPurchaseSuccess, OnPurchaseFailed))
		{
			MonoSingleton<UIManager>.Instance.ShowLoading();
			buyingItemIndex = listCoinItemIndex[orderIndex];
			buyingItemListIndex = orderIndex;
			MonoSingleton<AppEventManager>.Instance.SendAppEventPurchaseFunnelButtonClicked(purchaseTypeOfPopup, buyingItemIndex, GetPriceDollar(), (int)(DateTime.Now - AppEventManager.m_TempBox.PurchaseFunnelStepElapsedTime).TotalSeconds);
		}
#endif
	}

	public void OnPurchaseSuccess(PurchaseEventArgs e = null)
	{
		purchaseEvent = e;
		MonoSingleton<UIManager>.Instance.HideLoading();
		if (coinShopType == CoinShopType.DoubleCoin)
		{
			AppEventManager.m_TempBox.coinProductType = AppEventManager.CoinPurchasedProductType.Double_Coin_Pack;
		}
		else
		{
			AppEventManager.m_TempBox.coinProductType = AppEventManager.CoinPurchasedProductType.Regular_Coin_Pack;
		}
		int coin = MonoSingleton<PlayerDataManager>.Instance.Coin;
		int num = MonoSingleton<ServerDataTable>.Instance.m_dicTableCoin[buyingItemIndex].coin;
		if (coinShopType == CoinShopType.DoubleCoin)
		{
			num += num;
		}
		MonoSingleton<PlayerDataManager>.Instance.IncreaseCoin(num);
		int gid = (MonoSingleton<SceneControlManager>.Instance.CurrentSceneType != SceneType.Game) ? (-1) : MapData.main.gid;
		MonoSingleton<AppEventManager>.Instance.SendAppEventCoinPurchased(gid, coin, num, buyingItemListIndex + 1, GetPriceDollar());
		MonoSingleton<AppEventManager>.Instance.SendAppEventPurchased(gid, coin, num, buyingItemListIndex + 1, GetPriceDollar());
		MonoSingleton<AppEventManager>.Instance.SendAppEventCoinEarned(gid, coin, num, MonoSingleton<PlayerDataManager>.Instance.Coin, AppEventManager.ItemEarnedBy.Purchased_with_Real_Money);
		if (!MonoSingleton<PlayerDataManager>.Instance.IsPayUser())
		{
			MonoSingleton<AppEventManager>.Instance.SendAppEventFirstTimePurchase(gid, coin, num, buyingItemListIndex + 1, GetPriceDollar(), buyingItemIndex);
		}
		MonoSingleton<AppEventManager>.Instance.SendAppEventPurchaseFunnelConfirmed(purchaseTypeOfPopup, coin, buyingItemIndex, GetPriceDollar(), (int)(DateTime.Now - AppEventManager.m_TempBox.PurchaseFunnelStepElapsedTime).TotalSeconds);
		MonoSingleton<PlayerDataManager>.Instance.PayCount++;
		if (MonoSingleton<PlayerDataManager>.Instance.PayCount == 1 && coinShopType == CoinShopType.Regular)
		{
			MonoSingleton<PlayerDataManager>.Instance.EnabledDoubleShopCoin = true;
			MonoSingleton<UIManager>.Instance.ShowDoubleShopCoinAlarm(show: true);
		}
		if (coinShopType == CoinShopType.DoubleCoin)
		{
			MonoSingleton<PlayerDataManager>.Instance.EnabledDoubleShopCoin = false;
			MonoSingleton<UIManager>.Instance.ShowDoubleShopCoinAlarm(show: false);
		}
		purchaseReachedStep = AppEventManager.PurchaseReachedStep.Purchased;
		SendAppEventPurchaseFunnel(coin, AppEventManager.PurchaseReachedStep.Purchased);
		UIManager.holdOnUpdateCoin = true;
		if (MonoSingleton<PlayerDataManager>.Instance.EnabledDoubleShopCoin)
		{
			MonoSingleton<UIManager>.Instance.ShowGetCoinEffect(listItem[buyingItemListIndex].transform, new Vector2(-165f, 0f), OpenDoubleShopCoinAfterPurchased, num);
		}
		else
		{
			MonoSingleton<UIManager>.Instance.ShowGetCoinEffect(listItem[buyingItemListIndex].transform, new Vector2(-165f, 0f), null, num);
		}
		MonoSingleton<PlayerDataManager>.Instance.SavePayInfo();
	}

	public void OnPurchaseFailed(PurchaseFailureReason reason = PurchaseFailureReason.Unknown)
	{
		MonoSingleton<UIManager>.Instance.HideLoading();
		if (reason != PurchaseFailureReason.UserCancelled)
		{
			MonoSingleton<PopupManager>.Instance.Open(PopupType.PopupInAppPurchaseFailed);
			return;
		}
		MonoSingleton<AppEventManager>.Instance.SendAppEventPurchaseFunnelCanceled(purchaseTypeOfPopup, buyingItemIndex, GetPriceDollar(), (int)(DateTime.Now - AppEventManager.m_TempBox.PurchaseFunnelStepElapsedTime).TotalSeconds);
		purchaseReachedStep = AppEventManager.PurchaseReachedStep.ClickedPurchaseButton;
	}

	private void OpenDoubleShopCoinAfterPurchased()
	{
		OnEventClose();
		AppEventManager.m_TempBox.adAccessedBy = AppEventManager.AdAccessedBy.Coin_Store_Automatic_Popup;
		MonoSingleton<PopupManager>.Instance.Open(PopupType.PopupCoinDoubleShop);
	}

	private float GetPriceDollar()
	{
		float result = 0f;
		if (MonoSingleton<ServerDataTable>.Instance.m_dicTableCoin.ContainsKey(buyingItemIndex))
		{
			result = MonoSingleton<ServerDataTable>.Instance.m_dicTableCoin[buyingItemIndex].price;
		}
		return result;
	}

	public override void OnEventClose()
	{
		base.OnEventClose();
		UIManager.holdOnUpdateCoin = false;
		MonoSingleton<UIManager>.Instance.SetCoinCurrencyMenuLayer(isPopupOverLayer: false);
		if (MonoSingleton<PopupManager>.Instance.CurrentPopupType == PopupType.PopupInGameItemStore || (MonoSingleton<SceneControlManager>.Instance.CurrentSceneType == SceneType.Game && (MonoSingleton<PopupManager>.Instance.CurrentPopupType == PopupType.PopupGameOver || MonoSingleton<PopupManager>.Instance.CurrentPopupType == PopupType.PopupGameOverCollectInfo)))
		{
			MonoSingleton<UIManager>.Instance.SetCoinCurrencyMenuLayer(isPopupOverLayer: true);
		}
		else if (MonoSingleton<SceneControlManager>.Instance.CurrentSceneType == SceneType.Game)
		{
			MonoSingleton<UIManager>.Instance.HideCoinCurrentMenuLayer();
		}
		if (purchaseReachedStep != AppEventManager.PurchaseReachedStep.Purchased)
		{
			SendAppEventPurchaseFunnel(MonoSingleton<PlayerDataManager>.Instance.Coin, purchaseReachedStep);
		}
		PeriodEventData.CheckAndOpenPopup();
	}

	private void OnDestroy()
	{
	}
}
