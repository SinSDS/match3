using UnityEngine;

public class IAPPayLogger
{
	private static readonly string URL = "https://beagles.cookappsgames.com/jewel_blast/payment";

	public static void SendIAPLog(string order_id, string price, string device_id, string receipt)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("device", "android");
		wWWForm.AddField("device_id", device_id);
		wWWForm.AddField("price", price);
		wWWForm.AddField("order_id", order_id);
		wWWForm.AddField("receipt", receipt);
		WWW wWW = new WWW(URL, wWWForm);
	}
}
