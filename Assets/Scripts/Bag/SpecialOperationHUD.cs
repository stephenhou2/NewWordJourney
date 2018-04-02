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
				return;
			}

			Equipment equipment = equipmentCell.soDragControl.item as Equipment;

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

			bool needRefreshBag = false;

			Item equipment = equipmentCell.soDragControl.item;

			if (equipment != null) {
				Player.mainPlayer.AddItem (equipment);
				needRefreshBag = true;
			}

			Item functionalItem = functionalItemCell.soDragControl.item;

			if (functionalItem != null) {
				Player.mainPlayer.AddItem (functionalItem);
				needRefreshBag = true;
			}

			if (needRefreshBag) {
				refreshCurrentBagCallBack ();
			}

			gameObject.SetActive (false);

		}


	}
}
