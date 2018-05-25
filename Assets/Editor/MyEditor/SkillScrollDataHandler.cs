using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using System;

	public class SkillScrollDataHandler
    {


        [MenuItem("EditHelper/SkillScrollDataHelper")]
        public static void InitSpellItemsFromData()
        {

            string skillScrollOriFilePath = "/Users/houlianghong/Desktop/MyGameData/技能卷轴原始数据.csv";

			string skillScrollDataString = DataHandler.LoadDataString(skillScrollOriFilePath);

			SkillScrollDataHelper loader = new SkillScrollDataHelper();

			loader.LoadAllItemsWithFullDataString(skillScrollDataString);

            string propertyGemstoneDataPath = CommonData.originDataPath + "/GameItems/SkillScrollDatas.json";

			DataHandler.SaveInstanceListToFile<SkillScrollModel>(loader.skillScrollModels, propertyGemstoneDataPath);

            Debug.Log("技能卷轴数据完成");

        }

		public class SkillScrollDataHelper
        {
			public List<SkillScrollModel> skillScrollModels = new List<SkillScrollModel>();

            public void LoadAllItemsWithFullDataString(string fullItemDataString)
            {

                string[] seperateItemDataArray = fullItemDataString.Split(new string[] { "\n" }, StringSplitOptions.None);


                for (int i = 2; i < seperateItemDataArray.Length; i++)
                {

                    string skillScrollDataString = seperateItemDataArray[i].Replace("\r", "");

					string[] skillScrollDataArray = skillScrollDataString.Split(new char[] { ',' }, StringSplitOptions.None);

					int dataLength = skillScrollDataArray.Length;

					SkillScrollModel skillScrollModel = new SkillScrollModel();

					skillScrollModels.Add(skillScrollModel);

                    skillScrollModel.itemId = FromStringToInt16(skillScrollDataArray[0]);

                    skillScrollModel.itemName = skillScrollDataArray[1];

                    skillScrollModel.spriteName = skillScrollDataArray[4];

					skillScrollModel.itemDescription = skillScrollDataArray[5].Replace('+',',');

                    skillScrollModel.price = FromStringToInt32(skillScrollDataArray[6]);

					skillScrollModel.skillId = FromStringToInt16(skillScrollDataArray[7]);
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

}

