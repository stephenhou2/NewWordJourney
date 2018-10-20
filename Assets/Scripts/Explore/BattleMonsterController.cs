﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

    using Transform = UnityEngine.Transform;

    [System.Serializable]
    public struct MonsterActiveSkill{
        public ActiveSkill skill;
        public int probabilityx100;
    }

		
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

        public MonsterActiveSkill[] activeSkills ;

		private IEnumerator latelyEnterFightCouroutine;


		protected override void Awake(){

			agent = GetComponent<Monster> ();
            
			if((agent as Monster).isBoss){
				modelActive = transform.Find("DragonBones").gameObject;
			}else{
				modelActive = this.gameObject;
			}


			base.Awake ();

		}

		void Start(){
			SetRoleAnimTimeScale (1.0f);
		}

		public void KillRoleAnim(){
			armatureCom.animation.Stop ();
			isIdle = false;
		}


		public void SetAlive(){
			boxCollider.enabled = true;
            isInFight = false;
            isDead = false;
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

			if (isInFight)
            {
                return;
            }

			boxCollider.enabled = false;

            isInFight = true;

			ClearAllSkillCallBacks ();

			InitTriggeredPassiveSkillCallBacks (this,bpCtr);

			if(latelyEnterFightCouroutine != null){
				StopCoroutine(latelyEnterFightCouroutine);
			}

			latelyEnterFightCouroutine = LatelyEnterFight();
			StartCoroutine(latelyEnterFightCouroutine);

		}

        private ActiveSkill AutoRandomActiveSkill()
        {

            ActiveSkill randomAS = normalAttack;

            int joint = 0;

            List<int> jointsList = new List<int>();

            for (int i = 0; i < activeSkills.Length; i++)
            {

                MonsterActiveSkill monsterActiveSkill = activeSkills[i];

                joint += monsterActiveSkill.probabilityx100;

                jointsList.Add(joint);

            }

            int randomSeed = Random.Range(0, joint);

            for (int i = 0; i < jointsList.Count; i++)
            {
                if(i <= jointsList.Count - 1 && randomSeed <= jointsList[i]){
                    randomAS = activeSkills[i].skill;
                    break;
                }  
            }

            return randomAS;

        }
			
		/// <summary>
		/// 角色战斗逻辑
		/// </summary>
		public void Fight(){
			currentUsingActiveSkill = AutoRandomActiveSkill();
			UseSkill (currentUsingActiveSkill);
		}

		private IEnumerator LatelyEnterFight(){
			yield return new WaitForSeconds(0.3f);
			Fight();
		}

			
		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		public override void UseSkill (ActiveSkill skill)
		{
			if (isDead)
            {
                return;
            }

			if (attackCoroutine != null) {
				StopCoroutine (attackCoroutine);
			}
			currentUsingActiveSkill = skill;

			// 播放技能对应的角色动画，角色动画结束后播放攻击间隔动画
			this.PlayRoleAnim (skill.selfRoleAnimName, 1, () => {

				if(isDead){
					return;
				}
				this.PlayRoleAnim(CommonData.roleAttackIntervalAnimName,0,null);

			});

		}
				


		protected override void AgentExcuteHitEffect ()
		{
			if(isDead){
                return;
            }

			if (enemy == null) {
				return;
			}

            if(!isInFight){
                return;
            }
			
			if (currentUsingActiveSkill.sfxName != string.Empty)
            {
                // 播放技能对应的音效
                GameManager.Instance.soundManager.PlayAudioClip(currentUsingActiveSkill.sfxName);
            }

            // 技能效果
			currentUsingActiveSkill.AffectAgents (this, enemy);
            // 技能特效
			//currentUsingActiveSkill.SetEffectAnims(this, enemy);

			UpdateStatusPlane ();

			bool fightEnd = CheckFightEnd();

			if (enemy == null) {
				return;
			}

			enemy.UpdateStatusPlane();

			// 如果战斗没有结束，则默认在攻击间隔时间之后按照默认攻击方式进行攻击
			if((enemy as BattlePlayerController).isInFight && !enemy.isDead && !fightEnd){
				// 播放等待动画            
				currentUsingActiveSkill = AutoRandomActiveSkill();
				attackCoroutine = InvokeAttack (currentUsingActiveSkill);
				StartCoroutine (attackCoroutine);
			}

		}


		/// <summary>
		/// 判断战斗是否结束
		/// </summary>
		public override bool CheckFightEnd(){

			if (agent.health <= 0)
            {
                AgentDie();
                return true;
            }

			if (enemy == null)
            {
                return true;
            }

            if (enemy.agent.health <= 0)
            {
                MapMonster mm = GetComponent<MapMonster>();
                if (mm != null) { 
                    mm.isReadyToFight = false;
                }
				enemy.AgentDie ();
				return true;
			} 

			return false;
         
		}

		public override void UpdateStatusPlane(){
			bmUICtr.UpdateAgentStatusPlane ();
		}
        

		public override void QuitFight ()
		{
			for (int i = 0; i < activeSkills.Length;i++){
				activeSkills[i].skill.hasTriggered = false;
			}

			StopCoroutinesWhenFightEnd ();
			GetComponent<MapWalkableEvent> ().isTriggered = false;
            isInFight = false;
			currentUsingActiveSkill = null;
			SetRoleAnimTimeScale (1.0f);
			agent.ResetBattleAgentProperties (false);
		}

		/// <summary>
		/// 怪物死亡
		/// </summary>
		override public void AgentDie(){

			if (isDead) {
				return;
			}
         
			exploreManager.DisableExploreInteractivity ();

			isDead = true;

			if(enemy != null){
				enemy.isIdle = true;
				enemy.QuitFight();
			}

			MapMonster mm = GetComponent<MapMonster>();
			mm.DisableAllDetect();
			mm.AddToCurrentMapEventRecord();

         
            QuitFight();

			exploreManager.BattlePlayerWin(new Transform[] { transform });
         
			exploreManager.expUICtr.QuitFight();

			IEnumerator latelyDieCoroutine = LatelyDie();
			StartCoroutine (latelyDieCoroutine);
		}

		private IEnumerator LatelyDie(){

			yield return new WaitForSeconds (0.1f);

			GetComponent<MapWalkableEvent> ().ResetWhenDie ();
            
			this.armatureCom.animation.Stop ();

			PlayRoleAnim (CommonData.roleDieAnimName, 1, delegate {
				
				MapWalkableEvent mwe = GetComponent<MapWalkableEvent>();
				if(mwe is MapMonster){
					mwe.AddToPool(exploreManager.newMapGenerator.monstersPool);
				}else if (mwe is MapNPC){
					mwe.AddToPool(exploreManager.newMapGenerator.npcsPool);
				}
            
				AllEffectAnimsIntoPool();
				if(enemy != null){
					enemy.AllEffectAnimsIntoPool();
					if ((enemy as BattlePlayerController).fadeStepsLeft > 0)
                    {
                        enemy.SetEffectAnim(CommonData.yinShenEffectName, null, 0, 0);
                    }
                    enemy = null;
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

        /// <summary>
        /// 获取怪物的龙骨朝向[怪物探测方向分上下左右,但是怪物实际朝向只有左右]
        /// </summary>
        /// <returns>The monster bone towards.</returns>
		public MyTowards GetMonsterBoneTowards(){
			MyTowards myTowards = MyTowards.Right;
			if(armatureCom.armature.flipX){
				myTowards = MyTowards.Left;
			}
			return myTowards;
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
