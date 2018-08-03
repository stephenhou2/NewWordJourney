using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	
	public class CharacterSmallDetect : MapEvent {


		//public BoxCollider2D boxCollider2D;

		private CallBack characterFlyCalBk;


		public void SetBoxColliderEnable(bool isEnable){
			bc2d.enabled = isEnable;
		}

		public void InitSmallDetect(CallBack characterFlyCalBk){
			this.characterFlyCalBk = characterFlyCalBk;
		}


		public override void AddToPool(InstancePool pool)
		{
			
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			bp.isInEvent = false;

			if (characterFlyCalBk != null)
            {
                characterFlyCalBk();
            }
		}

		public override void MapEventTriggered(bool isSuccess, BattlePlayerController bp)
		{
			
		}

		public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
		{
			
		}

	}

}
