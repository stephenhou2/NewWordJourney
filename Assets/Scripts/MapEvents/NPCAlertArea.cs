using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DragonBones;

    /// <summary>
    /// npc探测区域【暂时没有用，npc暂时不做探测】
    /// </summary>
	public class NPCAlertArea : MonoBehaviour {


		public MapNPC mapNPC;

        private BoxCollider2D boxCollider2D;


		public void InitializeAlertArea(){

            boxCollider2D = GetComponent<BoxCollider2D> ();


            boxCollider2D.enabled = true;
		}
			

		public void EnableAlertDetect(){

            boxCollider2D.enabled = true;

		}

		public void DisableAlertDetect(){

            boxCollider2D.enabled = false;

		}

      

	}
}
