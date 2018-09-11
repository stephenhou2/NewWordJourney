using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class FinalChapterViewControlller : MonoBehaviour
    {

		public Text dialogText;

		public Transform dialogHUD;

		public Vector2 playerMoveDestination;

		public Button nextDialogButton;

		public TintHUD hintHUD;

		public RebuildPlayerView rebuildPlayerView;

		public Transform queryRebuildHUD;

		private CallBack rebuildFinishCallBack;

              
		public string[] finalDialogs = {
			"恭喜你，我的朋友，你完成了最终的试。",
			"你是如此的不同，在漫漫岁月里，只有你最后来到了这里。",
			"其实我的名字叫埃克托·文斯，也就是这座城堡的主人，很抱歉之前骗了你。",
			"我只是不想因为我的身份干扰你在城堡里的探险，希望你在冒险的过程中确实收获到了一些东西。",
			"真正的宝藏，其实是你在探索中所散发出的智慧和勇气。当然了，还有坚持不懈的毅力。",
			"我曾经拥有过这些东西，不过在很早之前就弄丢了。而我最大的心愿就是再看一眼这个宝藏。",
			"很感谢，你让我有机会见证这一切。请收下这由你自己所创造的宝藏吧！",
            "再见了，朋友，接下来出去拥抱无限光明的未来吧！"
		};

		private int dialogIndex;

		private Transform npcTrans;

		public void SetUpFinalChapterView(){

			if(ExploreManager.Instance.newMapGenerator.allNPCsInMap.Count > 0){
				
				npcTrans = ExploreManager.Instance.newMapGenerator.allNPCsInMap[0].transform;

				dialogIndex = 0;

                IEnumerator waitCoroutine = WaitPlayerWalkFinishAndShowDialogHUD();

                StartCoroutine(waitCoroutine);

			}else{
				ExploreManager.Instance.EnableExploreInteractivity();
			}
         
		}

		private IEnumerator WaitPlayerWalkFinishAndShowDialogHUD(){

			yield return new WaitUntil(() => TransformManager.FindTransform("CanvasContainer/LoadingCanvas") == null);

			ExploreManager.Instance.battlePlayerCtr.boxCollider.enabled = false;

			//yield return new WaitForSeconds(0.5f);

			ExploreManager.Instance.battlePlayerCtr.PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);

			ExploreManager.Instance.battlePlayerCtr.MoveToPosition(playerMoveDestination, ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray);

			yield return null;

			yield return new WaitUntil(() => ExploreManager.Instance.battlePlayerCtr.isIdle);

			ExploreManager.Instance.battlePlayerCtr.TowardsLeft();
         
			dialogHUD.gameObject.SetActive(true);

			dialogText.text = finalDialogs[dialogIndex];
		}

		public void OnNextDialogButtonClick()
		{

			dialogIndex++;

			if (dialogIndex < finalDialogs.Length - 1)
			{
				dialogText.text = finalDialogs[dialogIndex];
			}
			else if (dialogIndex == finalDialogs.Length - 1)
			{
				dialogHUD.gameObject.SetActive(false);
            
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", delegate
				{
				    
					TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.WeChat, null, null, delegate {
					
						dialogText.text = finalDialogs[dialogIndex];
                        nextDialogButton.interactable = true;
                        dialogHUD.gameObject.SetActive(true);
					});
				});
			}
			else if (dialogIndex == finalDialogs.Length){

				int posX = Mathf.RoundToInt(npcTrans.position.x);
				int posY = Mathf.RoundToInt(npcTrans.position.y);

				ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray[posX, posY] = 1;

				MapEventsRecord.AddEventTriggeredRecord(50, npcTrans.position);

				npcTrans.gameObject.SetActive(false);

				ExploreManager.Instance.newMapGenerator.allNPCsInMap.Clear();

				Destroy(npcTrans.gameObject, 0.3f);

				ExploreManager.Instance.EnableExploreInteractivity();

				QuitFinalDialogHUD();

				ExploreManager.Instance.battlePlayerCtr.boxCollider.enabled = true;
                  

			}      
		}

		private void QuitFinalDialogHUD(){

			dialogHUD.gameObject.SetActive(false);



			//GameManager.Instance.UIManager.RemoveCanvasCache("FinalChapterCanvas");

		}
        
		public void ShowQueryRebuildHUD(CallBack rebuildFinishCallBack){
			ExploreManager.Instance.DisableAllInteractivity();
			this.rebuildFinishCallBack = rebuildFinishCallBack;
			queryRebuildHUD.gameObject.SetActive(true);         
		}

		public void OnConfirmEnterRebuildButtonClick(){
			queryRebuildHUD.gameObject.SetActive(false);
			rebuildPlayerView.SetUpRebuildPlayerView(rebuildFinishCallBack);         
		}

		public void OnCancelEnterRebuildButtonClick(){
			queryRebuildHUD.gameObject.SetActive(false);
			ExploreManager.Instance.EnableExploreInteractivity();
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
		}


      
		public void DestroyInstances(){

			this.gameObject.SetActive(false);

			Destroy(this.gameObject, 0.3f);

		}


        

    }
}

