using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	[System.Serializable]
	public class ItemModel {

		public string itemName;
		public string itemDescription;
//		public string itemPropertyDescription;
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

	//		[System.Serializable]
	//		public struct ItemInfoForProduce
	//		{
	//			public int itemId;
	//			public int itemCount;
	//
	//			public ItemInfoForProduce(int itemId,int itemCount){
	//				this.itemId = itemId;
	//				this.itemCount = itemCount;
	//			}
	//		}

		public EquipmentType equipmentType;//装备类型

		public int maxHealthGain;//最大生命增益
		public int maxManaGain;//最大魔法增益

		public int attackGain;//攻击力增益
		public int magicAttackGain;//魔法攻击增益

		public int armorGain;//护甲增益
		public int magicResistGain;//魔抗增益

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

		public int quality;//装备品质（同一件装备细分为灰／蓝／金,暗金4级）

		public int attachedSkillId;

		//		public ItemInfoForProduce[] itemInfosForProduce;


		public override string ToString ()
		{
			return string.Format ("[EquipmentModel]:\n itemId:{0},itemName:{1},itemSpriteName:{2}itemDescription:{3}",
				itemId,itemName,spriteName,itemDescription);
		}

	}


	[System.Serializable]
	public class ConsumablesModel:ItemModel{

		public int healthGain;
		public int manaGain;
		public int experienceGain;

		public ConsumablesType type;
		public bool isShowInBagOnly;

	}

	[System.Serializable]
	public class SkillGemstoneModel:ItemModel{

		public int skillId;

		public SkillType skillType;

		public int manaConsume;

		public float coolenTime;

		public string skillName;

		public string skillIconName;

		public string skillDescription;

	}


}
