using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEditor;
	using System.Text;
	using System.IO;
	using System;

	public class EquipmentDataHandler {

		[MenuItem("EditHelper/NewEquipmentDataHandler")]
		public static void GenerateNewEquipmentData(){

			string newItemDataSourcePath = "/Users/houlianghong/Desktop/MyGameData/装备原始数据.csv";

			string newItemDataString = DataHandler.LoadDataString (newItemDataSourcePath);

			NewItemDataLoader loader = new NewItemDataLoader ();

			loader.LoadAllItemsWithFullDataString (newItemDataString);

			string newItemModelsDataPath = CommonData.originDataPath + "/EquipmentDatas.json";

			DataHandler.SaveInstanceListToFile<EquipmentModel> (loader.newItemModels, newItemModelsDataPath);

		}



		public class NewItemDataLoader{

//			private List<TempItemModel> tempItemModels = new List<TempItemModel>();

			public List<EquipmentModel> newItemModels= new List<EquipmentModel>();


			public void LoadAllItemsWithFullDataString(string fullItemDataString){

				string[] seperateItemDataArray = fullItemDataString.Split (new string[]{ "\n" }, StringSplitOptions.None);


				for (int i = 2; i < seperateItemDataArray.Length; i++) {

					string itemDataString = seperateItemDataArray [i].Replace("\r","");

					string[] itemDataArray = itemDataString.Split (new char[]{ ',' }, StringSplitOptions.None);

					int dataLength = itemDataArray.Length;

//					TempItemModel tempItemModel = new TempItemModel ();

//					tempItemModels.Add (tempItemModel);

					EquipmentModel em = new EquipmentModel();

					newItemModels.Add (em);

					em.itemId = FromStringToInt16 (itemDataArray [0]);

					em.itemName = itemDataArray [1];

					em.itemType = ItemType.Equipment;

					em.equipmentType = (EquipmentType) (FromStringToInt16(itemDataArray [2]));

					em.spriteName = itemDataArray [4];

					em.equipmentGrade = FromStringToInt16(itemDataArray [6]);

					em.price = FromStringToInt16(itemDataArray [7]);

					em.attachedSkillId = FromStringToInt16 (itemDataArray [8]);

					if (!IsSpecialProperty (itemDataArray [9])) {
						em.maxHealthGain = FromStringToInt16 (itemDataArray [9]);
					} else {
						int maxHealthGain = FromStringToInt16(itemDataArray [9].Remove (0, 1));
						em.specProperties.Add(new PropertySet(PropertyType.MaxHealth,maxHealthGain));
					}
					if (!IsSpecialProperty (itemDataArray [10])) {
						em.maxManaGain = FromStringToInt16 (itemDataArray [10]);
					} else {
						int maxManaGain = FromStringToInt16 (itemDataArray [10].Remove (0, 1));
						em.specProperties.Add(new PropertySet(PropertyType.MaxMana,maxManaGain));
					}

					if (!IsSpecialProperty (itemDataArray [11])) {
						em.attackGain = FromStringToInt16 (itemDataArray [11]);
					} else {
						int attackGain = FromStringToInt16 (itemDataArray [11].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.Attack, attackGain));
					}

					if (!IsSpecialProperty (itemDataArray [12])) {
						em.magicAttackGain = FromStringToInt16 (itemDataArray [12]);
					} else {
						int magicAttackGain = FromStringToInt16 (itemDataArray [12].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.MagicAttack, magicAttackGain));
					}

					if (!IsSpecialProperty (itemDataArray [13])) {
						em.armorGain = FromStringToInt16 (itemDataArray [13]);
					} else {
						int armorGain = FromStringToInt16 (itemDataArray [13].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.Armor, armorGain));
					}

					if (!IsSpecialProperty (itemDataArray [14])) {
						em.magicResistGain = FromStringToInt16 (itemDataArray [14]);
					} else {
						int magicResistGain = FromStringToInt16 (itemDataArray [14].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.MagicResist, magicResistGain));
					}

					if (!IsSpecialProperty (itemDataArray [15])) {
						em.armorDecreaseGain = FromStringToInt16 (itemDataArray [15]);
					} else {
						int armorDecreaseGain = FromStringToInt16 (itemDataArray [15].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.ArmorDecrease, armorDecreaseGain));
					}

					if (!IsSpecialProperty (itemDataArray [16])) {
						em.magicResistDecreaseGain = FromStringToInt16 (itemDataArray [16]);
					} else {
						int magicResistDecreaseGain = FromStringToInt16 (itemDataArray [16].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.MagicResistDecrease, magicResistDecreaseGain));
					}


					em.attackSpeed = FromStringToAttackSpeed (itemDataArray [17]);
			

					if (!IsSpecialProperty (itemDataArray [18])) {
						em.moveSpeedGain = FromStringToInt16 (itemDataArray [18]);
					} else {
						int moveSpeedGain = FromStringToInt16 (itemDataArray [18].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.MoveSpeed, moveSpeedGain));
					}

					if (!IsSpecialProperty (itemDataArray [19])) {
						em.critGain = FromStringToSingle (itemDataArray [19]);
					} else {
						float critGain = FromStringToSingle (itemDataArray [19].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.Crit, critGain));
					}

					if (!IsSpecialProperty (itemDataArray [20])) {
						em.dodgeGain = FromStringToSingle (itemDataArray [20]);
					} else {
						float dodgeGain = FromStringToSingle (itemDataArray [20].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.Dodge, dodgeGain));
					}

					if (!IsSpecialProperty (itemDataArray [21])) {
						em.critHurtScalerGain = FromStringToSingle (itemDataArray [21]);
					} else {
						float critHurtScalerGain = FromStringToSingle (itemDataArray [21].Remove (0, 1));
						em.specProperties.Add(new PropertySet(PropertyType.CritHurtScaler,critHurtScalerGain));
					}

					if (!IsSpecialProperty (itemDataArray [24])) {
						em.extraGoldGain = FromStringToInt16 (itemDataArray [24]);
					} else {
						int extraGoldGain = FromStringToInt16 (itemDataArray [24].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.ExtraGold, extraGoldGain));
					}

					if (!IsSpecialProperty (itemDataArray [25])) {
						em.extraExperienceGain = FromStringToInt16 (itemDataArray [25]);
					} else {
						int extraExperienceGain = FromStringToInt16 (itemDataArray [25].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.ExtraExperience, extraExperienceGain));
					}

					if (!IsSpecialProperty (itemDataArray [26])) {
						em.healthRecoveryGain = FromStringToInt16 (itemDataArray [26]);
					} else {
						int healthRecoveryGain = FromStringToInt16 (itemDataArray [26].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.HealthRecovery, healthRecoveryGain));
					}
					if (!IsSpecialProperty (itemDataArray [27])) {
						em.magicRecoveryGain = FromStringToInt16 (itemDataArray [27]);
					} else {
						int magicRecoveryGain = FromStringToInt16 (itemDataArray [27].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.MagicRecovery, magicRecoveryGain));
					}

				}
			}

			private bool IsSpecialProperty(string str){
				return str.Contains ("$");
			}

			private int FromStringToInt16(string str){
				return str == "" ? 0 : Convert.ToInt16 (str);
			}

			private int FromStringToInt32(string str){
				return str == "" ? 0 : Convert.ToInt32 (str);
			}

			private bool FromStringToBool(string str){
				return str == "" ? false : Convert.ToBoolean (str);
			}

			private float FromStringToSingle(string str){
				return str == "" ? 0 : Convert.ToSingle (str);
			}

			private AttackSpeed FromStringToAttackSpeed(string str){

				AttackSpeed speed = AttackSpeed.Slow;

				switch (str) {
				case "":
				case "慢速":
					speed = AttackSpeed.Slow;
					break;
				case "中速":
					speed = AttackSpeed.Medium;
					break;
				case "快速":
					speed = AttackSpeed.Fast;
					break;
				case "极快":
					speed = AttackSpeed.VeryFast;
					break;
				}

				return speed;

			}

		}

	}
}
