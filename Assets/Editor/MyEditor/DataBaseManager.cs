using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using UnityEditor;

namespace WordJourney{
	public class DataBaseManager {

//		private static string[] fieldNames;

//		private static List<string[]> itemsProperties = new List<string[]> ();


//		[MenuItem("Assets/BuildCET4")]
//		public static void BuildItemsDataBase(){
//
//
//			MySQLiteHelper sql = MySQLiteHelper.Instance;
//
//			sql.GetConnectionWith (CommonData.dataBaseName,"/Users/houlianghong/Desktop/Unityfolder/TestOnSkills/Assets/StreamingAssets/Data");
//
//
//
//	//		sql.CreatTable (CommonData.itemsTable,
//	//			new string[] {"itemId","itemName","itemDescription","spriteName","itemType","itemNameInEnglish",
//	//				"attackGain","powerGain","magicGain","critGain","armorGain","manaResistGain",
//	//				"dodgeGain","healthGain","strengthGain"},
//	//			new string[] {"PRIMARY Key","UNIQUE NOT NULL","NOT NULL","NOT NULL","","UNIQUE","","","","","","","","",""},
//	//			new string[] {"INTEGER","TEXT","TEXT","TEXT","INTEGER","TEXT","INTEGER","INTEGER","INTEGER",
//	//				"INTEGER","INTEGER","INTEGER","INTEGER","INTEGER","INTEGER" });
//	//
//	//		int[] stringTypeCols = new int[]{ 1, 2, 3, 5 };
//	//
//	//		itemsProperties.Clear ();
//	//
//	//		LoadItemsData ("itemsData.csv");
//	//
//	//		sql.CheckFiledNames (CommonData.itemsTable,fieldNames);
//	//
//	//		sql.DeleteAllDataFromTable (CommonData.itemsTable);
//	//
//	//		for(int i = 0;i<itemsProperties.Count;i++){
//	//			string[] values = itemsProperties [i];
//	//
//	//			foreach (int j in stringTypeCols) {
//	//				values [j] = "'" + values[j] + "'";
//	//
//	//			}
//	//
//	//			sql.InsertValues (CommonData.itemsTable, values);
//	//		}
//	//
//			if (sql.CheckTableExist (CommonData.CET4Table)) {
//				sql.DeleteTable (CommonData.CET4Table);
//			}
//
//			sql.CreateTable (CommonData.CET4Table,
//				new string[]{ "wordId", "spell", "phoneticSymbol", "explaination", "example","learnedTimes","ungraspTimes" },
//				new string[]{ "PRIMARY KEY NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL" },
//				new string[]{ "INTEGER", "TEXT", "TEXT", "TEXT", "TEXT","INTEGER DEFAULT 0","INTEGER DEFAULT 0" });
//
////			sql.CreateTable (CommonData.CET4Table,
////				new string[]{ "wordId", "spell", "phoneticSymbol", "explaination", "example","learnedTimes","ungraspTimes" },
////				new string[]{ "PRIMARY KEY NOT NULL", "UNIQUE NOT NULL", "", "NOT NULL", "","",""},
////				new string[]{ "INTEGER", "TEXT", "TEXT", "TEXT", "TEXT","INTEGER DEFAULT 0","INTEGER DEFAULT 0" });
//			
//
//
//
//			int[] stringTypeCols = new int[]{ 1, 2, 3, 4 };
//
//			itemsProperties.Clear ();
//
//			LoadCET4WordsData ();
//
////			sql.CheckFiledNames (CommonData.CET4Table, fieldNames);
//
//			sql.BeginTransaction ();
//
//			for(int i = 0;i<itemsProperties.Count;i++){
//
//				string[] values = itemsProperties [i];
//
//				foreach (int j in stringTypeCols) {
//
////					string sqliteStr = SqliteEscape (values [j]);
//					string sqliteStr = values [j];
//					values [j] = "'" + sqliteStr + "'";
//
//				}
//					 
//				sql.InsertValues (CommonData.CET4Table, values);
//			}
//
//			// 为单词表创建索引，以id为索引
////			sql.CreateIndex ("wordId_index", CommonData.CET4Table, new string[]{ "wordId" }, true);
////			// 为单词表创建索引，以学习次数为索引
////			sql.CreateIndex ("learnedTimes_index", CommonData.CET4Table, new string[]{ "learnedTimes" }, false);
////			// 为单词表创建索引，以点击 不熟悉&选错 的次数 为索引
////			sql.CreateIndex("ungraspTimes_index",CommonData.CET4Table, new string[]{ "ungraspTimes" }, false);
//
//
//			sql.EndTransaction ();
//
//			sql.CloseConnection (CommonData.dataBaseName);
//
//
//
//		}
//
//		private static void LoadCET4WordsData(){
//
//			string wordsPath = "/Users/houlianghong/Desktop/MyGameData/CET4.csv";
//
//			string wordsString = DataHandler.LoadDataString (wordsPath);
//
//			string[] wordsStrings = wordsString.Split (new string[]{ "\n" },System.StringSplitOptions.RemoveEmptyEntries);
//
//			itemsProperties.Clear ();
//
//			for (int i = 1; i < wordsStrings.Length; i++) {
//				string[] wordDataArray = wordsStrings [i].Split (new char[]{ ',' });
//				string wordId = (i-1).ToString ();
//				string spell = wordDataArray[0].Replace("'","''");
//				string phoneticSymbol = wordDataArray [1].Replace ('+', ',').Replace ("'", "''");
//				string explaination = wordDataArray [2].Replace('+',',');
//				string example = "";
//
//				itemsProperties.Add (new string[]{ wordId, spell, phoneticSymbol, explaination, example, "0", "0"});
//			}
//
//
//		}
//
//		public static string SqliteEscape(string keyWord){
//			keyWord = keyWord.Replace("/", "//");
//			keyWord = keyWord.Replace("'", "''");
//			keyWord = keyWord.Replace("[", "/[");
//			keyWord = keyWord.Replace("]", "/]");
//			keyWord = keyWord.Replace("%", "/%");
//			keyWord = keyWord.Replace("&","/&");
//			keyWord = keyWord.Replace("_", "/_");
//			keyWord = keyWord.Replace("(", "/(");
//			keyWord = keyWord.Replace(")", "/)");
//			return keyWord;
//		}

		// 从指定文件（txt／csv等文本文件）中读取数据 csv为从excel中导出的文本文件，导入unity之后需要选择结尾格式（mono里是这样的，在mono中打开csv文件后会有提示），否则在读取数据库时会报字段名不同的错误
//		private static void LoadWordsData(string dataPaths){
//
//			string itemsString = DataHandler.LoadDataString (dataPaths);
//
//			string[] stringsByLine = itemsString.Split (new string[]{ "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
//
//			fieldNames = stringsByLine [0].Split (new char[]{ ',' });
//
//			for (int i = 1; i < stringsByLine.Length; i++) {
//				itemsProperties.Add(stringsByLine [i].Split (new char[]{ ',' }));
//			}
//
//		}





//		private void ToLower(){
//			MySQLiteHelper sql = MySQLiteHelper.Instance;
//			sql.GetConnectionWith (CommonData.dataBaseName);
//
//			string tableName = "AllWordsTable";
//
//			int wordsCount = sql.GetItemCountOfTable (tableName,null,true);
//
//			for (int i = 0; i < 37336; i++) {
//
//				IDataReader reader = sql.ReadSpecificRowsOfTable (
//					"AllWordsData",
//					"Spell",
//					new string[]{ string.Format ("Id={0}", i) },
//					true);
//				reader.Read ();
//
//				string spell = reader.GetString (0);
//
//				string lowerSpell = spell.ToLower ();
//
//				if (lowerSpell == spell) {
//					continue;
//				}
//
//				lowerSpell = lowerSpell.Replace("'","''");
//
//				sql.UpdateValues ("AllWordsData", 
//					new string[]{ "Spell" },
//					new string[]{ string.Format("'{0}'",lowerSpell) },
//					new string[]{string.Format("Id = {0}",i)},
//					true);
//
//				reader.Close ();
//
//			}
//
//
//
//			sql.CloseConnection (CommonData.dataBaseName);
//		}

//		private void MoveData(){
//			MySQLiteHelper sql = MySQLiteHelper.Instance;
//
//			sql.GetConnectionWith (CommonData.dataBaseName);
//
//			sql.CreateTable ("AllWordsData",
//				new string[]{ "wordId", "Spell", "Explaination", "Valid" },
//				new string[]{ "PRIMARY KEY NOT NULL", "UNIQUE NOT NULL", "NOT NULL", "NOT NULL", "NOT NULL" },
//				new string[]{ "INTEGER", "TEXT", "TEXT", "INTEGER DEFAULT 1" });
//
//			sql.DeleteAllDataFromTable ("AllWordsData");
//
//			IDataReader reader = null;
//			int pad = 0;
//
//			for (int i = 0; i < 39286; i++) {
//
//				if (i == 34250) {
//					pad++;
//					continue;
//				}
//
//				reader = sql.ReadSpecificRowsOfTable ("AllWords", "*",
//					new string[]{ string.Format ("ID={0}", i) },
//					true);
//
//				reader.Read ();
//
//
//
//				int id = i - pad;
//				string spell = reader.GetString (1);
//				string explaination = reader.GetString (2);
//				int type = 0;
//				int valid = 1;
//
//				if (spell == string.Empty || explaination == string.Empty || spell == null || explaination == null) {
//					pad++;
//					continue;
//				}
//
//				spell = spell.Replace ("'", "''");
//				explaination = explaination.Replace ("'", "''");
//
//				sql.InsertValues ("AllWordsData",
//					new string[] {id.ToString (),
//						"'" + spell + "'",
//						"'" + explaination + "'",
//						type.ToString (),
//						valid.ToString ()
//					});
//
//				reader.Close ();
//			}
//
//
//			Debug.Log ("Finished");
//
//			sql.CloseConnection (CommonData.dataBaseName);
//
//		}


	}
}
