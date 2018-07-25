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

		public Image propertyPromotionIcon_1;
		public Image propertyPromotionIcon_2;

		private CallBack quitCallBack;
		public TintHUD tintHUD;

		private HLHNPC npc;

		public Sprite attackSprite;
		public Sprite magicAttackSprite;
		public Sprite armorSprite;
		public Sprite magicResistSprite;
		public Sprite dodgeSprite;
		public Sprite critSprite;
		public Sprite armorDecreaseSprite;
		public Sprite magicResistDecreaseSprite;
		public Sprite extraGoldSprite;
		public Sprite extraExperienceSprite;
		public Sprite healthRecoverySprite;
		public Sprite magicRecoverySprite;

		public void SetUpPropertyPromotionView(HLHNPC npc,CallBack quitCallBack){

			this.npc = npc;
			this.quitCallBack = quitCallBack;

			HLHPropertyPromotion propertyPromotion_1 = npc.propertyPromotionList[0];
			HLHPropertyPromotion propertyPromotion_2 = npc.propertyPromotionList[1];

			propertyPromotionText_1.text = propertyPromotion_1.GetPropertyPromotionTint();
			propertyPromotionText_2.text = propertyPromotion_2.GetPropertyPromotionTint();

			propertyPromotionPriceText_1.text = propertyPromotion_1.promotionPrice.ToString();
			propertyPromotionPriceText_2.text = propertyPromotion_2.promotionPrice.ToString();

			propertyPromotionIcon_1.sprite = GetPropertySprite(propertyPromotion_1.propertyType);         
			propertyPromotionIcon_2.sprite = GetPropertySprite(propertyPromotion_2.propertyType);

			this.gameObject.SetActive(true);

		}

		public Sprite GetPropertySprite(PropertyType propertyType){
			Sprite sprite = null;
			switch(propertyType){
                case PropertyType.Attack:
					sprite = attackSprite;
                    break;
                case PropertyType.MagicAttack:
					sprite = magicAttackSprite;
                    break;
                case PropertyType.Armor:
					sprite = armorSprite;
                    break;
                case PropertyType.MagicResist:
					sprite = magicResistSprite;
                    break;
                case PropertyType.Crit:
					sprite = critSprite;
                    break;
                case PropertyType.Dodge:
					sprite = dodgeSprite;
                    break;
				case PropertyType.ArmorDecrease:
					sprite = armorDecreaseSprite;
					break;
				case PropertyType.MagicResistDecrease:
					sprite = magicResistDecreaseSprite;
					break;
				case PropertyType.ExtraGold:
					sprite = extraGoldSprite;
					break;
				case PropertyType.ExtraExperience:
					sprite = extraExperienceSprite;
					break;
				case PropertyType.HealthRecovery:
					sprite = healthRecoverySprite;
					break;
				case PropertyType.MagicRecovery:
					sprite = magicRecoverySprite;
					break;
				default:
					break;
            
			}
			return sprite;
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

			ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();

		}

        

		public void OnBackButtonClick(){

			if(quitCallBack != null){
				quitCallBack();
			}

			this.gameObject.SetActive(false);

		}
    }

}
