using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;
	using UnityEngine.UI;

	public delegate void CharacterClickCallBack(int cellIndex);

	public class NewCharacterToFillCell : MonoBehaviour {

		public Button characterText;

		public Transform[] characterToFillButtons;

		public Text[] charactersToFillTexts;

		private int cellIndex;

		private CharacterFillChangeCallBack charChangeCallBack;

		private CharacterClickCallBack charClickCallBack;

		private bool[] validArray = new bool[3]{true,true,true};

//		private int characterToFillIndex;

		private char[] charactersToFillArray = new char[]{'?','?','?'};

		private int characterOffset = 120;

		private float characterFlyOutDuration = 0.2f;

		public bool isFoldout;

		/// <summary>
		/// 初始化字母填充cell
		/// </summary>
		/// <returns>The up character fill.</returns>
		/// <param name="charInQuestion">Char in question.</param>
		/// <param name="realChar">Real char.</param>
		public char SetUpCharacterFill(int cellIndex, char charInQuestion, char realChar,
			CharacterFillChangeCallBack charChangeCallBack,CharacterClickCallBack charClickCallBack)
		{

			this.cellIndex = cellIndex;

			this.charChangeCallBack = charChangeCallBack;

			this.charClickCallBack = charClickCallBack;

			for (int i = 0; i < characterToFillButtons.Length; i++) {
				characterToFillButtons [i].localPosition = Vector3.zero;
			}

			isFoldout = true;


			if (charInQuestion == '?') {

				for(int i = 0;i<charactersToFillArray.Length;i++){
					charactersToFillArray[i] = '?';
				}

				GenerateCharsToFill (realChar);

				characterText.GetComponentInChildren<Text>().text = string.Empty;
				characterText.GetComponent<Image> ().color = Color.cyan;

				for (int i = 0; i < charactersToFillTexts.Length; i++) {

					charactersToFillTexts [i].text = charactersToFillArray [i].ToString ();

				}

				return ' ';

			} else {

				characterText.GetComponentInChildren<Text>().text = charInQuestion.ToString ();
				characterText.GetComponent<Image> ().color = Color.white;
				return charInQuestion;

			}

		}


		/// <summary>
		/// 生成备选字母
		/// </summary>
		/// <param name="realChar">Real char.</param>
		private void GenerateCharsToFill(char realChar){

			for (int i = 0; i < validArray.Length; i++) {
				validArray [i] = true;
			}

			int index = GetARandomValidIndex ();

			charactersToFillArray [index] = realChar;

			int count = 0;

			while (count < 2) {

				char c = (char)(Random.Range (0, 26) + CommonData.aInASCII);

				if (CheckCharExist (c, charactersToFillArray)) {
					continue;
				}

				index = GetARandomValidIndex ();

				charactersToFillArray [index] = c;

				count++;
			}

		}

		private int GetARandomValidIndex(){

			int index = Random.Range (0, 3);

			while(!validArray[index]){
				index = Random.Range (0, 3);
			}

			validArray [index] = false;

			return index;

		}

		private bool CheckCharExist(char c,char[] cArray){

			bool exist = false;

			for (int i = 0; i < cArray.Length; i++) {

				if (cArray [i].Equals (c)) {
					exist = true;
					break;
				}
			}

			return exist;

		}

		public void OnCharacterButtonClick(){

			isFoldout = !isFoldout;

			if (isFoldout) {
				HideCharacterToFillButtons ();
			} else {
				ShowCharacterToFillButtons ();
			}

			charClickCallBack (cellIndex);

		}

		public void HideCharacterToFillButtons(){
			for (int i = 0; i < characterToFillButtons.Length; i++) {
				characterToFillButtons [i].DOLocalMoveY (0, characterFlyOutDuration);
			}
		}

		public void ShowCharacterToFillButtons(){
			for (int i = 0; i < characterToFillButtons.Length; i++) {
				characterToFillButtons [i].DOLocalMoveY (-(i + 1) * characterOffset, characterFlyOutDuration);
			}
		}

		public void OnCharacterToFillButtonClick(int index){

			HideCharacterToFillButtons ();

			char characterSelect = charactersToFillArray [index];

			characterText.GetComponentInChildren<Text> ().text = characterSelect.ToString ();

			charChangeCallBack (cellIndex, characterSelect);

		}


	}
}
