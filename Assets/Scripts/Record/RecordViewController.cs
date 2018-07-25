using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class RecordViewController : MonoBehaviour {


		public RecordView recordView;

		private int currentTabIndex;

		private int currentWordPageIndex;

		//private int minWordIndexOfCurrentPage{
		//	get{
		//		return CommonData.singleWordsRecordsPageVolume * currentWordPageIndex;
		//	}
		//}

		private List<HLHWord> allFamiliarWords = new List<HLHWord>();

		private List<HLHWord> allUnfamiliarWords = new List<HLHWord>();

        // 变更到已掌握状态的单词列表
		private List<HLHWord> changeToFamiliarWords = new List<HLHWord>();
        // 变更到未掌握状态的单词列表
		private List<HLHWord> changeToUnfamiliarWords = new List<HLHWord>();


		/// <summary>
		/// 初始化学习记录界面
		/// </summary>
		public void SetUpRecordView(){
			currentTabIndex = 0;
			recordView.SetUpRecordView (LearningInfo.Instance,ChangeWordStatusCallBack,OnNextWordButtonClick,OnLastWordButtonClick);
			if(allFamiliarWords.Count == 0){
				allFamiliarWords = LearningInfo.Instance.GetAllFamiliarWord();
            }  
			if(allUnfamiliarWords.Count == 0){
				allUnfamiliarWords = LearningInfo.Instance.GetAllUnfamiliarWord();
            }         
		}
	

		public void OnGeneralRecordButtonClick(){
			if (currentTabIndex == 0) {
				return;
			}
			currentTabIndex = 0;

			recordView.UpdateTabStatus(currentTabIndex);

			recordView.SetUpGeneralLearningInfo ();
		}


		public void OnGraspWordButtonClick(){
			
			if (currentTabIndex == 1)
            {
                return;
            }

            currentTabIndex = 1;
            
			currentWordPageIndex = 0;

			recordView.UpdateTabStatus(currentTabIndex);

			if (allFamiliarWords.Count == 0)
            {
                recordView.HidePageDisplay();
            }
            else
            {
                recordView.ShowPageDisplay();
                UpdateWordsOfCurrentPage();
            }

		}

		public void OnUngraspWordsButtonClick(){
			
			if (currentTabIndex == 2) {
				return;
			}

			currentTabIndex = 2;

			currentWordPageIndex = 0;

			recordView.UpdateTabStatus(currentTabIndex);

			if(allUnfamiliarWords.Count == 0){
				recordView.HidePageDisplay();
			}else{
				recordView.ShowPageDisplay();
				UpdateWordsOfCurrentPage();
			}

		}

      
		public void OnNextPageButtonClick(){

			int totalPage = 0;

			switch (currentTabIndex)
            {
                case 1:             
					totalPage = (allFamiliarWords.Count - 1) / CommonData.singleWordsRecordsPageVolume + 1;
                    break;
                case 2:               
					totalPage = (allUnfamiliarWords.Count - 1) / CommonData.singleWordsRecordsPageVolume + 1;
                    break;
            } 


			if(currentWordPageIndex == totalPage -1){
				return;
			}

			currentWordPageIndex++;         

			UpdateWordsOfCurrentPage();
         
		}

		public void OnLastPageButtonClick(){

			if(currentWordPageIndex == 0){
				return;
			}

			currentWordPageIndex--;

			UpdateWordsOfCurrentPage();

		}
        
        /// <summary>
        /// 在单词详细页里点击下一个单词按钮的回调
        /// </summary>
        /// <param name="wordIndexInList">Word index in list.</param>
		public void OnNextWordButtonClick(int wordIndexInList){
			List<HLHWord> wordList = null;
			switch(currentTabIndex){
				case 1:
					wordList = allFamiliarWords;
					break;
				case 2:
					wordList = allUnfamiliarWords;
					break;

			}

			if(wordIndexInList >= wordList.Count - 1){
				return;
			}

			wordIndexInList++;

			HLHWord word = wordList[wordIndexInList];

			recordView.wordDetail.UpdateWordDetailHUD(word, wordIndexInList);
		}

        /// <summary>
		/// 在单词详细页里点击上一个单词按钮的回调
        /// </summary>
        /// <param name="wordIndexInList">Word index in list.</param>
		public void OnLastWordButtonClick(int wordIndexInList){
			List<HLHWord> wordList = null;
            switch (currentTabIndex)
            {
                case 1:
                    wordList = allFamiliarWords;
                    break;
                case 2:
                    wordList = allUnfamiliarWords;
                    break;

            }

            if (wordIndexInList <= 0)
            {
                return;
            }

            wordIndexInList--;

            HLHWord word = wordList[wordIndexInList];

			recordView.wordDetail.UpdateWordDetailHUD(word, wordIndexInList);

		}
        
		private void UpdateWordsOfCurrentPage(){

			int totalPage = 0;
			int minWordIndexOfCurrentPage = currentWordPageIndex * CommonData.singleWordsRecordsPageVolume;
			int maxWordIndexOfCurrentPage = (currentWordPageIndex + 1) * CommonData.singleWordsRecordsPageVolume - 1;
                     
            switch (currentTabIndex)
            {
                case 1:
					if (minWordIndexOfCurrentPage >= allFamiliarWords.Count && currentWordPageIndex > 0)
                    {
						//if (currentWordPageIndex <= 0)
						//{
						//	return;
						//}
						//else
						//{
							currentWordPageIndex--;
							UpdateWordsOfCurrentPage();
						//}

                    }
                    else
					{
						totalPage = (allFamiliarWords.Count - 1) / CommonData.singleWordsRecordsPageVolume + 1;
                  
                        List<HLHWord> graspedWords = new List<HLHWord>();
                        
						maxWordIndexOfCurrentPage = maxWordIndexOfCurrentPage <= allFamiliarWords.Count - 1 ? maxWordIndexOfCurrentPage : allFamiliarWords.Count - 1;

                        for (int i = minWordIndexOfCurrentPage; i <= maxWordIndexOfCurrentPage; i++)
                        {
                            HLHWord word = allFamiliarWords[i];
                            graspedWords.Add(word);
                        }
						recordView.UpdateWordsOfCurrentPage(graspedWords, currentWordPageIndex,totalPage);
                    }
                    break;
                case 2:
					if (minWordIndexOfCurrentPage >= allUnfamiliarWords.Count && currentWordPageIndex > 0)
                    {
                        //if ()
                        //{
                        //    return;
                        //}
                        //else
                        //{
                            currentWordPageIndex--;
                            UpdateWordsOfCurrentPage();
                        //}

                    }
                    else
                    {
						totalPage = (allUnfamiliarWords.Count - 1) / CommonData.singleWordsRecordsPageVolume + 1;

                        List<HLHWord> ungraspedWords = new List<HLHWord>();

						maxWordIndexOfCurrentPage = maxWordIndexOfCurrentPage <= allUnfamiliarWords.Count - 1 ? maxWordIndexOfCurrentPage : allUnfamiliarWords.Count - 1;

                        for (int i = minWordIndexOfCurrentPage; i <= maxWordIndexOfCurrentPage; i++)
                        {
							HLHWord word = allUnfamiliarWords[i];
                            ungraspedWords.Add(word);
                        }
						recordView.UpdateWordsOfCurrentPage(ungraspedWords, currentWordPageIndex,totalPage);
                    }
                    break;
            }         
		}


		/// <summary>
        /// 更改单词的状态
        /// 【熟悉】->【不熟悉】  【不熟悉】->【熟悉】
        /// </summary>
        /// <param name="word">Word.</param>
        private void ChangeWordStatusCallBack(HLHWord word)
        {         
            // 如果是不熟悉的单词
			if (!word.isFamiliar)
            {
				word.isFamiliar = true;
				allFamiliarWords.Add(word);
				allUnfamiliarWords.Remove(word);
				changeToFamiliarWords.Add(word);
				Player.mainPlayer.totalUngraspWordCount--;
            }
			else// 如果是熟悉的单词
            {
				word.isFamiliar = false;
				allFamiliarWords.Remove(word);
				allUnfamiliarWords.Add(word);
				changeToUnfamiliarWords.Add(word);
				Player.mainPlayer.totalUngraspWordCount++;
            }

			UpdateWordsOfCurrentPage();         
        }


        /// <summary>
        /// 更新数据库中变更单词掌握状态的单词信息
        /// </summary>
		private void UpdateChangeStatusWords(){
         
			MySQLiteHelper sql = MySQLiteHelper.Instance;

            sql.GetConnectionWith(CommonData.dataBaseName);

            string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();

            string[] colFields = { "isFamiliar"};

            HLHWord word = null;

			for (int i = 0; i < changeToFamiliarWords.Count;i++){
				word = changeToFamiliarWords[i];
                word.learnedTimes++;
                string[] conditions = { "wordId=" + word.wordId };
                string[] values = { "1" };
                sql.UpdateValues(currentWordsTableName, colFields, values, conditions, true);
            }

			for (int i = 0; i < changeToUnfamiliarWords.Count;i++){
				word = changeToUnfamiliarWords[i];
                word.learnedTimes++;
                word.ungraspTimes++;      
                string[] conditions = { "wordId=" + word.wordId };
                string[] values = { "0" };
                sql.UpdateValues(currentWordsTableName, colFields, values, conditions, true);
            }
         
            sql.CloseConnection(CommonData.dataBaseName);  

		}


		/// <summary>
		/// 退出学习记录界面
		/// </summary>
		public void QuitRecordPlane(){

			UpdateChangeStatusWords();

			GameManager.Instance.pronounceManager.ClearPronunciationCache ();

			recordView.QuitRecordPlane ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
			});

			GameManager.Instance.UIManager.RemoveCanvasCache ("RecordCanvas");

		}



		/// <summary>
		/// 清理内存
		/// </summary>
		public void DestroyInstances(){

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.recordCanvasBundleName, true);

			Destroy (this.gameObject,0.3f);

		}
      
	}
}
