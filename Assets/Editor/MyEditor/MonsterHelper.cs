using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using DragonBones;
	using Transform = UnityEngine.Transform;

	public class MonsterHelper {

        
		[MenuItem("EditHelper/InitMonsters")]
		public static void InitAllMonsters(){

			string monsterDataPath = "/Users/houlianghong/Desktop/MyGameData/monsters.csv";

			MonsterModel[] monsterModels = LoadMonsterModels (monsterDataPath);

			for (int i = 0; i < monsterModels.Length; i++) {

				MonsterModel mm = monsterModels [i];

				string monsterName = mm.monsterSkeName;

				Transform monster = TransformManager.FindTransform (monsterName);

				if (monster == null) {
					Debug.LogFormat ("未找到名为{0}的怪物", monsterName);
					continue;
				}


				Monster monsterScript = monster.gameObject.GetComponent<Monster> ();
				if (monsterScript == null) {
					monsterScript = monster.gameObject.AddComponent<Monster> ();
				}

				//BattleMonsterController bmcScript = monster.gameObject.GetComponent<BattleMonsterController> ();

				MapMonster mmScript = monster.gameObject.GetComponent<MapMonster> ();
				if (mmScript == null) {
					mmScript = monster.gameObject.AddComponent<MapMonster> ();
				}

            

				monsterScript.monsterId = mm.monsterId;
				monsterScript.agentName = mm.monsterName;
				monsterScript.agentLevel = 1;
				monsterScript.rewardGold = mm.gold;
				monsterScript.rewardExperience = mm.experience;

				monsterScript.originalMaxHealth = mm.maxHealth;
				monsterScript.health = mm.maxHealth;
				monsterScript.maxHealth = mm.maxHealth;
				monsterScript.health = mm.maxHealth;

				monsterScript.mAttackInterval = mm.attackInterval;

				monsterScript.originalAttack = mm.attack;
				monsterScript.attack = mm.attack;

				monsterScript.originalMagicAttack = mm.magicAttack;
				monsterScript.magicAttack = mm.magicAttack;

				monsterScript.originalArmor = mm.armor;
				monsterScript.armor = mm.armor;

				monsterScript.originalMagicResist = mm.magicResist;
				monsterScript.magicResist = mm.magicResist;

				monsterScript.originalArmorDecrease = mm.armorDecrease;
				monsterScript.armorDecrease = mm.armorDecrease;

				monsterScript.originalMagicResistDecrease = mm.magicResistDecrease;
				monsterScript.magicResistDecrease = mm.magicResistDecrease;

				monsterScript.originalDodge = mm.dodge;
				monsterScript.dodge = mm.dodge;

				monsterScript.originalCrit = mm.crit;
				monsterScript.crit = mm.crit;

				monsterScript.originalCritHurtScaler = mm.critHurtScaler;
				monsterScript.critHurtScaler = mm.critHurtScaler;

				monsterScript.originalPhysicalHurtScaler = 1f;
				monsterScript.physicalHurtScaler = 1f;

				monsterScript.originalMagicalHurtScaler = 1f;
				monsterScript.magicalHurtScaler = 1f;

				//monsterScript.extraPoisonHurt = 0;
            
			}

		}

		[MenuItem("EditHelper/InitBoss")]
		public static void InitBoss()
        {

			string monsterDataPath = "/Users/houlianghong/Desktop/MyGameData/boss.csv";

			MonsterModel[] monsterModels = LoadMonsterModels(monsterDataPath);

            for (int i = 0; i < monsterModels.Length; i++)
            {

                MonsterModel mm = monsterModels[i];

                string monsterName = mm.monsterSkeName;

                Transform monster = TransformManager.FindTransform(monsterName);

                if (monster == null)
                {
                    Debug.LogFormat("未找到名为{0}的怪物", monsterName);
                    continue;
                }


                Monster monsterScript = monster.gameObject.GetComponent<Monster>();
                if (monsterScript == null)
                {
                    monsterScript = monster.gameObject.AddComponent<Monster>();
                }

                //BattleMonsterController bmcScript = monster.gameObject.GetComponent<BattleMonsterController>();

                MapMonster mmScript = monster.gameObject.GetComponent<MapMonster>();
                if (mmScript == null)
                {
                    mmScript = monster.gameObject.AddComponent<MapMonster>();
                }



                monsterScript.monsterId = mm.monsterId;
                monsterScript.agentName = mm.monsterName;
                monsterScript.agentLevel = 1;
                monsterScript.rewardGold = mm.gold;
                monsterScript.rewardExperience = mm.experience;

                monsterScript.originalMaxHealth = mm.maxHealth;
                monsterScript.health = mm.maxHealth;
                monsterScript.maxHealth = mm.maxHealth;
                monsterScript.health = mm.maxHealth;

                monsterScript.mAttackInterval = mm.attackInterval;

                monsterScript.originalAttack = mm.attack;
                monsterScript.attack = mm.attack;

                monsterScript.originalMagicAttack = mm.magicAttack;
                monsterScript.magicAttack = mm.magicAttack;

                monsterScript.originalArmor = mm.armor;
                monsterScript.armor = mm.armor;

                monsterScript.originalMagicResist = mm.magicResist;
                monsterScript.magicResist = mm.magicResist;

                monsterScript.originalArmorDecrease = mm.armorDecrease;
                monsterScript.armorDecrease = mm.armorDecrease;

                monsterScript.originalMagicResistDecrease = mm.magicResistDecrease;
                monsterScript.magicResistDecrease = mm.magicResistDecrease;

                monsterScript.originalDodge = mm.dodge;
                monsterScript.dodge = mm.dodge;

                monsterScript.originalCrit = mm.crit;
                monsterScript.crit = mm.crit;

                monsterScript.originalCritHurtScaler = mm.critHurtScaler;
                monsterScript.critHurtScaler = mm.critHurtScaler;

                monsterScript.originalPhysicalHurtScaler = 1f;
                monsterScript.physicalHurtScaler = 1f;

                monsterScript.originalMagicalHurtScaler = 1f;
                monsterScript.magicalHurtScaler = 1f;

                //monsterScript.extraPoisonHurt = 0;

            }

        }


		private static MonsterModel[] LoadMonsterModels(string monsterDataPath){         

			string monsterDataString = DataHandler.LoadDataString (monsterDataPath);

			string[] monsterDatas = monsterDataString.Split (new char[]{ '\n' }, System.StringSplitOptions.None);

			MonsterModel[] monsterModels = new MonsterModel[monsterDatas.Length-1];

			for (int i = 1; i < monsterDatas.Length; i++) {

				string monsterData = monsterDatas [i];
                
				MonsterModel mm = new MonsterModel ();

				monsterModels [i-1] = mm;

				string[] detailDatas = monsterData.Split(new char[]{ ',' }, System.StringSplitOptions.None);

				mm.monsterId = int.Parse(detailDatas [0]);

				mm.monsterName = detailDatas [1];

				mm.monsterSkeName = detailDatas [3];

				mm.attackInterval = float.Parse (detailDatas [5]);

				//mm.attackInterval = float.Parse(detailDatas[5]);

				mm.gold = int.Parse (detailDatas [6]);

				mm.experience = int.Parse (detailDatas [7]);

				mm.maxHealth = int.Parse (detailDatas [8]);

				mm.attack = int.Parse (detailDatas [9]);

				mm.armor = int.Parse (detailDatas [10]);

				mm.magicAttack = int.Parse (detailDatas [11]);

				mm.magicResist = int.Parse (detailDatas [12]);

				mm.crit = float.Parse (detailDatas [13]);

				mm.dodge = float.Parse (detailDatas [14]);

				mm.armorDecrease = int.Parse (detailDatas [15]);

				mm.magicResistDecrease = int.Parse (detailDatas [16]);

				mm.critHurtScaler = float.Parse (detailDatas [17]);

			}

			return monsterModels;
		}


		private class MonsterModel
		{
			//攻击间隔	金钱	经验	生命	攻击	护甲	魔法攻击	魔抗	暴击率	闪避率	护甲穿透	魔抗穿透	暴击倍率	技能
			public int monsterId;
			public string monsterName;
			public string monsterSkeName;
			public float attackInterval;
			public int gold;
			public int experience;
			public int maxHealth;
			public int attack;
			public int magicAttack;
			public int armor;
			public int magicResist;
			public float crit;
			public float dodge;
			public int armorDecrease;
			public int magicResistDecrease;
			public float critHurtScaler;


		}

	}
}
