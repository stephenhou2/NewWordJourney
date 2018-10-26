using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

    /// <summary>
    /// 加载类型
    /// </summary>
	public enum LoadingType{
		EnterExplore,// 进入探索场景
        QuitExplore// 退出探索场景
	}
    
    /// <summary>
    /// 加载场景控制器
    /// </summary>
	public class LoadingViewController : MonoBehaviour
    {
		// 加载进度条
		public HLHFillBar loadingBar;
        // 游戏小贴士
        public Text gameTint;
        // 灯光
        public Image lampLight;
        // 加载完成回调
		private CallBack finishLoadingCallBack;

		public float minAlpha = 0.5f;

		public float maxAlpha = 1f;

		public float alphaChangeCircle = 2f;

		private IEnumerator lampCoroutine;// 灯光闪烁的动画协程
        
        /// <summary>
        /// 初始化加载场景
        /// </summary>
        /// <param name="loadingType">加载类型.</param>
        /// <param name="beginLoadCallBack">Begin load call back.</param>
        /// <param name="finishLoadingCallBack">Finish loading call back.</param>
		public void SetUpLoadingView(LoadingType loadingType,CallBack beginLoadCallBack ,CallBack finishLoadingCallBack){

			//this.loadingType = loadingType;

			if(beginLoadCallBack != null){
				beginLoadCallBack();
			}
         
			this.finishLoadingCallBack = finishLoadingCallBack;

            // 初始化加载进度条
			loadingBar.InitHLHFillBar(100, 0);

			int tintIndex = Random.Range(0,CommonData.gameTints.Length);
			gameTint.text = CommonData.gameTints[tintIndex];

			GetComponent<Canvas>().enabled = true;

			lampCoroutine = LampAnimation();
			StartCoroutine(lampCoroutine);
            
			switch(loadingType){
				case LoadingType.EnterExplore:
					//Player.mainPlayer.canSave = false;
					IEnumerator loadingAndEnterExploreCoroutine = LoadingBarAnimationEnterExplore();
					StartCoroutine(loadingAndEnterExploreCoroutine);           
					break;
				case LoadingType.QuitExplore:
					IEnumerator loadingAndQuitExploreCoroutine = LoadingBarAnimationQuitExplore();
					StartCoroutine(loadingAndQuitExploreCoroutine);         
					break;
			}

         
		}

        

        /// <summary>
        /// 灯摇晃，灯光闪烁动画
        /// </summary>
        /// <returns>The animation.</returns>
		private IEnumerator LampAnimation(){

			float alphaChangeSpeed = (maxAlpha - minAlpha) / alphaChangeCircle;

			while(true){

				float lightAlpha = lampLight.color.a;

				int scaler = 1;

				if(lightAlpha >= maxAlpha){
					scaler = -1;               
				}else if(lightAlpha <= minAlpha){
					scaler = 1;
				}
                
				lampLight.color = new Color(1, 1, 1, lightAlpha + alphaChangeSpeed * Time.deltaTime * scaler);

				yield return null;
            
			}         
		}


        /// <summary>
        /// 进入探索时加载进度条的加载显示逻辑
        /// </summary>
        /// <returns>The bar animation enter explore.</returns>
		private IEnumerator LoadingBarAnimationEnterExplore(){
            
            // 0.6s的时间进度条加载30%
			loadingBar.changeDuration = 0.6f;
			loadingBar.value = 30;

			float timer = 0;

            // 等待探索场景从内存中加载出来
			Transform exploreManager = TransformManager.FindTransform("ExploreManager");
			while(exploreManager == null || timer < 0.6f){
				exploreManager = TransformManager.FindTransform("ExploreManager");
				timer += Time.unscaledDeltaTime;
				yield return null;
			}

            // 0.8s的时间进度条加载到80%
			loadingBar.changeDuration = 0.8f;
			loadingBar.value = 80;
            
            // 等待1.6s
			yield return new WaitForSecondsRealtime(1.6f);

            // 0.4s的时间进度条加载到100%
			loadingBar.changeDuration = 0.4f;
			loadingBar.value = 100;

			timer = 0;
            // 等待探索场景初始化完成
            while (!ExploreManager.Instance.exploreSceneReady || timer < 0.4f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            // 等待0.3s
			yield return new WaitForSecondsRealtime(0.3f);

			//Player.mainPlayer.canSave = true;
            
            // 停止灯光闪烁
			if(lampCoroutine != null){
				StopCoroutine(lampCoroutine);
			}

            // 加载完成回调
			if(finishLoadingCallBack != null){
				finishLoadingCallBack();
			}
            
            // 如果时新用户，此时应该进入引导场景，不播放探索音乐
            // 如果不是新用户，播放探索音乐
			if(!Player.mainPlayer.isNewPlayer){
				GameManager.Instance.soundManager.PlayBgmAudioClip(CommonData.exploreBgmName);
			}
         
            // 移除加载场景
			GameManager.Instance.UIManager.RemoveCanvasCache("LoadingCanvas");
         
		}

		private IEnumerator LoadingBarAnimationQuitExplore()
        {

            loadingBar.changeDuration = 0.6f;
            loadingBar.value = 30;

            float timer = 0;

			//GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.homeCanvasBundleName, "HomeCanvas", () => {
            //    TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
            //}, true, false);

            Transform exploreManager = TransformManager.FindTransform("HomeCanvas");
            while (exploreManager == null || timer < 0.6f)
            {
				exploreManager = TransformManager.FindTransform("HomeCanvas");
                timer += Time.unscaledDeltaTime;

                yield return null;
            }


            loadingBar.changeDuration = 1.2f;
            loadingBar.value = 100;

            yield return new WaitForSecondsRealtime(2.0f);




			if(lampCoroutine != null){
				StopCoroutine(lampCoroutine);
			}

            if (finishLoadingCallBack != null)
            {
                finishLoadingCallBack();
            }

			GameManager.Instance.soundManager.PlayBgmAudioClip(CommonData.homeBgmName);
         
            GameManager.Instance.UIManager.RemoveCanvasCache("LoadingCanvas");

        }

		public void DestroyInstances(){

			GetComponent<Canvas>().enabled = false;

			MyResourceManager.Instance.UnloadAssetBundle(CommonData.loadingCanvasBundleName, true);

            Destroy(this.gameObject, 0.3f);

		}
        
        
    }

}
