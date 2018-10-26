using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using System.Data;

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
        // 是否熟悉
		public bool isFamiliar;
        // 备用发音url
		public string backupProuounciationURL;

        // 构造函数
        public HLHWord(int wordId, string spell, string phoneticSymbol, string explaination, string sentenceEN, string sentenceCH, string pronounciationURL,
		               int wordLength, int learnedTimes, int ungraspTimes,bool isFamiliar,string backupPronounciationURL)
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
			this.isFamiliar = isFamiliar;
			this.backupProuounciationURL = backupPronounciationURL;
        }


		/// <summary>
        /// 从reader中读取单词数据，生成单词对象
        /// </summary>
        /// <returns>The word from reader.</returns>
        /// <param name="reader">Reader.</param>
        public static HLHWord GetWordFromReader(IDataReader reader)
        {

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

            string backupPronouncitaionURL = reader.GetString(11);

            HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL,
                                       wordLength, learnedTimes, ungraspTimes, isFamiliar, backupPronouncitaionURL);

            return word;
        }
              
        public override string ToString()
        {
            return string.Format("[LearnWord]---{0}:{1}", spell, explaination);
        }
    }


}
