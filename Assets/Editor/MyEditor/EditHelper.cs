namespace WordJourney
{
	using UnityEditor;
	using System.IO;
	using System.Collections.Generic;
	using System.Collections;
	using UnityEngine;

	public class EditHelper {
          
		//[MenuItem("EditHelper/EncodeTest")]
		//public static void EncodeTest(){

		//	string testFilePath = "/Users/houlianghong/Desktop/EncryptionTest/test.json";

		//	string encodeFilePath = "/Users/houlianghong/Desktop/EncryptionTest/encode.json";
            
		//	string decodeFilePaht = "/Users/houlianghong/Desktop/EncryptionTest/decode.json";

		//	string source = DataHandler.LoadDataString(testFilePath);

		//	string encode = StringEncryption.Encode(source);

		//	DataHandler.SaveDataString(encode, encodeFilePath);

		//	encode = DataHandler.LoadDataString(encodeFilePath);

		//	string decode = StringEncryption.Decode(encode);

		//	DataHandler.SaveDataString(decode, decodeFilePaht);


		//}



		[MenuItem("EditHelper/GenerateMapEventsRecords")]
		public static void GenerateMapEventsRecords()
		{
        
            List<MapEventsRecord> mapEventsRecords = new List<MapEventsRecord>();

            for (int i = 0; i <= CommonData.maxLevelIndex; i++)
            {
				mapEventsRecords.Add(new MapEventsRecord(i, new List<Vector2>(),false,false));
            }

			DataHandler.SaveInstanceListToFile<MapEventsRecord>(mapEventsRecords, CommonData.originDataPath + "/MapEventsRecord.json");

		}

      
		[MenuItem("EditHelper/MapDoorPairPositionCheck")]
		public static void MapDoorPairPositionCheck(){

			for (int i = 0; i < 50;i++){

				Debug.Log("地图" + i.ToString());
				
				HLHMapData mapData = HLHMapData.LoadMapDataOfLevel(i);

				int mapHeight = mapData.rowCount;

				MapAttachedInfoLayer layer = mapData.attachedInfoLayers.Find(delegate (MapAttachedInfoLayer obj)
				{
					return obj.layerName == "GearEventLayer";

				});
                
				Dictionary<string, string> doorInfoDic = new Dictionary<string, string>();

				for (int j = 0; j < layer.tileDatas.Count;j++){
                
					MapAttachedInfoTile tile = layer.tileDatas[j];

					if(!tile.type.Equals("doorGear") && !tile.type.Equals("keyDoorGear")){

						continue;
					}

					string pairDoorPosString = KVPair.GetPropertyStringWithKey("pairDoorPos", tile.properties);

					string[] posXY = pairDoorPosString.Split(new char[] { '_' }, System.StringSplitOptions.RemoveEmptyEntries);

					int pairDoorPosY = mapHeight - int.Parse(posXY[1]) - 1;

					pairDoorPosString = string.Format("{0}_{1}", posXY[0], pairDoorPosY);

					string doorPosString = string.Format("{0}_{1}", Mathf.RoundToInt(tile.position.x), Mathf.RoundToInt(tile.position.y));

					doorInfoDic.Add(doorPosString, pairDoorPosString);
               
				}

				IDictionaryEnumerator dictionaryEnumerator = doorInfoDic.GetEnumerator();

				while(dictionaryEnumerator.MoveNext()){
					
					string doorPos = dictionaryEnumerator.Key as string;
					string pairDoorPos = dictionaryEnumerator.Value as string;

					string pos = string.Empty;
					bool posRight = doorInfoDic.TryGetValue(pairDoorPos, out pos);
					if(!posRight){
						Debug.LogFormat("地图序：{0},门数据有问题的位置：{1},匹配位置：{2}", i, doorPos,pairDoorPos);
					}

				}
                
			}


		}

		//[MenuItem("EditHelper/MapEventOnWallCheck")]
		//public static void MapEventOnWallCheck()
        //{

        //    for (int i = 0; i < 50; i++)
        //    {

        //        Debug.Log("地图" + i.ToString());

        //        HLHMapData mapData = HLHMapData.LoadMapDataOfLevel(i);

        //        int mapHeight = mapData.rowCount;

        //        MapAttachedInfoLayer layer = mapData.attachedInfoLayers.Find(delegate (MapAttachedInfoLayer obj)
        //        {
        //            return obj.layerName == "GearEventLayer";

        //        });

        //        Dictionary<string, string> doorInfoDic = new Dictionary<string, string>();

        //        for (int j = 0; j < layer.tileDatas.Count; j++)
        //        {

        //            MapAttachedInfoTile tile = layer.tileDatas[j];

        //            if (!tile.type.Equals("doorGear") && !tile.type.Equals("keyDoorGear"))
        //            {

        //                continue;
        //            }

        //            string pairDoorPosString = KVPair.GetPropertyStringWithKey("pairDoorPos", tile.properties);

        //            string[] posXY = pairDoorPosString.Split(new char[] { '_' }, System.StringSplitOptions.RemoveEmptyEntries);

        //            int pairDoorPosY = mapHeight - int.Parse(posXY[1]) - 1;

        //            pairDoorPosString = string.Format("{0}_{1}", posXY[0], pairDoorPosY);

        //            string doorPosString = string.Format("{0}_{1}", Mathf.RoundToInt(tile.position.x), Mathf.RoundToInt(tile.position.y));

        //            doorInfoDic.Add(doorPosString, pairDoorPosString);

        //        }

        //        IDictionaryEnumerator dictionaryEnumerator = doorInfoDic.GetEnumerator();

        //        while (dictionaryEnumerator.MoveNext())
        //        {

        //            string doorPos = dictionaryEnumerator.Key as string;
        //            string pairDoorPos = dictionaryEnumerator.Value as string;

        //            string pos = string.Empty;
        //            bool posRight = doorInfoDic.TryGetValue(pairDoorPos, out pos);
        //            if (!posRight)
        //            {
        //                Debug.LogFormat("地图序：{0},门数据有问题的位置：{1},匹配位置：{2}", i, doorPos, pairDoorPos);
        //            }

        //        }

        //    }


        //}


		[MenuItem("EditHelper/GeneratePlayerJson")]
		public static void GeneratePlayerJsonData(){

			Player p = TransformManager.FindTransform ("Player").GetComponent<Player> ();

//			p.allEquipedEquipments = new Equipment[6];

			p.physicalHurtScaler = 1.0f;
			p.magicalHurtScaler = 1.0f;
			p.critHurtScaler = 1.5f;

			PlayerData pd = new PlayerData (p);

			string originalPlayerDataPath = CommonData.oriPlayerDataFilePath;

			DataHandler.SaveInstanceDataToFile<PlayerData> (pd, originalPlayerDataPath);

			string playerDataPath = CommonData.playerDataFilePath;

			DataHandler.SaveInstanceDataToFile<PlayerData>(pd, playerDataPath);
		}




//		// 将物品csv数据转化为json文件并存储直接使用这个方法
//		private static void ConvertItemToJson(){
//
//			EquipmentModel[] allItem = ItemsToJson ();
//
//			AllItemsJson aij = new AllItemsJson ();
//
//			aij.Items = allItem;
//
//			string allItemsJsonStr = JsonUtility.ToJson (aij);
//
//			File.WriteAllText (CommonData.persistDataPath + "/AllItemsJson.txt", allItemsJsonStr);
//
//		}
//
//		static private EquipmentModel[] ItemsToJson(){
//
//			string csv = DataHandler.LoadDataString ("itemsData.csv");
//
//			string[] dataArray = csv.Split (new string[]{ "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
//
//			List <EquipmentModel> allItem = new List<EquipmentModel> ();
//
//			for (int i = 1; i < dataArray.Length; i++) {
//
//				string[] itemStr = dataArray [i].Split (',');
//					
//				EquipmentModel itemModel = new EquipmentModel ();
//
//				itemModel.itemId = System.Convert.ToInt32 (itemStr [0]);
//				itemModel.itemName = itemStr[1];
//				itemModel.itemDescription = itemStr[2];
//				itemModel.spriteName = itemStr[3];
//				itemModel.itemType = (ItemType)System.Convert.ToInt16(itemStr[4]);
////				itemModel.itemNameInEnglish = itemStr[5];
//				itemModel.attackGain = System.Convert.ToInt16(itemStr[6]);
////				itemModel.attackSpeedGain = System.Convert.ToInt16(itemStr[7]);
//				itemModel.armorGain = System.Convert.ToInt16(itemStr[8]);
//				itemModel.magicResistGain = System.Convert.ToInt16(itemStr[9]);
//				itemModel.critGain = System.Convert.ToInt16(itemStr[10]);
//				itemModel.dodgeGain = System.Convert.ToInt16(itemStr[11]);
////				itemModel.healthGain = System.Convert.ToInt16(itemStr[12]);
////				itemModel.manaGain = System.Convert.ToInt16 (itemStr [13]);
////				itemModel.equipmentType = (EquipmentType)System.Convert.ToInt16 (itemStr [14]);
//
//				allItem.Add (itemModel);
//
//			}
//
//			EquipmentModel[] ItemArray = new EquipmentModel[allItem.Count];
//
//			allItem.CopyTo(ItemArray);
//
//
//			return ItemArray;
//
//		}
//
//		public class AllItemsJson{
//
//			public EquipmentModel[] Items;
//
//		}

	}
		
}