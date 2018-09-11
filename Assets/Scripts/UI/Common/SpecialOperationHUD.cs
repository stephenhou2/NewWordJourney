using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SpecialOperationHUD : MonoBehaviour {

		public SpecialOperationCell equipmentCell;
		public SpecialOperationCell functionalItemCell;

		public TintHUD tintHUD;

		private CallBackWithItem addGemstoneCallBack;

		public void InitSpecialOperationHUD(CallBackWithItem itemShortClickCallBack, CallBackWithItem addGemstoneCallBack){
			
			equipmentCell.InitSpecialOperaiton(itemShortClickCallBack);

			functionalItemCell.InitSpecialOperaiton(itemShortClickCallBack);

            this.addGemstoneCallBack = addGemstoneCallBack;
		}
        
		public bool SetUpHUDWhenAddItem(Item item){

			bool addSucceed = false;

			switch(item.itemType){
				case ItemType.Equipment:
					Equipment equipment = item as Equipment;

					if(equipment.attachedPropertyGemstones.Count == 2){
						tintHUD.SetUpSingleTextTintHUD("没有可用的宝石槽");
					}else{
						equipmentCell.SetUpSpeicalOperationCell(item);
						addSucceed = true;
					}               
					break;
				case ItemType.PropertyGemstone:
					functionalItemCell.SetUpSpeicalOperationCell(item);
					addSucceed = true;
					break;
			}

			return addSucceed;

		}

		public void SetUpHUDWhenRemoveItem(Item item){

			if(item == null){
				return;
			}

			switch(item.itemType){
				case ItemType.Equipment:
					equipmentCell.ResetSpecialOperationCell();
					break;
				case ItemType.PropertyGemstone:
					functionalItemCell.ResetSpecialOperationCell();
					break;

			}

		}

		public void OnAddGemstoneButtonClick(){

			if (Player.mainPlayer.totalGold < 50) {
				tintHUD.SetUpSingleTextTintHUD ("金币不足");
				return;
			}

			Equipment equipment = equipmentCell.soDragControl.item as Equipment;

			if(equipment == null || equipment.itemId < 0){
				return;
			}

			if (equipment.attachedPropertyGemstones.Count == 2) {
				tintHUD.SetUpSingleTextTintHUD ("没有可用的宝石槽");
				return;
			}

			PropertyGemstone gemstone = functionalItemCell.soDragControl.item as PropertyGemstone;

			if (equipment == null || gemstone == null || gemstone.itemId == -1) {
				return;
			}

			equipment.AddPropertyGemstone(gemstone);

			Player.mainPlayer.ResetBattleAgentProperties(false);
         
			Player.mainPlayer.RemoveItem (gemstone, 1);

			Player.mainPlayer.totalGold -= 50;
			ExploreManager.Instance.expUICtr.UpdatePlayerGold();

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.gemstoneAudioName);

			//equipmentCell.ResetSpecialOperationCell ();
           
			functionalItemCell.ResetSpecialOperationCell ();

			if(addGemstoneCallBack != null){
				addGemstoneCallBack(equipment);
			}


		}

		public void QuitSpecialOperationHUD(){

			equipmentCell.ResetSpecialOperationCell();

			functionalItemCell.ResetSpecialOperationCell();
            

		}


	}
}
