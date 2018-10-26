using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{
    
    /// <summary>
    /// 游戏设置数据模型
    /// </summary>
	[System.Serializable]
	public class GameSettings {
        // 自动发音【默认开启】
		public bool isAutoPronounce = true;
        // 系统音量
		public float systemVolume = 0.5f;
        // 单词类型【默认简单】
		public WordType wordType = WordType.Simple;
        // 标记是否已经看过了新手引导
		public bool newPlayerGuideFinished;
        // 安装日期/更新日期
		public string installDateString;
      
	}
}
