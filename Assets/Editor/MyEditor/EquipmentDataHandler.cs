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

			string newItemModelsDataPath = CommonData.originDataPath + "/GameItems/EquipmentDatas.json";

			DataHandler.SaveInstanceListToFile<EquipmentModel> (loader.newItemModels, newItemModelsDataPath);

			Debug.Log("装备数据完成");


		}



		public class NewItemDataLoader{
        

			public List<EquipmentModel> newItemModels= new List<EquipmentModel>();


			public void LoadAllItemsWithFullDataString(string fullItemDataString){

				string[] seperateItemDataArray = fullItemDataString.Split (new string[]{ "\n" }, StringSplitOptions.None);


				for (int i = 2; i < seperateItemDataArray.Length; i++) {

					string equipmentDataString = seperateItemDataArray [i].Replace("\r","");

					string[] equipmentDataArray = equipmentDataString.Split (new char[]{ ',' }, StringSplitOptions.None);

					int dataLength = equipmentDataArray.Length;
                    

					EquipmentModel em = new EquipmentModel();

					newItemModels.Add (em);

					em.itemId = FromStringToInt16 (equipmentDataArray [0]);

					em.itemName = equipmentDataArray [1];

					em.itemType = ItemType.Equipment;

					em.equipmentType = (EquipmentType) (FromStringToInt16(equipmentDataArray [2]));

					em.spriteName = equipmentDataArray [4];

					em.equipmentGrade = FromStringToInt16(equipmentDataArray [6]);

					em.price = FromStringToInt16(equipmentDataArray [7]);

					em.defaultQuality = (EquipmentDefaultQuality)FromStringToInt16(equipmentDataArray [8]);

					if (!IsSpecialProperty (equipmentDataArray [9])) {
						em.maxHealthGain = FromStringToInt16 (equipmentDataArray [9]);
					} else {
						int maxHealthGain = FromStringToInt16(equipmentDataArray [9].Remove (0, 1));
						em.specProperties.Add(new PropertySet(PropertyType.MaxHealth,maxHealthGain));
					}
					if (!IsSpecialProperty (equipmentDataArray [10])) {
						em.maxManaGain = FromStringToInt16 (equipmentDataArray [10]);
					} else {
						int maxManaGain = FromStringToInt16 (equipmentDataArray [10].Remove (0, 1));
						em.specProperties.Add(new PropertySet(PropertyType.MaxMana,maxManaGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [11])) {
						em.attackGain = FromStringToInt16 (equipmentDataArray [11]);
					} else {
						int attackGain = FromStringToInt16 (equipmentDataArray [11].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.Attack, attackGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [12])) {
						em.magicAttackGain = FromStringToInt16 (equipmentDataArray [12]);
					} else {
						int magicAttackGain = FromStringToInt16 (equipmentDataArray [12].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.MagicAttack, magicAttackGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [13])) {
						em.armorGain = FromStringToInt16 (equipmentDataArray [13]);
					} else {
						int armorGain = FromStringToInt16 (equipmentDataArray [13].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.Armor, armorGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [14])) {
						em.magicResistGain = FromStringToInt16 (equipmentDataArray [14]);
					} else {
						int magicResistGain = FromStringToInt16 (equipmentDataArray [14].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.MagicResist, magicResistGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [15])) {
						em.armorDecreaseGain = FromStringToInt16 (equipmentDataArray [15]);
					} else {
						int armorDecreaseGain = FromStringToInt16 (equipmentDataArray [15].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.ArmorDecrease, armorDecreaseGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [16])) {
						em.magicResistDecreaseGain = FromStringToInt16 (equipmentDataArray [16]);
					} else {
						int magicResistDecreaseGain = FromStringToInt16 (equipmentDataArray [16].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.MagicResistDecrease, magicResistDecreaseGain));
					}


					em.attackSpeed = FromStringToAttackSpeed (equipmentDataArray [17]);
			

					if (!IsSpecialProperty (equipmentDataArray [18])) {
						em.moveSpeedGain = FromStringToInt16 (equipmentDataArray [18]);
					} else {
						int moveSpeedGain = FromStringToInt16 (equipmentDataArray [18].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.MoveSpeed, moveSpeedGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [19])) {
						em.critGain = FromStringToSingle (equipmentDataArray [19]);
					} else {
						float critGain = FromStringToSingle (equipmentDataArray [19].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.Crit, critGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [20])) {
						em.dodgeGain = FromStringToSingle (equipmentDataArray [20]);
					} else {
						float dodgeGain = FromStringToSingle (equipmentDataArray [20].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.Dodge, dodgeGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [21])) {
						em.critHurtScalerGain = FromStringToSingle (equipmentDataArray [21]);
					} else {
						float critHurtScalerGain = FromStringToSingle (equipmentDataArray [21].Remove (0, 1));
						em.specProperties.Add(new PropertySet(PropertyType.CritHurtScaler,critHurtScalerGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [24])) {
						em.extraGoldGain = FromStringToInt16 (equipmentDataArray [24]);
					} else {
						int extraGoldGain = FromStringToInt16 (equipmentDataArray [24].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.ExtraGold, extraGoldGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [25])) {
						em.extraExperienceGain = FromStringToInt16 (equipmentDataArray [25]);
					} else {
						int extraExperienceGain = FromStringToInt16 (equipmentDataArray [25].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.ExtraExperience, extraExperienceGain));
					}

					if (!IsSpecialProperty (equipmentDataArray [26])) {
						em.healthRecoveryGain = FromStringToInt16 (equipmentDataArray [26]);
					} else {
						int healthRecoveryGain = FromStringToInt16 (equipmentDataArray [26].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.HealthRecovery, healthRecoveryGain));
					}
					if (!IsSpecialProperty (equipmentDataArray [27])) {
						em.magicRecoveryGain = FromStringToInt16 (equipmentDataArray [27]);
					} else {
						int magicRecoveryGain = FromStringToInt16 (equipmentDataArray [27].Remove (0, 1));
						em.specProperties.Add (new PropertySet (PropertyType.MagicRecovery, magicRecoveryGain));
					}

					em.weaponType = (WeaponType)FromStringToInt16 (equipmentDataArray [28]);

				}
			}

			private bool IsSpecialProperty(string str){
				return str.Contains ("$");
			}

			private int FromStringToInt16(string str){
				//Debug.Log(str);
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
