using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public enum TransitionType{
		Introduce,
		Death
	}

	public class TransitionView : MonoBehaviour {

		private string[] introduceStrings = new string[]{
			"阳光透过窗户洒进古老的殿堂",
			"黑暗中一件宝藏散发着淡淡微光",
			"隐约听到来自远古的呼唤",
			"世间的勇者们啊\n请接受我的馈赠\n来黑暗的深处拿取我的宝藏",
			"你的故事将从这里开始…"
		};
		private string[] deadStrings = new string[]{
			"渐渐得人们忘记了你的名字",
			"只是流传着曾经有一个勇者来过这里"
		};

		public Text transitionTextModel;

//		public Text[] introductions;

		public Transform transitionTextContainer;

		public Text clickTintText;

		public float fadeInTime = 1.5f;

		public float sentenceInterval = 1.0f;

		private TransitionType transitionType;

//		public CallBack finishIntroductionCallBack;

		public Image transitionPlaneMask;

		private bool hasUserClick;

		private int heightBase = 145;


//		public void InitIntroductionView(CallBack finishIntroductionCallBack){
//			hasUserClick = false;
//			this.finishIntroductionCallBack = finishIntroductionCallBack;
//		}

		public void PlayTransition(TransitionType transitionType,CallBack finishTransitionCallBack){

			hasUserClick = false;

			this.transitionType = transitionType;

			this.gameObject.SetActive (true);

			string[] transitionStrings = null;

			switch (transitionType) {
			case TransitionType.Introduce:
				transitionStrings = introduceStrings;
				break;
			case TransitionType.Death:
				transitionStrings = deadStrings;
				break;
			}

			IEnumerator transitionCoroutine = PlayTransition (transitionStrings,finishTransitionCallBack);

			StartCoroutine (transitionCoroutine);

		}

		public void UserClick(){
			hasUserClick = true;
		}

		private IEnumerator PlayTransition(string[] transitionStrings,CallBack finishTransitionCallBack){

			yield return new WaitForSeconds (1.0f);

			int totalSentenceCount = transitionStrings.Length;

			transitionTextContainer.localPosition = new Vector3 (0, totalSentenceCount * heightBase / 2, 0);

			transitionPlaneMask.enabled = false;

			clickTintText.enabled = false;

			float alphaChangeSpeed = 1.0f / fadeInTime;

			float alpha = 0;

			for (int i = 0; i < totalSentenceCount; i++) {

				Text t = Instantiate (transitionTextModel.gameObject,transitionTextContainer).GetComponent<Text> ();

				t.text = transitionStrings [i];

				alpha = 0;


				while (alpha < 1) {

					t.color = new Color (1, 1, 1, alpha);

					alpha += alphaChangeSpeed * Time.deltaTime;

					yield return null;

				}

				yield return new WaitForSeconds (sentenceInterval);
			}

			switch (transitionType) {
			case TransitionType.Introduce:
				transitionPlaneMask.enabled = true;
				transitionPlaneMask.color = new Color (0, 0, 0, 0);
				transitionPlaneMask.raycastTarget = true;
				clickTintText.enabled = true;
				alpha = 0.5f;

				while (!hasUserClick) {

					while (alpha < 1f) {

						clickTintText.color = new Color (1, 1, 1, alpha);

						alpha += alphaChangeSpeed * Time.deltaTime / 2;

						if (hasUserClick) {
							break;
						}

						yield return null;

					}

					while (alpha > 0.5f) {

						clickTintText.color = new Color (1, 1, 1, alpha);

						alpha -= alphaChangeSpeed * Time.deltaTime / 2;

						if (hasUserClick) {
							break;
						}

						yield return null;

					}
				}

				break;
			case TransitionType.Death:
				break;
			}
				
			alpha = 0;

			while (alpha < 1) {

				transitionPlaneMask.color = new Color (0, 0, 0, alpha);

				alpha += alphaChangeSpeed * Time.deltaTime;

				yield return null;
			}

			finishTransitionCallBack ();

			this.gameObject.SetActive (false);

			for (int i = 0; i < transitionTextContainer.childCount; i++) {
				Destroy (transitionTextContainer.GetChild (i).gameObject,0.3f);
			}

		}


	}
}
