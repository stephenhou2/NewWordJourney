using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
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

		public bool bag_2_unlocked;

		public bool bag_3_unlocked;

		public bool bag_4_unlocked;

		public bool extraEquipmentSlotUnlocked;

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

	}
}
