using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SavePointAutoDetect : MonoBehaviour
    {

		public bool isInSavingData;

		private CallBack autoDetectCallBack;

		public void InitSavePointAutoDetect(CallBack autoDetectCallBack){
			this.autoDetectCallBack = autoDetectCallBack;
		}


		private void OnTriggerEnter2D(Collider2D collision)
        {

            if (isInSavingData)
            {
                return;
            }

            if (collision == null)
            {
                return;
            }

            BattlePlayerController bp = collision.GetComponent<BattlePlayerController>();

            if (bp == null)
            {
                return;
            }

            if (bp.isInPosFixAfterFight)
            {
                return;
            }

			if(!MyTool.ApproximatelySameIntPosition2D(bp.singleMoveEndPos,this.transform.position)){
				return;
			}


			if(Mathf.Abs(bp.transform.position.x - transform.position.x) < 0.1f && Mathf.Abs(bp.transform.position.y - transform.position.y) < 0.1f){
				return;
			}


			if(autoDetectCallBack != null){
				autoDetectCallBack();
			}

            isInSavingData = false;

        }
    }
}

