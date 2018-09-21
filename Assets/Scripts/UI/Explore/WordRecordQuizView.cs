using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public struct WordRecordQuizResult{
		public int totalSourceWordsCount;
		public int healthGainTotal;
		public int manaGainTotal;
		public int correctWordsCount;
		public int wrongWordsCount;
		public int maxContinousHitCount;
		public List<PropertySet> extraPropertySets;
		public WordRecordQuizResult(int totalSourceWordsCount,int correctWordsCount,int wrongWordsCount,int maxContinousHitCount,
		                            int healthGainTotal,int manaGainTotal,List <PropertySet> extraPropertySets){
			this.totalSourceWordsCount = totalSourceWordsCount;
			this.healthGainTotal = healthGainTotal;
			this.manaGainTotal = manaGainTotal;
			this.correctWordsCount = correctWordsCount;
			this.wrongWordsCount = wrongWordsCount;
			this.maxContinousHitCount = maxContinousHitCount;
			this.extraPropertySets = extraPropertySets;
		}
	}

	public class WordRecordQuizView : MonoBehaviour
	{
		private enum QuizType{
			EnToCh,
            ChToEn
		}

		public Transform wordQuizContainer;
      
		public Button pronounceButton;

		public Text spellQuesiton;

		public Text explainationQuestion;

		public Text phoneticSymbolForExplainSelect;

		public Text exampleSentenceText;

		public Text[] choiceTexts;

		public Text pronounceNotAvalableHintTextInExplain;
      
		public Transform rewardHintContainer;

		public Text continousCorrectRequireText;
		public Text continousHitText;

		public HLHFillBar learnProgressBar;

		public Image interactiveMask;

		public Transform QueryQuitHUD;

		public QuizResultView quizResultHUD;

		public Transform queryEnterHUD;

		public Text queryEnterText;

		public Transform wordsNotEnoughHUD;

		public Image generalRewardIcon;

		public Sprite healthSprite;
		public Sprite manaSprite;

		public Text generalRewardCountText;

		public Image extraRewardIcon;

		public Sprite attackSprite;
		public Sprite magicAttackSprite;
		public Sprite armorSprite;
		public Sprite magicResistSprite;
		public Sprite dodgeSprite;
		public Sprite critSprite;

		public Text extraRewardCountText;

		public Image generalRewardSingleIcon;

		public Text generalRewardSingleCountText;

		private List<HLHWord> sourceWords;

		private int currentWordIndex;

		private int correctChoiceIndex;

		private int continousCorrectCount;

		private int extraRewardCalSeed;

		private QuizType currentQuizType;

		private IEnumerator waitAndShowNextCoroutine;
		//private IEnumerator continousCorrectHintZoomCoroutine;
		private IEnumerator rewardHintShowCoroutine;

		private int healthGainTotal;
		private int manaGainTotal;
		private int correctWordsCount;
		private int wrongWordsCount;
		private int maxContinousHitCount;
		private List<PropertySet> extraPropertySets = new List<PropertySet>();

		private CallBack enterQuizCallBack;

		private int nextExtraRewardPoint{
			get{
				return ((extraRewardCalSeed + 1) * extraRewardCalSeed / 2 + 1) * 5;
			}
		}


		public void SetUpwordRecordQuizView(List<HLHWord> sourceWords,CallBack enterQuizCallBack)
		{

			this.gameObject.SetActive(true);

			ResetRewardHint();

			if(sourceWords.Count < 3){
				wordsNotEnoughHUD.gameObject.SetActive(true);
				queryEnterHUD.gameObject.SetActive(false);
				wordQuizContainer.gameObject.SetActive(false);
				return;
			}

			wordsNotEnoughHUD.gameObject.SetActive(false);
			queryEnterHUD.gameObject.SetActive(true);
			queryEnterText.text = string.Format("本层共学习过<color=orange>{0}</color>个单词\n是否开始冥想？", sourceWords.Count);
			wordQuizContainer.gameObject.SetActive(false);

			this.sourceWords = sourceWords;

            this.enterQuizCallBack = enterQuizCallBack;
                     
		}

		public void OnConfirmEnterButtonClick(){         
			wordsNotEnoughHUD.gameObject.SetActive(false);
			queryEnterHUD.gameObject.SetActive(false);
			SetUpBasicUI();
			if(enterQuizCallBack != null){
				enterQuizCallBack();
            }
		}

		public void OnCancelEnterButtonClick(){
			QuitQuizView();
		}

		public void OnConfirmNotEnoughWordsButtonClick(){
			QuitQuizView();
		}

		private void SetUpBasicUI(){

            currentWordIndex = 0;

            continousCorrectCount = 0;

            extraRewardCalSeed = 0;

            healthGainTotal = 0;
            manaGainTotal = 0;
			correctWordsCount = 0;
			wrongWordsCount = 0;
			maxContinousHitCount = 0;
			extraPropertySets.Clear();

			continousCorrectRequireText.text = string.Format("连击奖励: 需连续答对{0}单词", nextExtraRewardPoint);

			continousHitText.text = string.Empty;

            interactiveMask.enabled = false;

            wordQuizContainer.gameObject.SetActive(true);

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                pronounceNotAvalableHintTextInExplain.enabled = true;
            }
            else
            {
                pronounceNotAvalableHintTextInExplain.enabled = false;
            }

            HLHWord[] quizWords = GetQuizWordArray(sourceWords, 3, currentWordIndex, out correctChoiceIndex);

            if (quizWords == null)
            {
                SetUpQuizResultView();
                return;
            }

            learnProgressBar.InitHLHFillBar(sourceWords.Count, 0);

            SetUpQuiz(quizWords, correctChoiceIndex);

		}


		private void SetUpQuiz(HLHWord[] words,int quizWordIndex){

			//int quizTypeInt = Random.Range(0, 2);

			currentQuizType = QuizType.ChToEn;

			HLHWord questionWord = words[quizWordIndex];

			switch(currentQuizType){
				case QuizType.EnToCh:
					pronounceButton.gameObject.SetActive(true);
					spellQuesiton.text = questionWord.spell;
					explainationQuestion.text = string.Empty;
					phoneticSymbolForExplainSelect.text = questionWord.phoneticSymbol;
					exampleSentenceText.text = questionWord.sentenceEN;
					for (int i = 0; i < words.Length;i++){
						if(i<choiceTexts.Length){
							Text choiceText = choiceTexts[i];
							choiceText.text = words[i].explaination;
							choiceText.color = CommonData.darkYellowTextColor;
						}                  
					}
					break;               
				case QuizType.ChToEn:
					pronounceButton.gameObject.SetActive(false);
					spellQuesiton.text = string.Empty;
					explainationQuestion.text = questionWord.explaination;
					phoneticSymbolForExplainSelect.text = string.Empty;
					exampleSentenceText.text = string.Empty;
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (i < choiceTexts.Length)
                        {
                            Text choiceText = choiceTexts[i];
							choiceText.text = words[i].spell;
							choiceText.color = CommonData.darkYellowTextColor;
                        }
                    }
					break;
			}

			interactiveMask.enabled = false;

		}
        
        /// <summary>
        /// 用户作出选择的回调
        /// </summary>
        /// <param name="index">Index.</param>
		public void OnChoiceMake(int index){


            HLHWord questionWord = sourceWords[currentWordIndex];
			GameManager.Instance.pronounceManager.PronounceWord(questionWord);
                     
			currentWordIndex++;

			interactiveMask.enabled = true;

			learnProgressBar.InitHLHFillBar(sourceWords.Count, currentWordIndex);

			bool choiceCorrect = index == correctChoiceIndex;
            
			int randomSeed = Random.Range(0, 2);
         
			switch (randomSeed)
            {
                case 0:
					int healthGain = Mathf.RoundToInt(0.02f * Player.mainPlayer.maxHealth);
					healthGainTotal += healthGain;
					generalRewardIcon.sprite = healthSprite;
					generalRewardCountText.text = string.Format("+{0}", healthGain);
					generalRewardSingleIcon.sprite = healthSprite;
					generalRewardSingleCountText.text = string.Format("+{0}", healthGain);
                    break;
                case 1:
					int manaGain = Mathf.RoundToInt(0.02f * Player.mainPlayer.maxMana);               
					manaGainTotal += manaGain;               
					generalRewardIcon.sprite = manaSprite;
					generalRewardCountText.text = string.Format("+{0}", manaGain);
					generalRewardSingleIcon.sprite = manaSprite;
					generalRewardSingleCountText.text = string.Format("+{0}", manaGain);
                    break;
            }
                     

			if (choiceCorrect)
			{            
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.correctTintAudioName);
				choiceTexts[index].color = Color.green;
				continousCorrectCount++;
				correctWordsCount++;
				if(continousCorrectCount > maxContinousHitCount){
					maxContinousHitCount = continousCorrectCount;
				}

				continousHitText.text = string.Format("连击 x {0}", continousCorrectCount);
				if (CheckIsExtraRewardNum(continousCorrectCount))
				{
					continousHitText.color = Color.green;
					continousHitText.GetComponent<Outline>().effectColor = Color.green; 
				}
				else{
					continousHitText.color = Color.yellow;
					continousHitText.GetComponent<Outline>().effectColor = Color.yellow;
				}

				//continousCorrectHintZoomCoroutine = CorrectHintTextZoomIn();
				//StartCoroutine(continousCorrectHintZoomCoroutine);

				if (rewardHintShowCoroutine != null){
					StopCoroutine(rewardHintShowCoroutine);
				}

				rewardHintShowCoroutine = RewardHintShow();

				StartCoroutine(rewardHintShowCoroutine);

			
			}else{

				//Player.mainPlayer.learnedWordsCountInCurrentExplore++;

				GameManager.Instance.soundManager.PlayAudioClip(CommonData.wrongTintAudioName);
				wrongWordsCount++;
				choiceTexts[correctChoiceIndex].color = Color.green;
				choiceTexts[index].color = Color.red;
				continousCorrectCount = 0;
				extraRewardCalSeed = 0;
				bool wordExistInRecord = ExploreManager.Instance.CheckWordExistInCorrectRecordList(questionWord) 
				                                       || ExploreManager.Instance.CheckWordExistInWrongRecordList(questionWord);
				if(questionWord.ungraspTimes == 0 && questionWord.learnedTimes == 0 && !wordExistInRecord){
					Player.mainPlayer.totalLearnedWordCount++;
					Player.mainPlayer.totalUngraspWordCount++;               
				}

				questionWord.isFamiliar = false;

                questionWord.learnedTimes++;
				questionWord.ungraspTimes = questionWord.learnedTimes;

                ExploreManager.Instance.RecordWord(questionWord, false);

			}


			// 检查是否触发额外奖励
            bool extraRewardTriggered = CheckIsExtraRewardNum(continousCorrectCount);
			// 如果触发额外奖励，将额外奖励触发的计算基数+1
			if (choiceCorrect && extraRewardTriggered)
            {
                extraRewardCalSeed++;
                // 这里执行额外奖励
                PropertySet propertySet = GetRandomProperty(continousCorrectCount);
                string propertyName = MyTool.GetPropertyName(propertySet.type);
                string propertyValue = MyTool.GetPropertyValueString(propertySet.value);
                            
				extraRewardIcon.enabled = true;

				switch(propertySet.type){
					case PropertyType.Attack:
						extraRewardIcon.sprite = attackSprite;
						extraRewardCountText.text = string.Format("+{0}", propertySet.value);
						break;
					case PropertyType.MagicAttack:
						extraRewardIcon.sprite = magicAttackSprite;
						extraRewardCountText.text = string.Format("+{0}", propertySet.value);
						break;
					case PropertyType.Armor:
						extraRewardIcon.sprite = armorSprite;
						extraRewardCountText.text = string.Format("+{0}", propertySet.value);
						break;
					case PropertyType.MagicResist:
						extraRewardIcon.sprite = magicResistSprite;
						extraRewardCountText.text = string.Format("+{0}", propertySet.value);
						break;
					case PropertyType.Dodge:
						extraRewardIcon.sprite = dodgeSprite;
						extraRewardCountText.text = string.Format("+{0}%", ((propertySet.value + float.Epsilon) * 100).ToString("0.0"));
						break;
					case PropertyType.Crit:
						extraRewardIcon.sprite = critSprite;
						extraRewardCountText.text = string.Format("+{0}%", ((propertySet.value + float.Epsilon) * 100).ToString("0.0"));
						break;
				}
                            
				extraPropertySets.Add(propertySet);

				generalRewardIcon.enabled = true;
				generalRewardCountText.enabled = true;

				generalRewardSingleIcon.enabled = false;
				generalRewardSingleCountText.enabled = false;

				extraRewardIcon.enabled = true;
				extraRewardCountText.enabled = true;

            }
            else
            {
				generalRewardIcon.enabled = false;
				generalRewardCountText.enabled = false;

				generalRewardSingleIcon.enabled = true;
				generalRewardSingleCountText.enabled = true;


				extraRewardIcon.enabled = false;
				extraRewardCountText.enabled = false;

            }

            // 所有本关单词已经复习完
			if(currentWordIndex >= sourceWords.Count){
				if (waitAndShowNextCoroutine != null)
                {
                    StopCoroutine(waitAndShowNextCoroutine);
                }

                waitAndShowNextCoroutine = WaitAndShowRightOrWrong(delegate
                {               
					SetUpQuizResultView();
                });


                StartCoroutine(waitAndShowNextCoroutine);

				return;
			}

			if(waitAndShowNextCoroutine != null){
				StopCoroutine(waitAndShowNextCoroutine);
			}

			continousCorrectRequireText.text = string.Format("连击奖励: 需连续答对{0}单词", nextExtraRewardPoint);

			waitAndShowNextCoroutine = WaitAndShowRightOrWrong(delegate
			{

				HLHWord[] quizWords = GetQuizWordArray(sourceWords, 3, currentWordIndex, out correctChoiceIndex);

				if (quizWords == null)
                {
                    SetUpQuizResultView();
                    return;
                }

                SetUpQuiz(quizWords, correctChoiceIndex);
			});


			StartCoroutine(waitAndShowNextCoroutine);
            
		}

        /// <summary>
        /// 检查是否是获得额外奖励的节点
        /// </summary>
        /// <returns><c>true</c>, if is extra reward index was checked, <c>false</c> otherwise.</returns>
		/// <param name="num">Index.</param>
		private bool CheckIsExtraRewardNum(int num){         
			return num == nextExtraRewardPoint;         
		}
        
		//private IEnumerator CorrectHintTextZoomIn(){
		//	float scale = 0.2f;
		//	float zoomInSpeed = (1 - scale) / 0.2f;
		//	while(scale < 1){
		//		continousHitText.transform.localScale = new Vector3(scale,scale,1);
		//		scale += Time.deltaTime * zoomInSpeed;
		//		yield return null;
		//	}
		//	yield return new WaitForSeconds(0.5f);
		//	continousHitText.text = string.Empty;
		//}

		private void ResetRewardHint(){
			int localPosX = 0;
            int localPosY = 125;
			rewardHintContainer.localPosition = new Vector3(localPosX, localPosY, 0);
		}

		private IEnumerator RewardHintShow(){
			int rewardHintMoveSpeed = 3000;
			int localPosX = 0;
			int moveDesPosX = 540;
			int localPosY = 125;

			rewardHintContainer.localPosition = new Vector3(localPosX, localPosY, 0);

			while (localPosX < moveDesPosX){
				localPosX += (int)(Time.deltaTime * rewardHintMoveSpeed);
				rewardHintContainer.localPosition = new Vector3(localPosX, localPosY, 0);
				yield return null;
			}

			yield return new WaitForSeconds(1f);

			while(localPosX > 0){
				localPosX -= (int)(Time.deltaTime * rewardHintMoveSpeed);
                rewardHintContainer.localPosition = new Vector3(localPosX, localPosY, 0);
                yield return null;
			}
		}

        /// <summary>
        /// 根据当前的单词序号获取额外的属性奖励
        /// </summary>
        /// <returns>The random property.</returns>
		/// <param name="levelSeed">获取属性时的计算因子</param>
		private PropertySet GetRandomProperty(int levelSeed){

			int randomSeed = Random.Range(0,100);
			PropertyType pt = PropertyType.Attack;
			float value = 0;

			if(randomSeed >=0 && randomSeed < 25){
				pt = PropertyType.Attack;
                value = (levelSeed - 1) / 10 + 1;
			}else if(randomSeed >= 25 && randomSeed < 50){
				pt = PropertyType.MagicAttack;
                value = (levelSeed - 1) / 10 + 1;
			}else if(randomSeed >= 50 && randomSeed < 70){
				pt = PropertyType.Armor;
                value = (levelSeed - 1) / 10 + 1;
			}else if(randomSeed >= 70 && randomSeed < 90){
				pt = PropertyType.MagicResist;
                value = (levelSeed - 1) / 10 + 1;
			}else if(randomSeed >= 90 && randomSeed < 95){
				pt = PropertyType.Dodge;
                value = (float)((levelSeed - 1) / 10 + 1) / 200;
			}else if(randomSeed >= 95 && randomSeed < 100){
				pt = PropertyType.Crit;
                value = (float)((levelSeed - 1) / 10 + 1) / 200;
			}

			PropertySet ps = new PropertySet(pt, value);

			return ps;

		}

		public void OnQuitButtonClick(){
			QueryQuitHUD.gameObject.SetActive(true);
		}

		public void OnConfirmQuitButtonClick(){
			QueryQuitHUD.gameObject.SetActive(false);
            
			SetUpQuizResultView();
		}
        
		public void OnCancelQuitButtonClick(){
			QueryQuitHUD.gameObject.SetActive(false);
		}


		private void SetUpQuizResultView(){

			StopAllCoroutines();

			wordQuizContainer.gameObject.SetActive(false);

			interactiveMask.enabled = false;

			WordRecordQuizResult quizResult = new WordRecordQuizResult(sourceWords.Count, correctWordsCount, wrongWordsCount, maxContinousHitCount, healthGainTotal, manaGainTotal, extraPropertySets);

			quizResultHUD.SetUpQuizResultView(quizResult,QuitQuizView);

			//quizResultHUD.gameObject.SetActive(true);         

		}

		public void OnConfirmQuizResult(){
			QuitQuizView();
		}

		public void QuitQuizView(){
			this.gameObject.SetActive(false);
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			ExploreManager.Instance.MapWalkableEventsStartAction();
		}


		private IEnumerator WaitAndShowRightOrWrong(CallBack callBack = null)
        {

            yield return new WaitForSeconds(1f);

            if (callBack != null)
            {
                callBack();
            }

        }

		public void OnPronounceButtonClick(){
			switch(currentQuizType){
				case QuizType.EnToCh:
					HLHWord questionWord = sourceWords[currentWordIndex];
                    GameManager.Instance.pronounceManager.PronounceWord(questionWord);
					break;
				case QuizType.ChToEn:
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.wrongTintAudioName);
					break;
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
        private HLHWord[] GetQuizWordArray(List<HLHWord> sourceWords, int count, int questionWordIndex, out int questionWordIndexInOutArray)
        {
            HLHWord[] words = new HLHWord[count];

            // 如果单词数据源长度为0，或者需获取的单词数量大于输入单词数组的长度，则返回空值
			if (sourceWords.Count == 0 || count > sourceWords.Count)
            {
                words = null;
				questionWordIndexInOutArray = 0;
				return words;
            }

			if (questionWordIndex < 0 && questionWordIndex >= sourceWords.Count)
            {
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
			for (int i = 0; i < sourceWords.Count; i++)
            {
                valableWordIndexInSourceArray.Add(i);
            }

            // 首先将题目单词放入输出的数组中
            int indexInSourceWordsArray = questionWordIndex;
            valableWordIndexInSourceArray.RemoveAt(indexInSourceWordsArray);
			HLHWord questionWord = sourceWords[indexInSourceWordsArray];


            int index = Random.Range(0, validIndexInTargetArrayList.Count);
            int indexInWordsArray = validIndexInTargetArrayList[index];
            validIndexInTargetArrayList.RemoveAt(index);

            words[indexInWordsArray] = questionWord;

            questionWordIndexInOutArray = indexInWordsArray;

            for (int i = 0; i < count - 1; i++)
            {

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

	}
}
