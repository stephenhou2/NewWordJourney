using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEditor;
	using DragonBones;
	using Transform = UnityEngine.Transform;

	using TMPro;

	public class MonsterHelper {

        

		//[MenuItem("EditHelper/CheckMonsterSay")]
		//public static void CheckTemp(){

		//	string monsterDataPath = "/Users/houlianghong/Desktop/MyGameData/monsters.csv";

  //          MonsterModel[] monsterModels = LoadMonsterModels(monsterDataPath);

		//	for (int i = 0; i < monsterModels.Length; i++)
		//	{

		//		MonsterModel mm = monsterModels[i];

		//		string monsterName = mm.monsterSkeName;

		//		Transform monster = TransformManager.FindTransform(monsterName);

		//		if (monster == null)
		//		{
		//			Debug.LogFormat("未找到名为{0}的怪物", monsterName);
		//			continue;
		//		}

		//		Transform container = monster.Find("Other/MonsterSayContainer");

		//		if(container == null){
		//			Debug.LogFormat("{0} monster container not active or null", mm.monsterSkeName);
		//			continue;
		//		}

		//		SpriteRenderer sr = container.GetComponent<SpriteRenderer>();

		//		if(sr.enabled = true){
		//			Debug.LogFormat("{0} sprite background is active!", mm.monsterSkeName);
		//		}

		//		TextMeshPro tm = container.Find("Word").GetComponent<TextMeshPro>();

		//		tm.text = string.Empty;
                


		//	}

		//}

		[MenuItem("EditHelper/SaveMonstersData")]
		public static void SaveMonstersData(){

			List<MonsterData> monsterDatas = new List<MonsterData>();

			string monsterDataPath = "/Users/houlianghong/Desktop/MyGameData/monsters.csv";

            MonsterModel[] monsterModels = LoadMonsterModels(monsterDataPath);

			for (int i = 0; i < monsterModels.Length; i++)
			{
				MonsterModel monsterModel = monsterModels[i];
				MonsterData monsterData = new MonsterData();
				monsterData.agentLevel = 1;
				monsterData.monsterId = monsterModel.monsterId;
				monsterData.monsterName = monsterModel.monsterName;

				monsterData.originalMaxHealth = monsterModel.maxHealth;//基础最大生命值
				monsterData.originalAttack = monsterModel.attack;//基础物理伤害
				monsterData.originalMagicAttack = monsterModel.magicAttack;//基础魔法伤害
				monsterData.originalArmor = monsterModel.armor;//基础护甲
				monsterData.originalMagicResist = monsterModel.magicResist;//基础抗性
				monsterData.originalArmorDecrease = monsterModel.armorDecrease;//基础护甲穿刺
				monsterData.originalMagicResistDecrease = monsterModel.magicResistDecrease;//基础抗性穿刺
				monsterData.originalCrit = monsterModel.crit;//基础暴击率
				monsterData.originalDodge = monsterModel.dodge;//基础闪避率
				monsterData.originalCritHurtScaler = monsterModel.critHurtScaler;//基础暴击系数
				monsterData.attackInterval = monsterModel.attackInterval;//攻击间隔

				monsterData.rewardExperience = monsterModel.experience;//奖励的经验值
				monsterData.rewardGold = monsterModel.gold;//奖励的金钱

				monsterData.attackSpeedLevel = monsterModel.attackSpeedLevel;
				monsterData.mosnterEvaluate = monsterModel.evaluate;
				monsterData.monsterStory = monsterModel.story.Replace('+',',');

				monsterDatas.Add(monsterData);
			}


			monsterDataPath = "/Users/houlianghong/Desktop/MyGameData/boss.csv";
         
			monsterModels = LoadMonsterModels(monsterDataPath);

			for (int i = 0; i < monsterModels.Length; i++)
            {
                MonsterModel monsterModel = monsterModels[i];
                MonsterData monsterData = new MonsterData();
                monsterData.agentLevel = 1;
                monsterData.monsterId = monsterModel.monsterId;
                monsterData.monsterName = monsterModel.monsterName;

                monsterData.originalMaxHealth = monsterModel.maxHealth;//基础最大生命值
                monsterData.originalAttack = monsterModel.attack;//基础物理伤害
                monsterData.originalMagicAttack = monsterModel.magicAttack;//基础魔法伤害
                monsterData.originalArmor = monsterModel.armor;//基础护甲
                monsterData.originalMagicResist = monsterModel.magicResist;//基础抗性
                monsterData.originalArmorDecrease = monsterModel.armorDecrease;//基础护甲穿刺
                monsterData.originalMagicResistDecrease = monsterModel.magicResistDecrease;//基础抗性穿刺
                monsterData.originalCrit = monsterModel.crit;//基础暴击率
                monsterData.originalDodge = monsterModel.dodge;//基础闪避率
                monsterData.originalCritHurtScaler = monsterModel.critHurtScaler;//基础暴击系数
                monsterData.attackInterval = monsterModel.attackInterval;//攻击间隔

                monsterData.rewardExperience = monsterModel.experience;//奖励的经验值
                monsterData.rewardGold = monsterModel.gold;//奖励的金钱

				monsterData.attackSpeedLevel = monsterModel.attackSpeedLevel;
				monsterData.mosnterEvaluate = monsterModel.evaluate;
				monsterData.monsterStory = monsterModel.story.Replace('+', ',');


                monsterDatas.Add(monsterData);
            }

			string targetPath = CommonData.originDataPath + "/MonstersData.json";

			DataHandler.SaveInstanceListToFile<MonsterData>(monsterDatas, targetPath);

			Debug.Log("怪物数据存储完毕!");

		}
        
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

				monsterScript.monsterSays = mm.monsterSays;

				//monsterScript.extraPoisonHurt = 0;
            
			}

			Debug.Log("monster init finish");
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

			Debug.Log("boss init finish");

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

				mm.attackSpeedLevel = int.Parse(detailDatas[18]);

				mm.evaluate = int.Parse(detailDatas[19]);

				mm.story = detailDatas[20];

				mm.monsterSays[0] = detailDatas[21].Replace('+', ',');

				mm.monsterSays[1] = detailDatas[22].Replace('+', ',');

				mm.monsterSays[2] = detailDatas[23].Replace('+', ',');

				//PropertyType type = GetProperty(int.Parse(detailDatas[21]));
				//float value = float.Parse(detailDatas[22]);

				//mm.puzzleCorrectDecrease = new PropertySet(type, value);

				//type = GetProperty(int.Parse(detailDatas[23]));
				//value = float.Parse(detailDatas[24]);

				//mm.puzzleWrongIncrease = new PropertySet(type, value);


			}

			return monsterModels;
		}

		//private static PropertyType GetProperty(int seed){
		//	PropertyType type = PropertyType.Attack;
		//	switch(seed){
		//		case 0:
		//			type = PropertyType.Attack;
		//			break;
		//		case 1:
		//			type = PropertyType.MagicAttack;
		//			break;
		//		case 2:
		//			type = PropertyType.Armor;
		//			break;
		//		case 3:
		//			type = PropertyType.MagicResist;
		//			break;
		//		case 4:
		//			type = PropertyType.Dodge;
		//			break;
		//		case 5:
		//			type = PropertyType.Crit;
		//			break;
		//	}
		//	return type;
		//}


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

			public int attackSpeedLevel;
			public int evaluate;
			public string story;

			public string[] monsterSays = new string[3];


		}

	}
}
