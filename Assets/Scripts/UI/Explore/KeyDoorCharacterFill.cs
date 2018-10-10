using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class KeyDoorCharacterFill : MonoBehaviour
    {
		public Text characterFillText;

		public char fillCharacter;

		private bool mFix;
		public bool fix{
			get{
				return mFix;
			}
		}

		public void SetUpKeyDoorCharacterFill(char character,bool fix){

			characterFillText.text = character.ToString();

			mFix = fix;

			fillCharacter = character;

		}

		public void Reset()
		{         
			mFix = false;

			fillCharacter = ' ';

			characterFillText.text = string.Empty;
		}

	}
}

