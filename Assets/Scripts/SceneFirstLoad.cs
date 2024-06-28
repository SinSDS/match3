using UnityEngine;

public class SceneFirstLoad : SceneClass
{
	private void Start()
	{
		Application.targetFrameRate = GlobalSetting.FPS;
		GlobalSetting.LoadConfigData();
		MonoSingleton<SceneControlManager>.Instance.CurrentScene = this;
		MonoSingleton<SceneControlManager>.Instance.CurrentSceneType = SceneType.FirstLoad;
		MonoSingleton<UIManager>.Instance.HideCoinCurrentMenuLayer();
		MonoSingleton<ServerDataTable>.Instance.LoadLangTableFromLocalFile();
		MonoSingleton<GameDataLoadManager>.Instance.MoveToLobbyScene();
		MonoSingleton<GameDataLoadManager>.Instance.StartLoadData();
	}
}
