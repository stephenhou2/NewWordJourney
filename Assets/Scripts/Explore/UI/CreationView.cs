using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using UnityEngine.UI;
	using System.Text;

	public class CreationView : MonoBehaviour
    {
		public Transform characterFragmentsContainer;

		public Transform characterFragmentModel;

		public InstancePool characterFragmentPool;

		public Transform createButton;

		private CallBack spellCallBack;
              
		public void InitCharacterFragmentsHUD(CallBack spellCallBack){
			this.spellCallBack = spellCallBack;
			createButton.gameObject.SetActive(false);
		}

		private void UpdateCharacterFragmentsButton(){

			createButton.gameObject.SetActive(CharactersEnoughToSpell());

		}

		public void HideCreateButton(){
			createButton.gameObject.SetActive(false);
		}

		public void ClearCharacterFragments(){
			characterFragmentPool.AddChildInstancesToPool(characterFragmentsContainer);
		}

		public void UpdateCharactersCollected(){

			ClearCharacterFragments();

			for (int i = 0; i < Player.mainPlayer.allCollectedCharacters.Count;i++){

				char character = Player.mainPlayer.allCollectedCharacters[i];

				Image characterFragment = characterFragmentPool.GetInstance<Image>(characterFragmentModel.gameObject, characterFragmentsContainer);

				string characterSpriteName = string.Format("character_{0}", character);

				Sprite characterSprite = GameManager.Instance.gameDataCenter.allCHaracterSprites.Find(delegate (Sprite obj)
				{
					return obj.name == characterSpriteName;
				});

				characterFragment.sprite = characterSprite;
			}

			UpdateCharacterFragmentsButton();         
		}

		public void OnSpellButtonClick(){

			ExploreManager.Instance.MapWalkableEventsStopAction();

			if(CharactersEnoughToSpell() && spellCallBack != null){
				spellCallBack();
			}

		}

		private bool CharactersEnoughToSpell(){

			bool canSpell = false;

			SpellItem spellItem = ExploreManager.Instance.newMapGenerator.spellItemOfCurrentLevel;

			List<char> charactersCollected = Player.mainPlayer.allCollectedCharacters;
            

			char[] charactersNeed = spellItem.spell.ToCharArray();


			//Debug.Log(spellItem.spell);

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

