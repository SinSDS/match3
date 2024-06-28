using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupInGameItemStore : Popup
{
	public List<ItemsInGameStore> listItems = new List<ItemsInGameStore>();

	public ItemsInGameStore ObjBaseItem;

	public Text TextTitle;

	public Text TextDesc;

	public override void OnEnable()
	{
		base.OnEnable();
	}

	private void Caching()
	{
	}

	public void SetPopup(Booster.BoosterType boosterType, bool isTutorial = false)
	{
		Caching();
		if (!isTutorial)
		{
			MonoSingleton<UIManager>.Instance.SetCoinCurrencyMenuLayer(isPopupOverLayer: true);
		}
		switch (boosterType)
		{
		case Booster.BoosterType.Hammer:
			TextTitle.text = "Magic Hammer";
			TextDesc.text = "Magic Hammer can break a gem or an obstacle.";
			break;
		case Booster.BoosterType.CandyPack:
			TextTitle.text = "Gem Bomb";
			TextDesc.text = "You can remove all of the gems and obstacles.";
			break;
		case Booster.BoosterType.HBomb:
			TextTitle.text = "Horizontal Bomb";
			TextDesc.text = "You can removes any gems that are either horizontally.";
			break;
		case Booster.BoosterType.VBomb:
			TextTitle.text = "Vertical Bomb";
			TextDesc.text = "You can removes any gems that are either vertically.";
			break;
		}
		listItems.Clear();
		int boosterItemIndex = MonoSingleton<ServerDataTable>.Instance.GetBoosterItemIndex(boosterType);
		if (!MonoSingleton<PlayerDataManager>.Instance.dicBoosterItemList.ContainsKey(boosterItemIndex))
		{
			return;
		}
		for (int i = 0; i < MonoSingleton<PlayerDataManager>.Instance.dicBoosterItemList[boosterItemIndex].Count; i++)
		{
			if (MonoSingleton<ServerDataTable>.Instance.m_dicTableItemShop.ContainsKey(MonoSingleton<PlayerDataManager>.Instance.dicBoosterItemList[boosterItemIndex][i]))
			{
				GameObject gameObject;
				if (i > 0)
				{
					gameObject = Object.Instantiate(ObjBaseItem.gameObject);
					gameObject.transform.SetParent(ObjBaseItem.transform.parent, worldPositionStays: false);
				}
				else
				{
					gameObject = ObjBaseItem.gameObject;
				}
				gameObject.GetComponent<ItemsInGameStore>().SetItem(MonoSingleton<ServerDataTable>.Instance.m_dicTableItemShop[MonoSingleton<PlayerDataManager>.Instance.dicBoosterItemList[boosterItemIndex][i]], boosterType, i);
				listItems.Add(gameObject.GetComponent<ItemsInGameStore>());
			}
		}
	}

	public override void OnEventClose()
	{
		base.OnEventClose();
		MonoSingleton<UIManager>.Instance.HideCoinCurrentMenuLayer();
	}
}
