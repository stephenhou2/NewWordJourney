using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    /// <summary>
    /// 物品模型
    /// </summary>
	[System.Serializable]
	public class ItemModel {

		public string itemName;
		public string itemDescription;
		public string spriteName;
		public int itemId;

		public ItemType itemType;

		public int itemCount;
		public int price;

		public bool isNewItem = true;

	}



	/// <summary>
	/// 物品模型类
	/// </summary>
	[System.Serializable]
	public class EquipmentModel:ItemModel{
      
		public EquipmentType equipmentType;//装备类型

		public int maxHealthGain;//最大生命增益
		public int maxManaGain;//最大魔法增益

		public int attackGain;//攻击力增益
		public int magicAttackGain;//魔法攻击增益

		public int armorGain;//护甲增益
		public int magicResistGain;//抗性增益

		public int armorDecreaseGain;//护甲穿刺增益
		public int magicResistDecreaseGain;//抗性穿刺增益


		public AttackSpeed attackSpeed;//攻速
		public int moveSpeedGain;//地图行走速度增益

		public float critGain;//暴击增益
		public float dodgeGain;//闪避增益

		public float critHurtScalerGain;//暴击倍率加成
		public float physicalHurtScalerGain;//物理伤害加成
		public float magicalHurtScalerGain;//魔法伤害加成

		public int extraGoldGain;//额外金钱增益
		public int extraExperienceGain;//额外经验增益

		public int healthRecoveryGain;//生命回复效果增益
		public int magicRecoveryGain;//魔法回复效果增益

		public List<PropertySet> specProperties = new List<PropertySet>();//特殊属性集

		public int equipmentGrade;//装备评级

		public EquipmentDefaultQuality defaultQuality;//装备默认品质
		public EquipmentQuality quality;//装备品质（同一件装备细分为灰／蓝／金／紫 4级）

		//public int attachedSkillId;

		public WeaponType weaponType;

		//		public ItemInfoForProduce[] itemInfosForProduce;


		public override string ToString ()
		{
			return string.Format ("[EquipmentModel]:\n itemId:{0},itemName:{1},itemSpriteName:{2}itemDescription:{3}",
				itemId,itemName,spriteName,itemDescription);
		}

	}

    /// <summary>
    /// 消耗品模型类
    /// </summary>
	[System.Serializable]
	public class ConsumablesModel:ItemModel{

		public int healthGain;
		public int manaGain;
		public int experienceGain;

		public int maxHealthGain;
        public int maxManaGain;
        public int attackGain;
        public int magicAttackGain;
        public int armorGain;
        public int magicResistGain;
        public int armorDecreaseGain;
        public int magicResistDecreaseGain;
        public int moveSpeedGain;
        public float critGain;
        public float dodgeGain;
        public float critHurtScalerGain;
        public float physicalHurtScalerGain;
        public float magicalHurtScalerGain;
        public int extraGoldGain;
        public int extraExperienceGain;
        public int healthRecoveryGain;
        public int magicRecoveryGain;

		public int consumablesGrade;// 消耗品等级【不同等级的怪物在掉落消耗品时，根据这个查询掉落物品的范围】

		public bool isShowInBagOnly;

		public string audioName;

	}

    /// <summary>
    /// 属性宝石模型类
    /// </summary>
	[System.Serializable]
	public class PropertyGemstoneModel:ItemModel{


		public int maxHealthGainBase;
        public int maxManaGainBase;
        public int attackGainBase;
        public int magicAttackGainBase;
        public int armorGainBase;
        public int magicResistGainBase;
        public int armorDecreaseGainBase;
        public int magicResistDecreaseGainBase;
        public int moveSpeedGainBase;
		public int critGainBase;
		public int dodgeGainBase;
		public int critHurtScalerGainBase;
		public int physicalHurtScalerGainBase;
		public int magicalHurtScalerGainBase;
        public int extraGoldGainBase;
        public int extraExperienceGainBase;
        public int healthRecoveryGainBase;
        public int magicRecoveryGainBase;

		public int healthGain;
        public int manaGain;
        public int experienceGain;

        public int maxHealthGain;
        public int maxManaGain;
        public int attackGain;
        public int magicAttackGain;
        public int armorGain;
        public int magicResistGain;
        public int armorDecreaseGain;
        public int magicResistDecreaseGain;
        public int moveSpeedGain;
		public int critGain;
		public int dodgeGain;
		public int critHurtScalerGain;
		public int physicalHurtScalerGain;
		public int magicalHurtScalerGain;
        public int extraGoldGain;
        public int extraExperienceGain;
        public int healthRecoveryGain;
        public int magicRecoveryGain;

		public GemstoneGrade grade;
	}

    /// <summary>
    /// 技能卷轴模型类
    /// </summary>
	[System.Serializable]
	public class SkillScrollModel:ItemModel{

		public int skillId;

	}

    /// <summary>
    /// 特殊物品模型类
    /// </summary>
	[System.Serializable]
	public class SpecialItemModel:ItemModel{

		public SpecialItemType specialItemType;

		public bool isShowInBagOnly;

	}

    /// <summary>
    /// 拼写物品模型类
    /// </summary>
	[System.Serializable]
	public class SpellItemModel:ItemModel{
		
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

        // 是否已经使用过
		public static bool CheckHasUsed(string spell){

			return Player.mainPlayer.spellRecord.Contains(spell);

		}

        /// <summary>
        /// 获取一个未使用过的拼写物品模型
        /// </summary>
        /// <returns>The AU nused spell item model.</returns>
		public static SpellItemModel GetAUnusedSpellItemModel(){
         
            List<SpellItemModel> allUnusedSpellItemModels = new List<SpellItemModel>();

            List<SpellItemModel> allSpellItemModels = GameManager.Instance.gameDataCenter.allSpellItemModels;

            for (int i = 0; i < allSpellItemModels.Count; i++)
            {

				if (!CheckHasUsed(allSpellItemModels[i].spell))
                {
                    allUnusedSpellItemModels.Add(allSpellItemModels[i]);
                }
            }

            // 如果所有拼写物品全都已经使用过了，就清空拼写记录，再重新随机拼写物品
			if(allUnusedSpellItemModels.Count == 0){
				Player.mainPlayer.spellRecord.Clear();
				return GetAUnusedSpellItemModel();
			}

            int randomSeed = Random.Range(0, allUnusedSpellItemModels.Count);
         
			return allUnusedSpellItemModels[randomSeed];         
		}

	}

}
