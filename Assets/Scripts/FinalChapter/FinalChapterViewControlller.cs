using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;


    /// <summary>
    /// 最终通关场景控制器
    /// </summary>
	public class FinalChapterViewControlller : MonoBehaviour
    {

        // 和老头子的对话内容
		public Text dialogText;

        // 和老头子的对话面板
		public Transform dialogHUD;

        // 玩家进入通关场景时，会自动移动到老头子旁边和老头子对话
        // 自动移动的终点
		public Vector2 playerMoveDestination;

        // 显示下段对话的按钮
		public Button nextDialogButton;

        // 提示文本面板
		public TintHUD hintHUD;

        // 重设人物属性的界面
		public RebuildPlayerView rebuildPlayerView;

        // 询问是否确认按照当前设置重设人物属性的界面
		public Transform queryRebuildHUD;

        // 重设完成回调
		private CallBack rebuildFinishCallBack;

        // 遮罩
		public Image mask;

           
        // 终章的所有对话内容
		private string[] finalDialogs = {
			"恭喜你，我的朋友，你完成了最终的试炼。",
			"你是如此的不同，在漫漫岁月里，只有你最后来到了这里。",
			"其实我的名字叫埃克托·文斯，也就是这座城堡的主人，很抱歉之前骗了你。",
			"我只是不想因为我的身份干扰你在城堡里的探险，希望你在冒险的过程中确实收获到了一些东西。",
			"真正的宝藏，其实是你在探索中所散发出的智慧和勇气。当然了，还有坚持不懈的毅力。",
			"我曾经拥有过这些东西，不过在很早之前就弄丢了。而我最大的心愿就是再看一眼这个宝藏。",
			"很感谢，你让我有机会见证这一切。请收下这由你自己所创造的宝藏吧！",
            "我的身后有一块永恒之石，它可以重新塑造你的灵魂。",
            "再见了，我的朋友，接下来去开始新的冒险吧！"
		};

        // 当前对话序号
		private int dialogIndex;

		private Transform npcTrans;

        /// <summary>
        /// 初始化终章场景
        /// </summary>
		public void SetUpFinalChapterView(){
                 
            // 如果老头子还没有消失
			if(ExploreManager.Instance.newMapGenerator.allNPCsInMap.Count > 0){

				mask.enabled = true;

				npcTrans = ExploreManager.Instance.newMapGenerator.allNPCsInMap[0].transform;
                // 对话序号置0
				dialogIndex = 0;
                // 等待玩家走到指定位置后显示和npc的对话面板
                IEnumerator waitCoroutine = WaitPlayerWalkFinishAndShowDialogHUD();

                StartCoroutine(waitCoroutine);

			}else{

				mask.enabled = false;

				ExploreManager.Instance.EnableExploreInteractivity();
			}
         
		}

        /// <summary>
        /// 等待玩家走到指定位置后显示和npc的对话界面
        /// </summary>
        /// <returns>The player walk finish and show dialog hud.</returns>
		private IEnumerator WaitPlayerWalkFinishAndShowDialogHUD(){

			yield return new WaitUntil(() => TransformManager.FindTransform("CanvasContainer/LoadingCanvas") == null);

			ExploreManager.Instance.battlePlayerCtr.boxCollider.enabled = false;

            // 等待1s
			yield return new WaitForSeconds(1f);
            // 播放idle动画
			ExploreManager.Instance.battlePlayerCtr.PlayRoleAnim(CommonData.roleIdleAnimName, 0, null);
            // 移动到指定位置
			ExploreManager.Instance.battlePlayerCtr.MoveToPosition(playerMoveDestination, ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray);

			//yield return null;

			//Debug.LogFormat("[{0},{1}]..........[{2},{3}]", ExploreManager.Instance.battlePlayerCtr.transform.position.x, ExploreManager.Instance.battlePlayerCtr.transform.position.y,
							//playerMoveDestination.x, playerMoveDestination.y);
            // 等待移动完成
			yield return new WaitUntil(() => (Mathf.Abs(ExploreManager.Instance.battlePlayerCtr.transform.position.x - playerMoveDestination.x) <= 0.01f
			                                  && Mathf.Abs(ExploreManager.Instance.battlePlayerCtr.transform.position.y - playerMoveDestination.y) <= 0.01f));

			ExploreManager.Instance.battlePlayerCtr.TowardsLeft();
         
			dialogHUD.gameObject.SetActive(true);

			dialogText.text = finalDialogs[dialogIndex];

			//mask.enabled = false;
		}

        /// <summary>
        /// 进入下个对话按钮
        /// </summary>
		public void OnNextDialogButtonClick()
		{
			dialogIndex++;

            // 来到最后一段对话
			if (dialogIndex == finalDialogs.Length){

                int posX = Mathf.RoundToInt(npcTrans.position.x);
                int posY = Mathf.RoundToInt(npcTrans.position.y);

                // npc的位置改为可行走
                ExploreManager.Instance.newMapGenerator.mapWalkableEventInfoArray[posX, posY] = 1;
                // 记录npc已经触发过
                MapEventsRecord.AddEventTriggeredRecord(50, npcTrans.position);

                npcTrans.gameObject.SetActive(false);

                // 回收npc
                ExploreManager.Instance.newMapGenerator.allNPCsInMap.Clear();

                Destroy(npcTrans.gameObject, 0.3f);

                ExploreManager.Instance.EnableExploreInteractivity();

                QuitFinalDialogHUD();

                ExploreManager.Instance.battlePlayerCtr.boxCollider.enabled = true;

				return;
            }      


            // 如果不是倒数第三个对话，直接显示对话内容
			if (dialogIndex != finalDialogs.Length - 2)
			{
				dialogText.text = finalDialogs[dialogIndex];
			}else{
				
				dialogHUD.gameObject.SetActive(false);

				// 倒数第三个对话的时候显示分享界面
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", delegate
				{
				    
					TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.WeChat, null, null, delegate {
					
						dialogText.text = finalDialogs[dialogIndex];
                        nextDialogButton.interactable = true;
                        dialogHUD.gameObject.SetActive(true);
					});
				});
			}

		}

        // 退出终章对话界面
		private void QuitFinalDialogHUD(){

			dialogHUD.gameObject.SetActive(false);

			mask.enabled = false;

		}
        
        // 显示重置面板
		public void ShowQueryRebuildHUD(CallBack rebuildFinishCallBack){
			ExploreManager.Instance.DisableAllInteractivity();
			this.rebuildFinishCallBack = rebuildFinishCallBack;
			queryRebuildHUD.gameObject.SetActive(true);         
		}

        // 确认进入重置面板
		public void OnConfirmEnterRebuildButtonClick(){
			queryRebuildHUD.gameObject.SetActive(false);
			rebuildPlayerView.SetUpRebuildPlayerView(rebuildFinishCallBack);         
		}

        // 取消进入重置面板
		public void OnCancelEnterRebuildButtonClick(){
			queryRebuildHUD.gameObject.SetActive(false);
			ExploreManager.Instance.EnableExploreInteractivity();
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
		}


        // 销毁终章整体UI界面
		public void DestroyInstances(){

			this.gameObject.SetActive(false);

			Destroy(this.gameObject, 0.3f);

		}


        

    }
}

