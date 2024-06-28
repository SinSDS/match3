using UnityEngine;

namespace cookapps.sr.maptool
{
	public class MapToolSelectObjectRescueBear : MapToolSelectObject
	{
		public override void Start()
		{
			base.Start();
		}

		public override void OnEnable()
		{
			base.OnEnable();
		}

		public override void SelectItem(GameObject selectedItem)
		{
			string spriteKeyName = selectedItem.name;
			if (selectedItem.name.Substring(0, 2) == "RB")
			{
				spriteKeyName = "RB";
			}
			MonoSingleton<MapToolManager>.Instance.OnSelectItem(menuType, spriteKeyName);
			MonoSingleton<MapToolManager>.Instance.SelectedSpriteKey = selectedItem.name;
		}
	}
}
