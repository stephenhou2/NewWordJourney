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

                    propertyGemstoneModel.maxHealthGainBase = FromStringToInt16(consumablesDataArray[6]);

					propertyGemstoneModel.maxManaGainBase = FromStringToInt16(consumablesDataArray[7]);

					propertyGemstoneModel.attackGainBase = FromStringToInt16(consumablesDataArray[8]);

					propertyGemstoneModel.magicAttackGainBase = FromStringToInt16(consumablesDataArray[9]);

					propertyGemstoneModel.armorGainBase = FromStringToInt16(consumablesDataArray[10]);

					propertyGemstoneModel.magicResistGainBase = FromStringToInt16(consumablesDataArray[11]);

					propertyGemstoneModel.armorDecreaseGainBase = FromStringToInt16(consumablesDataArray[12]);

					propertyGemstoneModel.magicResistDecreaseGainBase = FromStringToInt16(consumablesDataArray[13]);

					propertyGemstoneModel.moveSpeedGainBase = FromStringToInt16(consumablesDataArray[14]);

					propertyGemstoneModel.critGainBase = FromStringToInt16(consumablesDataArray[15]);

					propertyGemstoneModel.dodgeGainBase = FromStringToInt16(consumablesDataArray[16]);

					propertyGemstoneModel.critHurtScalerGainBase = FromStringToInt16(consumablesDataArray[17]);

					propertyGemstoneModel.physicalHurtScalerGainBase = FromStringToInt16(consumablesDataArray[18]);

					propertyGemstoneModel.magicalHurtScalerGainBase = FromStringToInt16(consumablesDataArray[19]);

					propertyGemstoneModel.extraGoldGainBase = FromStringToInt16(consumablesDataArray[20]);

					propertyGemstoneModel.extraExperienceGainBase = FromStringToInt16(consumablesDataArray[21]);

					propertyGemstoneModel.healthRecoveryGainBase = FromStringToInt16(consumablesDataArray[22]);

					propertyGemstoneModel.magicRecoveryGainBase = FromStringToInt16(consumablesDataArray[23]);

					propertyGemstoneModel.grade = (GemstoneGrade)FromStringToInt16(consumablesDataArray[24]);

					InitDescription(propertyGemstoneModel);
                }

            }

			private void InitDescription(PropertyGemstoneModel propertyGemstoneModel){

				string propertyRangeString = string.Empty;

				if(propertyGemstoneModel.maxHealthGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}",Mathf.RoundToInt(propertyGemstoneModel.maxHealthGainBase * 0.7f),Mathf.RoundToInt(propertyGemstoneModel.maxHealthGainBase * 1.3f));
				}else if(propertyGemstoneModel.maxManaGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.maxManaGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.maxManaGainBase * 1.3f));
				}else if(propertyGemstoneModel.attackGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.attackGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.attackGainBase * 1.3f));
				}else if(propertyGemstoneModel.magicAttackGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.magicAttackGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.magicAttackGainBase * 1.3f));
				}else if(propertyGemstoneModel.armorGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.armorGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.armorGainBase * 1.3f));
				}else if(propertyGemstoneModel.magicResistGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.magicResistGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.magicResistGainBase * 1.3f));
				}else if(propertyGemstoneModel.armorDecreaseGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.armorDecreaseGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.armorDecreaseGainBase * 1.3f));
				}else if(propertyGemstoneModel.magicResistDecreaseGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.magicResistDecreaseGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.magicResistDecreaseGainBase * 1.3f));
				}else if(propertyGemstoneModel.moveSpeedGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.moveSpeedGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.moveSpeedGainBase * 1.3f));
				}else if(propertyGemstoneModel.critGainBase > 0){
					propertyRangeString = string.Format("{0}%~{1}%", Mathf.RoundToInt(propertyGemstoneModel.critGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.critGainBase * 1.3f));
				}else if(propertyGemstoneModel.dodgeGainBase > 0){
					propertyRangeString = string.Format("{0}%~{1}%", Mathf.RoundToInt(propertyGemstoneModel.dodgeGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.dodgeGainBase * 1.3f));
				}else if(propertyGemstoneModel.critHurtScalerGainBase > 0){
					propertyRangeString = string.Format("{0}%~{1}%", Mathf.RoundToInt(propertyGemstoneModel.critHurtScalerGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.critHurtScalerGainBase * 1.3f));
				}else if(propertyGemstoneModel.physicalHurtScalerGainBase > 0){
					propertyRangeString = string.Format("{0}%~{1}%", Mathf.RoundToInt(propertyGemstoneModel.physicalHurtScalerGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.physicalHurtScalerGainBase * 1.3f));
				}else if(propertyGemstoneModel.magicalHurtScalerGainBase > 0){
					propertyRangeString = string.Format("{0}%~{1}%", Mathf.RoundToInt(propertyGemstoneModel.magicalHurtScalerGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.magicalHurtScalerGainBase * 1.3f));
				}else if(propertyGemstoneModel.extraGoldGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.extraGoldGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.extraGoldGainBase * 1.3f));
				}else if(propertyGemstoneModel.extraExperienceGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.extraExperienceGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.extraExperienceGainBase * 1.3f));
				}else if(propertyGemstoneModel.healthRecoveryGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.healthRecoveryGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.healthRecoveryGainBase * 1.3f));
				}else if(propertyGemstoneModel.magicRecoveryGainBase > 0){
					propertyRangeString = string.Format("{0}~{1}", Mathf.RoundToInt(propertyGemstoneModel.magicRecoveryGainBase * 0.7f), Mathf.RoundToInt(propertyGemstoneModel.magicRecoveryGainBase * 1.3f));
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
