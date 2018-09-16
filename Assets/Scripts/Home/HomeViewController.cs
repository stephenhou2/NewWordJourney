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
		}
        

		public void OnConfirmNetStatusButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip(CommonData.buttonClickAudioName);
			homeView.HideNoAvalableNetHintHUD();
		}



		public void OnConfirmBagToGoldButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip(CommonData.buttonClickAudioName);
			homeView.HideBagToGoldHintHUD();
		}


		public void OnExploreButtonClick(){
         
			if (Player.mainPlayer.needChooseDifficulty) {

				GameManager.Instance.soundManager.PlayAudioClip (CommonData.paperAudioName);

				homeView.SetUpDifficultyChoosePlane (OnDifficultyChoose);
			} else {

				homeView.ShowMaskImage ();

				GameManager.Instance.soundManager.StopBgm();

				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.loadingCanvasBundleName,"LoadingCanvas",delegate{
					TransformManager.FindTransform("LoadingCanvas").GetComponent<LoadingViewController>().SetUpLoadingView(LoadingType.EnterExplore, LoadExplore, ShowExplore);	
				});

			}

		}



		/// <summary>
		/// 玩家选择游戏难度【0:简单 1:中等 2:困难】
		/// </summary>
		/// <param name="difficulty">Difficulty.</param>
		public void OnDifficultyChoose(){

			Player.mainPlayer.needChooseDifficulty = false;

			homeView.ShowMaskImage ();

			GameManager.Instance.soundManager.StopBgm();

			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.loadingCanvasBundleName, "LoadingCanvas", delegate {
				TransformManager.FindTransform("LoadingCanvas").GetComponent<LoadingViewController>().SetUpLoadingView(LoadingType.EnterExplore, LoadExplore,ShowExplore);
            });

		}

		private void ShowExplore(){
			TransformManager.FindTransform("ExploreCanvas").GetComponent<ExploreUICotroller>().ShowExploreSceneSlowly();
		}

		private void LoadExplore(){
			IEnumerator loadExploreCoroutine = LoadExploreData();
			StartCoroutine(loadExploreCoroutine);
		}


		private IEnumerator LoadExploreData(){

			yield return null;

			QuitHomeView();

			GameManager.Instance.UIManager.RemoveMultiCanvasCache (new string[] {
				"HomeCanvas",
				"RecordCanvas",
				"SettingCanvas",
				"SpellCanvas",
				"LearnCanvas",
                "PlayRecordCanvas"
			});

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.exploreSceneBundleName, "ExploreCanvas", () => {
				if(Player.mainPlayer.mapIndexRecord.Count == 0){
					Player.mainPlayer.InitializeMapIndex();
				}            
				ExploreManager.Instance.SetUpExploreView(MapSetUpFrom.Home);            
			},false,true);

		}

		public void OnRecordButtonClick(){

			GameManager.Instance.soundManager.PlayAudioClip (CommonData.buttonClickAudioName);
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.recordCanvasBundleName, "RecordCanvas", () => {
				TransformManager.FindTransform("RecordCanvas").GetComponent<RecordViewController> ().SetUpRecordView();
				homeView.OnQuitHomeView();
			},false,true);
		}

      
		public void OnBagButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip (CommonData.buttonClickAudioName);
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
				TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController> ().SetUpBagView (true);
				homeView.OnQuitHomeView();
			},false,true);
		}

		public void OnSettingButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip (CommonData.buttonClickAudioName);
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController> ().SetUpSettingView ();
				homeView.OnQuitHomeView();
			},false,true);
		}

		//public void OnSpellButtonClick(){
		//	GameManager.Instance.soundManager.PlayAudioClip (CommonData.buttonClickAudioName);
		//	GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
		//		TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(null,null);
		//		homeView.OnQuitHomeView();
		//	},false,true);
		//}

		public void OnCommentButtonClick()
		{
#if UNITY_IPHONE || UNITY_EDITOR
			const string APP_ID = "1340788161";
			var url = string.Format("itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review", APP_ID);
			Application.OpenURL(url);         
#elif UNITY_ANDROID
			homeView.tintHUD.SetUpSingleTextTintHUD("评论功能暂未开放，敬请期待!");
#endif
		}

		public void OnWeChatShareButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip(CommonData.buttonClickAudioName);
         
			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", () =>
			{
				TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.WeChat, ShareSucceedCallBack, ShareFailedCallBack, null);
				homeView.OnQuitHomeView();
			}, false, true);

		}

		public void OnPlayRecordButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip(CommonData.buttonClickAudioName);

			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.playRecordCanvasBundleName, "PlayRecordCanvas", () =>
			{
				TransformManager.FindTransform("PlayRecordCanvas").GetComponent<PlayRecordViewController>().SetUpPlayerRecordView();
			}, false, true);

		}

		private void ShareSucceedCallBack(){
			
			string tintStr = "分享成功";    

			homeView.tintHUD.SetUpSingleTextTintHUD(tintStr);
		}

		private void ShareFailedCallBack(){
			string tintStr = "分享失败，未检测到客户端";
			homeView.tintHUD.SetUpSingleTextTintHUD(tintStr);
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
