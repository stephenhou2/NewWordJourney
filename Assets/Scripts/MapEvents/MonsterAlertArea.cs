using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DragonBones;
    /// <summary>
    /// 怪物的探测区域
    /// </summary>
	public class MonsterAlertArea : MonoBehaviour {

        // 地图怪物
		public MapEvent mapMonster;
      
        // 龙骨组件
		private UnityArmatureComponent alertAreaTint;
        // mesh
		private MeshRenderer mr;

        // 边界包围盒
		private EdgeCollider2D ec2D;

        /// <summary>
        /// 初始化探测区域
        /// </summary>
		public void InitializeAlertArea(){
			alertAreaTint = GetComponent<UnityArmatureComponent> ();
            mr = transform.Find("detect").GetComponent<MeshRenderer> ();
			ec2D = GetComponent<EdgeCollider2D> ();
			alertAreaTint.animation.timeScale = 0.2f;
			ec2D.enabled = false;
		}

        /// <summary>
        /// 显示探测区域
        /// </summary>
		public void ShowAlerAreaTint(){
			mr.enabled = true;
			alertAreaTint.enabled = true;
			alertAreaTint.animation.Play ("default", 0);
			ec2D.enabled = true;
		}

        /// <summary>
        /// 隐藏探测区域
        /// </summary>
		public void HideAlertAreaTint(){
			alertAreaTint.enabled = false;
			mr.enabled = false;
			ec2D.enabled = false;
		}

        /// <summary>
        /// disable探测功能
        /// </summary>
		public void DisableAlertDetect(){
			ec2D.enabled = false;
		}
			
        /// <summary>
        /// 包围盒检测到trigger进入
        /// </summary>
        /// <param name="col">Col.</param>
		public void OnTriggerEnter2D (Collider2D col)
		{
                 
			BattleAgentController ba = col.GetComponent<BattleAgentController> ();


			if (!(ba is BattlePlayerController)) {
				return;
			}

			//Debug.Log("detect player");

			BattlePlayerController bp = ba as BattlePlayerController;

			if (bp.isInEvent) {
				return;
			}

			//Debug.Log("player is not in event");

			if (bp.isInFight) {
				return;
			}
				
			//Debug.Log("player is not in figth");

			MapMonster mm = mapMonster as MapMonster;
			mm.isReadyToFight = true;

			mm.DetectPlayer (bp);
		}


	}
}
