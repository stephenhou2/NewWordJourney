using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class GoodsCell : MonoBehaviour {

		public Image goodsIcon;
		public Image selectedIcon;
		public Text goodsPrice;


		public void SetUpGoodsCell(Item goods){

			goodsPrice.text = goods.price.ToString ();

//			Item itemAsGoods = goods.GetGoodsItem ();

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (goods);

			goodsIcon.sprite = itemSprite;

			goodsIcon.enabled = itemSprite != null;

			selectedIcon.enabled = false;

		}


		public void SetSelection(bool selected){
			selectedIcon.enabled = selected;
		}


	}
}
