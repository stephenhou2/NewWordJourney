using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DragonBones;

	public class EffectAnim : MonoBehaviour {

		public string effectName;

		public Vector3 localPos;

		public int playTime;

		public bool isProtectedBeforeEnd;

		public UnityArmatureComponent effectCom;
        
		private IEnumerator effectCoroutine;

        /// <summary>
        /// Plaies the animation.
        /// </summary>
        /// <param name="effectName">特效的动画名称</param>
        /// <param name="cb">动画结束回调.</param>
        /// <param name="yScaler">在y方向上的动画位置比例</param>
		/// <param name="playTime">播放次数【0:循环播放 其他：播放次数】.</param>
		/// <param name="duration">如果是循环播发，使用这个参数定义循环播放的总时长【0: 播放至战斗结束】.</param>
		public void PlayAnim(string effectName,CallBack cb,float yScaler,int playTime,float duration){

			//transform.position = new Vector3(localPos.x, localPos.y * yScaler, localPos.z);

			this.playTime = playTime;

			effectCom.animation.Play (effectName, playTime);

			if (effectCoroutine != null) {
				StopCoroutine (effectCoroutine);
			}
			if(playTime > 0){
				effectCoroutine = WaitEffectAnimEndAndCallback(cb);
			}else if(playTime == 0 && duration > float.Epsilon){
				effectCoroutine = WaitForDurationAndCallBack(duration, cb);
			}

			if(effectCoroutine != null){
				StartCoroutine(effectCoroutine);
			}
		}

		private IEnumerator WaitEffectAnimEndAndCallback(CallBack cb){

			yield return new WaitUntil (() => effectCom.animation.isCompleted);

			if (cb != null) {
				cb ();
			}

			ExploreManager.Instance.newMapGenerator.AddEffectAnimToPool (this);
		}

        
		private IEnumerator WaitForDurationAndCallBack(float duration, CallBack callBack){

			yield return new WaitForSeconds(duration);

			if (callBack != null)
            {
				callBack();
            }

            ExploreManager.Instance.newMapGenerator.AddEffectAnimToPool(this);

		}


	}
}
