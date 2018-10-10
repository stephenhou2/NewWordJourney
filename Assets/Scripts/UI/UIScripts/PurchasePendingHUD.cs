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

		public QueryWatchAdHUD queryWatchADHUD;

		private IEnumerator pendingCoroutine;

		// 在安卓端该属性为播放的广告类型，ios端该属性没有用处
		private MyAdType currentAdType;

		//private AdRewardType adRewardType;

		// 在安卓端该回调为广告播放成功的回调，ios端该回调无用
		private AdCallBack adSuccessCallBack;

		// 在安卓端该回调为广告播放失败的回调，ios端该回调无用
		private AdCallBack adFailCallBack;

  //      // 在安卓端该回调为广告播放成功，但是没有完成奖励的回调【如没有看完激励视频广告】,ios端该回调无用
		//private AdCallBack adUnrewardCallBack;

		/// <summary>
		/// 初始化内购界面【安卓端为初始化广告界面】
		/// </summary>
		/// <param name="productId">Product identifier.</param>
		/// <param name="purchaseFinishCallBack">Purchase finish call back.</param>
		/// <param name="myAdType">My ad type.</param>
		public void SetUpPurchasePendingHUDOnIPhone(string productId, CallBack purchaseFinishCallBack)
		{

#if !UNITY_IOS && !UNITY_EDITOR
			return;
#endif

			Time.timeScale = 0;

			currentPurchasingItemId = productId;

			this.purchaseFinishCallBack = purchaseFinishCallBack;

			gameObject.SetActive(true);

			pendingTint.gameObject.SetActive(true);
			queryWatchADHUD.gameObject.SetActive(false);

			pendingCoroutine = PendingTintRotate();
			StartCoroutine(pendingCoroutine);

			GameManager.Instance.purchaseManager.PurchaseProduct(productId, OnPurchaseSucceed, OnPurchaseFail);         

		}

		public void SetUpPurchasePendingHUDOnAndroid(string productId, MyAdType myAdType,AdRewardType adRewardType, AdCallBack sucessCallBack, AdCallBack failCallBack)
		{
            
#if !UNITY_ANDROID && !UNITY_EDITOR
			return;
#endif
                     
			this.currentAdType = myAdType;

			Time.timeScale = 0;

			currentPurchasingItemId = productId;

			this.adSuccessCallBack = sucessCallBack;
			this.adFailCallBack = failCallBack;

			gameObject.SetActive(true);

			pendingTint.gameObject.SetActive(false);

			string query = string.Empty;

			switch(adRewardType){
				case AdRewardType.BagSlot_2:
					query = "观看广告后可\n<color=orange>解锁背包2</color>\n是否确认观看广告？";
					queryWatchADHUD.SetUpQueryWatchAdHUD(query, OnConfirmWatchAd, QuitPurchasePendingHUD);
					break;
				case AdRewardType.BagSlot_3:
					query = "观看广告后可\n<color=orange>解锁背包3</color>\n是否确认观看广告？";
					queryWatchADHUD.SetUpQueryWatchAdHUD(query, OnConfirmWatchAd, QuitPurchasePendingHUD);
					break;
				case AdRewardType.EquipmentSlot:
					query = "观看广告后可\n<color=orange>解锁装饰槽</color>\n是否确认观看广告？";
					queryWatchADHUD.SetUpQueryWatchAdHUD(query, OnConfirmWatchAd, QuitPurchasePendingHUD);
					break;
				case AdRewardType.Gold:
					double timeSpanInSeconds = 0;
					bool canRewatch = CheckCanReWatchGoldAd(out timeSpanInSeconds);
					if(canRewatch){
						query = "观看广告后可\n<color=orange>获得100金币</color>\n是否确认观看广告？";
						queryWatchADHUD.SetUpQueryWatchAdHUD(query, OnConfirmWatchAd, QuitPurchasePendingHUD);
					}else{
						int minuteLeft = (1800 - (int)timeSpanInSeconds) / 60;
						int secondsLeft = (1800 - (int)timeSpanInSeconds) % 60;
						query = string.Format("<color=orange>{0}{1}:{2}{3}</color>后可以重新观看广告",
						                      minuteLeft < 10 ? "0" : "",
						                      minuteLeft,
						                      secondsLeft < 10 ? "0": "",
						                      secondsLeft);
						queryWatchADHUD.SetUpQueryWatchAdHUDWhenCantWatch(query,timeSpanInSeconds,delegate {
							query = "观看广告后可\n<color=orange>获得100金币</color>\n是否确认观看广告？";
                            queryWatchADHUD.SetUpQueryWatchAdHUD(query, OnConfirmWatchAd, QuitPurchasePendingHUD);
						},QuitPurchasePendingHUD);
					}

					break;
				case AdRewardType.Life:
					OnConfirmWatchAd();
                    QuitPurchasePendingHUD();
					break;
				case AdRewardType.SkillPoint:
					OnConfirmWatchAd();
					QuitPurchasePendingHUD();
					break;
			}         

		}


		private bool CheckCanReWatchGoldAd(out double timeSpanInSeconds){

			bool canRewatch = false;

			System.TimeSpan timeSpan = System.DateTime.Now - System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

			double timeStamp = timeSpan.TotalSeconds;

			timeSpanInSeconds = timeStamp - BuyRecord.Instance.lastGoldAdTimeStamp;

			if(timeSpanInSeconds < 1800){
				canRewatch = false;
			}else{
				canRewatch = true;
			}

			return canRewatch;

		}
        
		public void OnConfirmWatchAd(){
			switch (currentAdType)
            {
                case MyAdType.CPAd:
                    if (!TGController.IsCPAdReady)
                    {
                        TGController.QuickRereshAd();
						OnAdNotReady();
                        return;
                    }
                    break;
                case MyAdType.RewardedVideoAd:
                    if (!TGController.IsRewardedVideoAdReady)
                    {
                        TGController.QuickRereshAd();
						OnAdNotReady();
                        return;
                    }
                    break;
            }

			TGController.ShowAd(currentAdType, OnWatchAdSuccess, OnWatchAdFail, OnRewardFail);
		}

		private void ShowQueryWatchADHUD(){
			queryWatchADHUD.gameObject.SetActive(true);
		}

		private void HidQueryWatchADHUD(){
			queryWatchADHUD.gameObject.SetActive(false);
		}
        


		private void OnWatchAdSuccess(MyAdType adType){
         
			string purchaseResult = string.Empty;

            if (currentPurchasingItemId.Equals(PurchaseManager.extra_bag_2_id))
            {
                purchaseResult = "解锁背包2";
            }
            else if (currentPurchasingItemId.Equals(PurchaseManager.extra_bag_3_id))
            {
                purchaseResult = "解锁背包3";
            }
            else if (currentPurchasingItemId.Equals(PurchaseManager.extra_bag_4_id))
            {
                purchaseResult = "解锁背包4";
            }
            else if (currentPurchasingItemId.Equals(PurchaseManager.extra_equipmentSlot_id))
            {
                purchaseResult = "解锁装备槽7";
            }
            else if (currentPurchasingItemId.Equals(PurchaseManager.gold_100_id))
            {
                //needUpdatePlayerGold = true;
                int goldGain = 0;
                if (currentPurchasingItemId == PurchaseManager.gold_100_id)
                {
                    goldGain = 100;
                }
                
                purchaseResult = string.Format("金币 + {0}", goldGain);

			}else if(currentPurchasingItemId.Equals(PurchaseManager.skill_point_id)){
				purchaseResult = "技能点 + 1";
			}

            tintHUD.SetUpSingleTextTintHUD(purchaseResult);

            BuyRecord.Instance.PurchaseSuccess(currentPurchasingItemId);

			if (adSuccessCallBack != null)
            {
                adSuccessCallBack(adType);
            }

            QuitPurchasePendingHUD();

		}

		private void OnWatchAdFail(MyAdType adType){

			if(adFailCallBack != null){
				adFailCallBack(adType);
			}

			string purchaseResult = "广告播放失败，请稍后重试";

            tintHUD.SetUpSingleTextTintHUD(purchaseResult);

            QuitPurchasePendingHUD();         
		}

		private void OnAdNotReady(){
			
			string purchaseResult = "广告仍在加载中，请稍后重试";

            tintHUD.SetUpSingleTextTintHUD(purchaseResult);

            QuitPurchasePendingHUD();
		}

		private void OnRewardFail(MyAdType adType)
        {

            if (adFailCallBack != null)
            {
                adFailCallBack(adType);
            }

            string purchaseResult = "广告未播放完成，请稍后重试";

            tintHUD.SetUpSingleTextTintHUD(purchaseResult);

            QuitPurchasePendingHUD();
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
			else if (currentPurchasingItemId.Equals(PurchaseManager.extra_bag_4_id))
			{
				purchaseResult = "解锁背包4";
			}
			else if (currentPurchasingItemId.Equals(PurchaseManager.extra_equipmentSlot_id))
			{
				purchaseResult = "解锁装备槽7";
			}
			else if (currentPurchasingItemId.Equals(PurchaseManager.gold_500_id)
			         || currentPurchasingItemId.Equals(PurchaseManager.gold_1600_id)
			         || currentPurchasingItemId.Equals(PurchaseManager.gold_3500_id)
			         || currentPurchasingItemId.Equals(PurchaseManager.gold_5000_id))
			{
				//needUpdatePlayerGold = true;
				//int goldGain = 0;
				//if(currentPurchasingItemId == PurchaseManager.gold_500_id){
				//	goldGain = 500;
				//}else if(currentPurchasingItemId == PurchaseManager.gold_1600_id){
				//	goldGain = 1600;
				//}else if(currentPurchasingItemId == PurchaseManager.gold_3500_id){
				//	goldGain = 3500;
				//}else if(currentPurchasingItemId == PurchaseManager.gold_5000_id){
				//	goldGain = 5000;
				//}
				//purchaseResult = string.Format("金币 + {0}", goldGain);
					
			}

			tintHUD.SetUpSingleTextTintHUD(purchaseResult);

			BuyRecord.Instance.PurchaseSuccess(currentPurchasingItemId);

         
			if(purchaseFinishCallBack != null){
				purchaseFinishCallBack();
			}


			QuitPurchasePendingHUD();
		}
      
		private void OnPurchaseFail(){

			string purchaseResult = "购买失败，请稍后重试";

			tintHUD.SetUpSingleTextTintHUD(purchaseResult);

			QuitPurchasePendingHUD();

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

#if UNITY_IOS || UNITY_EDITOR
			if(pendingCoroutine != null){
				StopCoroutine(pendingCoroutine);
			}

#endif

			HidQueryWatchADHUD();
			gameObject.SetActive (false);

		}

	}
}
