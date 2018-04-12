using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class WordHUD : MonoBehaviour {


		// ************* 选择正确释义的UI部分 *************** //
		public Transform explainationSelectPlane;

		public Text questionForExplainationSelect;

		public Text[] choiceTexts;
		// ************* 选择正确释义的UI部分 *************** //



		// ************* 选择正确字母的UI部分 *************** //

		public Transform characterFillPlane;

		public Text questionForCharacterFill;

		public Transform characterToFillCellModel;

		public Transform fillAndCodeModel;

		public InstancePool characterToFillCellPool;

		public Transform characterToFillCellContainer;

		public Image lockStatusIcon;

		public Sprite lockSprite;
		public Sprite unlockSprite;

		// ************* 选择正确释义的UI部分 *************** //

		// 退出WordHUD时的回调
		private bool quitWhenClickBackground;

		// 退出单词选择时的回调
		private CallBack quitCallBack;

		// （选择释义类型）选择释义之后的回调
		private ChooseCallBack explainationChooseCallBack;

		// (填写缺失字母类型）确认填写完成的回调
		private ChooseCallBack characterFillConfirmCallBack;

		// 作为问题使用的word
		private LearnWord questionWord;

		private char[] realCharacters;
		private char[] answerCharacters;

		// 释义随机排序时的记录列表(避免每次创建问题和释义时都创建list，所以指向同一个列表，无特殊功能，查询正确释义序号使用answerIndex属性)
		private bool[] validArray = new bool[]{true,true,true};

		private List<int> recordList = new List<int> ();

		// 正确选项的序号
		private int answerIndex;

		private bool canQuitWhenClickBackground;

		private List<FillAndCodeCell> fillAndCodeCells = new List<FillAndCodeCell> ();


		/// <summary>
		/// 初始化单词选择弹出框
		/// </summary>
		/// <param name="wordsArray">Words array.</param>
		public void InitWordHUD(bool quitWhenClickBackground,CallBack quitCallBack,
			ChooseCallBack explainationChooseCallBack,ChooseCallBack characterFillConfirmCallBack)
		{
			this.quitWhenClickBackground = quitWhenClickBackground;
			this.quitCallBack = quitCallBack;
			this.explainationChooseCallBack = explainationChooseCallBack;
			this.characterFillConfirmCallBack = characterFillConfirmCallBack;
		}

		public void SetUpWordHUDAndShow(LearnWord word){

			gameObject.SetActive (true);

			canQuitWhenClickBackground = true;

			explainationSelectPlane.gameObject.SetActive (false);
			characterFillPlane.gameObject.SetActive (true);

			realCharacters = word.spell.ToCharArray ();
			answerCharacters = GenerateCharacterFillArray (word.spell);

			questionForCharacterFill.text = word.explaination;

			lockStatusIcon.sprite = lockSprite;

			fillAndCodeCells.Clear ();

			for (int i = 0; i < realCharacters.Length; i++) {

				char charInQuestion = answerCharacters [i];

				char realChar = realCharacters [i];

				FillAndCodeCell cell = characterToFillCellPool.GetInstance<FillAndCodeCell> (fillAndCodeModel.gameObject, characterToFillCellContainer);

				answerCharacters [i] = cell.SetUpFillAndCodeCell (i,charInQuestion,realChar,CharacterChange,CharacterClick);
					
				fillAndCodeCells.Add (cell);

//				FillAndCodeCell2 cell = characterToFillCellPool.GetInstance<FillAndCodeCell2> (characterToFillCellModel.gameObject, characterToFillCellContainer);
//
//				answerCharacters [i] = cell.SetUpFillAndCodeCell (i,charInQuestion,realChar,CharacterChange);

			}

		}



		/// <summary>
		/// 根据拼写生成字母填充的字母数组，'?'代表需要填充的字母
		/// </summary>
		/// <returns>The character fill array.</returns>
		private char[] GenerateCharacterFillArray(string spell){

			int length = spell.Length;

			int characterToFillCount = (int)(spell.Length * 0.4f);

			if (characterToFillCount == 0) {
				characterToFillCount = 1;
			}

			char[] characters = spell.ToCharArray ();

			int count = 0;

			while (count < characterToFillCount) {

				int index = Random.Range (0, length);

				if (recordList.Contains (index)) {
					continue;
				}

				characters[index] = '?';

				recordList.Add (index);

				count++;

			}

			return characters;
		}

		private void CharacterClick(int characterCellIndex){
			
			for (int i = 0; i < characterToFillCellContainer.childCount; i++) {

				if (i == characterCellIndex) {
					continue;
				}

				FillAndCodeCell cell = characterToFillCellContainer.GetChild (i).GetComponent<FillAndCodeCell>();

				if (!cell.isFoldout) {
					cell.HideCodeButtons ();
					cell.isFoldout = true;
				}

			}
		}

		private void CharacterChange(int characterCellIndex,char changeTo){

			answerCharacters [characterCellIndex] = changeTo;

			bool isCharactersFillCorrect = CheckCharactersCorrect();

			if (isCharactersFillCorrect) {
				lockStatusIcon.sprite = unlockSprite;
				StartCoroutine ("DelayWhenCharactersAllFillCorrect");
			}


		}

		private IEnumerator DelayWhenCharactersAllFillCorrect(){
			yield return new WaitForSeconds (1.0f);
			QuitWordHUD ();
			if (characterFillConfirmCallBack != null) {
				characterFillConfirmCallBack (true);
			}
		}



		private bool CheckCharactersCorrect(){

			bool charactersCorrect = true;

			for (int i = 0; i < realCharacters.Length; i++) {

				if(!realCharacters[i].Equals(answerCharacters[i])){
					charactersCorrect = false;
					break;
				}
			}

			return charactersCorrect;
		}



		/// <summary>
		/// 初始化单词【单词数据规定：数组首项为测试的单词，剩余为混淆用单词】
		/// </summary>
		/// <param name="wordsArray">Words array.</param>
		public void SetUpWordHUDAndShow(LearnWord[] wordsArray){

			explainationSelectPlane.gameObject.SetActive (true);
			characterFillPlane.gameObject.SetActive (false);

			questionWord = null;
			canQuitWhenClickBackground = true;

			// 首项作为测试用的单词
			questionWord = wordsArray [0];
			questionForExplainationSelect.text = questionWord.spell;

			for (int i = 0; i < validArray.Length; i++) {
				validArray [i] = true;
			}

			answerIndex = GetARandomValidIndex ();

			choiceTexts [answerIndex].text = questionWord.explaination;
			choiceTexts [answerIndex].color = Color.white;

			for(int i = 1; i < 3; i++){

				LearnWord word = wordsArray [i];

				// 从记录列表中随机一个序号
				int randomIndex = GetARandomValidIndex();

				// 按照随机的序号拿到实际的随机序号(做为在选项卡中的序号)
//				int index = recordList [randomSeed];
					
				choiceTexts [randomIndex].text = word.explaination;
				choiceTexts [randomIndex].color = Color.white;

			}

			gameObject.SetActive (true);

		}


		private int GetARandomValidIndex(){

			int index = Random.Range (0, 3);

			while(!validArray[index]){
				index = Random.Range (0, 3);
			}

			validArray [index] = false;

			return index;

		}
			


		/// <summary>
		/// 作出单词选择
		/// </summary>
		/// <param name="index">Index.</param>
		public void OnMakeExplainationChoice(int index){

			canQuitWhenClickBackground = false;

			if (index == answerIndex) {
				choiceTexts [index].color = Color.green;
				explainationChooseCallBack (true);
			} else {
				choiceTexts [index].color = Color.red;
				choiceTexts [answerIndex].color = Color.green;
				explainationChooseCallBack (false);
			}

//			QuitWordHUD ();

		}

		public void OnBackgroundClicked(){
			
			if (quitWhenClickBackground && canQuitWhenClickBackground) {
				QuitWordHUD ();
			}
		}

		public void QuitWordHUD(){

			if (quitCallBack != null) {
				quitCallBack ();
			}

			characterToFillCellPool.AddChildInstancesToPool (characterToFillCellContainer);

			questionWord = null;

			recordList.Clear ();

			gameObject.SetActive (false);
		}

	}
}
