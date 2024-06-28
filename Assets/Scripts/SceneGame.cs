using PathologicalGames;
using System.Collections;
using UnityEngine;

public class SceneGame : SceneClass
{
	private GameObject ObjPoolGameCharacterEffectGretel;

	private GameObject ObjPoolGameCharacterEffectWitch;

	private GameObject ObjPoolGamePlaying;

	public GameObject PrefabGamePlayingPool;

	public override IEnumerator OnSceneShow()
	{
		Application.targetFrameRate = GlobalSetting.LOW_FPS;
		ObjPoolGamePlaying = Object.Instantiate(PrefabGamePlayingPool);
		if (MonoSingleton<SceneControlManager>.Instance.OldSceneType == SceneType.MapTool)
		{
			GameMain.main.StartProto(MapData.main);
			tk2dCamera component = Camera.main.GetComponent<tk2dCamera>();
			if ((bool)component)
			{
				Vector2 targetResolution = component.TargetResolution;
				if (targetResolution.x == 1900f)
				{
					Vector2 targetResolution2 = component.TargetResolution;
					if (targetResolution2.y == 900f)
					{
						Camera.main.GetComponent<tk2dCamera>().CameraSettings.orthographicPixelsPerMeter = 0.7f;
					}
				}
			}
		}
		else
		{
			GameMain.main.StartProto(MapData.main.gid);
		}
		
		yield return null;
	}

	public override void OnSceneHideStart()
	{
		BoardManager.main.RemoveBoard();
		if (PoolManager.Pools != null && PoolManager.Pools.Count > 0)
		{
			if (PoolManager.IsEnablePoolGameEffect)
			{
				PoolManager.PoolGameEffect.DespawnAll();
			}
			if (PoolManager.IsEnablePoolGameBlocks)
			{
				PoolManager.PoolGameBlocks.DespawnAll();
			}
		}
	}

	public override void OnSceneHideEnd()
	{
		if ((bool)ObjPoolGamePlaying)
		{
			UnityEngine.Object.Destroy(ObjPoolGamePlaying);
		}
		if ((bool)ObjPoolGameCharacterEffectGretel)
		{
			UnityEngine.Object.Destroy(ObjPoolGameCharacterEffectGretel);
		}
		if ((bool)ObjPoolGameCharacterEffectWitch)
		{
			UnityEngine.Object.Destroy(ObjPoolGameCharacterEffectWitch);
		}
	}
}
