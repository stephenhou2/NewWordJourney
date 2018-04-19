using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class ThornTrap : Trap {

		public int lifeLose;

		// 陷阱打开状态的图片
		public Sprite trapOnSprite;
		// 陷阱关闭状态的图片
		public Sprite trapOffSprite;

		private IEnumerator thornTrapTriggeredCoroutine;

		private Vector3 agentOriPos;
		private Vector3 backgroundOriPos;


//		private bool isTrapOn;

//		private Transform mExploreManager;
//		private Transform exploreManager{
//			get{
//				if (mExploreManager == null) {
//					mExploreManager = ExploreManager.Instance;
//				}
//				return mExploreManager;
//			}
//		}


		public override bool IsPlayerNeedToStopWhenEntered ()
		{
			return false;
		}



		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}



		public override void SetTrapOn ()
		{
//			mExploreManager.GetComponent<MapGenerator> ().mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 10;
			mapItemRenderer.sprite = trapOnSprite;
			isTrapOn = true;
		}

		public override void SetTrapOff ()
		{
//			mExploreManager.GetComponent<MapGenerator> ().mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 1;
			mapItemRenderer.sprite = trapOffSprite;
			isTrapOn = false;
		}


		public override void OnTriggerEnter2D (Collider2D col)
		{

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;

			if (!isTrapOn) {
				return;
			}

			GameManager.Instance.soundManager.PlayAudioClip("MapEffects/" + audioClipName);

			BattlePlayerController bp = col.GetComponent<BattlePlayerController> ();

			if (bp == null) {
				return;
			}

//			if(MyTool.ApproximatelySamePosition2D(bp.transform.position,this.transform.position)){

			agentOriPos = new Vector3 (Mathf.RoundToInt(col.transform.position.x), Mathf.RoundToInt(col.transform.position.y), 0);

//			backgroundOriPos = Camera.main.transform.Find ("Background").position;

//			}else{

//				agentOriPos = new Vector3 (Mathf.RoundToInt(col.transform.position.x), Mathf.RoundToInt(col.transform.position.y), 0);
//
//				backgroundOriPos = Camera.main.transform.Find ("Background").position;

//				Debug.Log (backgroundOriPos);

//			}

//			Debug.LogFormat ("oriPos:{0}----currentAgentPos:{1}", agentOriPos, col.transform.position);


			bp.StopMoveAndWait ();
			bp.singleMoveEndPos = agentOriPos;


//			bp.propertyCalculator.InstantPropertyChange (bp, PropertyType.Health, -lifeLose, false);
			bp.AddHurtAndShow (lifeLose, HurtType.Physical,bp.GetReversedTowards());

			if (thornTrapTriggeredCoroutine != null) {
				StopCoroutine (thornTrapTriggeredCoroutine);
			}

			thornTrapTriggeredCoroutine = ThornTrapTriggerEffect (bp);

			StartCoroutine (thornTrapTriggeredCoroutine);

		}



		private IEnumerator ThornTrapTriggerEffect(BattleAgentController ba){

			float timer = 0;

			float agentBackDuration = 0.1f;

//			Vector3 backgroundBackVector = (ba.transform.position - agentOriPos) * 0.2f;

			while (timer < agentBackDuration) {

				Vector3 agentBackVector = new Vector3 (
					(agentOriPos.x - ba.transform.position.x) / agentBackDuration,
					(agentOriPos.y - ba.transform.position.y) / agentBackDuration, 0) * Time.deltaTime;

				ba.transform.position += agentBackVector;
				timer += Time.deltaTime;
				yield return null;
			}

			ba.transform.position = agentOriPos;

//			Camera.main.transform.Find ("Background").position += backgroundBackVector;

//			Debug.Log (Camera.main.transform.Find ("Background").position);

		}


		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;
			bc2d.enabled = true;
			isTrapOn = bool.Parse (KVPair.GetPropertyStringWithKey ("isOpen", attachedInfo.properties));
			mapItemRenderer.sprite = isTrapOn ? trapOnSprite : trapOffSprite;
			SetSortingOrder (-Mathf.RoundToInt (attachedInfo.position.y));
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
		}

		public override void ChangeStatus ()
		{

			isTrapOn = !isTrapOn;

			mapItemRenderer.sprite = isTrapOn ? trapOnSprite : trapOffSprite;

			bc2d.enabled = isTrapOn;

			int posX = Mathf.RoundToInt (transform.position.x);

			int posY = Mathf.RoundToInt (transform.position.y);

			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [posX, posY] = isTrapOn ? 0 : 1;

		}

	}
}
