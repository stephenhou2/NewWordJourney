using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{

    using System.Data;

    // 普通单词模型  
    [System.Serializable]
    public struct GeneralWord
    {

        // 单词id
        public int wordId;
        // 单词拼写
        public string spell;
        // 单词释义
        public string explaination;
        //		// 单词是否可用于制造融合石
        //		public bool valid;


        public GeneralWord(int wordId, string spell, string explaination)
        {
            this.wordId = wordId;
            this.spell = spell;
            this.explaination = explaination;

        }


    }

    // 普通单词模型  
    [System.Serializable]
    public class HLHWord
    {

        // 单词id
        public int wordId;
        // 单词拼写
        public string spell;
        // 单词音标
        public string phoneticSymbol;
        // 单词释义
        public string explaination;
        // 英文例句
        public string sentenceEN;
        // 中文例句
        public string sentenceCH;
        // 发音URL
        public string pronounciationURL;
        // 单词长度
        public int wordLength;
        // 单词已学次数
        public int learnedTimes;
        // 单词背错的次数
        public int ungraspTimes;


        public HLHWord(int wordId, string spell, string phoneticSymbol, string explaination, string sentenceEN, string sentenceCH, string pronounciationURL, int wordLength, int learnedTimes, int ungraspTimes)
        {
            this.wordId = wordId;
            this.spell = spell;
            this.phoneticSymbol = phoneticSymbol;
            this.explaination = explaination;
            this.sentenceEN = sentenceEN;
            this.sentenceCH = sentenceCH;
            this.pronounciationURL = pronounciationURL;
            this.wordLength = wordLength;
            this.learnedTimes = learnedTimes;
            this.ungraspTimes = ungraspTimes;
        }


        //		public static LearnWord RandomWord(){
        //
        //			LearningInfo learnInfo = LearningInfo.Instance;
        //
        //			int wordId = 0;
        //
        //			if (learnInfo.learnedWordCount != 0) {
        //
        //				wordId = Random.Range (0, learnInfo.learnedWordCount);
        //
        //				List<LearnWord> learnedWords = learnInfo.GetAllLearnedWords ();
        //
        //				return learnedWords [wordId];
        //
        //			}
        //
        //
        //			string tableName = string.Empty;
        //
        //			WordType wt = GameManager.Instance.gameDataCenter.gameSettings.wordType;
        //
        //			switch (wt) {
        //			case WordType.CET46:
        //				tableName = CommonData.CET46Table;
        //				break;
        //			case WordType.NMET:
        //				tableName = CommonData.NMETTable;
        //				break;
        //			case WordType.GRE:
        //				tableName = CommonData.GRETable;
        //				break;
        //			case WordType.TOEFL:
        //				tableName = CommonData.TOEFLTable;
        //				break;
        //			}
        //
        //			MySQLiteHelper sql = MySQLiteHelper.Instance;
        //
        //			// 连接数据库
        //			sql.GetConnectionWith (CommonData.dataBaseName);
        //
        //			int wordsCount = sql.GetItemCountOfTable (tableName,null,true);
        //
        //			wordId = Random.Range (0, wordsCount);
        //
        //			string[] conditions = new string[]{string.Format ("id={0}", wordId)};
        //
        //			IDataReader reader = sql.ReadSpecificRowsOfTable (tableName, null, conditions, true);
        //
        //			reader.Read ();
        //
        //			string spell = reader.GetString (1);
        //
        //			string explaination = reader.GetString (2);
        //
        //			string phoneticSymbol = reader.GetString (3);
        //
        //			string example = reader.GetString (4);
        //
        //			int learnedTimes = reader.GetInt16 (5);
        //
        //			int ungraspTimes = reader.GetInt16 (6);
        //
        //			return new LearnWord (wordId, spell, explaination, phoneticSymbol, example, learnedTimes, ungraspTimes);
        //
        //		}

        public override string ToString()
        {
            return string.Format("[LearnWord]---{0}:{1}", spell, explaination);
        }
    }


}
