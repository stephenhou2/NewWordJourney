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


		public override string ToString ()
		{
			return string.Format ("[Skill]" + "\n[SkillName]:" + skillName);
		}

	}
		

}
	

