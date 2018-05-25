using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEditor;
	using System;


	public class SpecialItemDataHandler
	{

		[MenuItem("EditHelper/SpecialItemDataHelper")]
		public static void InitSpellItemsFromData()
		{

			string specialItemOriFilePath = "/Users/houlianghong/Desktop/MyGameData/特殊物品原始数据.csv";

			string speicalItemDataString = DataHandler.LoadDataString(specialItemOriFilePath);

			SpecialItemDataHelper loader = new SpecialItemDataHelper();

			loader.LoadAllItemsWithFullDataString(speicalItemDataString);

			string specialItemDataPath = CommonData.originDataPath + "/GameItems/SpecialItemDatas.json";

			DataHandler.SaveInstanceListToFile<SpecialItemModel>(loader.speciallItemModels, specialItemDataPath);

			Debug.Log("特殊物品数据完成");

		}

		public class SpecialItemDataHelper
		{
			public List<SpecialItemModel> speciallItemModels = new List<SpecialItemModel>();

			public void LoadAllItemsWithFullDataString(string fullItemDataString)
			{

				string[] seperateItemDataArray = fullItemDataString.Split(new string[] { "\n" }, StringSplitOptions.None);


				for (int i = 2; i < seperateItemDataArray.Length; i++)
				{

					string specialItemDataString = seperateItemDataArray[i].Replace("\r", "");

					string[] specialItemDataArray = specialItemDataString.Split(new char[] { ',' }, StringSplitOptions.None);

					int dataLength = specialItemDataArray.Length;

					SpecialItemModel specialItemModel = new SpecialItemModel();

					speciallItemModels.Add(specialItemModel);

					specialItemModel.itemId = FromStringToInt16(specialItemDataArray[0]);

					specialItemModel.itemName = specialItemDataArray[1];

					specialItemModel.spriteName = specialItemDataArray[3];

					specialItemModel.itemDescription = specialItemDataArray[4].Replace('+',',');

					specialItemModel.price = FromStringToInt32(specialItemDataArray[5]);

					specialItemModel.specialItemType = (SpecialItemType)FromStringToInt16(specialItemDataArray[6]);

					specialItemModel.isShowInBagOnly = FromStringToBool(specialItemDataArray[7]);
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

