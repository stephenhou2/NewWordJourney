using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class KeyDoorOperatorView : MonoBehaviour
    {

		public ToolButton toolButtonModel;

		public Transform toolButtonContainer;

		public InstancePool toolButtonPool;

		public Transform toolSelectPlane;
        
		private CallBackWithWord unlockSuccessCallBack;
        
		private CallBack unlockFailCallBack;
              
		public Transform unlockOperationPlane;

		public Image keyIcon;

		public Sprite ironKeySprite;
		public Sprite brassKeySprite;
		public Sprite goldKeySprite;

		public Image lockIcon;

		public Sprite lockOnSprite;

		public Sprite lockOffSprite;

		public Text hintText;

		public KeyDoorCharacterButton[] characterButtons;
      
		private List<int> filledIndex = new List<int>();

		public KeyDoorCharacterFill keyDoorCharacterModel;

		public InstancePool keyDoorCharacterPool;

		public Transform keyDoorCharacterContainer;

		private List<int> hintIndexList = new List<int>();

		private HLHWord keyDoorWord;

		private List<KeyDoorCharacterFill> keyDoorCharacterFillList = new List<KeyDoorCharacterFill>();

		public Image keyDoorOperationMask;

		public Transform queryQuitHUD;

		public void SetUpKeyDoorOperatorView(List<SpecialItem> keys, HLHWord keyDoorWord, CallBackWithWord unlockSuccessCallBack,CallBack unlockFailCallBack){

			this.unlockSuccessCallBack = unlockSuccessCallBack;

			this.unlockFailCallBack = unlockFailCallBack;

			this.keyDoorWord = keyDoorWord;
         
			SetUpKeysSelect(keys);

			lockIcon.sprite = lockOnSprite;


			this.gameObject.SetActive(true);

			toolSelectPlane.gameObject.SetActive(true);

			unlockOperationPlane.gameObject.SetActive(false);

			keyDoorOperationMask.gameObject.SetActive(false);

			queryQuitHUD.gameObject.SetActive(false);

			SetUpCharacterButtons(keyDoorWord);

            SetUpKeyDoorCharacterFill(keyDoorWord);
         
			ExploreManager.Instance.MapWalkableEventsStopAction();

		}
        
		private void ToolSelectCallBack(Item tool){

			SpecialItem key = tool as SpecialItem;

	
			bool needEnterUnlockOperator = true;

			hintIndexList.Clear();
			int hintCount = 0;
			List<int> allIndex = new List<int>();

			for (int i = 0; i < keyDoorWord.wordLength;i++){
				allIndex.Add(i);
			}

			switch(key.specialItemType){
				case SpecialItemType.TieYaoShi:               
					hintCount = 1;
					keyIcon.sprite = ironKeySprite;
					hintText.text = "铁钥匙可提示一个字母";
					break;
				case SpecialItemType.TongYaoShi:               
					hintCount = 2;
					keyIcon.sprite = brassKeySprite;
					hintText.text = "铜钥匙可提示两个字母";
					break;
				case SpecialItemType.JinYaoShi:
					keyIcon.sprite = goldKeySprite;
					hintText.text = "金钥匙可提示三个字母";
					hintCount = 3;
					break;
				case SpecialItemType.WanNengYaoShi:
					unlockSuccessCallBack(null);
					needEnterUnlockOperator = false;
					QuitKeyDoorOperatorView();
					break;
				case SpecialItemType.QiaoZhen:
					unlockSuccessCallBack(null);
					needEnterUnlockOperator = false;
					QuitKeyDoorOperatorView();
					break;
			}

			Player.mainPlayer.RemoveItem(key, 1);

			if(needEnterUnlockOperator)
			{
				toolButtonPool.AddChildInstancesToPool(toolButtonContainer);

				char[] characters = keyDoorWord.spell.ToCharArray();

				for (int i = 0; i < hintCount;i++){

					int randomSeed = Random.Range(0, allIndex.Count);

					int index = allIndex[randomSeed];

					hintIndexList.Add(index);

					char character = characters[index];

					SetCharacterAsHint(character,index);

					allIndex.RemoveAt(randomSeed);

				}

				unlockOperationPlane.gameObject.SetActive(true);
				toolSelectPlane.gameObject.SetActive(false);
			} 
		}

		private void SetCharacterAsHint(char character,int index){

			for (int i = 0; i < characterButtons.Length;i++){
                
				KeyDoorCharacterButton characterButton = characterButtons[i];

				if(characterButton.character == character && characterButton.characterButton.interactable){

					characterButton.SetToHint();

					break;
				}            
			}

         
			KeyDoorCharacterFill keyDoorCharacterFill = keyDoorCharacterFillList[index];

			keyDoorCharacterFill.SetUpKeyDoorCharacterFill(character, true);


		}
        
		private void SetUpKeysSelect(List<SpecialItem> keys){

			toolButtonPool.AddChildInstancesToPool(toolButtonContainer);
            for (int i = 0; i < keys.Count; i++)
            {

                SpecialItem tool = keys[i];

                ToolButton toolButton = toolButtonPool.GetInstance<ToolButton>(toolButtonModel.gameObject, toolButtonContainer);

                toolButton.SetUpToolButton(tool, ToolSelectCallBack);
            }


		}

		private void SetUpKeyDoorCharacterFill(HLHWord word){

			keyDoorCharacterFillList.Clear();

			char[] characters = word.spell.ToCharArray();

			for (int i = 0; i < word.wordLength;i++){

				KeyDoorCharacterFill keyDoorCharacter = keyDoorCharacterPool.GetInstance<KeyDoorCharacterFill>(keyDoorCharacterModel.gameObject, keyDoorCharacterContainer);

				keyDoorCharacter.Reset();

				keyDoorCharacterFillList.Add(keyDoorCharacter);
			}

		}

		private void SetUpCharacterButtons(HLHWord word){

			filledIndex.Clear();

			char[] characters = word.spell.ToCharArray();

			List<int> allIndex = new List<int>{ 0, 1, 2, 3, 4, 5, 6, 7 };

            // 生成一个列表，后面用于打乱字母顺序
			List<int> charIndexList = new List<int>();
            // 生成一个数组，用于存放字母顺序打乱后的字母序号信息
			int[] charIndexArray = new int[word.wordLength];

            // 8个可用位置中选出单词长度数量的位置
			for (int i = 0; i < word.wordLength;i++){

				int randomSeed = Random.Range(0, allIndex.Count);

				int index = allIndex[randomSeed];

				filledIndex.Add(index);

				allIndex.RemoveAt(randomSeed);

				charIndexList.Add(i);
			}

			// 组成单词的字母打成乱序
			for (int i = 0; i < word.wordLength;i++){
				
				int randomSeed = Random.Range(0, charIndexList.Count);

				int index = charIndexList[randomSeed];

				charIndexList.RemoveAt(randomSeed);

				charIndexArray[i] = index;

				//Debug.Log(index);

			}

			int fillCount = 0;

			for (int i = 0; i < characterButtons.Length;i++){
                        
				KeyDoorCharacterButton characterButton = characterButtons[i];
            
				if(filledIndex.Contains(i)){
                               
					int characterIndex = charIndexArray[fillCount];

					char character = characters[characterIndex];

					characterButton.SetUpKeyDoorCharacterButton(character, OnCharacterClick, true);

					fillCount++;
               
				}else{
					characterButton.SetUpKeyDoorCharacterButton('#', OnCharacterClick, false);
				}


			}


		}

		private void OnCharacterClick(char character,bool pushDown){

			if(pushDown){
				AddCharacterFill(character);
			}else{
				RemoveCharacterFill(character);
			}

		}

		private void AddCharacterFill(char character){
			
			for (int i = 0; i < keyDoorCharacterFillList.Count; i++)
            {

                KeyDoorCharacterFill doorCharacterFill = keyDoorCharacterFillList[i];

                if (doorCharacterFill.fillCharacter == ' ')
                {
					doorCharacterFill.SetUpKeyDoorCharacterFill(character, false);
					break;
                }
            }
		}

		private void RemoveCharacterFill(char character){

			for (int i = 0; i < keyDoorCharacterFillList.Count; i++)
            {

                KeyDoorCharacterFill doorCharacterFill = keyDoorCharacterFillList[i];

                if (doorCharacterFill.fillCharacter == character && !doorCharacterFill.fix)
                {
                    doorCharacterFill.Reset();
					break;
                }
            }

		}

		public void OnUnlockButtonClick(){

			char[] correctCharacters = keyDoorWord.spell.ToCharArray();

			bool fillCorrect = true;

			for (int i = 0; i < keyDoorCharacterFillList.Count;i++){
				
				KeyDoorCharacterFill doorCharacterFill = keyDoorCharacterFillList[i];

				if(doorCharacterFill.fillCharacter != correctCharacters[i]){
					fillCorrect = false;
					break;
				}

			}

			if(fillCorrect){

				lockIcon.sprite = lockOffSprite;

				GameManager.Instance.soundManager.PlayAudioClip(CommonData.lockOffAudioName);

				keyDoorOperationMask.gameObject.SetActive(true);
                            
				IEnumerator waitQuitCoroutine = UnlockAndWaitAndQuit();

				StartCoroutine(waitQuitCoroutine);
			}else{
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.wrongTintAudioName);
			}

		}
        
		private IEnumerator UnlockAndWaitAndQuit(){

			yield return new WaitForSeconds(1.0f);

			QuitKeyDoorOperatorView();

			unlockSuccessCallBack(keyDoorWord);

		}

		public void OnRefreshButtonClick(){

			for (int i = 0; i < filledIndex.Count;i++){
				
				int index = filledIndex[i];

				KeyDoorCharacterButton characterButton = characterButtons[index];

				characterButton.ResetOnRefresh();
				         
			}

			for (int i = 0; i < keyDoorCharacterFillList.Count;i++){
				KeyDoorCharacterFill keyDoorCharacterFill = keyDoorCharacterFillList[i];
				if(!keyDoorCharacterFill.fix){
					keyDoorCharacterFill.Reset();
				}
			}

		}

		public void OnQuitButtonClick(){
			queryQuitHUD.gameObject.SetActive(true);
		}

		public void OnConfirmQuitButtonClick(){
			QuitKeyDoorOperatorView();

		}

		public void OnCancelQuitButtonClick(){
			queryQuitHUD.gameObject.SetActive(false);
		}


		public void QuitKeyDoorOperatorView(){

			toolButtonPool.AddChildInstancesToPool(toolButtonContainer);

			keyDoorCharacterPool.AddChildInstancesToPool(keyDoorCharacterContainer);

			this.gameObject.SetActive(false);

			toolSelectPlane.gameObject.SetActive(false);

			unlockOperationPlane.gameObject.SetActive(false);

			queryQuitHUD.gameObject.SetActive(false);

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;


			ExploreManager.Instance.MapWalkableEventsStartAction();

		}



        
    }
}

