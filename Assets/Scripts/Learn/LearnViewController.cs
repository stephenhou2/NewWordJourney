using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using System.Data;


	public class LearnViewController : MonoBehaviour {

		// 单词学习view
		public LearnView learnView;

		// 是否从学习过程开始
		public bool beginWithLearn;

		// 背错的单词是否加到队列尾部继续学习
		private bool addToTrailIfWrong;

		// 一次学习的单词数量（单个水晶学习单词数量）
		private int singleLearnWordsCount;


		//***********不再按照记忆曲线对已测试过的单词进行复习,以下代码注释掉**************//

		// 背多少组循环一次
//		private int recycleGroupBase;

		// 背诵多少次进行一次大循环
//		private int recycleLearnTimeBase;

		/*背诵总次数	 0		 1		 2		 3
		 * 			【0】	【0】	【0】	【0】
		 * 
		 *背诵总次数 	 4		 5		 6		 7		
		 * 			【1】	【1】	【1】	【1】
		 * 
		 *背诵总次数 	 8		 9	  	 10		 11		
		 * 			【0】	【0】	【0】	【0】
		 * 
		 *背诵总次数 	 12		 13		 14		 15		
		 * 			【1】	【1】	【1】	【1】-----到这里8个单词刚好都背完两遍
		 * 
		 *背诵总次数 	 16		 17		 18		 19	
		 * 			【2】	【2】	【2】	【2】-----重新开始循环
		 * 
		 *背诵总次数 	 20		 21		 22		 23	
		 * 			【3】	【3】	【3】	【3】
		 * 
		 * 			 ............
		 *			 ............
		 *
		 * 带【】的数字表示当前使用的是背诵过几次的单词
		 * 上例中假设一共有8个单词，则是以4组为循环基数，以2次为背诵次数循环基数
		 */ 

		//***********不再按照记忆曲线对已测试过的单词进行复习,以上代码注释掉**************//



		// 本次所有需要记忆的单词数组
		private HLHWord[] wordsToLearnArray;

		// 未掌握的单词列表
		private List<HLHWord> ungraspedWordsList;

		private List<HLHWord> graspedWordsList;

		private int learnedWordCount;

		private int correctWordCount;

		private int coinGain;

//		private bool hasFinishWholeCurrentTypeWords;


		private MySQLiteHelper mySql;

		private string currentWordsTableName;


		private Examination.ExaminationType examType;

		// 当前正在学习的单词（未掌握单词列表的首项）
		private HLHWord currentLearningWord{
			get{
				if (beginWithLearn && ungraspedWordsList.Count > 0) {
					return ungraspedWordsList [0];
				} else if (currentExamination != null) {
					return currentExamination.question;
				} else {
					return null;
				}
			}
		}

		// 单词测试列表
		private List<Examination> finalExaminationsList ;

//		private List<Examination> learnExaminationsList;

		// 当前正在进行的单词测试（单词测试列表的首项）
		private Examination currentExamination{
			get{
				if (finalExaminationsList.Count > 0) {
					return finalExaminationsList [0];
				} else {
					return null;
				}
			}
		}

		void Awake(){
			singleLearnWordsCount = 10;
//			recycleGroupBase = 8;
//			recycleLearnTimeBase = 2;
			wordsToLearnArray = new HLHWord[singleLearnWordsCount];
			finalExaminationsList = new List<Examination> ();
//			learnExaminationsList = new List<Examination> ();
			ungraspedWordsList = new List<HLHWord> ();
			graspedWordsList = new List<HLHWord> ();

		}

		/// <summary>
		/// 初始化学习界面
		/// </summary>
		public void SetUpLearnView(){
//			e.PlayAudioClip ("UI/sfx_UI_Click");
			currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();
			Time.timeScale = 0;
			GameManager.Instance.soundManager.PauseBgm ();
//			StartCoroutine ("SetUpViewAfterDataReady");
//		}
//
//
//		private IEnumerator SetUpViewAfterDataReady(){
//
//			bool dataReady = false;
//
//			while (!dataReady) {
//
//				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
//					GameDataCenter.GameDataType.UISprites,
//				});
//				yield return null;
//			}

			GameSettings.LearnMode learnMode = GameManager.Instance.gameDataCenter.gameSettings.learnMode;

			switch (learnMode) {
			case GameSettings.LearnMode.Test:
				#warning 这里暂时只做英->中
				this.examType = Examination.ExaminationType.EngToChn;
				addToTrailIfWrong = false;
				beginWithLearn = false;
				break;
			case GameSettings.LearnMode.Learn:
				this.examType = Examination.ExaminationType.Both;
				addToTrailIfWrong = true;
				beginWithLearn = true;
				break;
			}

			learnedWordCount = 0;
			coinGain = 0;
			correctWordCount = 0;

			InitWordsToLearn ();

			learnView.InitLearnView (wordsToLearnArray.Length);

			if (beginWithLearn) {
				learnView.SetUpLearnViewWithWord (currentLearningWord);
			} else {
				GenerateFinalExams (examType);
				learnView.SetUpLearnViewWithFinalExam (finalExaminationsList [0], examType);
			}


		}


		public void ChangeAutoPronunciationEnability(){

			bool isAutoPronounce = !GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce;

			GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce = isAutoPronounce;

			learnView.UpdatePronounceControl (isAutoPronounce);

			GameManager.Instance.persistDataManager.SaveGameSettings ();

		}


		/// <summary>
		/// 初始化本次要学习的单词数组
		/// </summary>
		private void InitWordsToLearn(){

			ungraspedWordsList.Clear ();

			mySql = MySQLiteHelper.Instance;

			mySql.GetConnectionWith (CommonData.dataBaseName);

//			int totalLearnTimeCount = GameManager.Instance.gameDataCenter.learnInfo.totalLearnTimeCount;
//
//			int totalWordsCount = mySql.GetItemCountOfTable (CommonData.CET4Table,null,true);
//
//			// 大循环的次数
//			int bigCycleCount = totalLearnTimeCount * singleLearnWordsCount / (totalWordsCount * recycleLearnTimeBase);
//
//			currentWordsLearnedTime = totalLearnTimeCount % (recycleLearnTimeBase * recycleGroupBase) / recycleGroupBase + recycleLearnTimeBase * bigCycleCount;

			mySql.BeginTransaction ();

			string query = string.Format ("SELECT learnedTimes FROM {0} ORDER BY learnedTimes ASC", currentWordsTableName);

			IDataReader reader = mySql.ExecuteQuery (query);

			reader.Read ();

			int wholeLearnTime = reader.GetInt16 (0);

			query = string.Format ("SELECT COUNT(*) FROM {0} WHERE learnedTimes={1}", currentWordsTableName, wholeLearnTime);

			reader = mySql.ExecuteQuery (query);

			reader.Read ();

			int validWordCount = reader.GetInt32 (0);

			if (validWordCount < 10) {
				
				string[] colFields = new string[]{ "learnedTimes" };
				string[] values = new string[]{ (wholeLearnTime + 1).ToString() };
				string[] conditions = new string[]{"learnedTimes=" + wholeLearnTime.ToString()};

				mySql.UpdateValues (currentWordsTableName, colFields, values, conditions, true);

				wholeLearnTime++;

			}

			// 边界条件
			string[] condition = new string[]{ string.Format("learnedTimes={0} ORDER BY RANDOM() LIMIT 10",wholeLearnTime) };

			reader = mySql.ReadSpecificRowsOfTable (currentWordsTableName, null, condition, true);

			int index = 0;

			while(reader.Read()){

				
				int wordId = reader.GetInt32 (0);

				string spell = reader.GetString (1);

				string phoneticSymble = reader.GetString (2);

				string explaination = reader.GetString (3);

				string sentenceEN = reader.GetString (4);

				string sentenceCH = reader.GetString (5);

				string pronounciationURL = reader.GetString (6);

				int wordLength = reader.GetInt16 (7);

				int learnedTimes = reader.GetInt16 (8);

				int ungraspTimes = reader.GetInt16 (9);

				HLHWord word = new HLHWord (wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes);


//				Debug.LogFormat ("{0}---{1}次",word,learnedTimes);

				wordsToLearnArray [index] = word;

				index++;

			}

			mySql.EndTransaction ();

			mySql.CloseAllConnections ();

			// 当前要学习的单词全部加入到未掌握单词列表中，用户选择掌握或者学习过该单词后从未掌握单词列表中移除
			for (int i = 0; i < wordsToLearnArray.Length; i++) {
				ungraspedWordsList.Add (wordsToLearnArray [i]);
			}

//			firstIdOfCurrentLearningWords = wordsToLearnArray [0].wordId;

		}

		/// <summary>
		/// 直接从本次学习单词生成最终测试列表
		/// </summary>
		private void GenerateFinalExams(Examination.ExaminationType examType){

			for (int i = 0; i < ungraspedWordsList.Count; i++) {

				HLHWord word = ungraspedWordsList [i];

				Examination finalExam = new Examination (word, wordsToLearnArray, examType);

				finalExaminationsList.Add (finalExam);

			}

		}


		/// <summary>
		/// 用户点击了发音按钮
		/// </summary>
		public void OnPronunciationButtonClick(){

			HLHWord word = currentLearningWord;

			if (word == null) {
				return;
			}

			GameManager.Instance.pronounceManager.PronounceWord (word);

		}



		/// <summary>
		/// 用户点击了已掌握按钮
		/// </summary>
		public void OnHaveGraspedButtonClick(){

			GameManager.Instance.pronounceManager.CancelPronounce ();

			// 使用当前学习中的单词（在这时已掌握）生成对应的单词测试
			Examination exam = new Examination (currentLearningWord, wordsToLearnArray,examType);

			// 单词测试加入到最终测试列表中
			finalExaminationsList.Add (exam);

			// 将当前学习中的单词从未掌握单词列表中删除
			HLHWord word = ungraspedWordsList[0];
			ungraspedWordsList.RemoveAt (0);
			graspedWordsList.Add (word);

			if (ungraspedWordsList.Count == 0) {
				GenerateFinalExams (examType);
			}

			learnView.SetUpLearnViewWithFinalExam (finalExaminationsList [0],examType);

		}

		/// <summary>
		/// 用户点击了未掌握按钮
		/// </summary>
		public void OnHaveNotGraspedButtonClick(){
			if (ungraspedWordsList.Count == 1) {
				OnShowExplainationButtonClick ();
				return;
			}
			// 开始单词学习过程
			learnView.SetUpLearningProgress ();

		}

		/// <summary>
		/// 显示单词释义
		/// </summary>
		public void OnShowExplainationButtonClick(){
			learnView.ShowExplaination ();
		}

		/// <summary>
		/// 用户点击了不熟悉按钮
		/// </summary>
		public void OnUnfamiliarButtonClick(){

			// 将该单词移至未掌握单词列表的尾部
			HLHWord unfamiliarWord = currentLearningWord;

			ungraspedWordsList.RemoveAt (0);

			ungraspedWordsList.Add (unfamiliarWord);

			learnView.SetUpLearnViewWithWord (currentLearningWord);


		}

		private HLHWord GetWordFromWordsToLearnArrayWith(int wordId){

			for (int i = 0; i < wordsToLearnArray.Length; i++) {
				HLHWord word = wordsToLearnArray [i];
				if (word.wordId == wordId) {
					return word;
				}
			}

			return null;

		}

			

		/// <summary>
		/// 用户点击了最终测试界面中的答案选项卡
		/// </summary>
		public void OnAnswerChoiceButtonOfFinalExamsClick(HLHWord selectWord){

			learnedWordCount++;

			learnView.UpdateLearningProgress (learnedWordCount, wordsToLearnArray.Length, true);

			// 如果选择正确，则将该单词的测试从测试列表中移除
			if (selectWord.wordId == currentExamination.question.wordId) {
//				Debug.Log ("选择正确");

				GameManager.Instance.soundManager.PlayAudioClip ("UI/sfx_UI_RightTint");

				correctWordCount++;

				coinGain++;

				learnView.UpdateCrystalHarvest (coinGain);

				currentExamination.RemoveCurrentExamType ();

				// 如果当前单词测试的所有测试类型都已经完成（根据设置，测试类型有 英译中，英译中+中译英）都已经完成，则从测试列表中删除该测试
				bool currentExamFinished = currentExamination.CheckCurrentExamFinished();
				// 当前单词测试未完成
				Examination exam = currentExamination;
				if (currentExamFinished) {
					currentExamination.question.learnedTimes++;
					finalExaminationsList.RemoveAt (0);
				}else{
					finalExaminationsList.RemoveAt (0);
					finalExaminationsList.Add (exam);
				}
				exam.Clear ();
					
			} else {
				// 如果选择错误
//				Debug.Log ("选择错误");

				GameManager.Instance.soundManager.PlayAudioClip ("UI/sfx_UI_WrongTint");

				// 单词的背错次数+1
				GetWordFromWordsToLearnArrayWith(currentExamination.question.wordId).ungraspTimes++;

				// 当前测试加入到测试列表尾部
				Examination exam = currentExamination;

				finalExaminationsList.RemoveAt (0);

				exam.Clear ();

				if (addToTrailIfWrong) {
					finalExaminationsList.Add (exam);
					coinGain--;
				}
					
				learnView.ShowRightAnswerAndEnterNextExam (exam.correctAnswerIndex ,currentExamination);
			}

			// 单词测试环节结束
			if (finalExaminationsList.Count <= 0) {
				learnView.ShowFinishLearningHUD (coinGain,correctWordCount);
			} else {
				// 测试环节还没有结束，则初始化下一个单词的测试
				Examination.ExaminationType examType = currentExamination.GetCurrentExamType ();
				learnView.SetUpLearnViewWithFinalExam (currentExamination, examType);
			}

		}


		private void UpdateDataBase(){

			mySql.GetConnectionWith (CommonData.dataBaseName);

			mySql.BeginTransaction ();

			for (int i = 0; i < singleLearnWordsCount; i++) {
				HLHWord word = wordsToLearnArray [i];
				string condition = string.Format ("id={0}", word.wordId);
				string newLearnedTime = (word.learnedTimes).ToString ();
				string newUngraspTime = (word.ungraspTimes).ToString ();

				// 更新数据库中当前背诵单词的背诵次数和背错次数
				mySql.UpdateValues (currentWordsTableName, new string[]{ "learnedTimes", "ungraspTimes" }, new string[] {
					newLearnedTime,
					newUngraspTime
				}, new string[] {
					condition
				}, true);
			}

			mySql.EndTransaction ();

			mySql.CloseConnection (CommonData.dataBaseName);



		}

		/// <summary>
		/// 当前需要学习的单词组内的单词已经全部学习完毕
		/// </summary>
		private void UpdatePlayerData(){

            int goldGain = coinGain + Player.mainPlayer.extraGold;
			Player.mainPlayer.totalGold += coinGain;

			GameManager.Instance.persistDataManager.SaveCompletePlayerData ();

		}

		private void ClearCache(){

			ungraspedWordsList.Clear ();

			finalExaminationsList.Clear ();

			// 清理内存
			for (int i = 0; i < singleLearnWordsCount; i++) {
				wordsToLearnArray [i] = null;
			}

			GameManager.Instance.pronounceManager.ClearPronunciationCache ();

		}


		public void OnQuitButtonClick(){
			learnView.ShowQuitQueryHUD ();
		}

		public void OnCancelQuitButtonClick(){
			learnView.HideQuitQueryHUD ();
		}
			


		public void DestroyInstances(){
			
			Destroy (this.gameObject,0.3f);
			TransformManager.DestroyTransfromWithName (CommonData.learnCanvasPoolContainerName);
			MyResourceManager.Instance.UnloadAssetBundle (CommonData.learnCanvasBundleName, true);
		}

		public void QuitLearnView(bool finishLearning){

			Time.timeScale = 1f;

			if (finishLearning) {
				UpdateDataBase ();
				UpdatePlayerData ();
			}

			ClearCache ();

			learnView.QuitLearnView ();

			ExploreManager exploreManager = ExploreManager.Instance;

			if (exploreManager != null) {
				GameManager.Instance.UIManager.HideCanvas ("LearnCanvas");
				GameManager.Instance.soundManager.ResumeBgm ();
				if (finishLearning) {
					//exploreManager.ChangeCrystalStatus ();
					exploreManager.expUICtr.UpdatePlayerStatusBar ();
					GameManager.Instance.UIManager.RemoveCanvasCache ("LearnCanvas");
					Destroy (this.gameObject);
				}
			} else {
				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
					TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
					GameManager.Instance.UIManager.RemoveCanvasCache ("LearnCanvas");
					Destroy (this.gameObject);
				});
			}

		}

		void OnDestroy(){
//			learnView = null;
//			wordsToLearnArray = null;
//			finalExaminationsList = null;
//			ungraspedWordsList = null;
//			graspedWordsList = null;
//			mySql = null;
		}

	}

}
