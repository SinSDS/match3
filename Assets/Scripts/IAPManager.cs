//@TODO ENABLE_IAP
//#define ENABLE_IAP

using System;
using UnityEngine;
#if ENABLE_IAP
using UnityEngine.Purchasing;
#endif

public class IAPManager : MonoSingleton<IAPManager>
#if ENABLE_IAP
    , IStoreListener
#endif
{
    public enum StoreProductType
	{
		Coin1,
		Coin2,
		Coin3,
		Coin4,
		Coin5,
		StarterPack,
		BoosterPackBomb,
		BoosterPackHammer,
		BoosterPackHBomb,
		BoosterPackVBomb,
		PeriodEventChristmasPack
	}

#if ENABLE_IAP
	public IStoreController m_Controller;

	private IAppleExtensions m_AppleExtensions;
#endif

    private int m_SelectedItemIndex = -1;

	private bool m_PurchaseInProgress;

#if ENABLE_IAP
	private Action<PurchaseEventArgs> actionPurchaseSuccess;

	private Action<PurchaseFailureReason> actionPurchaseFail;
#endif
	public bool IOSCanMakePayments = true;

	public bool IsStoreInitialized;

	private readonly string[] productIds =
	{
		"jewelstore_coin_1_j",
		"jewelstore_coin_2_j",
		"jewelstore_coin_3_j",
		"jewelstore_coin_4_j",
		"jewelstore_coin_5_j",
		"pack_starter_j",
		"pack_bomb_j",
		"pack_hammer_j",
		"pack_column_j",
		"pack_row_j",
		"christmas_pack_j"
	};

	private readonly string[] priceDollar =
	{
		"599",
		"1299",
		"1899",
		"2599",
		"4999",
		"299",
		"1200",
		"480",
		"660",
		"660",
		"999"
	};

	public override void Awake()
	{
		base.Awake();
		if (Application.isMobilePlatform)
		{
#if ENABLE_IAP
			StandardPurchasingModule first = StandardPurchasingModule.Instance();
			ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(first);
			IOSCanMakePayments = configurationBuilder.Configure<IAppleConfiguration>().canMakePayments;
			configurationBuilder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAseqmyyrxtEO9cpQ0s2zD4/9b/sTypYV64KDEBVL+N331YTe3x5JJeNL/w6kpt7u5W2V5rSL3AJSqBI2GkNDLL8kKPwcNU2fWhSlRThU7HuBvZBHPw0eBSHxByg67bnj8r+copO9v5kyjLxc0e8dqhhQGOpI91isOtjpX61NlVS0K4y6UvBqB5/jziB2bw41iBQHBfqo4sEiUj84OG7/DMLfKjcjIKWfKoR1vB+UgWMWuz2QPFfD9PulXgCyESUcXR2sQ7sI4bsEqr497XDFZHjkJEjQCvN3eNvo8aG5w5Fp5zSjB8J9CD4PGFrCfMEQzQ8m1BEiaUFiGFdNqsslUCwIDAQAB");
			for (int i = 0; i < productIds.Length; i++)
			{
				configurationBuilder.AddProduct(productIds[i], ProductType.Consumable);
			}
			UnityPurchasing.Initialize(this, configurationBuilder);
#endif
		}
	}

#if ENABLE_IAP
	public Product GetProduct(int storeIdIndex)
	{
		if (m_Controller == null)
		{
			return null;
		}
		return m_Controller.products.WithStoreSpecificID(productIds[storeIdIndex]);
	}
#endif

#if ENABLE_IAP
	public bool BuyProduct(string productId, Action<PurchaseEventArgs> success, Action<PurchaseFailureReason> fail)
	{
		if (m_Controller == null)
		{
			return false;
		}
		for (int i = 0; i < productIds.Length; i++)
		{
			if (productIds[i] == productId)
			{
				m_SelectedItemIndex = i;
				break;
			}
		}
		m_PurchaseInProgress = true;
		actionPurchaseSuccess = success;
		actionPurchaseFail = fail;
		m_Controller.InitiatePurchase(m_Controller.products.WithStoreSpecificID(productId));
		return true;
	}

	public bool BuyProduct(int storeIdIndex, Action<PurchaseEventArgs> success, Action<PurchaseFailureReason> fail)
	{
		if (m_Controller == null)
		{
			return false;
		}
		m_SelectedItemIndex = storeIdIndex;
		m_PurchaseInProgress = true;
		actionPurchaseSuccess = success;
		actionPurchaseFail = fail;
		m_Controller.InitiatePurchase(m_Controller.products.WithStoreSpecificID(productIds[storeIdIndex]));
		return true;
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		IsStoreInitialized = true;
		m_Controller = controller;
		m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
		Product[] all = controller.products.all;
		foreach (Product product in all)
		{
			if (product.availableToPurchase)
			{
			}
		}
		if (m_Controller.products.all.Length > 0)
		{
			m_SelectedItemIndex = 0;
		}
		for (int j = 0; j < m_Controller.products.all.Length; j++)
		{
			Product product2 = m_Controller.products.all[j];
			string text = $"{product2.metadata.localizedTitle} - {product2.metadata.localizedPriceString}";
		}
	}
#endif

    public string GetPrettyLocalizedPriceString(string localizedPriceString)
	{
		if (!string.IsNullOrEmpty(localizedPriceString) && localizedPriceString.Length > 1 && (localizedPriceString[0] < '0' || localizedPriceString[0] > '9') && localizedPriceString[1] >= '0' && localizedPriceString[1] <= '9')
		{
			return localizedPriceString[0] + " " + localizedPriceString.Substring(1);
		}
		return localizedPriceString;
	}

#if ENABLE_IAP
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		m_PurchaseInProgress = false;
		if (actionPurchaseSuccess != null)
		{
			actionPurchaseSuccess(e);
		}
		IAPPayLogger.SendIAPLog(productIds[m_SelectedItemIndex], priceDollar[m_SelectedItemIndex], PlayerDataManager.GetDeviceID(), e.purchasedProduct.receipt);
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
	{
		m_PurchaseInProgress = false;
		if (actionPurchaseFail != null)
		{
			actionPurchaseFail(r);
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		PurchaseFailureReason obj = PurchaseFailureReason.Unknown;
		switch (error)
		{
		case InitializationFailureReason.PurchasingUnavailable:
			obj = PurchaseFailureReason.PurchasingUnavailable;
			break;
		case InitializationFailureReason.NoProductsAvailable:
			obj = PurchaseFailureReason.ProductUnavailable;
			break;
		}
		if (actionPurchaseFail != null)
		{
			actionPurchaseFail(obj);
		}
	}
#endif

	public void RestorePurchases()
	{
#if ENABLE_IAP
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
		{
			m_AppleExtensions.RestoreTransactions(delegate
			{
				for (int i = 0; i < m_Controller.products.all.Length; i++)
				{
					Product product = m_Controller.products.all[i];
					string text = $"{product.metadata.localizedTitle} - {product.metadata.localizedPriceString} - {product.hasReceipt}";
				}
			});
		}
#endif
	}

	private void OnTransactionsRestored(bool success)
	{
	}

#if ENABLE_IAP
	private void OnDeferred(Product item)
	{
	}
#endif
}
