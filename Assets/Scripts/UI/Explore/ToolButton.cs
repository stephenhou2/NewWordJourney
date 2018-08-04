using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEngine.UI;

	public class ToolButton : MonoBehaviour
    {
		public Image toolIcon;

		public Text toolName;

		public Text toolCount;
        
		public Button toolButton;

		public void SetUpToolButton(SpecialItem tool,CallBackWithItem toolSelectCallBack){

			toolIcon.sprite = GameManager.Instance.gameDataCenter.allSpecialItemSprites.Find(delegate(Sprite obj)
			{
				return obj.name == tool.spriteName;            
			});

			toolName.text = tool.itemName;

			toolCount.text = tool.itemCount.ToString();

			toolButton.onClick.RemoveAllListeners();

			toolButton.onClick.AddListener(delegate
			{
				toolSelectCallBack(tool);
			});
		}


    }

}
