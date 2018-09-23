using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using System.Text;

	public enum EquipmentType{
		Weapon,
		Helmet,
		Armor,
		Glove,
		Shoes,
		Ring
	}

	public enum EquipmentDefaultQuality{
		Random,
        Gold,
        Purple,
        Gray
	}

	public enum EquipmentQuality{
		Gray,
        Blue,
		Gold,
        Purple
	}

	public enum WeaponType{
		None,
		Sword,
		Staff,
		Axe,
		Dragger
	}


	[System.Serializable]
	public class Equipment : Item {

		public string attachedPropertyDescription;

		public int oriMaxHealthGain;//最大生命增益
		public int oriMaxManaGain;//最大魔法增益

		public int oriAttackGain;//攻击力增益
		public int oriMagicAttackGain;//魔法攻击增益

		public int oriArmorGain;//护甲增益
		public int oriMagicResistGain;//抗性增益

		public int oriArmorDecreaseGain;//护甲穿刺增益
		public int oriMagicResistDecreaseGain;//抗性穿刺增益

		public int oriMoveSpeedGain;//地图行走速度增益

		public float oriCritGain;//暴击增益
		public float oriDodgeGain;//闪避增益

		public float oriCritHurtScalerGain;//暴击倍率加成
		public float oriPhysicalHurtScalerGain;//物理伤害加成
		public float oriMagicalHurtScalerGain;//魔法伤害加成

		public int oriExtraGoldGain;//额外金钱增益
		public int oriExtraExperienceGain;//额外经验增益

		public int oriHealthRecoveryGain;//生命回复效果增益
		public int oriMagicRecoveryGain;//魔法回复效果增益

		public int oriPrice; //原始价格 

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

		public int equipmentGrade;//装备评级
		public EquipmentDefaultQuality defaultQuality;//装备默认品质
        public EquipmentQuality quality;//装备品质（同一件装备细分为灰／蓝／金／紫 4级）

		//public int attachedPropertyGemstoneId;// 镶嵌的属性宝石的id

		public List<PropertySet> specProperties;//装备的特殊属性（蓝色/金色/暗金装备会增加1/2/2个特殊属性）

		public WeaponType weaponType;

		public List<PropertyGemstone> attachedPropertyGemstones;// 镶嵌的属性宝石

//		public EquipmentModel.ItemInfoForProduce[] itemInfosForProduce;


		//装备是否已佩戴
		public bool equiped;

		public EquipmentType equipmentType;

		/// <summary>
		/// 空构造函数，初始化一个占位用的装备
		/// </summary>
		public Equipment(){
			itemId = -1;
			weaponType = WeaponType.None;
		}
			


		/// <summary>
		/// 构造函数
		/// </summary>
		public Equipment(EquipmentModel equipmentModel,int itemCount){

			this.itemType = ItemType.Equipment;
			this.itemCount = itemCount;

			// 初始化物品基本属性
			InitBaseProperties (equipmentModel);

			// 初始化装备属性(默认初始化为灰色装备)
			this.oriMaxHealthGain = equipmentModel.maxHealthGain;
			this.oriMaxManaGain = equipmentModel.maxManaGain;

			this.oriAttackGain = equipmentModel.attackGain;
			this.oriMagicAttackGain = equipmentModel.magicAttackGain;

			this.oriArmorGain = equipmentModel.armorGain;
			this.oriMagicResistGain = equipmentModel.magicResistGain;
			this.oriArmorDecreaseGain = equipmentModel.armorDecreaseGain;
			this.oriMagicResistDecreaseGain = equipmentModel.magicResistDecreaseGain;

			this.attackSpeed = (AttackSpeed)equipmentModel.attackSpeed;
			this.oriMoveSpeedGain = equipmentModel.moveSpeedGain;

			this.oriCritGain= equipmentModel.critGain;
			this.oriDodgeGain = equipmentModel.dodgeGain;

			this.oriCritHurtScalerGain = equipmentModel.critHurtScalerGain;
			this.oriPhysicalHurtScalerGain = equipmentModel.physicalHurtScalerGain;
			this.oriMagicalHurtScalerGain = equipmentModel.magicalHurtScalerGain;

			this.oriExtraGoldGain = equipmentModel.extraGoldGain;
			this.oriExtraExperienceGain = equipmentModel.extraExperienceGain;

			this.oriHealthRecoveryGain = equipmentModel.healthRecoveryGain;
			this.oriMagicRecoveryGain = equipmentModel.magicRecoveryGain;

			this.oriPrice = equipmentModel.price;
			this.price = equipmentModel.price;
			this.defaultQuality = equipmentModel.defaultQuality;

			this.equipmentType = (EquipmentType)(equipmentModel.equipmentType);
			this.equipmentGrade = equipmentModel.equipmentGrade;

			this.specProperties = equipmentModel.specProperties;

			switch(defaultQuality){
				case EquipmentDefaultQuality.Random:
					RebuildEquipment();
					break;
				case EquipmentDefaultQuality.Gold:
					quality = EquipmentQuality.Gold;
					ResetPropertiesByQuality(quality);
					break;
				case EquipmentDefaultQuality.Purple:
					quality = EquipmentQuality.Purple;
					ResetPropertiesByQuality(quality);
					break;
				case EquipmentDefaultQuality.Gray:
					quality = EquipmentQuality.Gray;
					ResetPropertiesByQuality(quality);
					break;
			}
            

			this.weaponType = equipmentModel.weaponType;

			attachedPropertyGemstones = new List<PropertyGemstone>();

			InitDescription ();
		}


		/// <summary>
		/// 装备点金
		/// </summary>
		public void SetToGoldQuality(){
			if(quality == EquipmentQuality.Purple){
				ResetPropertiesByQuality(EquipmentQuality.Purple);
			}else{
				ResetPropertiesByQuality(EquipmentQuality.Gold);
			}

		}


		/// <summary>
		/// 重铸装备【50%概率灰色，30%概率蓝色，20%概率金色】
		/// </summary>
		public void RebuildEquipment(){

			int randomSeed = Random.Range (0, 100);

			// 非暗金装备重铸时装备品质重新随机，暗金装备重铸后仍是暗金装备
			if (quality != EquipmentQuality.Purple && quality != EquipmentQuality.Gold) {
				if (randomSeed < 65) {
					quality = EquipmentQuality.Gray;
				} else if (randomSeed < 95) {
					quality = EquipmentQuality.Blue;
				} else if (randomSeed < 100) {
					quality = EquipmentQuality.Gold;
				}
			}

			ResetPropertiesByQuality (quality);
		}

		public void AddPropertyGemstone(PropertyGemstone propertyGemstone){
         
			propertyGemstone.GemStonePropertyConfigure();

			attachedPropertyGemstones.Add(propertyGemstone);

			//Debug.Log(attachedPropertyGemstone.itemCount);
		}

        /// <summary>
        /// 移除装备上镶嵌的所有宝石
        /// </summary>
        /// <returns>The property gemstons.</returns>
		public PropertyGemstone[] RemovePropertyGemstons(){

			PropertyGemstone[] propertyGemstones = new PropertyGemstone[attachedPropertyGemstones.Count];

			for (int i = 0; i < propertyGemstones.Length;i++){
				PropertyGemstone propertyGemstone = attachedPropertyGemstones[i];
				propertyGemstone.itemCount = 1;
				propertyGemstones[i] = propertyGemstone;
			}
         
			attachedPropertyGemstones.Clear();

			return propertyGemstones;

		}
        

		/// <summary>
		/// 根据装备品质初始化装备
		/// </summary>
		/// <param name="quality">Quality.</param>
		public void ResetPropertiesByQuality(EquipmentQuality quality){

			this.quality = quality;

			ResetEquipmentToEmptyProperty ();

			this.maxHealthGain = oriMaxHealthGain;
			this.maxManaGain = oriMaxManaGain;

			this.attackGain = oriAttackGain;
			this.magicAttackGain = oriMagicAttackGain;

			this.armorGain = oriArmorGain;
			this.magicResistGain = oriMagicResistGain;
			this.armorDecreaseGain = oriArmorDecreaseGain;
			this.magicResistDecreaseGain = oriMagicResistDecreaseGain;

			this.moveSpeedGain = oriMoveSpeedGain;

			this.critGain = oriCritGain;
			this.dodgeGain = oriDodgeGain;

			this.critHurtScalerGain = oriCritHurtScalerGain;
			this.physicalHurtScalerGain = oriPhysicalHurtScalerGain;
			this.magicalHurtScalerGain = oriMagicalHurtScalerGain;

			this.extraGoldGain = oriExtraGoldGain;
			this.extraExperienceGain = oriExtraExperienceGain;

			this.healthRecoveryGain = oriHealthRecoveryGain;
			this.magicRecoveryGain = oriMagicRecoveryGain;


			float gainScaler = 1;

			switch (quality){            
    			case EquipmentQuality.Gray:
    				gainScaler = Random.Range (1.0f, 1.2f);

					this.price = oriPrice;

    				break;
    			case EquipmentQuality.Blue:
    				gainScaler = Random.Range (1.2f, 1.3f);

					this.price = oriPrice * 2;
                                   
    				break;
    			case EquipmentQuality.Gold:

                    // 特殊金色装备属性不浮动，普通金色装备属性浮动30%-40%
					if(equipmentGrade >= 10){
						gainScaler = 1.0f;
						this.price = oriPrice;
					}else{
						gainScaler = Random.Range(1.3f, 1.4f);
						this.price = oriPrice * 3;
					}
    				break;
    			case EquipmentQuality.Purple:

    				gainScaler = 1.0f;
    				
					this.price = oriPrice;
    				break;
			}

			this.maxHealthGain = (int)(maxHealthGain * gainScaler);
			this.maxManaGain = (int)(maxManaGain * gainScaler);

			this.attackGain = (int)(attackGain * gainScaler);
			this.magicAttackGain = (int)(magicAttackGain * gainScaler);

			this.armorGain = (int)(armorGain * gainScaler);
			this.magicResistGain = (int)(magicResistGain * gainScaler);
			this.armorDecreaseGain = (int)(armorDecreaseGain * gainScaler);
			this.magicResistDecreaseGain = (int)(magicResistDecreaseGain * gainScaler);

			this.moveSpeedGain = (int)(moveSpeedGain * gainScaler);

			this.critGain = critGain * gainScaler;
			this.dodgeGain = dodgeGain * gainScaler;

			this.critHurtScalerGain = critHurtScalerGain * gainScaler;
			this.physicalHurtScalerGain = physicalHurtScalerGain * gainScaler;
			this.magicalHurtScalerGain = magicalHurtScalerGain * gainScaler;

			this.extraGoldGain = (int)(extraGoldGain * gainScaler);
			this.extraExperienceGain = (int)(extraExperienceGain * gainScaler);

			this.healthRecoveryGain = (int)(healthRecoveryGain * gainScaler);
			this.magicRecoveryGain = (int)(magicRecoveryGain * gainScaler);


            switch(quality){
                case EquipmentQuality.Gray:
                    break;
                case EquipmentQuality.Blue:

                    int randomSeed = Random.Range(0, specProperties.Count);

                    PropertySet ps = specProperties[randomSeed];

                    InitSpecificProperty(ps);

                    break;
                case EquipmentQuality.Gold:
				case EquipmentQuality.Purple:
                    for (int i = 0; i < specProperties.Count; i++)
                    {

                        ps = specProperties[i];

                        InitSpecificProperty(ps);
                    }

                    break;

            }

			//if (attachedSkillId > 0 && equiped) {
			//	attachedSkill = SkillGenerator.GenerateSkill (attachedSkillId);
			//}

			//PropertyChange propertyChange = new PropertyChange();

			//if (equiped) {
			//	propertyChange = Player.mainPlayer.ResetBattleAgentProperties (false);
			//}

			InitDescription ();

			//return propertyChange;

		}

		/// <summary>
		/// 重置装备属性为空属性
		/// </summary>
		private void ResetEquipmentToEmptyProperty(){

			this.maxHealthGain = 0;
			this.maxManaGain = 0;

			this.attackGain = 0;
			this.magicAttackGain = 0;

			this.armorGain = 0;
			this.magicResistGain = 0;
			this.armorDecreaseGain = 0;
			this.magicResistDecreaseGain = 0;

			this.moveSpeedGain = 0;

			this.critGain = 0;
			this.dodgeGain = 0;

			this.critHurtScalerGain = 1.5f;
			this.physicalHurtScalerGain = 1f;
			this.magicalHurtScalerGain = 1f;

			this.extraGoldGain = 0;
			this.extraExperienceGain = 0;

			this.healthRecoveryGain = 0;
			this.magicRecoveryGain = 0;

			//RemoveAttachedSkill ();

		}



		private void InitDescription(){

			StringBuilder sb = new StringBuilder ();
			StringBuilder attachedSb = new StringBuilder ();

			if (oriAttackGain > 0) {
				sb.AppendFormat ("攻击 +{0}\n", attackGain);
			} else if(oriAttackGain == 0 && attackGain > 0){
				attachedSb.AppendFormat ("攻击 +{0}\n", attackGain);
			}

			if (equipmentType == EquipmentType.Weapon) {

				switch (attackSpeed) {
				case AttackSpeed.Slow:
					sb.Append ("攻速：慢速\n");
					break;
				case AttackSpeed.Medium:
					sb.Append ("攻速：中速\n");
					break;
				case AttackSpeed.Fast:
					sb.Append ("攻速：快速\n");
					break;
				case AttackSpeed.VeryFast:
					sb.Append ("攻速：极快\n");
					break;
				}
			}

			if (oriMaxHealthGain > 0) {
				sb.AppendFormat ("生命 +{0}\n", maxHealthGain);
			} else if (oriMaxHealthGain == 0 && maxHealthGain > 0) {
				attachedSb.AppendFormat ("生命 +{0}\n", maxHealthGain);
			}

			if (oriMaxManaGain > 0) {
				sb.AppendFormat ("魔法 +{0}\n", maxManaGain);
			} else if (oriMaxManaGain == 0 && maxManaGain > 0) {
				attachedSb.AppendFormat ("魔法 +{0}\n", maxManaGain);
			}

			if (oriMagicAttackGain > 0) {
				sb.AppendFormat ("魔法攻击 +{0}\n", magicAttackGain);
			} else if (oriMagicAttackGain == 0 && magicAttackGain > 0) {
				attachedSb.AppendFormat ("魔法攻击 +{0}\n", magicAttackGain);
			}

			if (oriArmorGain > 0) {
				sb.AppendFormat ("护甲 +{0}\n", armorGain);
			} else if (oriArmorGain == 0 && armorGain > 0) {
				attachedSb.AppendFormat ("护甲 +{0}\n", armorGain);
			}

			if (oriMagicResistGain > 0) {
				sb.AppendFormat ("抗性 +{0}\n", magicResistGain);
			} else if (oriMagicResistGain == 0 && magicResistGain > 0) {
				attachedSb.AppendFormat ("抗性 +{0}\n", magicResistGain);
			}

			if (oriArmorDecreaseGain > 0) {
				sb.AppendFormat ("护甲穿透 +{0}\n", armorDecreaseGain);
			} else if (oriArmorDecreaseGain == 0 && armorDecreaseGain > 0) {
				attachedSb.AppendFormat ("护甲穿透 +{0}\n", armorDecreaseGain);
			}

			if (oriMagicResistDecreaseGain > 0) {
				sb.AppendFormat ("抗性穿透 +{0}\n", magicResistDecreaseGain);
			} else if (oriMagicResistDecreaseGain == 0 && magicResistDecreaseGain > 0) {
				attachedSb.AppendFormat ("抗性穿透 +{0}\n", magicResistDecreaseGain);
			}
				
			if (oriMoveSpeedGain > 0) {
				sb.AppendFormat ("移动速度 +{0}\n", moveSpeedGain);
			} else if (oriMoveSpeedGain == 0 && moveSpeedGain > 0) {
				attachedSb.AppendFormat ("移动速度 +{0}\n", moveSpeedGain);
			}

			if (oriCritGain > 0) {
				sb.AppendFormat ("暴击 +{0}%\n", (critGain * 100).ToString("F1"));
			} else if (System.Math.Abs(oriCritGain) < float.Epsilon && critGain > 0) {
				attachedSb.AppendFormat ("暴击 +{0}%\n", (critGain * 100).ToString("F1"));
			}

			if (oriDodgeGain > 0) {
				sb.AppendFormat ("闪避 +{0}%\n", (dodgeGain * 100).ToString("F1"));
			} else if (System.Math.Abs(oriDodgeGain) < float.Epsilon && dodgeGain > 0) {
				attachedSb.AppendFormat ("闪避 +{0}%\n", (dodgeGain * 100).ToString("F1"));
			}

			if (oriCritHurtScalerGain > 0) {
				sb.AppendFormat ("暴击伤害 +{0}%\n", (int)(critHurtScalerGain * 100));
			} else if (System.Math.Abs(oriCritHurtScalerGain) < float.Epsilon && critHurtScalerGain > 0) {
				attachedSb.AppendFormat("暴击伤害 +{0}%\n", (int)(critHurtScalerGain * 100));
			}

			if (oriExtraGoldGain > 0) {
				sb.AppendFormat ("额外金钱 +{0}\n", extraGoldGain);
			} else if (oriExtraGoldGain == 0 && extraGoldGain > 0) {
				attachedSb.AppendFormat ("额外金钱 +{0}\n", extraGoldGain);
			}

		
			if (oriExtraExperienceGain > 0) {
				sb.AppendFormat ("额外经验 +{0}\n", extraExperienceGain);
			} else if (oriExtraExperienceGain == 0 && extraExperienceGain > 0) {
				attachedSb.AppendFormat ("额外经验 +{0}\n", extraExperienceGain);
			}

			if (oriHealthRecoveryGain > 0) {
				sb.AppendFormat ("生命回复 +{0}\n", healthRecoveryGain);
			} else if (oriHealthRecoveryGain == 0 && healthRecoveryGain > 0) {
				attachedSb.AppendFormat ("生命回复 +{0}\n", healthRecoveryGain);
			}

			if (oriMagicRecoveryGain > 0) {
				sb.AppendFormat ("魔法回复 +{0}\n", magicRecoveryGain);
			} else if (oriMagicRecoveryGain == 0 && magicRecoveryGain > 0) {
				attachedSb.AppendFormat ("魔法回复 +{0}\n", magicRecoveryGain);
			}
            

			if (sb.Length > 0) {
				sb.Remove (sb.Length - 1, 1);
			}
			if (attachedSb.Length > 0) {
				attachedSb.Remove (attachedSb.Length - 1, 1);
			}

			itemDescription = sb.ToString();
			attachedPropertyDescription = attachedSb.ToString ();

		}


		private void InitSpecificProperty(PropertySet ps){

			switch (ps.type) {
			case PropertyType.MaxHealth:
				this.maxHealthGain += (int)ps.value;
				break;
			case PropertyType.MaxMana:
				this.maxManaGain += (int)ps.value;
				break;
			case PropertyType.Attack:
				this.attackGain += (int)ps.value;
				break;
			case PropertyType.MagicAttack:
				this.magicAttackGain += (int)ps.value;
				break;
			case PropertyType.MoveSpeed:
				this.moveSpeedGain += (int)ps.value;
				break;
			case PropertyType.Armor:
				this.armorGain += (int)ps.value;
				break;
			case PropertyType.MagicResist:
				this.magicResistGain += (int)ps.value;
				break;
			case PropertyType.ArmorDecrease:
				this.armorDecreaseGain += (int)ps.value;
				break;
			case PropertyType.MagicResistDecrease:
				this.magicResistDecreaseGain += (int)ps.value;
				break;
			case PropertyType.Crit:
				this.critGain += ps.value;
				break;
			case PropertyType.Dodge:
				this.dodgeGain += ps.value;
				break;
			case PropertyType.CritHurtScaler:
				this.critHurtScalerGain += ps.value;
				break;
			case PropertyType.ExtraGold:
				this.extraGoldGain += (int)ps.value;
				break;
			case PropertyType.ExtraExperience:
				this.extraExperienceGain += (int)ps.value;
				break;
			case PropertyType.HealthRecovery:
				this.healthRecoveryGain += (int)ps.value;
				break;
			case PropertyType.MagicRecovery:
				this.magicRecoveryGain += (int)ps.value;
				break;
			}

		}      

      
	}
		





}
