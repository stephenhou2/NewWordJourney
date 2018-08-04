using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;



namespace WordJourney
{
	
//	[System.Serializable]
	public class LearningInfo:Singleton<LearningInfo> {

		// 完成过多少次单词学习过程
//		public int totalLearnTimeCount;

		/// <summary>
		/// 空构造函数
		/// </summary>
		private LearningInfo()
		{
			
		}


		// 当前单词类型下所有单词的数量
		public int totalWordCount{
			get {
				return GetTotalWordsCount();
			}
		}

		// 当前单词类型下所有已学习过的单词数量
		public int learnedWordCount{
			get{
				return GetCurrentLearnedWordsCount();
			}
		}

		// 当前单词类型下所有背错过的单词数量
		public int ungraspedWordCount{
			get{
				return GetWrongWordsCount();
			}
		}

		// 当前学习的单词类型
		public WordType currentWordType{
			get{
				return GameManager.Instance.gameDataCenter.gameSettings.wordType;
			}
		}
        




		public string GetCurrentLearningWordsTabelName(){

			string tableName = string.Empty;

			switch (currentWordType) {
			case WordType.Simple:
				tableName = CommonData.simpleWordsTable;
				break;
			case WordType.Medium:
				tableName = CommonData.mediumWordsTabel;
				break;
			case WordType.Master:
				tableName = CommonData.masterWordsTabel;
				break;
			}

			return tableName;
		}

		private int GetTotalWordsCount(){

			string tableName = GetCurrentLearningWordsTabelName ();

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			// 连接数据库
			sql.GetConnectionWith (CommonData.dataBaseName);

			// 检查存放指定单词类型的表是否存在（目前只做了测试用的CET4这一个表，添加表使用参考editor文件夹下的DataBaseManager）
			if (!sql.CheckTableExist (tableName)) {
				sql.CloseConnection(CommonData.dataBaseName);
				return 0;
			}
				
			// 查询当前学习的单词类型中的所有单词数量
			int count = sql.GetItemCountOfTable (tableName,null,true);

			sql.CloseConnection(CommonData.dataBaseName);

			return count;

		}

		private int GetCurrentLearnedWordsCount(){

			string tableName = GetCurrentLearningWordsTabelName ();

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			// 连接数据库
			sql.GetConnectionWith (CommonData.dataBaseName);

			// 检查存放指定单词类型的表是否存在（目前只做了测试用的CET4这一个表，添加表使用参考editor文件夹下的DataBaseManager）
			if (!sql.CheckTableExist (tableName)) {
				sql.CloseConnection(CommonData.dataBaseName);
				return 0;
			}

			// 查询当前学习的单词类型中的所有单词数量
			string[] learnedCondition = {"learnedTimes>0"};
			int count = sql.GetItemCountOfTable (tableName, learnedCondition, true);

			sql.CloseConnection(CommonData.dataBaseName);

			return count;

		}

		private int GetWrongWordsCount(){

			string tableName = GetCurrentLearningWordsTabelName ();

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			// 连接数据库
			sql.GetConnectionWith (CommonData.dataBaseName);

			// 检查存放指定单词类型的表是否存在（目前只做了测试用的CET4这一个表，添加表使用参考editor文件夹下的DataBaseManager）
			if (!sql.CheckTableExist (tableName)) {
				sql.CloseConnection(CommonData.dataBaseName);
				return 0;
			}
                     
			// 查询当前学习的单词类型中所有背错过的单词数量
			string[] ungraspedCondition = {"ungraspTimes>=1"};
			int count = sql.GetItemCountOfTable (tableName, ungraspedCondition,true);

			sql.CloseConnection(CommonData.dataBaseName);

			return count;

		}
			


		public List<HLHWord> GetAllLearnedWords(){

			List<HLHWord> learnedWords = new List<HLHWord>();

			string tableName = GetCurrentLearningWordsTabelName ();

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			// 连接数据库
			sql.GetConnectionWith (CommonData.dataBaseName);

			string[] ungraspedCondition = {"learnedTimes>0"};

			// 读取器
			IDataReader reader = sql.ReadSpecificRowsOfTable(tableName,null,ungraspedCondition,true);

			// 从表中读取数据
			while (reader.Read ()) {

				if (reader == null) {
					sql.CloseAllConnections();
					return null;
				}

				int wordId = reader.GetInt32 (0);

				string spell = reader.GetString (1);

				string phoneticSymble = reader.GetString (2);

				string explaination = reader.GetString (3);

				string sentenceEN = reader.GetString (4);

				string sentenceCH = reader.GetString (5);

				string pronounciationURL = reader.GetString (6);

				int wordLength = reader.GetInt16 (7);

				int learnedTimes = reader.GetInt16 (8);

				int ungraspTimes = reader.GetInt16 (9);

				bool isFamiliar = reader.GetInt16(10) == 1;

                HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes, isFamiliar);


				learnedWords.Add (word);

			}

			sql.CloseAllConnections ();

			return learnedWords;



		}

		public List<HLHWord> GetAllGraspedWord(){

			List<HLHWord> graspedWords = new List<HLHWord>();

			string tableName = GetCurrentLearningWordsTabelName();

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			sql.GetConnectionWith(CommonData.dataBaseName);

			string[] graspedCondition = { "learnedTimes>ungraspTimes" };

			IDataReader reader = sql.ReadSpecificRowsOfTable(tableName, null, graspedCondition, true);

			while(reader.Read()){

				if (reader == null)
                {
					sql.CloseConnection(CommonData.dataBaseName);
                    return null;
                }

                int wordId = reader.GetInt32(0);

                string spell = reader.GetString(1);

                string phoneticSymble = reader.GetString(2);

                string explaination = reader.GetString(3);

                string sentenceEN = reader.GetString(4);

                string sentenceCH = reader.GetString(5);

                string pronounciationURL = reader.GetString(6);

                int wordLength = reader.GetInt16(7);

                int learnedTimes = reader.GetInt16(8);

                int ungraspTimes = reader.GetInt16(9);

				bool isFamiliar = reader.GetInt16(10) == 1;

                HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes, isFamiliar);


				graspedWords.Add(word);

			}


			sql.CloseConnection(CommonData.dataBaseName);

			return graspedWords;

		}

        /// <summary>
        /// 获取所有熟悉的单词
        /// </summary>
        /// <returns>The all familiar word.</returns>
		public List<HLHWord> GetAllFamiliarWord()
        {

            List<HLHWord> familiarWords = new List<HLHWord>();

            string tableName = GetCurrentLearningWordsTabelName();

            MySQLiteHelper sql = MySQLiteHelper.Instance;

            sql.GetConnectionWith(CommonData.dataBaseName);

            string[] graspedCondition = { "learnedTimes>0","isFamiliar=1" };

            IDataReader reader = sql.ReadSpecificRowsOfTable(tableName, null, graspedCondition, true);

            while (reader.Read())
            {

                if (reader == null)
                {
					sql.CloseConnection(CommonData.dataBaseName);
                    return null;
                }

                int wordId = reader.GetInt32(0);

                string spell = reader.GetString(1);

                string phoneticSymble = reader.GetString(2);

                string explaination = reader.GetString(3);

                string sentenceEN = reader.GetString(4);

                string sentenceCH = reader.GetString(5);

                string pronounciationURL = reader.GetString(6);

                int wordLength = reader.GetInt16(7);

                int learnedTimes = reader.GetInt16(8);

                int ungraspTimes = reader.GetInt16(9);

				bool isFamiliar = reader.GetInt16(10) == 1;

                HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes, isFamiliar);


				familiarWords.Add(word);

            }


			sql.CloseConnection(CommonData.dataBaseName);

			return familiarWords;

        }

        /// <summary>
        /// 获取所有不熟悉的单词
        /// </summary>
        /// <returns>The all unfamiliar word.</returns>
		public List<HLHWord> GetAllUnfamiliarWord()
        {

            List<HLHWord> unfamiliarWords = new List<HLHWord>();

            string tableName = GetCurrentLearningWordsTabelName();

            MySQLiteHelper sql = MySQLiteHelper.Instance;

            sql.GetConnectionWith(CommonData.dataBaseName);

            string[] graspedCondition = { "learnedTimes>0", "isFamiliar=0" };

            IDataReader reader = sql.ReadSpecificRowsOfTable(tableName, null, graspedCondition, true);

            while (reader.Read())
            {

                if (reader == null)
                {
					sql.CloseConnection(CommonData.dataBaseName);
                    return null;
                }

                int wordId = reader.GetInt32(0);

                string spell = reader.GetString(1);

                string phoneticSymble = reader.GetString(2);

                string explaination = reader.GetString(3);

                string sentenceEN = reader.GetString(4);

                string sentenceCH = reader.GetString(5);

                string pronounciationURL = reader.GetString(6);

                int wordLength = reader.GetInt16(7);

                int learnedTimes = reader.GetInt16(8);

                int ungraspTimes = reader.GetInt16(9);

				bool isFamiliar = reader.GetInt16(10) == 1;

                HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes, isFamiliar);


				unfamiliarWords.Add(word);

            }


            sql.CloseAllConnections();

			return unfamiliarWords;

        }


		public List<HLHWord> GetAllUngraspedWords(){

			List<HLHWord> ungraspWords = new List<HLHWord>();

			string tableName = GetCurrentLearningWordsTabelName ();
            
			MySQLiteHelper sql = MySQLiteHelper.Instance;

			// 连接数据库
			sql.GetConnectionWith (CommonData.dataBaseName);

			string[] ungraspedCondition = {"ungraspTimes>=1"};

			// 读取器
			IDataReader reader = sql.ReadSpecificRowsOfTable(tableName,null,ungraspedCondition,true);

			// 从表中读取数据
			while (reader.Read ()) {

				if (reader == null) {
					sql.CloseConnection(CommonData.dataBaseName);
					return null;
				}

				int wordId = reader.GetInt32 (0);

				string spell = reader.GetString (1);

				string phoneticSymble = reader.GetString (2);

				string explaination = reader.GetString (3);

				string sentenceEN = reader.GetString (4);

				string sentenceCH = reader.GetString (5);

				string pronounciationURL = reader.GetString (6);

				int wordLength = reader.GetInt16 (7);

				int learnedTimes = reader.GetInt16 (8);

				int ungraspTimes = reader.GetInt16 (9);

				bool isFamiliar = reader.GetInt16(10) == 1;

                HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes, isFamiliar);


				ungraspWords.Add (word);

			}

			sql.CloseAllConnections ();

			return ungraspWords;

		}



	}

}