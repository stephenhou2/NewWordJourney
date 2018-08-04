using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public class InstancePool: MonoBehaviour{

		public bool isDirty;

//		private List<GameObject> mInstancePool = new List<GameObject>();

//		public static InstancePool GetOrCreateInstancePool(string poolName,string parentName){
//
//			Transform trans = TransformManager.FindOrCreateTransform (parentName + "/" + poolName);
//
//			InstancePool instancePool = trans.GetComponent<InstancePool> ();
//
//			if (instancePool == null) {
//				instancePool = trans.gameObject.AddComponent<InstancePool> ();
//				instancePool.isDirty = false;
//			}else if(instancePool.isDirty){
//				GameObject pool = new GameObject ();
//				pool.name = poolName;
//				instancePool = pool.AddComponent<InstancePool> ();
//				instancePool.isDirty = false;
//			}
//			return instancePool;
//		}

		public T GetInstance<T>(GameObject instanceModel,Transform instanceParent)
			where T:Component
		{
			Transform mInstance = null;
			T target = null;

			if (this.isDirty) {
				mInstance = Instantiate (instanceModel,instanceParent).transform;
				ResetInstance (mInstance);
				mInstance.name = instanceModel.name;
				target = mInstance.GetComponent<T> ();
				return target;
			}
				
			try{
				if (this.transform.childCount > 0) {
					mInstance = this.transform.GetChild (0);
					if(mInstance != null){
						mInstance.SetParent (instanceParent,false);
						ResetInstance (mInstance);
					}
				} else{
					mInstance = Instantiate (instanceModel,instanceParent).transform;
					ResetInstance (mInstance);
					mInstance.name = instanceModel.name;
				}
			}catch(System.Exception e){
				Debug.LogError (e);
			}
				
			mInstance.gameObject.SetActive(true);	

			target = mInstance.GetComponent<T> ();

			if (target == null) {
				Debug.LogError ("获取游戏组件失败");
			}

			return target;
		}

		public T GetInstanceWithName<T>(string goName, GameObject instanceModel,Transform instanceParent)
			where T:Component
		{
			Transform mInstance = null;

			if (this.transform.childCount > 0) {

				for (int i = 0; i < this.transform.childCount; i++) {
					Transform t = this.transform.GetChild (i);
					if (t != null && t.name == goName) {
						mInstance = t;
						break;
					}
				}

				if (mInstance == null) {
					mInstance = Instantiate (instanceModel,instanceParent).transform;
					ResetInstance (mInstance);
					mInstance.name = instanceModel.name;
				}

				mInstance.transform.SetParent (instanceParent,false);

				ResetInstance (mInstance);
			} else {
				mInstance = Instantiate (instanceModel,instanceParent).transform;
				ResetInstance (mInstance);
				mInstance.name = instanceModel.name;
			}

			mInstance.gameObject.SetActive(true);

			return mInstance.GetComponent<T>();

		}

		public T GetInstanceWithName<T>(string goName)
			where T:Component
		{

			Transform mInstance = null;

			if (this.transform.childCount > 0) {

				for (int i = 0; i < this.transform.childCount; i++) {
					Transform t = this.transform.GetChild (i);
					if (t != null && t.name == goName) {
						mInstance = t;
						break;
					}
				}
			} 

			if (mInstance == null) {
				return null;
			}

			mInstance.gameObject.SetActive(true);

			return mInstance.GetComponent<T>();
		}

		public void AddChildInstancesToPool(Transform originalParent){

			int counter = 0;

			while(originalParent.childCount>0){

				GameObject instance = originalParent.GetChild (0).gameObject;

				instance.transform.SetParent (this.transform);
            
				instance.SetActive(false);
            
				instance.transform.localPosition = Vector3.zero;

				counter++;

//				if (counter > 500) {
//					Debug.LogError ("缓存池死循环");
//					return;
//				}
//
//				mInstancePool.Add (instance);
			}

			originalParent.DetachChildren ();

		}

		public void ClearInstancePool(){
			for(int i = 0;i<transform.childCount;i++){
				GameObject instance = transform.GetChild (i).gameObject;
//				mInstancePool.Remove (instance);
				Destroy (instance,0.3f);
			}
			transform.DetachChildren ();
		}

		public void AddInstanceToPool(GameObject instance){         

			instance.transform.SetParent (this.transform);
			instance.transform.localPosition = Vector3.zero;

			instance.gameObject.SetActive(false);
		}

		private void ResetInstance(Transform instance){

			instance.localRotation = Quaternion.identity;
			instance.localScale = Vector3.one;
			instance.gameObject.SetActive(false);

		}

		void OnDestroy(){
			isDirty = true;
		}

	}
}
