using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;



namespace WordJourney
{
	
//	[System.Serializable]
	public class LearningInfo:Singleton<LearningInfo> {


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
        


        
        /// <summary>
        /// 获取当前学习中的单词词库表名称
        /// </summary>
        /// <returns>The current learning words tabel name.</returns>
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

        /// <summary>
        /// 获取当前词库的单词总数
        /// </summary>
        /// <returns>The total words count.</returns>
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

        /// <summary>
        /// 获取当前词库已学习单词总数
        /// </summary>
        /// <returns>The current learned words count.</returns>
		private int GetCurrentLearnedWordsCount(){

			string tableName = GetCurrentLearningWordsTabelName ();

			MySQLiteHelper sql = MySQLiteHelper.Instance;

			// 连接数据库
			sql.GetConnectionWith (CommonData.dataBaseName);
            
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

        /// <summary>
        /// 获取当前词库错误过的单词总数
        /// </summary>
        /// <returns>The wrong words count.</returns>
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
			

        /// <summary>
        /// 获取所有已学习过的单词
        /// </summary>
        /// <returns>The all learned words.</returns>
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

				HLHWord word = HLHWord.GetWordFromReader(reader);            

				learnedWords.Add (word);

			}

			sql.CloseAllConnections ();

			return learnedWords;



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

				HLHWord word = HLHWord.GetWordFromReader(reader);            

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

				HLHWord word = HLHWord.GetWordFromReader(reader);

				unfamiliarWords.Add(word);

            }


            sql.CloseAllConnections();

			return unfamiliarWords;

        }




	}

}