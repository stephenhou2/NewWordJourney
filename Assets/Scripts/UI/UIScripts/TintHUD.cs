using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.UI;

	public class TintHUD : MonoBehaviour {

		public Text tintText;
		public Image tintImage;
		public Text singleTextTint;
		public float tintHUDShowDuration = 1f;
		private IEnumerator tintHUDCoroutine;

		public void SetUpTintHUD(string tint,Sprite sprite){

			gameObject.SetActive (true);

			if (sprite != null) {
				tintText.enabled = true;
				tintImage.enabled = true;
				singleTextTint.enabled = false;
				tintText.text = tint;
				tintImage.sprite = sprite;
			} else {
				tintText.enabled = false;
				tintImage.enabled = false;
				singleTextTint.enabled = true;
				singleTextTint.text = tint;
			}

			if (tintHUDCoroutine != null) {
				StopCoroutine (tintHUDCoroutine);
			}

			tintHUDCoroutine = TintHUDLatelyDisappear ();

			StartCoroutine (tintHUDCoroutine);

		}

		private IEnumerator TintHUDLatelyDisappear(){

			yield return new WaitForSecondsRealtime (tintHUDShowDuration);

			gameObject.SetActive (false);
		}

		public void QuitTintHUD(){
			if (tintHUDCoroutine != null) {
				StopCoroutine (tintHUDCoroutine);
			}
			gameObject.SetActive (false);
		}
	}
}
