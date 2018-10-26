using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{
	public class TransformManager:MonoBehaviour {

		// 按照给定的transform名称（带层级）查找，如果没有找到指定transform，则按照参数中的层级关系创建该transform
		public static Transform FindOrCreateTransform(string transformName){

            // 按‘/‘分割层级
			string[] strs = transformName.Split(new char[] {'/'});
			List<Transform> transList = new List<Transform> ();


			for (int i = 0; i < strs.Length; i++) {
				string hierarchy = null;
				for (int j = 0; j < i + 1; j++) {
					hierarchy += "/" + strs [j];
				}
				string mHierarchy = hierarchy.Substring (1);

				GameObject go = GameObject.Find (mHierarchy);

				if (go == null) {
					go = new GameObject ();
					go.name = strs [i];
				}
				transList.Add (go.transform);

				if (i != 0) {
					go.transform.SetParent (transList [i - 1]);
				}

			}

			return transList[transList.Count - 1];

		}
			
        /// <summary>
        /// 寻找场景中指定名称的游戏体
        /// </summary>
        /// <returns>The transform.</returns>
        /// <param name="transformName">Transform name.</param>
		public static Transform FindTransform (string transformName){

			GameObject go = GameObject.Find (transformName);

			if (go == null) {
				return null;
			}

			return go.transform;

		}

        /// <summary>
        /// 在父级物体上获取指定类型的组件
        /// </summary>
        /// <returns>The in parents.</returns>
        /// <param name="go">Go.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		static public T FindInParents<T>(GameObject go) where T : Component
		{
			if (go == null) return null;
			var comp = go.GetComponent<T>();

			if (comp != null)
				return comp;

			var t = go.transform.parent;
			while (t != null && comp == null)
			{
				comp = t.gameObject.GetComponent<T>();
				t = t.parent;
			}
			return comp;
		}

        /// <summary>
        /// 创建新的游戏体，放在指定的父级物体下，并重新命名为指定名称
        /// </summary>
        /// <returns>The transform.</returns>
        /// <param name="transformName">Transform name.</param>
        /// <param name="parentTrans">Parent trans.</param>
		public static Transform NewTransform(string transformName,Transform parentTrans = null){

			Transform mContainer = (new GameObject ()).transform;
			if (parentTrans != null) {
				mContainer.SetParent (parentTrans);
			}
			mContainer.name = transformName;
			return mContainer;
		}

      


        /// <summary>
        /// 销毁指定名称的游戏体
        /// </summary>
        /// <param name="transformName">Transform name.</param>
		public static void DestroyTransfromWithName(string transformName){

			Transform trans = FindTransform (transformName);


			if (trans == null) {
				return;
			}

			try{
				Destroy(trans.gameObject);
			}catch(System.Exception e){
				Debug.Log ("删除游戏物体失败" + e.ToString ());
			}

		}

	}
}
