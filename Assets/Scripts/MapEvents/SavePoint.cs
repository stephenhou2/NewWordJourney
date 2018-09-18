using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DragonBones;
	using Transform = UnityEngine.Transform;

	public class SavePoint : MapEvent
	{
		public UnityArmatureComponent baseAnim;

		public UnityArmatureComponent lightAnim;

		//private string defaultAnimName = "default";
		//private string triggeredAnimName = "trigger";

		private IEnumerator waitAnimEndCoroutine;

		private IEnumerator waitPlayerToTargetPosCoroutine;

		//public BoxCollider2D bigCollider;

		public bool isInSavingData;

		public SavePointAutoDetect autoDetect;

		public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
		{
			this.transform.position = attachedInfo.position;
			isInSavingData = false;
			autoDetect.isInSavingData = false;
			autoDetect.InitSavePointAutoDetect(delegate
			{
				MapEventTriggered(false, ExploreManager.Instance.battlePlayerCtr);            
			});
		}

		public override void AddToPool(InstancePool pool)
		{
			pool.AddInstanceToPool(this.gameObject);
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			//bigCollider.enabled = false;

			if(bp.fadeStepsLeft == 0){
				return;
			}

			bp.isInEvent = false;

            if (!Player.mainPlayer.canSave)
            {
                return;
            }

            Debug.Log("save data at save point");

            ExploreManager.Instance.DisableAllInteractivity();

            isInSavingData = true;
            autoDetect.isInSavingData = true;

            Player.mainPlayer.savePosition = this.transform.position;

            Player.mainPlayer.saveTowards = bp.towards;

			bp.ForceMoveToAndStopWhenEnconterWithMapEvent(new Vector3(transform.position.x,transform.position.y,0),delegate {

                ExploreManager.Instance.SaveDataInExplore(delegate {
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.skillUpgradeAudioName);

                    ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD("数据已存档");

                    PlayTriggerAnim();

				});

               
            });

			//if (waitPlayerToTargetPosCoroutine != null)
			//{
			//	StopCoroutine(waitPlayerToTargetPosCoroutine);
			//}

			//waitPlayerToTargetPosCoroutine = WaitPlayerToTargetPos();

			//StartCoroutine(waitPlayerToTargetPosCoroutine);
		}

		//private IEnumerator WaitPlayerToTargetPos()
		//{

		//	BattlePlayerController battlePlayer = ExploreManager.Instance.battlePlayerCtr;

		//	yield return new WaitUntil(() => Mathf.Abs(battlePlayer.transform.position.x - transform.position.x) < 0.1f
		//							   && Mathf.Abs(battlePlayer.transform.position.y - transform.position.y) < 0.1f);

		//	isInSavingData = false;
		//	autoDetect.isInSavingData = false;

		//}

		public override bool IsPlayerNeedToStopWhenEntered()
		{
			return false;
		}

		public override void MapEventTriggered(bool longDealy, BattlePlayerController bp)
		{
			bp.isInEvent = false;

			if(!Player.mainPlayer.canSave){
				return;
			}
                     
			Debug.Log("save data at save point");

			ExploreManager.Instance.DisableAllInteractivity();
         
			isInSavingData = true;
			autoDetect.isInSavingData = true;

			Player.mainPlayer.savePosition = this.transform.position;

			Player.mainPlayer.saveTowards = bp.towards;

			bp.StopMoveAtEndOfCurrentStep(delegate {
			
				ExploreManager.Instance.SaveDataInExplore(delegate {
				
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.skillUpgradeAudioName);

                    ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD("数据已存档");

                    PlayTriggerAnim();
				});


			});



			//IEnumerator saveHintCoroutine = SaveHint(longDealy ? 0.5f : 0.1f);

			//StartCoroutine(saveHintCoroutine);

		}

		//private IEnumerator SaveHint(float delay)
		//{

		//	yield return new WaitForSeconds(delay);


		//}

		/// <summary>
		/// 等待动画完成后执行回调
		/// </summary>
		/// <returns>The call back at end of animation.</returns>
		/// <param name="cb">Cb.</param>
		protected IEnumerator ExcuteCallBackAtEndOfRoleAnim(CallBack cb)
		{
			yield return new WaitUntil(() => lightAnim.animation.isCompleted);
			cb();
		}


		public void PlayTriggerAnim(){
			
			lightAnim.gameObject.SetActive(true);

            lightAnim.animation.Play("default", 1);

            if (waitAnimEndCoroutine != null)
            {
                StopCoroutine(waitAnimEndCoroutine);
            }

            waitAnimEndCoroutine = ExcuteCallBackAtEndOfRoleAnim(delegate
            {
                lightAnim.gameObject.SetActive(false);
            });


            StartCoroutine(waitAnimEndCoroutine);
		}

	}


}

