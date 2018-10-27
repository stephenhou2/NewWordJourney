using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
    
    /// <summary>
    /// 通关简易记录
    /// </summary>
	public class PlayRecordSimpleCell : MonoBehaviour
    {

        // 通关记录序号
		public Text recordIndexText;
        // 探索成果文本
		public Text exploreEndAt;
        // 最大探索层数文本
		public Text exploreMaxLevelText;
        // 通关评级文本
		public Text evaluateText;
        // 通关详细信息按钮
		public Button detailInfoButton;

        /// <summary>
        /// 初始化简易通关记录界面
        /// </summary>
        /// <param name="playRecord">Play record.</param>
        /// <param name="recordIndex">Record index.</param>
        /// <param name="clickDetailCallBack">Click detail call back.</param>
		public void SetUpPlayRecordSimpleCell(PlayRecord playRecord,int recordIndex, CallBack clickDetailCallBack){

			recordIndexText.text = (recordIndex + 1).ToString(); 

			if (playRecord.finishGame)
            {
                exploreEndAt.text = "获得艾尔文的宝藏";
            }
            else
            {
                exploreEndAt.text = string.Format("失败于: {0}", playRecord.dieFrom);
            }

			exploreMaxLevelText.text = string.Format("探索至: {0}层", playRecord.maxExploreLevel);


			evaluateText.text = string.Format("综合评级: {0}", playRecord.evaluateString);

			detailInfoButton.onClick.RemoveAllListeners();

			detailInfoButton.onClick.AddListener(delegate {
			    
				if(clickDetailCallBack != null){
					clickDetailCallBack();
				}

			});

		}

    }
}

