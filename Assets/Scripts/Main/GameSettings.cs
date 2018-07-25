using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

	[System.Serializable]
	public class GameSettings {

		public bool isAutoPronounce = true;

		public float systemVolume = 0.5f;

		public WordType wordType = WordType.Simple;

		public bool newPlayerGuideFinished;

		public string installDateString;
      
	}
}
