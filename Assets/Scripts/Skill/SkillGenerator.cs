using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SkillGenerator {

		public static Skill GenerateTriggeredSkill(int skillId){

			Skill skillModel = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
				return obj.skillId == skillId;
			});

			Skill skill = GameObject.Instantiate (skillModel.gameObject).GetComponent<Skill>();

			skill.gameObject.name = skillModel.name;

			return skill;
		}
	}
}
