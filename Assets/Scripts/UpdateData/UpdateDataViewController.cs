using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class UpdateDataViewController : MonoBehaviour
	{

		public HLHFillBar loadingBar;

		//void Start()
		//{         
		//	SetUpUpdateDataView();
		//}
        

		public void SetUpUpdateDataView(){

			loadingBar.InitHLHFillBar(100, 0);

			GetComponent<Canvas>().enabled = true;

			IEnumerator loadingBarCoroutine = LoadingBarAnimation();

			StartCoroutine(loadingBarCoroutine);

		}
	
		private IEnumerator LoadingBarAnimation(){

			yield return GameManager.Instance != null;

			GameManager.Instance.soundManager.PlayBgmAudioClip(CommonData.homeBgmName, true);

			loadingBar.changeDuration = 0.2f;
            loadingBar.value = 10;

            float timer = 0;

			GameLoader gameLoader = Camera.main.GetComponent<GameLoader>();

			while(gameLoader == null || timer < 0.4f){
				yield return null;
				gameLoader = Camera.main.GetComponent<GameLoader>();
				timer += Time.deltaTime;
			}

			yield return new WaitUntil(() => gameLoader.dataReady);

            loadingBar.changeDuration = 1f;
            loadingBar.value = 60;

            yield return new WaitForSecondsRealtime(1.6f);

			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.homeCanvasBundleName, "HomeCanvas", () =>
            {

                HomeViewController homeViewController = TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>();

                homeViewController.SetUpHomeView();

                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    homeViewController.homeView.ShowNoAvalableNetHintHUD();
                }
                else
                {
                    homeViewController.homeView.HideNoAvalableNetHintHUD();
                }

                GameManager.Instance.soundManager.PlayBgmAudioClip(CommonData.homeBgmName);

            });


			Transform homeCanvas = TransformManager.FindTransform("HomeCanvas");

			while(homeCanvas == null || timer < 0.6f){
                homeCanvas = TransformManager.FindTransform("HomeCanvas");
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            loadingBar.changeDuration = 0.6f;
            loadingBar.value = 100;
         
            yield return new WaitForSecondsRealtime(1f);

			//this.gameObject.SetActive(false);

			GameManager.Instance.UIManager.RemoveCanvasCache("UpdateDataCanvas");

			        
		}

		public void DestroyInstances(){

			GetComponent<Canvas>().enabled = false;

			MyResourceManager.Instance.UnloadAssetBundle(CommonData.updateDataCanvasBundleName, true);

            Destroy(this.gameObject, 0.3f);


		}
        
    }
}

