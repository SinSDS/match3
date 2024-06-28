#if FACEBOOK_SDK
using Facebook.MiniJSON;
using Facebook.Unity;
using JsonFx.Json;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FacebookManager : MonoSingleton<FacebookManager>
{
	public static string fbAccessToken;

	public static string FBName = string.Empty;

	public static string FBFullName = string.Empty;

	public static bool IsFacebookLoginUserControl;

	public static bool IsSyncFacebookGameServer;

	private readonly List<int> alreadyPosted = new List<int>();

	[HideInInspector]
	public bool alreadyPostedInOneShare;

	private Action CallAPI_AppRequest_CallBack;

	private string CallAPI_AppRequest_SendType;

	private Action<FB_Friends> CallAPI_friends_CallBack;

	private Action<FB_Friends> CallAPI_invitable_friends_CallBack;

	private readonly Dictionary<RawImage, RawImage> dicLoadProfileImages = new Dictionary<RawImage, RawImage>();

	[HideInInspector]
	public bool explicitlyPosting;

	[HideInInspector]
	public bool IsInitTrying;

	private bool isLoadingProfileImage;

	[HideInInspector]
	public bool IsLogin;

	public bool Permission_Publish_Actions;

	public bool Permission_User_Friends;

	public override void Awake()
	{
		base.Awake();
		Permission_Publish_Actions = false;
		Permission_User_Friends = false;
		IsLogin = false;
		fbAccessToken = null;
#if FACEBOOK_SDK
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
		}
		else
		{
			FB.Init(delegate
			{
				FB.ActivateApp();
			});
		}
#endif
	}

	public void ReserveDownloadProfileImage(string fbID, string url, RawImage targetImage)
	{
		if (!string.IsNullOrEmpty(fbID))
		{
			StopCoroutine(ProgressLoadProfileImage(targetImage, fbID, url));
			CancelReserveDownloadProfileImage(targetImage);
			if (!dicLoadProfileImages.ContainsKey(targetImage))
			{
				dicLoadProfileImages.Add(targetImage, targetImage);
			}
			StartCoroutine(ProgressLoadProfileImage(targetImage, fbID, url));
		}
	}

	public void CancelReserveDownloadProfileImage(RawImage targetImage)
	{
		if (dicLoadProfileImages.ContainsKey(targetImage))
		{
			dicLoadProfileImages.Remove(targetImage);
		}
	}

	private bool CanIDownloadImage()
	{
		return !isLoadingProfileImage;
	}

	public string GetPictureURL(string facebookID, int? width = default(int?), int? height = default(int?), string type = null, bool secure = true)
	{
		string text = "https://graph.facebook.com/" + facebookID + "/picture";
		string str = (!width.HasValue) ? string.Empty : ("&width=" + width);
		str += ((!height.HasValue) ? string.Empty : ("&height=" + height));
		str += ((type == null) ? string.Empty : ("&type=" + type));
		str = str + "&access_token=" + fbAccessToken;
		if (str != string.Empty)
		{
			text = text + "?g" + str;
		}
		return text;
	}

	private IEnumerator ProgressLoadProfileImage(RawImage targetImage, string fbID, string url)
	{
		yield return StartCoroutine(Utils.WaitFor(CanIDownloadImage, 0.1f));
		if (dicLoadProfileImages.ContainsKey(targetImage))
		{
			isLoadingProfileImage = true;
			string fileURL = Application.persistentDataPath + "/" + fbID + ".jpg";
			if (File.Exists(fileURL))
			{
				WWW wwwFile = new WWW("file:///" + fileURL);
				try
				{
					yield return wwwFile;
					if (wwwFile.texture != null && (bool)targetImage && dicLoadProfileImages.ContainsKey(targetImage) && dicLoadProfileImages[targetImage] != null)
					{
						dicLoadProfileImages[targetImage].texture = wwwFile.texture;
					}
				}
				finally
				{
				}
			}
			else
			{
				WWW www = new WWW(url);
				try
				{
					yield return www;
					if (string.IsNullOrEmpty(www.error))
					{
						if (www.texture != null && (bool)targetImage && dicLoadProfileImages.ContainsKey(targetImage) && dicLoadProfileImages[targetImage] != null)
						{
							dicLoadProfileImages[targetImage].texture = www.texture;
						}
						try
						{
							File.WriteAllBytes(fileURL, www.bytes);
						}
						catch (Exception)
						{
						}
					}
				}
				finally
				{
				}
			}
			if (dicLoadProfileImages.ContainsKey(targetImage))
			{
				dicLoadProfileImages.Remove(targetImage);
			}
			isLoadingProfileImage = false;
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
#if FACEBOOK_SDK
			if (FB.IsInitialized)
			{
				FB.ActivateApp();
			}
			else
			{
				FB.Init(delegate
				{
					FB.ActivateApp();
				});
			}
#endif
		}
	}

	public void CallFBInit()
	{
		if (!IsInitTrying)
		{
			IsInitTrying = true;
			IsFacebookLoginUserControl = false;
#if FACEBOOK_SDK
			FB.Init(OnInitComplete, OnHideUnity);
#endif
		}
	}

	private void OnInitComplete()
	{
#if FACEBOOK_SDK
		IsLogin = FB.IsLoggedIn;
		AppEventManager.m_TempBox.dateTimeLoadGame = DateTime.Now;
        if (IsLogin)
        {
            MonoSingleton<AppEventManager>.Instance.startTimeAfterLogin = Utils.ConvertToTimestamp(DateTime.UtcNow);
            CheckPermissions_Init();
            IsSyncFacebookGameServer = false;
            fbAccessToken = AccessToken.CurrentAccessToken.TokenString;
        }
        else
        {
            IsLogin = false;
        }
		AndroidNativeUtils.CheckFBPushOpen();
#endif
    }

    private void OnHideUnity(bool isGameShown)
	{
	}

	private void Login_All_OK()
	{
#if FACEBOOK_SDK
		if (AccessToken.CurrentAccessToken != null)
		{
			fbAccessToken = AccessToken.CurrentAccessToken.TokenString;
		}
		IsSyncFacebookGameServer = true;
		MonoSingleton<UIManager>.Instance.ShowLoading();
		MonoSingleton<GameDataLoadManager>.Instance.StartLoadData();
#endif
    }

    private void CheckPermissions_Init()
	{
#if FACEBOOK_SDK
		FB.API("/me/permissions", HttpMethod.GET, delegate(IGraphResult response)
		{
			if (response.Error == null)
			{
				MonoBehaviour.print("response.Text = " + response.RawResult);
				if (response.RawResult.Contains("{\"permission\":\"publish_actions\",\"status\":\"granted\"}"))
				{
					MonoBehaviour.print("Permission_Publish_Actions OK");
					Permission_Publish_Actions = true;
				}
				if (response.RawResult.Contains("{\"permission\":\"user_friends\",\"status\":\"granted\"}"))
				{
					MonoBehaviour.print("Permission_User_Friends OK");
					Permission_User_Friends = true;
				}
			}
		});
		CallAPI_GetPlayerName(CallAPI_GetPlayerNameCallBack);
		CallAPI_GetUserFullName(AccessToken.CurrentAccessToken.UserId, CallAPI_GetUserFullNameCallBack);
#endif
	}

	public void CallFBLogout()
	{
#if FACEBOOK_SDK
		IsFacebookLoginUserControl = false;
		FB.LogOut();
		Permission_Publish_Actions = false;
		Permission_User_Friends = false;
		IsLogin = false;
		IsInitTrying = false;
		fbAccessToken = null;
#endif
	}

	public void GetPermission_PublishActions()
	{
#if FACEBOOK_SDK
		AppEventManager.m_TempBox.permissionPublishActions = Permission_Publish_Actions;
		MonoSingleton<AppEventManager>.Instance.SendAppEventGetPermissionPublish(AppEventManager.GetPermissionActionType.Impression);
		List<string> list = new List<string>();
		list.Add("publish_actions");
		FB.LogInWithPublishPermissions(list, delegate(ILoginResult result)
		{
			if (result.Error == null)
			{
				FB.API("/me/permissions", HttpMethod.GET, delegate(IGraphResult response)
				{
					if (response.Error == null)
					{
						MonoBehaviour.print("response.Text = " + response.RawResult);
						if (response.RawResult.Contains("{\"permission\":\"publish_actions\",\"status\":\"granted\"}"))
						{
							MonoBehaviour.print("Permission_Publish_Actions OK");
							Permission_Publish_Actions = true;
							if (!AppEventManager.m_TempBox.permissionPublishActions)
							{
								MonoSingleton<AppEventManager>.Instance.SendAppEventGetPermissionPublish(AppEventManager.GetPermissionActionType.Response_Confirm);
							}
						}
						else if (!AppEventManager.m_TempBox.permissionPublishActions)
						{
							MonoSingleton<AppEventManager>.Instance.SendAppEventGetPermissionPublish(AppEventManager.GetPermissionActionType.Response_Later);
						}
					}
					MonoSingleton<UIManager>.Instance.HideLoading();
				});
			}
			MonoSingleton<UIManager>.Instance.HideLoading();
		});
#endif
	}

	public void GetPermission_UserFriends(AppEventManager.GetPermissionAccessedBy accessedBy)
	{
#if FACEBOOK_SDK
		AppEventManager.m_TempBox.permissionUserFriends = Permission_User_Friends;
		AppEventManager.m_TempBox.permissionAccessedBy = accessedBy;
		MonoSingleton<AppEventManager>.Instance.SendAppEventGetPermissionFriendList(AppEventManager.GetPermissionActionType.Impression);
		List<string> list = new List<string>();
		list.Add("user_friends");
		FB.LogInWithReadPermissions(list, delegate(ILoginResult result)
		{
			if (result.Error == null)
			{
				FB.API("/me/permissions", HttpMethod.GET, delegate(IGraphResult response)
				{
					if (response.Error == null)
					{
						MonoBehaviour.print("response.Text = " + response.RawResult);
						if (response.RawResult.Contains("{\"permission\":\"user_friends\",\"status\":\"granted\"}"))
						{
							MonoBehaviour.print("Permission_User_Friends OK");
							Permission_User_Friends = true;
							if (!AppEventManager.m_TempBox.permissionUserFriends)
							{
								MonoSingleton<AppEventManager>.Instance.SendAppEventGetPermissionFriendList(AppEventManager.GetPermissionActionType.Response_Confirm);
							}
						}
						else if (!AppEventManager.m_TempBox.permissionUserFriends)
						{
							MonoSingleton<AppEventManager>.Instance.SendAppEventGetPermissionFriendList(AppEventManager.GetPermissionActionType.Response_Later);
						}
					}
					MonoSingleton<UIManager>.Instance.HideLoading();
				});
			}
			MonoSingleton<UIManager>.Instance.HideLoading();
		});
#endif
	}

	public void CallFBLogin(AppEventManager.FacebookLoginFromWhere fbLoginWhere = AppEventManager.FacebookLoginFromWhere.ETC)
	{
#if FACEBOOK_SDK
		IsFacebookLoginUserControl = true;
		AppEventManager.m_TempBox.fbLoginWhere = fbLoginWhere;
		AppEventManager.m_TempBox.permissionUserFriends = Permission_User_Friends;
		AppEventManager.m_TempBox.permissionAccessedBy = AppEventManager.GetPermissionAccessedBy.AcceptDialog;
		MonoSingleton<AppEventManager>.Instance.SendAppEventGetPermissionFriendList(AppEventManager.GetPermissionActionType.Impression);
		List<string> list = new List<string>();
		list.Add("user_friends");
		FB.LogInWithReadPermissions(list, LoginCallback);
#endif
	}

#if FACEBOOK_SDK
	private void LoginCallback(ILoginResult result)
	{
		if (result.Error == null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
			if (dictionary.ContainsKey("error_code") || dictionary.ContainsKey("error"))
			{
				MonoSingleton<PopupManager>.Instance.OpenCommonPopup(PopupType.PopupCommonAlarm, "Error", "Facebook login failed");
				MonoSingleton<UIManager>.Instance.HideLoading();
				return;
			}
			if (dictionary.ContainsKey("cancelled"))
			{
				MonoSingleton<UIManager>.Instance.HideLoading();
				return;
			}
			IsLogin = FB.IsLoggedIn;
			if (IsLogin)
			{
				MonoSingleton<AppEventManager>.Instance.startTimeAfterLogin = Utils.ConvertToTimestamp(DateTime.UtcNow);
				CheckPermissions_CallFBLogin();
			}
		}
		else
		{
			MonoSingleton<UIManager>.Instance.HideLoading();
		}
    }
#endif

    private void CheckPermissions_CallFBLogin()
	{
#if FACEBOOK_SDK
		FB.API("/me/permissions", HttpMethod.GET, delegate(IGraphResult response)
		{
			if (response.Error == null)
			{
				MonoBehaviour.print("response.Text = " + response.RawResult);
				if (response.RawResult.Contains("{\"permission\":\"publish_actions\",\"status\":\"granted\"}"))
				{
					MonoBehaviour.print("Permission_Publish_Actions OK");
					Permission_Publish_Actions = true;
				}
				if (response.RawResult.Contains("{\"permission\":\"user_friends\",\"status\":\"granted\"}"))
				{
					MonoBehaviour.print("Permission_User_Friends OK");
					Permission_User_Friends = true;
					if (!AppEventManager.m_TempBox.permissionUserFriends)
					{
						MonoSingleton<AppEventManager>.Instance.SendAppEventGetPermissionFriendList(AppEventManager.GetPermissionActionType.Response_Confirm);
					}
				}
				else if (!AppEventManager.m_TempBox.permissionUserFriends)
				{
					MonoSingleton<AppEventManager>.Instance.SendAppEventGetPermissionFriendList(AppEventManager.GetPermissionActionType.Response_Later);
				}
			}
			Login_All_OK();
		});
		CallAPI_GetPlayerName(CallAPI_GetPlayerNameCallBack);
		CallAPI_GetUserFullName(AccessToken.CurrentAccessToken.UserId, CallAPI_GetUserFullNameCallBack);
#endif
	}

	public void CallAPI_invitable_friends(Action<FB_Friends> _callAPI_invitable_friends_CallBack)
	{
#if FACEBOOK_SDK
		CallAPI_invitable_friends_CallBack = _callAPI_invitable_friends_CallBack;
		FB.API("/me/invitable_friends?fields=id,name,first_name,picture&width=50&height=50&offset=0&limit=1000", HttpMethod.GET, CallBack_invitable_friends);
#endif
	}

#if FACEBOOK_SDK
	private void CallBack_invitable_friends(IGraphResult result)
	{
		if (result != null && result.Error == null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
			if (!dictionary.ContainsKey("error_code") && !dictionary.ContainsKey("cancelled") && !dictionary.ContainsKey("error"))
			{
				FB_Friends obj = JsonReader.Deserialize<FB_Friends>(result.RawResult);
				if (CallAPI_invitable_friends_CallBack != null)
				{
					CallAPI_invitable_friends_CallBack(obj);
				}
			}
		}
		MonoSingleton<UIManager>.Instance.HideLoading();
	}
#endif

	public void CallAPI_friends_ForGameInfo()
	{
#if FACEBOOK_SDK
		FB.API("/me/friends?fields=id,name,first_name,picture&offset=0&limit=1000", HttpMethod.GET, CallBack_friends_ForGameInfo);
#endif
	}

#if FACEBOOK_SDK
	private void CallBack_friends_ForGameInfo(IGraphResult result)
	{
		if (result == null || result.Error != null)
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
		if (!dictionary.ContainsKey("error_code") && !dictionary.ContainsKey("cancelled") && !dictionary.ContainsKey("error"))
		{
			FB_Friends fB_Friends = JsonReader.Deserialize<FB_Friends>(result.RawResult);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < fB_Friends.data.Length; i++)
			{
				stringBuilder.Append(fB_Friends.data[i].id);
				stringBuilder.Append(",");
			}
		}
	}
#endif

	public void CallAPI_friends(Action<FB_Friends> _callAPI_friends_CallBack)
	{
#if FACEBOOK_SDK
		CallAPI_friends_CallBack = _callAPI_friends_CallBack;
		FB.API("/me/friends?fields=id,name,first_name,picture&offset=0&width=50&height=50&limit=1000", HttpMethod.GET, CallBack_friends);
#endif
	}

#if FACEBOOK_SDK
	private void CallBack_friends(IGraphResult result)
	{
		if (result == null || result.Error != null)
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
		if (!dictionary.ContainsKey("error_code") && !dictionary.ContainsKey("cancelled") && !dictionary.ContainsKey("error"))
		{
			FB_Friends obj = JsonReader.Deserialize<FB_Friends>(result.RawResult);
			if (CallAPI_friends_CallBack != null)
			{
				CallAPI_friends_CallBack(obj);
			}
		}
	}
#endif

	public void CallAPI_AppRequest(string[] friends, string _title, string msg, string _sendType, Action _callAPI_AppRequest_CallBack)
	{
#if FACEBOOK_SDK
		CallAPI_AppRequest_CallBack = _callAPI_AppRequest_CallBack;
		FB.AppRequest(msg, friends, null, null, null, null, _title, CallBack_CallAPI_AppRequest);
#endif
	}

#if FACEBOOK_SDK
	private void CallBack_CallAPI_AppRequest(IAppRequestResult result)
	{
		if (result != null && result.Error == null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
			if (!dictionary.ContainsKey("error_code") && !dictionary.ContainsKey("cancelled") && !dictionary.ContainsKey("error"))
			{
				FB_Request fB_Request = JsonReader.Deserialize<FB_Request>(result.RawResult);
				if (CallAPI_AppRequest_CallBack != null)
				{
					CallAPI_AppRequest_CallBack();
				}
			}
		}
		MonoSingleton<UIManager>.Instance.HideLoading();
	}

	public void CallAPI_GetPlayerName(FacebookDelegate<IGraphResult> _callBack)
	{
		FBName = PlayerPrefs.GetString(AccessToken.CurrentAccessToken.UserId + "_FirstName", string.Empty);
		if (string.IsNullOrEmpty(FBName))
		{
			FB.API("/me?fields=first_name", HttpMethod.GET, _callBack);
		}
	}

	private void CallAPI_GetPlayerNameCallBack(IGraphResult result)
	{
		if (!string.IsNullOrEmpty(result.RawResult))
		{
			try
			{
				Dictionary<string, object> dictionary = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
				if (dictionary.ContainsKey("first_name"))
				{
					string text = dictionary["first_name"] as string;
					string text2 = dictionary["id"] as string;
					PlayerPrefs.SetString(text2 + "_FirstName", text);
					if (text2.Equals(AccessToken.CurrentAccessToken.UserId))
					{
						FBName = text;
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public void CallAPI_GetUserFullName(string fbID, FacebookDelegate<IGraphResult> _callBack)
	{
		FBFullName = PlayerPrefs.GetString(AccessToken.CurrentAccessToken.UserId + "_FullName", string.Empty);
		if (string.IsNullOrEmpty(FBFullName))
		{
			FB.API("/me?fields=name", HttpMethod.GET, _callBack);
		}
	}

	private void CallAPI_GetUserFullNameCallBack(IGraphResult result)
	{
		MonoBehaviour.print("CallAPI_GetUserFullNameCallBack : " + result.RawResult);
		Dictionary<string, object> dictionary = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
		if (dictionary.ContainsKey("name"))
		{
			string text = dictionary["name"] as string;
			string text2 = dictionary["id"] as string;
			PlayerPrefs.SetString(text2 + "_FullName", text);
			MonoBehaviour.print("fullName = " + text + ", id = " + text2);
			MonoBehaviour.print("AccessToken.CurrentAccessToken.UserId = " + AccessToken.CurrentAccessToken.UserId);
			if (text2.Equals(AccessToken.CurrentAccessToken.UserId))
			{
				FBFullName = text;
			}
		}
	}
#endif

	private IEnumerator waitSendAppEventPushRemoteNoti(string pushId, string title, string message, Dictionary<string, string> dict)
	{
		yield return new WaitForSeconds(1f);
		MonoSingleton<AppEventManager>.Instance.SendAppEventPushRemoteNoti(pushId, title, message, dict);
	}
}
