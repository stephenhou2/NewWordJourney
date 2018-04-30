using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class WordItemView : CellDetailView {

		public Text spellText;
		public Text explainationText;
		public Button pronounceButton;

		public override void SetUpCellDetailView (object data)
		{
			HLHWord word = data as HLHWord;

			spellText.text = word.spell;

			explainationText.text = word.explaination;

			pronounceButton.onClick.RemoveAllListeners ();

			pronounceButton.onClick.AddListener (delegate {
				GameManager.Instance.pronounceManager.CancelPronounce();
				GameManager.Instance.pronounceManager.PronounceWord (word);
			});

		}

	}
}
