using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


namespace WordJourney
{
    
    /// <summary>
    /// 主界面控制器
    /// </summary>
	public class HomeViewController : MonoBehaviour {

		public HomeView homeView;

        /// <summary>
        /// 初始化主界面
        /// </summary>
		public void SetUpHomeView(){

			homeView.SetUpHomeView ();

			Resources.UnloadUnusedAssets();

			GC.Collect();

			Time.timeScale = 1f;
		}
        
        /// <summary>
        /// 确认网络状态
        /// </summary>
		public void OnConfirmNetStatusButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip(CommonData.buttonClickAudioName);
			homeView.HideNoAvalableNetHintHUD();
		}


        /// <summary>
        /// 确认背包4换做500金币
        /// </summary>
		public void OnConfirmBagToGoldButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip(CommonData.buttonClickAudioName);
			homeView.HideBagToGoldHintHUD();
		}

        /// <summary>
        /// 探索按钮点击响应
        /// </summary>
		public void OnExploreButtonClick(){
         
            // 如果需要选择单词难度
			if (Player.mainPlayer.needChooseDifficulty) {

				GameManager.Instance.soundManager.PlayAudioClip (CommonData.paperAudioName);
                // 显示难度选择界面
				homeView.SetUpDifficultyChoosePlane (OnDifficultyChoose);
			} else {

				homeView.ShowMaskImage ();

				GameManager.Instance.soundManager.StopBgm();

				// 初始化加载界面
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

            // 存储玩家数据
			GameManager.Instance.persistDataManager.SaveCompletePlayerData();
			// 初始化加载界面
			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.loadingCanvasBundleName, "LoadingCanvas", delegate {
				TransformManager.FindTransform("LoadingCanvas").GetComponent<LoadingViewController>().SetUpLoadingView(LoadingType.EnterExplore, LoadExplore,ShowExplore);
            });

		}

        /// <summary>
        /// 显示探索场景
        /// </summary>
		private void ShowExplore(){
			TransformManager.FindTransform("ExploreCanvas").GetComponent<ExploreUICotroller>().ShowExploreSceneSlowly();
		}

        // 加载探索场景
		private void LoadExplore(){
			IEnumerator loadExploreCoroutine = LoadExploreData();
			StartCoroutine(loadExploreCoroutine);
		}

        /// <summary>
        /// 加载探索场景数据
        /// </summary>
        /// <returns>The explore data.</returns>
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

        /// <summary>
        /// 学习记录按钮点击响应
        /// </summary>
		public void OnRecordButtonClick(){

			GameManager.Instance.soundManager.PlayAudioClip (CommonData.buttonClickAudioName);
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.recordCanvasBundleName, "RecordCanvas", () => {
				TransformManager.FindTransform("RecordCanvas").GetComponent<RecordViewController> ().SetUpRecordView();
				homeView.OnQuitHomeView();
			},false,true);
		}
      
        /// <summary>
        /// 设置按钮点击响应
        /// </summary>
		public void OnSettingButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip (CommonData.buttonClickAudioName);
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController> ().SetUpSettingView ();
				homeView.OnQuitHomeView();
			},false,true);
		}

      
        /// <summary>
        /// 评论按钮点击响应
        /// </summary>
		public void OnCommentButtonClick()
		{
#if UNITY_IOS
			const string APP_ID = "1340788161";
			var url = string.Format("itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review", APP_ID);
			Application.OpenURL(url);
#elif UNITY_ANDROID
			Application.OpenURL("https://www.taptap.com/app/141656");
#elif UNITY_EDITOR
			UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
            
            switch (buildTarget)
            {
                case UnityEditor.BuildTarget.Android:
			        Application.OpenURL("https://www.taptap.com/app/141656");
                    break;
                case UnityEditor.BuildTarget.iOS:
        			const string APP_ID = "1340788161";
                    var url = string.Format("itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review", APP_ID);
                    Application.OpenURL(url);
                    break;
            }

			Debug.Log(buildTarget);
#endif


		}

        
        /// <summary>
        /// 微信分享按钮点击响应
        /// </summary>
		public void OnWeChatShareButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip(CommonData.buttonClickAudioName);
         
			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", () =>
			{
				TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.WeChat, ShareSucceedCallBack, ShareFailedCallBack, null);
				homeView.OnQuitHomeView();
			}, false, true);

		}

        /// <summary>
        /// 通过记录按钮点击响应
        /// </summary>
		public void OnPlayRecordButtonClick(){
			GameManager.Instance.soundManager.PlayAudioClip(CommonData.buttonClickAudioName);

			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.playRecordCanvasBundleName, "PlayRecordCanvas", () =>
			{
				TransformManager.FindTransform("PlayRecordCanvas").GetComponent<PlayRecordViewController>().SetUpPlayerRecordView();
			}, false, true);

		}
            
        /// <summary>
        /// 分享成功的提示
        /// </summary>
		private void ShareSucceedCallBack(){
			
			string tintStr = "分享成功";    

			homeView.tintHUD.SetUpSingleTextTintHUD(tintStr);
		}

        /// <summary>
        /// 分享失败的提示
        /// </summary>
		private void ShareFailedCallBack(){
			string tintStr = "分享失败，未检测到客户端";
			homeView.tintHUD.SetUpSingleTextTintHUD(tintStr);
		}

        /// <summary>
        /// 退出主界面
        /// </summary>
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
