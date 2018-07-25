using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{
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

		public Transform wordContainer;

		public Transform pageOperationContainer;

		private LearningInfo learnInfo;

		public WordRecordDetailHUD wordDetail;
              
		private CallBackWithWord changeWordStatusCallBack;

		public ChangeWordStatusQueryView changeWordStatusQueryHUD;

		//private int totalWordsCountOfCurrentType;

		private int learnedWordsCountOfCurrentType;

		private int wrongWordsCountOfCurrentType;


		/// <summary>
		/// 初始化记录页面
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		/// <param name="tabIndex">选项卡序号 【0:基本信息 1:错误单词】.</param>
		public void SetUpRecordView(LearningInfo learnInfo,CallBackWithWord changeStatusCallBack,CallBackWithInt nextWordDetailClickCallBack,CallBackWithInt lastWordDetailClickCallBack){

			this.learnInfo = learnInfo;

			//totalWordsCountOfCurrentType = learnInfo.totalWordCount;

			learnedWordsCountOfCurrentType = learnInfo.learnedWordCount;

			wrongWordsCountOfCurrentType = learnInfo.ungraspedWordCount;

			this.changeWordStatusCallBack = changeStatusCallBack;

			SetUpGeneralLearningInfo ();

			wordDetail.InitWordRecordDetailHUD(nextWordDetailClickCallBack,lastWordDetailClickCallBack);

			GetComponent<Canvas>().enabled = true;

		}

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

		public void ShowWordDetail(HLHWord word,int wordIndexInList){         
			wordDetail.SetUpWordDetailHUD(word,wordIndexInList);         
		}
        
		//public void SetUpGraspWords(List<HLHWord> words, int currentPageIndex,int totalPage){

		//	int minWordIndexOfCurrentPage = currentPageIndex * CommonData.singleWordsRecordsPageVolume;
		//	int maxWordIndexOfCurrentPage = (currentPageIndex + 1) * CommonData.singleWordsRecordsPageVolume - 1;

  //          maxWordIndexOfCurrentPage = maxWordIndexOfCurrentPage > words.Count ? words.Count : maxWordIndexOfCurrentPage;


		//	for (int j = minWordIndexOfCurrentPage; j <= maxWordIndexOfCurrentPage; j++)
  //          {
                
  //              HLHWord word = words[j];

		//		WordItemView wordItem = wordPool.GetInstance<WordItemView>(wordModel.gameObject, wordContainer);

		//		int indexInCurrentPage = j;

		//		wordItem.InitWordItemView(ShowChangeWordStatusQueruHUD, delegate {               
		//			ShowWordDetail(word, minWordIndexOfCurrentPage + indexInCurrentPage);
		//		}, wordPool);
  //              wordItem.SetUpCellDetailView(word);            

  //          }

		//	wordPageText.text = string.Format("{0}/{1}", currentPageIndex + 1, totalPage);
         
  //          generalInfoPlane.gameObject.SetActive(false);
  //          wordsPlane.gameObject.SetActive(true);
            
		//	UpdateTabStatus(1);

  //          GetComponent<Canvas>().enabled = true;         
		//}

		//public void SetUpUngraspWords(List<HLHWord> words,int currentPageIndex, int totalPage){
                 
		//	int minWordIndexOfCurrentPage = currentPageIndex * CommonData.singleWordsRecordsPageVolume;
		//	int maxWordIndexOfCurrentPage = (currentPageIndex + 1) * CommonData.singleWordsRecordsPageVolume - 1;

		//	maxWordIndexOfCurrentPage = maxWordIndexOfCurrentPage > words.Count ? words.Count : maxWordIndexOfCurrentPage;

		//	for (int j = minWordIndexOfCurrentPage; j <= maxWordIndexOfCurrentPage; j++)
  //          {

		//		HLHWord word = words[j];

		//		WordItemView wordItem = wordPool.GetInstance<WordItemView>(wordModel.gameObject, wordContainer);

		//		int indexInCurrentPage = j;

		//		wordItem.InitWordItemView(ShowChangeWordStatusQueruHUD,delegate {               
		//			ShowWordDetail(word, minWordIndexOfCurrentPage + indexInCurrentPage);
		//		}, wordPool);
  //              wordItem.SetUpCellDetailView(word);            
  //          }

		//	wordPageText.text = string.Format("{0}/{1}", currentPageIndex + 1, totalPage);
                       
  //          generalInfoPlane.gameObject.SetActive(false);
  //          wordsPlane.gameObject.SetActive(true);

		//	UpdateTabStatus(2);
         
  //          GetComponent<Canvas>().enabled = true;

		//}

        /// <summary>
        /// 当某个单词列表内没有单词时，将页数显示
        /// </summary>
		public void HidePageDisplay(){
			pageOperationContainer.gameObject.SetActive(false);
		}

		public void ShowPageDisplay(){
			pageOperationContainer.gameObject.SetActive(true);
		}
        

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
      
		public void UpdateTabStatus(int tabIndex){
		
			switch (tabIndex)
            {
				case 0:
					recordTitle.color = CommonData.selectedColor;
					graspedWordsTitle.color = CommonData.deselectedColor;
					ungraspedWordsTitle.color = CommonData.deselectedColor;
					currentTabTitleText.text = "学习记录";
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
