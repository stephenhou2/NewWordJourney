using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, 
// one of the existing Survival Shooter scripts.
namespace WordJourney
{
	/// <summary>
    /// 购买控制器
    /// </summary>
	public class PurchaseManager : MonoBehaviour, IStoreListener
	{
		private IStoreController controller;
		private IAppleExtensions m_appleExtension;
		private ConfigurationBuilder builder;

		//private string receipt;
		//private IExtensionProvider extensions;

        // 商品id
		public static string extra_equipmentSlot_id = "com.yougan233.wordjourney.extraEquipmentSlot_Ring";
		public static string extra_bag_2_id = "com.yougan233.wordjourney.extraBagSlot_2";
		public static string extra_bag_3_id = "com.yougan233.wordjourney.extraBagSlot_3";
		// 注：version1.1中背包4取消，已购买的用户改为奖励500金币
		public static string extra_bag_4_id = "com.yougan233.wordjourney.extraBagSlot_4";
		public static string new_life_id = "com.yougan233.wordjourney.lifeCard";

		//version1.1新增商品

		public static string gold_500_id = "com.yougan233.wordjourney.500_gold";//500金币
		public static string gold_1600_id = "com.yougan233.wordjourney.1600_gold";//1600金币
		public static string gold_3500_id = "com.yougan233.wordjourney.3500_gold";//3000金币
		public static string gold_5000_id = "com.yougan233.wordjourney.5000_gold";//5000金币

        // 以下为安卓平台特有
		public static string gold_100_id = "com.yougan233.wordjourney.100_gold";//奖励100金币【安卓平台】
		public static string skill_point_id = "com.yougan233.wordjourney.skill_point";//奖励一个技能点【安卓平台】

		// 已购买项目如果存在变动，则变动信息放在这个列表中
		public List<string> buyedGoodsChange = new List<string>();

        // 购买成功回调
		private CallBack purchaseSucceedCallback;
        // 购买失败回调
		private CallBack purchaseFailCallback;
        // 恢复内置购买回调
		private CallBackWithInt restoreCallBack;

        // 初始化，并创建商品
		void Start()
		{

			builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

			builder.AddProduct(PurchaseManager.extra_equipmentSlot_id, ProductType.NonConsumable);
			builder.AddProduct(PurchaseManager.extra_bag_2_id, ProductType.NonConsumable);
			builder.AddProduct(PurchaseManager.extra_bag_3_id, ProductType.NonConsumable);
			builder.AddProduct(PurchaseManager.extra_bag_4_id, ProductType.NonConsumable);
			builder.AddProduct(PurchaseManager.new_life_id, ProductType.Consumable);

			builder.AddProduct(PurchaseManager.gold_500_id, ProductType.Consumable);
			builder.AddProduct(PurchaseManager.gold_1600_id, ProductType.Consumable);
			builder.AddProduct(PurchaseManager.gold_3500_id, ProductType.Consumable);
			builder.AddProduct(PurchaseManager.gold_5000_id, ProductType.Consumable);	         

		    if (Application.internetReachability != NetworkReachability.NotReachable)
            {
			    UnityPurchasing.Initialize(this, builder);
		    }

		}


		/// <summary>
		/// 初始化
		/// </summary>
		public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
		{
			if (IsInitialized())
			{
				return;
			}

			this.controller = controller;

			this.m_appleExtension = extensions.GetExtension<IAppleExtensions>();
         
		}

        /// <summary>
        /// 检查是否已经完成初始化
        /// </summary>
        /// <returns><c>true</c>, if initialized was ised, <c>false</c> otherwise.</returns>
		private bool IsInitialized()
		{
			// Only say we are initialized if both the Purchasing references are set.
			return controller != null && m_appleExtension != null;
		}
        
      
        /// <summary>
        /// 恢复非消耗类商品的购买状态
        /// </summary>
		public void RestoreItems(CallBackWithInt restoreCallBack){


			if (!IsInitialized() && Application.internetReachability != NetworkReachability.NotReachable)
            {
				UnityPurchasing.Initialize(this, builder);
            }

			this.restoreCallBack = restoreCallBack;

			if(IsInitialized()){
				m_appleExtension.RestoreTransactions(OnTransitionRestored);
			}
            
		}

        /// <summary>
        /// 恢复内购项
        /// </summary>
        /// <param name="success">If set to <c>true</c> success.</param>
		private void OnTransitionRestored(bool success){
			if(restoreCallBack != null){
				restoreCallBack(success ? 1 : 0);
				restoreCallBack = null;
			}
		}
        

        /// <summary>
        /// 购买指定id的商品
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <param name="successCallback">成功回调</param>
        /// <param name="failCallback">失败回调</param>
		public void PurchaseProduct(string productId,CallBack successCallback,CallBack failCallback){

			if (!IsInitialized() && Application.internetReachability != NetworkReachability.NotReachable)
            {
				UnityPurchasing.Initialize(this, builder);
            }

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
		/// 购买成功时走的原始方法
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
		/// 购买失败时走的原始方法
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
			purchaseSucceedCallback = null;
			purchaseFailCallback = null;
		}

	}
}
