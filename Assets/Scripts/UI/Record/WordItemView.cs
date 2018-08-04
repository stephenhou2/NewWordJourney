using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class WordItemView : CellDetailView {

		public Text spellText;
		public Text explainationText;
		public Text phoneticSymbolText;
        
		private HLHWord word;
		private CallBackWithWord changeStatusButtonClickCallBack;
		private CallBack showWordDetailCallBack;
		//private InstancePool wordPool;
        
		public void InitWordItemView(CallBackWithWord changeStatusButtonClickCallBack, CallBack showWordDetailCallBack, InstancePool wordPool){
			this.changeStatusButtonClickCallBack = changeStatusButtonClickCallBack;
			this.showWordDetailCallBack = showWordDetailCallBack;
			//this.wordPool = wordPool;
		}

		public override void SetUpCellDetailView (object data)
		{
			this.word = data as HLHWord;

			spellText.text = word.spell;

			explainationText.text = word.explaination;

			phoneticSymbolText.text = word.phoneticSymbol;

		}



		public void OnWordDetailButtonClick(){
			if(showWordDetailCallBack != null){
				showWordDetailCallBack();
			}
		}

		public void OnChangeStatusButtonClick(){         
			changeStatusButtonClickCallBack(word);
		}


	}
}
