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

		public UnlockOperatorController unlockOperator;
        
		private CallBack unlockSuccessCallBack;
        
		private CallBack unlockFailCallBack;

		private int doorDifficulty;
              
		public void SetUpKeyDoorOperatorView(List<SpecialItem> keys,int doorDifficulty,CallBack unlockSuccessCallBack,CallBack unlockFailCallBack){

			this.unlockSuccessCallBack = unlockSuccessCallBack;

			this.unlockFailCallBack = unlockFailCallBack;

			if(doorDifficulty == 0){
				doorDifficulty = 1;
			}

			this.doorDifficulty = doorDifficulty;

			toolButtonPool.AddChildInstancesToPool(toolButtonContainer);
            
			for (int i = 0; i < keys.Count;i++){

				SpecialItem tool = keys[i];
            
				ToolButton toolButton = toolButtonPool.GetInstance<ToolButton>(toolButtonModel.gameObject, toolButtonContainer);

				toolButton.SetUpToolButton(tool, ToolSelectCallBack);
			}

			this.gameObject.SetActive(true);

			unlockOperator.gameObject.SetActive(false);

			ExploreManager.Instance.MapWalkableEventsStopAction();

		}
        
		private void ToolSelectCallBack(Item tool){

			SpecialItem key = tool as SpecialItem;

			KeyType keyType = KeyType.Iron;
			bool needEnterUnlockOperator = true;

			switch(key.specialItemType){
				case SpecialItemType.TieYaoShi:
					keyType = KeyType.Iron;
					break;
				case SpecialItemType.TongYaoShi:
					keyType = KeyType.Brass;
					break;
				case SpecialItemType.JinYaoShi:
					keyType = KeyType.Gold;
					break;
				case SpecialItemType.WanNengYaoShi:
					Player.mainPlayer.RemoveItem(key,1);
					unlockSuccessCallBack();
					needEnterUnlockOperator = false;
					QuitKeyDoorOperatorView();
					break;
				case SpecialItemType.QiaoZhen:
					Player.mainPlayer.RemoveItem(key, 1);
					unlockSuccessCallBack();
					needEnterUnlockOperator = false;
					QuitKeyDoorOperatorView();
					break;
			}


			if(needEnterUnlockOperator)
			{
				toolButtonPool.AddChildInstancesToPool(toolButtonContainer);

				unlockOperator.StartUnlockCheck(keyType, doorDifficulty, delegate{
					if(unlockSuccessCallBack != null){
						unlockSuccessCallBack();
						Player.mainPlayer.RemoveItem(key, 1);
                    }
				}, delegate {
					if(unlockFailCallBack != null){
						unlockFailCallBack();
						Player.mainPlayer.RemoveItem(key, 1);
					}               
				},QuitKeyDoorOperatorView);
			} 
		}

      

		public void QuitKeyDoorOperatorView(){

			toolButtonPool.AddChildInstancesToPool(toolButtonContainer);

			this.gameObject.SetActive(false);

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;

			ExploreManager.Instance.MapWalkableEventsStartAction();

		}

        
    }
}

