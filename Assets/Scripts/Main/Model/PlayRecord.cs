using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System;

    /// <summary>
    /// 通关记录
    /// </summary>
	[System.Serializable]
	public class PlayRecord
    {
        // 最大探索层数
		public int maxExploreLevel;
        // 本次通关探索天数
		public int totalExploreDays;
        // 本次通关击败过的怪物数量
		public int totalDefeatMonsterCount;
        // 本次通关学习的单词数量
		public int totalLearnedWordCount;
        // 本次通关单词学习正确率（x100的整数）
		public int learnCorrectPercentageX100;
        // 本次通关的最大生命值
		public int maxHealth;
        // 本次通关的最大魔法值
		public int maxMana;
		// 本次通关的攻击力
		public int attack;
		// 本次通关的魔法攻击力
		public int magicAttack;
		// 本次通关的护甲
		public int armor;
		// 本次通关的抗性
		public int magicResist;
		// 本次通关的护甲穿透
		public int armorDecrease;
		// 本次通关的抗性穿透
		public int magicResistDecrease;
		// 本次通关的闪避
		public float dodge;
		// 本次通关的暴击
		public float crit;
		// 本次通关的额外金币
		public int extraGold;
		// 本次通关的额外经验
		public int extraExperience;
		// 本次通关的生命回复
		public int healthRecovery;
		// 本次通关的魔法回复
		public int magicRecovery;
		// 本次通关的装备
		public List<Equipment> equipedEquipments = new List<Equipment>();
		// 本次通关的技能
		public List<SkillModel> learnedSkillRecords = new List<SkillModel>();
        // 记录是否通关【这个属性已废弃】
		public bool finishGame = false;
        // 死亡记录【这个属性已废弃】
		public string dieFrom;

        // 探索评分
		public int evaluatePoint;

        // 探索评级描述
		public string evaluateString;


        // 构造函数
		public PlayRecord(){

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
            
            // 计算评分
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

