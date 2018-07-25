using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{

	using UnityEngine.UI;

	public class ConsumablesCell : MonoBehaviour {

		public Image itemIcon;
		public Text itemCount;
		public Button consumablesButton;

		private Item item;

		private CallBack refreshCallBack;

		public void SetUpConsumablesCell(Item item,CallBack refreshCallBack){

			consumablesButton.interactable = true;

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);
			itemIcon.sprite = itemSprite;

			itemIcon.enabled = itemSprite != null;

			itemCount.text = item.itemCount.ToString ();

			this.item = item;

			this.refreshCallBack = refreshCallBack;

		}

		public void ClearConsumablesCell(){
			itemIcon.sprite = null;
			itemIcon.enabled = false;
			itemCount.text = "";
			consumablesButton.interactable = false;
		}

		public void OnConsumablesClick(){
            
			BattlePlayerController battlePlayerController = ExploreManager.Instance.battlePlayerCtr;
			if(item.itemType == ItemType.Consumables){

				Consumables cons = item as Consumables;

				cons.UseConsumables(battlePlayerController);

                bool isLevelUp = Player.mainPlayer.LevelUpIfExperienceEnough();
                if (isLevelUp)
                {
                    ExploreManager.Instance.battlePlayerCtr.SetEffectAnim(CommonData.levelUpEffectName);
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.levelUpAudioName);               
                    ExploreManager.Instance.expUICtr.ShowLevelUpPlane();
                }

				GameManager.Instance.soundManager.PlayAudioClip(cons.audioName);
			}else if(item.itemType == ItemType.SpecialItem){
				SpecialItem specialItem = item as SpecialItem;

				specialItem.UseSpecialItem(specialItem, null);

				//GameManager.Instance.soundManager.PlayAudioClip(specialItem.audioName);
			}

			Player.mainPlayer.RemoveItem (item, 1);
                     
			ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();

			if (refreshCallBack != null) {
				refreshCallBack ();
			}
		}
	}
}
