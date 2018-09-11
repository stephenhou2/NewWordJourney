using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public enum LoadingType{
		EnterExplore,
        QuitExplore
	}

	public class LoadingViewController : MonoBehaviour
    {
		public HLHFillBar loadingBar;

        public Text gameTint;

        public Image lampLight;

		private CallBack finishLoadingCallBack;

		public float minAlpha = 0.5f;

		public float maxAlpha = 1f;

		public float alphaChangeCircle = 2f;

		private IEnumerator lampCoroutine;// 灯光闪烁的动画协程
        

		public void SetUpLoadingView(LoadingType loadingType,CallBack beginLoadCallBack ,CallBack finishLoadingCallBack){

			//this.loadingType = loadingType;

			if(beginLoadCallBack != null){
				beginLoadCallBack();
			}
         
			this.finishLoadingCallBack = finishLoadingCallBack;

			loadingBar.InitHLHFillBar(100, 0);

			int tintIndex = Random.Range(0,CommonData.gameTints.Length);
			gameTint.text = CommonData.gameTints[tintIndex];

			GetComponent<Canvas>().enabled = true;

			lampCoroutine = LampAnimation();
			StartCoroutine(lampCoroutine);
            
			switch(loadingType){
				case LoadingType.EnterExplore:
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

		private IEnumerator LoadingBarAnimationEnterExplore(){
            
			loadingBar.changeDuration = 0.6f;
			loadingBar.value = 30;

			float timer = 0;

			Transform exploreManager = TransformManager.FindTransform("ExploreManager");
			while(exploreManager == null || timer < 0.6f){
				exploreManager = TransformManager.FindTransform("ExploreManager");
				timer += Time.unscaledDeltaTime;
				yield return null;
			}

         
			loadingBar.changeDuration = 0.8f;
			loadingBar.value = 80;

			yield return new WaitForSecondsRealtime(1.6f);

			loadingBar.changeDuration = 0.4f;
			loadingBar.value = 100;

			timer = 0;
            while (!ExploreManager.Instance.exploreSceneReady || timer < 0.4f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

			yield return new WaitForSecondsRealtime(0.3f);

			if(lampCoroutine != null){
				StopCoroutine(lampCoroutine);
			}

			if(finishLoadingCallBack != null){
				finishLoadingCallBack();
			}
         
			if(!Player.mainPlayer.isNewPlayer){
				GameManager.Instance.soundManager.PlayBgmAudioClip(CommonData.exploreBgmName);
			}
         
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
