using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	using DG.Tweening;
	using UnityEngine.UI;
	using System.Text;

	public class SpellView: MonoBehaviour {

		public Text spellRequestText;

		public Button[] inputCharacterButtons;

		public Text charactersEntered;

		public Transform createCountHUD;

		public Button minusBtn;

		public Button plusBtn;

		public Text createCount;

		public Slider countSlider;


		public float characterTintDuration = 0.2f;

		public TintHUD tintHUD;
		public ItemDetail itemDetail;
	

		public void SetUpSpellViewWith(EquipmentModel itemModel){

			if (itemModel == null) {
				spellRequestText.text = "请拼写正确的单词";
			} else {

//				if (itemModel.itemNameInEnglish == "") {
//					string error = string.Format ("{0}对应的物品没有拼写，理论上应该无法进入拼写界面，请检查数据是否正确");
//					Debug.LogError (error);
//					return;
//				}

				spellRequestText.text = string.Format ("请拼写 <size=60><color=orange>{0}</color></size>", itemModel.itemName);

			}

			ClearEnteredCharactersPlane ();

			InitCharacterButtons ();

			GetComponent<Canvas> ().enabled = true;

		}


		public void ClearEnteredCharactersPlane(){

			charactersEntered.text = string.Empty;

		}

		/// <summary>
		/// 根据玩家拥有的字母碎片初始化所有的字母按钮状态
		/// </summary>
		public void InitCharacterButtons(){

//			for (int i = 0; i < inputCharacterButtons.Length; i++) {
//				inputCharacterButtons [i].interactable = Player.mainPlayer.charactersCount[i] > 0;
//			}

		}


		public void UpdateCharactersEntered(string characters,int[] charactersInsufficientArray){

			charactersEntered.text = characters;

//			for (int i = 0; i < inputCharacterButtons.Length; i++) {
//				inputCharacterButtons [i].interactable = charactersInsufficientArray [i] == 0 
//					&& Player.mainPlayer.charactersCount[i] > 0;
//			}

		}

		public void ShowCharacterTintHUD(int index){
			Button characterButton = inputCharacterButtons [index];
			if (characterButton.interactable) {
				characterButton.transform.Find ("CharacterTint").gameObject.SetActive (true);
			}
		}

		public void HideCharacterTintHUD(int index){
			StartCoroutine ("LatelyHideCharacterTintHUD", index);
		}

		private IEnumerator LatelyHideCharacterTintHUD(int index){
			yield return new WaitForSecondsRealtime (characterTintDuration);
			Button characterButton = inputCharacterButtons [index];
//			if (characterButton.interactable) {
				characterButton.transform.Find ("CharacterTint").gameObject.SetActive (false);
//			}
		}

		public void UpdateCharactersPlane(){
//			for (int i = 0; i < inputCharacterButtons.Length; i++) {
//				inputCharacterButtons [i].interactable = Player.mainPlayer.charactersCount[i] > 0;
//			}
		}

		public void SetUpCreateCountHUD(int minValue,int maxValue){

			createCountHUD.gameObject.SetActive (true);

			countSlider.minValue = minValue;
			countSlider.maxValue = maxValue;

			countSlider.value = minValue;

			createCount.text = "制作1个";

		}

		public void UpdateCreateCountHUD(int count){

			countSlider.value = count;

			createCount.text = "制作" + count.ToString() + "个";

		}

		public void SetUpCreatedItemDetailHUD(Item item){

			QuitSpellCountHUD ();

//			ShowAddToBagButton ();
//			itemDetail.SetUpItemDetail(item);

			ClearEnteredCharactersPlane ();

		}



		public void QuitSpellCountHUD(){
			createCountHUD.gameObject.SetActive (false);
		}



//		private void ShowAddToBagButton(){
//			addToBagButton.gameObject.SetActive (true);
//		}
//
//		private void HideAddToBagButton(){
//			addToBagButton.gameObject.SetActive (false);
//		}


		public void SetUpTintHUD(string tint,Sprite sprite){

			tintHUD.SetUpSingleTextTintHUD (tint);

		}




		public void OnQuitSpellPlane(){
			tintHUD.QuitTintHUD ();
			StopAllCoroutines ();
			SetAllCharacterTintInactive ();
			GetComponent<Canvas> ().enabled = false;
		}

		private void SetAllCharacterTintInactive(){
			for (int i = 0; i < inputCharacterButtons.Length; i++) {
				Button characterButton = inputCharacterButtons [i];
				characterButton.transform.Find ("CharacterTint").gameObject.SetActive (false);
			}
		}

		void OnDestroy(){
//			createCountHUD = null;
//			tintHUD = null;
//			charactersInBag = null;
//			itemDetail = null;
		}
	}
}
