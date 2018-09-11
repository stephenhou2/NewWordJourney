using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace WordJourney
{

	public enum PropertyType{
		MaxHealth,
		MaxMana,
		Attack,
		MagicAttack,
		MoveSpeed,
		Armor,
		MagicResist,
		ArmorDecrease,
		MagicResistDecrease,
		Crit,
		Dodge,
		CritHurtScaler,
        PhysicalHurtScaler,
        MagicalHurtScaler,
		ExtraGold,
		ExtraExperience,
		HealthRecovery,
		MagicRecovery,
        Health,
        Mana
	}
		

	public enum AttackSpeed{
		Slow,
		Medium,
		Fast,
		VeryFast,
        NoInterval
	}

	public abstract class Agent : MonoBehaviour {

		public string agentName;

		public int agentLevel;

		//[HideInInspector]public bool isDead;

		protected BattleAgentController mBattleAgentController;
		protected BattleAgentController battleAgentCtr{
			get{
				if (mBattleAgentController == null) {
					mBattleAgentController = GetComponent<BattleAgentController> ();
				}
				if (mBattleAgentController == null) {
					mBattleAgentController = transform.Find ("BattlePlayer").GetComponent<BattleAgentController> ();
				}
				return mBattleAgentController;
			}
		}
			

		//*****人物基础信息(无装备，无状态加成时的人物属性)********//
		public int originalMaxHealth;//基础最大生命值
		public int originalMaxMana;//基础最大魔法值
		public int originalAttack;//基础物理伤害
		public int originalMagicAttack;//基础魔法伤害
		public int originalArmor;//基础护甲
		public int originalMagicResist;//基础抗性
		public int originalArmorDecrease;//基础护甲穿刺
		public int originalMagicResistDecrease;//基础抗性穿刺
		public int originalMoveSpeed;//基础地图行走速度
		public float originalCrit;//基础暴击率
		public float originalDodge;//基础闪避率
		public int originalExtraGold;//基础额外金币
		public int originalExtraExperience;//基础额外经验
		public float originalPhysicalHurtScaler;//基础物理伤害系数
		public float originalMagicalHurtScaler;//基础魔法伤害系数
		public float originalCritHurtScaler;//基础暴击系数
		public int originalHealthRecovery;//基础生命回复
		public int originalMagicRecovery;//基础魔法回复
        //*****人物基础信息(无装备，无状态加成时的人物属性)********//


        //**** 人物因技能引起的属性变化**********//
        [HideInInspector]public int maxHealthChangeFromSkill;
        [HideInInspector]public int maxManaChangeFromSkill;
        [HideInInspector]public int attackChangeFromSkill;
        [HideInInspector]public int magicAttackChangeFromSkill;
        [HideInInspector]public int armorChangeFromSkill;
        [HideInInspector]public int magicResistChangeFromSkill;
        [HideInInspector]public int armorDecreaseChangeFromSkill;
        [HideInInspector]public int magicResistDecreaseChangeFromSkill;
        [HideInInspector]public int moveSpeedChangeFromSkill;
        [HideInInspector]public float critChangeFromSkill;
        [HideInInspector]public float dodgeChangeFromSkill;
        [HideInInspector] public int extraGoldChangeFromSkill;
        [HideInInspector]public int extraExperienceChangeFromSkill;
        [HideInInspector]public float physicalHurtScalerChangeFromSkill;
        [HideInInspector]public float magicalHurtScalerChangeFromSkill;
        [HideInInspector]public float critHurtScalerChangeFromSkill;
        [HideInInspector]public int healthRecoveryChangeFromSkill;
        [HideInInspector]public int magicRecoveryChangeFromSkill;
        //**** 人物因技能引起的属性变化**********//


		//********人物最终的实际属性信息*********//
		public int mMaxHealth;//实际最大血量
		public int mHealth;//实际生命
		public int mMaxMana;//实际最大魔法值
		public int mMana;//实际魔法
		public int mAttack;//实际物理伤害
		public int mMagicAttack;//实际魔法伤害
		public int mArmor;//实际护甲
		public int mMagicResist;//实际抗性
		public int mArmorDecrease;
		public int mMagicResistDecrease;
		public int mMoveSpeed;//实际行走速度
		public float mCrit;//实际暴击
		public float mDodge;//实际闪避
		public int mExtraGold;//实际额外金钱
		public int mExtraExperience;//实际额外经验
		public float mPhysicalHurtScaler;//实际物理伤害系数
		public float mMagicalHurtScaler;//实际魔法伤害系数
		public float mCritHurtScaler;//实际暴击系数
		public int mHealthRecovery;//实际生命回复
		public int mMagicRecovery;//实际魔法回复
		//********人物最终的实际属性信息*********//


		public int maxHealth{
			get{ return mMaxHealth; }
			set{ mMaxHealth = value > 0 ? value : 0;}
		}
			
		public int health{
			get{ return mHealth; }
			set{ 
				if (value >= 0) {
					mHealth = value >= maxHealth ? maxHealth : value;
				} else {
					mHealth = 0;
				}
			}
		}

		public int maxMana{
			get{ return mMaxMana; }
			set{ mMaxMana = value > 0 ? value : 0;}
		}

		public int mana{
			get{ return mMana; }
			set{ 
				if (value >= 0) {
					mMana = value >= maxMana ? maxMana : value;
				} else {
					mMana = 0;
				}
			}
		}

		public int attack{
			get{ return mAttack; }
			set{ mAttack = value > 0 ? value : 0;}
		}

		public int magicAttack{
			get{ return mMagicAttack; }
			set{ mMagicAttack = value > 0 ? value : 0; }
		}
			
		public int armor{
			get{ return mArmor; }
			set{ mArmor = value > 0 ? value : 0; }
		}

		public int magicResist{
			get{ return mMagicResist; }
			set{ mMagicResist = value > 0 ? value : 0; }
		}

		public int armorDecrease{
			get{ return mArmorDecrease; }
			set{ mArmorDecrease = value > 0 ? value : 0; }
		}

		public int magicResistDecrease{
			get{ return mMagicResistDecrease; }
			set{ mMagicResistDecrease = value > 0 ? value : 0; }
		}
			


		public int moveSpeed{
			get{ return mMoveSpeed; }
			set{ mMoveSpeed = value > 20 ? value : 20; 
				if (mMoveSpeed > 100) {
					mMoveSpeed = 100;
				}
			}
		}

		public float crit{
			get{ return mCrit; }
			set{ mCrit = value > 0 ? value : 0; }
		}

		public float dodge {
			get{ return mDodge; }
			set{ mDodge = value > 0 ? value : 0; }
		}

		public int extraGold{
			get { return mExtraGold; }
			set{ mExtraGold = value > 0 ? value : 0; }
		}

		public int extraExperience{
			get { return mExtraExperience; }
			set{ mExtraExperience = value > 0 ? value : 0; }
		}

		public float physicalHurtScaler{
			get{ return mPhysicalHurtScaler; }
			set { mPhysicalHurtScaler = value > 0 ? value : 0; }
		}

		public float magicalHurtScaler{
			get { return mMagicalHurtScaler; }
			set{ mMagicalHurtScaler = value > 0 ? value : 0; }
		}

		public float critHurtScaler{
			get{ return mCritHurtScaler; }
			set{ mCritHurtScaler = value > 0 ? value : 0; }
		}

		public int healthRecovery{
			get{ return mHealthRecovery; }
			set{ mHealthRecovery = value > 0 ? value : 0; }
		}

		public int magicRecovery{
			get{ return mMagicRecovery; }
			set{ mMagicRecovery = value > 0 ? value : 0; }
		}
			

		public int shenLuTuTengScaler = 0;
		//public int extraPoisonHurt = 0;

		public int physicalHurtToEnemy;
		public int magicalHurtToEnemy;
//		public float dodgeFixScaler;//闪避修正系数
//		public float critFixScaler;//暴击修正系数


		public Transform skillsContainer;
			

		public List<Equipment> allEquipmentsInBag = new List<Equipment> ();


		public Equipment[] allEquipedEquipments;



		public List<ActiveSkill> attachedActiveSkills = new List<ActiveSkill> ();
		public List<PermanentPassiveSkill> attachedPermanentPassiveSkills = new List<PermanentPassiveSkill> ();
		public List<TriggeredPassiveSkill> attachedTriggeredSkills = new List<TriggeredPassiveSkill>();

		public List<string> allStatus = new List<string> ();



		// 攻击间隔
		public virtual float attackInterval{get;set;}
			

		public virtual void Awake(){

			ResetBattleAgentProperties (false);

		}
			

        public void ClearPropertyChangesFromSkill()
        {
            maxHealthChangeFromSkill = 0;
            maxManaChangeFromSkill = 0;
            attackChangeFromSkill = 0;
            magicAttackChangeFromSkill = 0;
            armorChangeFromSkill = 0;
            magicResistChangeFromSkill = 0;
            armorDecreaseChangeFromSkill = 0;
            magicResistDecreaseChangeFromSkill = 0;
            moveSpeedChangeFromSkill = 0;
            critChangeFromSkill = 0;
            dodgeChangeFromSkill = 0;
            extraGoldChangeFromSkill = 0;
            extraExperienceChangeFromSkill = 0;
            physicalHurtScalerChangeFromSkill = 0;
            magicalHurtScalerChangeFromSkill = 0;
            critHurtScalerChangeFromSkill = 0;
            healthRecoveryChangeFromSkill = 0;
            magicRecoveryChangeFromSkill = 0;

        }

		// 仅根据物品重新计人物的属性，其余属性重置为初始状态
		public abstract PropertyChange ResetBattleAgentProperties (bool toOriginalState = false);


		public override string ToString ()
		{
			return string.Format ("[agent]:{0}", agentName);
		}
	}

}
