using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	[System.Serializable]
	public class HLHGameLevelData{

		// 关卡序号
		public int gameLevelIndex;

		// 瓦罐中能开出来的物品id数组
		public List<int> itemIdsInPot;

		// 木桶中能开出来的物品id数组
		public List<int> itemIdsInBucket;

		// 宝箱中可能会开出来的物品id数组
		public List<int> itemIdsInTreasureBox;

		public List<int> goodsIdsListArray_0 = new List<int>();
		public List<int> goodsIdsListArray_1 = new List<int>();
		public List<int> goodsIdsListArray_2 = new List<int>();
		public List<int> goodsIdsListArray_3 = new List<int>();
		public List<int> goodsIdsListArray_4 = new List<int>();

		public HLHGameLevelData(string dataString,int currenLevel){

			dataString = dataString.Replace ("\r", "");

			string[] dataStrings = dataString.Split (new char[]{ ','},System.StringSplitOptions.None);

			gameLevelIndex = int.Parse (dataStrings [0]);

			itemIdsInPot = InitIntArrayWithString (dataStrings [1]);

			itemIdsInBucket = InitIntArrayWithString (dataStrings [2]);

			itemIdsInTreasureBox = InitIntArrayWithString (dataStrings [3]);

			goodsIdsListArray_0 = InitIntArrayWithString (dataStrings [4]);
			goodsIdsListArray_1 = InitIntArrayWithString (dataStrings [5]);
			goodsIdsListArray_2 = InitIntArrayWithString (dataStrings [6]);
			goodsIdsListArray_3 = InitIntArrayWithString (dataStrings [7]);
			goodsIdsListArray_4 = InitIntArrayWithString (dataStrings [8]);



		}

		private List<int> InitIntArrayWithString(string dataString){
			string[] idStrings = dataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);
			List<int> idList = new List<int> ();
			for (int i = 0; i < idStrings.Length; i++) {
				string idAndCount = idStrings [i];
//				Debug.Log (idAndCount);
				string[] data = idAndCount.Split (new char[]{ '^' }, System.StringSplitOptions.None);
				int id = int.Parse (data [0]);
				int count = int.Parse (data [1]);
				for (int j = 0; j < count; j++) {
					idList.Add (id);
				}
			}
			return idList;
		}

	
	}
}
