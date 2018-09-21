using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public enum TrapType{
		Thorn,
        Fire,
        Poison
	}

	public class Trap : TriggeredGear {
                  
        // 陷阱打开状态的图片
        public Sprite thornTrapOnSprite;
        // 陷阱关闭状态的图片
        public Sprite thornTrapOffSprite;
        // 火焰陷阱图片
		public Sprite fireTrapSprite;
        // 毒雾陷阱图片
		public Sprite poisonTrapSprite;

        private TrapType trapType;

		private IEnumerator trapHurtCoroutine;

        public override bool IsPlayerNeedToStopWhenEntered()
        {
            return false;
        }

		public override void ChangeStatus()
		{
			SetTrapOff();
		}

        // 关闭陷阱
		public void SetTrapOff(){
			bc2d.enabled = false;
		}

		public override void AddToPool(InstancePool pool)
        {
            bc2d.enabled = false;
            pool.AddInstanceToPool(this.gameObject);
        }


        public void OnTriggerEnter2D(Collider2D col)
        {
            BattlePlayerController bp = col.GetComponent<BattlePlayerController>();

            if (bp == null)
            {
                return;
            }

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;

			MapEventTriggered(true, bp);

        }

		private IEnumerator ThornTrapOff(){
			yield return new WaitForSeconds(0.5f);
			mapItemRenderer.sprite = thornTrapOffSprite;
		}

		//public void OnTriggerExit2D(Collider2D collision)
		//{
		//	switch(trapType){
		//		case TrapType.Thorn:
		//			mapItemRenderer.sprite = thornTrapOffSprite;
		//			break;
		//		case TrapType.Fire:
		//		case TrapType.Poison:               
		//			break;
		//	}
		//}


		public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
        {
            transform.position = attachedInfo.position;
            bc2d.enabled = true;
			mapItemRenderer.enabled = true;

			//Debug.Log(attachedInfo.type);

			switch(attachedInfo.type){
				case "thornTrap":
					trapType = TrapType.Thorn;
					mapItemRenderer.sprite = thornTrapOffSprite;
					break;
				case "fireTrap":
					trapType = TrapType.Fire;
					mapItemRenderer.sprite = fireTrapSprite;
					break;
				case "poisonTrap":
					trapType = TrapType.Poison;
					mapItemRenderer.sprite = poisonTrapSprite;
					break;
			}
        }

        public override void EnterMapEvent(BattlePlayerController bp)
        {
			
			if(bp.fadeStepsLeft > 0){
				bp.isInEvent = false;            
			}
        }

        public override void MapEventTriggered(bool isSuccess, BattlePlayerController bp)
        {
			bp.isInEvent = false;

			if(bp.fadeStepsLeft > 0){
				return;
			}

			switch(trapType){
				case TrapType.Thorn:
					ThornTrapTriggered(bp);
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.thornTrapAudioName);
					IEnumerator trapOffCoroutine = ThornTrapOff();
					StartCoroutine(trapOffCoroutine);
					break;
				case TrapType.Fire:
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.fireTrapAudioName);
					if(trapHurtCoroutine != null){
						StopCoroutine(trapHurtCoroutine);
					}
					trapHurtCoroutine = FireTrapTriggered(bp);
					StartCoroutine(trapHurtCoroutine);
					break;
				case TrapType.Poison:
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.poisonTrapAudioName);
					if (trapHurtCoroutine != null)
                    {
                        StopCoroutine(trapHurtCoroutine);
                    }
					trapHurtCoroutine = PoisonTrapTriggered(bp);
                    StartCoroutine(trapHurtCoroutine);
					break;               
			}
        }


		private void ThornTrapTriggered(BattlePlayerController battlePlayer){
			mapItemRenderer.sprite = thornTrapOnSprite;
			MyTowards towards = battlePlayer.GetReversedTowards();
			battlePlayer.AddHurtAndShow(50, HurtType.Physical, towards);
			if (battlePlayer.agent.health <= 0)
            {
                battlePlayer.AgentDie();
            }
			ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();
		}

		private IEnumerator FireTrapTriggered(BattlePlayerController battlePlayer){
			int count = 0;
			battlePlayer.SetEffectAnim(CommonData.burnedEffectName,null,0,3f);
			while(count < 3){
				yield return new WaitForSeconds(1f);
				MyTowards towards = battlePlayer.GetReversedTowards();
				battlePlayer.AddHurtAndShow(50, HurtType.Physical, towards);
				if(battlePlayer.agent.health<=0){
					battlePlayer.AgentDie();
				}
				ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();
				count++;
			}
		}

		private IEnumerator PoisonTrapTriggered(BattlePlayerController battlePlayer){
			int count = 0;
			battlePlayer.SetEffectAnim(CommonData.poisonedEffectName,null,0,3f);
            while (count < 3)
            {
                yield return new WaitForSeconds(1f);
                MyTowards towards = battlePlayer.GetReversedTowards();
                battlePlayer.AddHurtAndShow(100, HurtType.Physical, towards);
				if (battlePlayer.agent.health <= 0)
                {
                    battlePlayer.AgentDie();
                }
				ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();
                count++;
            }
		}

	}
}
