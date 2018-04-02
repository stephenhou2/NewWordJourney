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

			MonsterModel[] monsterModels = LoadMonsterModels ();

			Transform other = TransformManager.FindTransform ("Other");

			for (int i = 0; i < monsterModels.Length; i++) {

				MonsterModel mm = monsterModels [i];

				string monsterName = mm.monsterSkeName;

				Transform monster = TransformManager.FindTransform (monsterName);

				if (monster == null) {
					Debug.LogFormat ("未找到名为{0}的怪物", monsterName);
					continue;
				}

				monster.gameObject.layer = 9;

				monster.GetComponent<UnityArmatureComponent>().sortingLayerName = "ItemAndRoles";

				Monster monsterScript = monster.gameObject.GetComponent<Monster> ();
				if (monsterScript == null) {
					monsterScript = monster.gameObject.AddComponent<Monster> ();
				}

				BattleMonsterController bmcScript = monster.gameObject.GetComponent<BattleMonsterController> ();
				if (bmcScript == null) {
					bmcScript = monster.gameObject.AddComponent<BattleMonsterController> ();
				}


				NormalAttack na = monster.gameObject.GetComponent<NormalAttack> ();
				if (na == null) {
					na = monster.gameObject.AddComponent<NormalAttack> ();
				}

				BoxCollider2D bc2d = monster.gameObject.GetComponent<BoxCollider2D> ();
				if (bc2d == null) {
					bc2d = monster.gameObject.AddComponent<BoxCollider2D> ();
				}

				MapMonster mmScript = monster.gameObject.GetComponent<MapMonster> ();
				if (mmScript == null) {
					mmScript = monster.gameObject.AddComponent<MapMonster> ();
				}

				Transform newOther = monster.Find ("Other");
				if (newOther == null) {
					newOther = GameObject.Instantiate (other.gameObject).transform;
					newOther.transform.SetParent (monster);

					newOther.name = "Other";

					newOther.localPosition = Vector3.zero;
					newOther.localScale = Vector3.one;
					newOther.localRotation = Quaternion.identity;

				}


				Transform skillsContainer = newOther.transform.Find ("SkillsContainer");


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

				monsterScript.poisonHurtScaler = 1f;

				monsterScript.skillsContainer = skillsContainer;

				bc2d.size = new Vector2 (0.9f, 0.9f);

				na.skillType = SkillType.Active;
				na.selfRoleAnimName = "attack";
				na.sfxName = "hitNormal";
				na.skillId = 0;
				na.skillName = "普通攻击";

				bmcScript.isIdle = true;


				bmcScript.collosionLayer = 1<<9;
				bmcScript.boxCollider = bc2d;
				bmcScript.normalAttack = na;
				Transform effectAnimContainer = newOther.transform.Find ("EffectAnimContainer");
				bmcScript.effectAnimContainer = effectAnimContainer;

				Transform alertAreasContainer = newOther.transform.Find ("AlertAreasContainer");
				Transform alertIcon = newOther.transform.Find ("AlertIcon");

				mmScript.alertAreas = alertAreasContainer.GetComponentsInChildren<MonsterAlertArea> ();
				mmScript.alertToFightIcon = alertIcon.GetComponent<SpriteRenderer> ();

				mmScript.collisionLayer = 1<<9;

				for (int j = 0; j < mmScript.alertAreas.Length; j++) {
					mmScript.alertAreas [j].mapMonster = mmScript;
				}

			}




		}

		private static MonsterModel[] LoadMonsterModels(){

			string monsterDataPath = "/Users/houlianghong/Desktop/MyGameData/monsters.csv";

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

				mm.attackInterval = float.Parse (detailDatas [4]);

				mm.gold = int.Parse (detailDatas [5]);

				mm.experience = int.Parse (detailDatas [6]);

				mm.maxHealth = int.Parse (detailDatas [7]);

				mm.attack = int.Parse (detailDatas [8]);

				mm.armor = int.Parse (detailDatas [9]);

				mm.magicAttack = int.Parse (detailDatas [10]);

				mm.magicResist = int.Parse (detailDatas [11]);

				mm.crit = float.Parse (detailDatas [12]);

				mm.dodge = float.Parse (detailDatas [13]);

				mm.armorDecrease = int.Parse (detailDatas [14]);

				mm.magicResistDecrease = int.Parse (detailDatas [15]);

				mm.critHurtScaler = float.Parse (detailDatas [16]);

//				mm.skillId = int.Parse (detailDatas [17]);
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
			public int skillId;

		}

	}
}
