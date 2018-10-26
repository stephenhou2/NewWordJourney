using UnityEngine;


namespace WordJourney
{
	public class BagViewController : MonoBehaviour {

        // 背包界面
		public BagView bagView;

        // 当前背包中选中的物品
		public Item currentSelectItem;

        // 退出背包的回调
		private CallBack quitCallBack;

		void Awake(){
		}


        /// <summary>
        /// 初始化背包界面
        /// </summary>
        /// <param name="setVisible">If set to <c>true</c> set visible.</param>
        /// <param name="quitCallBack">Quit call back.</param>
		public void SetUpBagView(bool setVisible, CallBack quitCallBack){

			this.quitCallBack = quitCallBack;

			bagView.SetUpBagView (setVisible);

            // 如果要显示背包界面，则同时播放背包打开的声音
			if(setVisible){
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.bagAudioName);
			}         
		}
        
        /// <summary>
        /// 背包中物品的点击响应
        /// </summary>
        /// <param name="item">Item.</param>
		public void OnItemInBagClick(Item item){

			currentSelectItem = item;
         
			bagView.SetUpItemDetail (item);

			bagView.HideAllSelectedIcon();
         
		}

		/// <summary>
        /// 装备栏中的装备的点击响应方法
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="equipmentIndexInPanel">Equipment index in panel.</param>
        public void OnItemInEquipmentPlaneClick(Item item, int equipmentIndexInPanel)
        {
            // 如果点击的是装备槽6，并且还没有解锁，则显示购买界面
            if (equipmentIndexInPanel == 6 && !BuyRecord.Instance.extraEquipmentSlotUnlocked)
            {
#if UNITY_IOS
				bagView.SetUpPurchasePlaneOnIPhone(PurchaseManager.extra_equipmentSlot_id);
#elif UNITY_ANDROID
				bagView.SetUpPurchasePlaneOnAndroid(PurchaseManager.extra_equipmentSlot_id);
#elif UNITY_EDITOR
                UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

                switch (buildTarget) {
                    case UnityEditor.BuildTarget.Android:
        				bagView.SetUpPurchasePlaneOnAndroid(PurchaseManager.extra_equipmentSlot_id);
                        break;
                    case UnityEditor.BuildTarget.iOS:
        				bagView.SetUpPurchasePlaneOnIPhone(PurchaseManager.extra_equipmentSlot_id);
                        break;
                }
#endif
            }

            // 装备槽中没有装备，则直接返回
            // 默认装备槽中都放了一个空装备占位，空装备的id为-1
            if (item == null || item.itemId < 0)
            {
                return;
            }

            // 更新当前选中物品
            GetComponent<BagViewController>().currentSelectItem = item;

            // 显示物品详细信息
			bagView.SetUpItemDetail(item);

            // 隐藏所有装备和物品的选中图标
			bagView.HideAllSelectedIcon();

        }


		/// <summary>
		/// 在物品详细信息页点击了装备按钮（装备）
		/// </summary>
		public void OnEquipButtonClick(){

            // 当前选中的物品为空时，直接返回
			if (currentSelectItem == null) {
				return;
			}

			Equipment equipment = currentSelectItem as Equipment;

            // 装备的类【武器，护甲，头盔等】
			int equipmentIndexInPanel = (int)equipment.equipmentType;

            // 获取物品在背包中的序号
			int oriItemIndexInBag = Player.mainPlayer.GetItemIndexInBag (currentSelectItem);

            // 创建一个空的属性变化对象
            PropertyChange propertyChangeFromUnload = new PropertyChange();

            // 如果额外装备槽已解锁，并且想要装上的装备是戒指，并且原有戒指槽已经有装备，则该戒指撞到额外装备槽上
            if(equipment.equipmentType == EquipmentType.Ring && BuyRecord.Instance.extraEquipmentSlotUnlocked && Player.mainPlayer.allEquipedEquipments[5].itemId >= 0 && Player.mainPlayer.allEquipedEquipments[6].itemId < 0){
                equipmentIndexInPanel = 6;
            }

            // 如果已经有同类型的已装备的额物品，则卸下原装备
            if (Player.mainPlayer.allEquipedEquipments [equipmentIndexInPanel].itemId >= 0) {
				Equipment equipmentToUnload = Player.mainPlayer.allEquipedEquipments [equipmentIndexInPanel];
                // 卸下原装备的属性变化
				propertyChangeFromUnload = Player.mainPlayer.UnloadEquipment (equipmentToUnload, equipmentIndexInPanel);
				bagView.AddBagItem(equipmentToUnload);
			}

            // 装备物品产生的属性变化
			PropertyChange propertyChangeFromEquip = Player.mainPlayer.EquipEquipment (currentSelectItem as Equipment, equipmentIndexInPanel);

            // 合并两个属性变化获得最终的属性变化
			PropertyChange finalPropertyChange = PropertyChange.MergeTwoPropertyChange (propertyChangeFromUnload, propertyChangeFromEquip);

            // 更新装备面板
			bagView.SetUpEquipedEquipmentsPlane ();

            // 更新玩家属性面板
			bagView.SetUpPlayerStatusPlane (finalPropertyChange);

            // 更新背包物品面板
			bagView.UpdateCurrentBagItemsPlane();

            // 更新操作按钮
			bagView.itemDetail.SetUpOperationButtons (false, true, false);

            // 如果玩家正在战斗过程中，则更新技能按钮状态【有可能更换装备后原来魔法不够，更换完成后魔法够用了】
			if (ExploreManager.Instance.battlePlayerCtr.isInFight) {
				ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons ();
			}

		}



		/// <summary>
		/// 在物品详细信息页点击了卸下按钮（装备）
		/// </summary>
		public void OnUnloadButtonClick(){

            // 如果选中的物品为空，直接返回
			if (currentSelectItem == null) {
				return;
			}

            // 检查背包是否已经满了，如果满了提示背包已满，并直接返回
			if (Player.mainPlayer.CheckBagFull (currentSelectItem)) {
				bagView.SetUpSingleTextTintHUD ("背包已满");
				return;
			}

            // 卸下装备，
			Equipment equipmentToUnload = currentSelectItem as Equipment;

			int equipmentIndexInPanel = Player.mainPlayer.GetEquipmentIndexInPanel (equipmentToUnload);
         
			PropertyChange propertyChange = Player.mainPlayer.UnloadEquipment (equipmentToUnload,equipmentIndexInPanel);

			// 更新装备面板
			bagView.SetUpEquipedEquipmentsPlane ();

            // 更新状态面板
			bagView.SetUpPlayerStatusPlane (propertyChange);

            // 背包中添加卸载的装备
			bagView.AddBagItem (currentSelectItem);

            // 清除物品详细信息面板
			bagView.ClearItemDetail ();

            // 如果玩家正在战斗中，则更新技能按钮的状态
			if (ExploreManager.Instance.battlePlayerCtr.isInFight) {
				ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons ();
			}
		}
			
        
		/// <summary>
		/// 在物品详细信息页点击了使用按钮
		/// </summary>
		public void OnUseButtonClick(){
              
			// 如果选中的物品为空，直接返回
			if (currentSelectItem == null) {
				return;
			}

            // 标记是否清除物品详细信息【如果物品使用完成后数量为0，从背包中移除了，则清除物品的详细信息】
			bool clearItemDetail = false;

            // 进行特殊操作物品【如点金石点的装备，重铸石重铸的装备等】
			Item specialOperaitonItem = null;

            // 标记是否从背包中移除
			bool totallyRemoved = true;

            // 根据当前选中物品的类型不同，区分不同的使用逻辑
			switch(currentSelectItem.itemType){
				// 消耗品使用逻辑
				case ItemType.Consumables:
					
					Consumables consumables = currentSelectItem as Consumables;

					PropertyChange propertyChange = consumables.UseConsumables(null);

					if(consumables.itemCount > 0){                  
						totallyRemoved = false;                  
					}


					bagView.SetUpPlayerStatusPlane(propertyChange);

					GameManager.Instance.soundManager.PlayAudioClip(consumables.audioName);

					break;
                // 技能卷轴的使用逻辑
				case ItemType.SkillScroll:

					SkillScroll skillScroll = currentSelectItem as SkillScroll;

                    // 检查技能是否已经学满了                
					if(Player.mainPlayer.CheckSkillFull()){
						string skillFullHint = string.Format("只能学习{0}个技能", Player.mainPlayer.maxSkillCount);
						bagView.SetUpSingleTextTintHUD(skillFullHint);
						return;
					}

                    // 检查技能是否已经学习过了
					bool skillHasLearned = Player.mainPlayer.CheckSkillHasLearned(skillScroll.skillId);

					if(skillHasLearned){
						bagView.SetUpSingleTextTintHUD("不能重复学习技能");
						return;
					}

					totallyRemoved = true;

					propertyChange = skillScroll.UseSkillScroll();

					GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);

                    // 由于有被动技能，学习后玩家属性上可能有变化，所以学习技能后也要更新属性面板
					bagView.SetUpPlayerStatusPlane(propertyChange);

					break;
                // 特殊物品的使用逻辑
				case ItemType.SpecialItem:

					SpecialItem specialItem = currentSelectItem as SpecialItem;

					Item itemForSpecialOperation = bagView.itemDetail.soCell.itemInCell;

					specialOperaitonItem = itemForSpecialOperation;

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

            // 如果玩家正在战斗中，更新技能按钮状态
			if (ExploreManager.Instance.battlePlayerCtr.isInFight)
            {
                ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();
            }

            // 从背包中移除当前选中的物品，如果该物品完全从背包中移除了，则清空物品详细信息面板
			clearItemDetail = Player.mainPlayer.RemoveItem(currentSelectItem, 1);
        
            // 更新当前背包
			bagView.UpdateCurrentBagItemsPlane();

            
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
            // 非特殊操作的物品，如果使用完之后还没有从背包中完全移除，则显示物品的选中框
			else if(!totallyRemoved){
				int itemIndexInBag = Player.mainPlayer.GetItemIndexInBag(currentSelectItem);
				if (itemIndexInBag >= 0)
                {
					int itemIndexInCurrentBag = itemIndexInBag % CommonData.singleBagItemVolume;
                    bagView.bagItemsDisplay.SetSelectionIcon(itemIndexInCurrentBag, true);
                }
			}
                     

		}
      
        /// <summary>
        /// 移除按钮点击响应
        /// </summary>
		public void OnRemoveButtonClick(){
			if (currentSelectItem == null) {
				return;
			}
			bagView.ShowRemoveQueryHUD ();

		}

        /// <summary>
        /// 确认移除按钮点击响应
        /// </summary>
		public void OnConfirmRemoveButtonClick(){
			
			if (currentSelectItem == null) {
				return;
			}

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.dropItemAudioName);

            // 确认移除将会把该物品完全移除出背包
			Player.mainPlayer.RemoveItem (currentSelectItem,currentSelectItem.itemCount);
         
            // 更新当前背包面板
			bagView.UpdateCurrentBagItemsPlane ();
            // 更新探索界面的底部bar
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
