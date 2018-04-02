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


		public Button recordTitle;
		public Button wrongWordsTitle;

		public Sprite normalSprite;
		public Sprite selectedSprite;


		public Text wordType;

		public Image completionImage;

		public Text correctPercentageText;

		public Text learnedWordsCountText;

		public Text unGraspedWordsCountText;

		// 单词cell模型
		public Transform wordModel;

		// 复用缓存池
		public InstancePool wordPool;

		public Transform wordContainer;

		private LearningInfo learnInfo;


		/// <summary>
		/// 初始化记录页面
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		/// <param name="tabIndex">选项卡序号 【0:基本信息 1:错误单词】.</param>
		public void SetUpRecordView(LearningInfo learnInfo){

			this.learnInfo = learnInfo;

			// 创建缓存池
//			wordPool = InstancePool.GetOrCreateInstancePool ("WordItemPool",CommonData.poolContainerName);

			SetUpGeneralLearningInfo ();

			GetComponent<Canvas>().enabled = true;

		}

		/// <summary>
		/// 初始化学习记录页
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		public void SetUpGeneralLearningInfo(){

			string wordTypeStr = GameManager.Instance.gameDataCenter.gameSettings.GetWordTypeString ();

			wordType.text = wordTypeStr;

			float percentage = 0;

			int totalWordsCount = learnInfo.totalWordCount;
			int learnedWordsCount = learnInfo.learnedWordCount;
			int wrongWordsCount = learnInfo.ungraspedWordCount;

			if (learnedWordsCount != 0) {
				percentage = (float)(learnedWordsCount - wrongWordsCount) / learnedWordsCount;
			}
			 

			completionImage.fillAmount = percentage;

			correctPercentageText.text = ((int)(percentage * 100)).ToString() + "%";

			learnedWordsCountText.text = learnedWordsCount.ToString ();

			unGraspedWordsCountText.text = wrongWordsCount.ToString ();

			generalInfoPlane.gameObject.SetActive (true);
			wordsPlane.gameObject.SetActive (false);

			recordTitle.GetComponent<Image> ().sprite = selectedSprite;
			recordTitle.GetComponentInChildren<Text>().color = new Color (
				CommonData.selectedColor.x, 
				CommonData.selectedColor.y, 
				CommonData.selectedColor.z);
			wrongWordsTitle.GetComponent<Image> ().sprite = normalSprite;
			wrongWordsTitle.GetComponentInChildren<Text>().color = new Color (
				CommonData.deselectedColor.x, 
				CommonData.deselectedColor.y, 
				CommonData.deselectedColor.z);

			GetComponent<Canvas> ().enabled = true;

		}

		/// <summary>
		/// 初始化已学习页
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
//		public void SetUpAllLearnedWords(){
//
//			wordsPlane.gameObject.SetActive (true);
//
//			List<LearnWord> allLearnedWords = learnInfo.GetAllLearnedWords ();
//
//			for (int i = 0; i < allLearnedWords.Count; i++) {
//
//				LearnWord word = allLearnedWords [i];
//
//				Transform wordItem = wordPool.GetInstance <Transform> (wordModel.gameObject, wordContainer);
//
//				wordItem.GetComponent<WordItemView> ().SetUpCellDetailView (word);
//
//			}
//
//		}

		/// <summary>
		/// 初始化未学习页
		/// </summary>
		/// <param name="learnInfo">Learn info.</param>
		public void SetUpAllUngraspedWords(){
			
			List<LearnWord> allUngraspedWords = learnInfo.GetAllUngraspedWords ();
		
			wordPool.AddChildInstancesToPool (wordContainer);

			for (int i = 0; i < allUngraspedWords.Count; i++) {

				LearnWord word = allUngraspedWords [i];

				Transform wordItem = wordPool.GetInstance <Transform> (wordModel.gameObject, wordContainer);

				wordItem.GetComponent<WordItemView> ().SetUpCellDetailView (word);

			}

			generalInfoPlane.gameObject.SetActive (false);
			wordsPlane.gameObject.SetActive (true);

			wrongWordsTitle.GetComponent<Image> ().sprite = selectedSprite;
			wrongWordsTitle.GetComponentInChildren<Text>().color = new Color (
				CommonData.selectedColor.x, 
				CommonData.selectedColor.y, 
				CommonData.selectedColor.z);
			recordTitle.GetComponent<Image> ().sprite = normalSprite;
			recordTitle.GetComponentInChildren<Text>().color = new Color (
				CommonData.deselectedColor.x, 
				CommonData.deselectedColor.y, 
				CommonData.deselectedColor.z);

			GetComponent<Canvas> ().enabled = true;
				
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

		void OnDestroy(){
//			normalSprite = null;
//			selectedSprite = null;
//			wordPool = null;
//			learnInfo = null;
		}

	}
}
