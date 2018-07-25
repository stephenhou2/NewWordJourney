using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public enum SkillType{
		Active,
		PermanentPassive,
		TriggeredPassive
	}

	public enum HurtType{
		Physical,
		Magical
	}

	public abstract class Skill:MonoBehaviour {

		public string skillName;// 技能名称

		public string sfxName;//音效名称

		public int skillId;

		public string skillDescription;

		public string skillIconName;

		public string selfEffectAnimName;
		public string enemyEffectAnimName;


		public SkillType skillType;

		public int skillLevel = 1;

        // 在npc处学习技能的价格
		public int price;

		[HideInInspector] public bool hasTriggered;

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
			
		public void SkillLevelUp(){
			skillLevel++;
		}


		public void DestroySelf(){
			Destroy (this.gameObject);
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
	

