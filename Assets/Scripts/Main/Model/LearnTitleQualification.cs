using System.Collections;
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

			int continuousCorrectWordCount = 0;

			switch(LearningInfo.Instance.currentWordType){
				case WordType.Simple:
					continuousCorrectWordCount = Player.mainPlayer.maxSimpleWordContinuousRightRecord;
					break;
				case WordType.Medium:
					continuousCorrectWordCount = Player.mainPlayer.maxMediumWordContinuousRightRecord;
					break;
				case WordType.Master:
					continuousCorrectWordCount = Player.mainPlayer.maxMasterWordContinuousRightRecord;
					break;
			}
            

            float correctPercentage = totalLearnedWordCount == 0 ? 0 : (float)(totalLearnedWordCount - totalUngraspWordCount) / totalLearnedWordCount;

            for (int i = 0; i < CommonData.learnTitleQualifications.Length; i++)
            {

				bool titleQualified = false;
				switch(LearningInfo.Instance.currentWordType){
					case WordType.Simple:
						titleQualified = Player.mainPlayer.titleQualificationsOfSimple[i];
						break;
					case WordType.Medium:
						titleQualified = Player.mainPlayer.titleQualificationsOfMedium[i];
						break;
					case WordType.Master:
						titleQualified = Player.mainPlayer.titleQualificationsOfMaster[i];
						break;
				}
            
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
						switch(LearningInfo.Instance.currentWordType){
							case WordType.Simple:
								Player.mainPlayer.titleQualificationsOfSimple[i] = true;
								break;
							case WordType.Medium:
								Player.mainPlayer.titleQualificationsOfMedium[i] = true;
								break;
							case WordType.Master:
								Player.mainPlayer.titleQualificationsOfMaster[i] = true;
								break;
						}
                        
                        qualificationIndex = i;
                        break;
                    }
                    else
                    {
                        Player.mainPlayer.totalLearnedWordCount = learnWordCountFromDB;
                        Player.mainPlayer.totalUngraspWordCount = ungraspWordCountFromDB;
                        
                        return CheckLearnTitleQualification();

                    }
                }
            }
            return qualificationIndex;
        }
      }   
}
