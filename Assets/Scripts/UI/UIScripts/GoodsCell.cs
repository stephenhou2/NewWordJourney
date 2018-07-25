using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class GoodsCell : MonoBehaviour {

		public Image goodsIcon;
		public Image selectedIcon;
		public Image goodsBackground;
		public Text goodsName;
		public Text goodsPrice;

		public Sprite grayFrame;
		public Sprite blueFrame;
		public Sprite goldFrame;
		public Sprite purpleFrame;


		public void SetUpGoodsCell(HLHNPCGoods goods){

			Item itemAsGoods = goods.GetGoodsItem();

			goodsName.text = itemAsGoods.itemName;

			goodsName.color = CommonData.grayEquipmentColor;
			goodsBackground.sprite = grayFrame;

			if(itemAsGoods is Equipment){
				
				Equipment equipment = itemAsGoods as Equipment;

				switch(equipment.quality){
					case EquipmentQuality.Gray:
						goodsName.color = CommonData.grayEquipmentColor;
                        goodsBackground.sprite = grayFrame;
						break;
					case EquipmentQuality.Blue:
						goodsName.color = CommonData.blueEquipmentColor;
						goodsBackground.sprite = blueFrame;
						break;
					case EquipmentQuality.Gold:
						goodsName.color = CommonData.goldEquipmentColor;
						goodsBackground.sprite = goldFrame;
						break;
					case EquipmentQuality.Purple:
						goodsName.color = CommonData.purpleEquipmentColor;
						goodsBackground.sprite = purpleFrame;
						break;
				}
					
			}
            

			int price = goods.GetGoodsPrice();

			goodsPrice.text = price.ToString ();
         
			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (itemAsGoods);

			goodsIcon.sprite = itemSprite;

			goodsIcon.enabled = itemSprite != null;

			selectedIcon.enabled = false;

		}


		public void SetSelection(bool selected){
			selectedIcon.enabled = selected;
		}


	}
}
