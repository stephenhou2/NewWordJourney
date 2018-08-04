using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using System;

    /// <summary>
	/// 消耗品数据初始化工具
    /// </summary>
	public class ConsumablesDataHandler
    {

		[MenuItem("EditHelper/ConsumablesDataHelper")]
        public static void InitSpellItemsFromData()
        {
            /******************** 注意： *******************/
			// 1. 原始数据格式必须使用【消耗品原始数据表（02_resume_design）】导出的csv文件，原始表格尾部不能有空行
            // 2. 原始数据存放的地址根据实际地址进行修改
            string consumablesOriFilePath = "/Users/houlianghong/Desktop/MyGameData/消耗品原始数据.csv";
			/******************** 注意： *******************/

			string consumablesDataString= DataHandler.LoadDataString(consumablesOriFilePath);

			ConsumablesDataHelper loader = new ConsumablesDataHelper();

			loader.LoadAllItemsWithFullDataString(consumablesDataString);

            string consumablesDataPath = CommonData.originDataPath + "/GameItems/ConsumablesDatas.json";

			DataHandler.SaveInstanceListToFile<ConsumablesModel>(loader.consumablesModels, consumablesDataPath);

			Debug.Log("消耗品数据完成");

        }
    }

	public class ConsumablesDataHelper
    {      
		public List<ConsumablesModel> consumablesModels = new List<ConsumablesModel>();

        public void LoadAllItemsWithFullDataString(string fullItemDataString)
        {

            string[] seperateItemDataArray = fullItemDataString.Split(new string[] { "\n" }, StringSplitOptions.None);


            for (int i = 2; i < seperateItemDataArray.Length; i++)
            {

                string consumablesDataString = seperateItemDataArray[i].Replace("\r", "");

				string[] consumablesDataArray = consumablesDataString.Split(new char[] { ',' }, StringSplitOptions.None);

                int dataLength = consumablesDataArray.Length;

				ConsumablesModel consumablesModel = new ConsumablesModel();

                consumablesModels.Add(consumablesModel);

                consumablesModel.itemId = FromStringToInt16(consumablesDataArray[0]);

                consumablesModel.itemName = consumablesDataArray[1];

                consumablesModel.spriteName = consumablesDataArray[3];
            
				consumablesModel.itemDescription = consumablesDataArray[4].Replace('+',',');
                

                consumablesModel.price = FromStringToInt32(consumablesDataArray[5]);

				consumablesModel.isShowInBagOnly = FromStringToBool(consumablesDataArray[6]);

				consumablesModel.audioName = consumablesDataArray[7];

				//最大生命 最大魔法    物理攻击 魔法攻击    护甲 魔抗  护甲穿透 魔抗穿透    移速 暴击  闪避 暴击倍率    物理伤害系数 魔法伤害系数  额外金钱 额外经验    生命回复 魔法回复

				consumablesModel.healthGain = FromStringToInt16(consumablesDataArray[8]);

				consumablesModel.manaGain = FromStringToInt16(consumablesDataArray[9]);

				consumablesModel.experienceGain = FromStringToInt16(consumablesDataArray[10]);

                consumablesModel.maxHealthGain = FromStringToInt16(consumablesDataArray[11]);

                consumablesModel.maxManaGain = FromStringToInt16(consumablesDataArray[12]);

                consumablesModel.attackGain = FromStringToInt16(consumablesDataArray[13]);

                consumablesModel.magicAttackGain = FromStringToInt16(consumablesDataArray[14]);

                consumablesModel.armorGain = FromStringToInt16(consumablesDataArray[15]);

                consumablesModel.magicResistGain = FromStringToInt16(consumablesDataArray[16]);

                consumablesModel.armorDecreaseGain = FromStringToInt16(consumablesDataArray[17]);

                consumablesModel.magicResistDecreaseGain = FromStringToInt16(consumablesDataArray[18]);

                consumablesModel.moveSpeedGain = FromStringToInt16(consumablesDataArray[19]);

                consumablesModel.critGain = FromStringToInt16(consumablesDataArray[20]);

                consumablesModel.dodgeGain = FromStringToInt16(consumablesDataArray[21]);

                consumablesModel.critHurtScalerGain = FromStringToInt16(consumablesDataArray[22]);

                consumablesModel.physicalHurtScalerGain = FromStringToInt16(consumablesDataArray[23]);

                consumablesModel.magicalHurtScalerGain = FromStringToInt16(consumablesDataArray[24]);

                consumablesModel.extraGoldGain = FromStringToInt16(consumablesDataArray[25]);

                consumablesModel.extraExperienceGain = FromStringToInt16(consumablesDataArray[26]);

                consumablesModel.healthRecoveryGain = FromStringToInt16(consumablesDataArray[27]);

                consumablesModel.magicRecoveryGain = FromStringToInt16(consumablesDataArray[28]);

				consumablesModel.consumablesGrade = FromStringToInt16(consumablesDataArray[29]);
                            

            }

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
