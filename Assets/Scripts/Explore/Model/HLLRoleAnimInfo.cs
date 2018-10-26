using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
    /// 角色动画信息模型
    /// </summary>
	public class HLHRoleAnimInfo{

        // 角色动画名称
		public string roleAnimName;
        // 角色动画播放次数
		public int playTimes;
        // 角色动画已播放次数
		public float roleAnimTime;
        // 角色动画结束后的回调
		public CallBack animEndCallback;

        /// <summary>
        /// 构造函数
        /// </summary>
		public HLHRoleAnimInfo(string roleAnimName,int playTimes,float roleAnimTime,CallBack animEndCallback){
			this.roleAnimName = roleAnimName;
			this.playTimes = playTimes;
			this.roleAnimTime = roleAnimTime;
			this.animEndCallback = animEndCallback;
		}

		public override string ToString()
		{
			return string.Format("name:{0},playTimes:{1},time:{2}", roleAnimName, playTimes, roleAnimTime);
		}
	}
}
