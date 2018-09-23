using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System;

	[System.Serializable]
	public class PlayRecord
    {

		public int maxExploreLevel;

		public int totalExploreDays;

		public int totalDefeatMonsterCount;

		public int totalLearnedWordCount;

		public int learnCorrectPercentageX100;

		public int maxHealth;

		public int maxMana;

		public int attack;

		public int magicAttack;

		public int armor;

		public int magicResist;

		public int armorDecrease;

		public int magicResistDecrease;

		public float dodge;

		public float crit;

		public int extraGold;

		public int extraExperience;

		public int healthRecovery;

		public int magicRecovery;

		public List<Equipment> equipedEquipments = new List<Equipment>();

		public List<SkillModel> learnedSkillRecords = new List<SkillModel>();
        
		public bool finishGame = false;

		public string dieFrom;

        // 探索评分
		public int evaluatePoint;

        // 探索评级描述
		public string evaluateString;

		public PlayRecord(bool finishGame,string dieFrom){

			Debug.Log("generate play record");

			Player player = Player.mainPlayer;

			player.ResetBattleAgentProperties(false);

			this.maxExploreLevel = player.maxUnlockLevelIndex + 1 > 50 ? 50 : Player.mainPlayer.maxUnlockLevelIndex + 1;

			DateTime now = DateTime.Now;

			DateTime installDate = Convert.ToDateTime(player.currentExploreStartDateString);

            TimeSpan timeSpan = now.Subtract(installDate);

			this.totalExploreDays = timeSpan.Days + 1;

			LearningInfo learningInfo = LearningInfo.Instance;

			int learnedWordCountOfCurrentType = player.learnedWordsCountInCurrentExplore;

			int correctWordCountOfCurrentType = player.correctWordsCountInCurrentExplore;
         
			this.totalDefeatMonsterCount = player.totaldefeatMonsterCount;

			this.totalLearnedWordCount = learnedWordCountOfCurrentType;
                     
			this.learnCorrectPercentageX100 = learnedWordCountOfCurrentType == 0 ? 0 : correctWordCountOfCurrentType * 100 / learnedWordCountOfCurrentType;

			this.maxHealth = player.maxHealth;

			this.maxMana = player.maxMana;

			this.attack = player.attack;

			this.magicAttack = player.magicAttack;

			this.armor = player.armor;

			this.magicResist = player.magicResist;

			this.armorDecrease = player.armorDecrease;

			this.magicResistDecrease = player.magicResistDecrease;

			this.dodge = player.dodge;

			this.crit = player.crit;

			this.extraGold = player.extraGold;

			this.extraExperience = player.extraExperience;

			this.healthRecovery = player.healthRecovery;

			this.magicRecovery= player.magicRecovery;

			for (int i = 0; i < player.allEquipedEquipments.Length;i++){
				Equipment equipment = player.allEquipedEquipments[i];
				if(equipment.itemId < 0){
					continue;
				}
				this.equipedEquipments.Add(equipment);
			}

			for (int i = 0; i < player.allLearnedSkillsRecord.Count;i++){
				SkillModel learnedSkill = player.allLearnedSkillsRecord[i];
				if(learnedSkill.skillId <= 0){
					continue;
				}
				this.learnedSkillRecords.Add(learnedSkill);
			}
			//this.learnedSkillRecords = player.allLearnedSkillsRecord;

			this.finishGame = finishGame;

			this.dieFrom = dieFrom;
            
			this.evaluatePoint = (int)(totalDefeatMonsterCount * 0.3f) + totalLearnedWordCount * learnCorrectPercentageX100 * learnCorrectPercentageX100 / 10000;


			if(evaluatePoint < 100){
				evaluateString = "F";
			}
			else if(evaluatePoint>=100 && evaluatePoint<300)
			{
				evaluateString = "E";            
			}
			else if(evaluatePoint >= 300 && evaluatePoint < 550)
			{
				evaluateString = "D";  
			}
			else if (evaluatePoint >= 550 && evaluatePoint < 850)
            {
                evaluateString = "C";
			}
			else if (evaluatePoint >= 850 && evaluatePoint < 1250)
            {
                evaluateString = "B";
            }
			else if (evaluatePoint >= 1250 && evaluatePoint < 1700)
            {
                evaluateString = "A";
            }
			else if (evaluatePoint >= 1700 && evaluatePoint < 2200)
            {
				evaluateString = "S";
            }
			else if (evaluatePoint >= 2200 && evaluatePoint < 2750)
            {
				evaluateString = "SS";
            }
			else if (evaluatePoint >= 2750)
            {
                evaluateString = "SSS";
            }

		}

        
    }
}

