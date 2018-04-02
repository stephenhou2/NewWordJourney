using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class IntroductionView : MonoBehaviour {

		public Text[] introductions;

		public Text clickTintText;

		public float fadeInTime = 1.0f;

		public float sentenceInterval = 1.0f;

		public CallBack finishIntroductionCallBack;

		public Image introductionMask;

		private bool hasUserClick;


		public void InitIntroductionView(CallBack finishIntroductionCallBack){
			hasUserClick = false;
			this.finishIntroductionCallBack = finishIntroductionCallBack;
		}

		public void PlayIntroductionTransition(){

			this.gameObject.SetActive (true);

			StartCoroutine ("PlayIntroduction");

		}

		public void UserClick(){
			hasUserClick = true;
		}

		private IEnumerator PlayIntroduction(){

			int totalSentenceCount = introductions.Length;

			float alphaChangeSpeed = 1.0f / fadeInTime;

			float alpha = 0;

			for (int i = 0; i < totalSentenceCount; i++) {

				alpha = 0;

				while (alpha < 1) {

					introductions [i].color = new Color (1, 1, 1, alpha);

					alpha += alphaChangeSpeed * Time.deltaTime;

					yield return null;

				}

				yield return new WaitForSeconds (sentenceInterval);
			}

			introductionMask.raycastTarget = true;

			alpha = 0.5f;

			while (!hasUserClick) {

				while (alpha < 1f) {

					clickTintText.color = new Color (1, 1, 1, alpha);

					alpha += alphaChangeSpeed * Time.deltaTime;

					if (hasUserClick) {
						break;
					}

					yield return null;

				}

				while (alpha > 0.5f) {

					clickTintText.color = new Color (1, 1, 1, alpha);

					alpha -= alphaChangeSpeed * Time.deltaTime;

					if (hasUserClick) {
						break;
					}

					yield return null;

				}

			}

			alpha = 0;

			while (alpha < 1) {
				
				introductionMask.color = new Color (0, 0, 0, alpha);

				alpha += alphaChangeSpeed * Time.deltaTime;

				yield return null;
			}
				
			this.gameObject.SetActive (false);

			finishIntroductionCallBack ();

		}


	}
}
