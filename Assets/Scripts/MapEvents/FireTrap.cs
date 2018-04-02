using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class FireTrap : Trap {

		public int lifeLose;
		public int lifeLoseDuration;

		public Animator mapItemAnimator;

//		private MapGenerator mMapGenerator;
//		private MapGenerator mapGenerator{
//			get{
//				if (mMapGenerator == null) {
//					mMapGenerator = ExploreManager.Instance.GetComponent<MapGenerator> ();
//				}
//				return mMapGenerator;
//			}
//		}

		private IEnumerator lifeLoseCoroutine;


		public override bool IsPlayerNeedToStopWhenEntered ()
		{
			return false;
		}

		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			SetTrapOn ();
			SetSortingOrder (-(int)transform.position.y);
		}


		public override void AddToPool(InstancePool pool){
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public override void SetTrapOn ()
		{
			mapItemAnimator.SetBool ("Play",false);
			bc2d.enabled = true;
			isTrapOn = true;

//			int trapPosX = Mathf.RoundToInt (transform.position.x);
//			int trapPosY = Mathf.RoundToInt (transform.position.y);
//
//			if(trapPosX>= mapGenerator.mapWalkableInfoArray
			NewMapGenerator mapGenerator = ExploreManager.Instance.newMapGenerator;
			mapGenerator.mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 10;
		}

		public override void SetTrapOff ()
		{
			mapItemAnimator.SetBool ("Play",true);
			bc2d.enabled = false;
			isTrapOn = false;
			bc2d.enabled = false;
			NewMapGenerator mapGenerator = ExploreManager.Instance.newMapGenerator;
			mapGenerator.mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 1;

			
		}

		public override void OnTriggerEnter2D (Collider2D col)
		{
//			triggered = !triggered;

//			if (!isTrapOn) {
//				return;
//			}

		
			SoundManager.Instance.PlayAudioClip("MapEffects/" + audioClipName);

			BattleAgentController ba = col.GetComponent<BattleAgentController> ();


			if (lifeLoseCoroutine != null) {
				StopCoroutine (lifeLoseCoroutine);
			}

			lifeLoseCoroutine = LoseLifeContinous (ba);

			StartCoroutine (lifeLoseCoroutine);


		}




		private IEnumerator LoseLifeContinous(BattleAgentController target){

			float timer = 0;

			while (timer < lifeLoseDuration) {

//				target.propertyCalculator.InstantPropertyChange (target, PropertyType.Health, -lifeLose, false);

				target.AddHurtAndShow (lifeLose, HurtType.Physical,target.GetReversedTowards());

				yield return new WaitForSeconds (1);

				timer += 1;

			}

		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			throw new System.NotImplementedException ();
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			throw new System.NotImplementedException ();
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			throw new System.NotImplementedException ();
		}

		public override void ChangeStatus ()
		{
			throw new System.NotImplementedException ();
		}

	}
}
