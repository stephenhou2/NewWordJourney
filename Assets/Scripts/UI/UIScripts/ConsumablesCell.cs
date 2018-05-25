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

		private Consumables cons;

		private CallBack refreshCallBack;

		public void SetUpConsumablesCell(Consumables consumables,CallBack refreshCallBack){

			consumablesButton.interactable = true;

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (consumables);
			itemIcon.sprite = itemSprite;

			itemIcon.enabled = itemSprite != null;

			itemCount.text = consumables.itemCount.ToString ();

			this.cons = consumables;

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

			//switch (cons.type) {
    			//case ConsumablesType.ShuXingTiSheng:

					if (cons.healthGain > 0)
                    {

						int healthGain = cons.healthGain + Player.mainPlayer.healthRecovery;

						battlePlayerController.AddHealthGainAndShow(healthGain);

						battlePlayerController.SetEffectAnim(CommonData.healthHealEffecttName);
					}
                    
					//if(cons.manaGain != 0){
					//	int manaGain = cons.manaGain + Player.mainPlayer.magicRecovery;
					//}

					if (cons.manaGain > 0)
                    {            
						battlePlayerController.AddManaGainAndShow(cons.manaGain + Player.mainPlayer.magicRecovery);

						battlePlayerController.SetEffectAnim(CommonData.magicHealAudioName);

					}else if (cons.manaGain < 0){

						Player.mainPlayer.mana += cons.manaGain;

					}                
    				
    				Player.mainPlayer.experience += cons.experienceGain;
    				Player.mainPlayer.RemoveItem (cons, 1);
					GameManager.Instance.soundManager.PlayAudioClip(cons.audioName);
					ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();
    				//break;  
    			//case ConsumablesType.ChongZhuShi:
    			//case ConsumablesType.DianJinShi:
    			//case ConsumablesType.XiaoMoJuanZhou:
    				//break;
			//    case ConsumablesType.YinShenJuanZhou:
			//	    ExploreManager.Instance.PlayerFade ();
   //                 Player.mainPlayer.RemoveItem(cons, 1);
			//		GameManager.Instance.soundManager.PlayAudioClip(cons.audioName);
			//	break;

			//}
			if (refreshCallBack != null) {
				refreshCallBack ();
			}
		}
	}
}
