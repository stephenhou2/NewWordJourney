using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SpecialOperationHUD : MonoBehaviour {

		public SpecialOperationCell equipmentCell;
		public SpecialOperationCell functionalItemCell;

		private CallBack refreshCurrentBagCallBack;
		private CallBack addSkillCallBack;

		public void InitSpecialOperationHUD(CallBack refreshCurrentBagCallBack,CallBack addSkillCallBack){
			this.refreshCurrentBagCallBack = refreshCurrentBagCallBack;
			this.addSkillCallBack = addSkillCallBack;
//			equipmentCell.InitSpecialOperationCell (refreshCurrentBagCallBack);
//			functionalItemCell.InitSpecialOperationCell (refreshCurrentBagCallBack);
		}

		public void OnAddSkillButtonClick(){

			if (Player.mainPlayer.totalGold < 200) {
				ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD ("金币不足");
				return;
			}

			Equipment equipment = equipmentCell.soDragControl.item as Equipment;

			if (equipment.attachedSkillId > 0) {
				ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD ("不能重复附魔");
				return;
			}

			SkillGemstone gemstone = functionalItemCell.soDragControl.item as SkillGemstone;

			if (equipment == null || gemstone == null) {
				return;
			}

			equipment.AddSkill (gemstone.skillId);

//			Player.mainPlayer.AddItem (equipment);

			Player.mainPlayer.RemoveItem (gemstone, 1);

			Player.mainPlayer.totalGold -= 200;

			equipmentCell.ResetSpecialOperationCell ();

			functionalItemCell.ResetSpecialOperationCell ();

			refreshCurrentBagCallBack ();

			addSkillCallBack ();

		}

		public void QuitSpecialOperationHUD(){

            equipmentCell.soDragControl.Reset();

            functionalItemCell.soDragControl.Reset();

			//gameObject.SetActive (false);

		}


	}
}
