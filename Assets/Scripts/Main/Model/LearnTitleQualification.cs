using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney  
{
	/// <summary>
    /// 学习称号达成条件模型
    /// </summary>
	public class LearnTitleQualification
	{
		// 总学习单词要求
        public int totalWordsCount;
        // 正确率要求
        public float totalCorrectPercentage;
        // 连续正确单词要求
        public int continuousCorrectWordCount;
        // 称号名称
        public string title;
        // 称号需求描述文本
        public string qualificationNeed;
        // 称号达成奖励的金币
        public int rewardGold;

        //构造函数
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
						if(Player.mainPlayer.titleQualificationsOfSimple != null){
							titleQualified = Player.mainPlayer.titleQualificationsOfSimple[i];
                        }
						break;
					case WordType.Medium:
						if(Player.mainPlayer.titleQualificationsOfMedium != null){
							titleQualified = Player.mainPlayer.titleQualificationsOfMedium[i];
                        }
						break;
					case WordType.Master:
						if(Player.mainPlayer.titleQualificationsOfMaster != null){
							titleQualified = Player.mainPlayer.titleQualificationsOfMaster[i];
                        }
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
								if (Player.mainPlayer.titleQualificationsOfSimple != null)
								{
									Player.mainPlayer.titleQualificationsOfSimple[i] = true;
									qualificationIndex = i;
								}
								break;
							case WordType.Medium:
								if (Player.mainPlayer.titleQualificationsOfMedium != null)
								{
									Player.mainPlayer.titleQualificationsOfMedium[i] = true;
									qualificationIndex = i;
								}
								qualificationIndex = i;
								break;
							case WordType.Master:
								if (Player.mainPlayer.titleQualificationsOfMaster != null)
								{
									Player.mainPlayer.titleQualificationsOfMaster[i] = true;
									qualificationIndex = i;
								}
								break;
						}
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
