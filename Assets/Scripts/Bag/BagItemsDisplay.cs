using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class BagItemsDisplay : MonoBehaviour {

		public InstancePool bagItemsPool;
		public Transform bagItemsContainer;
		public Transform bagItemModel;

		public Button[] bagTabs;
		public Sprite bagTabSelectedIcon;
		public Sprite bagTabDeselectedIcon;

		public Text totalGoldText;


//		private int singleBagItemVolume = 24;
        public int currentBagIndex;

		private int minItemIndexOfCurrentBag {
			get {
				return currentBagIndex * CommonData.singleBagItemVolume;
			}
		}

		private int maxItemIndexOfCurrentBag {
			get {
				return minItemIndexOfCurrentBag + CommonData.singleBagItemVolume - 1;
			}
		}
		private ShortClickCallBack shortClickCallBack;
		private CallBack initPurchaseBag;

		public void InitBagItemsDisplayPlane(ShortClickCallBack shortClickCallBack,CallBack initPurchaseBag){
			this.shortClickCallBack = shortClickCallBack;
			this.initPurchaseBag = initPurchaseBag;
		}

		/// <summary>
		/// 初始化背包物品界面
		/// </summary>
		public void SetUpBagItemsPlane(int bagIndex){

			totalGoldText.text = Player.mainPlayer.totalGold.ToString ();

			bagItemsPool.AddChildInstancesToPool (bagItemsContainer);

			for (int i = 0; i < bagTabs.Length; i++) {
				if (i == currentBagIndex) {
					bagTabs[i].GetComponent<Image>().sprite = bagTabSelectedIcon;
				}else{
					bagTabs[i].GetComponent<Image>().sprite = bagTabDeselectedIcon;
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
            
            totalGoldText.text = Player.mainPlayer.totalGold.ToString();

            bagItemsPool.AddChildInstancesToPool(bagItemsContainer);

            for (int i = 0; i < bagTabs.Length; i++)
            {
                if (i == currentBagIndex)
                {
                    bagTabs[i].GetComponent<Image>().sprite = bagTabSelectedIcon;
                }
                else
                {
                    bagTabs[i].GetComponent<Image>().sprite = bagTabDeselectedIcon;
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
			bagTabs [3].transform.Find ("LockIcon").gameObject.SetActive (!BuyRecord.Instance.extraBagUnlocked);
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
			


		public void ChangeBag(int bagIndex){

			if (bagIndex == 3 && !BuyRecord.Instance.extraBagUnlocked) {
				initPurchaseBag ();
				return;
			}

			if (bagIndex == currentBagIndex) {
				return;
			}

			//GameManager.Instance.soundManager.PlayAudioClip(CommonData.bagAudioName);

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
			AddSequenceItemsIfBagNotFull ();
		}

		public void RemoveBagItem(Item item){

			int itemIndexInBag = FindItemIndexInBag (item);
			int actualIndex = itemIndexInBag - minItemIndexOfCurrentBag;
			Transform removedItem = bagItemsContainer.GetChild (actualIndex);

			if (itemIndexInBag == -1) {
				Debug.LogError ("背包中没有找到物品：" + item.itemName);
			}

			bagItemsPool.AddInstanceToPool (removedItem.gameObject);

			AddSequenceItemsIfBagNotFull ();
		}

		public void AddSequenceItemsIfBagNotFull(){


			if (minItemIndexOfCurrentBag + bagItemsContainer.childCount >= Player.mainPlayer.allItemsInBag.Count) {
				return;
			}

			for (int i = minItemIndexOfCurrentBag + bagItemsContainer.childCount; i <= maxItemIndexOfCurrentBag; i++) {

				if (i >= Player.mainPlayer.allItemsInBag.Count) {
					return;
				}

				AddBagItem (Player.mainPlayer.allItemsInBag [i]);

			}

		}

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
			dragItemScript.InitItemInBagDragControl (item, shortClickCallBack);


			bagItem.GetComponent<ItemInBagCell> ().SetUpItemInBagCell (item);


			if (atIndex >= 0) {
				bagItem.SetSiblingIndex (atIndex - minItemIndexOfCurrentBag);
			}

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
