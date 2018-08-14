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

		public string[] finalDialogs = {
			"恭喜你，我的朋友，你完成了最终的试炼",
			"你是如此的不同，在漫漫岁月里，只有你最后来到了这里",
			"其实我的名字叫埃克托·文斯，也就是这座城堡的主人，很抱歉之前骗了你",
			"我只是不想因为我的身份干扰你在城堡里的探险，希望你在冒险的过程中确实收获到了一些东西",
			"真正的宝藏，其实是你在探索中所散发出的智慧和勇气。当然了，还有坚持不懈的毅力",
			"我曾经拥有过这些东西，不过在很早之前就弄丢了。而我最大的心愿就是再看一眼这个宝藏",
			"很感谢，你让我有机会见证这一切。请收下这由你自己所创造的宝藏吧！",
            "再见了，朋友，接下来出去拥抱无限光明的未来吧！"
		};

		private int dialogIndex;

		//private Transform loadingCanvas;

		public void SetUpFinalChapterView(){
       
			//loadingCanvas = TransformManager.FindTransform("LoadingCanvas");

			dialogIndex = 0;

			StartCoroutine("WaitPlayerWalkFinishAndShowDialogHUD");

		}

		private IEnumerator WaitPlayerWalkFinishAndShowDialogHUD(){

			yield return new WaitForSeconds(0.5f);
			//yield return new WaitUntil(() => !loadingCanvas.gameObject.activeInHierarchy);

			ExploreManager.Instance.battlePlayerCtr.MoveToPosition(playerMoveDestination, ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray);

			ExploreManager.Instance.battlePlayerCtr.isIdle = false;

			yield return new WaitUntil(() => ExploreManager.Instance.battlePlayerCtr.isIdle);
         
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
				dialogText.text = finalDialogs[dialogIndex];

				nextDialogButton.interactable = false;

				//if (Application.internetReachability == NetworkReachability.NotReachable)
				//{
				//	hintHUD.SetUpSingleTextTintHUD("无网络连接");
				//}
				//else
				//{
					GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", delegate
					{
						nextDialogButton.interactable = true;
						TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.WeChat, null, null, null);
					});
				//}
			}
			else if (dialogIndex == finalDialogs.Length){

				QuitFinalDialogHUD();
            
				ExploreManager.Instance.expUICtr.transitionMask.gameObject.SetActive(true);
				ExploreManager.Instance.expUICtr.transitionMask.color = new Color(0, 0, 0, 1);

				GameManager.Instance.soundManager.StopBgm();

				ExploreManager.Instance.expUICtr.transitionView.PlayTransition(TransitionType.End, delegate
				{
					GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal();

					GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.loadingCanvasBundleName, "LoadingCanvas", delegate
                    {               
						ExploreManager.Instance.QuitExploreScene();

                        TransformManager.FindTransform("LoadingCanvas").GetComponent<LoadingViewController>().SetUpLoadingView(LoadingType.QuitExplore, null, delegate {
                            GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.homeCanvasBundleName, "HomeCanvas", null);
                        });

                    });

				});


			}      
		}

		private void QuitFinalDialogHUD(){

			GameManager.Instance.UIManager.RemoveCanvasCache("FinalChapterCanvas");

		}

		public void DestroyInstances(){

			this.gameObject.SetActive(false);

			Destroy(this.gameObject, 0.3f);

		}


        

    }
}

