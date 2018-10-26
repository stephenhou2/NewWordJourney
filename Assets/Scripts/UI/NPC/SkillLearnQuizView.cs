using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using System.Data;

    public class SkillLearnQuizView : MonoBehaviour {

		public Transform skillLearnQuizContainer;

		public Text questionForExplainSelect;

        public Text phoneticSymbolForExplainSelect;

        public Text exampleSentenceText;

        public Text[] choiceTexts;

		public Text learnTintText;
      
        public Text pronounceNotAvalableHintTextInExplain;

		public HLHFillBar learnProgressBar;

		public Image skillIcon;
        
		public Image interactiveMask;

		public LearnedSkillFromQuizHUD learnedSkillFromQuizHUD;

		public Transform queryRefreshHUD;

		public Text npcName;

		public Text failureText;

		private HLHNPC npc;

		private Skill currentLearningSkill;

		private HLHWord[] sourceWords;

		private List<HLHWord> testedWords = new List<HLHWord>();

		private int currentWordIndex;
        
		private int correctChoiceIndex;

		private CallBack quitSkillLearnQuizCallBack;
        
		private IEnumerator waitAndShowNextCoroutine;

        
		public void SetUpSkillLearnQuizView(HLHNPC npc,Skill skill,CallBack quitSkillLearnQuizCallBack){

			this.npc = npc;

			this.currentLearningSkill = skill;

			this.quitSkillLearnQuizCallBack = quitSkillLearnQuizCallBack;

			testedWords.Clear();

			if(skill.wordCountToLearn == 0){
				skill.wordCountToLearn = 10;
			}

			sourceWords = GetUnlearnedWordsOfCount(skill.wordCountToLearn);

			for (int i = 0; i < sourceWords.Length;i++){
				HLHWord word = sourceWords[i];
				GameManager.Instance.pronounceManager.DownloadPronounceCache(word);
			}

			if(sourceWords.Length == 0){
				Debug.LogError("获取单词失败");
				return;
			}

			currentWordIndex = 0;
         
			if(Application.internetReachability == NetworkReachability.NotReachable){
				pronounceNotAvalableHintTextInExplain.enabled = true;
			}else{
				pronounceNotAvalableHintTextInExplain.enabled = false;
			}

			HLHWord[] quizWords = GetQuizWordArray(sourceWords, 3, currentWordIndex, out correctChoiceIndex);

			SetUpQuiz(quizWords,correctChoiceIndex);

			learnProgressBar.InitHLHFillBar(skill.wordCountToLearn, 0);

			skillIcon.sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj)
			{
				return obj.name == skill.skillIconName;            
			});

			skillIcon.enabled = true;

			skillLearnQuizContainer.gameObject.SetActive(true);

			queryRefreshHUD.gameObject.SetActive(false);

			learnedSkillFromQuizHUD.gameObject.SetActive(false);

			if(waitAndShowNextCoroutine != null){
				StopCoroutine(waitAndShowNextCoroutine);
			}

			learnTintText.text = string.Format("连续完成测试后可以掌握<color=orange>{0}</color>",skill.skillName);

			gameObject.SetActive(true);

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);

		}



        /// <summary>
        /// 设置选项的显示内容
        /// </summary>
        /// <param name="quizWords">本道题目中所有涉及的所有单词.</param>
		/// <param name="questionWordIndex">正确的选项序号</param>
		private void SetUpQuiz(HLHWord[] quizWords,int questionWordIndex){
                          
            correctChoiceIndex = questionWordIndex;

			HLHWord questionWord = quizWords[questionWordIndex];

			GameManager.Instance.pronounceManager.PronounceWord(questionWord);

            questionForExplainSelect.text = questionWord.spell;

            phoneticSymbolForExplainSelect.text = questionWord.phoneticSymbol;

            exampleSentenceText.text = questionWord.sentenceEN;

			for (int i = 0; i < quizWords.Length;i++){
				choiceTexts[i].text = quizWords[i].explaination;
				choiceTexts[i].color = CommonData.darkYellowTextColor;
			}

			interactiveMask.enabled = false;
		}


		public void Pronounce(){

			HLHWord questionWord = sourceWords[currentWordIndex];

			GameManager.Instance.pronounceManager.PronounceWord(questionWord);

		}


		/// <summary>
        /// 选择选项后的回调
        /// </summary>
        /// <param name="index">Index.</param>
        public void OnExplainationChoose(int index)
        {

			interactiveMask.enabled = true;

			HLHWord word = sourceWords[currentWordIndex];

			testedWords.Add(word);

            if (index == correctChoiceIndex)// 如果选择正确
            {
				currentWordIndex++;
                
				learnProgressBar.value++;

				Player.mainPlayer.learnedWordsCountInCurrentExplore++;
				Player.mainPlayer.correctWordsCountInCurrentExplore++;
                

				if (word.ungraspTimes == 0 && word.learnedTimes == 0)
                {
					word.isFamiliar = true;
                    Player.mainPlayer.totalLearnedWordCount++;
                }

				switch(LearningInfo.Instance.currentWordType){
					case WordType.Simple:
						Player.mainPlayer.simpleWordContinuousRightRecord++;
						if (Player.mainPlayer.simpleWordContinuousRightRecord > Player.mainPlayer.maxSimpleWordContinuousRightRecord)
                        {
							Player.mainPlayer.maxSimpleWordContinuousRightRecord = Player.mainPlayer.simpleWordContinuousRightRecord;
                        }
						break;
					case WordType.Medium:
						Player.mainPlayer.mediumWordContinuousRightRecord++;
						if (Player.mainPlayer.mediumWordContinuousRightRecord > Player.mainPlayer.maxMediumWordContinuousRightRecord)
                        {
							Player.mainPlayer.maxMediumWordContinuousRightRecord = Player.mainPlayer.mediumWordContinuousRightRecord;
                        }
						break;
					case WordType.Master:
						Player.mainPlayer.masterWordContinuousRightRecord++;
						if (Player.mainPlayer.masterWordContinuousRightRecord > Player.mainPlayer.maxMasterWordContinuousRightRecord)
                        {
							Player.mainPlayer.maxMasterWordContinuousRightRecord = Player.mainPlayer.masterWordContinuousRightRecord;
                        }
						break;
				}
                
            
				word.learnedTimes++;

				ExploreManager.Instance.RecordWord(word, true);
                     
				choiceTexts[index].color = Color.green;

				GameManager.Instance.soundManager.PlayAudioClip(CommonData.correctTintAudioName);

				if(currentWordIndex >= sourceWords.Length){

					Player.mainPlayer.LearnSkill(currentLearningSkill.skillId);

					if(waitAndShowNextCoroutine != null){
						StopCoroutine(waitAndShowNextCoroutine);
					}

					ExploreManager.Instance.UpdateWordDataBase();

					waitAndShowNextCoroutine = WaitAndShowRightOrWrong(delegate{
						
						skillLearnQuizContainer.gameObject.SetActive(false);

						npc.hasTeachedASkill = true;

						learnedSkillFromQuizHUD.SetUpLearnedSkillHUD(currentLearningSkill, QuitSkillLearnQuizView);

					});

					StartCoroutine(waitAndShowNextCoroutine);

					return;
				}

				if(waitAndShowNextCoroutine != null){
					StopCoroutine(waitAndShowNextCoroutine);
				}

				waitAndShowNextCoroutine = WaitAndShowRightOrWrong(delegate
				{

					HLHWord[] quizWords = GetQuizWordArray(sourceWords, 3, currentWordIndex, out correctChoiceIndex);

                    SetUpQuiz(quizWords, correctChoiceIndex);

				});

				StartCoroutine(waitAndShowNextCoroutine);

            }
            else//如果选择错误
            {
				currentWordIndex = 0;

				Player.mainPlayer.learnedWordsCountInCurrentExplore++; 

				// 如果当前单词不是从错误列表中抽出来重新背的，那么背诵正确时更新玩家学习单词的总数和背错单词的总数
				if (word.ungraspTimes == 0 && word.learnedTimes == 0)
                {
					word.isFamiliar = false;
                    Player.mainPlayer.totalLearnedWordCount++;
                    Player.mainPlayer.totalUngraspWordCount++;
                }
            
				switch(LearningInfo.Instance.currentWordType){
					case WordType.Simple:
						Player.mainPlayer.simpleWordContinuousRightRecord = 0;
						break;
					case WordType.Medium:
						Player.mainPlayer.mediumWordContinuousRightRecord = 0;
						break;
					case WordType.Master:
						Player.mainPlayer.masterWordContinuousRightRecord = 0;
						break;
				}            

				word.learnedTimes++;
				word.ungraspTimes++;

				ExploreManager.Instance.RecordWord(word,false);

            
				choiceTexts[index].color = Color.red;

                choiceTexts[correctChoiceIndex].color = Color.green;

				GameManager.Instance.soundManager.PlayAudioClip(CommonData.wrongTintAudioName);

				ExploreManager.Instance.UpdateWordDataBase();

				if (waitAndShowNextCoroutine != null)
                {
                    StopCoroutine(waitAndShowNextCoroutine);
                }

                waitAndShowNextCoroutine = WaitAndShowRightOrWrong(delegate
                {

					skillLearnQuizContainer.gameObject.SetActive(false);

					ShowQueryRefreshHUD();

                });

                StartCoroutine(waitAndShowNextCoroutine);            
            }



        }

		private IEnumerator WaitAndShowRightOrWrong(CallBack callBack=null){

			yield return new WaitForSeconds(1f);
            
			if(callBack != null){
				callBack();
			}

		}

		private void ShowQueryRefreshHUD(){

			queryRefreshHUD.gameObject.SetActive(true);

			npcName.text = string.Format("{0}:",npc.npcName);

			failureText.text = string.Format("看来你对<color=orange>「{0}」</color>掌握的还不够熟练，当你能够<color=orange>连续完成测试</color>的时候才真正学会了这个技能，下次想学的时候再来找我吧...",currentLearningSkill.skillName);
                     
		}

		public void OnRefreshButtonClick(){
         
			queryRefreshHUD.gameObject.SetActive(false);

			sourceWords = GetUnlearnedWordsOfCount(currentLearningSkill.wordCountToLearn);

			HLHWord[] quizWords = GetQuizWordArray(sourceWords, 3, currentWordIndex, out correctChoiceIndex);

            SetUpQuiz(quizWords, correctChoiceIndex);

			learnProgressBar.InitHLHFillBar(currentLearningSkill.wordCountToLearn, 0);
         
			skillLearnQuizContainer.gameObject.SetActive(true);

            queryRefreshHUD.gameObject.SetActive(false);

            learnedSkillFromQuizHUD.gameObject.SetActive(false);

            if (waitAndShowNextCoroutine != null)
            {
                StopCoroutine(waitAndShowNextCoroutine);
            }

		}

		public void QuitSkillLearnQuizView(){

			ExploreManager.Instance.expUICtr.UpdateWordRecords(testedWords);
			
			gameObject.SetActive(false);

			if (quitSkillLearnQuizCallBack != null)
			{
				quitSkillLearnQuizCallBack();
			}
		}

		/// <summary>
		/// 获取测试用的单词数组
		/// </summary>
		/// <returns>返回测试用的单词数组</returns>
		/// 需确保欲获取的单词数量小于 原始给定的单词数组长度， 否则返回一个空值
		/// <param name="count">单词数组长度【即需获取的单词数量】</param>
		/// <param name="questionWordIndex">必须包括的单词序号【即测试题目中的单词】</param>
		/// <param name="questionWordIndexInOutArray">测试的题目单词在输出数组中的序号</param>
		private HLHWord[] GetQuizWordArray(HLHWord[] sourceWordsArray, int count, int questionWordIndex,out int questionWordIndexInOutArray)
		{
			HLHWord[] words = new HLHWord[count];

			// 如果单词数据源长度为0，或者需获取的单词数量大于输入单词数组的长度，则返回空值
			if (sourceWordsArray.Length == 0 || count > sourceWordsArray.Length){
				words = null;
			}
                     
			if(questionWordIndex<0 && questionWordIndex >= sourceWordsArray.Length){
				questionWordIndex = 0;
			}

            // 生成一个可用序号列表，后面获取单词后用于在生成的测试用单词数组中随机定位
			List<int> validIndexInTargetArrayList = new List<int>();
			for (int i = 0; i < count; i++)
			{
				validIndexInTargetArrayList.Add(i);
			}

			// 生成一个原始数据中可用序号列表，获取单词的时候保证单词不重复
			List<int> valableWordIndexInSourceArray = new List<int>();
			for (int i = 0; i < sourceWordsArray.Length;i++)
			{
				valableWordIndexInSourceArray.Add(i);
			}

            // 首先将题目单词放入输出的数组中
			int indexInSourceWordsArray = questionWordIndex;
			valableWordIndexInSourceArray.RemoveAt(indexInSourceWordsArray);
			HLHWord questionWord = sourceWordsArray[indexInSourceWordsArray];


			int index = Random.Range(0, validIndexInTargetArrayList.Count);
			int indexInWordsArray = validIndexInTargetArrayList[index];
			validIndexInTargetArrayList.RemoveAt(index);

			words[indexInWordsArray] = questionWord;

			questionWordIndexInOutArray = indexInWordsArray;

			for (int i = 0; i < count - 1;i++){

				index = Random.Range(0, validIndexInTargetArrayList.Count);
				indexInWordsArray = validIndexInTargetArrayList[index];
				validIndexInTargetArrayList.RemoveAt(index);

				index = Random.Range(0, valableWordIndexInSourceArray.Count);
				indexInSourceWordsArray = valableWordIndexInSourceArray[index];
				valableWordIndexInSourceArray.RemoveAt(index);

				words[indexInWordsArray] = sourceWords[indexInSourceWordsArray];
			}

			return words;


		}

       

        


		/// <summary>
        /// 初始化指定数量的未学习单词
        /// 使用该方法需注意
        /// </summary>
        /// <returns>The learn words in map.</returns>
        public HLHWord[] GetUnlearnedWordsOfCount(int count)
        {

			MySQLiteHelper mySql = MySQLiteHelper.Instance;

            mySql.GetConnectionWith(CommonData.dataBaseName);

            HLHWord[] words = new HLHWord[count];

            string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();

            string query = string.Format("SELECT learnedTimes FROM {0} ORDER BY learnedTimes ASC", currentWordsTableName);

            IDataReader reader = mySql.ExecuteQuery(query);

            reader.Read();

            int wholeLearnTime = reader.GetInt16(0);

            query = string.Format("SELECT COUNT(*) FROM {0} WHERE learnedTimes=ungraspTimes AND learnedTimes>0", currentWordsTableName);

            reader = mySql.ExecuteQuery(query);

            reader.Read();

            int wrongWordCount = reader.GetInt32(0);

            if (wrongWordCount >= count)
            {
                query = string.Format("SELECT * FROM {0} WHERE learnedTimes=ungraspTimes AND learnedTimes>0 LIMIT {1}", currentWordsTableName, count);

                int index = 0;

                reader = mySql.ExecuteQuery(query);

                for (int i = 0; i < count; i++)
                {
                    reader.Read();

					HLHWord word = HLHWord.GetWordFromReader(reader);

                    words[index] = word;

                    index++;

                }
            }
            else
            {

                query = string.Format("SELECT COUNT(*) FROM {0} WHERE learnedTimes={1}", currentWordsTableName, wholeLearnTime);

                reader = mySql.ExecuteQuery(query);

                reader.Read();

                int validWordCount = reader.GetInt32(0);

                if (validWordCount < count - wrongWordCount)
                {

                    string[] colFields = { "learnedTimes" };
                    string[] values = { (wholeLearnTime + 1).ToString() };
                    string[] conditions = { "learnedTimes=" + wholeLearnTime.ToString() };

                    mySql.UpdateValues(currentWordsTableName, colFields, values, conditions, true);

                    wholeLearnTime++;

                }

                int index = 0;

                //Debug.Log(wrongWordCount);

                query = string.Format("SELECT * FROM {0} WHERE learnedTimes=ungraspTimes AND learnedTimes>0 LIMIT {1}", currentWordsTableName, wrongWordCount);

                reader = mySql.ExecuteQuery(query);

                for (int i = 0; i < wrongWordCount; i++)
                {
                    reader.Read();

					HLHWord word = HLHWord.GetWordFromReader(reader);

                    words[index] = word;

                    index++;

                }

                // 边界条件
                string[] condition = { string.Format("learnedTimes={0} ORDER BY RANDOM() LIMIT {1}", wholeLearnTime, count - wrongWordCount) };

                reader = mySql.ReadSpecificRowsOfTable(currentWordsTableName, null, condition, true);

                while (reader.Read())
                {
					HLHWord word = HLHWord.GetWordFromReader(reader);
                    
                    words[index] = word;

                    index++;

                }

            }

			mySql.CloseConnection(CommonData.dataBaseName);


            return words;
        }

	

    }
}
