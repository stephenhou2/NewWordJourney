using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PurchasePendingHUD : MonoBehaviour {

		public Transform pendingTint;

		private int rotateSpeed = 90;

		private int timeout = 20;

		public void SetUpPurchasePendingHUD(){

			gameObject.SetActive (true);

			StartCoroutine ("PendingTintRotate");

		}

		private IEnumerator PendingTintRotate(){

			pendingTint.localRotation = Quaternion.identity;

			yield return null;

			while (true) {

				Vector3 newRotation = new Vector3 (0, 0, -rotateSpeed * Time.realtimeSinceStartup);

				pendingTint.localRotation = Quaternion.Euler (newRotation);

				yield return null;

			}

		}

		public void QuitPurchasePendingHUD(){

			StopCoroutine ("PendingTintRotate");

			gameObject.SetActive (false);

		}

	}
}
