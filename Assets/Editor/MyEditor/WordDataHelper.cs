using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using System.Data;
	using System.Text;

	public class WordDataHelper {

//		private List<string[]> simpleWordsInputList = new List<string[]> ();
//		private List<string[]> mediumWordsInputList = new List<string[]> ();
//		private List<string[]> masterWordsInputList = new List<string[]> ();

		private static string filePathOfSimpleLevelWords = "/Users/houlianghong/Desktop/MyGameData/单词原始数据/初级单词.csv";
		private static string filePathOfMediumLevelWords = "/Users/houlianghong/Desktop/MyGameData/单词原始数据/中级单词.csv";
		private static string filePathOfMasterLevelWords = "/Users/houlianghong/Desktop/MyGameData/单词原始数据/高级单词.csv";

		[MenuItem("EditHelper/InitWordToDataBase")]
		public static void InitSimpleLevelWords(){

			WordDataHelper wdh = new WordDataHelper ();

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			sql.GetConnectionWith (CommonData.dataBaseName,CommonData.originDataPath);

			wdh.WordsDataToDataBase (filePathOfSimpleLevelWords, CommonData.simpleWordsTable, sql);

			wdh.WordsDataToDataBase (filePathOfMediumLevelWords, CommonData.mediumWordsTabel, sql);

			wdh.WordsDataToDataBase (filePathOfMasterLevelWords, CommonData.masterWordsTabel, sql);

			sql.CloseConnection (CommonData.dataBaseName);

		}



		private void WordsDataToDataBase(string wordsFilePath,string wordsTableName,MySQLiteHelper sql){


			bool isSimpleWordsTableExist = sql.CheckTableExist (wordsTableName);

			if (isSimpleWordsTableExist) {
				sql.DeleteTable (wordsTableName);;
			}

            // isFamiliar 是否熟悉,熟悉为1,不熟悉为0

			sql.CreateTable (wordsTableName,
				new string[] {
					"wordId",
					"spell",
					"phoneticSymbol",
					"explaination",
					"sentenceEN",
					"sentenceCH",
					"pronouncationURL",
					"wordLength",
					"learnedTimes",
					"ungraspTimes",
                    "isFamiliar"
				},
				new string[] {
					"PRIMARY KEY NOT NULL",
					"NOT NULL",
					"NOT NULL",
					"NOT NULL",
					"NOT NULL",
					"NOT NULL",
					"NOT NULL",
					"NOT NULL",
					"NOT NULL",
					"NOT NULL",
                    "NOT NULL"
				},
				new string[] {
					"INTEGER",
					"TEXT",
					"TEXT",
					"TEXT",
					"TEXT",
					"TEXT",
					"TEXT",
					"INTEGER DEFAULT 0",
					"INTEGER DEFAULT 0",
					"INTEGER DEFAULT 0",
                    "INTEGER DEFAULT 0"
				});




			string simpleWordsData = DataHandler.LoadDataString (wordsFilePath);

			string[] simpleWordsDataArray = simpleWordsData.Split (new char[]{ '\n' }, System.StringSplitOptions.RemoveEmptyEntries);


			sql.BeginTransaction ();

			for (int i = 1; i < simpleWordsDataArray.Length; i++) {

				string singleWordData = simpleWordsDataArray [i];
                

				string[] datas = singleWordData.Split (new char[]{ ',' }, System.StringSplitOptions.RemoveEmptyEntries);

				string wordId = (i - 1).ToString();
				string spell = "'" + datas [0].Replace("'","''") + "'";
				string explaination = "'" + ExplainationNormalization(datas [1]) + "'";
				string phoneticSymbol = "'" + datas [2].Replace ("'", "''") + "'";
				string sentenceEN = "'" + datas [3].Replace ("'", "''").Replace('+',',') + "'";
				string sentenceCH = "'" + datas [4].Replace ("'", "''").Replace('+',',') + "'";
				string pronounciationURL = "'" + datas [5] + "'";

				string wordLength = (spell.Length-2).ToString();

				string[] wordInput = new string[] {wordId, spell, phoneticSymbol, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, "0", "0","0"};

				sql.InsertValues (wordsTableName, wordInput);

			}


			sql.EndTransaction ();

		}
			

		private string ExplainationNormalization(string oriExplaination){

			oriExplaination = oriExplaination.Replace ('/', ',').Replace('+',',');

			oriExplaination = oriExplaination.Replace ("prep.", "<color=orange>prep.</color>");

			oriExplaination = oriExplaination.Replace ("n.", "<color=orange>n.</color>");

			oriExplaination = oriExplaination.Replace ("adj.", "<color=orange>adj.</color>");

			oriExplaination = oriExplaination.Replace ("adv.", "<color=orange>adv.</color>");

			oriExplaination = oriExplaination.Replace ("v.", "<color=orange>v.</color>");

			oriExplaination = oriExplaination.Replace ("vt.", "<color=orange>vt.</color>");

			oriExplaination = oriExplaination.Replace ("vi.", "<color=orange>vi.</color>");

			oriExplaination = oriExplaination.Replace ("pron.", "<color=orange>pron.</color>");

			oriExplaination = oriExplaination.Replace ("num.", "<color=orange>num.</color>");

			oriExplaination = oriExplaination.Replace ("int.", "<color=orange>int.</color>");

			oriExplaination = oriExplaination.Replace ("conj.", "<color=orange>conj.</color>");

			oriExplaination = oriExplaination.Replace ("art.", "<color=orange>art.</color>");

			oriExplaination = oriExplaination.Replace ("abbr.", "<color=orange>abbr.</color>");

			oriExplaination = oriExplaination.Replace ("aux.", "<color=orange>aux.</color>");

			return oriExplaination;
		}

	}

}
