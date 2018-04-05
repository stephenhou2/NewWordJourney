using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney
{
	public class BattlePlayerUIController : BattleAgentUIController {

		private Player player;

		public Button[] equipedConsumablesButtons;

		public HLHFillBar manaBar;
		public HLHFillBar experienceBar;
		public Text playerLevelText;

		public ConsumablesDisplay consDisplay;


		public Transform activeSkillButtonContainer;
		public InstancePool activeSkillButtonPool;
		public Transform activeSkillButtonModel;




		private BattlePlayerController mBpCtr;
		private BattlePlayerController bpCtr{
			get{
				if (mBpCtr == null) {
					mBpCtr = player.transform.Find("BattlePlayer").GetComponent<BattlePlayerController> ();
				}
				return mBpCtr;
			}
		}


		public Transform miniMap;

		public Transform directionArrow;

		public Transform levelUpPlane;



		/// <summary>
		/// 初始化探索界面中玩家UI
		/// 包括：人物状态栏 底部物品栏 战斗中的技能栏 所有消耗品显示栏
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="skillSelectCallBack">Skill select call back.</param>
		public void SetUpExplorePlayerView(Player player){

			this.player = player;

			healthBar.InitHLHFillBar (player.maxHealth, player.health);
			manaBar.InitHLHFillBar (player.maxMana, player.mana);
			experienceBar.InitHLHFillBar (player.upgradeExprience, player.experience);

			consDisplay.InitConsumablesDisplay (UpdateAgentStatusPlane);

			SetUpConsumablesButtons ();

			miniMap.gameObject.SetActive (player.hasCompass);
		}


		protected void UpdateManaBarAnim(Agent agent){
			manaBar.maxValue = player.maxMana;
			manaBar.value = player.mana;
		}

		protected void UpdateExperienceBarAnim(Agent agent){
			experienceBar.maxValue = player.upgradeExprience;
			experienceBar.value = player.experience;
		}


		/// <summary>
		/// 更新人物状态栏
		/// </summary>
		public override void UpdateAgentStatusPlane(){

			UpdateHealthBarAnim(player);
			UpdateManaBarAnim (player);
			UpdateExperienceBarAnim (player);
			playerLevelText.text = player.agentLevel.ToString();
			UpdateSkillStatusPlane (player);

		}


		public void RefreshMiniMap(){

			if (!Player.mainPlayer.hasCompass) {
				return;
			}
			
			Vector3 directionVector = ExploreManager.Instance.newMapGenerator.GetDirectionVectorTowardsExit ();

			directionArrow.localRotation = Quaternion.FromToRotation (new Vector3 (0, 1, 0), directionVector);

		}

		public void SetUpFightPlane(){
			SetUpActiveSkillButtons ();
		}

		/// <summary>
		/// 退出战斗时重用物体进缓存池
		/// </summary>
		public override void QuitFightPlane(){
			activeSkillButtonPool.AddChildInstancesToPool (activeSkillButtonContainer);
			statusTintPool.AddChildInstancesToPool (statusTintContainer);
		}

		/// <summary>
		/// 初始化主动技能按钮
		/// </summary>
		private void SetUpActiveSkillButtons(){
			for (int i = 0; i < player.attachedActiveSkills.Count; i++) {
				ActiveSkill skill = player.attachedActiveSkills [i];
				ActiveSkillButton activeSkillButton = activeSkillButtonPool.GetInstance<ActiveSkillButton> (activeSkillButtonModel.gameObject, activeSkillButtonContainer);
				int index = i;
				activeSkillButton.SetUpActiveSkillButton (skill, index, activeSkillButtonContainer);
				activeSkillButton.AddListener (OnActiveSkillButtonClick);
			}
		}

		/// <summary>
		/// 玩家点击主动技能的响应
		/// </summary>
		/// <param name="index">Index.</param>
		private void OnActiveSkillButtonClick(int index){

			ActiveSkill skill = player.attachedActiveSkills [index];

			bpCtr.UseSkill (skill);

			player.mana -= skill.manaConsume;

		}

		/// <summary>
		/// 背包按钮点击响应
		/// </summary>
		public void OnBagButtonClick(){

			Time.timeScale = 0;

			// 初始化背包界面并显示
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
				Transform bagCanvas = TransformManager.FindTransform("BagCanvas");
				bagCanvas.GetComponent<BagViewController>().SetUpBagView(true);
			}, false,true);

		}




		public void OnProduceButtonClick(){
			Time.timeScale = 0f;
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(null,null);
			}, false, true);
		}

		public void SetUpConsumablesButtons(){
			consDisplay.SetUpConsumablesButtons ();
		}
				

		/// <summary>
		/// 显示升级时的属性强化面板
		/// </summary>
		public void ShowLevelUpPlane(){
			levelUpPlane.gameObject.SetActive (true);
			ExploreManager.Instance.AllWalkableEventsStopMove ();
		}

		/// <summary>
		/// 玩家升级时选择的强化类型
		/// 【0:血量 1:攻击 2:魔法 3:护甲 4:抗性 5:魔法攻击】
		/// </summary>
		/// <param name="type">Type.</param>
		public void SelectPropertyPromote(int type){
			switch (type) {
			case 0:
				int oriMaxHealthRecord = player.originalMaxHealth;
				player.originalMaxHealth += 10;
				player.health = (int)((float)player.originalMaxHealth / oriMaxHealthRecord * player.health);
				break;
			case 1:
				player.originalAttack += 1;
				player.attack += 1;
				break;
			case 2:
				int oriMaxManaRecord = player.originalMaxMana;
				player.originalMaxMana += 5;
				player.mana = (int)((float)player.originalMaxMana / oriMaxManaRecord * player.mana);
				break;
			case 3:
				player.originalArmor += 1;
				player.armor += 1;
				break;
			case 4:
				player.originalMagicResist += 1;
				player.magicResist += 1;
				break;
			case 5:
				player.originalMagicAttack += 1;
				player.magicAttack += 1;
				break;
			}
			GetComponent<ExploreUICotroller> ().HideMask ();
			UpdateAgentStatusPlane ();
			levelUpPlane.gameObject.SetActive (false);
			ExploreManager.Instance.AllWalkableEventsStartMove ();
		}
			

	}
}
