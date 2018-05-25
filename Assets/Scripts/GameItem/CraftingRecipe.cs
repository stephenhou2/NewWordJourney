using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	[System.Serializable]
	public class CraftingRecipe : Item {

		public int craftItemId;


		public CraftingRecipe(int craftItemId){

//			EquipmentModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate (EquipmentModel obj) {
//				return obj.itemId == craftItemId;
//			});

//			InitWithItemModel (itemModel);
		}


		public CraftingRecipe(EquipmentModel itemModel){
//			InitWithItemModel (itemModel);
		}

		private void InitWithItemModel(EquipmentModel itemModel){

//			this.itemType = ItemType.CraftingRecipes;
			this.craftItemId = itemModel.itemId;

			this.spriteName = "craftingRecipes";

			this.itemName = itemModel.itemName;

			this.itemId = itemModel.itemId + 400;

			this.itemDescription = itemModel.itemDescription;
//			this.itemPropertyDescription = itemModel.itemPropertyDescription;

			this.itemCount = 1;

		}

		//public override string GetItemTypeString ()
		//{
		//	return "合成配方";
		//}


	}
}
