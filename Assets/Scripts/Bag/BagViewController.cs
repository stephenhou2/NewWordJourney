using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BagViewController : MonoBehaviour {

		public BagView bagView;

		public Item currentSelectItem;

		//public Item itemForSpecialOperation;
        
		//private Item itemToAddWhenBagFull;
        
		void Awake(){

			//for (int i = 501; i < 545;i++){
			//	Player.mainPlayer.AddItem(Item.NewItemWith(i, 1));
			//}

			//Player.mainPlayer.AddItem(Item.NewItemWith(600, 10));
			//Player.mainPlayer.AddItem(Item.NewItemWith(601, 10));
			//Player.mainPlayer.AddItem(Item.NewItemWith(1, 21));
			//Player.mainPlayer.AddItem(Item.NewItemWith(11, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(12, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(1, 1));

			//Player.mainPlayer.AddItem(Item.NewItemWith(507, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(508, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(509, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(511, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(513, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(514, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(515, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(527, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(531, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(539, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(543, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(544, 1));
		}

		//public void AddBagItemWhenBagFull(Item item){

		//	SetUpBagView (true);

		//	bagView.SetUpSingleTextTintHUD ("背包已满，请先整理背包");

		//	itemToAddWhenBagFull = item;

		//}



		public void SetUpBagView(bool setVisible){

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

			currentSelectItem = item;

            bagView.SetUpItemDetail(item);
         
			bagView.HideAllEquipedEquipmentsSelectIcon();
			bagView.HideAllItemSelectedTintIcon();
         
		}




		public void OnItemInBagClick(Item item){

			currentSelectItem = item;
         
			bagView.SetUpItemDetail (item);

			bagView.HideAllItemSelectedTintIcon ();
			bagView.HideAllEquipedEquipmentsSelectIcon();
         
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
				bagView.AddBagItem(equipmentToUnload);
			}

			PropertyChange propertyChangeFromEquip = Player.mainPlayer.EquipEquipment (currentSelectItem as Equipment, equipmentIndexInPanel);

			PropertyChange finalPropertyChange = PropertyChange.MergeTwoPropertyChange (propertyChangeFromUnload, propertyChangeFromEquip);

			bagView.SetUpEquipedEquipmentsPlane ();

			bagView.SetUpPlayerStatusPlane (finalPropertyChange);

			//bagView.RemoveBagItemAt (oriItemIndexInBag);

			bagView.SetUpCurrentBagItemsPlane();

			//AddItemInWait ();

			bagView.SetUpEquipedEquipmentsPlane ();

			bagView.itemDetail.SetUpOperationButtons (false, true, false);

			if (ExploreManager.Instance.battlePlayerCtr.isInFight) {
				ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons ();
			}

		}


//		public void AddItemInWait(){

//			if (itemToAddWhenBagFull == null) {
//				return;
//			}
			
//			Player.mainPlayer.AddItem (itemToAddWhenBagFull);

//			bagView.AddBagItem (itemToAddWhenBagFull);

//			itemToAddWhenBagFull = null;

////			string tint = "";
////
////			switch (itemToAddWhenBagFull.itemType) {
////			case ItemType.UnlockScroll:
////				tint = string.Format ("获得 解锁卷轴{0}{1}{2}", CommonData.diamond, itemToAddWhenBagFull.itemName, CommonData.diamond);
////				break;
////			case ItemType.CraftingRecipes:
////				tint = string.Format ("获得 合成卷轴{0}{1}{2}", CommonData.diamond, itemToAddWhenBagFull.itemName, CommonData.diamond);
////				break;
////			default:
////				tint = string.Format ("获得 {0} x1", itemToAddWhenBagFull.itemName);
////				break;
////			}
////
////			Sprite goodsSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
////				return obj.name == itemToAddWhenBagFull.spriteName;
////			});
////
////			bagView.SetUpTintHUD (tint,goodsSprite);

		//}

		/// <summary>
		/// 在物品详细信息页点击了卸下按钮（装备）
		/// </summary>
		public void OnUnloadButtonClick(){

			if (currentSelectItem == null) {
				return;
			}

			if (Player.mainPlayer.CheckBagFull (currentSelectItem)) {
				bagView.SetUpSingleTextTintHUD ("背包已满");
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

			Item specialOperaitonItem = null;

			switch(currentSelectItem.itemType){
				case ItemType.Consumables:
					
					Consumables consumables = currentSelectItem as Consumables;

					PropertyChange propertyChange = consumables.UseConsumables(null);

					bool isLevelUp = Player.mainPlayer.LevelUpIfExperienceEnough();
                    if (isLevelUp)
                    {
						ExploreManager.Instance.battlePlayerCtr.SetEffectAnim(CommonData.levelUpEffectName);
                        GameManager.Instance.soundManager.PlayAudioClip(CommonData.levelUpAudioName);
                        ExploreManager.Instance.expUICtr.ShowLevelUpPlane();
                    }

					bagView.SetUpPlayerStatusPlane(propertyChange);

					GameManager.Instance.soundManager.PlayAudioClip(consumables.audioName);

					break;
				case ItemType.SkillScroll:

					SkillScroll skillScroll = currentSelectItem as SkillScroll;

					if(Player.mainPlayer.allLearnedSkills.Count >= 6){
						bagView.SetUpSingleTextTintHUD("只能学习6个技能");
						return;
					}

					bool skillHasLearned = Player.mainPlayer.CheckSkillHasLearned(skillScroll.skillId);

					if(skillHasLearned){
						bagView.SetUpSingleTextTintHUD("不能重复学习技能");
						return;
					}
               
					propertyChange = skillScroll.UseSkillScroll();

					GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);

					bagView.SetUpPlayerStatusPlane(propertyChange);

					break;
				case ItemType.SpecialItem:

					SpecialItem specialItem = currentSelectItem as SpecialItem;

					Item itemForSpecialOperation = bagView.itemDetail.soCell.itemInCell;

					specialOperaitonItem = itemForSpecialOperation;

					//currentSelectItem = specialItem;

					switch(specialItem.specialItemType){
						case SpecialItemType.ChongZhuShi:
						case SpecialItemType.DianJinFuShi:
							if(itemForSpecialOperation == null){
								return;
							}                     
							break;
						case SpecialItemType.TuiMoJuanZhou:
							if (itemForSpecialOperation == null)
                            {
                                return;
                            }
							Equipment equipment = itemForSpecialOperation as Equipment;
							if(equipment.attachedPropertyGemstone.itemId == -1)
							{
								bagView.tintHUD.SetUpSingleTextTintHUD("当前装备未镶嵌宝石");
								return;
							}

							break;
						default:
							break;

					}

					propertyChange = specialItem.UseSpecialItem(itemForSpecialOperation,bagView.itemDetail.SetUpItemDetail);

					bagView.SetUpEquipedEquipmentsPlane();

					bagView.SetUpPlayerStatusPlane(propertyChange);

					break;

			}




			if (ExploreManager.Instance.battlePlayerCtr.isInFight)
            {
                ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();
            }

			clearItemDetail = Player.mainPlayer.RemoveItem(currentSelectItem, 1);
           
                     
			bagView.SetUpCurrentBagItemsPlane();

			if (clearItemDetail) {
				bagView.ClearItemDetail ();
			}

            // 进行特殊操作的物品，特殊操作结束后显示被操作物品的信息，并在背包中将该物品的选中框高亮
			if(specialOperaitonItem != null){
				currentSelectItem = specialOperaitonItem;
				bagView.SetUpItemDetail(specialOperaitonItem);
				int specialOperaitonItemIndexInBag = Player.mainPlayer.GetItemIndexInBag(specialOperaitonItem);
				if(specialOperaitonItemIndexInBag >= 0){
					int itemIndexInCurrentBag = specialOperaitonItemIndexInBag % CommonData.singleBagItemVolume;
					bagView.bagItemsDisplay.SetSelectionIcon(itemIndexInCurrentBag, true);
                }
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
			ExploreManager.Instance.expUICtr.UpdateBottomBar();

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


		// 退出背包界面
		public void OnQuitBagPlaneButtonClick(){

			//itemToAddWhenBagFull = null;

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
