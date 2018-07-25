using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public enum TargetAgent{
		Self,
        Enemy
	}

	public class PropertyChangeSkill : ActiveSkill
    {

		public PropertyType propertyType;

		public bool canOverlay;

		public float change;

		public TargetAgent target;


		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy)
		{

			Agent targetAgent = self.agent;

			switch(target){
				case TargetAgent.Self:
					targetAgent = self.agent;
					break;
				case TargetAgent.Enemy:
					targetAgent = enemy.agent;
					break;
			}

			if (!hasTriggered || canOverlay)
			{
				hasTriggered = true;

				string tintString = string.Empty;

				switch (propertyType)
				{
					case PropertyType.MaxHealth:
						int maxHealthRecord = targetAgent.maxHealth;
						targetAgent.maxHealth += (int)change;
						targetAgent.health = targetAgent.health * targetAgent.maxHealth / maxHealthRecord;
						tintString = change > 0 ?  "生命上限\n提升" : "生命上限\n降低";
						break;
					case PropertyType.MaxMana:
						int maxManaRecord = targetAgent.maxMana;
						targetAgent.maxMana += (int)change;
						targetAgent.mana = targetAgent.mana * targetAgent.maxMana / maxManaRecord;
						tintString = change > 0 ? "魔法上限\n提升" : "魔法上限\n降低";
						break;
					case PropertyType.Attack:
						targetAgent.attack += (int)change;
						targetAgent.attackChangeFromSkill += (int)change;
						tintString = change > 0 ? "攻击\n提升" : "攻击\n降低";
						break;
					case PropertyType.MagicAttack:
						targetAgent.magicAttack += (int)change;
						targetAgent.magicAttack += (int)change;
						tintString = change > 0 ? "魔攻\n提升" : "魔攻\n降低";
						break;
					case PropertyType.MoveSpeed:
						targetAgent.moveSpeed += (int)change;
						targetAgent.moveSpeedChangeFromSkill += (int)change;
						tintString = change > 0 ? "移速\n提升" : "移速\n降低";
						break;
					case PropertyType.Armor:
						targetAgent.armor += (int)change;
						targetAgent.armorChangeFromSkill += (int)change;
						tintString = change > 0 ? "护甲\n提升" : "护甲\n降低";
						break;
					case PropertyType.MagicResist:
						targetAgent.magicResist += (int)change;
						targetAgent.magicResistChangeFromSkill += (int)change;
						tintString = change > 0 ? "抗性\n提升" : "抗性\n降低";
						break;
					case PropertyType.ArmorDecrease:
						targetAgent.armorDecrease += (int)change;
						targetAgent.armorDecreaseChangeFromSkill += (int)change;
						tintString = change > 0 ? "护甲穿透\n提升" : "护甲穿透\n降低";
						break;
					case PropertyType.MagicResistDecrease:
						targetAgent.magicResistDecrease += (int)change;
						targetAgent.magicResistDecreaseChangeFromSkill += (int)change;
						tintString = change > 0 ? "魔法穿透\n提升" : "魔法穿透\n降低";
						break;
					case PropertyType.Crit:
						targetAgent.crit += change;
						targetAgent.critChangeFromSkill += change;
						tintString = change > 0 ? "暴击\n提升" : "暴击\n降低";
						break;
					case PropertyType.Dodge:
						targetAgent.dodge += change;
						targetAgent.dodgeChangeFromSkill += change;
						tintString = change > 0 ? "闪避\n提升" : "闪避\n降低";
						break;
					case PropertyType.CritHurtScaler:
						targetAgent.critHurtScaler += change;
						targetAgent.critHurtScalerChangeFromSkill += change;
						tintString = change > 0 ? "暴击伤害\n提升" : "暴击伤害\n降低";
						break;
				}

				if (selfEffectAnimName != string.Empty)
				{
					self.SetEffectAnim(selfEffectAnimName);
				}

				if (enemyEffectAnimName != string.Empty){
					enemy.SetEffectAnim(enemyEffectAnimName);
				}

				switch(target){
					case TargetAgent.Self:
						self.AddTintTextToQueue(tintString);
						break;
					case TargetAgent.Enemy:
						enemy.AddTintTextToQueue(tintString);
						break;
				}

			}


		}
	}
}

