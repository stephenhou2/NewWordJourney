using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using TMPro;

	// 地图物品类型枚举
//	public enum MapItemType{
//		Buck,
//		Pot,
//		TreasureBox,
//		Tree,
//		Stone,
//		NormalTrapOn,
//		NormalTrapOff,
//		Switch,
//		Door,
//		MovableFloor,
//		Transport,
//		Billboard,
//		FireTrap,
//		Hole,
//		MovableBox,
//		LauncherTowardsUp,
//		LauncherTowardsDown,
//		LauncherTowardsLeft,
//		LauncherTowardsRight,
//		Plant,
//		PressSwitch,
//		Crystal,
//		MapNPC,
//		Docoration
//	}

//	public class MapEnglishTriggerInfo{
//		
//		public LearnWord targetWord;
//		public LearnWord confuseWord_1;
//		public LearnWord confuseWord_2;
//
//		public MapEnglishTriggerInfo(LearnWord[] wordsArray){
//			this.targetWord = wordsArray[0];
//			this.confuseWord_1 = confuseWord1;
//			this.confuseWord_2 = confuseWord2;
//		}
//	}

	public abstract class MapEvent : MonoBehaviour {

		public string audioClipName;

		protected SpriteRenderer mapItemRenderer;

		public TextMeshPro tmPro;

		protected CallBack animEndCallBack;

		protected BoxCollider2D bc2d;

		// 单词数组，第0项为显示的目标单词
		public HLHWord[] wordsArray;

		protected bool isWordTriggered{
			get{
				return wordsArray != null && wordsArray.Length > 0;
			}
		}

//		protected ExploreManager mExploreManager;
//		protected ExploreManager exploreManager{
//			get{
//				if (mExploreManager == null) {
//					mExploreManager = ExploreManager.Instance.GetComponent<ExploreManager> ();
//				}
//				return mExploreManager;
//			}
//		}

		protected virtual void Awake(){

			mapItemRenderer = GetComponent<SpriteRenderer> ();

			bc2d = GetComponent<BoxCollider2D> ();

			//Transform wordTrans = transform.Find ("Word");
			//if (wordTrans != null) {
			//	tmPro = wordTrans.GetComponent<TextMeshPro> ();
			//}
		}

//		public abstract void InitMapItem ();

		public abstract void AddToPool (InstancePool pool);

		public virtual void SetSortingOrder(int order){
			mapItemRenderer.sortingOrder = order;
		}

		void OnDestroy(){
			animEndCallBack = null;
		}

		public abstract void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo);


		/// <summary>
		/// 显示事件单词
		/// </summary>
		protected virtual void CheckIsWordTriggeredAndShow(){

			if (isWordTriggered && tmPro != null) {

				HLHWord targetWord = wordsArray [0];

				tmPro.text = targetWord.spell;

				tmPro.enabled = true;

			} else {
				
				tmPro.enabled = false;

			}

		}

		public virtual bool IsPlayerNeedToStopWhenEntered (){
			return true;
		}

		/// <summary>
		/// 遇到地图事件
		/// </summary>
		/// <param name="bp">Bp.</param>
		public abstract void EnterMapEvent(BattlePlayerController bp);


		/// <summary>
		/// 地图事件触发
		/// </summary>
		/// <param name="isSuccess">If set to <c>true</c> is success.</param>
		/// <param name="bp">Bp.</param>
		public abstract void MapEventTriggered (bool isSuccess,BattlePlayerController bp);

	}


}
