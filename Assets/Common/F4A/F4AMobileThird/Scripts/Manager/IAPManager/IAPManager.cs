using System;
using System.Collections;
using System.Linq;
using UnityEngine;

#if DEFINE_IAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

using Newtonsoft.Json;

namespace com.F4A.MobileThird
{
	using Newtonsoft.Json.Linq;
    using System.IO;

    public enum InAppProductType
	{
		Consumable = 0,
		NonConsumable = 1,
		Subscription = 2
	}

	/// <summary>
	/// 
	/// </summary>
	public enum EIapPopular
	{
		None,
		Hot,
		Best,
		Popular,
		Sale,
        New,
    }

    /// <summary>
	/// 
	/// </summary>
	public enum ETagProduct
    {
        None,
        Hot,
        Best,
        Popular,
        Sale,
        New,
    }

    public enum EIapIcon
	{
		None = -1,
		VerySmall,
		Small,
		MediumSmall,
		MediumLarge,
		Large,
		VeryLarge
	}

	public enum EIapNonConsumableType
	{
		None,
		RemoveAds,
	}
	/// <summary>
	/// 
	/// </summary>
	[System.Serializable]
	public class IAPProductInfo
	{
		public string Id = "";
		public string Name = "";
		public string Description = "";

		[Space(10)]
		[Header("Price of product ($)")]
		public float Price = 0.99f;
		public int Coin = 25;
		public int Discount = 0;
		public EIapIcon typeCoinIcon = EIapIcon.None;
		public ETagProduct InAppTag = ETagProduct.None;
        public InAppProductType Type = InAppProductType.Consumable;
		public EIapNonConsumableType nonConsumableType = EIapNonConsumableType.None;

		public bool IsTypeRemoveAds()
		{
			return Type == InAppProductType.NonConsumable && nonConsumableType == EIapNonConsumableType.RemoveAds;
		}

		public bool IsConsumable()
		{
			return Type == InAppProductType.Consumable;
		}

		public bool IsNonConsumable()
		{
			return Type == InAppProductType.NonConsumable;
		}

		public bool IsSubscription()
		{
			return Type == InAppProductType.Subscription;
		}

        public bool HasTag()
        {
            return InAppTag != ETagProduct.None;
        }
    }

	public enum EStatusBuyIAP
	{
		None,
		NotInitializeIAP,
		NotContainInStore,
		InProcess,
		Success,
	}

    [System.Serializable]
    public class IAPSettingInfo
    {
        public bool enableAndroid = false;
        public bool enableIos = false;

        public bool EnableUnityIAP()
        {
#if UNITY_ANDROID
            return enableAndroid;
#elif UNITY_IOS
            return enableIos;
#else
            return false;
#endif
        }

        // Product identifiers for all products capable of being purchased:
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        public IAPProductInfo[] AndroidProductInfos;
        public IAPProductInfo[] IOSProductInfos;
    }

	[AddComponentMenu("F4A/IAPManager")]
	public class IAPManager : SingletonMono<IAPManager>
#if DEFINE_IAP
	, IStoreListener
#endif
	{
#region Delegates, Consts
	    
		/// <summary>
		/// Occurs when on buy purchase successes.
		/// </summary>
		/// string id
		public static event Action<string> OnBuyPurchaseSuccessed = delegate { };
		/// <summary>
		/// 
		/// </summary>
		/// string id, string failureReason
		public static event Action<string, string> OnBuyPurchaseFailed = delegate { };
		//public static Action<IAPProductInfo> Action_Consumable_IAP;
		//      public static Action<IAPProductInfo> Action_NonConsumable_IAP;
		//      public static Action<IAPProductInfo> Action_Subscription_IAP;
		public static event Action<bool> OnRestorePurchases = delegate { };
        #endregion

        [SerializeField]
        private bool isOnline = true;
        [SerializeField]
        private string urlConfigInApp = "";
        [SerializeField]
        private TextAsset textConfigDefault = null;

        public IAPSettingInfo iapSettingInfo = new IAPSettingInfo();

#if DEFINE_IAP
        private IStoreController m_StoreController;
		public IStoreController StoreController
		{
			get { return m_StoreController; }
		}
		// The Unity Purchasing system.
		private IExtensionProvider m_StoreExtensionProvider;
        private CrossPlatformValidator validator;
        // The store-specific Purchasing subsystems.

        //		private IAppleExtensions m_AppleExtensions;
        //		private IMoolahExtension m_MoolahExtensions;
        //		private ISamsungAppsExtensions m_SamsungExtensions;
        //		private IMicrosoftExtensions m_MicrosoftExtensions;
        //		private IUnityChannelExtensions m_UnityChannelExtensions;
#endif

        private void Start()
		{
            F4ACoreManager.OnDownloadF4AConfigCompleted += F4ACoreManager_OnDownloadF4AConfigCompleted;
        }

        private void OnDestroy()
        {
            F4ACoreManager.OnDownloadF4AConfigCompleted -= F4ACoreManager_OnDownloadF4AConfigCompleted;
        }

        private void F4ACoreManager_OnDownloadF4AConfigCompleted(F4AConfigData configData, bool success)
        {

            if (configData != null && !string.IsNullOrEmpty(configData.urlInAppPurchase))
            {
                urlConfigInApp = configData.urlInAppPurchase;
            }

            if (isOnline)
            {
                StartCoroutine(DMCMobileUtils.AsyncGetDataFromUrl(urlConfigInApp, textConfigDefault, (string data) =>
                {
                    if (!string.IsNullOrEmpty(data))
                    {
                        iapSettingInfo = JsonConvert.DeserializeObject<IAPSettingInfo>(data);
                    }
                    InitializePurchasing();
                }));
            }
            else
            {
                InitializePurchasing();
            }
        }

		public void InitializePurchasing()
		{
#if DEFINE_IAP
			// If we haven't set up the Unity Purchasing reference
			if (m_StoreController != null) {
				return;
			}

			// If we have already connected to Purchasing ...
			if (IsInitialized ()) {
				// ... we are done here.
				return;
			}

			// Create a builder, first passing in a suite of Unity provided stores.
			var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());

#if UNITY_ANDROID
			foreach (var product in iapSettingInfo.AndroidProductInfos) {
				if (product.Type == InAppProductType.Consumable) {
					builder.AddProduct (product.Id, ProductType.Consumable);
				} else if (product.Type == InAppProductType.NonConsumable) {
					builder.AddProduct (product.Id, ProductType.NonConsumable);
				} else if (product.Type == InAppProductType.Subscription) {
					builder.AddProduct (product.Id, ProductType.Subscription, new IDs ()
					{ { product.Name, GooglePlay.Name } });
				}
			}

#elif UNITY_IOS
			foreach (var product in iapSettingInfo.IOSProductInfos) {
			if (product.Type == InAppProductType.Consumable) {
			builder.AddProduct (product.Id, ProductType.Consumable);
			} else if (product.Type == InAppProductType.NonConsumable) {
			builder.AddProduct (product.Id, ProductType.NonConsumable);
			} else if (product.Type == InAppProductType.Subscription) {
			builder.AddProduct (product.Id, ProductType.Subscription, new IDs ()
			{ { product.Name, AppleAppStore.Name } });
			}
			}
#else
#endif

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize (this, builder);
#endif
		}


		private bool IsInitialized()
		{
#if DEFINE_IAP
			// Only say we are initialized if both the Purchasing references are set.
			return m_StoreController != null && m_StoreExtensionProvider != null;
#else
			return false;
#endif
		}

        public bool EnableUnityIAP()
        {
#if DEFINE_IAP
            return iapSettingInfo != null && iapSettingInfo.EnableUnityIAP();
#endif
            return false;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="productName"></param>
		/// <returns></returns>
		public EStatusBuyIAP BuyProductByName(string productName)
		{
#if DEFINE_IAP
			IAPProductInfo[] products = null;
#if UNITY_ANDROID
			products = iapSettingInfo.AndroidProductInfos;
#elif UNITY_IOS
			products = iapSettingInfo.IOSProductInfos;
#endif
            if (products != null)
			{
				var product = products.Where(p => p.Name.Equals(productName)).FirstOrDefault();
				if (product != null)
				{
					return BuyProductByID(product.Id);
				}
			}
			return EStatusBuyIAP.None;
#else
			return EStatusBuyIAP.None;
#endif

		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="productId"></param>
		/// <returns></returns>
		public EStatusBuyIAP BuyProductByID(string productId)
		{
#if DEFINE_IAP
			// If Purchasing has been initialized ...
			if (IsInitialized ()) {
				// ... look up the Product reference with the general product identifier and the Purchasing 
				// system's products collection.
				Product product = m_StoreController.products.WithID (productId);

				// If the look up found a product for this device's store and that product is ready to be sold ... 
				if (product != null && product.availableToPurchase) {
					Debug.Log (string.Format ("Purchasing product asychronously: '{0}'", product.definition.id));
					// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
					// asynchronously.
					m_StoreController.InitiatePurchase (product);
					return EStatusBuyIAP.InProcess;
				}
				// Otherwise ...
				else {
					// ... report the product look-up failure situation  
					Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
					return EStatusBuyIAP.NotContainInStore;
				}
			}
			// Otherwise ...
			else {
				// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
				// retrying initialization.
				Debug.Log ("BuyProductID FAIL. Not initialized.");
				return EStatusBuyIAP.NotInitializeIAP;
			}
#else
			return EStatusBuyIAP.None;
#endif
		}

		public EStatusBuyIAP BuyProductRemoveAds()
		{
#if DEFINE_IAP
			IAPProductInfo[] products = null;
#if UNITY_ANDROID
			products = iapSettingInfo.AndroidProductInfos;
#elif UNITY_IPHONE
			products = iapSettingInfo.IOSProductInfos;
#else
#endif
            IAPProductInfo product = null;
			if (products != null)
			{
				product = products.Where(p => p.IsTypeRemoveAds()).FirstOrDefault();
				return BuyProductByID(product.Id);
			}
			return EStatusBuyIAP.None;
#else
			return EStatusBuyIAP.None;
#endif
		}

		// Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google.
		// Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
		public void RestorePurchases()
		{
			Debug.Log("<color=blue>RestorePurchases Start</color>");
#if DEFINE_IAP
			// If Purchasing has not yet been set up ...
			if (!IsInitialized ()) {
				// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
				Debug.Log ("RestorePurchases FAIL. Not initialized.");
				return;
			}

			// If we are running on an Apple device ... 
			if (Application.platform == RuntimePlatform.IPhonePlayer ||
				Application.platform == RuntimePlatform.OSXPlayer) {
				// ... begin restoring purchases
				Debug.Log ("RestorePurchases started ...");

				// Fetch the Apple store-specific subsystem.
				var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions> ();
				// Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
				// the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
				apple.RestoreTransactions ((result) => {
					// The first phase of restoration. If no more responses are received on ProcessPurchase then 
					// no purchases are available to be restored.
					Debug.Log ("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
					if(OnRestorePurchases != null)
					{
						OnRestorePurchases(result);
					}
				});
				}
			else if(Application.platform == RuntimePlatform.Android)
			{
			}
			// Otherwise ...
			else {
				// We are not running on an Apple device. No work is necessary to restore purchases.
				Debug.Log ("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
#endif
		}


#region IStoreListener

#if DEFINE_IAP
		public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
		{
			// Purchasing has succeeded initializing. Collect our Purchasing references.
			Debug.Log ("IAPManager OnInitialized: PASS");

			// Overall Purchasing system, configured with products for this application.
			m_StoreController = controller;
			// Store specific subsystem, for accessing device-specific store features.
			m_StoreExtensionProvider = extensions;

			//			m_AppleExtensions = extensions.GetExtension<IAppleExtensions> ();
			//			m_SamsungExtensions = extensions.GetExtension<ISamsungAppsExtensions> ();
			//			m_MoolahExtensions = extensions.GetExtension<IMoolahExtension> ();
			//			m_MicrosoftExtensions = extensions.GetExtension<IMicrosoftExtensions> ();
			//			m_UnityChannelExtensions = extensions.GetExtension<IUnityChannelExtensions> ();

#if UNITY_ANDROID
			RestorePurchases();
#endif
		}


		public void OnInitializeFailed (InitializationFailureReason error)
		{
			// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
			Debug.Log ("IAPManager OnInitializeFailed InitializationFailureReason:" + error);
		}
#endif

		//        /// <summary>
		//        /// This will be called when a purchase completes.
		//        /// </summary>
		//        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		//        {
		//            Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
		//            Debug.Log("Receipt: " + e.purchasedProduct.receipt);

		//            m_LastTransactionID = e.purchasedProduct.transactionID;
		//            m_PurchaseInProgress = false;

		//            // Decode the UnityChannelPurchaseReceipt, extracting the gameOrderId
		//            if (m_IsUnityChannelSelected)
		//            {
		//                var unifiedReceipt = JsonUtility.FromJson<UnifiedReceipt>(e.purchasedProduct.receipt);
		//                if (unifiedReceipt != null && !string.IsNullOrEmpty(unifiedReceipt.Payload))
		//                {
		//                    var purchaseReceipt = JsonUtility.FromJson<UnityChannelPurchaseReceipt>(unifiedReceipt.Payload);
		//                    Debug.LogFormat(
		//                        "UnityChannel receipt: storeSpecificId = {0}, transactionId = {1}, orderQueryToken = {2}",
		//                        purchaseReceipt.storeSpecificId, purchaseReceipt.transactionId, purchaseReceipt.orderQueryToken);
		//                }
		//            }

		//#if RECEIPT_VALIDATION // Local validation is available for GooglePlay, Apple, and UnityChannel stores
		//        if (m_IsGooglePlayStoreSelected ||
		//            (m_IsUnityChannelSelected && m_FetchReceiptPayloadOnPurchase) ||
		//            Application.platform == RuntimePlatform.IPhonePlayer ||
		//            Application.platform == RuntimePlatform.OSXPlayer ||
		//            Application.platform == RuntimePlatform.tvOS) {
		//            try {
		//                var result = validator.Validate(e.purchasedProduct.receipt);
		//                Debug.Log("Receipt is valid. Contents:");
		//                foreach (IPurchaseReceipt productReceipt in result) {
		//                    Debug.Log(productReceipt.productID);
		//                    Debug.Log(productReceipt.purchaseDate);
		//                    Debug.Log(productReceipt.transactionID);

		//                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
		//                    if (null != google) {
		//                        Debug.Log(google.purchaseState);
		//                        Debug.Log(google.purchaseToken);
		//                    }

		//                    UnityChannelReceipt unityChannel = productReceipt as UnityChannelReceipt;
		//                    if (null != unityChannel) {
		//                        Debug.Log(unityChannel.productID);
		//                        Debug.Log(unityChannel.purchaseDate);
		//                        Debug.Log(unityChannel.transactionID);
		//                    }

		//                    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
		//                    if (null != apple) {
		//                        Debug.Log(apple.originalTransactionIdentifier);
		//                        Debug.Log(apple.subscriptionExpirationDate);
		//                        Debug.Log(apple.cancellationDate);
		//                        Debug.Log(apple.quantity);
		//                    }

		//                    // For improved security, consider comparing the signed
		//                    // IPurchaseReceipt.productId, IPurchaseReceipt.transactionID, and other data
		//                    // embedded in the signed receipt objects to the data which the game is using
		//                    // to make this purchase.
		//                }
		//            } catch (IAPSecurityException ex) {
		//                Debug.Log("Invalid receipt, not unlocking content. " + ex);
		//                return PurchaseProcessingResult.Complete;
		//            }
		//        }
		//#endif

		//            // Unlock content from purchases here.
		//#if USE_PAYOUTS
		//        if (e.purchasedProduct.definition.payouts != null) {
		//            Debug.Log("Purchase complete, paying out based on defined payouts");
		//            foreach (var payout in e.purchasedProduct.definition.payouts) {
		//                Debug.Log(string.Format("Granting {0} {1} {2} {3}", payout.quantity, payout.typeString, payout.subtype, payout.data));
		//            }
		//        }
		//#endif
		//            // Indicate if we have handled this purchase.
		//            //   PurchaseProcessingResult.Complete: ProcessPurchase will not be called
		//            //     with this product again, until next purchase.
		//            //   PurchaseProcessingResult.Pending: ProcessPurchase will be called
		//            //     again with this product at next app launch. Later, call
		//            //     m_Controller.ConfirmPendingPurchase(Product) to complete handling
		//            //     this purchase. Use to transactionally save purchases to a cloud
		//            //     game service.
		//#if DELAY_CONFIRMATION
		//        StartCoroutine(ConfirmPendingPurchaseAfterDelay(e.purchasedProduct));
		//        return PurchaseProcessingResult.Pending;
		//#else
		//            UpdateProductUI(e.purchasedProduct);
		//            return PurchaseProcessingResult.Complete;
		//#endif
		//        }


#if DEFINE_IAP
		public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args)
		{
			var product = GetProductInfoById(args.purchasedProduct.definition.id);
			if (product != null && product.IsTypeRemoveAds())
			{
				AdsManager.Instance.RemoveAds(true);
			}

            OnBuyPurchaseSuccessed?.Invoke(args.purchasedProduct.definition.id);

            if (
                //m_IsGooglePlayStoreSelected ||
                //(m_IsUnityChannelSelected && m_FetchReceiptPayloadOnPurchase) ||
                Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer ||
                Application.platform == RuntimePlatform.tvOS)
            {
                try
                {
                    IPurchaseReceipt[] result = validator.Validate(args.purchasedProduct.receipt);
                    Debug.Log("Receipt is valid. Contents:");
                    foreach (IPurchaseReceipt productReceipt in result)
                    {
                        Debug.Log(productReceipt.productID);
                        Debug.Log(productReceipt.purchaseDate);
                        Debug.Log(productReceipt.transactionID);

                        GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                        if (null != google)
                        {
                            Debug.Log(google.purchaseState);
                            Debug.Log(google.purchaseToken);
                        }

                        UnityChannelReceipt unityChannel = productReceipt as UnityChannelReceipt;
                        if (null != unityChannel)
                        {
                            Debug.Log(unityChannel.productID);
                            Debug.Log(unityChannel.purchaseDate);
                            Debug.Log(unityChannel.transactionID);
                        }

                        AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                        if (null != apple)
                        {
                            Debug.Log(apple.originalTransactionIdentifier);
                            Debug.Log(apple.subscriptionExpirationDate);
                            Debug.Log(apple.cancellationDate);
                            Debug.Log(apple.quantity);
                        }

                        // For improved security, consider comparing the signed
                        // IPurchaseReceipt.productId, IPurchaseReceipt.transactionID, and other data
                        // embedded in the signed receipt objects to the data which the game is using
                        // to make this purchase.
                    }
                }
                catch (IAPSecurityException ex)
                {
                    Debug.Log("Invalid receipt, not unlocking content. " + ex);
                    return PurchaseProcessingResult.Complete;
                }
            }

            return PurchaseProcessingResult.Complete;
		}


		public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
		{
			string id = product.definition.storeSpecificId;
			if (OnBuyPurchaseFailed != null)
				OnBuyPurchaseFailed(id, failureReason.ToString());
			// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
			// this reason with the user to guide their troubleshooting actions.
			Debug.Log (string.Format ("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", id, failureReason));
		}
#endif
#endregion


		public IAPProductInfo[] GetAllProductInfo()
		{
			IAPProductInfo[] products = null;
#if UNITY_ANDROID
			products = iapSettingInfo.AndroidProductInfos;
#elif UNITY_IOS
			products = iapSettingInfo.IOSProductInfos;
#endif
            if (products != null){
				return products;
			}
			else{
				return new IAPProductInfo[0];
			}
		}
        
		/// <summary>
		/// 
		/// </summary>
		/// <param name="productId"></param>
		/// <returns></returns>
		public IAPProductInfo GetProductInfoById(string productId)
		{
			IAPProductInfo[] products = null;
#if UNITY_ANDROID
			products = iapSettingInfo.AndroidProductInfos;
#elif UNITY_IOS
			products = iapSettingInfo.IOSProductInfos;
#endif
            if (products != null)
			{
				return products.Where(item => String.Equals(productId, item.Id)).FirstOrDefault();
			}
			else
			{
				return null;
			}
		}
        
		public IAPProductInfo GetProductInfoByName(string productName)
		{
			IAPProductInfo[] products = null;
#if UNITY_ANDROID
			products = iapSettingInfo.AndroidProductInfos;
#elif UNITY_IOS
			products = iapSettingInfo.IOSProductInfos;
#endif
            if (products != null)
			{
				return products.Where(item => String.Equals(productName, item.Name)).FirstOrDefault();
			}
			else
			{
				return null;
			}
		}

#if VINSAGA_IAP
        public ProductMetadata GetProductMetadataByName(string productName)
        {
            var info = GetProductInfoByName(productName);
            return GetProductMetadataById(info.IdPurchase);
        }

        public ProductMetadata GetProductMetadataById(string productId)
        {
            if (IsInitialized())
            {
                var product = _storeController.products.all.Where(pro => pro.definition.id.Equals(productId)).FirstOrDefault();
                if (product != null) return product.metadata;
            }

            return null;
        }
#else
        public UnityEngine.Object GetProductMetadataByName(string productName)
        {
            return null;
        }

        public UnityEngine.Object GetProductMetadataById(string productId)
        {
            return null;
        }
#endif

        public string GetProductPriceByName(string productName)
        {
            var info = GetProductInfoByName(productName);
            return GetProductPriceById(info.Id);
        }

        public string GetProductPriceById(string productId)
        {
            //#if DEFINE_IAP
            //            var meta = GetProductMetadataById(productId);
            //            if (meta != null) return meta.localizedPriceString;
            //#endif
            var info = GetProductInfoById(productId);
            if (info != null)
            {
                return info.Price.ToString();
            }
            return "$$$$";
        }

        public IAPProductInfo[] GetProductHasTag()
        {
            var products = GetAllProductInfo();
            if (products != null)
            {
                return products.Where(product => product.HasTag()).ToArray();
            }
            return null;
        }

        public IAPProductInfo[] GetProductWithTag(ETagProduct tagProduct)
        {
            var products = GetAllProductInfo();
            if (products != null)
            {
                return products.Where(product => product.InAppTag == tagProduct).ToArray();
            }
            return null;
        }

        public int GetIndexProduct(string id)
        {
            var products = GetAllProductInfo();
            if (products == null) return -1;
            for (int counter = 0; counter < products.Length; counter++)
            {
                if (products[counter].Id.Equals(id)) return counter;
            }
            return -1;
        }

        public bool CheckProductBought(string productID)
        {
#if DEFINE_IAP
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productID);
                return product != null && product.hasReceipt;
            }
            else
            {
                return false;
            }
#else
            return false;
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Save Info In App Purchase")]
        public void SaveInfo()
		{
            string pathFolder = Application.dataPath + "/Common/Data";
            DMCMobileUtils.CreateDirectory(pathFolder);
			string path = pathFolder + "/IAPInfo.txt";
            string str = JsonConvert.SerializeObject(iapSettingInfo);
            System.IO.StreamWriter file = new System.IO.StreamWriter (path);
            file.WriteLine(str);
            file.Close ();
			UnityEditor.AssetDatabase.Refresh ();
		}

        [ContextMenu("Load Info In App Purchase")]
        public void LoadInfo()
		{
			string path = Application.dataPath + "/Common/Data/IAPInfo.txt";
			string text = System.IO.File.ReadAllText (path);
			iapSettingInfo = JsonConvert.DeserializeObject<IAPSettingInfo> (text);
        }
#endif
    }
}