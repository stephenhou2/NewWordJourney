using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

    /// <summary>
    /// 单词学习记录页面控制器
    /// </summary>
	public class RecordViewController : MonoBehaviour {

        // 单词学习记录界面
		public RecordView recordView;
        // 当前选中的tab button序号
		private int currentTabIndex;
        // 当前页数序号
		private int currentWordPageIndex;
        // 所有的熟悉单词
		private List<HLHWord> allFamiliarWords = new List<HLHWord>();
        // 所有的不熟悉单词
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
	

        /// <summary>
		/// 学习记录基础信息按钮点击响应
        /// </summary>
		public void OnGeneralRecordButtonClick(){
			// 如果当前就在基础信息面板内，直接返回
			if (currentTabIndex == 0) {
				return;
			}
			// 序号改为0
			currentTabIndex = 0;
            // 更新单词记录界面显示
			recordView.UpdateTabStatus(currentTabIndex);
            // 显示基础学习信息
			recordView.SetUpGeneralLearningInfo ();
		}


        /// <summary>
        /// 熟悉单词按钮点击响应
        /// </summary>
		public void OnGraspWordButtonClick(){
			// 如果当前就是在熟悉单词面板，直接返回
			if (currentTabIndex == 1)
            {
                return;
            }

            // 序号改为1
            currentTabIndex = 1;
            
            // 当前页数置0
			currentWordPageIndex = 0;

            // 更新单词记录界面
			recordView.UpdateTabStatus(currentTabIndex);

            // 如果学习过的单词，没有熟悉单词【即没有答对过的单词】，则不显示底部页面
			if (allFamiliarWords.Count == 0)
            {
                recordView.HidePageDisplay();
            }
            else
            {
				// 显示页面
                recordView.ShowPageDisplay();
                // 更新当前页面的单词显示
                UpdateWordsOfCurrentPage();
            }

		}

        /// <summary>
        /// 不熟悉单词按钮点击响应
        /// </summary>
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


        /// <summary>
        /// 下一页按钮点击响应
        /// </summary>
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

        /// <summary>
        /// 上一页按钮点击响应
        /// </summary>
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
        
        /// <summary>
        /// 更新当前页面的单词显示
        /// </summary>
		private void UpdateWordsOfCurrentPage(){

			int totalPage = 0;
            // 计算本页最小单词序号和最大单词序号
			int minWordIndexOfCurrentPage = currentWordPageIndex * CommonData.singleWordsRecordsPageVolume;
			int maxWordIndexOfCurrentPage = (currentWordPageIndex + 1) * CommonData.singleWordsRecordsPageVolume - 1;
                     
            switch (currentTabIndex)
            {
				// 显示熟悉单词列表
                case 1:
					// 如果本页最小单词的序号已经超过了所有熟悉单词数据的数量，并且不是第1页
					if (minWordIndexOfCurrentPage >= allFamiliarWords.Count && currentWordPageIndex > 0)
                    {
                        // 向前翻一页
						currentWordPageIndex--;
						UpdateWordsOfCurrentPage();
                  
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

                        currentWordPageIndex--;
                        UpdateWordsOfCurrentPage();
                      
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

            // 更新数据库中的单词数据
            // 更新所有待转移到熟悉列表的单词
			for (int i = 0; i < changeToFamiliarWords.Count;i++){
				word = changeToFamiliarWords[i];
                word.learnedTimes++;
                string[] conditions = { "wordId=" + word.wordId };
                string[] values = { "1" };
                sql.UpdateValues(currentWordsTableName, colFields, values, conditions, true);
            }
			// 更新所有待转移到不熟悉列表的单词
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
