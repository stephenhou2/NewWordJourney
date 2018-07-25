using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney{

    using UnityEngine.UI;

    public class BillboardHUD : ZoomHUD {

        // 句子或诗歌英文文本
        public Text SAPEN;
        // 句子或诗歌中文文本
        public Text SAPCH;


        private CallBack quitCallBack;

        //private float offsetEN_CH = 50;


        public void SetUpBillboard(Billboard bb,CallBack quitCallBack){

            SAPEN.text = bb.sap.sapEN;

            SAPCH.text = bb.sap.sapCH;

            switch(bb.sap.sapType){
                case HLHSAPType.Sentence:
                    SAPEN.alignment = TextAnchor.MiddleLeft;
                    SAPCH.alignment = TextAnchor.MiddleLeft;
                    break;
                case HLHSAPType.Poem:
                    SAPEN.alignment = TextAnchor.MiddleCenter;
                    SAPCH.alignment = TextAnchor.MiddleCenter;
                    break;

            }

            this.quitCallBack = quitCallBack;

            this.gameObject.SetActive(true);

            if(zoomCoroutine != null){
                StopCoroutine(zoomCoroutine);
            }

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);
            zoomCoroutine = HUDZoomIn();

            StartCoroutine(zoomCoroutine);
        }

       

        public void QuitBillboard(){

			if (inZoomingOut)
            {
                return;
            }


            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);
         
            zoomCoroutine = HUDZoomOut();

            StartCoroutine(zoomCoroutine);

            if (quitCallBack != null)
            {
                quitCallBack();
            }

        }

    }

}

