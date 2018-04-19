using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.UI;

	public class TintHUD : MonoBehaviour {

		public Text tintText;
		public Image goldIcon;
		public Text singleTextTint;
		public float tintHUDShowDuration = 1f;
		private IEnumerator tintHUDCoroutine;

		public void SetUpSingleTextTintHUD(string tint){
			gameObject.SetActive (true);
			tintText.enabled = false;
			goldIcon.enabled = false;
			singleTextTint.enabled = true;
			singleTextTint.text = tint;
			if (tintHUDCoroutine != null) {
				StopCoroutine (tintHUDCoroutine);
			}

			tintHUDCoroutine = TintHUDLatelyDisappear ();

			StartCoroutine (tintHUDCoroutine);
		}

		public void SetUpGoldTintHUD(int goldGain){
			gameObject.SetActive (true);
			tintText.enabled = true;
			goldIcon.enabled = true;
			singleTextTint.enabled = false;
			tintText.text = string.Format ("+{0}", goldGain);

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
