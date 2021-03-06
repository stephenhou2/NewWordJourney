﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    /// <summary>
    /// 陷阱类型
    /// </summary>
	public enum TrapType{
		Thorn,
        Fire,
        Poison
	}

    /// <summary>
    /// 陷阱类
    /// </summary>
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

        // 陷阱造成伤害的协程
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


        // 添加到缓存池中
		public override void AddToPool(InstancePool pool)
        {
            bc2d.enabled = false;
            pool.AddInstanceToPool(this.gameObject);
        }


        /// <summary>
        /// 当有trigger进入到包围盒中
        /// </summary>
        /// <param name="col">Col.</param>
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

        /// <summary>
        /// 尖刺陷阱延迟一段事件后将尖刺收回
        /// </summary>
        /// <returns>The trap off.</returns>
		private IEnumerator ThornTrapOff(){
			yield return new WaitForSeconds(0.5f);
			mapItemRenderer.sprite = thornTrapOffSprite;
		}
      
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="attachedInfo">Attached info.</param>
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

        /// <summary>
		/// 触发尖刺陷阱
        /// </summary>
        /// <param name="battlePlayer">Battle player.</param>
		private void ThornTrapTriggered(BattlePlayerController battlePlayer){
			mapItemRenderer.sprite = thornTrapOnSprite;
			MyTowards towards = battlePlayer.GetReversedTowards();
			battlePlayer.AddHurtAndShow(50, HurtType.Physical, towards);
			if (battlePlayer.agent.health <= 0)
            {
				//ExploreManager.Instance.DisableAllInteractivity();
				battlePlayer.AgentDie();
               
            }
			ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();
		}

        /// <summary>
        /// 触发火陷阱
        /// </summary>
        /// <returns>The trap triggered.</returns>
        /// <param name="battlePlayer">Battle player.</param>
		private IEnumerator FireTrapTriggered(BattlePlayerController battlePlayer){
			int count = 0;
			battlePlayer.SetEffectAnim(CommonData.burnedEffectName,null,0,3f);
			while(count < 3){

				yield return new WaitForSeconds(1f);

				yield return new WaitUntil(() => !battlePlayer.isInEvent);

				MyTowards towards = battlePlayer.GetReversedTowards();

				battlePlayer.AddHurtAndShow(50, HurtType.Physical, towards);
				ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();

				if(battlePlayer.agent.health<=0){
					//ExploreManager.Instance.DisableAllInteractivity();
					battlePlayer.AgentDie();
					yield break;
				}

				count++;
			}
		}



        /// <summary>
        /// 触发毒陷阱
        /// </summary>
        /// <returns>The trap triggered.</returns>
        /// <param name="battlePlayer">Battle player.</param>
		private IEnumerator PoisonTrapTriggered(BattlePlayerController battlePlayer){
			int count = 0;
			battlePlayer.SetEffectAnim(CommonData.poisonedEffectName,null,0,3f);
            while (count < 3)
            {
				yield return new WaitForSeconds(1f);

				yield return new WaitUntil(() => !battlePlayer.isInEvent);
            
                MyTowards towards = battlePlayer.GetReversedTowards();

                battlePlayer.AddHurtAndShow(100, HurtType.Physical, towards);
				ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();

				if (battlePlayer.agent.health <= 0)
                {
					//ExploreManager.Instance.DisableAllInteractivity();
					battlePlayer.AgentDie();
					yield break;
                }

                count++;
            }
		}

	}
}
