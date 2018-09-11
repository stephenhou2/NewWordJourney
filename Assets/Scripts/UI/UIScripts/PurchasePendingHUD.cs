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

		private IEnumerator pendingCoroutine;

		public void SetUpPurchasePendingHUD(string productId, CallBack purchaseFinishCallBack)
		{

			Time.timeScale = 0;

			currentPurchasingItemId = productId;

            this.purchaseFinishCallBack = purchaseFinishCallBack;

			gameObject.SetActive(true);


#if UNITY_IPHONE || UNITY_EDITOR   

			pendingTint.gameObject.SetActive(true);
			queryShareHUD.gameObject.SetActive(false);

			pendingCoroutine = PendingTintRotate();
			StartCoroutine(pendingCoroutine);
         
			GameManager.Instance.purchaseManager.PurchaseProduct(productId, OnPurchaseSucceed, OnPurchaseFail);
         
#elif UNITY_ANDROID         
			pendingTint.gameObject.SetActive(false);
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
			         
			//if (Application.internetReachability == NetworkReachability.NotReachable)
			//{
			//	tintHUD.SetUpSingleTextTintHUD("无网络连接");
			//}
			//else
			//{

				HideShareQueryHUD();

				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", delegate
				{
					TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.WeChat, OnPurchaseSucceed, OnShareFaild, OnQuitButtonClick);


				});
			//}
		}

		public void OnWeiboShareButtonClick(){
         
			//if (Application.internetReachability == NetworkReachability.NotReachable)
			//{
			//	tintHUD.SetUpSingleTextTintHUD("无网络连接");
			//}
			//else
			//{

				HideShareQueryHUD();

				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", delegate
				{
					TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.Weibo, OnPurchaseSucceed, OnShareFaild, OnQuitButtonClick);
				});
			//}

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

			BuyRecord.Instance.PurchaseSuccess(currentPurchasingItemId);

         
			if(purchaseFinishCallBack != null){
				purchaseFinishCallBack();
			}

			QuitPurchasePendingHUD();
		}


		private void OnShareFaild(){
			
			string shareResult = "未检测到客户端";

			tintHUD.SetUpSingleTextTintHUD(shareResult);

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

			Time.timeScale = 1f;

#if UNITY_IPHONE || UNITY_EDITOR    
			if(pendingCoroutine != null){
				StopCoroutine(pendingCoroutine);
			}

#endif         
			gameObject.SetActive (false);

		}

	}
}
