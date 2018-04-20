using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class HLHRoleAnimInfo{

		public string roleAnimName;
		public int playTimes;
		public float roleAnimTime;
		public CallBack animEndCallback;

		public HLHRoleAnimInfo(string roleAnimName,int playTimes,float roleAnimTime,CallBack animEndCallback){
			this.roleAnimName = roleAnimName;
			this.playTimes = playTimes;
			this.roleAnimTime = roleAnimTime;
			this.animEndCallback = animEndCallback;
		}

	}
}
