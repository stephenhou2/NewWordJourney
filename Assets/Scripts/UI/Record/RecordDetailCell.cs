using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class RecordDetailCell : MonoBehaviour
    {
    
        // 背诵单词数量要求
		public int qualifiedLearnWordCount;

		// 背诵单词数量达到要求后显示的图片
        public Image wordCountQualifiedIcon;

        // 正确率要求
		public float qualifiedCorrectPercentage;

		// 背诵单词正确率达到要求后显示的图片
        public Image correctPercentageQualifiedIcon;

        // 连续背诵正确的数量要求
		public int qualifiedContinuousCorrectCount;

		// 连续正确背诵单词数量达到要求后显示的图片
        public Image continuousCorrectCountQualifiedIcon;
        
        // 完成称号内的所有要求是显示的图标
		public Image titleQualifiedIcon;

		public void SetUpRecordDetailCell(int learnedWordCountOfCurrentType,int wrongWordCountOfCurrentType){

			bool learnedWordCountQualified = learnedWordCountOfCurrentType >= qualifiedLearnWordCount;

			float correctPercentage = learnedWordCountOfCurrentType == 0 ? 0 : (float)(learnedWordCountOfCurrentType - wrongWordCountOfCurrentType) / learnedWordCountOfCurrentType;

			bool correctPercentageQualified = correctPercentage >= qualifiedCorrectPercentage;         

			bool continuousCorrectCountQualified = Player.mainPlayer.wordContinuousRightRecord >= qualifiedContinuousCorrectCount
			                                             || Player.mainPlayer.maxWordContinuousRightRecord >= qualifiedContinuousCorrectCount;
         
			bool titleQualified = learnedWordCountQualified && correctPercentageQualified && continuousCorrectCountQualified;

			if(titleQualifiedIcon != null)
			{
				titleQualifiedIcon.enabled = titleQualified;
			}      

			if (wordCountQualifiedIcon != null)
            {
				wordCountQualifiedIcon.enabled = learnedWordCountQualified && !titleQualified;
            }

			if (correctPercentageQualifiedIcon != null)
            {
				correctPercentageQualifiedIcon.enabled = correctPercentageQualified && !titleQualified;
            }
         
            if (continuousCorrectCountQualifiedIcon != null)
            {
				continuousCorrectCountQualifiedIcon.enabled = continuousCorrectCountQualified && !titleQualified;
            }

		}
        



    }

}

