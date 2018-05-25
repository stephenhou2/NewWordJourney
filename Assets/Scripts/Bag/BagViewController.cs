using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BagViewController : MonoBehaviour {

		public BagView bagView;

		public Item currentSelectItem;

		public Item itemForSpecialOperation;

		private Item itemToAddWhenBagFull;

		void Awake(){

			for (int i = 300; i < 350;i++){
				Player.mainPlayer.AddItem(Item.NewItemWith(i, 1));

			}
		}

		public void AddBagItemWhenBagFull(Item item){

			SetUpBagView (true);

			bagView.SetUpSingleTextTintHUD ("背包已满，请先整理背包");

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

			if(setVisible){
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.bagAudioName);
			}


		}

		public void OnItemInEquipmentPlaneClick(Item item,int equipmentIndexInPanel){

			if (equipmentIndexInPanel == 6 && !BuyRecord.Instance.extraEquipmentSlotUnlocked) {
				bagView.SetUpPurchasePlane (PurchaseManager.extra_equipmentSlot_id);
			}

			if (item == null || item.itemId<0) {
				return;
			}

			OnItemInBagClick (item);

		}




		public void OnItemInBagClick(Item item){

			currentSelectItem = item;
         
			bagView.SetUpItemDetail (item);

			bagView.HideAllItemSelectedTintIcon ();


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

            // 如果额外装备槽已解锁，并且想要装上的装备是戒指，并且原有戒指槽已经有装备，则该戒指撞到额外装备槽上
            if(equipment.equipmentType == EquipmentType.Ring && BuyRecord.Instance.extraEquipmentSlotUnlocked && Player.mainPlayer.allEquipedEquipments[5].itemId >= 0 && Player.mainPlayer.allEquipedEquipments[6].itemId < 0){
                equipmentIndexInPanel = 6;
            }

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

			if (ExploreManager.Instance.battlePlayerCtr.isInFight) {
				ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons ();
			}

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
				bagView.SetUpSingleTextTintHUD ("背包已满,请先整理背包");
				return;
			}

			Equipment equipmentToUnload = currentSelectItem as Equipment;

			int equipmentIndexInPanel = Player.mainPlayer.GetEquipmentIndexInPanel (equipmentToUnload);


			PropertyChange propertyChange = Player.mainPlayer.UnloadEquipment (equipmentToUnload,equipmentIndexInPanel);

			bagView.SetUpEquipedEquipmentsPlane ();

			bagView.SetUpPlayerStatusPlane (propertyChange);

			bagView.AddBagItem (currentSelectItem);

			bagView.ClearItemDetail ();

			if (ExploreManager.Instance.battlePlayerCtr.isInFight) {
				ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons ();
			}
		}
			
        
		/// <summary>
		/// 在物品详细信息页点击了使用按钮
		/// </summary>
		public void OnUseButtonClick(){

			if (currentSelectItem == null) {
				return;
			}

			bool clearItemDetail = false;

			switch(currentSelectItem.itemType){
				case ItemType.Consumables:
					
					Consumables consumables = currentSelectItem as Consumables;

                    PropertyChange propertyChange = consumables.UseConsumables();

					bagView.SetUpPlayerStatusPlane(propertyChange);

					GameManager.Instance.soundManager.PlayAudioClip(consumables.audioName);

					break;
				case ItemType.SkillScroll:
					
					SkillScroll skillScroll = currentSelectItem as SkillScroll;

					propertyChange = skillScroll.UseSkillScroll();

					bagView.SetUpPlayerStatusPlane(propertyChange);

					break;
				case ItemType.SpecialItem:

					SpecialItem specialItem = currentSelectItem as SpecialItem;

					specialItem.UseSpecialItem(itemForSpecialOperation,bagView.itemDetail.SetUpItemDetail);

					bagView.propertyDisplay.UpdateStatusBars();

					break;

			}


			bool isLevelUp = Player.mainPlayer.LevelUpIfExperienceEnough();
            if (isLevelUp)
            {
                ExploreManager.Instance.expUICtr.ShowLevelUpPlane();
            }

			if (ExploreManager.Instance.battlePlayerCtr.isInFight)
            {
                ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();
            }

			clearItemDetail = Player.mainPlayer.RemoveItem(currentSelectItem, 1);


			if (itemToAddWhenBagFull != null && !Player.mainPlayer.CheckBagFull (itemToAddWhenBagFull)) {
				AddItemInWait ();
			}
                     
			bagView.SetUpCurrentBagItemsPlane();

			if (clearItemDetail) {
				bagView.ClearItemDetail ();
			}
            



		}
      

		public void OnRemoveButtonClick(){
			if (currentSelectItem == null) {
				return;
			}
			bagView.ShowRemoveQueryHUD ();

		}


		public void OnConfirmRemoveButtonClick(){
			if (currentSelectItem == null) {
				return;
			}

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.dropItemAudioName);

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


	



		// 退出背包界面
		public void OnQuitBagPlaneButtonClick(){

			itemToAddWhenBagFull = null;

			bagView.QuitBagPlane ();

			ExploreUICotroller expUICtr = ExploreManager.Instance.expUICtr;
			expUICtr.UpdatePlayerStatusBar ();
			expUICtr.UpdateBottomBar ();

			Time.timeScale = 1;

		}



		// 完全清理背包界面内存
		public void DestroyInstances(){

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.bagCanvasBundleName,true);

			Destroy (this.gameObject,0.3f);
		}

	}
}
