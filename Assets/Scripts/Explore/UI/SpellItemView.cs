using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using System.Text;

	public class SpellItemView : ZoomHUD
    {

		public Button characterModel;
		public Transform characterContainer;
		public InstancePool characterPool;

		public Text spellText;

		private SpellItem spellItem;

		//public Button generateButton;

		private StringBuilder playerSpell;

		public TintHUD tintHUD;

		public ItemDisplayView itemDisplay;

		public void SetUpSpellView(SpellItem spellItem){

			this.spellItem = spellItem;

			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);

			RefreshSpell();

		}

		public void RefreshSpell(){

			spellText.text = string.Empty;

			string spell = spellItem.spell;

			playerSpell = new StringBuilder();

			characterPool.AddChildInstancesToPool(characterContainer);

			for (int i = 0; i < Player.mainPlayer.allCollectedCharacters.Count;i++){
				char character = Player.mainPlayer.allCollectedCharacters[i];
				Button characterButton = characterPool.GetInstance<Button>(characterModel.gameObject, characterContainer);
				characterButton.GetComponentInChildren<Text>().text = character.ToString();
				characterButton.onClick.RemoveAllListeners();
				characterButton.onClick.AddListener(delegate
				{

					characterPool.AddInstanceToPool(characterButton.gameObject);

					playerSpell.Append(character.ToString());

					spellText.text = playerSpell.ToString();

				});
			}
            
		}
              
		public void OnGenerateButtonClick(){

			if(!IsSpellRight()){
				tintHUD.SetUpSingleTextTintHUD("当前组合错误");
				RefreshSpell();
				return;
			}

			Item generatedItem = spellItem.GenerateItem();


			Player.mainPlayer.AddItem(generatedItem);

			itemDisplay.SetUpItemDisplayView(spellItem);

			this.gameObject.SetActive(false);

		}

		public bool IsSpellRight(){

			bool spellRight = true;;

			char[] spellCharacters = spellText.text.ToCharArray();

			char[] rightSpellCharacters = ExploreManager.Instance.newMapGenerator.spellItemOfCurrentLevel.spell.ToCharArray();

			if(spellCharacters.Length != rightSpellCharacters.Length){
				return false;
			}

			for (int i = 0; i < rightSpellCharacters.Length;i++){

				char rightCharacter = rightSpellCharacters[i];

				char playerSpelledCharater = spellCharacters[i];

				if(rightCharacter != playerSpelledCharater){
					spellRight = false;
					break;
				}

			}

			return spellRight;

		}
        
		public void QuitSpellItemView(){

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut();

			StartCoroutine(zoomCoroutine);         

		}
        
    }

}

