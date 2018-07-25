using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	//using UnityEngine.UI;

	public class DifficultySelectHUD : ZoomHUD
    {

        // 难度选择选中的图片数组【简单-中等-困难】
		public Transform[] difficultySelectedIcons;

		private int difficulty;

		private CallBack selectDifficultyCallBack;

		public void SetUpDifficultySelectHUD(CallBack selectDifficultyCallBack){

			this.selectDifficultyCallBack = selectDifficultyCallBack;

			difficultySelectedIcons[0].gameObject.SetActive(true);
			difficultySelectedIcons[1].gameObject.SetActive(false);
			difficultySelectedIcons[2].gameObject.SetActive(false);

			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);
                     
		}
        

        /// <summary>
        /// 难度选择响应
        /// </summary>
        /// <param name="difficulty">Difficulty.</param>
		public void OnDifficultySelected(int difficulty){
			this.difficulty = difficulty;
			for (int i = 0; i < difficultySelectedIcons.Length;i++){
				difficultySelectedIcons[i].gameObject.SetActive(difficulty == i);
			}
		}

		public void OnConfirmDifficultyButtonClick(){
			
			WordType wordType = (WordType)difficulty;

			if(wordType != GameManager.Instance.gameDataCenter.gameSettings.wordType){
				GameManager.Instance.gameDataCenter.gameSettings.wordType = wordType;
				GameManager.Instance.persistDataManager.SaveGameSettings();
			}

			selectDifficultyCallBack();

			QuitDifficultySelectHUD();
		}

		public void QuitDifficultySelectHUD(){

			if (inZoomingOut)
            {
                return;
            }
         
			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}
         
			zoomCoroutine = HUDZoomOut();

			StartCoroutine(zoomCoroutine);

		}

    }

}

