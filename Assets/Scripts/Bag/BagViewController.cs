using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BagViewController : MonoBehaviour {

		public BagView bagView;

		public Item currentSelectItem;

		private CallBack quitCallBack;

		void Awake(){
			//Equipment equipment = Item.NewItemWith(99, 1) as Equipment;
			//equipment.SetToGoldQuality();
			//Player.mainPlayer.AddItem(equipment);
			//Player.mainPlayer.AddItem(Item.NewItemWith(400, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(525, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(534, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(542, 1));

			//Player.mainPlayer.AddItem(Item.NewItemWith(59, 1));
			//Player.mainPlayer.AddItem(Item.NewItemWith(602, 10));
			//Player.mainPlayer.AddItem(Item.NewItemWith(600, 10));
   //         Player.mainPlayer.AddItem(Item.NewItemWith(601, 10));
   //         Player.mainPlayer.AddItem(Item.NewItemWith(603, 10));
			//Player.mainPlayer.AddItem(Item.NewItemWith(604, 3));
			//Player.mainPlayer.AddItem(Item.NewItemWith(605, 3));
			//Player.mainPlayer.AddItem(Item.NewItemWith(606, 3));
			//Player.mainPlayer.AddItem(Item.NewItemWith(607, 3));         
			//Player.mainPlayer.AddItem(Item.NewItemWith(608, 3));


			//Player.mainPlayer.AddItem(Item.NewItemWith(304, 10));
			//Player.mainPlayer.AddItem(Item.NewItemWith(307, 10));

			//Player.mainPlayer.totalGold = 6666;
		}



		public void SetUpBagView(bool setVisible, CallBack quitCallBack){

			this.quitCallBack = quitCallBack;

			bagView.SetUpBagView (setVisible);

			if(setVisible){
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.bagAudioName);
			}         
		}






		public void OnItemInBagClick(Item item){

			currentSelectItem = item;
         
			bagView.SetUpItemDetail (item);

			bagView.HideAllItemSelectedTintIcon ();
			bagView.HideAllEquipedEquipmentsSelectIcon();

			//Debug.Log("click " + currentSelectItem);
         
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

			//Debug.Log("use " + currentSelectItem);

			if (currentSelectItem == null) {
				return;
			}

			bool clearItemDetail = false;

			Item specialOperaitonItem = null;

			bool totallyRemoved = true;

			switch(currentSelectItem.itemType){
				case ItemType.Consumables:
					
					Consumables consumables = currentSelectItem as Consumables;

					PropertyChange propertyChange = consumables.UseConsumables(null);

					if(consumables.itemCount > 0){                  
						totallyRemoved = false;                  
					}

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

					totallyRemoved = true;

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

							if(equipment.attachedPropertyGemstones.Count == 0)
							{
								bagView.hintHUD.SetUpSingleTextTintHUD("当前装备未镶嵌宝石");
								return;
							}

							int addItemCount = 0;

							for (int i = 0; i < equipment.attachedPropertyGemstones.Count;i++){
								PropertyGemstone propertyGemstone = equipment.attachedPropertyGemstones[i];
								bool gemstoneExist = Player.mainPlayer.CheckItemExistInBag(propertyGemstone);
								if(!gemstoneExist){
									addItemCount++;
								}                         
							}

							if(specialItem.itemCount == 1){
								addItemCount--;
							}

							bool bagFull = Player.mainPlayer.allItemsInBag.Count + addItemCount >= Player.mainPlayer.maxBagCount * CommonData.singleBagItemVolume;

							if(bagFull){
								bagView.hintHUD.SetUpSingleTextTintHUD("背包已满");
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
			}else if(!totallyRemoved){
				int itemIndexInBag = Player.mainPlayer.GetItemIndexInBag(currentSelectItem);
				if (itemIndexInBag >= 0)
                {
					int itemIndexInCurrentBag = itemIndexInBag % CommonData.singleBagItemVolume;
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

			//Item itemInSoCell = bagView.itemDetail.soCell.soDragControl.item;

			//if (itemInSoCell != null) {
			//	Player.mainPlayer.AddItem (itemInSoCell);
			//}

			bagView.SetUpCurrentBagItemsPlane ();
			ExploreManager.Instance.expUICtr.UpdateBottomBar();

			PropertyChange propertyChange = new PropertyChange();

			if (currentSelectItem.itemType == ItemType.Equipment) {
				Equipment eqp = currentSelectItem as Equipment;
				if (eqp.equiped) {
					propertyChange = Player.mainPlayer.ResetBattleAgentProperties(false);
					bagView.SetUpEquipedEquipmentsPlane ();
				}
			}

			bagView.SetUpPlayerStatusPlane (propertyChange);

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

			if(quitCallBack != null){
				quitCallBack();
			}

		}



		// 完全清理背包界面内存
		public void DestroyInstances(){

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.bagCanvasBundleName,true);

			Destroy (this.gameObject,0.3f);
		}

	}
}
