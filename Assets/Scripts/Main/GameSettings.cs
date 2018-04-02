using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

	using System.IO;

	[System.Serializable]
	public class GameSettings {

		public enum LearnMode
		{
			Test,
			Learn
		}

		public bool isAutoPronounce = false;

		public float systemVolume = 0.5f;

		public WordType wordType = WordType.NMET;

		public LearnMode learnMode;

		public string GetWordTypeString(){

			string wordTypeString = "";

			switch (wordType) {
			case WordType.CET46:
				wordTypeString = "大学英语四级六级";
				break;
			case WordType.NMET:
				wordTypeString = "高中英语";
				break;
			case WordType.GRE:
				wordTypeString = "GRE";
				break;
			case WordType.TOEFL:
				wordTypeString = "TOEFL";
				break;
			}

			return wordTypeString;


		}


		public override string ToString ()
		{
			return string.Format ("[GameSettings]-isAutoPronounce{0},systemVolume{3},wordType{4}",isAutoPronounce,systemVolume,wordType);
		}


	}
}
