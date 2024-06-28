using UnityEngine;

namespace cookapps.sr.maptool
{
	public class GeneratorSpecialDropItems : MonoBehaviour
	{
		public void OnEndEdit(string text)
		{
			base.transform.parent.parent.GetComponent<GeneratorSpecialDropList>().OnChangeItemsProb(base.gameObject.name, text);
		}
	}
}
