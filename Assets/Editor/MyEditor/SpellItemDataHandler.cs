using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using System;

	public class SpellItemDataHandler
	{


		[MenuItem("EditHelper/SpellItemDataHelper")]
		public static void InitSpellItemsFromData()
		{

			string spellItemDataPath = "/Users/houlianghong/Desktop/MyGameData/拼写物品原始数据.csv";

			string spellItemDataString = DataHandler.LoadDataString(spellItemDataPath);

			SpellItemDataHelper loader = new SpellItemDataHelper();

			loader.LoadAllItemsWithFullDataString(spellItemDataString);

			string spellITemDataPath = CommonData.originDataPath + "/GameItems/SpellItemDatas.json";

			DataHandler.SaveInstanceListToFile<SpellItemModel>(loader.spellItemModels, spellITemDataPath);

			Debug.Log("拼写物品数据完成");

		}

	}

	public class SpellItemDataHelper{


		public List<SpellItemModel> spellItemModels = new List<SpellItemModel>();

		public void LoadAllItemsWithFullDataString(string fullItemDataString)
		{

			string[] seperateItemDataArray = fullItemDataString.Split(new string[] { "\n" }, StringSplitOptions.None);


			for (int i = 2; i < seperateItemDataArray.Length; i++)
			{

				string spellItemDataString = seperateItemDataArray[i].Replace("\r", "");

				string[] spellItemDataArray = spellItemDataString.Split(new char[] { ',' }, StringSplitOptions.None);

				int dataLength = spellItemDataArray.Length;

				SpellItemModel spellItemModel = new SpellItemModel();

				spellItemModels.Add(spellItemModel);

				spellItemModel.itemId = FromStringToInt16(spellItemDataArray[0]);

				spellItemModel.itemName = spellItemDataArray[1];

				spellItemModel.spriteName = spellItemDataArray[3];

				spellItemModel.spell = spellItemDataArray[4];

				//spellItemModel.itemDescription = spellItemDataArray[4];

				spellItemModel.phoneticSymbol = spellItemDataArray[5];

				spellItemModel.pronounciationURL = spellItemDataArray[6];

				spellItemModel.spellItemType = (SpellItemType)(FromStringToInt16(spellItemDataArray[7]));

				spellItemModel.attachInfo_1 = FromStringToInt16(spellItemDataArray[8]);

				spellItemModel.attachInfo_2 = FromStringToInt16(spellItemDataArray[9]);

				//spellItemModel.price = FromStringToInt32(spellItemDataArray[8]); 


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
