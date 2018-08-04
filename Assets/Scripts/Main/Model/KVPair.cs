using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
	/// 键值对扩展类，用于将json数据中的字典格式转换为对象格式
	/// </summary>
	[System.Serializable]
	public struct KVPair {
		
		public string key;
		public string value;

		public KVPair(string key,string value){
			this.key = key;
			this.value = value;
		}

		public static bool ContainsKey(string key,List<KVPair> KVPairs){
			
			bool result = false;

			for (int i = 0; i < KVPairs.Count; i++) {
				KVPair kv = KVPairs [i];
				if (kv.key.Equals (key)) {
					result = true;
					break;
				}
			}

			return result;

		}

		public static string GetPropertyStringWithKey(string key,List<KVPair> KVPairs){

			string value = null;

			for (int i = 0; i < KVPairs.Count; i++) {
				KVPair kv = KVPairs [i];
				if (kv.key.Equals (key)) {
					value = kv.value;
					break;
				}
			}

			return value;
		}
	}
}
