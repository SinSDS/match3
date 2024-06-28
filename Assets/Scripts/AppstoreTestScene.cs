using UnityEngine;

public class AppstoreTestScene : MonoBehaviour
{
	public string m_appID_IOS = "840066184";

	public string m_appID_Android = "com.orangenose.noone";

	public void OnButtonClick(string buttonName)
	{
		if (buttonName == "ViewApp" && !Application.isEditor)
		{
			MonoSingleton<AppstoreHandler>.Instance.openAppInStore(m_appID_Android);
		}
	}
}
