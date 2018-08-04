using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class PropertyDisplay : MonoBehaviour {
          
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

		public Image attackChangeTint;
		public Image magicAttackChangeTint;
		public Image armorChangeTint;
		public Image magicResistChangeTint;
		public Image armorDecreaseChangeTint;
		public Image magicResistDecreaseChangeTint;
		public Image critChangeTint;
		public Image dodgeChangeTint;
		public Image extraGoldChangeTint;
		public Image extraExperienceChangeTint;
		public Image healthRecoveryChangeTint;
		public Image magicRecoveryChangeTint;

		public Sprite promoteArrowSprite;
		public Sprite decreaseArrowSprite;

		private Sequence[] changeTintFromEqSequences = new Sequence[12];

		//public void UpdateStatusBars(){
		//	Player player = Player.mainPlayer;
  //          healthBar.maxValue = player.maxHealth;
  //          healthBar.value = player.health;
  //          manaBar.maxValue = player.maxMana;
  //          manaBar.value = player.mana;
  //          experienceBar.maxValue = player.upgradeExprience;
  //          experienceBar.value = player.experience;
  //          goldText.text = player.totalGold.ToString();
  //          levelText.text = string.Format("等级 Lv.{0}", player.agentLevel);         
		//}



		public void UpdatePropertyDisplay(PropertyChange pc){
			Player player = Player.mainPlayer;
			ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();
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



			ShowEquipmentChangeTint (attackChangeTint, pc.attackChange,0);
			ShowEquipmentChangeTint (magicAttackChangeTint, pc.magicAttackChange,1);
			ShowEquipmentChangeTint (armorChangeTint, pc.armorChange,2);
			ShowEquipmentChangeTint (magicResistChangeTint, pc.magicResistChange,3);
			ShowEquipmentChangeTint (armorDecreaseChangeTint, pc.armorDecreaseChange,4);
			ShowEquipmentChangeTint (magicResistDecreaseChangeTint, pc.magicResistDecreaseChange,5);
			ShowEquipmentChangeTint (critChangeTint, pc.critChange,6);
			ShowEquipmentChangeTint (dodgeChangeTint, pc.dodgeChange,7);
			ShowEquipmentChangeTint (extraGoldChangeTint, pc.extraGoldChange,8);
			ShowEquipmentChangeTint (extraExperienceChangeTint, pc.extraExperienceChange,9);
			ShowEquipmentChangeTint (healthRecoveryChangeTint, pc.healthRecoveryChange,10);
			ShowEquipmentChangeTint (magicRecoveryChangeTint, pc.magicRecoveryChange,11);

		}



		private void ShowEquipmentChangeTint(Image changeTint,float change,int indexInPanel){

			int changeResult = CheckPropertyChange (change);

			changeTint.enabled = false;

			if (changeTintFromEqSequences [indexInPanel] != null && !changeTintFromEqSequences [indexInPanel].IsComplete()) {
				changeTintFromEqSequences [indexInPanel].Complete ();
			}

			if (changeResult == 0) {
				return;
			}

			changeTint.enabled = true;

			changeTint.sprite = changeResult > 0 ? promoteArrowSprite : decreaseArrowSprite;

			changeTint.color = changeResult > 0 ? Color.green : Color.red;


			if (changeTintFromEqSequences[indexInPanel] == null) {
				Sequence changeTintSequence = DOTween.Sequence ();
				changeTintSequence
					.Append(changeTint.DOFade (0.2f, 1))
					.Append(changeTint.DOFade (1f, 1))
					.AppendCallback(()=>{
						changeTint.enabled = false;
					});
				changeTintSequence.SetUpdate (true);
				changeTintFromEqSequences[indexInPanel] = changeTintSequence;
				changeTintSequence.SetAutoKill (false);
				return;
			}

			changeTintFromEqSequences [indexInPanel].Restart ();

		}

		private int CheckPropertyChange(float change){
			int changeResult = 0;
			if (change > 0) {
				changeResult = 1;
			} else if (change < 0) {
				changeResult = -1;
			}
			return changeResult;
		}

		public void ClearPropertyDisplay(){
			for (int i = 0; i < changeTintFromEqSequences.Length; i++) {
				changeTintFromEqSequences [i].Kill (false);
				changeTintFromEqSequences [i] = null;
			}
		}

	}
}
