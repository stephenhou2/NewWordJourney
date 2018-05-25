using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class PropertyPromotionView : MonoBehaviour
    {

		public Text propertyPromotionText_1;
		public Text propertyPromotionText_2;

		public Text propertyPromotionPriceText_1;
		public Text propertyPromotionPriceText_2;


		private CallBack quitCallBack;
		public TintHUD tintHUD;

		private HLHNPC npc;

		public void SetUpPropertyPromotionView(HLHNPC npc,CallBack quitCallBack){

			this.npc = npc;
			this.quitCallBack = quitCallBack;

			HLHPropertyPromotion propertyPromotion_1 = npc.propertyPromotionList[0];
			HLHPropertyPromotion propertyPromotion_2 = npc.propertyPromotionList[1];

			propertyPromotionText_1.text = propertyPromotion_1.GetPropertyPromotionTint();
			propertyPromotionText_2.text = propertyPromotion_2.GetPropertyPromotionTint();

			propertyPromotionPriceText_1.text = propertyPromotion_1.promotionPrice.ToString();
			propertyPromotionPriceText_2.text = propertyPromotion_2.promotionPrice.ToString();

			this.gameObject.SetActive(true);

		}

		public void OnPropertyPromotionSelect(int index){

			HLHPropertyPromotion propertyPromotion = npc.propertyPromotionList[index];

			if(Player.mainPlayer.totalGold < propertyPromotion.promotionPrice){
				tintHUD.SetUpSingleTextTintHUD("金币不足");
				return;
			}

			Player.mainPlayer.totalGold -= propertyPromotion.promotionPrice;

			Player.mainPlayer.PlayerPropertyChange(propertyPromotion.propertyType, propertyPromotion.promotion);

			string tint = propertyPromotion.GetPropertyPromotionTint();

			tintHUD.SetUpSingleTextTintHUD(tint);

		}

        

		public void OnBackButtonClick(){

			if(quitCallBack != null){
				quitCallBack();
			}

			this.gameObject.SetActive(false);

		}
    }

}
