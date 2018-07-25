using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class ToolSelectView : MonoBehaviour
    {

		public ToolButton toolButtonModel;

		public Transform toolButtonContainer;

		public InstancePool toolButtonPool;

		private CallBackWithItem toolSelectCallBack;
              
		public void SetUpToolSelectView(List<SpecialItem> tools,CallBackWithItem toolSelectCallBack){

			this.toolSelectCallBack = toolSelectCallBack;

			toolButtonPool.AddChildInstancesToPool(toolButtonContainer);
            
			for (int i = 0; i < tools.Count;i++){

				SpecialItem tool = tools[i];
            
				ToolButton toolButton = toolButtonPool.GetInstance<ToolButton>(toolButtonModel.gameObject, toolButtonContainer);

				toolButton.SetUpToolButton(tool, ToolSelectCallBack);
			}

			this.gameObject.SetActive(true);

			ExploreManager.Instance.MapWalkableEventsStopAction();

		}

		private void ToolSelectCallBack(Item tool){

			toolSelectCallBack(tool);

			QuitToolSelectView();
		}


		public void QuitToolSelectView(){

			toolButtonPool.AddChildInstancesToPool(toolButtonContainer);

			this.gameObject.SetActive(false);

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;

			ExploreManager.Instance.MapWalkableEventsStartAction();

		}

        
    }
}

