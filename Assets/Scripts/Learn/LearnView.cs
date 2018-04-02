using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class LearnView : MonoBehaviour {

		// 遮罩
		public Transform mask;

		public Text wordTypeText;

		public Image pronounceOnImage;
		public Image pronounceOffImage;


		// 考察的单词时的英文单词文本 或者 考察从中文释义到英文单词时的释义文本
		public Text questionText;

		// 音标文本
		public Text phoneticSymbolText;

		// 考察单词时对应的中文释义文本
		public Text explainationText;

		// 自查单词掌握情况的按钮容器
		public Transform graspConditionButtonsContainer;

		// 学习单词时学习操作按钮容器
		public Transform wordsLearnOperationButtonsContainer;

		// 单词问题的答案按钮容器
		public Transform choicesContainer;

		// 单词答案选项按钮数组
		public Button[] choices;


		public Text correctExplaination;

		// 单词释义淡出时间
		public float explainationFadeOutDuration;

//		private Button currentSelectChoice;

		public Transform learnProgressContainer;

		public Image learnProgressBar;

		public Text learnProgress;


		private Tweener characterFragmentAnim;

		public Transform quitQueryHUD;

		public Transform learningResultHUD;

		public Text crystalHarvestCount;

		public Transform crystalContainer;

		public InstancePool crystalPool;

		public Transform crystalModel;

		public Text cystalHarvestInFinishHUD;
		public Text correctPercentageInFinishHUD;

		private int totalTurnCount;

		private bool isShowRightAnswerFinished;

		public void InitLearnView(int totalWordsCount){
			
			crystalHarvestCount.text = "0";

			UpdateLearningProgress (0, totalWordsCount,false);

			totalTurnCount = 0;

			isShowRightAnswerFinished = true;

//			if (crystalPool == null) {
//				crystalPool = InstancePool.GetOrCreateInstancePool ("CrystalPool", CommonData.learnCanvasPoolContainerName);
//			}
			SetUpBasicInformation ();

		}


		private void SetUpBasicInformation(){

			wordTypeText.text = GameManager.Instance.gameDataCenter.gameSettings.GetWordTypeString ();

			UpdatePronounceControl (GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce);

		}

		public void UpdatePronounceControl(bool enable){

			if (!enable) {
				pronounceOnImage.gameObject.SetActive (false);
				pronounceOffImage.gameObject.SetActive (true);
			} else {
				pronounceOnImage.gameObject.SetActive (true);
				pronounceOffImage.gameObject.SetActive (false);
			}

		}



		public void SetUpLearnViewWithWord(LearnWord word){

			questionText.text = word.spell;

			phoneticSymbolText.text = word.phoneticSymbol;

			explainationText.text = word.explaination;

			explainationText.enabled = false;

			ShowContainers (true, false, false,false);

			GetComponent<Canvas> ().enabled = true;

		}

		private void DisableInteractivity(){
			mask.gameObject.SetActive (true);
		}

		private void EnableInteractivity(){
			mask.gameObject.SetActive (false);
		}


		public void SetUpLearningProgress(){

			explainationText.enabled = true;
			explainationText.color = new Color (1, 1, 1, 1);

			ShowContainers (false, false, false,false);

			DisableInteractivity ();

			StartCoroutine ("ExplainationShowAndHideAnim");

		}


		private IEnumerator ExplainationShowAndHideAnim(){

			yield return new WaitForSecondsRealtime (2.5f);

			Tween colorTween = explainationText.DOColor (new Color (1, 1, 1, 0), explainationFadeOutDuration).OnComplete (delegate {
				wordsLearnOperationButtonsContainer.gameObject.SetActive (true);
				explainationText.enabled = false;
				explainationText.color = new Color(1,1,1,1);
			});

			colorTween.SetUpdate (true);

			EnableInteractivity ();
		}

		public void ShowExplaination(){
			explainationText.enabled = true;
		}
			


		public void SetUpLearnViewWithFinalExam(Examination exam,Examination.ExaminationType examType){

			IEnumerator waitShowRightAnswerFinishCoroutine = WaitShowRightAnswerFinish (delegate {
				
				MySetUpLearnViewWithFinalExam(exam,examType);

				GetComponent<Canvas> ().enabled = true;

				if(GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce){
					GameManager.Instance.pronounceManager.PronounceWord(exam.question);
				}

			});

			StartCoroutine (waitShowRightAnswerFinishCoroutine);
		}

		private void MySetUpLearnViewWithFinalExam(Examination exam,Examination.ExaminationType examType){

			totalTurnCount++;

			switch (examType) {

			case Examination.ExaminationType.EngToChn:

				questionText.text = exam.question.spell;

				phoneticSymbolText.text = exam.question.phoneticSymbol;

				phoneticSymbolText.enabled = true;

				for (int i = 0; i < choices.Length; i++) {

					Button choiceButton = choices [i];

					choiceButton.GetComponentInChildren<Text> ().color = Color.white;

					LearnWord answer = exam.answers [i];

					choiceButton.GetComponentInChildren<Text>().text = answer.explaination;

					choiceButton.onClick.RemoveAllListeners ();

//					int currentSelectChoiceIndex = i;

					choiceButton.onClick.AddListener (delegate {
//						currentSelectChoice = choices[currentSelectChoiceIndex];
						GetComponent<LearnViewController>().OnAnswerChoiceButtonOfFinalExamsClick(answer);
					});

				}
				break;
			case Examination.ExaminationType.ChnToEng:

				questionText.text = exam.question.explaination;

				phoneticSymbolText.enabled = false;

				for (int i = 0; i < choices.Length; i++) {

					Button choiceButton = choices [i];

					LearnWord answer = exam.answers [i];

					choiceButton.GetComponentInChildren<Text>().text = answer.spell;

					choiceButton.onClick.RemoveAllListeners ();

//					int currentSelectChoiceIndex = i;

					choiceButton.onClick.AddListener (delegate {
//						currentSelectChoice = choices[currentSelectChoiceIndex];
						GetComponent<LearnViewController>().OnAnswerChoiceButtonOfFinalExamsClick(answer);
					});

				}
				break;
			}

			correctExplaination.text = exam.question.explaination;

			correctExplaination.enabled = false;

			ShowContainers (false, false, true,true);

		}

		public void ShowContainers(bool graspCondition,bool wordsLearnOperation,bool answers,bool energySlider){

			graspConditionButtonsContainer.gameObject.SetActive (graspCondition);
			wordsLearnOperationButtonsContainer.gameObject.SetActive (wordsLearnOperation);
			choicesContainer.gameObject.SetActive (answers);
			learnProgressContainer.gameObject.SetActive (energySlider);

		}
			

		public void ShowRightAnswerAndEnterNextExam(int correctAnswerIndex, Examination nextExam){

			DisableInteractivity ();

			isShowRightAnswerFinished = false;

			ShowRightExplaination ();

//			Transform correctAnswer = choices [correctAnswerIndex];
//
//			correctAnswer.Find ("ChoiceButton").GetComponentInChildren<Text> ().color = Color.green;
//
//			currentSelectChoice.Find ("ChoiceButton").GetComponentInChildren<Text> ().color = Color.red;

			if (nextExam == null) {
				StartCoroutine ("StopForAWhile");
				return;
			}
			StartCoroutine("StopForAWhileAndEnterNextExam",nextExam);
		}

		private IEnumerator StopForAWhile(){
			
			yield return new WaitForSecondsRealtime (2f);

			isShowRightAnswerFinished = true;

			EnableInteractivity ();
		}

		private IEnumerator StopForAWhileAndEnterNextExam(Examination nextExam){
			
			yield return new WaitForSecondsRealtime (2f);

//			SetUpLearnViewWithFinalExam (nextExam, nextExam.GetCurrentExamType ());

			isShowRightAnswerFinished = true;

			HideRightExplaination ();

			EnableInteractivity ();
		}

		private void ShowRightExplaination(){
			
			choicesContainer.gameObject.SetActive (false);

			correctExplaination.enabled = true;
		}

		private void HideRightExplaination(){
			
			choicesContainer.gameObject.SetActive (true);

			correctExplaination.enabled = false;
		}
			

		public void UpdateLearningProgress(int learnedCount, int totalCount,bool isAnim){
			float fillAmount = (float)learnedCount / totalCount;
			if (isAnim) {
				Tween progressBarUpdate = learnProgressBar.DOFillAmount (fillAmount, 0.5f);
				progressBarUpdate.SetUpdate (true);
			} else {
				learnProgressBar.fillAmount = fillAmount;
			}
			learnProgress.text = string.Format ("{0}/{1}", learnedCount.ToString(), totalCount.ToString ());
		}

		public void ShowFinishLearningHUD(int harvestCount,int correctWordCount){
			IEnumerator waitShowRightAnswerFinishRoroutine = WaitShowRightAnswerFinish (delegate {
				learningResultHUD.gameObject.SetActive (true);
				cystalHarvestInFinishHUD.text = harvestCount.ToString ();
				correctPercentageInFinishHUD.text = string.Format ("{0}%", (int)(correctWordCount * 100 / totalTurnCount ));
			});
			StartCoroutine (waitShowRightAnswerFinishRoroutine);
		}

		private IEnumerator WaitShowRightAnswerFinish(CallBack callBack){

			yield return new WaitUntil (() => isShowRightAnswerFinished);

			if (callBack != null) {
				callBack ();
			}
		}
			


		public void ShowQuitQueryHUD(){
			quitQueryHUD.gameObject.SetActive (true);
		}

		public void HideQuitQueryHUD(){
			quitQueryHUD.gameObject.SetActive (false);
		}

		private void HideFinishLearningQuitHUD(){
			learningResultHUD.gameObject.SetActive (false);
		}

		public void UpdateCrystalHarvest(int totalCount){

			Transform crystal = crystalPool.GetInstance<Transform> (crystalModel.gameObject, crystalContainer);

			crystal.localPosition = new Vector3(0,450,0);
			crystal.GetComponent<Image> ().enabled = true;

			Tween crystalMove = crystal.DOLocalMove (new Vector3 (-445, 680, 0), 0.5f).OnComplete (delegate {
				crystal.GetComponent<Image> ().enabled = false;
				crystalPool.AddInstanceToPool(crystal.gameObject);
				crystalHarvestCount.text = totalCount.ToString ();
			});

			crystalMove.SetUpdate (true);
		}

		public void QuitLearnView(){
			
			HideQuitQueryHUD ();
			HideFinishLearningQuitHUD ();

			GetComponent<Canvas> ().enabled = false;
		}

		void OnDestroy(){
//			quitQueryHUD = null;
//			learningResultHUD = null;
//			crystalPool = null;
		}

	}
}
