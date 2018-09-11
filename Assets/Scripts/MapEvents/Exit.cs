using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public enum ExitType{
		NextLevel,
        LastLevel
	}

	public enum SealType{
		ReturnExit,
        Boss
	}

	public class Exit : TriggeredGear {

		private int direction;

		// 出口被封印的动画
		public Transform sealAnimReturn;

		public Transform sealAnimBoss;

		public bool isOpen;

		public ExitType exitType;


		public override void AddToPool(InstancePool pool){
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public void SetUpExitType(ExitType exitType){
			this.exitType = exitType;
			switch(exitType){
				case ExitType.LastLevel:
					sealAnimReturn.gameObject.SetActive(true);
					break;
				case ExitType.NextLevel:
					sealAnimReturn.gameObject.SetActive(false);
					break;
			}
		}


		public void SealExit(SealType sealType){

			isOpen = false;

			switch(sealType){
				case SealType.ReturnExit:
					sealAnimReturn.gameObject.SetActive(true);
					sealAnimBoss.gameObject.SetActive(false);
					break;
				case SealType.Boss:
					sealAnimReturn.gameObject.SetActive(false);
					sealAnimBoss.gameObject.SetActive(true);
					break;
			}         

		}

		public void OpenSeal(){
			
			isOpen = true;

			sealAnimReturn.gameObject.SetActive(false);

			sealAnimBoss.gameObject.SetActive(false);
                 

		}

		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			sealAnimBoss.gameObject.SetActive(false);
			sealAnimReturn.gameObject.SetActive(false);


			if(exitType == ExitType.NextLevel){
				direction = int.Parse(KVPair.GetPropertyStringWithKey("direction",attachedInfo.properties)); 
				switch (direction)
                {
                    case 0:
						sealAnimBoss.localRotation = Quaternion.Euler(Vector3.zero);
                        break;
                    case 1:
						sealAnimBoss.localRotation = Quaternion.Euler(new Vector3(0,0,180));
                        break;
                    case 2:
						sealAnimBoss.localRotation = Quaternion.Euler(new Vector3(0,0,-90));
                        break;
                    case 3:
						sealAnimBoss.localRotation = Quaternion.Euler(new Vector3(0,0,90));
                        break;
                }

			}
            

			isOpen = !HLHGameLevelData.IsBossLevel();
         
			bc2d.enabled = true;

			SetSortingOrder (-(int)transform.position.y);


		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (!isOpen) {
				ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD ("这里被一股奇怪的魔力封印着");    
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.exitAudioName);
				bp.isInEvent = false;
			} else {
				MapEventTriggered (true, bp);
			}

		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			if (exitType == ExitType.LastLevel)
            {
				ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD("返回上一层的出口已经被封印了");
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.exitAudioName);
				ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
                return;
            }
			ExploreManager.Instance.expUICtr.ShowEnterExitQueryHUD(exitType);
		}
        
		public override void ChangeStatus ()
		{
			isOpen = true;

		    OpenSeal();

			mapItemRenderer.sprite = null;
		}

	}
}
