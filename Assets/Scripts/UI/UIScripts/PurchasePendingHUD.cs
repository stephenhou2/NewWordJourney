using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	//public delegate void CallBackWithString(string arg);

	public class PurchasePendingHUD : MonoBehaviour
	{

		public Transform pendingTint;

		private int rotateSpeed = 45;

		private float rotateInterval = 0.15f;

		public TintHUD tintHUD;

		private CallBack purchaseFinishCallBack;

		private string currentPurchasingItemId;

		public Transform queryShareHUD;



		public void SetUpPurchasePendingHUD(string productId, CallBack purchaseFinishCallBack)
		{

			//Time.timeScale = 0;

			currentPurchasingItemId = productId;

            this.purchaseFinishCallBack = purchaseFinishCallBack;


#if UNITY_IPHONE || UNITY_EDITOR   

			pendingTint.gameObject.SetActive(true);
			queryShareHUD.gameObject.SetActive(false);

			gameObject.SetActive(true);
                     
			StartCoroutine("PendingTintRotate");

			GameManager.Instance.purchaseManager.PurchaseProduct(productId, OnPurchaseSucceed, OnPurchaseFail);

#elif UNITY_ANDROID
			pendingTint.gameObject.SetActive(false);
			queryShareHUD.gameObject.SetActive(true);
			ShowShareQueryHUD();
#endif         
		}


		private void ShowShareQueryHUD(){
			queryShareHUD.gameObject.SetActive(true);
		}

		private void HideShareQueryHUD(){
			queryShareHUD.gameObject.SetActive(false);
		}

		public void OnWechatShareButtonClick(){
			
			HideShareQueryHUD();

			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", delegate
            {
				TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.WeChat, OnPurchaseSucceed, OnPurchaseFail,OnQuitButtonClick);


            });
		}

		public void OnWeiboShareButtonClick(){

			HideShareQueryHUD();

			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", delegate
			{
				TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.Weibo, OnPurchaseSucceed, OnPurchaseFail,OnQuitButtonClick);
			});

		}

		public void OnQuitButtonClick(){
			HideShareQueryHUD();
			gameObject.SetActive(false);
		}


		private void OnPurchaseSucceed(){

			string purchaseResult = string.Empty;

			if (currentPurchasingItemId.Equals(PurchaseManager.extra_bag_2_id))
            {
                purchaseResult = "解锁背包2";
            }
            else if (currentPurchasingItemId.Equals(PurchaseManager.extra_bag_3_id))
            {
				purchaseResult = "解锁背包3";
            }
			else if(currentPurchasingItemId.Equals(PurchaseManager.extra_bag_4_id))
			{
				purchaseResult = "解锁背包4";
			}
			else if(currentPurchasingItemId.Equals(PurchaseManager.extra_equipmentSlot_id))
			{
				purchaseResult = "解锁装备槽7";
			}

			tintHUD.SetUpSingleTextTintHUD(purchaseResult);

			if(purchaseFinishCallBack != null){
				purchaseFinishCallBack();
			}

			QuitPurchasePendingHUD();
		}


		private void OnPurchaseFail(){

			string purchaseResult = "购买失败，请稍后重试";

			tintHUD.SetUpSingleTextTintHUD(purchaseResult);

			QuitPurchasePendingHUD();

			//QuitPurchasePendingHUD ();

		}

		private IEnumerator PendingTintRotate(){

			pendingTint.localRotation = Quaternion.identity;

			yield return null;

			int count = 0;

			while (true) {

				Vector3 newRotation = new Vector3 (0, 0, -rotateSpeed * count);

				pendingTint.localRotation = Quaternion.Euler (newRotation);

				yield return new WaitForSecondsRealtime(rotateInterval);

				count++;

			}

		}

		private void QuitPurchasePendingHUD(){

#if UNITY_IPHONE || UNITY_EDITOR         
			StopCoroutine ("PendingTintRotate");
#endif         
			gameObject.SetActive (false);

		}

	}
}
