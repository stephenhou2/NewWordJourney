using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DragonBones;
	using Transform = UnityEngine.Transform;

	public class SavePoint : MapEvent
    {
		public UnityArmatureComponent savePointAnim;

		private string defaultAnimName = "default";
		private string triggeredAnimName = "trigger";

		private IEnumerator waitAnimEndCoroutine;

		private IEnumerator waitPlayerToTargetPosCoroutine;

		//public BoxCollider2D bigCollider;

		private bool isInSavingData;
          
		public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
        {
			this.transform.position = attachedInfo.position;
         
        }
      
		public override void AddToPool(InstancePool pool)
		{
			pool.AddInstanceToPool(this.gameObject);
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			//bigCollider.enabled = false;
			MapEventTriggered(true, bp);

			if(waitPlayerToTargetPosCoroutine != null){
				StopCoroutine(waitPlayerToTargetPosCoroutine);
			}

			waitPlayerToTargetPosCoroutine = WaitPlayerToTargetPos();

			StartCoroutine(waitPlayerToTargetPosCoroutine);
		}      
        
		private IEnumerator WaitPlayerToTargetPos(){

			BattlePlayerController battlePlayer = ExploreManager.Instance.battlePlayerCtr;

			yield return new WaitUntil(() => Mathf.Abs(battlePlayer.transform.position.x - transform.position.x) < 0.1f 
			                           && Mathf.Abs(battlePlayer.transform.position.y - transform.position.y) < 0.1f);

			isInSavingData = false;

		}

		public override bool IsPlayerNeedToStopWhenEntered()
		{
			return false;
		}

		public override void MapEventTriggered(bool longDealy, BattlePlayerController bp)
		{
			isInSavingData = true;

			Player.mainPlayer.savePosition = this.transform.position;

			Player.mainPlayer.saveTowards = bp.towards;
                     

			if (ExploreManager.Instance != null)
            {
                ExploreManager.Instance.UpdateWordDataBase();
            }

			GameManager.Instance.persistDataManager.SaveBuyRecord();
			GameManager.Instance.persistDataManager.SaveGameSettings();
			GameManager.Instance.persistDataManager.SaveMapEventsRecord();
			GameManager.Instance.persistDataManager.SaveCompletePlayerData();
			GameManager.Instance.persistDataManager.SaveMiniMapRecords();
			GameManager.Instance.persistDataManager.SaveCurrentMapEventsRecords();

            MySQLiteHelper.Instance.CloseAllConnections();

			bp.isInEvent = false;

			IEnumerator saveHintCoroutine = SaveHint(longDealy ? 0.5f : 0.1f);

			StartCoroutine(saveHintCoroutine);

		}

		private IEnumerator SaveHint(float delay){

			yield return new WaitForSeconds(delay);

			ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD("数据已存档");

			savePointAnim.animation.Play(triggeredAnimName, 1);

			if(waitAnimEndCoroutine != null){
				StopCoroutine(waitAnimEndCoroutine);
			}

			waitAnimEndCoroutine = ExcuteCallBackAtEndOfRoleAnim(delegate
			{            
				savePointAnim.animation.Play(defaultAnimName, 0);
			});


			StartCoroutine(waitAnimEndCoroutine);         
		}
        
		/// <summary>
        /// 等待动画完成后执行回调
        /// </summary>
        /// <returns>The call back at end of animation.</returns>
        /// <param name="cb">Cb.</param>
        protected IEnumerator ExcuteCallBackAtEndOfRoleAnim(CallBack cb)
        {
			yield return new WaitUntil(() => savePointAnim.animation.isCompleted);
            cb();
        }


		private void OnTriggerEnter2D(Collider2D collision)
		{
         
			if(isInSavingData){
				return;
			}

			if(collision == null){
				return;
			}

			BattlePlayerController bp = collision.GetComponent<BattlePlayerController>();

			if(bp == null){
				return;
			}


			MapEventTriggered(false, bp);

			isInSavingData = false;

		}



	}

}

