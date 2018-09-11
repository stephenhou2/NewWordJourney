using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class UpdateDataViewController : MonoBehaviour
	{

		public HLHFillBar loadingBar;

		//public Image lampLight;

		void Start(){		
			SetUpUpdateDataView();
		}

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

			//gameLoader.PersistData();

			yield return new WaitUntil(() => gameLoader.dataReady);

            loadingBar.changeDuration = 1f;
            loadingBar.value = 60;

            yield return new WaitForSecondsRealtime(1.6f);

			Transform homeCanvas = TransformManager.FindTransform("HomeCanvas");

			while(homeCanvas == null || timer < 0.6f){
                homeCanvas = TransformManager.FindTransform("HomeCanvas");
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            loadingBar.changeDuration = 0.6f;
            loadingBar.value = 100;
         
            yield return new WaitForSecondsRealtime(1f);

			this.gameObject.SetActive(false);

			Destroy(this.gameObject);         
		}
        
    }
}

