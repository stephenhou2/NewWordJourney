using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BagViewController : MonoBehaviour {

		public BagView bagView;

		public Item currentSelectItem;

		public PurchaseManager purchaseManager;

//		private int currentBagIndex;

		private Item itemToAddWhenBagFull;

		void Awake(){
			for (int i = 1; i < 33; i++) {
				Player.mainPlayer.AddItem (Item.NewItemWith (i, 1));
			}
			for(int i = 300;i<316;i++){
				Player.mainPlayer.AddItem (Item.NewItemWith (i, 3));
			}
			for (int i = 401; i < 403; i++) {
				Player.mainPlayer.AddItem (Item.NewItemWith (i, 1));
			}
		}

		public void AddBagItemWhenBagFull(Item item){

			SetUpBagView (true);

			bagView.SetUpTintHUD ("背包已满，请先整理背包", null);

			itemToAddWhenBagFull = item;

		}



		public void SetUpBagView(bool setVisible){

//			StartCoroutine ("SetUpViewAfterDataReady",setVisible);
//		}
//
//		private IEnumerator SetUpViewAfterDataReady(bool setVisible){
//
//			bool dataReady = false;
//
//			while (!dataReady) {
//				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
//					GameDataCenter.GameDataType.UISprites,
//					GameDataCenter.GameDataType.ItemModels,
//					GameDataCenter.GameDataType.ItemSprites,
//					GameDataCenter.GameDataType.Skills
//				});
//				yield return null;
//			}

			bagView.SetUpBagView (setVisible);

		}

		public void OnItemInEquipmentPlaneClick(Item item,int equipmentIndexInPanel){

			if (equipmentIndexInPanel == 6 && !BuyRecord.Instance.extraEquipmentSlotUnlocked) {
				string productId = "";
//				if (equipmentIndexInPanel == 4) {
					productId = PurchaseManager.equipmentSlot_6_id;
//				} else if (equipmentIndexInPanel == 5) {
//					productId = PurchaseManager.equipmentSlot_6_id;
//				}

				InitPurchase (productId);
			}

			if (item == null) {
				return;
			}

			OnItemInBagClick (item);

		}

		private void InitPurchase(string productId){
			bagView.SetUpPurchasePlane ();
			purchaseManager.PurchaseProduct (productId, PurchaseSuccessCallback, PurchaseFailCallback);

		}

		private void PurchaseSuccessCallback(){

			bagView.SetUpEquipedEquipmentsPlane ();

			bagView.QuitPurchasePlane ();
		}

		private void PurchaseFailCallback(){

			bagView.QuitPurchasePlane ();
		}

		public void OnItemInBagClick(Item item){

			currentSelectItem = item;

//			if (item is CraftingRecipe) {
//				bagView.SetUpCraftRecipesDetailHUD (item);
//			} else {
				bagView.SetUpItemDetail (item);
			bagView.HideAllItemSelectedTintIcon ();

//			}

		}

		/// <summary>
		/// 在物品详细信息页点击了装备按钮（装备）
		/// </summary>
		public void OnEquipButtonClick(){

			if (currentSelectItem == null) {
				return;
			}

//			bool[] equipmentSlotUnlockedArray = BuyRecord.Instance.equipmentSlotUnlockedArray;

			Equipment equipment = currentSelectItem as Equipment;

			int equipmentIndexInPanel = (int)equipment.equipmentType;

			int oriItemIndexInBag = Player.mainPlayer.GetItemIndexInBag (currentSelectItem);

			PropertyChange propertyChangeFromUnload = new PropertyChange();

			if (Player.mainPlayer.allEquipedEquipments [equipmentIndexInPanel].itemId >= 0) {
				Equipment equipmentToUnload = Player.mainPlayer.allEquipedEquipments [equipmentIndexInPanel];
				propertyChangeFromUnload = Player.mainPlayer.UnloadEquipment (equipmentToUnload, equipmentIndexInPanel);
			}

			PropertyChange propertyChangeFromEquip = Player.mainPlayer.EquipEquipment (currentSelectItem as Equipment, equipmentIndexInPanel);

			PropertyChange finalPropertyChange = PropertyChange.MergeTwoPropertyChange (propertyChangeFromUnload, propertyChangeFromEquip);

			bagView.SetUpEquipedEquipmentsPlane ();

			bagView.SetUpPlayerStatusPlane (finalPropertyChange);

			bagView.RemoveBagItemAt (oriItemIndexInBag);

			AddItemInWait ();

			bagView.SetUpEquipedEquipmentsPlane ();

			bagView.itemDetail.SetUpOperationButtons (false, true, false);
//			bagView.SetUpTintHUD ("装备栏已满",null);

		}


		public void AddItemInWait(){

			if (itemToAddWhenBagFull == null) {
				return;
			}
			
			Player.mainPlayer.AddItem (itemToAddWhenBagFull);

			bagView.AddBagItem (itemToAddWhenBagFull);

			itemToAddWhenBagFull = null;

//			string tint = "";
//
//			switch (itemToAddWhenBagFull.itemType) {
//			case ItemType.UnlockScroll:
//				tint = string.Format ("获得 解锁卷轴{0}{1}{2}", CommonData.diamond, itemToAddWhenBagFull.itemName, CommonData.diamond);
//				break;
//			case ItemType.CraftingRecipes:
//				tint = string.Format ("获得 合成卷轴{0}{1}{2}", CommonData.diamond, itemToAddWhenBagFull.itemName, CommonData.diamond);
//				break;
//			default:
//				tint = string.Format ("获得 {0} x1", itemToAddWhenBagFull.itemName);
//				break;
//			}
//
//			Sprite goodsSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
//				return obj.name == itemToAddWhenBagFull.spriteName;
//			});
//
//			bagView.SetUpTintHUD (tint,goodsSprite);

		}

		/// <summary>
		/// 在物品详细信息页点击了卸下按钮（装备）
		/// </summary>
		public void OnUnloadButtonClick(){

			if (currentSelectItem == null) {
				return;
			}

			if (Player.mainPlayer.CheckBagFull (currentSelectItem)) {
				bagView.SetUpTintHUD ("背包已满",null);
				return;
			}

			Equipment equipmentToUnload = currentSelectItem as Equipment;

			int equipmentIndexInPanel = Player.mainPlayer.GetEquipmentIndexInPanel (equipmentToUnload);


			PropertyChange propertyChange = Player.mainPlayer.UnloadEquipment (equipmentToUnload,equipmentIndexInPanel);

			bagView.SetUpEquipedEquipmentsPlane ();

			bagView.SetUpPlayerStatusPlane (propertyChange);

			bagView.AddBagItem (currentSelectItem);

			bagView.ClearItemDetail ();

	
		}
			

		/// <summary>
		/// 在物品详细信息页点击了使用按钮（消耗品）
		/// </summary>
		public void OnUseButtonClick(){

			if (currentSelectItem == null) {
				return;
			}
			
			Consumables consumables = currentSelectItem as Consumables;

			Player player = Player.mainPlayer;

			bool removeAndUpdate = false;

			switch (consumables.type) {
			case ConsumablesType.ShuXingTiSheng:

				player.health += consumables.healthGain;
				player.mana += consumables.manaGain;
				player.experience += consumables.experienceGain;

				player.RemoveItem (consumables, 1);

				bool isLevelUp = player.LevelUpIfExperienceEnough ();
				if (isLevelUp) {
					ExploreManager.Instance.expUICtr.ShowLevelUpPlane ();
				}

				bagView.SetUpPlayerStatusPlane (new PropertyChange());
				removeAndUpdate = true;

				break;  
			case ConsumablesType.ChongZhuShi:
				Equipment eqp = bagView.RebuildEquipment ();
				if (eqp != null) {
					currentSelectItem = eqp;
					removeAndUpdate = true;
				}
				break;
			case ConsumablesType.DianJinShi:
				eqp = bagView.UpgradeEquipmentToGold ();
				if (eqp != null) {
					currentSelectItem = eqp;
					removeAndUpdate = true;
				}
				break;
			case ConsumablesType.XiaoMoJuanZhou:
				eqp = bagView.RemoveEquipmentAttachedSkill ();
				if (eqp != null) {
					currentSelectItem = eqp;
					removeAndUpdate = true;
				}
				break;
			case ConsumablesType.YinShenJuanZhou:
				ExploreManager.Instance.PlayerFade ();
				removeAndUpdate = true;
				break;
			}


			if (itemToAddWhenBagFull != null && !Player.mainPlayer.CheckBagFull (itemToAddWhenBagFull)) {
				AddItemInWait ();
			}
				
			if (removeAndUpdate) {
				player.RemoveItem (consumables, 1);
				bagView.SetUpCurrentBagItemsPlane ();
			}

		}


		public void OnRemoveButtonClick(){
			bagView.ShowRemoveQueryHUD ();
		}


		public void OnConfirmRemoveButtonClick(){
			if (currentSelectItem == null) {
				return;
			}

			Player.mainPlayer.RemoveItem (currentSelectItem,currentSelectItem.itemCount);

			Item itemInSoCell = bagView.itemDetail.soCell.soDragControl.item;

			if (itemInSoCell != null) {
				Player.mainPlayer.AddItem (itemInSoCell);
			}

			bagView.SetUpCurrentBagItemsPlane ();

			if (currentSelectItem.itemType == ItemType.Equipment) {
				Equipment eqp = currentSelectItem as Equipment;
				if (eqp.equiped) {
					bagView.SetUpEquipedEquipmentsPlane ();
				}
			}

			bagView.SetUpPlayerStatusPlane (new PropertyChange());

			bagView.ClearItemDetail ();

			currentSelectItem = null;

			bagView.HideRemoveQueryHUD ();
		}

		public void OnCancelRemoveButtonClick(){
			bagView.HideRemoveQueryHUD ();
		}


		/// <summary>
		/// 在物品详细信息页点击了分解按钮
		/// </summary>
//		public void OnResolveButtonClick(){
//			bagView.ShowQueryResolveHUD ();
//		}


		/// <summary>
		/// 在分解确认页点击了确认按钮
		/// </summary>
//		public void OnConfirmResolveButtonClick(){
////			ResolveCurrentSelectItemAndGetCharacters ();
//			bagView.QuitQueryResolveHUD ();
//			bagView.QuitItemDetailHUD ();
//			if (!Player.mainPlayer.CheckBagFull ()) {
//				AddItemInWait ();
//			}
//		}

		/// <summary>
		/// 在分解确认页点击了取消按钮
		/// </summary>
//		public void OnCancelResolveButtonClick(){
//			bagView.QuitQueryResolveHUD ();
//		}




		/// <summary>
		/// 分解物品并获得字母碎片
		/// </summary>
		/// <param name="item">Item.</param>
//		public void ResolveCurrentSelectItemAndGetCharacters(){
//
//			List<char> charactersReturn = Player.mainPlayer.ResolveItemAndGetCharacters (currentSelectItem,1);
//
////			List<CharacterFragment> resolveGainCharacterFragments = new List<CharacterFragment> ();
//
//			// 返回的有字母，生成字母碎片表
////			if (charactersReturn.Count > 0) {
////
////				foreach (char c in charactersReturn) {
////					resolveGainCharacterFragments.Add (new CharacterFragment (c));
////				}
////
////			}
//
////			Item itemInBag = Player.mainPlayer.allItemsInBag.Find (delegate(Item obj) {
////				return obj == currentSelectItem;
////			});
////
////			if (itemInBag == null) {
////				bagView.QuitItemDetailHUD ();
////			}
//
//			if (currentSelectItem is Equipment && (currentSelectItem as Equipment).equiped) {
//				bagView.SetUpEquipedEquipmentsPlane ();
//			}
//
//
//			bagView.SetUpBagItemsPlane (currentBagIndex);
//				
//			bagView.SetUpResolveGainHUD (charactersReturn);
//		}

//		public void CraftCurrentSelectItem(){
//
//			CraftingRecipe craftRecipe = currentSelectItem as CraftingRecipe;
//
//			EquipmentModel craftItemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(EquipmentModel obj) {
//				return obj.itemId == craftRecipe.craftItemId;
//			});
//
//			for (int i = 0; i < craftItemModel.itemInfosForProduce.Length; i++) {
//				EquipmentModel.ItemInfoForProduce itemInfo = craftItemModel.itemInfosForProduce [i];
//				for (int j = 0; j < itemInfo.itemCount; j++) {
//					Item item = Player.mainPlayer.allItemsInBag.Find (delegate (Item obj) {
//						return obj.itemId == itemInfo.itemId;
//					});
////					bagView.RemoveBagItem (item);
//
//					if (item == null) {
//						item = Player.mainPlayer.GetEquipedEquipment (itemInfo.itemId);
//					}
//
//					if (item != null) {
//						Player.mainPlayer.RemoveItem (item, 1);
//					}
//				}
//			}
//
//			Item craftedItem = Item.NewItemWith (craftItemModel,1);
////			bagView.AddBagItem (craftedItem);
//
//			string tint = string.Format ("获得 <color=orange>{0}</color> x1", craftedItem.itemName);
//
//			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
//				return obj.name == craftedItem.spriteName;
//			});
//
////			int oriIndexOfCraftingRecipe = Player.mainPlayer.GetItemIndexInBag (currentSelectItem);
//			Player.mainPlayer.RemoveItem (currentSelectItem,1);
//			Player.mainPlayer.AddItem (craftedItem);
//
//			bagView.SetUpTintHUD (tint,itemSprite);
////			bagView.RemoveBagItemAt (oriIndexOfCraftingRecipe);
//
//			if (Player.mainPlayer.CheckBagFull ()) {
//				AddItemInWait ();
//			}
//
//			bagView.SetUpCurrentBagItemsPlane ();
//			bagView.SetUpEquipedEquipmentsPlane ();
//
//		}

		public void OnEquipmentSlotPurchaseSucceed(int slotIndex){

			BuyRecord.Instance.extraEquipmentSlotUnlocked = true;

			GameManager.Instance.persistDataManager.SaveBuyRecord ();

			bagView.SetUpTintHUD (string.Format ("成功解锁装备槽{0}", slotIndex + 1), null);

			bagView.SetUpEquipedEquipmentsPlane ();
		}


		public void OnEquipmentSlotPurchaseFail(int slotIndex){

			bagView.SetUpTintHUD ("购买失败", null);

		}
	

//		public void RemoveItem(Item item){
////			bagView.SetUpBagItemsPlane (currentBagIndex);
//			bagView.RemoveBagItem (item);
//		}



		// 退出背包界面
		public void OnQuitBagPlaneButtonClick(){

			itemToAddWhenBagFull = null;

			bagView.QuitBagPlane ();

//			Transform exploreCanvas = TransformManager.FindTransform ("ExploreCanvas");
//
//			if (exploreCanvas == null) {
//
//				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
//
//					TransformManager.FindTransform ("HomeCanvas").GetComponent<HomeViewController> ().SetUpHomeView ();
//
////					GameManager.Instance.gameDataCenter.ReleaseDataWithNames(new string[]{"AllItemSprites","AllMaterialSprites","AllMaterials","AllItemModels"});
//
////					TransformManager.DestroyTransfromWithName ("PoolContainerOfBagCanvas", TransformRoot.PoolContainer);
//				});
//			} else {
				ExploreUICotroller expUICtr = ExploreManager.Instance.expUICtr;
				expUICtr.UpdatePlayerStatusBar ();
				expUICtr.UpdateBottomBar ();
//			}


			Time.timeScale = 1;

		}



		// 完全清理背包界面内存
		public void DestroyInstances(){

//			GameManager.Instance.UIManager.RemoveCanvasCache ("BagCanvas");


			MyResourceManager.Instance.UnloadAssetBundle (CommonData.bagCanvasBundleName,true);


//			StartCoroutine ("LatelyDestroyView");
//		}
//
//		private IEnumerator LatelyDestroyView(){
//
//			yield return new WaitForSeconds (0.3f);

			Destroy (this.gameObject,0.3f);
		}


		void OnDestroy(){
			Debug.Log ("销毁背包");
			bagView = null;
			currentSelectItem = null;
			purchaseManager = null;
			itemToAddWhenBagFull = null;
		}
	}
}
