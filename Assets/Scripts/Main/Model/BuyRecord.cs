using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
    /// 购买记录类
    /// </summary>
	[System.Serializable]
	public class BuyRecord{

		private static BuyRecord mInstance;
		public static BuyRecord Instance{
			get{
				if (mInstance == null) {
					mInstance = DataHandler.LoadDataToSingleModelWithPath<BuyRecord> (CommonData.buyRecordFilePath);
				}

				return mInstance;
			}
		}

        // 标记背包2是否已经解锁
		public bool bag_2_unlocked;
        // 标记背包3是否已经解锁
		public bool bag_3_unlocked;
        // 标记背包4是否已经解锁
		public bool bag_4_unlocked;
        // 标记额外装备槽是否已经解锁
		public bool extraEquipmentSlotUnlocked;

        // 上次获取金币的时间戳【仅安卓平台使用，用于广告获取金币的间隔控制】
		public double lastGoldAdTimeStamp;
        
        
        /// <summary>
        /// 购买成功时，对应商品标记为已解锁
        /// </summary>
        /// <param name="productId">Product identifier.</param>
		public void PurchaseSuccess(string productId){

			if (productId == PurchaseManager.extra_equipmentSlot_id) {
				BuyRecord.Instance.extraEquipmentSlotUnlocked = true;
			}else if (productId == PurchaseManager.extra_bag_2_id){
                BuyRecord.Instance.bag_2_unlocked = true;
            }else if (productId == PurchaseManager.extra_bag_3_id) {
				BuyRecord.Instance.bag_3_unlocked = true;
			}else if(productId == PurchaseManager.extra_bag_4_id){
				BuyRecord.Instance.bag_4_unlocked = true;
			}

			GameManager.Instance.persistDataManager.SaveBuyRecord ();

		}

        /// <summary>
		/// 记录上次金币广告的观看事件【仅安卓平台使用，用于广告获取金币的间隔控制】
        /// </summary>
		public void RecordLastGoldAdTime(){

			System.TimeSpan timeSpan = System.DateTime.Now - System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

			lastGoldAdTimeStamp = timeSpan.TotalSeconds;

			GameManager.Instance.persistDataManager.SaveBuyRecord();

		}

	}
}
