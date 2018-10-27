using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
    /// 技能生成器
    /// </summary>
	public class SkillGenerator {

        /// <summary>
        /// 生成技能
        /// </summary>
        /// <returns>The skill.</returns>
        /// <param name="skillId">Skill identifier.</param>
        /// <param name="skillLevel">Skill level.</param>
		public static Skill GenerateSkill(int skillId,int skillLevel = 1){

			Skill skillModel = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
				return obj.skillId == skillId;
			});

			Skill skill = GameObject.Instantiate (skillModel.gameObject).GetComponent<Skill>();

			skill.gameObject.name = skillModel.name;

			skill.skillLevel = skillLevel;

			return skill;
		}
	}
}
