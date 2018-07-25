using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{

	using UnityEngine.UI;

	public delegate void CallBackWithInt(int arg);

	public class WordRecordDetailHUD : ZoomHUD
    {
        
		public Text spellText;

		public Text phoneticSymbolText;

		public Text explainationText;

		public Text sentenceENText;
        
		public Text sentenceCHText;
        
		private HLHWord word;

        // 单词在所在列表中的序号【已掌握单词列表和未掌握单词列表】
		private int wordIndexInList;

		//private CallBackWithWord changeWordStatusCallback;
		private CallBackWithInt nextWordButtonClickCallBack; 

		private CallBackWithInt lastWordButtonClickCallBack;

		public void InitWordRecordDetailHUD(CallBackWithInt nextWordButtonClickCallBack, CallBackWithInt lastWordButtonClickCallBack){

			this.nextWordButtonClickCallBack = nextWordButtonClickCallBack;

            this.lastWordButtonClickCallBack = lastWordButtonClickCallBack;

		}

		/// <summary>
        /// 更新单词详细页面【有页面缩放动画】
        /// </summary>
        /// <param name="word">Word.</param>
        /// <param name="wordIndexInList">Word index in list.</param>
		public void SetUpWordDetailHUD(HLHWord word,int wordIndexInList){

			this.word = word;

			this.wordIndexInList = wordIndexInList;

			spellText.text = word.spell;

			phoneticSymbolText.text = word.phoneticSymbol;

			explainationText.text = word.explaination;

			sentenceENText.text = word.sentenceEN;

			sentenceCHText.text = word.sentenceCH;

			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);
         
		}

        /// <summary>
        /// 更新单词详细页面【没有页面缩放动画】
        /// </summary>
        /// <param name="word">Word.</param>
        /// <param name="wordIndexInList">Word index in list.</param>
		public void UpdateWordDetailHUD(HLHWord word,int wordIndexInList){
			
			this.word = word;

            this.wordIndexInList = wordIndexInList;

            spellText.text = word.spell;

            phoneticSymbolText.text = word.phoneticSymbol;

            explainationText.text = word.explaination;

            sentenceENText.text = word.sentenceEN;

            sentenceCHText.text = word.sentenceCH;

            this.gameObject.SetActive(true);

		}

		public void OnPronunceButtonClick(){         
			GameManager.Instance.pronounceManager.PronounceWord(word);
		}
      

		public void OnNextWordButtonClick(){
			nextWordButtonClickCallBack(wordIndexInList);
		}

		public void OnLastWordButtonClick(){
			lastWordButtonClickCallBack(wordIndexInList);
		}

		public void QuitWordRecordDetailHUD(){
			
			if (inZoomingOut)
            {
                return;
            }


			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}
         
			zoomCoroutine = HUDZoomOut();

			StartCoroutine(zoomCoroutine);

		}


    }


}
