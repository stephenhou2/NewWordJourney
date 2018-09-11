using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	using UnityEngine.UI;

	public class RebuildPlayerView : ZoomHUD
    {

		public Text maxHealthText;
		public Text maxManaText;
		public Text attackText;
		public Text magicAttackText;
		public Text armorText;
		public Text magicResistText;
		public Text dodgeText;
		public Text critText;

		public Text remainPointText;

		public Text maxHealthPointText;
		public Text maxManaPointText;
		public Text attackPointText;
		public Text magicAttackPointText;
		public Text armorPointText;
		public Text magicResistPointText;
		public Text dodgePointText;
		public Text critPointText;

		private int maxHealthPoint;
		private int maxManaPoint;
		private int attackPoint;
		private int magicAttackPoint;
		private int armorPoint;
		private int magicResistPoint;
		private int dodgePoint;
		private int critPoint;

		private int maxHealth = 800;
		private int maxMana = 100;
		private int attack = 5;
		private int magicAttack = 5;
		private int armor = 5;
		private int magicResist = 5;
		private float dodge = 0;
		private float crit = 0;

		private int pointLeft;

		public Transform queryQuitHUD;

		public Transform queryResetHUD;

		public Text queryText;

		private CallBack quitCallBack;
        
		public void SetUpRebuildPlayerView(CallBack quitCallBack){

			this.quitCallBack = quitCallBack;

			Reset();

			SetUpDisplay();

			queryResetHUD.gameObject.SetActive(false);
			queryQuitHUD.gameObject.SetActive(false);
			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);

		}


		private void SetUpDisplay(){

			maxHealthText.text = maxHealth.ToString();
			maxManaText.text = maxMana.ToString();
			attackText.text = attack.ToString();
			magicAttackText.text = magicAttack.ToString();
			armorText.text = armor.ToString();
			magicResistText.text = magicResist.ToString();
			dodgeText.text = string.Format("{0}%",(dodge * 100).ToString("0.0"));
			critText.text = string.Format("{0}%",(crit * 100).ToString("0.0"));

			remainPointText.text = string.Format("剩余可用点数: <color=orange>{0}</color>", pointLeft);

			maxHealthPointText.text = maxHealthPoint.ToString();
			maxManaPointText.text = maxManaPoint.ToString();
			attackPointText.text = attackPoint.ToString();
			magicAttackPointText.text = magicAttack.ToString();
			armorPointText.text = armorPoint.ToString();
			magicResistPointText.text = magicResistPoint.ToString();
			dodgePointText.text = dodgePoint.ToString();
			critPointText.text = critPoint.ToString();



		}

		public void AddPointAt(int index){
			if(pointLeft <= 0){
				return;
			}
			switch(index){
				case 0:
					maxHealth += 20;
					maxHealthPoint++;
					break;
				case 1:
					maxMana += 5;
					maxManaPoint++;
					break;
				case 2:
					attack++;
					attackPoint++;
					break;
				case 3:
					magicAttack++;
					magicAttackPoint++;
					break;
				case 4:
					armor++;
					armorPoint++;
					break;
				case 5:
					magicResist++;
					magicResistPoint++;
					break;
				case 6:
					dodge += 0.003f + float.Epsilon;
					dodgePoint++;
					break;
				case 7:
					crit += 0.003f + float.Epsilon;
					critPoint++;
					break;
			}
			pointLeft--;
			SetUpDisplay();
		}

		public void RemovePointAt(int index){
			
			switch (index)
			{
				case 0:
					if(maxHealthPoint>0){
						maxHealth -= 20;
						maxHealthPoint--;
						pointLeft++;
					}
					break;
				case 1:
					if (maxManaPoint > 0)
                    {
						maxMana -= 5;
						maxManaPoint--;
                        pointLeft++;
                    }
					break;
				case 2:
					if (attackPoint > 0)
                    {
						attack--;
						attackPoint--;
                        pointLeft++;
                    }
					break;
				case 3:
					if (magicAttackPoint > 0)
                    {
						magicAttack--;
						magicAttackPoint--;
                        pointLeft++;
                    }
					break;
				case 4:
					if (armorPoint > 0)
                    {
						armor--;
						armorPoint--;
                        pointLeft++;
                    }
					break;
				case 5:
					if (magicResistPoint > 0)
                    {
						magicResist--;
						magicResistPoint--;
                        pointLeft++;
                    }
					break;
				case 6:
					if (dodgePoint > 0)
                    {
						dodge -= 0.003f - float.Epsilon;
						dodgePoint--;
                        pointLeft++;
                    }
					break;
				case 7:
					if (critPoint > 0)
                    {
						crit -= 0.003f - float.Epsilon;
						critPoint--;
                        pointLeft++;
                    }
					break;
			}

			SetUpDisplay();

		}

		public void OnConfirmButtonClick(){

			if(pointLeft > 0){
				queryText.text = string.Format("角色属性重设未完成\n<color=orange>剩余可分配点数:{0}</color>\n是否确认退出？", pointLeft);
				queryQuitHUD.gameObject.SetActive(true);
				return;
			}

			queryResetHUD.gameObject.SetActive(true);
         
		}

		public void OnConfirmResetButtonClick(){
			queryResetHUD.gameObject.SetActive(false);
			QuitRebuildView();
		}

		public void OnCancelResetButtonClick(){
			queryResetHUD.gameObject.SetActive(false);
		}

		public void OnConfirmQuitButtonClick(){
			
			queryQuitHUD.gameObject.SetActive(false);
			QuitRebuildView();
		}

		public void OnCancelQuitButtonClick(){
			queryQuitHUD.gameObject.SetActive(false);
		}




		public void OnResetAllButtonClick(){

			Reset();
			SetUpDisplay();

		}

		private void Reset()
		{
			maxHealthPoint = 0;
            maxManaPoint = 0;
            attackPoint = 0;
            magicAttackPoint = 0;
            armorPoint = 0;
            magicResistPoint = 0;
            dodgePoint = 0;
            critPoint = 0;


            maxHealth = 800;
            maxMana = 100;
            attack = 5;
            magicAttack = 5;
            armor = 5;
            magicResist = 5;
            dodge = 0;
            crit = 0;

            pointLeft = 50;
		}

		private void QuitRebuildView(){

			ResetPlayerDataAndSave();

			if (inZoomingOut)
            {
                return;
            }

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut(delegate {            
				if(quitCallBack != null){
					quitCallBack();
				}
				ExploreManager.Instance.EnableExploreInteractivity();
				ExploreManager.Instance.battlePlayerCtr.SetEffectAnim(CommonData.levelUpEffectName);
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.levelUpAudioName);
			});

			StartCoroutine(zoomCoroutine);
		}

		private void ResetPlayerDataAndSave(){

			Player.mainPlayer.InitializeMapIndex();

			PlayerData playerData = new PlayerData(Player.mainPlayer);

			playerData.agentLevel = 1;


			playerData.maxHealth = maxHealth;
            playerData.originalMaxHealth = maxHealth;
            playerData.health = maxHealth;

            playerData.originalMaxMana = maxMana;
            playerData.maxMana = maxMana;
            playerData.mana = maxMana;

            playerData.originalAttack = attack;
            playerData.attack = attack;

            playerData.originalMagicAttack = magicAttack;
            playerData.magicAttack = magicAttack;

            playerData.originalArmor = armor;
            playerData.armor = armor;

            playerData.originalMagicResist = magicResist;
            playerData.magicResist = magicResist;

            playerData.originalDodge = dodge;
            playerData.dodge = dodge;

            playerData.originalCrit = crit;
            playerData.crit = crit;

         
			playerData.originalAttackSpeed = AttackSpeed.Slow;

			playerData.originalMoveSpeed = 20;


			playerData.originalArmorDecrease = 0;
			playerData.originalMagicResistDecrease = 0;



			playerData.originalCritHurtScaler = 1.5f;
			playerData.originalPhysicalHurtScaler = 1f;
			playerData.originalMagicalHurtScaler = 1f;

			playerData.originalExtraGold = 0;
			playerData.originalExtraExperience = 0;

			playerData.originalHealthRecovery = 0;
			playerData.originalMagicRecovery = 0;

         
			playerData.attackSpeed = AttackSpeed.Slow;
			playerData.moveSpeed = 20;


			playerData.armorDecrease = 0;
			playerData.magicResistDecrease = 0;
            

			playerData.critHurtScaler = 1.5f;
			playerData.physicalHurtScaler = 1f;
			playerData.magicalHurtScaler = 1f;

			playerData.extraGold = 0;
			playerData.extraExperience = 0;

			playerData.healthRecovery = 0;
			playerData.magicRecovery = 0;


			playerData.allEquipmentsInBag.Clear();
            playerData.allConsumablesInBag.Clear();
            playerData.allSkillScrollsInBag.Clear();
            playerData.allSpecialItemsInBag.Clear();
            playerData.allLearnedSkillsRecord.Clear();
            playerData.allPropertyGemstonesInBag.Clear();
            

			playerData.maxUnlockLevelIndex = 0;
			playerData.currentLevelIndex = 0;


			playerData.totalGold = 0;
			playerData.experience = 0;

			playerData.isNewPlayer = false;
			playerData.needChooseDifficulty = true;

			playerData.skillNumLeft = 0;

			playerData.luckInOpenTreasure = 0;
			playerData.luckInMonsterTreasure = 0;

			DataHandler.SaveInstanceDataToFile<PlayerData>(playerData, CommonData.oriPlayerDataFilePath);



		}

	}
}

