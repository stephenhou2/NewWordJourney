using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney
{
	public abstract class BattleAgentUIController : MonoBehaviour {

		//血量槽
		public HLHFillBar healthBar;
	

		//战斗中文本模型
		public Transform exploreTextModel;

		// 文本缓存池
		public InstancePool exploreTextPool;

		// 文本在场景中的容器
		public Transform exploreTextContainer;

		public ExploreTextManager exploreTextManager;

		//public Transform statusTintContainer;
		//public Transform statusTintModel;
		//public InstancePool statusTintPool;


		public void InitExploreAgentView(){
			exploreTextManager.InitExploreTextManager (exploreTextPool, exploreTextModel, exploreTextContainer);
		}

		public abstract void UpdateAgentStatusPlane ();

		// 更新血量槽的动画（首次进入设置血量不播放动画，在ResetBattleAgentProperties（BattleAgent）后开启动画）
		protected void UpdateHealthBarAnim(Agent agent){
			healthBar.maxValue = agent.maxHealth;
			healthBar.value = agent.health;
		}
			
		//protected void UpdateSkillStatusPlane(Agent agent){

		//	statusTintPool.AddChildInstancesToPool (statusTintContainer);

		//	for (int i = 0; i < agent.allStatus.Count; i++) {
		//		string status = agent.allStatus [i];
		//		Image statusTint = statusTintPool.GetInstance<Image> (statusTintModel.gameObject, statusTintContainer);

		//		Sprite sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find (delegate(Sprite obj) {
		//			return obj.name == status;
		//		});

		//		statusTint.sprite = sprite;
		//		statusTint.enabled = sprite != null;
		//	}

		//}

		public virtual void PrepareForRefreshment(){

			exploreTextManager.AllExploreTextClearAndIntoPool ();
		}

//		// 动画管理方法，复杂回调单独写函数传入，简单回调使用拉姆达表达式
//		private void ManageAnimations(Tweener newTweener,CallBack tc){
//			allTweeners.Add (newTweener);
//
//			newTweener.OnComplete (
//				() => {
//					allTweeners.Remove (newTweener);
//					if (tc != null) {
//						tc ();
//					}
//				});
//
//		}

		public abstract void QuitFightPlane ();

	}
}
