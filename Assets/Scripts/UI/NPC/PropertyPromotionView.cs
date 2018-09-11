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

			PlayerPropertyChange(propertyPromotion.propertyType, propertyPromotion.promotion);

			string tint = propertyPromotion.GetPropertyPromotionTint();

			tintHUD.SetUpSingleTextTintHUD(tint);

			ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();

		}

		public void PlayerPropertyChange(PropertyType type, int change)
        {
			Player player = Player.mainPlayer;

            switch (type)
            {
                case PropertyType.MaxHealth:
					int maxHealthRecord = player.maxHealth;
					player.originalMaxHealth += change;
					player.maxHealth += change;
					player.health = (int)(player.health * (float)player.maxHealth / maxHealthRecord);
                    break;
                //case PropertyType.Health:
                //health += change;
                //break;
                case PropertyType.MaxMana:
					int maxManaRecord = player.maxMana;
					player.originalMaxMana += change;
					player.maxMana += change;
					player.mana = (int)(player.mana * (float)player.maxMana / maxManaRecord);
                    break;
                case PropertyType.Attack:
					player.originalAttack += change;
					player.attack += change;
                    break;
                case PropertyType.MagicAttack:
					player.originalMagicAttack += change;
					player.magicAttack += change;
                    break;
                case PropertyType.Armor:
					player.originalArmor += change;
					player.armor += change;
                    break;
                case PropertyType.MagicResist:
					player.originalMagicResist += change;
					player.magicResist += change;
                    break;
                case PropertyType.ArmorDecrease:
					player.originalArmorDecrease += change;
					player.armorDecrease += change;
                    break;
                case PropertyType.MagicResistDecrease:
					player.originalMagicResistDecrease += change;
					player.magicResistDecrease += change;
                    break;
                case PropertyType.MoveSpeed:
					player.originalMoveSpeed += change;
					player.moveSpeed += change;
                    break;
                case PropertyType.Dodge:
                    float changeInFloat = (float)change / 1000;
					player.originalDodge += changeInFloat;
					player.dodge += changeInFloat;
                    break;
                case PropertyType.Crit:
                    changeInFloat = (float)change / 1000;
					player.originalCrit += changeInFloat;
					player.crit += changeInFloat;
                    break;
                case PropertyType.CritHurtScaler:
                    changeInFloat = (float)change / 1000;
					player.originalCritHurtScaler += changeInFloat;
					player.critHurtScaler += changeInFloat;
                    break;
                case PropertyType.PhysicalHurtScaler:
                    changeInFloat = (float)change / 1000;
					player.originalPhysicalHurtScaler += changeInFloat;
					player.physicalHurtScaler += changeInFloat;
                    break;
                case PropertyType.MagicalHurtScaler:
                    changeInFloat = (float)change / 1000;
					player.originalMagicalHurtScaler += changeInFloat;
					player.magicalHurtScaler += changeInFloat;
                    break;
                case PropertyType.ExtraGold:
					player.originalExtraGold += change;
					player.extraGold += change;
                    break;
                case PropertyType.ExtraExperience:
					player.originalExtraExperience += change;
					player.extraExperience += change;
                    break;
                case PropertyType.HealthRecovery:
					player.originalHealthRecovery += change;
					player.healthRecovery += change;
                    break;
                case PropertyType.MagicRecovery:
					player.originalMagicRecovery += change;
					player.magicRecovery += change;
                    break;
            }
        }

        

		public void OnBackButtonClick(){

			if(quitCallBack != null){
				quitCallBack();
			}

			this.gameObject.SetActive(false);

		}
    }

}
