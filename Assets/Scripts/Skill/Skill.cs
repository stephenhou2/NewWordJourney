using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	// 技能类型
	public enum SkillType{
		Active,
		PermanentPassive,
		TriggeredPassive
	}

    // 伤害类型
	public enum HurtType{
		Physical,
		Magical
	}


    /// <summary>
    /// 技能类
    /// </summary>
	public abstract class Skill:MonoBehaviour {

		public string skillName;// 技能名称

		public string sfxName;//音效名称

		public int skillId;//技能id

		public string skillDescription;//技能描述

		public string skillIconName;//技能图片名称

		public string selfEffectAnimName;
		public string enemyEffectAnimName;


		public SkillType skillType;//技能类型

		public int skillLevel = 1;//技能等级

		public int wordCountToLearn;//学习技能所需单词数量

        // 在npc处学习技能的价格
		//public int price;

		[HideInInspector] public bool hasTriggered;//是否已经触发过了

        // 升级所需技能点
		public int upgradeNum{
			get{
				return skillLevel;
			}
		}

       

		/// <summary>
		/// 技能作用效果
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		/// <param name="skillLevel">Skill level.</param>
		public abstract void AffectAgents (BattleAgentController self, BattleAgentController enemy);
			
        /// <summary>
        /// 升级技能
        /// </summary>
		public void SkillLevelUp(){
			skillLevel++;
		}

        

		// 判断概率性技能是否生效
		protected virtual bool isEffective(float chance){
			float randomNum = Random.Range (0, 100)/100f;
			return randomNum < chance - float.Epsilon;
		}

        /// <summary>
        /// 公式转实际数值的技能描述
        /// </summary>
        /// <returns>The display description.</returns>
		public virtual string GetDisplayDescription()
		{
			return string.Empty;
		}

		public override string ToString ()
		{
			return string.Format ("[Skill]" + "\n[SkillName]:" + skillName);
		}

	}


    /// <summary>
    /// 技能数据模型
    /// </summary>
	[System.Serializable]
	public class SkillModel{
		public int skillId;
	    public int skillLevel;
		public SkillModel(int skillId,int skillLevel){
			this.skillId = skillId;
			this.skillLevel = skillLevel;
		}
	}
		

}
	

