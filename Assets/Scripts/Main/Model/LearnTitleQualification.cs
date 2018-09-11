﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney  
{
	public class LearnTitleQualification
	{
        public int totalWordsCount;
        public float totalCorrectPercentage;
        public int continuousCorrectWordCount;
        public string title;
        public string qualificationNeed;
        public int rewardGold;

        public LearnTitleQualification(int totalWordsCount, float totalCorrectPercentage, int continuousCorrectWordCount, string title, string qualificationNeed, int rewardGold)
        {
            this.totalWordsCount = totalWordsCount;
            this.totalCorrectPercentage = totalCorrectPercentage;
            this.continuousCorrectWordCount = continuousCorrectWordCount;
            this.qualificationNeed = qualificationNeed;
            this.title = title;
            this.rewardGold = rewardGold;
        }

        /// <summary>
        /// 检查是否达成称号
        /// 如果没有新的称号达成，返回-1
        /// 如果有新的称号达成，返回称号序号
        /// </summary>
        public static int CheckLearnTitleQualification()
        {

            int qualificationIndex = -1;

            int totalLearnedWordCount = Player.mainPlayer.totalLearnedWordCount;

            int totalUngraspWordCount = Player.mainPlayer.totalUngraspWordCount;

            int continuousCorrectWordCount = Player.mainPlayer.maxWordContinuousRightRecord;

            float correctPercentage = totalLearnedWordCount == 0 ? 0 : (float)(totalLearnedWordCount - totalUngraspWordCount) / totalLearnedWordCount;

            for (int i = 0; i < CommonData.learnTitleQualifications.Length; i++)
            {

                bool titleQualified = Player.mainPlayer.titleQualifications[i];

                if (titleQualified)
                {
                    continue;
                }

                LearnTitleQualification qualification = CommonData.learnTitleQualifications[i];

                titleQualified = totalLearnedWordCount >= qualification.totalWordsCount
                                 && correctPercentage >= qualification.totalCorrectPercentage - float.Epsilon
                                 && continuousCorrectWordCount >= qualification.continuousCorrectWordCount;

                bool dataCorrect = false;

                if (titleQualified)
                {

                    ExploreManager.Instance.UpdateWordDataBase();

                    int learnWordCountFromDB = LearningInfo.Instance.learnedWordCount;
                    int ungraspWordCountFromDB = LearningInfo.Instance.ungraspedWordCount;
                    dataCorrect = learnWordCountFromDB == totalLearnedWordCount && ungraspWordCountFromDB == totalUngraspWordCount;

                    if (dataCorrect)
                    {
                        Player.mainPlayer.titleQualifications[i] = true;
                        qualificationIndex = i;
                        break;
                    }
                    else
                    {
                        Player.mainPlayer.totalLearnedWordCount = learnWordCountFromDB;
                        Player.mainPlayer.totalUngraspWordCount = ungraspWordCountFromDB;
                        GameManager.Instance.persistDataManager.SaveCompletePlayerData();
                        return CheckLearnTitleQualification();

                    }
                }
            }
            return qualificationIndex;
        }
      }   
}