using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using System;

	public class PropertyGemstoneDataHandler {


		[MenuItem("EditHelper/PropertyGemstoneDataHelper")]
        public static void InitSpellItemsFromData()
        {

            string propertyGemstoneOriFilePath = "/Users/houlianghong/Desktop/MyGameData/属性宝石原始数据.csv";

			string propertyGemstoneDataString = DataHandler.LoadDataString(propertyGemstoneOriFilePath);

			PropertyGemstoneDataHelper loader = new PropertyGemstoneDataHelper();

			loader.LoadAllItemsWithFullDataString(propertyGemstoneDataString);

            string propertyGemstoneDataPath = CommonData.originDataPath + "/GameItems/PropertyGemstoneDatas.json";

			DataHandler.SaveInstanceListToFile<PropertyGemstoneModel>(loader.propertyGemstoneModels, propertyGemstoneDataPath);

			Debug.Log("属性宝石数据完成");

        }

		public class PropertyGemstoneDataHelper
        {
			public List<PropertyGemstoneModel> propertyGemstoneModels = new List<PropertyGemstoneModel>();

            public void LoadAllItemsWithFullDataString(string fullItemDataString)
            {

                string[] seperateItemDataArray = fullItemDataString.Split(new string[] { "\n" }, StringSplitOptions.None);


                for (int i = 2; i < seperateItemDataArray.Length; i++)
                {

                    string propertyGemstoneDataString = seperateItemDataArray[i].Replace("\r", "");

					string[] consumablesDataArray = propertyGemstoneDataString.Split(new char[] { ',' }, StringSplitOptions.None);

                    int dataLength = consumablesDataArray.Length;

					PropertyGemstoneModel propertyGemstoneModel = new PropertyGemstoneModel();

					propertyGemstoneModels.Add(propertyGemstoneModel);

                    propertyGemstoneModel.itemId = FromStringToInt16(consumablesDataArray[0]);

                    propertyGemstoneModel.itemName = consumablesDataArray[1];

                    propertyGemstoneModel.spriteName = consumablesDataArray[3];

                    propertyGemstoneModel.itemDescription = consumablesDataArray[4];

                    propertyGemstoneModel.price = FromStringToInt32(consumablesDataArray[5]);

                    propertyGemstoneModel.maxHealthGain = FromStringToInt16(consumablesDataArray[6]);

                    propertyGemstoneModel.maxManaGain = FromStringToInt16(consumablesDataArray[7]);

                    propertyGemstoneModel.attackGain = FromStringToInt16(consumablesDataArray[8]);

                    propertyGemstoneModel.magicAttackGain = FromStringToInt16(consumablesDataArray[9]);

                    propertyGemstoneModel.armorGain = FromStringToInt16(consumablesDataArray[10]);

                    propertyGemstoneModel.magicResistGain = FromStringToInt16(consumablesDataArray[11]);

                    propertyGemstoneModel.armorDecreaseGain = FromStringToInt16(consumablesDataArray[12]);

                    propertyGemstoneModel.magicResistDecreaseGain = FromStringToInt16(consumablesDataArray[13]);

                    propertyGemstoneModel.moveSpeedGain = FromStringToInt16(consumablesDataArray[14]);

                    propertyGemstoneModel.critGain = FromStringToInt16(consumablesDataArray[15]);

                    propertyGemstoneModel.dodgeGain = FromStringToInt16(consumablesDataArray[16]);

                    propertyGemstoneModel.critHurtScalerGain = FromStringToInt16(consumablesDataArray[17]);

                    propertyGemstoneModel.physicalHurtScalerGain = FromStringToInt16(consumablesDataArray[18]);

                    propertyGemstoneModel.magicalHurtScalerGain = FromStringToInt16(consumablesDataArray[19]);

                    propertyGemstoneModel.extraGoldGain = FromStringToInt16(consumablesDataArray[20]);

                    propertyGemstoneModel.extraExperienceGain = FromStringToInt16(consumablesDataArray[21]);

                    propertyGemstoneModel.healthRecoveryGain = FromStringToInt16(consumablesDataArray[22]);

                    propertyGemstoneModel.magicRecoveryGain = FromStringToInt16(consumablesDataArray[23]);

					propertyGemstoneModel.grade = (GemstoneGrade)FromStringToInt16(consumablesDataArray[24]);

					InitDescription(propertyGemstoneModel);
                }

            }

			private void InitDescription(PropertyGemstoneModel propertyGemstoneModel){

				string propertyRangeString = string.Empty;

				if(propertyGemstoneModel.maxHealthGain > 0){
					propertyRangeString = string.Format("{0}~{1}",Mathf.RoundToInt(propertyGemstoneModel.maxHealthGain * 0.7f),Mathf.RoundToInt(propertyGemstoneModel.maxHealthGain * 1.3f));
				}else if(propertyGemstoneModel.maxManaGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.maxManaGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.maxManaGain * 1.3f));
				}else if(propertyGemstoneModel.attackGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.attackGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.attackGain * 1.3f));
				}else if(propertyGemstoneModel.magicAttackGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.magicAttackGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.magicAttackGain * 1.3f));
				}else if(propertyGemstoneModel.armorGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.armorGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.armorGain * 1.3f));
				}else if(propertyGemstoneModel.magicResistGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.magicResistGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.magicResistGain * 1.3f));
				}else if(propertyGemstoneModel.armorDecreaseGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.armorDecreaseGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.armorDecreaseGain * 1.3f));
				}else if(propertyGemstoneModel.magicResistDecreaseGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.magicResistDecreaseGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.magicResistDecreaseGain * 1.3f));
				}else if(propertyGemstoneModel.moveSpeedGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.moveSpeedGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.moveSpeedGain * 1.3f));
				}else if(propertyGemstoneModel.critGain > 0){
					propertyRangeString = string.Format("{0}%~{1}%", Mathf.RoundToInt(propertyGemstoneModel.critGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.critGain * 1.3f));
				}else if(propertyGemstoneModel.dodgeGain > 0){
					propertyRangeString = string.Format("{0}%~{1}%", Mathf.RoundToInt(propertyGemstoneModel.dodgeGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.dodgeGain * 1.3f));
				}else if(propertyGemstoneModel.critHurtScalerGain > 0){
					propertyRangeString = string.Format("{0}%~{1}%", Mathf.RoundToInt(propertyGemstoneModel.critHurtScalerGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.critHurtScalerGain * 1.3f));
				}else if(propertyGemstoneModel.physicalHurtScalerGain > 0){
					propertyRangeString = string.Format("{0}%~{1}%", Mathf.RoundToInt(propertyGemstoneModel.physicalHurtScalerGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.physicalHurtScalerGain * 1.3f));
				}else if(propertyGemstoneModel.magicalHurtScalerGain > 0){
					propertyRangeString = string.Format("{0}%~{1}%", Mathf.RoundToInt(propertyGemstoneModel.magicalHurtScalerGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.magicalHurtScalerGain * 1.3f));
				}else if(propertyGemstoneModel.extraGoldGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.extraGoldGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.extraGoldGain * 1.3f));
				}else if(propertyGemstoneModel.extraExperienceGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.extraExperienceGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.extraExperienceGain * 1.3f));
				}else if(propertyGemstoneModel.healthRecoveryGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.healthRecoveryGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.healthRecoveryGain * 1.3f));
				}else if(propertyGemstoneModel.magicRecoveryGain > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.magicRecoveryGain * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.magicRecoveryGain * 1.3f));
				}

				propertyGemstoneModel.itemDescription = propertyGemstoneModel.itemDescription.Replace("#", propertyRangeString);            
			}

            private int FromStringToInt16(string str)
            {
                return str == "" ? 0 : Convert.ToInt16(str);
            }

            private int FromStringToInt32(string str)
            {
                return str == "" ? 0 : Convert.ToInt32(str);
            }

            private bool FromStringToBool(string str)
            {
                return str == "" ? false : Convert.ToBoolean(str);
            }

            private float FromStringToSingle(string str)
            {
                return str == "" ? 0 : Convert.ToSingle(str);
            }
        }


    }
}
