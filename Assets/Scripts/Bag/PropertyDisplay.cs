using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class PropertyDisplay : MonoBehaviour {

		public HLHFillBar healthBar;
		public HLHFillBar manaBar;
		public HLHFillBar experienceBar;

		public Text goldText;
		public Text levelText;

		public Text attackText;
		public Text magicAttackText;
		public Text armorText;
		public Text magicResistText;
		public Text armorDecreaseText;
		public Text magicResistDecreaseText;
		public Text critText;
		public Text dodgeText;
		public Text extraGoldText;
		public Text extraExperienceText;
		public Text healthRecoveryText;
		public Text magicRecoveryText;

		public void UpdatePropertyDisplay(){
			Player player = Player.mainPlayer;
			healthBar.maxValue = player.maxHealth;
			healthBar.value = player.health;
			manaBar.maxValue = player.maxMana;
			manaBar.value = player.mana;
			experienceBar.maxValue = player.upgradeExprience;
			experienceBar.value = player.experience;
			goldText.text = player.totalGold.ToString ();
			levelText.text = string.Format ("等级 Lv.{0}", player.agentLevel);
			attackText.text = player.attack.ToString ();
			magicAttackText.text = player.magicAttack.ToString ();
			armorText.text = player.armor.ToString ();
			magicResistText.text = player.magicResist.ToString ();
			armorDecreaseText.text = player.armorDecrease.ToString ();
			magicResistDecreaseText.text = player.magicResistDecrease.ToString ();
			critText.text = string.Format ("{0}%", (player.crit * 100).ToString("F1"));
			dodgeText.text = string.Format ("{0}%", (player.dodge * 100).ToString("F1"));
			extraGoldText.text = player.extraGold.ToString ();
			extraExperienceText.text = player.extraExperience.ToString ();
			healthRecoveryText.text = player.healthRecovery.ToString ();
			magicRecoveryText.text = player.magicRecovery.ToString ();
		}

	}
}
