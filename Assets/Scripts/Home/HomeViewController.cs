using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


namespace WordJourney
{
	public class HomeViewController : MonoBehaviour {

		public HomeView homeView;

		public void SetUpHomeView(){

			homeView.SetUpHomeView ();

			Time.timeScale = 1f;


			if (!GameManager.Instance.soundManager.bgmAS.isPlaying 
				|| GameManager.Instance.soundManager.bgmAS.clip.name != CommonData.homeBgmName) {
				GameManager.Instance.soundManager.PlayBgmAudioClip (CommonData.homeBgmName, true);
			}

		}
			

		public void OnExploreButtonClick(){
			
			PlayerData playerData = GameManager.Instance.persistDataManager.LoadPlayerData ();

			Player.mainPlayer.SetUpPlayerWithPlayerData (playerData);


			if (Player.mainPlayer.isNewPlayer) {

				GameManager.Instance.soundManager.PlayAudioClip ("UI/sfx_UI_Paper");

				homeView.SetUpDifficultyChoosePlane ();
			} else {

				homeView.ShowMaskImage ();

				StartCoroutine ("LoadExploreData");

			}

		}

		public void QuitDifficultyChoosePlane(){
			homeView.QuitDifficultyChoosePlane ();
		}

		public void SelectChapter(int chapterIndex){

//			e.PlayAudioClip ("UI/sfx_UI_Click");
			
//			Player.mainPlayer.currentLevelIndex = 5 * chapterIndex;

			homeView.ShowMaskImage ();

			StartCoroutine ("LoadExploreData");

			#warning 下面这个代码是使用场景管理器方式加载探索界面
//			SceneManager.LoadSceneAsync ("ExploreScene", LoadSceneMode.Single);

		}

		/// <summary>
		/// 玩家选择游戏难度【0:简单 1:中等 2:困难】
		/// </summary>
		/// <param name="difficulty">Difficulty.</param>
		public void OnDifficultyChoose(int difficulty){

			GameManager.Instance.soundManager.PlayAudioClip ("UI/sfx_UI_Click");

			switch (difficulty) {
			case 0:
				GameManager.Instance.gameDataCenter.gameSettings.wordType = WordType.Simple;
				break;
			case 1:
				GameManager.Instance.gameDataCenter.gameSettings.wordType = WordType.Medium;
				break;
			case 2:
				GameManager.Instance.gameDataCenter.gameSettings.wordType = WordType.Master;
				break;
			}

			GameManager.Instance.persistDataManager.SaveGameSettings ();

			homeView.ShowMaskImage ();

			StartCoroutine ("LoadExploreData");

		}


		private IEnumerator LoadExploreData(){

			yield return null;

			QuitHomeView();

			GameManager.Instance.UIManager.RemoveMultiCanvasCache (new string[] {
				"UnlockedItemsCanvas",
				"RecordCanvas",
				"SettingCanvas",
				"SpellCanvas",
				"LearnCanvas"
			});

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", () => {

				ExploreManager.Instance.SetUpExploreView();

			},false,false);

		}

		public void OnRecordButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip ("UI/sfx_UI_Click");
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.recordCanvasBundleName, "RecordCanvas", () => {
				TransformManager.FindTransform("RecordCanvas").GetComponent<RecordViewController> ().SetUpRecordView();
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnLearnButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip ("UI/sfx_UI_Click");
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.learnCanvasBundleName, "LearnCanvas", () => {
				TransformManager.FindTransform("LearnCanvas").GetComponent<LearnViewController>().SetUpLearnView();
				homeView.OnQuitHomeView();
			},false,true);

		}
			

		public void OnBagButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip ("UI/sfx_UI_Click");
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
				TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController> ().SetUpBagView (true);
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnSettingButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip ("UI/sfx_UI_Click");
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController> ().SetUpSettingView ();
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnSpellButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip ("UI/sfx_UI_Click");
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(null,null);
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnCommentButtonClick(){
			#if UNITY_IPHONE || UNITY_EDITOR
			#warning 这里是应用的apple id
			const string APP_ID = "564457517"; 
			var url = string.Format("itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review",APP_ID);
			Application.OpenURL(url);
			#endif
		}


		private void QuitHomeView(){
			homeView.OnQuitHomeView ();
			GameManager.Instance.UIManager.RemoveCanvasCache ("HomeCanvas");
		}

		public void DestroyInstances(){

			GetComponent<Canvas> ().enabled = false;

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.homeCanvasBundleName, true);

			Destroy (this.gameObject,0.3f);

		}

	}
}
