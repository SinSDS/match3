using UnityEngine;
using System.Collections;

#if DEFINE_ADMOB
using GoogleMobileAds.Api;
#endif

#if DEFINE_CHARTBOOST
using ChartboostSDK;
#endif

using System;
using Newtonsoft.Json;
using System.IO;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace com.F4A.MobileThird
{
    #region Classes
    #region AdColony
    [Serializable]
    public class DMCAdColonyConfigData
    {
        [SerializeField]
        private bool _enableAd;
        public bool EnableAd
        {
            get { return _enableAd; }
            set { _enableAd = value; }
        }


        [SerializeField]
        private DMCAdColonyAppInfo[] _androidInfos;
        public DMCAdColonyAppInfo[] AndroidInfos
        {
            get { return _androidInfos; }
            set { _androidInfos = value; }
        }

        [SerializeField]
        private DMCAdColonyAppInfo[] _iosInfos;
        public DMCAdColonyAppInfo[] IosInfos
        {
            get { return _iosInfos; }
            set { _iosInfos = value; }
        }

        public DMCAdColonyAppInfo GetAppInfo()
        {
            DMCAdColonyAppInfo app = null;
            var infos = _androidInfos;
#if UNITY_IOS
            infos = _iosInfos;
#endif
            if(infos != null && infos.Length > 0)
            {
                app = infos[Random.Range(0, infos.Length)];
            }
            return app;
        }

        public List<string> GetConfigApp()
        {
            List<string> config = new List<string>();
            var app = GetAppInfo();
            if(app != null)
            {
                config.Add(app.AppId);

                foreach(var zone in app.Zones)
                {
                    config.Add(zone.ZoneId);
                }
            }
            return config;
        }

        public Dictionary<string, string[]> GetDictionConfigApp()
        {
            Dictionary<string, string[]> config = null;
            var app = GetAppInfo();
            if (app != null && app.Zones.Length > 0)
            {
                config = new Dictionary<string, string[]>();
                string[] zones = new string[app.Zones.Length];
                for(int counter = 0; counter < app.Zones.Length; counter++)
                {
                    zones[counter] = app.Zones[counter].ZoneId;
                }

                config[app.AppId] = zones;
            }
            return config;
        }
    }

    [Serializable]
    public class DMCAdColonyAppInfo
    {
        [SerializeField]
        private string _appName;
        public string AppName
        {
            get { return _appName; }
            set { _appName = value; }
        }

        [SerializeField]
        private string _appId;
        public string AppId
        {
            get { return _appId; }
            set { _appId = value; }
        }

        [SerializeField]
        private DMCAdColonyZoneInfo[] _zones;
        public DMCAdColonyZoneInfo[] Zones
        {
            get { return _zones; }
            set { _zones = value; }
        }
    }

    [Serializable]
    public class DMCAdColonyZoneInfo
    {
        [SerializeField]
        private string _zoneName;
        public string ZoneName
        {
            get { return _zoneName; }
            set { _zoneName = value; }
        }

        [SerializeField]
        private string _zoneId;
        public string ZoneId
        {
            get { return _zoneId; }
            set { _zoneId = value; }
        }

    }
    #endregion

    #region AdMob
    [System.Serializable]
    public class AdmobConfigData
    {
        [SerializeField]
        private string _version;
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        [SerializeField]
        private bool _isEnableAd = true;
        public bool IsEnableAd
        {
            get { return _isEnableAd; }
            set { _isEnableAd = value; }
        }

        //[JsonProperty("id_device_tests")]
        [SerializeField]
        private string[] _idDeviceTests = { "id01", "id02" };
        public string[] IdDeviceTests
        {
            get { return _idDeviceTests; }
            set { _idDeviceTests = value; }
        }

        [SerializeField]
        private DMCAdmobAppInfo[] _androidAppInfos = new DMCAdmobAppInfo[1];
        public DMCAdmobAppInfo[] AndroidAppInfos
        {
            get { return _androidAppInfos; }
            set { _androidAppInfos = value; }
        }

        [SerializeField]
        private DMCAdmobAppInfo[] _iosAppInfos = new DMCAdmobAppInfo[1];
        public DMCAdmobAppInfo[] IosAppInfos
        {
            get { return _iosAppInfos; }
            set { _iosAppInfos = value; }
        }

        public DMCAdmobAppInfo GetAppInfo()
        {
#if UNITY_ANDROID
            if (AndroidAppInfos != null && AndroidAppInfos.Length > 0)
            {
                return AndroidAppInfos[Random.Range(0, AndroidAppInfos.Length)];
            }
#elif UNITY_IOS
            if (IosAppInfos != null && IosAppInfos.Length > 0)
            {
                return IosAppInfos[Random.Range(0, IosAppInfos.Length)];
            }
#endif
            return null;
        }

        public string GetAppId()
        {
            DMCAdmobAppInfo appInfo = GetAppInfo();
            if (appInfo != null)
            {
                return appInfo.AppId;
            }
            return "";
        }

        public string GetBannerId()
        {
            DMCAdmobAppInfo appInfo = GetAppInfo();
            if (appInfo != null)
            {
                return appInfo.BannerId;
            }
            return "";
        }

        public string GetInterstitialId()
        {
            DMCAdmobAppInfo appInfo = GetAppInfo();
            if (appInfo != null)
            {
                return appInfo.InterstitialId;
            }
            return "";
        }

        public string GetRewardId()
        {
            DMCAdmobAppInfo appInfo = GetAppInfo();
            if (appInfo != null)
            {
                return appInfo.RewardId;
            }
            return "";
        }
    }

    [System.Serializable]
    public class DMCAdmobAppInfo
    {
        [SerializeField]
        private string _appName = "_appName";
        public string AppName
        {
            get { return _appName; }
            set { _appName = value; }
        }

        [SerializeField]
        private string _appId = "ca-app-pub-3940256099942544~3347511713";
        public string AppId
        {
            get { return _appId; }
            set { _appId = value; }
        }

        [SerializeField]
        private string _bannerName = "_bannerName";
        public string BannerName
        {
            get { return _bannerName; }
            set { _bannerName = value; }
        }

        [SerializeField]
        private string _bannerId = "ca-app-pub-3940256099942544/6300978111";
        public string BannerId
        {
            get { return _bannerId; }
            set { _bannerId = value; }
        }

        [SerializeField]
        private string _interstitialName = "_interstitialName";
        public string InterstitialName
        {
            get { return _interstitialName; }
            set { _interstitialName = value; }
        }

        [SerializeField]
        private string _interstitialId = "ca-app-pub-3940256099942544/1033173712";
        public string InterstitialId
        {
            get { return _interstitialId; }
            set { _interstitialId = value; }
        }

        [SerializeField]
        private string _interstitialVideoName = "_interstitialVideoName";
        public string InterstitialVideoName
        {
            get { return _interstitialVideoName; }
            set { _interstitialVideoName = value; }
        }
        [SerializeField]
        private string _interstitialVideoId = "ca-app-pub-3940256099942544/8691691433";
        public string InterstitialVideoId
        {
            get { return _interstitialVideoId; }
            set { _interstitialVideoId = value; }
        }

        [SerializeField]
        private string _rewardName = "_rewardName";
        public string RewardName
        {
            get { return _rewardName; }
            set { _rewardName = value; }
        }

        [SerializeField]
        private string _rewardId = "ca-app-pub-3940256099942544/5224354917";
        public string RewardId
        {
            get { return _rewardId; }
            set { _rewardId = value; }
        }

        [SerializeField]
        private string _nativeAdvancedName = "_nativeAdvancedName";
        public string NativeAdvancedName
        {
            get { return _nativeAdvancedName; }
            set { _nativeAdvancedName = value; }
        }
        [SerializeField]
        private string _nativeAdvancedId = "ca-app-pub-3940256099942544/2247696110";
        public string NativeAdvancedId
        {
            get { return _nativeAdvancedId; }
            set { _nativeAdvancedId = value; }
        }


        [SerializeField]
        private string _nativeAdvancedVideoName = "_nativeAdvancedVideoName";
        public string NativeAdvancedVideoName
        {
            get { return _nativeAdvancedVideoName; }
            set { _nativeAdvancedVideoName = value; }
        }
        [SerializeField]
        private string _nativeAdvancedVideoId = "ca-app-pub-3940256099942544/1044960115";
        public string NativeAdvancedVideoId
        {
            get { return _nativeAdvancedVideoId; }
            set { _nativeAdvancedVideoId = value; }
        }

        public DMCAdmobAppInfo()
        {
            _appId = "ca-app-pub-3940256099942544~3347511713";
            _bannerId = "ca-app-pub-3940256099942544/6300978111";
            _interstitialId = "ca-app-pub-3940256099942544/1033173712";
            _interstitialVideoId = "ca-app-pub-3940256099942544/8691691433";
            _rewardId = "ca-app-pub-3940256099942544/5224354917";
            _nativeAdvancedId = "ca-app-pub-3940256099942544/2247696110";
            _nativeAdvancedVideoId = "ca-app-pub-3940256099942544/1044960115";
        }
    }
    #endregion

    #region Unity Ads
    [System.Serializable]
    public class UnityAdConfigData
    {
        public bool enableAd = false;

        [SerializeField]
        private DMCUnityAdsAppInfo[] _androidAppInfos;
        public DMCUnityAdsAppInfo[] AndroidAppInfos
        {
            get { return _androidAppInfos; }
            set { _androidAppInfos = value; }
        }

        [SerializeField]
        private DMCUnityAdsAppInfo[] _iosdAppInfos;
        public DMCUnityAdsAppInfo[] IosdAppInfos
        {
            get { return _iosdAppInfos; }
            set { _iosdAppInfos = value; }
        }

        public DMCUnityAdsAppInfo GetAppInfo()
        {
#if UNITY_ANDROID
            if(AndroidAppInfos != null && AndroidAppInfos.Length > 0)
            {
                return AndroidAppInfos[Random.Range(0, AndroidAppInfos.Length)];
            }
#elif UNITY_IOS
            if(IosdAppInfos != null && IosdAppInfos.Length > 0)
            {
                return IosdAppInfos[Random.Range(0, IosdAppInfos.Length)];
            }
#endif
            return null;

        }

        public string GetIdAd()
        {
            DMCUnityAdsAppInfo app = GetAppInfo();
            if (app != null)
            {
                return app.IdAd;
            }
            return "";
        }
    }

    [Serializable]
    public class DMCUnityAdsAppInfo
    {
        [SerializeField]
        private string _nameApp;
        public string NameApp
        {
            get { return _nameApp; }
            set { _nameApp = value; }
        }

        [SerializeField]
        private string _idApp;
        public string IdApp
        {
            get { return _idApp; }
            set { _idApp = value; }
        }

        [SerializeField]
        private string _idAd;
        public string IdAd
        {
            get { return _idAd; }
            set { _idAd = value; }
        }

    }
    #endregion

    [System.Serializable]
    public class VungleConfigData
    {
        public bool enableAd = false;
        public string[] androidIds = { };
        public string[] iosIds = { };
        public string[] winphoneIds = { };
    }

    [System.Serializable]
    public class FacebookAdConfigData
    {
        public bool enableAd = false;

        public string[] androidBannerIds = { };
        public string[] androidInterstitialIds = { };

        public string[] iosBannerIds = { };
        public string[] iosInterstitialIds = { };
    }

    [System.Serializable]
    public class DMCChartboostConfigData
    {
        [SerializeField]
        private bool _enableAd;
        public bool EnableAd
        {
            get { return _enableAd; }
            set { _enableAd = value; }
        }

        [SerializeField]
        private DMCChartboostAppInfo[] _androidApps = new DMCChartboostAppInfo[1];
        public DMCChartboostAppInfo[] AndroidApps
        {
            get { return _androidApps; }
            set { _androidApps = value; }
        }

        [SerializeField]
        private DMCChartboostAppInfo[] _iosApps = new DMCChartboostAppInfo[1];
        public DMCChartboostAppInfo[] IosApps
        {
            get { return _iosApps; }
            set { _iosApps = value; }
        }
    }

    [SerializeField]
    public class DMCChartboostAppInfo
    {
        [SerializeField]
        private string _appId;
        public string AppId
        {
            get { return _appId; }
            set { _appId = value; }
        }

        [SerializeField]
        private string _appSignature;
        public string AppSignature
        {
            get { return _appSignature; }
            set { _appSignature = value; }
        }

    }

    [System.Serializable]
    public class AdDataComponent
    {
        [JsonProperty("name")]
        public string Name;//{get;set;}
        [JsonProperty("priority")]
        public int Priority;//{get;set;}
        [JsonProperty("enable")]
        public bool Enable;
    }

    [System.Serializable]
    public class AdConfigData
    {
        [SerializeField]
        private bool isEnableAds;
        public bool IsEnableAds
        {
            get { return isEnableAds; }
            set { isEnableAds = value; }
        }

        [SerializeField]
        private EAdPosition adPosition = EAdPosition.Bottom;
        public EAdPosition AdPosition
        {
            get { return adPosition; }
            set { adPosition = value; }
        }

        [Header("Ad Network")]
        public ERewardedAdNetwork[] rewardAdOrder = { ERewardedAdNetwork.UnityAds, ERewardedAdNetwork.AdMob, ERewardedAdNetwork.Vungle };
        public EInterstitialAdNetwork[] interstitialAdsOrder = { EInterstitialAdNetwork.AdMob, EInterstitialAdNetwork.UnityAds };
        public EVideoAdNetwork[] videoAdsOrder = { EVideoAdNetwork.UnityAds };
        public EBannerAdNetwork[] bannerAdsOrder = { EBannerAdNetwork.AdMob, EBannerAdNetwork.UnityAds };

        [Header("Ads")]
        public DMCAdColonyConfigData adColonyConfigData;
        public AdmobConfigData admobConfig;
        public UnityAdConfigData unityAdConfig;
        public VungleConfigData vungleConfig;
        public FacebookAdConfigData facebookAdConfig;
        public DMCChartboostConfigData chartboostConfigData;

        [Header("Config")]
        public int timesEventShowInterstitial = 2;

        public bool isShowBannerAd = false;
    }
#endregion

    /// <summary>
    /// A class is manager all ads in project
    /// AdMob, UnityAds, Vungle, Chartboost, StartApp
    /// </summary>
    [AddComponentMenu("F4A/AdsManager")]
    public partial class AdsManager : SingletonMono<AdsManager>
    {
#pragma warning disable 414
        public static event Action<bool> OnRemoveAds = delegate { };

        public static event Action<ERewardedAdNetwork> OnRewardedAdCompleted = delegate { };
        public static event Action<ERewardedAdNetwork> OnRewardedAdSkiped = delegate { };
        public static event Action<ERewardedAdNetwork> OnRewardedAdFailed = delegate { };

        public static event Action<EVideoAdNetwork> OnVideodAdCompleted = delegate { };
        public static event Action<EVideoAdNetwork> OnVideodAdFailed = delegate { };

        public static event Action<EInterstitialAdNetwork> OnInterstitialAdClosed = delegate { };
        public static event Action<EInterstitialAdNetwork> OnInterstitialAdFailed = delegate { };

#region Variables
        [Header("ADS MANAGER")]
        [SerializeField]
        private bool isOnline = true;
        [SerializeField]
        private string urlFileDataSetting = null;
        [SerializeField]
        private TextAsset fileAdConfigDefault = null;
        [SerializeField]
        private AdConfigData adConfigData;

        private int countEventShowInterstitial = 0;
        private int countCheckBannerAds = 0;
#endregion

#region Unity Method
        // Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
        private void Start()
        {
            F4ACoreManager.OnDownloadF4AConfigCompleted += F4ACoreManager_OnDownloadF4AConfigCompleted;
        }

        protected void OnDestroy()
        {
            F4ACoreManager.OnDownloadF4AConfigCompleted -= F4ACoreManager_OnDownloadF4AConfigCompleted;
        }
#endregion

        private void CheckShowBannerAds()
        {
            try
            {
                countCheckBannerAds++;
                if (countCheckBannerAds <= 10)
                {
                    DOVirtual.DelayedCall(1, () =>
                    {
                        if (IsBannerAdsReady())
                        {
                            ShowBannerAds();
                        }
                        else
                        {
                            CheckShowBannerAds();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                AnalyticManager.Instance.CustomeEvent("CheckShowBannerAds Failed! ex:" + ex.Message);
            }
        }

#region Handles, Events
        private void F4ACoreManager_OnDownloadF4AConfigCompleted(F4AConfigData configData, bool success)
        {
            if (configData != null)
            {
                if (!string.IsNullOrEmpty(configData.urlConfigAds))
                {
                    urlFileDataSetting = configData.urlConfigAds;
                }
                if (!string.IsNullOrEmpty(configData.urlConfigWebView))
                {
                    urlWebViewConfig = configData.urlConfigWebView;
                }
            }

            InitWebViewScroll();

            if (isOnline)
            {
                StartCoroutine(DMCMobileUtils.AsyncGetDataFromUrl(urlFileDataSetting, fileAdConfigDefault, (string data) =>
                {
                    if (!string.IsNullOrEmpty(data))
                    {
                        adConfigData = JsonConvert.DeserializeObject<AdConfigData>(data);
                    }
                    InitAds();
                }));
            }
            else
            {
                InitAds();
            }
        }

#endregion

        private void InitAds()
        {
            InitializationAdmob();
            InitializationUnityAds();
            InitVungle();
            InitChartboost();
            InitAdColony();
            LoadWebViewScroll();

            if (adConfigData.isShowBannerAd)
            {
                countCheckBannerAds = 0;
                CheckShowBannerAds();
            }
        }

        public bool IsEnalbleAds()
        {
            return adConfigData != null && adConfigData.IsEnableAds;
        }
	    
#region Show Ad Methods

        public bool IsInterstitialAdsReady()
        {
            try
            {
	            if (IsRemoveAds() || !IsEnalbleAds())
		            return false;
	            if(countEventShowInterstitial + 1 <  adConfigData.timesEventShowInterstitial)
		            return false;
                int count = adConfigData.interstitialAdsOrder.Length;
                for (int i = 0; i < count; i++)
                {
                    if (IsInterstitialAdReady(adConfigData.interstitialAdsOrder[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("IsInterstitialAdsReady ex:" + ex.Message);
                return false;
            }
        }

        protected bool IsInterstitialAdReady(EInterstitialAdNetwork adsType)
        {
            try
            {
                // can't show because it remove ads.
	            if (IsRemoveAds() || !IsEnalbleAds())
		            return false;
	            if(countEventShowInterstitial + 1 < adConfigData.timesEventShowInterstitial)
		            return false;

#if DEFINE_ADMOB
		        if (adsType == EInterstitialAdNetwork.AdMob)
	            {
	                if (SystemLanguage.Chinese != Application.systemLanguage
	                    && SystemLanguage.ChineseSimplified != Application.systemLanguage
	                    && SystemLanguage.ChineseTraditional != Application.systemLanguage)
	                {
	                    return interstitialAdmob.IsLoaded();
	                }
	        	}
#endif

#if DEFINE_CHARTBOOST
	            if (adsType == EInterstitialAdNetwork.Chartboost)
	            {
					return Chartboost.hasInterstitial (CBLocation.Default);
	            }
#endif

#if DEFINE_UNITY_ADS
	            if (adsType == EInterstitialAdNetwork.UnityAds)
	            {
	                return IsInterstitialUnityAdReady();
	            }
#endif
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("IsInterstitialAdReady ex:" + ex.Message);
                return false;
            }
        }

        public bool ShowInterstitialAds()
        {
            try
            {
                if (IsRemoveAds())
                    return false;
                Debug.Log("<color=green>AdsManager.ShowInterstitialAds Start</color>");
                countEventShowInterstitial++;
	            if (countEventShowInterstitial >= adConfigData.timesEventShowInterstitial)
                {
                    int count = adConfigData.interstitialAdsOrder.Length;
                    for (int i = 0; i < count; i++)
                    {
                        var adNetwork = adConfigData.interstitialAdsOrder[i];
                        if (ShowInterstitialAd(adNetwork))
                        {
                            LogEventAds("ShowInterstitialAds", "AdNetwork", adNetwork.ToString());
                            countEventShowInterstitial = 0;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("ShowInterstitialAds ex:" + ex.Message);
                return false;
            }
        }

	    protected bool ShowInterstitialAd(EInterstitialAdNetwork adsType)
        {
            try
            {
                Debug.Log("<color=green>ShowInterstitialAd adsType: " + adsType + "</color>");
                // can't show because it remove ads.
                if (IsRemoveAds())
                    return false;
                if (adsType == EInterstitialAdNetwork.AdMob)
                {
#if DEFINE_ADMOB
			    	if (SystemLanguage.Chinese != Application.systemLanguage
				    	&& SystemLanguage.ChineseSimplified != Application.systemLanguage
				    	&& SystemLanguage.ChineseTraditional != Application.systemLanguage) {
				    	return ShowInterstitialAdmob();
			    	}
#endif
                }
                else if (adsType == EInterstitialAdNetwork.Chartboost)
                {
#if DEFINE_CHARTBOOST
			    	if (Chartboost.hasInterstitial (CBLocation.Default)) {
			    		Chartboost.showInterstitial (CBLocation.Default);
			    		Chartboost.cacheInterstitial (CBLocation.Default);
	                	InitChartboost ();	
	                	return true;
			    	}
			    	InitChartboost ();
#endif
                }
                else if (adsType == EInterstitialAdNetwork.UnityAds)
                {
#if DEFINE_UNITY_ADS
			    	return ShowInterstitialUnityAd();
#endif
                }
                else if(adsType == EInterstitialAdNetwork.WebView)
                {
                	if(webViewRequestData.webViewConfigData.activeAds)
                	{
                		return ShowInterstitialWebView();
                	}
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("ShowInterstitialAd ex:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Determines whether this instance is reward video ready.
        /// </summary>
        /// <returns><c>true</c> if this instance is reward video ready; otherwise, <c>false</c>.</returns>
        public bool IsRewardAdsReady()
        {
#if UNITY_EDITOR
            return true;
#endif
            try
            {
                int count = adConfigData.rewardAdOrder.Length;
                for (int i = 0; i < count; i++)
                {
                    if (IsRewardAdsReady(adConfigData.rewardAdOrder[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("IsRewardAdsReady ex:" + ex.Message);
                return false;
            }
        }

        public bool IsRewardAdsReady(ERewardedAdNetwork adsType)
        {
            try
            {
#if DEFINE_UNITY_ADS
	            if (adsType == ERewardedAdNetwork.UnityAds)
                {
	                return IsRewardedUnityAdReady();
		        }
#endif

#if DEFINE_ADMOB
                if (adsType == ERewardedAdNetwork.AdMob)
                {
                    return IsRewardAdMobReady();
                }
#endif

#if DEFINE_VUNGLE
        	if (adsType == ERewardedAdNetwork.Vungle) {
			string placement_id = "";
#if UNITY_IOS
			placement_id = VunglePlacementIos;
#elif UNITY_ANDROID
			placement_id = VunglePlacementAndroid;
#elif UNITY_WSA_10_0 || UNITY_WINRT_8_1 || UNITY_METRO
			placement_id = VunglePlacementWindowPhone;
#endif
			if (Vungle.isAdvertAvailable(placement_id)) {
			return true;
			}
			else
		        return false;
        }	
#endif

#if DEFINE_CHARTBOOST
	            if (adsType == ERewardedAdNetwork.Chartboost){
                    return IsRewardChartboostReady();
	        	}
#endif

#if DEFINE_ADCOLONY
                if(adsType == ERewardedAdNetwork.AdColony)
                {
                    return IsAdColonyReady();
                }
#endif
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("IsRewardAdsReady ex:" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// This method is show video and reward when watch complete video
        /// </summary>
        public bool ShowRewardAds()
        {
#if UNITY_EDITOR
            OnRewardedAdCompleted?.Invoke(ERewardedAdNetwork.None);
            return true;
#endif

            try
            {
                int count = adConfigData.rewardAdOrder.Length;
                for (int i = 0; i < count; i++)
                {
                    if (ShowRewardAd(adConfigData.rewardAdOrder[i]))
                    {
                        LogEventAds("ShowRewardAds", "AdNetwork", adConfigData.rewardAdOrder[i].ToString());
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("AdsManager.ShowRewardAds ex:" + ex.Message);
                return false;
            }
        }

        private bool ShowRewardAd(ERewardedAdNetwork adsType)
        {
            try
            {
#if DEFINE_UNITY_ADS
                if (adsType == ERewardedAdNetwork.UnityAds)
                {
                    return ShowRewardUnityAd();
                }
#endif

#if DEFINE_ADMOB
                if (adsType == ERewardedAdNetwork.AdMob)
                {
                    return ShowRewardAdmobAd();
                }
#endif

#if DEFINE_VUNGLE
			if (adsType == ERewardedAdNetwork.Vungle) {
				string placement_id = "";
#if UNITY_IOS
				placement_id = VunglePlacementIos;
#elif UNITY_ANDROID
				placement_id = VunglePlacementAndroid;
#elif UNITY_WSA_10_0 || UNITY_WINRT_8_1 || UNITY_METRO
				placement_id = VunglePlacementWindowPhone;
#endif
				if (Vungle.isAdvertAvailable(placement_id)) {
					Vungle.playAd (placement_id);
					return true;
				}
			}
#endif

#if DEFINE_CHARTBOOST
                if (adsType == ERewardedAdNetwork.Chartboost)
                {
                    return ShowRewardCharstboost();
                }
#endif

#if DEFINE_ADCOLONY
                if (adsType == ERewardedAdNetwork.AdColony)
                {
                    return ShowAdColony();
                }
#endif
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("AdsManager.ShowRewardAd ex:" + ex.Message);
                return false;
            }
        }

        public bool IsVideoAdsReady()
        {
            return false;
        }

        private bool IsVideoAdReady(EVideoAdNetwork adType)
        {
            return false;
        }

        /// <summary>
        /// This method is only show video and don't reward when watch video
        /// </summary>
        public bool ShowVideoAds()
        {
            try
            {
                // can't show because it remove ads.
                if (IsRemoveAds())
                    return false;
                int count = adConfigData.videoAdsOrder.Length;
                for (int i = 0; i < count; i++)
                {
                    if (ShowVideoAd(adConfigData.videoAdsOrder[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("AdsManager.ShowVideoAds ex:" + ex.Message);
                return false;
            }
        }

        protected bool ShowVideoAd(EVideoAdNetwork adType)
        {
            try
            {
                // can't show because it remove ads.
                if (IsRemoveAds())
                    return false;
#if DEFINE_UNITY_ADS
		    if (adType == EVideoAdNetwork.UnityAds)
		    {
			    return ShowVideoUnityAd();
		    }
#endif
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("AdsManager.ShowVideoAd ex:" + ex.Message);
                return false;
            }
        }

#region Banner
        public bool IsBannerAdsReady()
        {
            try
            {
                // can't show because it remove ads.
                if (IsRemoveAds())
                    return false;
                int count = adConfigData.bannerAdsOrder.Length;
                for (int i = 0; i < count; i++)
                {
                    if (ShowBannerAd(adConfigData.bannerAdsOrder[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("AdsManager.ShowBannerAds ex:" + ex.Message);
                return false;
            }
        }

        private bool IsBannerAdReady(EBannerAdNetwork type)
        {
            try
            {
                // can't show because it remove ads.
                if (IsRemoveAds())
                    return false;
                if (type == EBannerAdNetwork.WebView)
                {
                    if (webViewRequestData.webViewConfigData.activeAds)
                    {
                        return IsBannerWebViewReady();
                    }
                }
                else if (type == EBannerAdNetwork.AdMob)
                {
                    return IsBannerAdAdmobReady();
                }
                else if(type == EBannerAdNetwork.UnityAds){
                	return IsBannerUnityAdReady();
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("ShowVideoAd ex:" + ex.Message);
                return false;
            }
        }

        public bool ShowBannerAds()
        {
            try
            {
                // can't show because it remove ads.
                if (IsRemoveAds())
                    return false;
                Debug.Log("<color=green>ShowBannerAds Start</color>");
                int count = adConfigData.bannerAdsOrder.Length;
                for (int i = 0; i < count; i++)
                {
                    if (ShowBannerAd(adConfigData.bannerAdsOrder[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("ShowBannerAds ex:" + ex.Message);
                return false;
            }
        }

        protected bool ShowBannerAd(EBannerAdNetwork type)
        {
            try
            {
                // can't show because it remove ads.
                if (IsRemoveAds()) return false;

                Debug.Log("<color=green>AdsManager.ShowBannerAd type:" + type + "</color>");

                if (type == EBannerAdNetwork.WebView)
                {
                    if (webViewRequestData.webViewConfigData.activeAds)
                    {
                        return ShowBannerWebView();
                    }
                }
                else if (type == EBannerAdNetwork.AdMob)
                {
                    return ShowBannerAdAdmob();
                }
                else if (type == EBannerAdNetwork.UnityAds)
                {
                    return ShowBannerUnityAd();
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("AdsManager.ShowVideoAd ex:" + ex.Message);
                return false;
            }
        }
        
	    public bool HideBannerAds(){
	    	try
	    	{
                Debug.Log("<color=red>HideBannerAds Start</color>");
                int count = adConfigData.bannerAdsOrder.Length;
		    	for (int i = 0; i < count; i++)
		    	{
			    	HideBannerAd(adConfigData.bannerAdsOrder[i]);
		    	}
		    	return true;
	    	}
	    	catch (Exception ex)
	    	{
		    	Debug.Log("HideBannerAds ex:" + ex.Message);
		    	return false;
	    	}
	    }
	    
	    private bool HideBannerAd(EBannerAdNetwork type){
	    	try
	    	{
		    	if (type == EBannerAdNetwork.WebView)
		    	{
			    	if (webViewRequestData.webViewConfigData.activeAds)
			    	{
				    	return HideBannerWebView();
			    	}
		    	}
		    	else if (type == EBannerAdNetwork.AdMob)
		    	{
#if DEFINE_ADMOB
			    	return HideBannerAdAdmob();
#endif
		    	}
		    	return false;
	    	}
	    	catch (Exception ex)
	    	{
		    	Debug.Log("HideBannerAd ex:" + ex.Message);
		    	return false;
	    	}
	    }

        public bool DestroyBannerAds(){
            try
            {
                int count = adConfigData.bannerAdsOrder.Length;
                for (int i = 0; i < count; i++)
                {
                    DestroyBannerAd(adConfigData.bannerAdsOrder[i]);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("DestroyBannerAds ex:" + ex.Message);
                return false;
            }
        }

        private bool DestroyBannerAd(EBannerAdNetwork type)
        {
            try
            {
                if (type == EBannerAdNetwork.WebView)
                {
                    if (webViewRequestData.webViewConfigData.activeAds)
                    {
                        return HideBannerWebView();
                    }
                }
                else if (type == EBannerAdNetwork.AdMob)
                {
#if DEFINE_ADMOB
                    DestroyBannerAdAdmob();
#endif
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.Log("HideBannerAd ex:" + ex.Message);
                return false;
            }
        }
#endregion
        /// <summary>
        /// Removes the ads.
        /// </summary>
        public void RemoveAds(bool value)
        {
            SetRemoveAds(value);
        }

        /// <summary>
        /// Sets the remove ads.
        /// </summary>
        /// <param name="isRemoveAd">If set to <c>true</c> is remove.</param>
	    private void SetRemoveAds(bool isRemoveAd)
        {
            //PlayerPrefs.SetInt(F4AUtils.KeyRemoveAds, isRemoveAd ? F4AUtils.AD_REMOVE : F4AUtils.AD_DONT_REMOVE);
            CPlayerPrefs.SetBool(DMCMobileUtils.KeyRemoveAds, isRemoveAd);
            if (isRemoveAd)
            {
                DestroyBannerAds();
            }

            if(OnRemoveAds != null)
            {
                Debug.Log("SetRemoveAds isRemoveAd: " + isRemoveAd);
                OnRemoveAds(isRemoveAd);
            }
        }

        public bool IsRemoveAds()
        {
            //return PlayerPrefs.GetInt(F4AUtils.KeyRemoveAds, F4AUtils.AD_DONT_REMOVE) == F4AUtils.AD_REMOVE;
            return CPlayerPrefs.GetBool(DMCMobileUtils.KeyRemoveAds, false);
        }

#endregion //GENERAL

        private void LogEventAds(string nameEvent, string nameParamater, string valueParamater)
        {
            AnalyticManager.Instance.CustomeEvent(nameEvent, new Dictionary<string, object>()
                            {
                                {nameParamater, valueParamater }
                            });
            FirebaseManager.Instance.LogEvent(nameEvent, nameParamater, valueParamater);
        }

        IEnumerator IEWaitForSeconds(float delay, System.Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack();
        }


#if UNITY_EDITOR
        public void SaveInfo()
        {
	        string str = JsonConvert.SerializeObject (adConfigData);
	        string pathDirectory = Path.Combine(Application.dataPath, DMCMobileUtils.PathFolderData);
            DMCMobileUtils.CreateDirectory(pathDirectory);

	        string path = Path.Combine(pathDirectory, @"AdsInfo.txt");
			StreamWriter file = new System.IO.StreamWriter (path);
			file.WriteLine (str);
			file.Close ();

			UnityEditor.AssetDatabase.Refresh ();
        }

        public void LoadInfo()
        {
            string path = Path.Combine(Application.dataPath, DMCMobileUtils.PathFolderData);
            path = Path.Combine(path, @"AdsInfo.txt");
			string text = System.IO.File.ReadAllText (path);
            adConfigData = JsonConvert.DeserializeObject<AdConfigData>(text);
        }
#endif
    }
}
