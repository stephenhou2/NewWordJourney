using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEngine.UI;

    /// <summary>
    /// 主界面
    /// </summary>
	public class HomeView : MonoBehaviour {

        // 遮罩
		public Image maskImage;

        // logo
		public Transform logoBandage;

        // 单词难度选择面板
		public DifficultySelectHUD difficultySelectHUD;

        // 提示
		public TintHUD tintHUD;

        // logo浮动速度
		public float logoFloatSpeed;
        // logo单次浮动周期
		public float singleDuration;
        // logo 浮动的控制协程
		private IEnumerator logoFloatCoroutine;

        // 提示没有网络的提示框
		public Transform noAvalableNetHintHUD;

        // 提示背包4置换500金币的提示框
		public Transform bagToGoldHintHUD;

        // 通关记录按钮
		public Transform playRecordButton;

        /// <summary>
        /// 初始化主界面
        /// </summary>
		public void SetUpHomeView(){

			GetComponent<Canvas> ().enabled = true;

            // logo浮动动画
			LogoBandageStartFloat();

			if(GameManager.Instance.purchaseManager.buyedGoodsChange.Contains("Bag_4")){
				ShowBagToGoldHintHUD();
				GameManager.Instance.purchaseManager.buyedGoodsChange.Remove("Bag_4");
			}         
		}
        
        /// <summary>
        /// 通关记录缩放提醒
        /// </summary>
		public void PlayRecordHint(){
			IEnumerator myPlayRecordHintCoroutine = PlayRecordButtonZoom();
			StartCoroutine(myPlayRecordHintCoroutine);
		}

        /// <summary>
        /// 通过记录缩放动画控制
        /// </summary>
        /// <returns>The record button zoom.</returns>
		private IEnumerator PlayRecordButtonZoom(){

			float scale = 1f;

			float zoomSpeed = 0.5f;

			while(scale<1.2f){

				scale += zoomSpeed * Time.deltaTime;

				playRecordButton.localScale = new Vector3(scale, scale, 1);

				yield return null;
			}

			while (scale > 1f)
            {

                scale -= zoomSpeed * Time.deltaTime;

                playRecordButton.localScale = new Vector3(scale, scale, 1);

                yield return null;
            }

			yield return new WaitForSeconds(0.5f);

			while (scale < 1.2f)
            {

                scale += zoomSpeed * Time.deltaTime;

                playRecordButton.localScale = new Vector3(scale, scale, 1);

                yield return null;
            }

            while (scale > 1f)
            {

                scale -= zoomSpeed * Time.deltaTime;

                playRecordButton.localScale = new Vector3(scale, scale, 1);

                yield return null;
            }

			yield return new WaitForSeconds(0.5f);

            while (scale < 1.2f)
            {

                scale += zoomSpeed * Time.deltaTime;

                playRecordButton.localScale = new Vector3(scale, scale, 1);

                yield return null;
            }

            while (scale > 1f)
            {

                scale -= zoomSpeed * Time.deltaTime;

                playRecordButton.localScale = new Vector3(scale, scale, 1);

                yield return null;
            }

			playRecordButton.localScale = Vector3.one;

		}
			
        /// <summary>
        /// 开启屏幕遮罩
        /// </summary>
		public void ShowMaskImage (){
			maskImage.gameObject.SetActive (true);
		}


        /// <summary>
        /// 关闭屏幕遮罩
        /// </summary>
		private void HideMaskImage(){
			maskImage.gameObject.SetActive (false);
		}


		public void ShowNoAvalableNetHintHUD(){
			noAvalableNetHintHUD.gameObject.SetActive(true);
		}

		public void HideNoAvalableNetHintHUD(){
			noAvalableNetHintHUD.gameObject.SetActive(false);
		}


        /// <summary>
        /// 显示背包4置换为500金币的提示框
        /// </summary>
		public void ShowBagToGoldHintHUD(){
			bagToGoldHintHUD.gameObject.SetActive(true);
		}

		public void HideBagToGoldHintHUD(){
			bagToGoldHintHUD.gameObject.SetActive(false);
		}
        
		/// <summary>
		/// 初始化难度选择面板
		/// </summary>
		public void SetUpDifficultyChoosePlane(CallBack selectDifficultyCallBack){
                 
			difficultySelectHUD.SetUpDifficultySelectHUD(selectDifficultyCallBack);

		}

        /// <summary>
        /// logo开始浮动
        /// </summary>
		private void LogoBandageStartFloat(){
			if(logoFloatCoroutine == null){
				logoFloatCoroutine = LogoBandageFloat();
				StartCoroutine(logoFloatCoroutine);
			}
		}
        
        /// <summary>
        /// logo浮动动画控制协程
        /// </summary>
        /// <returns>The bandage float.</returns>
		private IEnumerator LogoBandageFloat(){

			logoBandage.localPosition = Vector3.zero;

			float timer = 0;

			while(true){

				while(timer<singleDuration){

					Vector3 moveVector = new Vector3(0, -logoFloatSpeed * Time.deltaTime, 0);

					logoBandage.localPosition += moveVector;

					yield return null;

					timer += Time.deltaTime;

				}

				timer = 0;

				while(timer<singleDuration){

					Vector3 moveVector = new Vector3(0, logoFloatSpeed * Time.deltaTime, 0);

                    logoBandage.localPosition += moveVector;

                    yield return null;

                    timer += Time.deltaTime;               
				}

				timer = 0;

			}

		}

      
		public void OnQuitHomeView(){

			StopAllCoroutines();

			difficultySelectHUD.gameObject.SetActive(false);

			HideMaskImage ();
		}

	}
}
