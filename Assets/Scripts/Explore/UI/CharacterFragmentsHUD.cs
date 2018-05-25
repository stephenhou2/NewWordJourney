using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using UnityEngine.UI;
	using System.Text;

	public class CharacterFragmentsHUD : MonoBehaviour
    {
		public Transform characterFragmentsContainer;

		public Transform characterFragmentsButton;

		public Text charactersText;

		private CallBack spellCallBack;

		public void InitCharacterFragmentsHUD(CallBack spellCallBack){
			this.spellCallBack = spellCallBack;
			characterFragmentsButton.gameObject.SetActive(false);
		}

		private void UpdateCharacterFragmentsButton(){

			characterFragmentsButton.gameObject.SetActive(CharactersEnoughToSpell());

		}

		public void UpdateCharactersCollected(){

			characterFragmentsContainer.gameObject.SetActive(true);

			StringBuilder stringBuilder = new StringBuilder();

			for (int i = 0; i < Player.mainPlayer.allCollectedCharacters.Count;i++){

				char character = Player.mainPlayer.allCollectedCharacters[i];

				stringBuilder.AppendFormat("{0} ", character);

			}

			if(stringBuilder.Length >= 2){
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}

			charactersText.text = stringBuilder.ToString();

			UpdateCharacterFragmentsButton();         
		}

		public void OnSpellButtonClick(){

			if(CharactersEnoughToSpell() && spellCallBack != null){
				spellCallBack();
			}

		}

		private bool CharactersEnoughToSpell(){

			bool canSpell = false;

			SpellItem spellItem = ExploreManager.Instance.newMapGenerator.spellItemOfCurrentLevel;

			List<char> charactersCollected = Player.mainPlayer.allCollectedCharacters;
            

			char[] charactersNeed = spellItem.spell.ToCharArray();


			Debug.Log(spellItem.spell);

			if(charactersNeed.Length != charactersCollected.Count){
				canSpell = false;
				return canSpell;
			}

			for (int i = 0; i < charactersNeed.Length;i++){

				char character = charactersNeed[i];

				if(charactersCollected.Contains(character)){
					canSpell = true;
					break;
				}            
			}

			return canSpell;
		}

        
    }

}

