using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney{


    using UnityEngine.UI;

    public class WordDetailHUD : ZoomHUD
    {

        public Text spellText;
        public Text phoneticSymbolText;
        public Text explainationText;
        public Text sentenceEN;
        public Text sentenceCH;

        private HLHWord wordRecord;

        private CallBack quitCallBack;


        public void SetUpWordDetailHUD(HLHWord word, CallBack quitCallBack){

            this.wordRecord = word;

            spellText.text = word.spell;
            phoneticSymbolText.text = word.phoneticSymbol;
            explainationText.text = word.explaination;
            sentenceEN.text = word.sentenceEN;
            sentenceCH.text = word.sentenceCH;

            this.quitCallBack = quitCallBack;

            this.gameObject.SetActive(true);
            if(zoomCoroutine != null){
                StopCoroutine(zoomCoroutine);
            }
            zoomCoroutine = HUDZoomIn();
            StartCoroutine(zoomCoroutine);

        }

       
        public void OnPronunceButtonClick(){

            GameManager.Instance.pronounceManager.PronounceWord(wordRecord);
        }

        public void QuitWordDetailHUD(){

            if(zoomCoroutine != null){
                StopCoroutine(zoomCoroutine);
            }

            zoomCoroutine = HUDZoomOut();

            StartCoroutine(zoomCoroutine);

            if(quitCallBack != null){
                quitCallBack();
            }

        }

      
    }
}
