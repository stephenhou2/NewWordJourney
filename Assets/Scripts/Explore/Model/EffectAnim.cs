using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DragonBones;

    // 特效动画类
	public class EffectAnim : MonoBehaviour {

        // 特效名称
		public string effectName;
        // 相对于父物体的位置信息
		public Vector3 localPos;
        // 播放次数
		public int playTime;
        // 播放保护
		public bool isProtectedBeforeEnd;
        // 龙骨控制器
		public UnityArmatureComponent effectCom;
        // 播放检测协程
		private IEnumerator effectCoroutine;

        /// <summary>
        /// 播放特效动画
        /// </summary>
        /// <param name="effectName">特效的动画名称</param>
        /// <param name="cb">动画结束回调.</param>
        /// <param name="yScaler">在y方向上的动画位置比例</param>
		/// <param name="playTime">播放次数【0:循环播放 其他：播放次数】.</param>
		/// <param name="duration">如果是循环播发，使用这个参数定义循环播放的总时长【0: 播放至战斗结束】.</param>
		public void PlayAnim(string effectName,CallBack cb,float yScaler,int playTime,float duration){

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

        /// <summary>
        /// 等待动画结束执行回调的协程
        /// </summary>
        /// <returns>The effect animation end and callback.</returns>
        /// <param name="cb">Cb.</param>
		private IEnumerator WaitEffectAnimEndAndCallback(CallBack cb){

			yield return new WaitUntil (() => effectCom.animation.isCompleted);

            // 执行动画结束的回调
			if (cb != null) {
				cb ();
			}
            // 回收动画
			ExploreManager.Instance.newMapGenerator.AddEffectAnimToPool (this);
		}

        // 等待动画播放一定时间后执行回调的协程
		private IEnumerator WaitForDurationAndCallBack(float duration, CallBack callBack){

			yield return new WaitForSeconds(duration);
			// 执行动画结束的回调
			if (callBack != null)
            {
				callBack();
            }
			// 回收动画
            ExploreManager.Instance.newMapGenerator.AddEffectAnimToPool(this);

		}


	}
}
