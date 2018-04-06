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

		public bool extraBagUnlocked;

		public bool extraEquipmentSlotUnlocked;

		public void PurchaseSuccess(string productId){

			if (productId == PurchaseManager.extra_equipmentSlot_id) {
				BuyRecord.Instance.extraEquipmentSlotUnlocked = true;
			} else if (productId == PurchaseManager.extra_bag_id) {
				BuyRecord.Instance.extraBagUnlocked = true;
			}

			GameManager.Instance.persistDataManager.SaveBuyRecord ();

		}

	}
}
