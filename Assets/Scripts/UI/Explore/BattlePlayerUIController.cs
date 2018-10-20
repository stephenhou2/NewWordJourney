using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEngine.UI;
    using DG.Tweening;
	using Transform = UnityEngine.Transform;

	public class BattlePlayerUIController : BattleAgentUIController {

		private Player player;

		public Button[] equipedConsumablesButtons;

		public HLHFillBar manaBar;
		public HLHFillBar experienceBar;
		public Text playerLevelText;
		public Text playerGoldText;

		public ConsumablesDisplay consDisplay;


		public Transform escapeBar;
		public Image escapeBarFill;

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
			playerGoldText.text = player.totalGold.ToString();
			playerLevelText.text = player.agentLevel.ToString();

            consDisplay.InitConsumablesDisplay (delegate{
                UpdateAgentStatusPlane();
            });

			SetUpConsumablesButtons ();


		}

		public void RefreshGold(){
			playerGoldText.text = player.totalGold.ToString();
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
			playerGoldText.text = player.totalGold.ToString();

		}

        

		public void SetUpFightPlane(){
			escapeBar.gameObject.SetActive (false);
			InitActiveSkills();
			InitAllActiveSkillButtons();
			SetUpActiveSkillButtons ();         
		}

		public void EscapeDisplay(float escapeTime,CallBack escapeCallBack){

			escapeBarFill.fillAmount = 0f;

			escapeBar.gameObject.SetActive (true);

			escapeBarFill.DOFillAmount (1.0f, escapeTime).OnComplete(() => {
				if(escapeCallBack != null){
					escapeCallBack();
				}
			});
		}

		public void StopEscapeDisplay(){
			escapeBarFill.DOKill(false);
			escapeBar.gameObject.SetActive(false);
		}

		/// <summary>
		/// 退出战斗时重用物体进缓存池
		/// </summary>
		public override void QuitFightPlane(){
			
			for (int i = 0; i < activeSkillButtonContainer.childCount;i++){
				activeSkillButtonContainer.GetChild(i).GetComponent<ActiveSkillButton>().Reset();
			}

			activeSkillButtonPool.AddChildInstancesToPool (activeSkillButtonContainer);

		}

        /// <summary>
        /// 战斗过程中如果遗忘技能，需要将该主动技能的按钮移除
        /// </summary>
		public void RemoveActiveSkillButton(Skill skill){
			
			for (int i = 0; i < activeSkillButtonContainer.childCount; i++)
            {
                ActiveSkillButton activeSkillButton = activeSkillButtonContainer.GetChild(i).GetComponent<ActiveSkillButton>();
            
				if(activeSkillButton.skill.skillId == skill.skillId){
					activeSkillButton.Reset();
					activeSkillButtonPool.AddInstanceToPool(activeSkillButton.gameObject);
				}
            }

            // 移除技能按钮时要更新技能按钮的点击响应，因为响应方法中有一个序号参数需要更新
			//（进入战斗初始化的时候按照技能顺序给按钮定了序号，点击时根据序号参数获得使用的是哪个技能，遗忘技能后该序号参数应该也更新一次）
			for (int i = 0; i < player.attachedActiveSkills.Count; i++)
            {          
				
                ActiveSkill activeSkill = player.attachedActiveSkills[i];
				ActiveSkillButton activeSkillButton = activeSkillButtonContainer.GetChild(i).GetComponent<ActiveSkillButton>();
                int index = i;

				float coolenPercentage = activeSkillButton.mask.fillAmount;
				activeSkill.coolenPercentage = (int)(coolenPercentage * 100);


				activeSkillButton.SetUpActiveSkillButton(activeSkill, index, activeSkillButtonContainer);
                activeSkillButton.AddListener(OnActiveSkillButtonClick);
            }

		}

		public void InitActiveSkills(){

			for (int i = 0; i < player.attachedActiveSkills.Count; i++)
            {
                ActiveSkill activeSkill = player.attachedActiveSkills[i];
                activeSkill.coolenPercentage = 0;
                activeSkill.skillStatus = ActiveSkillStatus.None;            
            }         
		}

		public void InitAllActiveSkillButtons(){
         
			for (int i = 0; i < activeSkillButtonContainer.childCount; i++)
			{
				ActiveSkillButton activeSkillButton = activeSkillButtonContainer.GetChild(i).GetComponent<ActiveSkillButton>();

				activeSkillButton.validTint.gameObject.SetActive(false);

				activeSkillButton.Reset();
              
			}

		}

		/// <summary>
		/// 初始化主动技能按钮
		/// </summary>
		public void SetUpActiveSkillButtons()
		{
			
			for (int i = 0; i < activeSkillButtonContainer.childCount; i++)
            {
                ActiveSkillButton activeSkillButton = activeSkillButtonContainer.GetChild(i).GetComponent<ActiveSkillButton>();

				float coolenPercentage = activeSkillButton.mask.fillAmount;
				player.attachedActiveSkills[i].coolenPercentage = (int)(coolenPercentage * 100);


            }

			for (int i = 0; i < activeSkillButtonContainer.childCount; i++)
            {
                activeSkillButtonContainer.GetChild(i).GetComponent<ActiveSkillButton>().Reset();
            }     
            
			activeSkillButtonPool.AddChildInstancesToPool(activeSkillButtonContainer);

         
			if (bpCtr.towards == MyTowards.Left || bpCtr.towards == MyTowards.Right) { 
				
    			for (int i = 0; i < player.attachedActiveSkills.Count; i++)
    			{
    				ActiveSkill skill = player.attachedActiveSkills[i];
    				ActiveSkillButton activeSkillButton = activeSkillButtonPool.GetInstance<ActiveSkillButton>(activeSkillButtonModel.gameObject, activeSkillButtonContainer);
    				int index = i;
					//Debug.LogFormat("skill:{0},coolenPercentage:{1}", player.attachedActiveSkills[i].skillName, player.attachedActiveSkills[i].coolenPercentage);
    				activeSkillButton.SetUpActiveSkillButton(skill, index, activeSkillButtonContainer);
    				activeSkillButton.AddListener(OnActiveSkillButtonClick);
    			}
		    }
		}


		/// <summary>
		/// 玩家点击主动技能的响应
		/// </summary>
		/// <param name="index">Index.</param>
		private void OnActiveSkillButtonClick(int index){

			if(bpCtr.isDead){
				return;
			}

			ActiveSkill skill = player.attachedActiveSkills [index];

			bpCtr.UseSkill (skill);         
		}

        /// <summary>
        /// 玩家角色的攻击动作结束时更新技能的可点击状态
        /// </summary>
		public void UpdateActiveSkillButtonsWhenAttackAnimFinish(){

            for (int i = 0; i < activeSkillButtonContainer.childCount; i++)
            {
                ActiveSkillButton activeSkillButton = activeSkillButtonContainer.GetChild(i).GetComponent<ActiveSkillButton>();

				ActiveSkill activeSkill = player.attachedActiveSkills[i];
				//float coolenPercentage = activeSkillButton.mask.fillAmount;

				activeSkillButton.UpdateActiveSkillButton(activeSkill);



                
            }

		}

		/// <summary>
		/// 背包按钮点击响应
		/// </summary>
		public void OnBagButtonClick(){

			if(ExploreManager.Instance.expUICtr.rejectNewUI){
				return;
			}

			Time.timeScale = 0;
            
			if(ExploreManager.Instance.battlePlayerCtr.isInMoving){
				ExploreManager.Instance.battlePlayerCtr.StopMoveAtEndOfCurrentStep();
            }

			ExploreManager.Instance.expUICtr.rejectNewUI = true;

			// 初始化背包界面并显示
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
				Transform bagCanvas = TransformManager.FindTransform("BagCanvas");
				bagCanvas.GetComponent<BagViewController>().SetUpBagView(true,delegate {               
					ExploreManager.Instance.expUICtr.rejectNewUI = false;
				});
			}, false,true);

		}




//		public void OnProduceButtonClick(){
//			Time.timeScale = 0f;
//			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
//				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(null,null);
//			}, false, true);
//		}

		public void SetUpConsumablesButtons(){
			consDisplay.SetUpConsumablesButtons ();
		}
				

		/// <summary>
		/// 显示升级时的属性强化面板
		/// </summary>
		public void ShowLevelUpPlane(){
			levelUpPlane.gameObject.SetActive (true);
			ExploreManager.Instance.MapWalkableEventsStopAction ();
		}

		/// <summary>
		/// 玩家升级时选择的强化类型
		/// 【0:血量 1:攻击 2:魔法 3:护甲 4:抗性 5:魔法攻击】
		/// </summary>
		/// <param name="type">Type.</param>
		public void SelectPropertyPromote(int type){
			switch (type) {
			case 0:
				int maxHealthRecord = player.maxHealth;
				player.originalMaxHealth += 20;
				player.maxHealth += 20;
				player.health = (int)((float)player.maxHealth / maxHealthRecord * player.health);
					bpCtr.SetEffectAnim(CommonData.healthAddUpEffectName, null, 1, 0, true);
				break;
			case 1:
				player.originalAttack += 2;
				player.attack += 2;
					bpCtr.SetEffectAnim(CommonData.attackUpEffectName, null, 1, 0, true);
				break;
			case 2:
				player.originalMagicAttack += 2;
                player.magicAttack += 2;
					bpCtr.SetEffectAnim(CommonData.magicAttackUpEffectName, null, 1, 0, true);
				break;
			case 3:
				player.originalArmor += 2;
				player.armor += 2;
					bpCtr.SetEffectAnim(CommonData.armorUpEffectName, null, 1, 0, true);
				break;
			case 4:
				player.originalMagicResist += 2;
				player.magicResist += 2;
					bpCtr.SetEffectAnim(CommonData.magicResistUpEffectName, null, 1, 0, true);
				break;
			}

			UpdateAgentStatusPlane ();
			levelUpPlane.gameObject.SetActive (false);
			ExploreManager.Instance.MapWalkableEventsStartAction ();
            ExploreManager.Instance.EnableExploreInteractivity ();
			GameManager.Instance.soundManager.PlayAudioClip(CommonData.propertyPromotionAudioName);

			//bpCtr.SetEffectAnim(CommonData.propertyIncreaseEffectName);
		}
			

	}
}
