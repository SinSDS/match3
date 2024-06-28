using UnityEngine;
using System;
#if DEFINE_ADMOB
using GoogleMobileAds.Api;
#endif

namespace com.F4A.MobileThird
{
    public partial class AdsManager
	{
#if DEFINE_ADMOB
        AdRequest adRequestAdmob = null;

        private InterstitialAd interstitialAdmob;
        private BannerView bannerViewAdmob;
        private RewardBasedVideoAd rewardBasedVideo;
#endif

        private bool IsEnableAdmobAd()
        {
            return adConfigData != null && adConfigData.admobConfig != null && adConfigData.admobConfig.IsEnableAd;
        }

        private void InitializationAdmob()
        {
            try
            {
#if DEFINE_ADMOB
                if (IsEnableAdmobAd())
                {
                    //MobileAds.SetiOSAppPauseOnBackground(true);

                    string appId = "";
                    if (adConfigData != null && adConfigData.admobConfig != null)
                    {
                        appId = adConfigData.admobConfig.GetAppId();
                    }
                    if (!string.IsNullOrEmpty(appId))
                    {
                        // Initialize the Google Mobile Ads SDK.
                        MobileAds.Initialize(appId);
                    }

                    AdRequest.Builder builder = new AdRequest.Builder();
                    for (int i = 0; i < adConfigData.admobConfig.IdDeviceTests.Length; i++)
                    {
                        builder.AddTestDevice(adConfigData.admobConfig.IdDeviceTests[i]);
                    }
                    adRequestAdmob = builder.Build();

                    InitBannerAdmob();
                    InitInterstitialAdmob();
                    InitRewardVideoAdmob();
                }
#endif
            }
            catch (Exception ex)
            {
                AnalyticManager.Instance.CustomeEvent("InitializationAdmob Failed!" + ex.Message);
            }
        }

#region Interstitial
        private void InitInterstitialAdmob()
        {
#if DEFINE_ADMOB
            RequestInterstitialAdmob();

            interstitialAdmob.OnAdClosed += InterstitialAdmob_HandleAdClosed;
            interstitialAdmob.OnAdFailedToLoad += InterstitialAdmob_HandleAdFailedToLoad;
            interstitialAdmob.OnAdLeavingApplication += InterstitialAdmob_HandleAdLeavingApplication;
            interstitialAdmob.OnAdLoaded += InterstitialAdmob_HandleAdLoaded;
            interstitialAdmob.OnAdOpening += InterstitialAdmob_HandleAdOpening;
#endif
        }
        
	    private void DestroyInterstitialAdmob(){
#if DEFINE_ADMOB
		    if(interstitialAdmob != null){
		    	interstitialAdmob.OnAdClosed -= InterstitialAdmob_HandleAdClosed;
			    interstitialAdmob.OnAdFailedToLoad -= InterstitialAdmob_HandleAdFailedToLoad;
			    interstitialAdmob.OnAdLeavingApplication -= InterstitialAdmob_HandleAdLeavingApplication;
			    interstitialAdmob.OnAdLoaded -= InterstitialAdmob_HandleAdLoaded;
			    interstitialAdmob.OnAdOpening -= InterstitialAdmob_HandleAdOpening;
		    }
#endif
	    }

        private void RequestInterstitialAdmob()
        {
#if DEFINE_ADMOB
	        if (IsEnableAdmobAd())
	        {
	        	string id = "";
		        id = adConfigData.admobConfig.GetInterstitialId();
                Debug.Log("<color=blue>AdsManager.Admob.RequestInterstitialAdmob id:" + id + "</color>");
                if (!string.IsNullOrEmpty(id))
                {
                    interstitialAdmob = new InterstitialAd(id);
                    interstitialAdmob.LoadAd(adRequestAdmob);
                }
            }
#endif
        }
        
	    private bool ShowInterstitialAdmob(){
#if DEFINE_ADMOB
            if (IsEnableAdmobAd())
            {
                if (interstitialAdmob.IsLoaded())
                {
					Debug.Log("AdsManager.Admob.ShowInterstitialAdmob");
                    interstitialAdmob.Show();
                    return true;
                }
                else
                {
                    RequestInterstitialAdmob();
                    return false;
                }
            }
#endif
            return false;
        }

        public Action InterstitialAdmob_OnleAdFailedToLoad;
        public Action InterstitialAdmob_OnleAdLeavingApplication;
        public Action InterstitialAdmob_OnleAdLoaded;
        public Action InterstitialAdmob_OnleAdOpening;

#if DEFINE_ADMOB
        private void InterstitialAdmob_HandleAdClosed(object sender, EventArgs e)
        {
            RequestInterstitialAdmob();
            OnInterstitialAdClosed?.Invoke(EInterstitialAdNetwork.AdMob);
        }

        private void InterstitialAdmob_HandleAdOpening(object sender, EventArgs e)
        {
            if (InterstitialAdmob_OnleAdOpening != null)
                InterstitialAdmob_OnleAdOpening();
        }

        private void InterstitialAdmob_HandleAdLoaded(object sender, EventArgs e)
        {
            if (InterstitialAdmob_OnleAdLoaded != null)
                InterstitialAdmob_OnleAdLoaded();
        }

        private void InterstitialAdmob_HandleAdLeavingApplication(object sender, EventArgs e)
        {
            if (InterstitialAdmob_OnleAdLeavingApplication != null)
                InterstitialAdmob_OnleAdLeavingApplication();
        }

        private void InterstitialAdmob_HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            RequestInterstitialAdmob();
            OnInterstitialAdFailed?.Invoke(EInterstitialAdNetwork.AdMob);
        }
#endif
        #endregion

        #region Reward
        public void InitRewardVideoAdmob()
        {
#if DEFINE_ADMOB
            RequestRewardAdmob();

            rewardBasedVideo.OnAdClosed += RewardBasedVideo_HandleAdClosed;
            rewardBasedVideo.OnAdFailedToLoad += RewardBasedVideo_HandleAdFailedToLoad;
            rewardBasedVideo.OnAdLeavingApplication += RewardBasedVideo_HandleAdLeavingApplication;
            rewardBasedVideo.OnAdLoaded += RewardBasedVideo_HandleAdLoaded;
            rewardBasedVideo.OnAdOpening += RewardBasedVideo_HandleAdOpening;
            rewardBasedVideo.OnAdRewarded += RewardBasedVideo_HandleAdRewarded;
            rewardBasedVideo.OnAdStarted += RewardBasedVideo_HandleAdStarted;
#endif
        }

        private void RequestRewardAdmob()
        {
#if DEFINE_ADMOB
            rewardBasedVideo = RewardBasedVideoAd.Instance;
            string id = "";
            if (adConfigData != null && adConfigData.admobConfig != null)
            {
                id = adConfigData.admobConfig.GetRewardId();
            }
            rewardBasedVideo.LoadAd(adRequestAdmob, id);
#endif
        }

        protected bool IsRewardAdMobReady()
        {
#if DEFINE_ADMOB
            return rewardBasedVideo != null && rewardBasedVideo.IsLoaded();
#endif
            return false;
        }

	    protected bool ShowRewardAdmobAd()
        {
#if DEFINE_ADMOB
            if (IsEnableAdmobAd())
            {
                if (rewardBasedVideo.IsLoaded())
                {
                    rewardBasedVideo.Show();
                    return true;
                }
                else
                {
                    RequestRewardAdmob();
                }
            }
#endif
            return false;
        }


        public Action RewardBasedVideo_OnAdLeavingApplication;
        public Action RewardBasedVideo_OnAdLoaded;
        public Action RewardBasedVideo_OnAdOpening;
        public Action RewardBasedVideo_OnAdStarted;
#if DEFINE_ADMOB
        private void RewardBasedVideo_HandleAdStarted(object sender, EventArgs e)
        {
            if (RewardBasedVideo_OnAdStarted != null)
            {
                RewardBasedVideo_OnAdStarted();
            }
        }

        private void RewardBasedVideo_HandleAdRewarded(object sender, Reward e)
        {
	        if (OnRewardedAdCompleted != null)
	        {
		        OnRewardedAdCompleted(ERewardedAdNetwork.AdMob);
	        }
            RequestRewardAdmob();
        }

        private void RewardBasedVideo_HandleAdClosed(object sender, EventArgs e)
        {
            if (OnRewardedAdSkiped != null)
            {
                OnRewardedAdSkiped(ERewardedAdNetwork.AdMob);
            }
            RequestRewardAdmob();
        }

        private void RewardBasedVideo_HandleAdOpening(object sender, EventArgs e)
        {
            if (RewardBasedVideo_OnAdOpening != null)
            {
                RewardBasedVideo_OnAdOpening();
            }
        }

        private void RewardBasedVideo_HandleAdLoaded(object sender, EventArgs e)
        {
            if (RewardBasedVideo_OnAdLoaded != null)
            {
                RewardBasedVideo_OnAdLoaded();
            }
        }

        private void RewardBasedVideo_HandleAdLeavingApplication(object sender, EventArgs e)
        {
            if (RewardBasedVideo_OnAdLeavingApplication != null)
            {
                RewardBasedVideo_OnAdLeavingApplication();
            }
        }

        private void RewardBasedVideo_HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            if (OnRewardedAdFailed != null)
            {
	            OnRewardedAdFailed(ERewardedAdNetwork.AdMob);
            }
            RequestRewardAdmob();
        }
#endif
        #endregion

        #region Banner Ad

        private void InitBannerAdmob()
	    {
#if DEFINE_ADMOB
            //RequestBannerAdmob();
#endif
        }

	    private void RequestBannerAdmob()
	    {
#if DEFINE_ADMOB
		    string id = "";

		    if(adConfigData != null && adConfigData.admobConfig != null)
			{
			    id = adConfigData.admobConfig.GetBannerId();
		    }
			if(!string.IsNullOrEmpty(id))
			{
				Debug.Log("AdsManager.Admob.RequestBannerAdmob id: " + id);
				bannerViewAdmob = new BannerView(id, AdSize.Banner, (AdPosition)adConfigData.AdPosition);
				bannerViewAdmob.LoadAd(adRequestAdmob);
			}
#endif
	    }

        protected bool IsBannerAdAdmobReady()
        {
#if DEFINE_ADMOB
            RequestBannerAdmob();

            if (bannerViewAdmob != null)
            {
				Debug.Log("AdsManager.Admob.IsBannerAdAdmobReady");
                return true;
            }
#endif
            return false;
        }

        protected bool ShowBannerAdAdmob()
        {
#if DEFINE_ADMOB
            if (IsEnableAdmobAd())
            {
                RequestBannerAdmob();

                if (bannerViewAdmob != null)
                {
                    Debug.Log("AdsManager.Admob.ShowBannerAdAdmob");
                    bannerViewAdmob.Show();
                    return true;
                }
            }
#endif
            return false;
        }

	    protected bool HideBannerAdAdmob()
        {
#if DEFINE_ADMOB
            if (IsEnableAdmobAd())
            {
                if (bannerViewAdmob != null)
                {
                    bannerViewAdmob.Hide();
                    return true;
                }
            }
#endif
	        return false;
        }

        protected bool DestroyBannerAdAdmob()
	    {
#if DEFINE_ADMOB
		    if(IsEnableAdmobAd() && bannerViewAdmob != null)
		    {
		    	bannerViewAdmob.Hide();
		    	bannerViewAdmob.Destroy();
                return true;
		    }
#endif
            return false;
        }
#endregion
    }
}
