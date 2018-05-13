using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DragonBones;

	public class EffectAnim : MonoBehaviour {

		public string effectName;

		public Vector3 localPos;

		public UnityArmatureComponent effectCom;

		private IEnumerator effectCouroutine;

        public void PlayAnim(string effectName,int playTime,CallBack cb,float yScaler){

            //transform.position = new Vector3(localPos.x, localPos.y * yScaler, localPos.z);

			effectCom.animation.Play (effectName, playTime);

			if (effectCouroutine != null) {
				StopCoroutine (effectCouroutine);
			}

			effectCouroutine = WaitEffectAnimEndAndCallback (cb);

			StartCoroutine (effectCouroutine);

		}

		private IEnumerator WaitEffectAnimEndAndCallback(CallBack cb){

			yield return new WaitUntil (() => effectCom.animation.isCompleted);

			if (cb != null) {
				cb ();
			}

			ExploreManager.Instance.newMapGenerator.AddEffectAnimToPool (this);
		}




	}
}
