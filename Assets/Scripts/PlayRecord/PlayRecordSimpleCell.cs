﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
    
	public class PlayRecordSimpleCell : MonoBehaviour
    {

		public Text recordIndexText;

		public Text exploreEndAt;

		public Text exploreMaxLevelText;

		public Text totalLearnedWordCountText;

		public Button detailInfoButton;

		public void SetUpPlayRecordSimpleCell(PlayRecord playRecord,int recordIndex, CallBack clickDetailCallBack){

			recordIndexText.text = (recordIndex + 1).ToString(); 

			if (playRecord.finishGame)
            {
                exploreEndAt.text = "获得艾尔文的宝藏";
            }
            else
            {
                exploreEndAt.text = string.Format("失败于:{0}", playRecord.dieFrom);
            }

			exploreMaxLevelText.text = string.Format("最大探索层数:{0}", playRecord.maxExploreLevel);


			totalLearnedWordCountText.text = string.Format("总计学习单词数:{0}", playRecord.totalLearnedWordCount);

			detailInfoButton.onClick.RemoveAllListeners();

			detailInfoButton.onClick.AddListener(delegate {
			    
				if(clickDetailCallBack != null){
					clickDetailCallBack();
				}

			});

		}

    }
}
