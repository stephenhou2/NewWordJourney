using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class ChangeWordStatusQueryView : MonoBehaviour
    {

		public Text queryText;

		private HLHWord word;

		private CallBackWithWord changeWordStatusCallBack;
              
		public void SetUpChangeWordStatusQueryHUD(HLHWord word, CallBackWithWord changeWordStatusCallBack){

			this.word = word;

			this.changeWordStatusCallBack = changeWordStatusCallBack;

			bool hasGrasped = word.isFamiliar;

			string wordListName = hasGrasped ? "不熟悉" : "熟悉";

			string query = string.Format("是否将单词\n\n<color=orange><size=70>{0}</size></color>\n\n移至<color=orange>{1}</color>单词列表？",word.spell,wordListName);

			queryText.text = query;

			this.gameObject.SetActive(true);
         
		}

		public void OnConfirmChangeWordStatus(){

			changeWordStatusCallBack(word);

			QuitChangeWordStatusQueryHUD();
		}

		public void OnCancelChangeWordStatus(){         
			QuitChangeWordStatusQueryHUD();
		}
        

		public void QuitChangeWordStatusQueryHUD(){         
			this.gameObject.SetActive(false);
		}
        

    }


}
