using UnityEngine;

public class ButtonLinkToCSUrl : MonoBehaviour
{
	private void Start()
	{
	}

	public void OnPressButton()
	{
		Application.OpenURL(MonoSingleton<ServerDataTable>.Instance.faqCsUrl);
	}
}
