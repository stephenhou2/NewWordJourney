using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    // 拼写物品类型
	public enum SpellItemType{
        Equipment,
        ProeprtyGemstone,
        Consumables,
        SkillScroll,
        SpecialItem    
	}

	public class SpellItem : Item
    {

		public SpellItemType spellItemType;
              
        // 物品的拼写
		public string spell;

		// 拼写的音标
		public string phoneticSymbol;

		// 单词发音URL
		public string pronounciationURL;

		// 如果是装备，代表装备类【0:武器  1:头盔  2:护甲  3:手套  4:鞋子  5:戒指】
        // 其余物品对应物品id
        public int attachInfo_1;

        // 仅在拼写物品生成装备时，代表武器类型【0:剑 1:匕首 2:法杖 3:斧子】
        public int attachInfo_2;

      
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="spellItemModel">Spell item model.</param>
        /// <param name="itemCount">Item count.</param>
		public SpellItem(SpellItemModel spellItemModel,int itemCount){

			InitBaseProperties(spellItemModel);

			this.itemType = ItemType.SpellItem;

			this.spellItemType = spellItemModel.spellItemType;

			this.attachInfo_1 = spellItemModel.attachInfo_1;
            
			this.attachInfo_2 = spellItemModel.attachInfo_2;

			this.spell = spellItemModel.spell;

			this.phoneticSymbol = spellItemModel.phoneticSymbol;

			this.pronounciationURL = spellItemModel.pronounciationURL;

			this.itemCount = itemCount;
                   
		}

        /// <summary>
        /// Generates the item.
        /// </summary>
        /// <returns>The item.</returns>
		public Item GenerateItem()
        {
			Item generatedItem = null;

            // 如果拼写物品是装备
			if (spellItemType == SpellItemType.Equipment)
            {
                // 获取装备类型
                EquipmentType equipmentType = (EquipmentType)attachInfo_1;

                // 根据玩家探索到的关卡确定装备等级
                int equipmentGrade = (Player.mainPlayer.maxUnlockLevelIndex - 1) / 5 + 1;

                // 没有10级的装备，最后开出来的都是9级装备
				if(equipmentGrade == 10){
					equipmentGrade = 9;
				}

                EquipmentModel equipmentModel = GameManager.Instance.gameDataCenter.allEquipmentModels.Find(delegate (EquipmentModel obj)
                {
                    if (equipmentType == EquipmentType.Weapon)
                    {
                        return obj.equipmentGrade == equipmentGrade
                                  && obj.equipmentType == equipmentType
                                  && obj.weaponType == (WeaponType)attachInfo_2;
                    }
                    else
                    {
                        return obj.equipmentGrade == equipmentGrade
                                  && obj.equipmentType == equipmentType;
                    }

                });

                generatedItem = Item.NewItemWith(equipmentModel.itemId, 1);

				(generatedItem as Equipment).SetToGoldQuality();
            }
            else
            {
				generatedItem = Item.NewItemWith(attachInfo_1,1);
            }


			return generatedItem;
        }
	}
   
}
