using UnityEngine;
using UnityEngine.UI;

public class PopupLiteTutorial : Popup
{
	private string[] tutorialDesc = new string[8]
	{
		"Try to match 3 same Gems!",
		"Match the Gems within the Jail!",
		"Match the Gems around the Blocks.",
		"Match the Gems on the Tile.",
		"Match the Gems on the Gold to spread it.",
		"Bring the Crowns down to the bottom of the board.",
		"Find the Holy Grail behind the Tiles!",
		"Match the Gems & Find the Silvers!"
	};

	private string[] tutorialTitle = new string[0];

	public GameObject[] ObjTutorailGroups;

	public Text TextTutorialTitle;

	public Text TextTutorialDesc;

	public void SetData(int tutorialIndex)
	{
		ObjTutorailGroups[tutorialIndex].SetActive(value: true);
		TextTutorialDesc.text = tutorialDesc[tutorialIndex];
	}
}
