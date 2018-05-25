using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SpecialOperationHUD : MonoBehaviour {

		public SpecialOperationCell equipmentCell;
		public SpecialOperationCell functionalItemCell;


		private CallBackWithItem addSkillCallBack;

		public void SetUpHUDWhenAddItem(Item item,CallBackWithItem itemShortClickCallBack,CallBackWithItem addSkillCallBack){
			
			//if(item == null){
			//	return;
			//}

			//switch(item.itemType){
				//case ItemType.Equipment:
					equipmentCell.InitCell(item, itemShortClickCallBack);
					//break;
				//case ItemType.Gemstone:
					functionalItemCell.InitCell(item, itemShortClickCallBack);
					//break;
			//}
         
			this.addSkillCallBack = addSkillCallBack;
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

		public void OnAddSkillButtonClick(){

			if (Player.mainPlayer.totalGold < 120) {
				ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD ("金币不足");
				return;
			}

			Equipment equipment = equipmentCell.soDragControl.item as Equipment;

			if(equipment == null || equipment.itemId < 0){
				return;
			}

			if (equipment.attachedPropertyGemstone != null) {
				ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD ("该装备已经镶嵌过宝石");
				return;
			}

			PropertyGemstone gemstone = functionalItemCell.soDragControl.item as PropertyGemstone;

			if (equipment == null || gemstone == null) {
				return;
			}

			//equipment.AddSkill (gemstone.skillId);
           
			//if(equipment.equiped){
			//	Player.mainPlayer.AddSkill(equipment.attachedSkill);
			//	Player.mainPlayer.ResetBattleAgentProperties(false);
			//}

			Player.mainPlayer.RemoveItem (gemstone, 1);

			Player.mainPlayer.totalGold -= 120;

			//GameManager.Instance.soundManager.PlayAudioClip(CommonData.xiangQianJiNengAudioName);

			equipmentCell.ResetSpecialOperationCell ();
           
			functionalItemCell.ResetSpecialOperationCell ();

			if(addSkillCallBack != null){
				addSkillCallBack(equipment);
			}


		}

		public void QuitSpecialOperationHUD(){

            equipmentCell.soDragControl.Reset();

            functionalItemCell.soDragControl.Reset();

			//gameObject.SetActive (false);

		}


	}
}
