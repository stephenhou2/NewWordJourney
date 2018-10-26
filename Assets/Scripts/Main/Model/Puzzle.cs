using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	/// <summary>
    /// 谜语数据模型
    /// </summary>
	[System.Serializable]
	public class Puzzle
    {
		// 谜语id
		public int puzzleId;
        // 谜题
		public string question;
        // 正确回答
		public string answer;
        // 混淆项1
		public string confusion_1;
        // 混淆项2
		public string confusion_2;
        // 构造函数
		public Puzzle(int puzzleId,string question,string answer,string confusion_1,string confusion_2){
			this.puzzleId = puzzleId;
			this.question = question;
			this.answer = answer;
			this.confusion_1 = confusion_1;
			this.confusion_2 = confusion_2;
		}
       
    }
	
}

