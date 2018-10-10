using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEngine.UI;

	public delegate void CallBackWithCharAndBool(char arg1,bool arg2);

	public class KeyDoorCharacterButton : MonoBehaviour
    {
        
		public Text characterText;

		public Button characterButton;

		public char character;

		private bool pushDown;



		public void SetUpKeyDoorCharacterButton(char character,CallBackWithCharAndBool callBack,bool interactable){

			pushDown = false;

			this.character = character;

			characterButton.interactable = interactable;

			characterText.text = character.ToString();

			if(interactable){
				characterText.color = CommonData.darkYellowTextColor;
			}else if(character == '-'){
				characterText.color = Color.gray;
			}else{
				characterText.color = Color.gray;
			}

			characterButton.onClick.RemoveAllListeners();

			characterButton.onClick.AddListener(delegate
			{
				pushDown = !pushDown;

				characterText.color = pushDown ? CommonData.orangeTextColor : CommonData.darkYellowTextColor;

				callBack(character, pushDown);

			});

		}

		public void SetToHint(){
			
			pushDown = true;

			characterButton.interactable = false;

			characterText.color = Color.gray;
		}

		public void ResetOnRefresh()
		{
			if(!characterButton.interactable){
				return;
			}

			pushDown = false;

			characterButton.interactable = true;

			characterText.text = character.ToString();

			characterText.color = CommonData.darkYellowTextColor;

			//characterButton.onClick.RemoveAllListeners();

		}


	}
}

