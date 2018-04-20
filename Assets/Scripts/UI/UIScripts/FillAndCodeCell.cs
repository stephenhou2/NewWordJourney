using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;
	using UnityEngine.UI;

	public delegate void CharacterClickCallBack(int cellIndex);

	public class FillAndCodeCell : MonoBehaviour {

		public Image fillBackground;
		public Button fillButton;
		public Text fillText;

		public Transform[] codeButtons;

		public Text[] codeTexts;

		private int cellIndex;

		private CharacterFillChangeCallBack codeChangeCallBack;

		private CharacterClickCallBack fillClickCallBack;

		private bool[] validArray = new bool[3]{true,true,true};

//		private int characterToFillIndex;

		private char[] codeArray = new char[]{'?','?','?'};

		private int codeOffset = 120;

		private float codeFlyOutDuration = 0.2f;

		public bool isFoldout;

		/// <summary>
		/// 初始化字母填充cell
		/// </summary>
		/// <returns>The up character fill.</returns>
		/// <param name="charInQuestion">Char in question.</param>
		/// <param name="realChar">Real char.</param>
		public char SetUpFillAndCodeCell(int cellIndex, char charInQuestion, char realChar,
			CharacterFillChangeCallBack codeChangeCallBack,CharacterClickCallBack fillClickCallBack)
		{

			this.cellIndex = cellIndex;

			this.codeChangeCallBack = codeChangeCallBack;

			this.fillClickCallBack = fillClickCallBack;

			for (int i = 0; i < codeButtons.Length; i++) {
				codeButtons [i].localPosition = Vector3.zero;
			}

			isFoldout = true;


			if (charInQuestion == '?') {

				for(int i = 0;i<codeArray.Length;i++){
					codeArray[i] = '?';
				}

				GenerateCharsToFill (realChar);

				fillText.text = string.Empty;
				fillBackground.color = Color.cyan;
				fillButton.interactable = true;

				for (int i = 0; i < codeTexts.Length; i++) {
					codeTexts [i].text = codeArray [i].ToString ();
				}

				return ' ';

			} else {

				fillText.text = charInQuestion.ToString ();
				fillBackground.color = Color.white;
				fillButton.interactable = false;
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

			codeArray [index] = realChar;

			int count = 0;

			while (count < 2) {

				char c = (char)(Random.Range (0, 26) + CommonData.aInASCII);

				if (CheckCharExist (c, codeArray)) {
					continue;
				}

				index = GetARandomValidIndex ();

				codeArray [index] = c;

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

		public void OnFillButtonClick(){

			isFoldout = !isFoldout;

			if (isFoldout) {
				HideCodeButtons ();
			} else {
				ShowCodeButtons ();
			}

			fillClickCallBack (cellIndex);

		}

		public void HideCodeButtons(){
			for (int i = 0; i < codeButtons.Length; i++) {
				codeButtons [i].DOLocalMoveY (0, codeFlyOutDuration);
			}
		}

		public void ShowCodeButtons(){
			for (int i = 0; i < codeButtons.Length; i++) {
				codeButtons [i].DOLocalMoveY (-(i + 1) * codeOffset, codeFlyOutDuration);
			}
		}

		public void OnCodeButtonClick(int index){

			HideCodeButtons ();

			isFoldout = true;

			char characterSelect = codeArray [index];

			fillText.text = characterSelect.ToString ();

			codeChangeCallBack (cellIndex, characterSelect);

		}


	}
}
