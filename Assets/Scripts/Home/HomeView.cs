﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace WordJourney
{

	using UnityEngine.UI;

	public class HomeView : MonoBehaviour {


		public Text wordTypeText;
		public Text coinCount;

		public Image maskImage;

		public Transform difficultyChoosePlane;
		public Transform difficultyChooseContainer;

//		public Transform chapterSelectPlane;
//		public Transform chaptersContainer;

//		public Button[] chapterButtons;

		public float zoomInDuration = 0.2f;



		public void SetUpHomeView(){
			
			SetUpBasicInformation ();

			GetComponent<Canvas> ().enabled = true;

		}



		private void SetUpBasicInformation(){

			wordTypeText.text = GameManager.Instance.gameDataCenter.gameSettings.GetWordTypeString ();

			coinCount.text = Player.mainPlayer.totalGold.ToString ();

		}


		public void ShowMaskImage (){
			maskImage.gameObject.SetActive (true);
		}



		private void HideMaskImage(){
			maskImage.gameObject.SetActive (false);
		}
			




		/// <summary>
		/// 初始化难度选择面板
		/// </summary>
		public void SetUpDifficultyChoosePlane(){

			difficultyChoosePlane.gameObject.SetActive (true);

			difficultyChooseContainer.localScale = new Vector3 (0.1f, 0.1f, 1);

			StartCoroutine ("DifficultyChooseHUDZoomIn");

		}

		private IEnumerator DifficultyChooseHUDZoomIn(){


			float difficultyChooseHUDScale = difficultyChooseContainer.localScale.x;

			float difficultyChooseHUDZoomSpeed = (1 - difficultyChooseHUDScale) / zoomInDuration;

			while (difficultyChooseHUDScale < 1) {
				float zoomInDelta = difficultyChooseHUDZoomSpeed * Time.deltaTime;
				difficultyChooseContainer.localScale += new Vector3 (zoomInDelta, zoomInDelta, 0);
				difficultyChooseHUDScale += zoomInDelta;
				yield return null;
			}

			difficultyChooseContainer.localScale = Vector3.one;

		}

		public void QuitDifficultyChoosePlane(){

			StopCoroutine ("DifficultyChooseHUDZoomIn");

			difficultyChoosePlane.gameObject.SetActive (false);

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

//			chapterSelectPlane.gameObject.SetActive (false);

			difficultyChoosePlane.gameObject.SetActive (false);

			HideMaskImage ();
		}

	}
}
