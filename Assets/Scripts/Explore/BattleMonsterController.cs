using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

    using Transform = UnityEngine.Transform;

    /// <summary>
    /// 怪物主动技能类【只有boss才有主动技能，按照概率释放技能】
    /// </summary>
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

        //怪物所有主动技能
        public MonsterActiveSkill[] activeSkills ;

		private IEnumerator latelyEnterFightCouroutine;


		protected override void Awake(){

			agent = GetComponent<Monster> ();
            
            // 绑定龙骨
			if((agent as Monster).isBoss){
				modelActive = transform.Find("DragonBones").gameObject;
			}else{
				modelActive = this.gameObject;
			}


			base.Awake ();

		}

		void Start(){
			// 初始化动画播放倍率为1.0
			SetRoleAnimTimeScale (1.0f);
		}
        
        /// <summary>
		/// 停止播放骨骼动画
        /// </summary>
		public void KillRoleAnim(){
			armatureCom.animation.Stop ();
			isIdle = false;
		}


        /// <summary>
        /// 初始化怪物
        /// </summary>
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
		public void StartFight(BattlePlayerController bpCtr){

            // 如果已经在战斗中，则直接返回
			if (isInFight)
            {
                return;
            }

			boxCollider.enabled = false;

            isInFight = true;

            // 清除所有的技能回调
			ClearAllSkillCallBacks ();

            // 初始化所有被动技能回调
			InitTriggeredPassiveSkillCallBacks (this,bpCtr);

			if(latelyEnterFightCouroutine != null){
				StopCoroutine(latelyEnterFightCouroutine);
			}

			latelyEnterFightCouroutine = LatelyEnterFight();
			StartCoroutine(latelyEnterFightCouroutine);

		}

        /// <summary>
        /// 自动按照预设概率选择释放主动技能【boss】
        /// </summary>
        /// <returns>The random active skill.</returns>
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

        /// <summary>
        /// 怪物比玩家角色延迟0.3s进入战斗
        /// </summary>
        /// <returns>The enter fight.</returns>
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

                // 死亡后不再执行后续逻辑
				if(isDead){
					return;
				}

                // 攻击动画结束后播放攻击间隔动画
				this.PlayRoleAnim(CommonData.roleAttackIntervalAnimName,0,null);

			});

		}
				
        
        /// <summary>
        /// 执行技能逻辑
        /// </summary>
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

            // 播放技能特效
			currentUsingActiveSkill.AffectAgents (this, enemy);

            // 更新自己的状态显示
			UpdateStatusPlane ();

			bool fightEnd = CheckFightEnd();

			if (enemy == null) {
				return;
			}

            // 敌方更新状态显示
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

        /// <summary>
        /// 更新状态显示
        /// </summary>
		public override void UpdateStatusPlane(){
			bmUICtr.UpdateAgentStatusPlane ();
		}
        

        /// <summary>
        /// 退出战斗
        /// </summary>
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

            // 防止死亡逻辑重复执行
			if (isDead) {
				return;
			}
            
            // 禁止屏幕点击
			exploreManager.DisableExploreInteractivity ();

			isDead = true;

			if(enemy != null){
				enemy.isIdle = true;
				enemy.QuitFight();
			}

			MapMonster mm = GetComponent<MapMonster>();
            // 禁止进行探测
			mm.DisableAllDetect();
            // 添加到本层事件记录中
			mm.AddToCurrentMapEventRecord();

            // 退出战斗
            QuitFight();

            // 战斗中玩家获胜
			exploreManager.BattlePlayerWin(new Transform[] { transform });
         
            // 退出战斗界面
			exploreManager.expUICtr.QuitFight();

            // 播放死亡动画及死亡逻辑
			IEnumerator latelyDieCoroutine = LatelyDie();
			StartCoroutine (latelyDieCoroutine);
		}


        /// <summary>
        /// 播放死亡动画后执行死亡逻辑
        /// </summary>
        /// <returns>The die.</returns>
		private IEnumerator LatelyDie(){

			yield return new WaitForSeconds (0.1f);

			GetComponent<MapWalkableEvent> ().ResetWhenDie ();
            
			this.armatureCom.animation.Stop ();

			PlayRoleAnim (CommonData.roleDieAnimName, 1, delegate {

                // 回收
				MapWalkableEvent mwe = GetComponent<MapWalkableEvent>();
				if(mwe is MapMonster){
					mwe.AddToPool(exploreManager.newMapGenerator.monstersPool);
				}else if (mwe is MapNPC){
					mwe.AddToPool(exploreManager.newMapGenerator.npcsPool);
				}
            
                // 技能特效回收
				AllEffectAnimsIntoPool();

                // 敌方的一些资源回收和特效播放
				if(enemy != null){
					// 特效回收
					enemy.AllEffectAnimsIntoPool();
                    // 如果处在隐身状态下，播放隐身动画
					if ((enemy as BattlePlayerController).fadeStepsLeft > 0)
                    {
                        enemy.SetEffectAnim(CommonData.yinShenEffectName, null, 0, 0);
                    }
                    enemy = null;
				}
            

			});
		}

        /// <summary>
        /// 角色朝向左方
        /// </summary>
        /// <param name="andWait">是否在设置完朝向后播放等待动画</param>
		public override void TowardsLeft (bool andWait = true)
		{
			armatureCom.armature.flipX = true;
			towards = MyTowards.Left;
		}

		/// <summary>
        /// 角色朝向右方
        /// </summary>
        /// <param name="andWait">是否在设置完朝向后播放等待动画</param>
		public override void TowardsRight (bool andWait = true)
		{
			armatureCom.armature.flipX = false;
			towards = MyTowards.Right;
		}

		/// <summary>
        /// 角色朝向上方
        /// </summary>
        /// <param name="andWait">是否在设置完朝向后播放等待动画</param>
		public override void TowardsUp (bool andWait = true)
		{
			towards = MyTowards.Up;
		}


		/// <summary>
        /// 角色朝向下方
        /// </summary>
        /// <param name="andWait">是否在设置完朝向后播放等待动画</param>
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



	}
}
