using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using DragonBones;
	using Transform = UnityEngine.Transform;
	using TMPro;
		
	public class BattleMonsterController : BattleAgentController {

		// 怪物UI控制器
		private BattleMonsterUIController mBmUICtr;
		private BattleMonsterUIController bmUICtr{
			get{

				if (mBmUICtr == null) {
					Transform canvas = TransformManager.FindTransform ("ExploreCanvas");
					mBmUICtr = canvas.GetComponent<BattleMonsterUIController> ();
				}

				return mBmUICtr;
			}
		}

		// 玩家控制器
//		private BattlePlayerController bpCtr;


//		public Vector3 originalPos;

		// 是否可以交谈
//		public bool canTalk;



//		public LearnWord[] wordsArray;


		protected override void Awake(){

			agent = GetComponent<Monster> ();

			modelActive = this.gameObject;

			base.Awake ();

		}

		void Start(){
			SetRoleAnimTimeScale (1.0f);
		}

		public void KillRoleAnim(){
			armatureCom.animation.Stop ();
		}

		public void SetAlive(){
			boxCollider.enabled = true;
			PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);
			SetSortingOrder (-Mathf.RoundToInt(transform.position.y));
		}




		/// <summary>
		/// 初始化碰到的怪物
		/// </summary>
		public void InitMonster(Transform monsterTrans){

			Monster monster = agent as Monster;

			bmUICtr.monster = monster;

			// 初始化怪物状态栏
			bmUICtr.SetUpMonsterStatusPlane (monster);

			agentUICtr = ExploreManager.Instance.expUICtr.GetComponent<BattleMonsterUIController> ();



		}
			

		public void SetEnemy(BattlePlayerController bpCtr){
			this.enemy = bpCtr;
		}



		/// <summary>
		/// 怪物进入战斗
		/// </summary>
		/// <param name="bpCtr">Bp ctr.</param>
		/// <param name="playerWinCallBack">Player window call back.</param>
		public void StartFight(BattlePlayerController bpCtr){

			boxCollider.enabled = false;

			ClearAllSkillCallBacks ();

			InitTriggeredPassiveSkillCallBacks (this,bpCtr);

			// 怪物比玩家延迟0.3s进入战斗状态
			Invoke ("Fight", 0.3f);

		}
			
		/// <summary>
		/// 角色战斗逻辑
		/// </summary>
		public override void Fight(){
			currentUsingActiveSkill = normalAttack;
			UseSkill (currentUsingActiveSkill);
		}


			
		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		public override void UseSkill (ActiveSkill skill)
		{
			if (attackCoroutine != null) {
				StopCoroutine (attackCoroutine);
			}
			currentUsingActiveSkill = skill;

			// 播放技能对应的角色动画，角色动画结束后播放攻击间隔动画
			this.PlayRoleAnim (skill.selfRoleAnimName, 1, () => {
				// 播放等待动画
				this.PlayRoleAnim(CommonData.roleAttackIntervalAnimName,0,null);
			});

		}
			


		protected override void AgentExcuteHitEffect ()
		{
			
			GameManager.Instance.soundManager.PlayAudioClip ("Skill/" + currentUsingActiveSkill.sfxName);

			currentUsingActiveSkill.AffectAgents (this, enemy);

			UpdateStatusPlane ();
			enemy.UpdateStatusPlane ();

//			isAttackActionFinish = true;

			if (enemy == null) {
				return;
			}

			// 如果战斗没有结束，则默认在攻击间隔时间之后按照默认攻击方式进行攻击
			if(!CheckFightEnd()){
				currentUsingActiveSkill = normalAttack;
//				Debug.Log (currentSkill);
				attackCoroutine = InvokeAttack (currentUsingActiveSkill);
				StartCoroutine (attackCoroutine);
			}
				

//			this.UpdateStatusPlane();
//
//			bpCtr.UpdateStatusPlane();

//			Player player = Player.mainPlayer;

//			switch (currentSkill.hurtType) {
//			case HurtType.Physical:
//
//				// 玩家受到物理攻击，已装备的护具中随机一个护具的耐久度降低
//				List<Equipment> allEquipedProtector = player.allEquipedEquipments.FindAll (delegate (Equipment obj) {
//					int equipmentTypeToInt = (int)obj.equipmentType;
//					return equipmentTypeToInt >= 1 && equipmentTypeToInt <= 5;
//				});
//
//				if (allEquipedProtector.Count == 0) {
//					break;
//				}
//
//				int randomIndex = Random.Range (0, allEquipedProtector.Count);
//
//				Equipment damagedEquipment = allEquipedProtector [randomIndex];
//
//				bool completeDamaged = damagedEquipment.EquipmentDamaged (EquipmentDamageSource.BePhysicalAttacked);
//
//				if (completeDamaged) {
//					string tint = string.Format("{0}完全损坏",damagedEquipment.itemName);
//					bmUICtr.GetComponent<ExploreUICotroller> ().SetUpTintHUD (tint);
//
//				}
//
//				break;
//			case HurtType.Magical:
//
//				List<Equipment> allEquipedOrnaments = player.allEquipedEquipments.FindAll (delegate(Equipment obj) {
//					int equipmentTypeToInt = (int)obj.equipmentType;
//					return equipmentTypeToInt >= 5 && equipmentTypeToInt <= 6;
//				});
//
//				if (allEquipedOrnaments.Count == 0) {
//					break;
//				}
//
//				randomIndex = Random.Range (0, allEquipedOrnaments.Count);
//
//				damagedEquipment = allEquipedOrnaments [randomIndex];
//
//				completeDamaged = damagedEquipment.EquipmentDamaged (EquipmentDamageSource.BeMagicAttacked);
//
//				if (completeDamaged) {
//					string tint = string.Format("{0}完全损坏",damagedEquipment.itemName);
//					bmUICtr.GetComponent<ExploreUICotroller> ().SetUpTintHUD (tint);
//				}
//				break;
//			default:
//				break;
//			}
		}

//		public override void InitFightTextDirectionTowards (BattleAgentController enemy)
//		{
//			MyTowards fightTextTowards = MyTowards.Left;
//
//			if (enemy.transform.position.y == transform.position.y) {
//				fightTextTowards = enemy.transform.position.x < transform.position.x ? MyTowards.Right : MyTowards.Left;
//			} else {
//				fightTextTowards = towards == MyTowards.Left ? MyTowards.Right : MyTowards.Left;
//			}
//
//			bmUICtr.fightTextManager.SetUpFightTextManager (transform.position, fightTextTowards);
//		}

//		public override void InitFightTextDirectionTowards (Vector3 position)
//		{
//			MyTowards fightTextTowards = MyTowards.Left;
//
//			fightTextTowards = position.x < transform.position.x ? MyTowards.Right : MyTowards.Left;
//
//			bmUICtr.fightTextManager.SetUpFightTextManager (transform.position, fightTextTowards);
//
//		}



//		public override void ShowFightTextInOrder ()
//		{
//			bmUICtr.fightTextManager.ShowFightTextInOrder ();
//		}


		/// <summary>
		/// 判断战斗是否结束
		/// </summary>
		public override bool CheckFightEnd(){

			if (enemy.agent.health <= 0) {
				enemy.AgentDie ();
				return true;
			} else if (agent.health <= 0) {
				AgentDie ();
				return true;
			}else {
				return false;
			}

		}

		public override void UpdateStatusPlane(){
			bmUICtr.UpdateAgentStatusPlane ();
		}


		public override void QuitFight ()
		{
			StopCoroutinesWhenFightEnd ();
			boxCollider.enabled = true;
			enemy = null;
			currentUsingActiveSkill = null;
			SetRoleAnimTimeScale (1.0f);
			agent.ResetBattleAgentProperties (false);
		}

		/// <summary>
		/// 怪物死亡
		/// </summary>
		override public void AgentDie(){

			if (agent.isDead) {
				return;
			}

			agent.isDead = true;

			StartCoroutine ("LatelyDie");
		}

		private IEnumerator LatelyDie(){

			yield return new WaitForSeconds (0.1f);

			ExploreUICotroller expUICtr = bmUICtr.GetComponent<ExploreUICotroller> ();

			GetComponent<MapWalkableEvent> ().ResetWhenDie ();

			expUICtr.QuitFight ();

			enemy.QuitFight();

			QuitFight ();

			this.armatureCom.animation.Stop ();

			exploreManager.BattlePlayerWin (new Transform[]{ transform });

			PlayRoleAnim (CommonData.roleDieAnimName, 1, delegate {
				MapWalkableEvent mwe = GetComponent<MapWalkableEvent>();
				if(mwe is MapMonster){
					mwe.AddToPool(exploreManager.newMapGenerator.monstersPool);
				}else if (mwe is MapNPC){
					mwe.AddToPool(exploreManager.newMapGenerator.npcsPool);
				}
			});
		}

		public override void TowardsLeft (bool andWait = true)
		{
			armatureCom.armature.flipX = true;
			towards = MyTowards.Left;
		}

		public override void TowardsRight (bool andWait = true)
		{
			armatureCom.armature.flipX = false;
			towards = MyTowards.Right;
		}

		public override void TowardsUp (bool andWait = true)
		{
			towards = MyTowards.Up;
		}

		public override void TowardsDown(bool andWait = true){
			towards = MyTowards.Down;
		}

		void OnDestroy(){
//			StopAllCoroutines ();
//			mBmUICtr = null;
//			bpCtr = null;
//			agent = null;
//			propertyCalculator = null;
//			mExploreManager = null;
//			expUICtr = null;
//			enemy = null;
//			for (int i = 0; i < beforeFightTriggerExcutors.Count; i++) {
//				beforeFightTriggerExcutors [i].OnClear ();
//			}
//			for (int i = 0; i < attackTriggerExcutors.Count; i++) {
//				attackTriggerExcutors [i].OnClear ();
//			}
//			for (int i = 0; i < beAttackedTriggerExcutors.Count; i++) {
//				beAttackedTriggerExcutors [i].OnClear ();
//			}
//			for (int i = 0; i < hitTriggerExcutors.Count; i++) {
//				hitTriggerExcutors [i].OnClear ();
//			}
//			for (int i = 0; i < beHitTriggerExcutors.Count; i++) {
//				beHitTriggerExcutors [i].OnClear ();
//			}
//			for (int i = 0; i < fightEndTriggerExcutors.Count; i++) {
//				fightEndTriggerExcutors [i].OnClear ();
//			}
//			currentSkill = null;
//			for (int i = 0; i < activeSkills.Count; i++) {
//				Destroy (activeSkills [i].gameObject);
//			}
		}

	}
}
