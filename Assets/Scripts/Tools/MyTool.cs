using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public static class MyTool {


		public static bool isIphoneX{
			get{
				return Screen.currentResolution.height / Screen.currentResolution.width > 2;
			}
		}

		/// <summary>
		/// 世界坐标转2D画布中的坐标
		/// </summary>
		/// <returns>The point in canvas.</returns>
		/// <param name="worldPos">World position.</param>
		public static Vector3 ToPointInCanvas(Vector3 worldPos){

			Vector3 posInScreen = Camera.main.WorldToScreenPoint (worldPos);

			Vector3 posInCanvas = new Vector3 (posInScreen.x * CommonData.scalerToPresetResulotion, posInScreen.y * CommonData.scalerToPresetResulotion, posInScreen.z);

			return posInCanvas;

		}

		/// <summary>
		/// 两个点是否近似重合（x，y方向各0.1的容差）
		/// </summary>
		/// <returns><c>true</c>, if same position2 d was approximatelyed, <c>false</c> otherwise.</returns>
		/// <param name="pos1">Pos1.</param>
		/// <param name="pos2">Pos2.</param>
		public static bool ApproximatelySamePosition2D(Vector3 pos1,Vector3 pos2){

			bool posXApproximate = pos1.x >= pos2.x - 0.1f && pos1.x <= pos2.x + 0.1f;
			bool posYApproximate = pos1.y >= pos2.y - 0.1f && pos1.y <= pos2.y + 0.1f;



			return posXApproximate && posYApproximate;

		}

		/// <summary>
		/// 两个点的近似整数点是否相同
		/// </summary>
		/// <returns><c>true</c>, if same int position2 d was approximatelyed, <c>false</c> otherwise.</returns>
		/// <param name="pos1">Pos1.</param>
		/// <param name="pos2">Pos2.</param>
		public static bool ApproximatelySameIntPosition2D(Vector3 pos1,Vector3 pos2){

			int pos1_x = Mathf.RoundToInt (pos1.x);
			int pos1_y = Mathf.RoundToInt (pos1.y);

			int pos2_x = Mathf.RoundToInt (pos2.x);
			int pos2_y = Mathf.RoundToInt (pos2.y);

			return pos1_x == pos2_x && pos1_y == pos2_y;
		}

//		public static Vector3 RoundToIntPos(Vector3 oriPos){
//			int pos_x = Mathf.RoundToInt (oriPos.x);
//			int pos_y = Mathf.RoundToInt (oriPos.y);
//			int pos_z = Mathf.RoundToInt (oriPos.z);
//			return new Vector3 (pos_x, pos_y, pos_z);
//
//		}


		public static Vector3 FindNearestPos(Vector3 oriPos,List<Vector3> endPosList){

			Vector3 nearestPos = oriPos;
			float distance = float.MaxValue;

			for (int i = 0; i < endPosList.Count; i++) {

				Vector3 potentialPosition = endPosList [i];

				if (potentialPosition == oriPos) {
					continue;
				}

				float newDistance = Vector3.Magnitude (potentialPosition - oriPos);

				if (newDistance < distance) {
					nearestPos = potentialPosition;
					distance = newDistance;
				}

			}

			return nearestPos;

		}


		public static int FindNearestPosIndex(Vector3 oriPos,List<Vector3> endPosList){

			int posIndex = 0;

			float distance = float.MaxValue;

			for (int i = 0; i < endPosList.Count; i++) {

				Vector3 potentialPosition = endPosList [i];

				if (potentialPosition == oriPos) {
					continue;
				}

				float newDistance = Vector3.Magnitude (potentialPosition - oriPos);

				if (newDistance < distance) {
					posIndex = i;
					distance = newDistance;
				}

			}

			return posIndex;

		}




		private static string[] chineseNums = new string[] {"一","二","三","四","五","六","七","八","九","十","十一","十二","十三","十四","十五","十六","十七","十八","十九","二十"};


		public static string NumberToChinese(int number){

			if (number == 0 || number > 20) {
				return "";
			}

			return chineseNums [number - 1];

		}

		private static string[] normalMonsterNames = new string[]{
			"01_skeleton",
			"02_alchemist",
			"03_berserker",
			"04_ghoul",
			"05_bloody_warrior",
			"06_blue_devil",
			"07_armed_warrior",
			"08_arcane_wizard",
			"09_death_bear",
			"10_explorer",
			"11_ice_beast",
			"12_fire_wizard",
			"13_orc_warrior",
			"14_bloody_wizard",
			"15_dark_knight",
			"16_glutton",
			"17_zombie_sword",
			"18_dark_parasite",
			"19_butcher",
			"20_dark_wizard",
			"21_magic_creature",
			"22_break_orc",
			"23_heresy_wizard",
			"24_swordsman",
			"25_lava_beast",
			"26_death_wolf",
			"27_cyclops"
		};

		private static string[] bossNames = new string[] {
			"01_boss_magic_armor",
			"02_boss_commando",
			"03_boss_pharmacist",
			"04_boss_forsaken",
			"05_boss_mutant",
			"06_boss_earthen",
			"07_boss_puppet",
			"08_boss_death",
			"09_boss_demon",
			"10_boss_dragon"
		};

		public static string GetMonsterName(int monsterId){
            if(monsterId <= 0)
            {
                return string.Empty;
            }
			else if (monsterId < 100) 
            {
				return normalMonsterNames [monsterId - 1];
			} 
            else 
            {
				return bossNames [monsterId - 101];
			}
		}

		private static string[] npcNames = new string[]{
			"wiseman",
			"merchant",
			"ranger",
			"thief",
			"knight",
			"barbarian",
			"scholar",
			"wizard",
			"warrior",
			"goddess",
			"samurai",
			"assassin",
			"berserker"
		};

		public static string GetNpcName(int npcId){
	
			string npcName = npcNames [npcId];

			return npcName;
		}


		public static string GetPropertyName(PropertyType type){
			string propertyName = string.Empty;
			switch (type) {
			case PropertyType.MaxHealth:
				propertyName = "生命上限";
				break;
			case PropertyType.MaxMana:
				propertyName = "魔法上限";
				break;
			case PropertyType.Attack:
				propertyName = "物理伤害";
				break;
			case PropertyType.MagicAttack:
				propertyName = "魔法伤害";
				break;
			case PropertyType.MoveSpeed:
				propertyName = "移动速度";
				break;
			case PropertyType.Armor:
				propertyName = "护甲";
				break;
			case PropertyType.MagicResist:
				propertyName = "抗性";
				break;
			case PropertyType.ArmorDecrease:
				propertyName = "护甲穿透";
				break;
			case PropertyType.MagicResistDecrease:
				propertyName = "魔法穿透";
				break;
			case PropertyType.Crit:
				propertyName = "暴击";
				break;
			case PropertyType.Dodge:
				propertyName = "闪避";
				break;
			case PropertyType.CritHurtScaler:
				propertyName = "暴击倍率";
				break;
			case PropertyType.ExtraGold:
				propertyName = "额外金钱";
				break;
			case PropertyType.ExtraExperience:
				propertyName = "额外经验";
				break;
			case PropertyType.HealthRecovery:
				propertyName = "生命回复";
				break;
			case PropertyType.MagicRecovery:
				propertyName = "魔法回复";
				break;
			
			}
			return propertyName;
		}

		public static Dictionary<string,string> propertyChangeStrings = new Dictionary<string, string>{
			{"Status_Poison_Durative","中毒"},
			{"Status_Burn_Durative","烧伤"},
			{"Status_DecreaseAttack","攻击降低"},
			{"Status_DecreaseMana","魔法降低"},
			{"Status_DecreaseHit","命中降低"},
			{"Status_DecreaseAttackSpeed","攻速降低"},
			{"Status_DecreaseArmor","护甲降低"},
			{"Status_DecreaseMagicResist","抗性降低"},
			{"Status_DecreaseDodge","闪避提升"},
			{"Status_IncreaseAttack","攻击提升"},
			{"Status_IncreaseMana","魔法提升"},
			{"Status_IncreaseHit","命中提升"},
			{"Status_IncreaseAttackSpeed","攻速提升"},
			{"Status_IncreaseArmor","攻速提升"},
			{"Status_IncreaseMagicResist","攻速提升"},
			{"Status_IncreaseDodge","闪避提升"},
			{"Status_IncreaseCrit","暴击提升"}
		};


	}
}
