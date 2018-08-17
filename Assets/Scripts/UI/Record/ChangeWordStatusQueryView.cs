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

			string wordListName = hasGrasped ? "已掌握单词" : "未掌握单词";

			string query = string.Format("是否确定将{0}移除出{1}列表？",word.spell,wordListName);

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
