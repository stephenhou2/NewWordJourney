using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using System;

	public enum TransitionType{
		None,
		Introduce,
		Death,
        End,
        ResetGameHint
	}

	public class TransitionView : MonoBehaviour
	{

		private string[] introduceStrings = {
			"阳光透过窗户洒进古老的殿堂",
			"黑暗中一件宝藏散发着淡淡微光",
			"隐约听到来自远古的呼唤",
			"世间的勇者们啊\n请接受我的馈赠\n来黑暗的深处拿取我的宝藏",
			"你的故事将从这里开始…"
		};
		private string[] deadStrings = {
			"你渐渐倒在了黑暗之中",
            "耳畔不断传来阵阵呼唤",
            "隐约间一道微光闪过",
            "你的身体又恢复了知觉..."         
		};

		private string[] endStrings = {
			"岁月在地板和墙上雕刻出痕迹",
			"在这里学到的东西也会慢慢模糊消融",
			"但这座城堡将会永远伫立在这里",
            "对于所有想要回来的人们",
            "城堡的大门将永远敞开"
		};

		private string[] resetGameHintStrings = {};


		public Text transitionTextModel;

		public Transform transitionTextContainer;

		public Text clickTintText;

		public Text resetGameDataHintText;

		public float fadeInTime = 1.5f;

		public float sentenceInterval = 0.5f;

		private TransitionType transitionType;

		public Image transitionPlaneMask;

		private bool hasUserClick;

		private int heightBase = 145;

		//public void SetUpTransitionView(TransitionType transitionType){

		//	this.transitionType = transitionType;

		//}
              

		public void PlayTransition(TransitionType transitionType,CallBack finishTransitionCallBack){

			hasUserClick = false;

			transitionPlaneMask.enabled = true;

			this.transitionType = transitionType;

			this.gameObject.SetActive (true);

			string[] transitionStrings = null;

			switch (transitionType) {
			case TransitionType.Introduce:
				transitionStrings = introduceStrings;
				break;
			case TransitionType.Death:
				transitionStrings = deadStrings;               
				break;
			case TransitionType.End:
				transitionStrings = endStrings;
                break;
			case TransitionType.ResetGameHint:
				transitionStrings = resetGameHintStrings;
				break;
    		}

			IEnumerator transitionCoroutine = PlayTransition (transitionStrings,finishTransitionCallBack);

			StartCoroutine (transitionCoroutine);

		}

		public void UserClick(){
			hasUserClick = true;
		}

		private IEnumerator PlayTransition(string[] transitionStrings,CallBack finishTransitionCallBack){

			if(transitionType != TransitionType.None){
				transitionPlaneMask.enabled = true;
				yield return new WaitForSeconds(1.0f);
				transitionPlaneMask.enabled = false;
			}

			Transform loadingCanvas = TransformManager.FindTransform("LoadingCanvas");

			while(loadingCanvas != null){
				yield return null;
			}

			if(transitionType == TransitionType.ResetGameHint){
				resetGameDataHintText.enabled = true;
			}else{
				resetGameDataHintText.enabled = false;
			}

			if(transitionStrings != null){

				int totalSentenceCount = transitionStrings.Length;

                transitionTextContainer.localPosition = new Vector3(0, totalSentenceCount * heightBase / 2, 0);

                transitionPlaneMask.enabled = false;

                clickTintText.enabled = false;

                float alphaChangeSpeed = 1.0f / fadeInTime;

                float alpha = 0;

                for (int i = 0; i < totalSentenceCount; i++)
                {

                    Text t = Instantiate(transitionTextModel.gameObject, transitionTextContainer).GetComponent<Text>();

                    t.text = transitionStrings[i];

                    alpha = 0;


                    while (alpha < 1)
                    {

                        t.color = new Color(1, 1, 1, alpha);

						//alpha += alphaChangeSpeed * Time.unscaledDeltaTime;
						alpha += alphaChangeSpeed * Time.deltaTime;

                        yield return null;

                    }

					//yield return new WaitForSecondsRealtime(sentenceInterval);

					yield return new WaitForSeconds(sentenceInterval);
                }

				bool clickContinue = false;

				switch (transitionType)
                {
                    case TransitionType.Introduce:
                        transitionPlaneMask.enabled = true;
                        transitionPlaneMask.color = new Color(0, 0, 0, 0);
                        transitionPlaneMask.raycastTarget = true;
						clickTintText.text = "点击屏幕继续";
                        clickTintText.enabled = true;
                        alpha = 0.5f;
						clickContinue = true;                  
                        break;
					case TransitionType.None:
                        break;
                    case TransitionType.Death:
                        break;
                    case TransitionType.End:
						transitionPlaneMask.enabled = true;
                        transitionPlaneMask.color = new Color(0, 0, 0, 0);
                        transitionPlaneMask.raycastTarget = true;
                        clickTintText.text = "点击屏幕重置进度";
                        clickTintText.enabled = true;
                        alpha = 0.5f;
						clickContinue = true;
						break;
					case TransitionType.ResetGameHint:
						transitionPlaneMask.enabled = true;
                        transitionPlaneMask.color = new Color(0, 0, 0, 0);
                        transitionPlaneMask.raycastTarget = true;
                        clickTintText.text = "点击屏幕继续";
                        clickTintText.enabled = true;
                        alpha = 0.5f;
						clickContinue = true;
						break;
 
                }

                // 如果需要点击屏幕才可以继续的情况
				if(clickContinue){
					while (!hasUserClick)
					{
						
						while (alpha < 1f)
						{
							
							clickTintText.color = new Color(1, 1, 1, alpha);
							
							//alpha += alphaChangeSpeed * Time.unscaledDeltaTime / 2;
							
							alpha += alphaChangeSpeed * Time.deltaTime / 2;
							
							if (hasUserClick)
							{
								break;
							}
							
							yield return null;
							
						}
						
						while (alpha > 0.5f)
						{
							
							clickTintText.color = new Color(1, 1, 1, alpha);
							
							alpha -= alphaChangeSpeed * Time.deltaTime / 2;
							
							if (hasUserClick)
							{
								break;
							}
							
							yield return null;
							
						}
					}
                }

				alpha = 0;

                while (alpha < 1)
                {
                    transitionPlaneMask.color = new Color(0, 0, 0, alpha);

					//alpha += alphaChangeSpeed * Time.unscaledDeltaTime;
					alpha += alphaChangeSpeed * Time.deltaTime;

                    yield return null;
                }
            
			}

			switch (transitionType)
            {
                case TransitionType.Introduce:
                    GameManager.Instance.soundManager.PlayBgmAudioClip(CommonData.exploreBgmName);
					Player.mainPlayer.isNewPlayer = false;
                    GameManager.Instance.persistDataManager.SaveCompletePlayerData();
                    break;
				case TransitionType.None:
                case TransitionType.Death:

					break;
                case TransitionType.End: 
					PlayRecord playRecord = new PlayRecord(true, "");
                    List<PlayRecord> playRecords = GameManager.Instance.gameDataCenter.allPlayRecords;
                    playRecords.Add(playRecord);
                    GameManager.Instance.persistDataManager.SavePlayRecords(playRecords);
                    break;
				case TransitionType.ResetGameHint:
					break;
            }
		
			if (finishTransitionCallBack != null)
            {
				finishTransitionCallBack();
            }


			this.gameObject.SetActive (false);         

			for (int i = 0; i < transitionTextContainer.childCount; i++) {
				Destroy (transitionTextContainer.GetChild (i).gameObject,0.3f);
			}

		}


	}
}
