using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public delegate void CharacterFillChangeCallBack(int cellIndex,char changeTo);

	public class CharacterFillCell : MonoBehaviour {

		public Text characterText;

		public Button upButton;

		public Button downButton;

		public Transform charactersToFillContainer;

		public Text[] charactersToFillTexts;

		private int cellIndex;

		private CharacterFillChangeCallBack charChangeCallBack;

		private bool[] validArray = new bool[3]{true,true,true};

		private int characterToFillIndex;

		private char[] charactersToFillArray = new char[]{'?','?','?'};

		private Vector3 characterOffsetVector = new Vector3(0,100,0);

		/// <summary>
		/// 初始化字母填充cell
		/// </summary>
		/// <returns>The up character fill.</returns>
		/// <param name="charInQuestion">Char in question.</param>
		/// <param name="realChar">Real char.</param>
		public char SetUpCharacterFill(int cellIndex, char charInQuestion,char realChar,
			CharacterFillChangeCallBack charChangeCallBack)
		{
			
			this.cellIndex = cellIndex;

			this.charChangeCallBack = charChangeCallBack;

			if (charInQuestion == '?') {

				for(int i = 0;i<charactersToFillArray.Length;i++){
					charactersToFillArray[i] = '?';
				}

				GenerateCharsToFill (realChar);

				characterText.text = string.Empty;



				for (int i = 0; i < charactersToFillTexts.Length; i++) {

					charactersToFillTexts [i].text = charactersToFillArray [i].ToString ();

				}

				charactersToFillContainer.localPosition = Vector3.zero;

				charactersToFillContainer.gameObject.SetActive (true);

				upButton.gameObject.SetActive (true);

				downButton.gameObject.SetActive (true);

				characterToFillIndex = 0;

				return charactersToFillArray [0];

			} else {

				characterText.text = charInQuestion.ToString ();

				upButton.gameObject.SetActive (false);

				downButton.gameObject.SetActive (false);

				charactersToFillContainer.gameObject.SetActive (false);

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


		public void OnUpButtonClick(){

			if (characterToFillIndex == 0) {
				return;
			}

			characterToFillIndex--;

			charactersToFillContainer.localPosition -= characterOffsetVector;

			charChangeCallBack (cellIndex, charactersToFillArray [characterToFillIndex]);

		}

		public void OnDownButtonClick(){
			
			if (characterToFillIndex == 2) {
				return;
			}

			characterToFillIndex++;

			charactersToFillContainer.localPosition += characterOffsetVector;

			charChangeCallBack (cellIndex, charactersToFillArray [characterToFillIndex]);
		}

	}
}
