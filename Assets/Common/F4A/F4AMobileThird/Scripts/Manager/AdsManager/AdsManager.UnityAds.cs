using UnityEngine;

#if DEFINE_UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace com.F4A.MobileThird
{
    /// <summary>
    /// 
    /// </summary>
	public partial class AdsManager
    {
        private const string VideoUnityZone = "video";
	    private const string RewardedVideoUnityZone = "rewardedVideo";
	    private const string BannerUnityZone = "banner";
	    
        private void InitializationUnityAds()
	    {
		    RequestUnityAds();
        }

        private bool IsEnableUnityAd()
        {
            return adConfigData != null && adConfigData.unityAdConfig != null && adConfigData.unityAdConfig.enableAd;
        }

        private void RequestUnityAds()
        {
#if DEFINE_UNITY_ADS
            if (IsEnableUnityAd())
            {
                string id = adConfigData.unityAdConfig.GetIdAd();
                if (!string.IsNullOrEmpty(id))
                {
                    Debug.Log("RequestUnityAds id: " + id);
                    //------------------ initialize Unity Ads. ----------------------//
                    if (Advertisement.isSupported)
                    { // If the platform is supported,
                        Advertisement.Initialize(id);
                    }
                }
            }
#endif
        }

        protected bool IsRewardedUnityAdReady()
        {
#if DEFINE_UNITY_ADS
            if (IsEnableUnityAd() && Advertisement.IsReady(RewardedVideoUnityZone))
            {
                return true;
            }
#endif
            return false;
        }

        protected bool ShowRewardUnityAd()
        {
#if DEFINE_UNITY_ADS
            if (IsRewardedUnityAdReady())
            {
                Advertisement.Show(RewardedVideoUnityZone, new ShowOptions()
                {
                    resultCallback = result =>
                    {
                        switch (result)
                        {
                            case ShowResult.Finished:
	                            if (OnRewardedAdCompleted != null)
                                {
		                            OnRewardedAdCompleted(ERewardedAdNetwork.UnityAds);
                                }
                                break;
                            case ShowResult.Skipped:
                                if (OnRewardedAdSkiped != null)
                                {
                                    OnRewardedAdSkiped(ERewardedAdNetwork.UnityAds);
                                }
                                break;
                            case ShowResult.Failed:
                                if (OnRewardedAdFailed != null)
                                {
                                    OnRewardedAdFailed(ERewardedAdNetwork.UnityAds);
                                }
                                break;
                        }
                    }
                });
                return true;
            }
            return false;
#else
            return false;
#endif
        }

        protected bool IsVideoUnityAdReady()
        {
#if DEFINE_UNITY_ADS
            if (IsEnableUnityAd() && Advertisement.IsReady(VideoUnityZone.ToString()))
            {
                return true;
            }
#endif

            return false;
        }

        protected bool ShowVideoUnityAd()
        {
#if DEFINE_UNITY_ADS
            if (IsVideoUnityAdReady())
            {
                Advertisement.Show(VideoUnityZone.ToString(), new ShowOptions()
                {
                    resultCallback = result =>
                    {
                        switch (result)
                        {
                            case ShowResult.Finished:
                                if (OnVideodAdCompleted != null)
                                {
                                    OnVideodAdCompleted(EVideoAdNetwork.UnityAds);
                                }
                                break;
                            case ShowResult.Skipped:
                                break;
                            case ShowResult.Failed:
                                break;
                        }
                    }
                });
                return true;
            }
            return false;
#else
            return false;
#endif
        }

        protected bool IsInterstitialUnityAdReady()
        {
#if DEFINE_UNITY_ADS
            if (IsEnableUnityAd())
            {
                if (Advertisement.IsReady(VideoUnityZone.ToString()))
                {
                    return true;
                }
                else if (Advertisement.IsReady(RewardedVideoUnityZone.ToString()))
                {
                    return true;
                }
            }
#endif
            return false;
        }

        protected bool ShowInterstitialUnityAd()
        {
#if DEFINE_UNITY_ADS
            if (IsVideoUnityAdReady())
            {
                Advertisement.Show(VideoUnityZone.ToString(), new ShowOptions()
                {
                    resultCallback = result =>
                    {
                        switch (result)
                        {
                            case ShowResult.Finished:
                                if (OnInterstitialAdClosed != null)
                                {
                                    OnInterstitialAdClosed(EInterstitialAdNetwork.UnityAds);
                                }
                                break;
                            case ShowResult.Skipped:
                                break;
                            case ShowResult.Failed:
                                break;
                        }
                    }
                });
                return true;
            }
            else if (IsRewardedUnityAdReady())
            {
                Advertisement.Show(RewardedVideoUnityZone, new ShowOptions()
                {
                    resultCallback = result =>
                    {
                        switch (result)
                        {
                            case ShowResult.Finished:
                                if (OnInterstitialAdClosed != null)
                                {
                                    OnInterstitialAdClosed(EInterstitialAdNetwork.UnityAds);
                                }
                                break;
                            case ShowResult.Skipped:
                                break;
                            case ShowResult.Failed:
                                break;
                        }
                    }
                });
                return true;
            }
#endif
            return false;
        }
        
	    protected bool IsBannerUnityAdReady()
	    {
#if DEFINE_UNITY_ADS
		    if (IsEnableUnityAd() && Advertisement.IsReady(BannerUnityZone.ToString()))
		    {
			    return true;
		    }
#endif
		    return false;
	    }
	    
	    protected bool ShowBannerUnityAd()
	    {
#if DEFINE_UNITY_ADS
		    if (IsBannerUnityAdReady())
		    {
			    Advertisement.Show(BannerUnityZone.ToString(), new ShowOptions()
			    {
				    resultCallback = result =>
				    {
					    switch (result)
					    {
						    case ShowResult.Finished:
							    break;
						    case ShowResult.Skipped:
							    break;
						    case ShowResult.Failed:
							    break;
					    }
				    }
                });
			    return true;
		    }
#endif
		    return false;
	    }
    }
}