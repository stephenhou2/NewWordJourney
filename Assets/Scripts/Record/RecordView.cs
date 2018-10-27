using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{

    /// <summary>
    /// 学习记录显示界面
    /// </summary>
	public class RecordView : MonoBehaviour {


		public Transform generalInfoPlane;
		public Transform wordsPlane;

		public Text recordTitle;
		public Text graspedWordsTitle;
		public Text ungraspedWordsTitle;

		public Text currentTabTitleText;

		public Text[] wordTypeAndDetails;

		public Text correctPercentageText;

		public Text learnedWordCountText;
      
		public Text wrongWordsCountText;
      
		public Text wordPageText;

		public RecordDetailCell[] recordDetailCells;
              

		// 单词cell模型
		public Transform wordModel;

		// 复用缓存池
		public InstancePool wordPool;

        // 单词cell容器
		public Transform wordContainer;

        // 页面操作容器【如果没有单词记录时需要整体隐藏，下级物体包括页面文本，上一页按钮和下一页按钮】
		public Transform pageOperationContainer;

        // 学习信息
		private LearningInfo learnInfo;

        // 单词详细记录界面
		public WordRecordDetailHUD wordDetail;
        
        // 更改单词状态回调
		private CallBackWithWord changeWordStatusCallBack;

        // 更改单词状态询问界面
		public ChangeWordStatusQueryView changeWordStatusQueryHUD;


        // 当前词库的已学习单词数
		private int learnedWordsCountOfCurrentType;
        // 当前单词的错误单词数
		private int wrongWordsCountOfCurrentType;


		/// <summary>
		/// 初始化记录页面
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		/// <param name="tabIndex">选项卡序号 【0:基本信息 1:错误单词】.</param>
		public void SetUpRecordView(LearningInfo learnInfo,CallBackWithWord changeStatusCallBack,CallBackWithInt nextWordDetailClickCallBack,CallBackWithInt lastWordDetailClickCallBack){

			this.learnInfo = learnInfo;

			learnedWordsCountOfCurrentType = learnInfo.learnedWordCount;

			wrongWordsCountOfCurrentType = learnInfo.ungraspedWordCount;

			this.changeWordStatusCallBack = changeStatusCallBack;

			SetUpGeneralLearningInfo ();

			wordDetail.InitWordRecordDetailHUD(nextWordDetailClickCallBack,lastWordDetailClickCallBack);

			GetComponent<Canvas>().enabled = true;

		}
        
        /// <summary>
        /// 从学习信息中获取难度信息和词源文本
        /// </summary>
        /// <returns>The word type and details.</returns>
		private string[] GetWordTypeAndDetails()
        {
            
            string[] wordTypeAndDetailsArray = new string[4];

			switch (learnInfo.currentWordType)
            {
                case WordType.Simple:
					wordTypeAndDetailsArray[0] = "当前难度 简单";
					wordTypeAndDetailsArray[1] = "高中\n大纲词汇";
					wordTypeAndDetailsArray[2] = "朗文\n常用3000";
					wordTypeAndDetailsArray[3] = "牛津\n常用3000";
                    break;
                case WordType.Medium:
					wordTypeAndDetailsArray[0] = "当前难度 中等";
					wordTypeAndDetailsArray[1] = "大学\n英语四级";
					wordTypeAndDetailsArray[2] = "大学\n英语六级";
					wordTypeAndDetailsArray[3] = "考研\n英语词汇";
                    break;
                case WordType.Master:
					wordTypeAndDetailsArray[0] = "当前难度 困难";
					wordTypeAndDetailsArray[1] = "雅思\n高级词汇";
					wordTypeAndDetailsArray[2] = "托福\n高级词汇";
					wordTypeAndDetailsArray[3] = "GRE\n高级词汇";
                    break;
            }

			return wordTypeAndDetailsArray;
        }

		/// <summary>
		/// 初始化学习记录页
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		public void SetUpGeneralLearningInfo(){

			string[] wordTypeDetailsArray = GetWordTypeAndDetails();

			for (int i = 0; i < wordTypeDetailsArray.Length;i++){
				
				Text wordTypeDetailText = wordTypeAndDetails[i];

				wordTypeDetailText.text = wordTypeDetailsArray[i];
			}

			int correctPercentageMultiply100 = learnedWordsCountOfCurrentType == 0 ? 0 : (learnedWordsCountOfCurrentType - wrongWordsCountOfCurrentType) * 100 / learnedWordsCountOfCurrentType;
         
			correctPercentageText.text = string.Format("<size=60>{0}</size> %", correctPercentageMultiply100);

			learnedWordCountText.text = learnedWordsCountOfCurrentType.ToString();

			wrongWordsCountText.text = wrongWordsCountOfCurrentType.ToString();

			Player.mainPlayer.totalLearnedWordCount = learnedWordsCountOfCurrentType;

			Player.mainPlayer.totalUngraspWordCount = wrongWordsCountOfCurrentType;

			for (int i = 0; i < recordDetailCells.Length;i++){
				recordDetailCells[i].SetUpRecordDetailCell(learnedWordsCountOfCurrentType,wrongWordsCountOfCurrentType);
			}

			generalInfoPlane.gameObject.SetActive (true);
			wordsPlane.gameObject.SetActive (false);

			UpdateTabStatus(0);
         
			GetComponent<Canvas> ().enabled = true;
            
		}
        
        /// <summary>
        /// 显示指定单词的详细信息界面
        /// </summary>
        /// <param name="word">Word.</param>
        /// <param name="wordIndexInList">Word index in list.</param>
		public void ShowWordDetail(HLHWord word,int wordIndexInList){         
			wordDetail.SetUpWordDetailHUD(word,wordIndexInList);         
		}
        
      
        /// <summary>
        /// 当某个单词列表内没有单词时，将页数显示
        /// </summary>
		public void HidePageDisplay(){
			pageOperationContainer.gameObject.SetActive(false);
		}

        /// <summary>
        /// 显示页数
        /// </summary>
		public void ShowPageDisplay(){
			pageOperationContainer.gameObject.SetActive(true);
		}
        
        /// <summary>
        /// 显示单词切换单词本的询问界面
        /// </summary>
        /// <param name="word">Word.</param>
		private void ShowChangeWordStatusQueruHUD(HLHWord word){
			changeWordStatusQueryHUD.SetUpChangeWordStatusQueryHUD(word,changeWordStatusCallBack);
		}

        /// <summary>
        /// 更新指定页面的已学习单词
        /// </summary>
		public void UpdateWordsOfCurrentPage(List<HLHWord> words , int currentPageIndex, int totalPage){

			wordPool.AddChildInstancesToPool(wordContainer);

			int minWordIndexOfCurrentPage = currentPageIndex * CommonData.singleWordsRecordsPageVolume;
            int maxWordIndexOfCurrentPage = (currentPageIndex + 1) * CommonData.singleWordsRecordsPageVolume - 1;

            maxWordIndexOfCurrentPage = maxWordIndexOfCurrentPage > words.Count ? words.Count : maxWordIndexOfCurrentPage;

			for (int i = 0; i < words.Count;i++){

				HLHWord word = words[i];

				WordItemView wordItem = wordPool.GetInstance<WordItemView>(wordModel.gameObject, wordContainer);

				int indexInCurrentPage = i;

				wordItem.InitWordItemView(ShowChangeWordStatusQueruHUD, delegate{
					ShowWordDetail(word, minWordIndexOfCurrentPage + indexInCurrentPage);
				},wordPool);
                wordItem.SetUpCellDetailView(word);
			}

			generalInfoPlane.gameObject.SetActive(false);
            wordsPlane.gameObject.SetActive(true);
            
			wordPageText.text = string.Format("{0}/{1}", currentPageIndex + 1, totalPage);

			GetComponent<Canvas>().enabled = true;
		}
      
        /// <summary>
        /// 更新底部tab button
        /// </summary>
        /// <param name="tabIndex">Tab index.</param>
		public void UpdateTabStatus(int tabIndex){
		
			switch (tabIndex)
            {
				case 0:
					recordTitle.color = CommonData.selectedColor;
					graspedWordsTitle.color = CommonData.deselectedColor;
					ungraspedWordsTitle.color = CommonData.deselectedColor;
					currentTabTitleText.text = "词库学习记录";
					generalInfoPlane.gameObject.SetActive(true);
					wordsPlane.gameObject.SetActive(false);
					break;
                case 1:
					recordTitle.color = CommonData.deselectedColor;
					graspedWordsTitle.color = CommonData.selectedColor;
					ungraspedWordsTitle.color = CommonData.deselectedColor;
					currentTabTitleText.text = "已掌握单词";
					wordPool.AddChildInstancesToPool(wordContainer);
					generalInfoPlane.gameObject.SetActive(false);
					wordsPlane.gameObject.SetActive(true);
                    break;
                case 2:
					recordTitle.color = CommonData.deselectedColor;
					graspedWordsTitle.color = CommonData.deselectedColor;
					ungraspedWordsTitle.color = CommonData.selectedColor;
					currentTabTitleText.text = "未掌握单词";
					wordPool.AddChildInstancesToPool(wordContainer);
					generalInfoPlane.gameObject.SetActive(false);
                    wordsPlane.gameObject.SetActive(true);
                    break;
            }

		}


		/// <summary>
		/// 退出整个单词记录几面
		/// </summary>
		/// <param name="cb">Cb.</param>
		public void QuitRecordPlane(){
			ClearCache ();
			GetComponent<Canvas> ().enabled = false;
		}

		private void ClearCache(){
			Destroy (wordPool.gameObject);
		}



	}
}
