using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, 
// one of the existing Survival Shooter scripts.
namespace WordJourney
{
	// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
	public class PurchaseManager : MonoBehaviour, IStoreListener
	{
		private IStoreController controller;
		//private IExtensionProvider extensions;

		// Product identifiers for all products capable of being purchased: 
		// "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
		// counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
		// also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.
		public static string extra_equipmentSlot_id = "com.yougan233.wordjourney.extraEquipmentSlot_ring";
		public static string extra_bag_2_id = "com.yougan233.wordjourney.extraBag_02";
		public static string extra_bag_3_id = "com.yougan233.wordjourney.extraBag_03";
		public static string extra_bag_4_id = "com.yougan233.wordjourney.extraBag_04";
		public static string new_life_id = "com.yougan233.wordjourney.lifeCard";


		private CallBack purchaseSucceedCallback;
		private CallBack purchaseFailCallback;


		void Start()
		{
			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

			builder.AddProduct(PurchaseManager.extra_equipmentSlot_id, ProductType.Consumable);
			builder.AddProduct(PurchaseManager.extra_bag_2_id, ProductType.Consumable);
			builder.AddProduct (PurchaseManager.extra_bag_3_id, ProductType.Consumable);
			builder.AddProduct(PurchaseManager.extra_bag_4_id, ProductType.Consumable);
			builder.AddProduct (PurchaseManager.new_life_id, ProductType.Consumable);

			UnityPurchasing.Initialize(this, builder);
		}
			

		/// <summary>
		/// Called when Unity IAP is ready to make purchases.
		/// </summary>
		public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
		{
			if (IsInitialized())
			{
				return;
			}


			this.controller = controller;

//			this.extensions = extensions;
//			Debug.Log("Initialize success");
		}

		private bool IsInitialized()
		{
			// Only say we are initialized if both the Purchasing references are set.
			return controller != null; 
//				&& extensions != null;
		}


		public void PurchaseProduct(string productId,CallBack successCallback,CallBack failCallback){

//			Debug.Log ("purchase action!!");

			this.purchaseSucceedCallback = successCallback;
			this.purchaseFailCallback = failCallback;


			try{
				controller.InitiatePurchase (productId);
			}catch(Exception e){
				Debug.Log (e);
				if(purchaseFailCallback != null){
					purchaseFailCallback ();
                }
			}

		}



		/// <summary>
		/// Called when Unity IAP encounters an unrecoverable initialization error.
		///
		/// Note that this will not be called if Internet is unavailable; Unity IAP
		/// will attempt initialization until it becomes available.
		/// </summary>
		public void OnInitializeFailed (InitializationFailureReason error)
		{
			Debug.Log (error);
		}

		/// <summary>
		/// Called when a purchase completes.
		///
		/// May be called at any time after OnInitialized().
		/// </summary>
		public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
		{
			Debug.Log ("purchase success!!");

			BuyRecord.Instance.PurchaseSuccess (e.purchasedProduct.definition.id);

			if(purchaseSucceedCallback != null){
				purchaseSucceedCallback ();
            }

			return PurchaseProcessingResult.Complete;
		}

		/// <summary>
		/// Called when a purchase fails.
		/// </summary>
		public void OnPurchaseFailed (Product i, PurchaseFailureReason p)
		{
			Debug.Log ("purchase failed!!");
			if(purchaseFailCallback != null){
				purchaseFailCallback ();
            }
		}


		void OnDestroy(){
			controller = null;
			//extensions = null;
			purchaseSucceedCallback = null;
			purchaseFailCallback = null;
		}

	}
}
