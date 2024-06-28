using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.F4A.MobileThird
{
	public partial class AdsManager {
		#region DEFINE_VUNGLE

		private bool InitVungle ()
		{
#if DEFINE_VUNGLE
			Debug.Log("InitVungle");
			string appID = "";
			#if UNITY_IOS
			appID = VungleIdIos;
			Dictionary<string, bool> placements = new Dictionary<string, bool>
			{
			{ VunglePlacementIos, false },
			};
			#elif UNITY_ANDROID
			appID = VungleIdAndroid;
			Dictionary<string, bool> placements = new Dictionary<string, bool> {
				{ VunglePlacementAndroid, false },
			};
			#elif UNITY_WSA_10_0 || UNITY_WINRT_8_1 || UNITY_METRO
			appID = VungleIdWindowPhone;
			Dictionary<string, bool> placements = new Dictionary<string, bool>
			{
			{ VunglePlacementWindowPhone, false },
			};
			#endif
			string[] array = new string[placements.Keys.Count];
			placements.Keys.CopyTo (array, 0);
			//Your App IDs can be found in the Vungle Dashboard on your apps' pages
			Vungle.init (appID, array);
			Vungle.onInitializeEvent += () => {
				Debug.Log("Initialize Vungle Success");
			};

			Vungle.onAdFinishedEvent += (placementID, args) => {
				Debug.Log ("Ad finished - placementID " + placementID + ", was call to action clicked:" + args.WasCallToActionClicked +  ", is completed view:"
					+ args.IsCompletedView);
				if(OnRewardedAdCompleted != null)
					OnRewardedAdCompleted(ERewardedAdNetwork.Vungle);
			};
#endif
			return true;
		}

		#endregion
	}
}