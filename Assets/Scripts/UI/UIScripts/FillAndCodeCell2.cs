using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public delegate void CharacterFillChangeCallBack(int cellIndex,char changeTo);

	public class FillAndCodeCell2 : MonoBehaviour {

		public Text fillText;

		public Button upButton;

		public Button downButton;

		public Transform codeButtonsContainer;

		public Text[] codeTexts;

		private int cellIndex;

		private CharacterFillChangeCallBack codeChangeCallBack;

		private bool[] validArray = new bool[3]{true,true,true};

		private int codeIndex;

		private char[] codeArray = new char[]{'?','?','?'};

		private Vector3 codeOffsetVector = new Vector3(0,100,0);

		/// <summary>
		/// 初始化字母填充cell
		/// </summary>
		/// <returns>The up character fill.</returns>
		/// <param name="charInQuestion">Char in question.</param>
		/// <param name="realChar">Real char.</param>
		public char SetUpFillAndCodeCell(int cellIndex, char charInQuestion, char realChar,
			CharacterFillChangeCallBack codeChangeCallBack)
		{
			
			this.cellIndex = cellIndex;

			this.codeChangeCallBack = codeChangeCallBack;

			if (charInQuestion == '?') {

				for(int i = 0;i<codeArray.Length;i++){
					codeArray[i] = '?';
				}

				GenerateCharsToFill (realChar);

				fillText.text = string.Empty;

				for (int i = 0; i < codeTexts.Length; i++) {

					codeTexts [i].text = codeArray [i].ToString ();

				}

				codeButtonsContainer.localPosition = Vector3.zero;

				codeButtonsContainer.gameObject.SetActive (true);

				if (!codeArray [0].Equals (realChar)) {
					codeIndex = 0;
				} else {
					int randomSeed = Random.Range (1, 3);
					for (int i = 0; i < randomSeed; i++) {
						OnDownButtonClick ();
					}
					codeIndex = randomSeed;
				}

				upButton.gameObject.SetActive (codeIndex != 0);

				downButton.gameObject.SetActive (codeIndex != 2);

				return codeArray [codeIndex];

			} else {

				fillText.text = charInQuestion.ToString ();

				upButton.gameObject.SetActive (false);

				downButton.gameObject.SetActive (false);

				codeButtonsContainer.gameObject.SetActive (false);

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


		public void OnUpButtonClick(){

			if (codeIndex == 0) {
				return;
			}
				
			codeIndex--;

			upButton.gameObject.SetActive (codeIndex != 0);

			downButton.gameObject.SetActive (codeIndex != 2);

			codeButtonsContainer.localPosition -= codeOffsetVector;

			codeChangeCallBack (cellIndex, codeArray [codeIndex]);

		}

		public void OnDownButtonClick(){
			
			if (codeIndex == 2) {
				return;
			}

			codeIndex++;

			upButton.gameObject.SetActive (codeIndex != 0);

			downButton.gameObject.SetActive (codeIndex != 2);

			codeButtonsContainer.localPosition += codeOffsetVector;

			codeChangeCallBack (cellIndex, codeArray [codeIndex]);
		}

	}
}
