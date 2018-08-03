using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
	/// 普通攻击
	/// </summary>
	public class NormalAttack: ActiveSkill {


		protected override void ExcuteActiveSkillLogic(BattleAgentController self, BattleAgentController enemy){

            // 如果是玩家角色，则在每次攻击时重设攻速
			if(self is BattlePlayerController){
				int attackSpeedInt = 0;

                for (int i = 0; i < self.agent.allEquipedEquipments.Length; i++)
                {
                    Equipment equipment = self.agent.allEquipedEquipments[i];
                    if (equipment.itemId == -1)
                    {
                        continue;
                    }

                    if (equipment.equipmentType == EquipmentType.Weapon)
                    {
                        attackSpeedInt = (int)equipment.attackSpeed;
                        break;
                    }
                }

                for (int i = 0; i < self.agent.attachedPermanentPassiveSkills.Count; i++)
                {
                    PermanentPassiveSkill permanentPassiveSkill = self.agent.attachedPermanentPassiveSkills[i];
                    if (permanentPassiveSkill is JiSu)
                    {
                        attackSpeedInt++;
                        break;
                    }
                }

                if (attackSpeedInt > 3)
                {
                    attackSpeedInt = 3;
                }

				(self.agent as Player).attackSpeed = (AttackSpeed)attackSpeedInt;
			}

			SetEffectAnims(self, enemy);

			// 执行攻击触发事件回调
			for(int i = 0;i < self.attackTriggerExcutors.Count; i++) {
				TriggeredSkillExcutor excutor = self.attackTriggerExcutors[i];
				excutor.triggeredCallback (self, enemy);
			}

			// 敌方执行被攻击触发事件回调
			for(int i = 0; i<enemy.beAttackedTriggerExcutors.Count; i++) {
				TriggeredSkillExcutor excutor = enemy.beAttackedTriggerExcutors[i];
				excutor.triggeredCallback (enemy, self);
			}
				
			//计算对方闪避率(敌方的基础闪避率 - 己方的闪避修正)
			float enemyDodge = enemy.agent.dodge;
			//判断对方是否闪避成功
			if(isEffective(enemyDodge)){
				enemy.AddTintTextToQueue ("闪避");
				return;
			}

			int actualPhysicalHurt = 0;
			int actualMagicalHurt = 0;

			if (self.agent.attack > 0) {
				
				//int oriPhysicalHurt = self.agent.attack + self.agent.armorDecrease / 4;

				int oriPhysicalHurt = self.agent.attack;

				float crit = self.agent.crit;

				if (isEffective (crit)) {
					enemy.AddTintTextToQueue ("暴击");
					oriPhysicalHurt = (int)(oriPhysicalHurt * self.agent.critHurtScaler);
				}

				int armorCal = enemy.agent.armor - self.agent.armorDecrease/2;

				if (armorCal < -50)
                {
                    armorCal = -50;
                }

				actualPhysicalHurt = (int)(oriPhysicalHurt / (armorCal / 100f + 1));


				//actualPhysicalHurt = oriPhysicalHurt - enemy.agent.armor / 4;

				if (actualPhysicalHurt < 0) {
					actualPhysicalHurt = 0;
				}

				enemy.AddHurtAndShow (actualPhysicalHurt, HurtType.Physical,self.towards);

			}

			if (self.agent.magicAttack > 0) {

				int magicResistCal = enemy.agent.magicResist - self.agent.magicResistDecrease/2;

				if (magicResistCal < -50)
                {
                    magicResistCal = -50;
                }

				actualMagicalHurt = (int)(self.agent.magicAttack / (magicResistCal / 100f + 1));

				if (actualMagicalHurt < 0) {
					actualMagicalHurt = 0;
				}

				enemy.AddHurtAndShow (actualMagicalHurt, HurtType.Magical,self.towards);

			}

			enemy.PlayShakeAnim ();



			self.agent.physicalHurtToEnemy = actualPhysicalHurt;
			self.agent.magicalHurtToEnemy = actualMagicalHurt;

			// 执行己方攻击命中的回调
			for(int i = 0;i<self.hitTriggerExcutors.Count;i++) {
				TriggeredSkillExcutor excutor = self.hitTriggerExcutors[i];
				excutor.triggeredCallback (self, enemy);
			}
				
			// 执行敌方被击中的回调
			for(int i = 0;i < enemy.beHitTriggerExcutors.Count; i++) {
				TriggeredSkillExcutor excutor = enemy.beHitTriggerExcutors[i];
				excutor.triggeredCallback (enemy, self);
			}



		}

	}
}
