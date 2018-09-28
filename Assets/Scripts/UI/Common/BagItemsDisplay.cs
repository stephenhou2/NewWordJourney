using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class BagItemsDisplay : MonoBehaviour {

        // 背包物品缓存池
		public InstancePool bagItemsPool;
        // 背包物品容器
		public Transform bagItemsContainer;
        // 背包物品模型
		public Transform bagItemModel;

        // 背包切换按钮数组
		public Button[] bagTabs;
        
        // 当前所在背包序号
        public int currentBagIndex;


        // 获取当前背包理论上首个显示的物品在背包中的理论序号
		private int minItemIndexOfCurrentBag {
			get {
				return currentBagIndex * CommonData.singleBagItemVolume;
			}
		}

		// 获取当前背包理论上可容纳的最后一个物品在背包中的理论序号
		private int maxItemIndexOfCurrentBag {
			get {
				return minItemIndexOfCurrentBag + CommonData.singleBagItemVolume - 1;
			}
		}
        
        // 背包物品点击事件回调
		private CallBackWithItem itemClickCallBack;

        // 触发appstore购买行为的回调
		private CallBackWithInt initPurchaseBag;

        // 进入购买金币的界面
		private CallBack enterPurchaseGold;
        
        /// <summary>
        /// 初始化背包物品显示界面[不初始化任何显示，只初始化各种回调和背包切换按钮状态]
        /// </summary>
        /// <param name="itemClickCallBack">Item click call back.</param>
        /// <param name="initPurchaseBag">Init purchase bag.</param>
		public void InitBagItemsDisplayPlane(CallBackWithItem itemClickCallBack,CallBackWithInt initPurchaseBag,CallBack enterPurchaseGold){

			currentBagIndex = 0;

			this.itemClickCallBack = itemClickCallBack;

			this.initPurchaseBag = initPurchaseBag;

			this.enterPurchaseGold = enterPurchaseGold;

			UpdateBagTabs();

		}

		/// <summary>
		/// 初始化背包物品显示界面
		/// </summary>
		public void SetUpBagItemsPlane(int bagIndex){

			currentBagIndex = bagIndex;

            // 将所有的背包物品先放回缓存池中，等待接下来更新背包界面的时候再使用
			bagItemsPool.AddChildInstancesToPool (bagItemsContainer);

            // 更新所有背包切换按钮的显示状态
			for (int i = 0; i < bagTabs.Length; i++)
            {
				if (i == bagIndex)
                {
                    if (i != 4)
                    {
                        bagTabs[i].GetComponentInChildren<Text>().color = CommonData.tabBarTitleSelectedColor;
                    }
                    else
                    {
                        bagTabs[i].transform.Find("EquipmentIcon").GetComponent<Image>().color = CommonData.tabBarTitleSelectedColor;
                    }

                }
                else
                {
                    if (i != 4)
                    {
                        bagTabs[i].GetComponentInChildren<Text>().color = CommonData.tabBarTitleNormalColor;
                    }
                    else
                    {
                        bagTabs[i].transform.Find("EquipmentIcon").GetComponent<Image>().color = Color.white;
                    }
                }
            }


			if (Player.mainPlayer.allItemsInBag.Count <= minItemIndexOfCurrentBag) {
				return;
			}
				
			for (int i = minItemIndexOfCurrentBag; i <= maxItemIndexOfCurrentBag; i++) {
				if (i >= Player.mainPlayer.allItemsInBag.Count) {
					return;
				}
				AddBagItem (Player.mainPlayer.allItemsInBag[i]);
			}

		}

        public void SetUpEquipedEquipments(){

            bagItemsPool.AddChildInstancesToPool(bagItemsContainer);
            
			for (int i = 0; i < bagTabs.Length; i++)
            {
                if (i == currentBagIndex)
                {
                    if (i != 4)
                    {
                        bagTabs[i].GetComponentInChildren<Text>().color = CommonData.tabBarTitleSelectedColor;
                    }
                    else
                    {
						bagTabs[i].transform.Find("EquipmentIcon").GetComponent<Image>().color = CommonData.tabBarTitleSelectedColor;
                    }

                }
                else
                {
                    if (i != 4)
                    {
                        bagTabs[i].GetComponentInChildren<Text>().color = CommonData.tabBarTitleNormalColor;
                    }
                    else
                    {
						bagTabs[i].transform.Find("EquipmentIcon").GetComponent<Image>().color = Color.white;
                    }
                }
            }

            for (int i = 0; i < Player.mainPlayer.allEquipedEquipments.Length; i++)
            {
                Equipment eqp = Player.mainPlayer.allEquipedEquipments[i];

                if(eqp.itemId < 0){
                    continue;
                }

                AddBagItem(eqp);
            }

        }

		public void UpdateBagTabs(){
			bagTabs[1].transform.Find("LockIcon").gameObject.SetActive(!BuyRecord.Instance.bag_2_unlocked);

            bagTabs[1].transform.GetComponentInChildren<Text>().enabled = BuyRecord.Instance.bag_2_unlocked;

			bagTabs[2].transform.Find("LockIcon").gameObject.SetActive(!BuyRecord.Instance.bag_3_unlocked);

            bagTabs[2].transform.GetComponentInChildren<Text>().enabled = BuyRecord.Instance.bag_3_unlocked;
		}

		public void ArrangeBag(){
			GameManager.Instance.soundManager.PlayAudioClip(CommonData.bagAudioName);
			Player.mainPlayer.ArrangeBagItems ();
			SetUpCurrentBagItemsPlane ();
		}

		public void SetUpCurrentBagItemsPlane(){
            if(currentBagIndex != 4){
                SetUpBagItemsPlane(currentBagIndex);
            }else{
                SetUpEquipedEquipments();        
            }
        }
			

        
		public void BagTabsChangeTo(int bagIndex){

			if (bagIndex == currentBagIndex)
            {
                return;
            }


			if (bagIndex == 1 && !BuyRecord.Instance.bag_2_unlocked)
            {
                initPurchaseBag(1);
                return;
            }         

			if (bagIndex == 2 && !BuyRecord.Instance.bag_3_unlocked)
            {
				if(!BuyRecord.Instance.bag_2_unlocked){
					initPurchaseBag(1);
				}
				else
				{
					initPurchaseBag(2);
				}

                return;
            }

            
			if (bagIndex == 3 && enterPurchaseGold != null) {
				enterPurchaseGold();         
				return;
			}
         
			currentBagIndex = bagIndex;

            if(bagIndex != 4){
                SetUpBagItemsPlane(bagIndex);
            }else{
                SetUpEquipedEquipments();
            }
			

		}


		/// <summary>
		/// 查询物品在背包中的位置序号（-1代表背包中没有这个物品）
		/// </summary>
		/// <returns>The item index in bag.</returns>
		/// <param name="item">Item.</param>
		public int FindItemIndexInBag(Item item){

			int itemIndex = -1;

			for (int i = 0; i < Player.mainPlayer.allItemsInBag.Count; i++) {
				if (Player.mainPlayer.allItemsInBag [i] == item) {
					itemIndex = i;
					return itemIndex;
				}

			}

			return itemIndex;

		}

		public void RemoveBagItemAt(int itemIndexInBag){
			int actualIndex = itemIndexInBag - minItemIndexOfCurrentBag;
			Transform removedItem = bagItemsContainer.GetChild (actualIndex);
			bagItemsPool.AddInstanceToPool (removedItem.gameObject);
			//AddSequenceItemsIfBagNotFull ();
		}

		public void RemoveBagItem(Item item){

			int itemIndexInBag = FindItemIndexInBag (item);
			int actualIndex = itemIndexInBag - minItemIndexOfCurrentBag;
			Transform removedItem = bagItemsContainer.GetChild (actualIndex);

			if (itemIndexInBag == -1) {
				Debug.LogError ("背包中没有找到物品：" + item.itemName);
			}

			bagItemsPool.AddInstanceToPool (removedItem.gameObject);

			//AddSequenceItemsIfBagNotFull ();
		}

		//public void AddSequenceItemsIfBagNotFull(){


		//	if (minItemIndexOfCurrentBag + bagItemsContainer.childCount >= Player.mainPlayer.allItemsInBag.Count) {
		//		return;
		//	}

		//	for (int i = minItemIndexOfCurrentBag + bagItemsContainer.childCount; i <= maxItemIndexOfCurrentBag; i++) {

		//		if (i >= Player.mainPlayer.allItemsInBag.Count) {
		//			return;
		//		}

		//		AddBagItem (Player.mainPlayer.allItemsInBag [i]);

		//	}

		//}

		/// <summary>
		/// 背包中单个物品按钮的初始化方法,序号-1代表添加到背包尾部
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="btn">Button.</param>
		public void AddBagItem(Item item,int atIndex = -1,bool forceAdd = false){

			// 如果当前背包已满
			if (bagItemsContainer.childCount == CommonData.singleBagItemVolume && !forceAdd) {
				return;
			}

				
			Transform bagItem = bagItemsPool.GetInstance<Transform> (bagItemModel.gameObject, bagItemsContainer);

			ItemInBagDragControl dragItemScript = bagItem.GetComponent<ItemInBagDragControl> ();
			dragItemScript.InitItemDragControl (item, itemClickCallBack);


			bagItem.GetComponent<ItemInBagCell> ().SetUpItemInBagCell (item);
            

			if (atIndex >= 0) {
				bagItem.SetSiblingIndex (atIndex - minItemIndexOfCurrentBag);
			}

		}

		public void SetSelectionIcon(int index,bool active){
			ItemInBagCell itemInBagCell = bagItemsContainer.GetChild(index).GetComponent<ItemInBagCell>();
			itemInBagCell.SetSelectionIcon(active);
		}

		public void HideAllItemSelectedTintIcon(){
			for (int i = 0; i < bagItemsContainer.childCount; i++) {
				bagItemsContainer.GetChild (i).Find ("SelectedTint").GetComponent<Image> ().enabled = false;
			}
		}

//		public int RemoveItemInBag(Item item){
//
//			int itemIndex = -1;
//
//			for (int i = 0; i < bagItemsContainer.childCount; i++) {
//
//				Transform bagItem = bagItemsContainer.GetChild (i);
//
//				if (bagItem.GetComponent<ItemDragControl> ().item == item) {
//
//					itemIndex = i;
//
//					bagItemsPool.AddInstanceToPool (bagItem.gameObject);
//
//					return itemIndex;
//				}
//			}
//
//			return itemIndex;
//
//		}

		public void QuitBagItemPlane(){
			bagItemsPool.AddChildInstancesToPool (bagItemsContainer);
		}



	}
}
