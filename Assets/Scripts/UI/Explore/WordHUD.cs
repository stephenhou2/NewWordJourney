using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;


    public delegate void CallBackWithWord(HLHWord word);


	public class WordHUD : MonoBehaviour {


		// ************* 选择正确释义的UI部分 *************** //
		public Transform explainationSelectPlane;

		public Text questionForExplainSelect;

        public Text phoneticSymbolForExplainSelect;

		public Text exampleSentenceText;

		public Text[] choiceTexts;

		public Image extraInfoBackground;

		public Text extraInfoText;

		public Text pronounceNotAvalableHintTextInExplain;

		public Text pronounceNotAvalableHintTextInFill;      
		// ************* 选择正确释义的UI部分 *************** //



		// ************* 选择正确字母的UI部分 *************** //      
		public Transform characterFillPlane;

		public Text questionForCharacterFill;

        public Text phoneticSymbolForCharacterFill;

		public Transform fillAndCodeModel;

		public InstancePool characterToFillCellPool;

		public Transform characterToFillCellContainer;

		public Image lockStatusIcon;

		public Sprite lockSprite;
		public Sprite unlockSprite;
		// ************* 选择正确释义的UI部分 *************** //


		public Image mask; // 选择完成时的屏幕遮挡，禁止多次屏幕点击 

		// 退出WordHUD时的回调
		private bool quitWhenClickBackground;

		// 退出单词选择时的回调
        private CallBackWithWord quitCallBack;

		// （选择释义类型）选择释义之后的回调
		private ChooseCallBack explainationChooseCallBack;

		// (填写缺失字母类型）确认填写完成的回调
		private ChooseCallBack characterFillConfirmCallBack;

		// 作为问题使用的word
		private HLHWord questionWord;

		private char[] realCharacters;
		private char[] answerCharacters;

		// 释义随机排序时的记录列表(避免每次创建问题和释义时都创建list，所以指向同一个列表，无特殊功能，查询正确释义序号使用answerIndex属性)
		private bool[] validArray = new bool[]{true,true,true};

		private List<int> recordList = new List<int> ();

		// 正确选项的序号
		private int answerIndex;

		private bool canQuitWhenClickBackground;

		private List<FillAndCodeCell> fillAndCodeCells = new List<FillAndCodeCell> ();

		private bool hasMakeChoice;




		/// <summary>
		/// 初始化单词选择弹出框
		/// </summary>
		/// <param name="wordsArray">Words array.</param>
        public void InitWordHUD(bool quitWhenClickBackground,CallBackWithWord quitCallBack,
			ChooseCallBack explainationChooseCallBack,ChooseCallBack characterFillConfirmCallBack)
		{
			this.quitWhenClickBackground = quitWhenClickBackground;
			this.quitCallBack = quitCallBack;
			this.explainationChooseCallBack = explainationChooseCallBack;
			this.characterFillConfirmCallBack = characterFillConfirmCallBack;
		}



		public void SetUpWordHUDAndShow(HLHWord word){

            this.questionWord = word;

			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				pronounceNotAvalableHintTextInFill.enabled = true;

			}
			else
			{
				pronounceNotAvalableHintTextInFill.enabled = false;
			}

			pronounceNotAvalableHintTextInExplain.enabled = false;

			gameObject.SetActive (true);

			EnableClick ();

			canQuitWhenClickBackground = true;

			explainationSelectPlane.gameObject.SetActive (false);
			characterFillPlane.gameObject.SetActive (true);

			realCharacters = word.spell.ToCharArray ();
			answerCharacters = GenerateCharacterFillArray (word.spell);

			questionForCharacterFill.text = word.explaination;
            phoneticSymbolForCharacterFill.text = word.phoneticSymbol;

			lockStatusIcon.sprite = lockSprite;

			fillAndCodeCells.Clear ();

			for (int i = 0; i < realCharacters.Length; i++) {

				char charInQuestion = answerCharacters [i];

				char realChar = realCharacters [i];

				FillAndCodeCell cell = characterToFillCellPool.GetInstance<FillAndCodeCell> (fillAndCodeModel.gameObject, characterToFillCellContainer);

				cell.gameObject.SetActive(true);

				answerCharacters [i] = cell.SetUpFillAndCodeCell (i,charInQuestion,realChar,CharacterChange,CharacterClick);
					
				fillAndCodeCells.Add (cell);

			}

			//if(GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce){
			//	OnPronunceButtonClick();
			//}         
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
				DisableClick ();
				canQuitWhenClickBackground = false;
				lockStatusIcon.sprite = unlockSprite;
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.lockOffAudioName);
				IEnumerator delayCoroutine = DelayWhenCharactersAllFillCorrect();
				StartCoroutine (delayCoroutine);
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
		public void SetUpWordHUDAndShow(HLHWord[] wordsArray,string extraInfo = null){

			hasMakeChoice = false;

			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				pronounceNotAvalableHintTextInExplain.enabled = true;

			}
			else
			{
				pronounceNotAvalableHintTextInExplain.enabled = false;
			}

			pronounceNotAvalableHintTextInFill.enabled = false;

			gameObject.SetActive (true);

			explainationSelectPlane.gameObject.SetActive (true);
			characterFillPlane.gameObject.SetActive (false);

			EnableClick ();

			questionWord = null;
			canQuitWhenClickBackground = true;

			// 首项作为测试用的单词
			questionWord = wordsArray [0];
			questionForExplainSelect.text = questionWord.spell;
            phoneticSymbolForExplainSelect.text = questionWord.phoneticSymbol;
			exampleSentenceText.text = questionWord.sentenceEN;

			for (int i = 0; i < validArray.Length; i++) {
				validArray [i] = true;
			}

			answerIndex = GetARandomValidIndex ();

			choiceTexts [answerIndex].text = questionWord.explaination;
			choiceTexts[answerIndex].color = CommonData.darkYellowTextColor;

			for(int i = 1; i < 3; i++){

				HLHWord word = wordsArray [i];

				// 从记录列表中随机一个序号
				int randomIndex = GetARandomValidIndex();

				// 按照随机的序号拿到实际的随机序号(做为在选项卡中的序号)
//				int index = recordList [randomSeed];
					
				choiceTexts [randomIndex].text = word.explaination;
				choiceTexts [randomIndex].color = CommonData.darkYellowTextColor;

			}

			if (extraInfo != null) {
				extraInfoBackground.enabled = true;
				extraInfoText.enabled = true;
				extraInfoText.text = extraInfo;
			} else {
				extraInfoBackground.enabled = false;
				extraInfoText.enabled = false;
			}

			if(GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce){
				OnPronunceButtonClick();
			}         
			canQuitWhenClickBackground = false;
				
		}

        public void OnPronunceButtonClick(){

			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return;
			}

			GameManager.Instance.pronounceManager.PronounceWord(questionWord);
            
            
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
        public void OnMakeExplainationChoice(int index)
        {
			if(hasMakeChoice){
				return;            
			}

			hasMakeChoice = true;

            canQuitWhenClickBackground = false;

            bool chooseCorrect = index == answerIndex;

            if (chooseCorrect)
            {
				Player.mainPlayer.learnedWordsCountInCurrentExplore++;
                Player.mainPlayer.correctWordsCountInCurrentExplore++;


				if (questionWord.ungraspTimes == 0 && questionWord.learnedTimes == 0)
                {
					questionWord.isFamiliar = true;
                    Player.mainPlayer.totalLearnedWordCount++;
                }

                Player.mainPlayer.wordContinuousRightRecord++;


				questionWord.learnedTimes++;

                choiceTexts[index].color = Color.green;
				IEnumerator delayCoroutine = ShowChooseResultForAWhile(true);
				StartCoroutine(delayCoroutine);
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.correctTintAudioName);
            
                // 如果当前单词不是从错误列表中抽出来重新背的，那么背诵正确时更新玩家学习单词的总数

				//Debug.Log(Player.mainPlayer.totalLearnedWordCount);

				ExploreManager.Instance.RecordWord(questionWord, true);            
            }
            else
            {
				Player.mainPlayer.learnedWordsCountInCurrentExplore++;

				// 如果当前单词不是从错误列表中抽出来重新背的，那么背诵正确时更新玩家学习单词的总数和背错单词的总数
                if (questionWord.ungraspTimes == 0 && questionWord.learnedTimes == 0)
                {
					questionWord.isFamiliar = false;
                    Player.mainPlayer.totalLearnedWordCount++;
                    Player.mainPlayer.totalUngraspWordCount++;
                }         

				if (Player.mainPlayer.wordContinuousRightRecord > Player.mainPlayer.maxWordContinuousRightRecord)
                {
                    Player.mainPlayer.maxWordContinuousRightRecord = Player.mainPlayer.wordContinuousRightRecord;
                }

                Player.mainPlayer.wordContinuousRightRecord = 0;


				questionWord.learnedTimes++;
				questionWord.ungraspTimes = questionWord.learnedTimes;

                choiceTexts[index].color = Color.red;
                choiceTexts[answerIndex].color = Color.green;

				IEnumerator delayCoroutine = ShowChooseResultForAWhile(false);
				StartCoroutine(delayCoroutine);
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.wrongTintAudioName);
				ExploreManager.Instance.RecordWord(questionWord, false);
            
				Debug.Log(Player.mainPlayer.totalLearnedWordCount);
            }


			//Debug.LogFormat("total player record:{0},total data base record:{1}", Player.mainPlayer.totalLearnedWordCount, LearningInfo.Instance.learnedWordCount);

			DisableClick ();

		}




		private IEnumerator ShowChooseResultForAWhile(bool isChooseCorrect){

			yield return new WaitForSeconds (1.0f);

			QuitWordHUD ();

			explainationChooseCallBack (isChooseCorrect);

		}



		/// <summary>
		/// 禁止屏幕点击响应
		/// </summary>
		private void DisableClick(){
			mask.enabled = true;
		}


		/// <summary>
		/// 开始接收屏幕点击
		/// </summary>
		private void EnableClick(){
			mask.enabled = false;
		}


		public void OnBackgroundClicked(){
			if (quitWhenClickBackground && canQuitWhenClickBackground) {
				ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
                questionWord = null;
				QuitWordHUD ();
			}
		}

		public void QuitWordHUD(){

			if (quitCallBack != null) {
                quitCallBack(questionWord);
			}

			characterToFillCellPool.AddChildInstancesToPool (characterToFillCellContainer);

			questionWord = null;

			recordList.Clear ();

			gameObject.SetActive (false);
		}

	}
}
