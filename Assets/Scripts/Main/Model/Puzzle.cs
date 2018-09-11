using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	[System.Serializable]
	public class Puzzle
    {
		public int puzzleId;

		public string question;

		public string answer;

		public string confusion_1;

		public string confusion_2;

		public Puzzle(int puzzleId,string question,string answer,string confusion_1,string confusion_2){
			this.puzzleId = puzzleId;
			this.question = question;
			this.answer = answer;
			this.confusion_1 = confusion_1;
			this.confusion_2 = confusion_2;
		}
       
    }
	
}

