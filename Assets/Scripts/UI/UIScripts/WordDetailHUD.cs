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

		public Text indexTint;

		public Text pronounceNotAvalableHintText;
              
        private CallBack quitCallBack;

		private int wordIndex;

		private List<HLHWord> wordRecords;
		private HLHWord currentWord;


        public void SetUpWordDetailHUD(List<HLHWord> wordRecords, CallBack quitCallBack){

			this.wordRecords = wordRecords;

			this.quitCallBack = quitCallBack;

			wordIndex = wordRecords.Count - 1;

			currentWord = wordRecords[wordIndex];

			SetUpWordDetailHUD(currentWord);

			indexTint.text = string.Format("{0}/{1}", wordRecords.Count, wordRecords.Count);

			ExploreManager.Instance.battlePlayerCtr.isInEvent = true;

			this.gameObject.SetActive(true);
            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

			if (GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce)
			{
				zoomCoroutine = HUDZoomIn(delegate{
					
					if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        pronounceNotAvalableHintText.enabled = true;

                    }
                    else
                    {
                        pronounceNotAvalableHintText.enabled = false;
						OnPronunceButtonClick();
                    }

				});
			}else{
				zoomCoroutine = HUDZoomIn();
			}
           
            StartCoroutine(zoomCoroutine);
            GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);



        }

		private void SetUpWordDetailHUD(HLHWord word){

			spellText.text = word.spell;
			phoneticSymbolText.text = word.phoneticSymbol;
			explainationText.text = word.explaination;
			sentenceEN.text = word.sentenceEN;
			sentenceCH.text = word.sentenceCH;

			if(GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce){
				OnPronunceButtonClick();
			}

         
		}

		public void OnNextWordButtonClick(){
			
			if (wordIndex >= wordRecords.Count - 1)
            {
                return;
            }
         
			wordIndex++;

			indexTint.text = string.Format("{0}/{1}", wordIndex + 1, wordRecords.Count);
         
			currentWord = wordRecords[wordIndex];

			SetUpWordDetailHUD(currentWord);      

		}

		public void OnLastWordButtonClick(){

			if(wordIndex == 0){
				return;
			}

			wordIndex--;

			indexTint.text = string.Format("{0}/{1}", wordIndex + 1, wordRecords.Count);

            currentWord = wordRecords[wordIndex];

            SetUpWordDetailHUD(currentWord); 

		}


        public void OnPronunceButtonClick(){
			
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return;
			}

			GameManager.Instance.pronounceManager.PronounceWord(currentWord);
           
        }

        public void QuitWordDetailHUD(){

			if (inZoomingOut)
            {
                return;
            }

            if(zoomCoroutine != null){
                StopCoroutine(zoomCoroutine);
            }         

            zoomCoroutine = HUDZoomOut();

            StartCoroutine(zoomCoroutine);

			pronounceNotAvalableHintText.enabled = false;

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;

            if(quitCallBack != null){
                quitCallBack();
            }

        }

      
    }
}
