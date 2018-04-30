using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney{

    using UnityEngine.UI;

    public class BillboardHUD : ZoomHUD {

        public Text proverbEN;
        public Text proverbCH;

        private CallBack quitCallBack;

        public void SetUpBillboard(Billboard bb,CallBack quitCallBack){

            proverbEN.text = bb.proverb.proverbEN;

            proverbCH.text = bb.proverb.proverbCH;

            this.quitCallBack = quitCallBack;

            this.gameObject.SetActive(true);

            if(zoomCoroutine != null){
                StopCoroutine(zoomCoroutine);
            }

            zoomCoroutine = HUDZoomIn();

            StartCoroutine(zoomCoroutine);
        }

        public void QuitBillboard(){

            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

            zoomCoroutine = HUDZoomOut();

            StartCoroutine(zoomCoroutine);

            if (quitCallBack != null)
            {
                quitCallBack();
            }

        }

    }

}

