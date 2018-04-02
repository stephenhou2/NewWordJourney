using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney
{
	public class BattlePlayerUIController : BattleAgentUIController {

		private Player player;

		public Text coinCount;

		public Button[] equipedConsumablesButtons;

		public HLHFillBar manaBar;

//		public Transform skillsContainer;
//		private Transform skillButtonModel;
	
		/**********  ConsumablesPlane UI *************/
		public Transform consumablesInBagPlane;
		public Transform consumablesInBagContainer;
		public Transform consumablesButtonModel;
		public InstancePool consumablesButtonPool;
		/**********  ConsumablesPlane UI *************/

		public Button allConsumablesButton;


		public Transform activeSkillButtonContainer;
		public InstancePool activeSkillButtonPool;
		public Transform activeSkillButtonModel;


//		private ExploreManager mExploreManager;
//		private ExploreManager exploreManager{
//			get{
//				if (mExploreManager == null) {
//					mExploreManager = ExploreManager.Instance.GetComponent<ExploreManager>();
//				}
//				return mExploreManager;
//			}
//		}

		private BattlePlayerController mBpCtr;
		private BattlePlayerController bpCtr{
			get{
				if (mBpCtr == null) {
					mBpCtr = player.transform.Find("BattlePlayer").GetComponent<BattlePlayerController> ();
				}
				return mBpCtr;
			}
		}

		public Transform directionArrow;


		private int consumablesCountInOnePage = 6;

		private int currentConsumablesPage;

		public Button nextPageButton;
		public Button lastPageButton;

		public Transform levelUpPlane;



		/// <summary>
		/// 初始化探索界面中玩家UI
		/// 包括：人物状态栏 底部物品栏 战斗中的技能栏 所有消耗品显示栏
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="skillSelectCallBack">Skill select call back.</param>
		public void SetUpExplorePlayerView(Player player){

//			if (consumablesButtonPool == null) {
//				consumablesButtonPool = InstancePool.GetOrCreateInstancePool ("ConsumablesButtonPool", CommonData.exploreScenePoolContainerName);
//			}
			
			currentConsumablesPage = 0;

			this.player = player;

			healthBar.InitHLHFillBar (player.maxHealth, player.health);
			manaBar.InitHLHFillBar (player.maxMana, player.mana);

			coinCount.text = player.totalGold.ToString ();

			SetUpBottomConsumablesButtons ();

		}


		/// <summary>
		/// 初始化人物状态栏
		/// </summary>
		private void SetUpPlayerStatusPlane(){

			healthBar.maxValue = player.maxHealth;
			healthBar.value = player.health;
			coinCount.text = player.totalGold.ToString ();

		}

		protected void UpdateManaBarAnim(Agent agent){
			manaBar.maxValue = player.maxMana;
			manaBar.value = player.mana;
		}


		/// <summary>
		/// 更新人物状态栏
		/// </summary>
		public override void UpdateAgentStatusPlane(){
	
			coinCount.text = player.totalGold.ToString ();

			UpdateHealthBarAnim(player);
			UpdateManaBarAnim (player);
			UpdateSkillStatusPlane (player);

//			if (bpCtr.isInFight) {
//				attackCheckController.UpdateHealth ();
//			}
		}


		public void RefreshMiniMap(){
			
			Vector3 directionVector = ExploreManager.Instance.newMapGenerator.GetDirectionVectorTowardsExit ();

//			float rotation = Mathf.

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

			UpdateSkillButtonsStatus ();

		}

		private void UpdateSkillButtonsStatus(){



		}

			
		private void UpdateCoin(){
			coinCount.text = player.totalGold.ToString ();
		}

		/// <summary>
		/// 更新底部物品栏状态
		/// </summary>
		public void SetUpBottomConsumablesButtons(){

			int totalConsumablesCount = player.allConsumablesInBag.Count;

			for (int i = 0; i < equipedConsumablesButtons.Length; i++) {

				Button equipedConsumablesButton = equipedConsumablesButtons [i];

				if (i < totalConsumablesCount) {

					Consumables consumables = player.allConsumablesInBag [i];

					equipedConsumablesButton.GetComponent<ConsumablesInBagCell> ().SetUpConsumablesInBagCell (consumables);

				} else {
					equipedConsumablesButton.GetComponent<ConsumablesInBagCell> ().SetUpConsumablesInBagCell (null);
				}

			}
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
			

		/// <summary>
		/// 打开所有消耗品界面的 箭头按钮 的点击响应
		/// </summary>
		public void OnShowConsumablesInBagButtonClick(){

			currentConsumablesPage = 0;

			// 如果箭头朝下，则退出所有消耗品显示界面
			if (allConsumablesButton.transform.localRotation != Quaternion.identity) {

				QuitConsumablesInBagPlane ();

				return;

			}

			allConsumablesButton.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 180));

			Time.timeScale = 0f;

			// 箭头朝上，初始化剩余的消耗品显示界面
			SetUpConsumablesInBagPlane ();

			consumablesInBagPlane.gameObject.SetActive (true);

		}

		private void UpdatePageButtonStatus(){

			bool nextButtonEnable = player.allConsumablesInBag.Count > equipedConsumablesButtons.Length + (currentConsumablesPage + 1) * consumablesCountInOnePage;
			bool lastButtonEnable = currentConsumablesPage >= 1;

			nextPageButton.gameObject.SetActive (nextButtonEnable);
			lastPageButton.gameObject.SetActive (lastButtonEnable);

		}

		/// <summary>
		/// 初始化所有消耗品显示界面
		/// </summary>
		public void SetUpConsumablesInBagPlane(){
			
			UpdatePageButtonStatus ();

			consumablesButtonPool.AddChildInstancesToPool (consumablesInBagContainer);

			if (player.allConsumablesInBag.Count <= equipedConsumablesButtons.Length) {
				return;
			}
				
			int firstIndexOfCurrentPage = equipedConsumablesButtons.Length + currentConsumablesPage * consumablesCountInOnePage; 

			int firstIndexOfNextPage = firstIndexOfCurrentPage + consumablesCountInOnePage;

			int endIndexOfConsumablesInCurrentPage = player.allConsumablesInBag.Count < firstIndexOfNextPage ? player.allConsumablesInBag.Count - 1 : firstIndexOfNextPage - 1;

			for (int i = firstIndexOfCurrentPage; i <= endIndexOfConsumablesInCurrentPage; i++) {

				Consumables consumables = Player.mainPlayer.allConsumablesInBag [i];

				Button consumablesButton = consumablesButtonPool.GetInstance<Button> (consumablesButtonModel.gameObject, consumablesInBagContainer);

				consumablesButton.GetComponent<ConsumablesInBagCell> ().SetUpConsumablesInBagCell (consumables);

			}

		}

		public void OnNextPageButtonClick(){
			currentConsumablesPage++;
			SetUpConsumablesInBagPlane ();
		}

		public void OnLastPageButtonClick(){
			currentConsumablesPage--;
			SetUpConsumablesInBagPlane ();
		}



		public void OnEquipedConsumablesButtonClick(int indexInPanel){
			Consumables consumables = player.allConsumablesInBag [indexInPanel];
			OnConsumablesButtonClick (consumables);
		}


		public void OnConsumablesButtonClick(Consumables consumables){

			player.health += consumables.healthGain;
			player.mana += consumables.manaGain;

			player.RemoveItem (consumables, 1);

			SetUpBottomConsumablesButtons ();
			SetUpConsumablesInBagPlane ();

			UpdateAgentStatusPlane ();

			QuitConsumablesInBagPlane ();

		}


		public void OnProduceButtonClick(){
			Time.timeScale = 0f;
			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
				TransformManager.FindTransform("SpellCanvas").GetComponent<SpellViewController>().SetUpSpellViewForCreate(null,null);
			}, false, true);
		}


		/// <summary>
		/// 退出所有消耗品显示栏
		/// </summary>
		public void QuitConsumablesInBagPlane(){

			Time.timeScale = 1f;

			allConsumablesButton.transform.localRotation = Quaternion.identity;

			consumablesInBagPlane.gameObject.SetActive (false);

		}

		/// <summary>
		/// 更新底部物品栏和人物状态栏
		/// </summary>
		public void UpdateItemButtonsAndCoins(){

			SetUpBottomConsumablesButtons ();
			UpdateCoin ();

		}

			


		public override void PrepareForRefreshment ()
		{
			base.PrepareForRefreshment ();
			QuitConsumablesInBagPlane ();
		}



		/// <summary>
		/// 显示升级时的属性强化面板
		/// </summary>
		public void ShowLevelUpPlane(){
			levelUpPlane.gameObject.SetActive (true);
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
		}
			

		void OnDestroy(){
//			consumablesButtonPool = null;
//			fightTextManager = null;
//			fightTextPool = null;
//			statusTintPool = null;
//			mExploreManager = null;
//			mBpCtr = null;
//			attackCheckController = null;
		}

	}
}
