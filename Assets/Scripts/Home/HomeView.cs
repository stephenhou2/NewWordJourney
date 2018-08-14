using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace WordJourney
{

	using UnityEngine.UI;

	public class HomeView : MonoBehaviour {


		public Image maskImage;

		public Transform logoBandage;

		public DifficultySelectHUD difficultySelectHUD;

		public TintHUD tintHUD;

		public float logoFloatSpeed;

		public float singleDuration;

		private IEnumerator logoFloatCoroutine;

		public Transform noAvalableNetHintHUD;

		public void SetUpHomeView(){

			GetComponent<Canvas> ().enabled = true;

			LogoBandageStartFloat();

		}
			

		public void ShowMaskImage (){
			maskImage.gameObject.SetActive (true);
		}



		private void HideMaskImage(){
			maskImage.gameObject.SetActive (false);
		}


		public void ShowNoAvalableNetHintHUD(){
			noAvalableNetHintHUD.gameObject.SetActive(true);
		}

		public void HideNoAvalableNetHintHUD(){
			noAvalableNetHintHUD.gameObject.SetActive(false);
		}
        
		/// <summary>
		/// 初始化难度选择面板
		/// </summary>
		public void SetUpDifficultyChoosePlane(CallBack selectDifficultyCallBack){
                 
			difficultySelectHUD.SetUpDifficultySelectHUD(selectDifficultyCallBack);

		}

        
		private void LogoBandageStartFloat(){
			if(logoFloatCoroutine == null){
				logoFloatCoroutine = LogoBandageFloat();
				StartCoroutine(logoFloatCoroutine);
			}
		}
        

		private IEnumerator LogoBandageFloat(){

			logoBandage.localPosition = Vector3.zero;

			float timer = 0;

			while(true){

				while(timer<singleDuration){

					Vector3 moveVector = new Vector3(0, -logoFloatSpeed * Time.deltaTime, 0);

					logoBandage.localPosition += moveVector;

					yield return null;

					timer += Time.deltaTime;

				}

				timer = 0;

				while(timer<singleDuration){

					Vector3 moveVector = new Vector3(0, logoFloatSpeed * Time.deltaTime, 0);

                    logoBandage.localPosition += moveVector;

                    yield return null;

                    timer += Time.deltaTime;               
				}

				timer = 0;

			}

		}

//		public void SetUpChapterSelectPlane(){
//
//			int maxUnlockChapterIndex = Player.mainPlayer.maxUnlockLevelIndex / 5;
//
//			for (int i = 0; i < chapterButtons.Length; i++) {
//
//				Button chapterButton = chapterButtons [i];
//
//				Text chapterNameText = chapterButton.GetComponentInChildren<Text> ();
//
//				if (i <= maxUnlockChapterIndex) {
//					
//					chapterButton.interactable = true;
//
//					string chapterName = GameManager.Instance.gameDataCenter.gameLevelDatas [5 * i].chapterName;
//
//					string chapterIndexInChinese = MyTool.NumberToChinese (i + 1);
//
//					string fullName = string.Format ("第{0}章  {1}", chapterIndexInChinese, chapterName);
//
//					chapterNameText.text = fullName;
//
//				} else {
//					chapterButton.interactable = false;
//					chapterNameText.text = "? ? ? ?";
//				}
//
//			}
//
//			chaptersContainer.localScale = new Vector3 (0.1f, 0.1f, 1);
//
//			chapterSelectPlane.gameObject.SetActive (true);
//
//			IEnumerator chapterSelectZoomInCoroutine = ChapterSelectHUDZoomIn ();
//
//			StartCoroutine (chapterSelectZoomInCoroutine);
//
//		}
//
//		private IEnumerator ChapterSelectHUDZoomIn(){
//			
//
//			float chapterSelectHUDScale = chaptersContainer.localScale.x;
//
//			float chapterSelectHUDZoomSpeed = (1 - chapterSelectHUDScale) / chapterSelectPlaneZoomInDuration;
//
//			while (chapterSelectHUDScale < 1) {
//				float zoomInDelta = chapterSelectHUDZoomSpeed * Time.deltaTime;
//				chaptersContainer.localScale += new Vector3 (zoomInDelta, zoomInDelta, 0);
//				chapterSelectHUDScale += zoomInDelta;
//				yield return null;
//			}
//
//			chaptersContainer.localScale = Vector3.one;
//
//		}
//
//		public void QuitChapterSelectPlane(){
//			chapterSelectPlane.gameObject.SetActive (false);
//		}

		public void OnQuitHomeView(){

			difficultySelectHUD.gameObject.SetActive(false);

			HideMaskImage ();
		}

	}
}
