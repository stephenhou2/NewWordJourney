using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class PurchasePendingHUD : MonoBehaviour {

		public Transform pendingTint;

		private int rotateSpeed = 90;

//		private int timeout = 20;

		public TintHUD tintHUD;

		private CallBack purchaseFinishCallBack;

		private string currentPurchasingItemId;

		public void SetUpPurchasePendingHUD(string productId,CallBack purchaseFinishCallBack){

			currentPurchasingItemId = productId;

			this.purchaseFinishCallBack = purchaseFinishCallBack;

			gameObject.SetActive (true);

			StartCoroutine ("PendingTintRotate");

			GameManager.Instance.purchaseManager.PurchaseProduct (productId, OnPurchaseSucceed, OnPurchaseFail);

		}


		private void OnPurchaseSucceed(){

			if(currentPurchasingItemId.Equals(PurchaseManager.extra_bag_id)){
				tintHUD.SetUpTintHUD ("成功解锁装备槽7", null);
			}else if(currentPurchasingItemId.Equals(PurchaseManager.extra_equipmentSlot_id)){
				tintHUD.SetUpTintHUD ("解锁成功背包4", null);
			}else if (currentPurchasingItemId.Equals(PurchaseManager.new_life_id)){
				
			}

			purchaseFinishCallBack ();

			QuitPurchasePendingHUD ();
		}


		private void OnPurchaseFail(){

			tintHUD.SetUpTintHUD ("购买失败，请稍后重试", null);

			QuitPurchasePendingHUD ();

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
