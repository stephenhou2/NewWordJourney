using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{

	using Transform = UnityEngine.Transform;
	using DragonBones;
	using DG.Tweening;

	public delegate void SkillCallBack(BattleAgentController selfBaCtr,BattleAgentController enemyBaCtr);


    /// <summary>
    /// 被动技能执行器
    /// </summary>
	public class TriggeredSkillExcutor{
		public TriggeredPassiveSkill triggeredSkill;
		public SkillCallBack triggeredCallback;

		public TriggeredSkillExcutor(TriggeredPassiveSkill ts,SkillCallBack callBack){
			this.triggeredSkill = ts;
			this.triggeredCallback = callBack;
		}

		public void Clear(){
			triggeredSkill = null;
			triggeredCallback = null;
		}
	}

    /// <summary>
    /// 角色控制器
    /// </summary>
	[RequireComponent(typeof(NormalAttack))]
	public abstract class BattleAgentController : MonoBehaviour {

		// 控制的角色
		[HideInInspector]public Agent agent;
        
        // 敌方角色控制器
		public BattleAgentController enemy;

        // 角色UI控制
		protected BattleAgentUIController agentUICtr;

		public bool isIdle;// 是否在空闲状态中

        public bool isInFight; //是否在战斗中

        public bool isDead;// 是否已经死亡

		// 进入战斗前触发事件回调队列
		public List<TriggeredSkillExcutor> beforeFightTriggerExcutors = new List<TriggeredSkillExcutor>();

		// 角色攻击触发事件回调队列
		public List<TriggeredSkillExcutor> attackTriggerExcutors = new List<TriggeredSkillExcutor>();

		// 角色攻击命中触发的时间回调队列
		public List<TriggeredSkillExcutor> hitTriggerExcutors = new List<TriggeredSkillExcutor>();

		// 角色被攻击触发事件回调队列
		public List<TriggeredSkillExcutor> beAttackedTriggerExcutors = new List<TriggeredSkillExcutor>();

		// 角色被攻击命中触发的事件回调队列
		public List<TriggeredSkillExcutor> beHitTriggerExcutors = new List<TriggeredSkillExcutor>();

		// 战斗结束触发事件回调队列
		public List<TriggeredSkillExcutor> fightEndTriggerExcutors = new List<TriggeredSkillExcutor>();


		// 碰撞检测层
		public LayerMask collosionLayer;

		// 碰撞检测包围盒
		public BoxCollider2D boxCollider;

		// 激活的龙骨状态模型
		protected GameObject modelActive;

		// 自动攻击协程
		public IEnumerator attackCoroutine;
		// 等待角色动画结束协程
		public IEnumerator waitRoleAnimEndCoroutine;

        // 动画结束后的回调
		protected CallBack animEndCallBack;

		// 角色的普通攻击技能
		public NormalAttack normalAttack;

        // 当前正在使用的主动技能
		protected ActiveSkill currentUsingActiveSkill;

        // 单步移动周期
		public float moveDuration { 
			get 
			{ 
				return agent.moveSpeed / (agent.moveSpeed /0.26f - 19.7803f); 
			}
		}

		// 骨骼动画控制器
		protected UnityArmatureComponent armatureCom{
			get{
				return modelActive.GetComponent<UnityArmatureComponent> ();
			}
		}

        // 探索场景控制器
		protected ExploreManager exploreManager
		{
			get{ 
				return ExploreManager.Instance; 
			}
		}
        
        // 角色朝向
		public MyTowards towards;

        // 特效动画容器
		public Transform effectAnimContainer;

		//特效动画在y轴方向上的调整系数（按照实际场景中相对玩家角色的大小【高度1.2】来定）
        public float effectAnimYScaler;


		protected virtual void Awake(){

			boxCollider = GetComponent <BoxCollider2D> ();

			// 角色攻击关键帧添加监听
			ListenerDelegate<EventObject> keyFrameListener = KeyFrameMessage;

			if (this is BattleMonsterController ) {
				armatureCom.AddEventListener (EventObject.FRAME_EVENT, keyFrameListener);
			} else if (this is BattlePlayerController) {
				BattlePlayerController bpctr = this as BattlePlayerController;
				UnityArmatureComponent playerSideArmature = bpctr.playerSide.GetComponent<UnityArmatureComponent> ();
				UnityArmatureComponent playerForwardArmature = bpctr.playerForward.GetComponent<UnityArmatureComponent> ();
				UnityArmatureComponent playerBackwardArmature = bpctr.playerBackWard.GetComponent<UnityArmatureComponent> ();
				playerSideArmature.AddEventListener(EventObject.FRAME_EVENT, keyFrameListener);
				playerForwardArmature.AddEventListener(EventObject.FRAME_EVENT, keyFrameListener);
				playerBackwardArmature.AddEventListener(EventObject.FRAME_EVENT, keyFrameListener);
			}

            isDead = false;

		}

		/// <summary>
		/// 初始化角色身上的触发型被动技能的回调
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		public void InitTriggeredPassiveSkillCallBacks(BattleAgentController self,BattleAgentController enemy){
			self.ClearAllSkillCallBacks();
			for (int i = 0; i < self.agent.attachedTriggeredSkills.Count; i++) {
				Skill skill = self.agent.attachedTriggeredSkills [i];
				skill.AffectAgents (self, enemy);
			}
		}
			

		/// <summary>
		/// 添加伤害文字动画到队列
		/// </summary>
		/// <param name="text">Text.</param>
		public void AddHurtTextToQueue(string text,MyTowards towards){

            // 角色的世界位置转化为在画布中的位置
			Vector3 basePosition = MyTool.ToPointInCanvas(transform.position);

            // 根据分辨率不同调整屏幕位置
			basePosition = new Vector3(basePosition.x * CommonData.scalerToPresetHW, basePosition.y, 0);

            // 生成探索文本
			ExploreText ft = new ExploreText (text,towards,basePosition);
         
            // 将探索文本加入伤害文本队列
			agentUICtr.exploreTextManager.AddHurtText(ft);

		}


		/// <summary>
		/// 添加纵向提示文字动画到队列
		/// </summary>
		/// <param name="text">Text.</param>
		public void AddTintTextToQueue (string text)
		{
			// 角色的世界位置转化为在画布中的位置
			Vector3 basePosition = MyTool.ToPointInCanvas (transform.position);
			// 根据分辨率不同调整屏幕位置
			basePosition = new Vector3(basePosition.x * CommonData.scalerToPresetHW, basePosition.y, 0);
			// 生成探索文本
			ExploreText ft = new ExploreText (text,MyTowards.Left,basePosition);
			// 将探索文本加入提示文本队列
			agentUICtr.exploreTextManager.AddHintText (ft);
		}


        /// <summary>
        /// 获取角色朝向的反方向
        /// </summary>
        /// <returns>The reversed towards.</returns>
		public MyTowards GetReversedTowards(){
			
			MyTowards reversedTowards = MyTowards.Left;

			switch (towards) {
			case MyTowards.Left:
				reversedTowards = MyTowards.Right;
				break;
			case MyTowards.Right:
				reversedTowards = MyTowards.Left;
				break;
			case MyTowards.Up:
				reversedTowards = MyTowards.Right;
				break;
			case MyTowards.Down:
				reversedTowards = MyTowards.Right;
				break;
			}

			return reversedTowards;
		}


		/// <summary>
		/// 受到伤害并播放伤害文字动画
		/// </summary>
		/// <param name="hurt">Hurt.</param>
		/// <param name="type">Type.</param>
		/// <param name="specialAttack">Special attack.</param>
		public void AddHurtAndShow(int hurt,HurtType type,MyTowards textTowards){

            // 至少有1点伤害值
			if(hurt == 0){
				hurt = 1;
			}

			string hurtString = string.Empty;

			agent.health -= hurt;
         
			string colorText = string.Empty;
         
            // 根据伤害类型添加颜色富文本
			switch (type) {
			case HurtType.Physical:
					if (this is BattlePlayerController)
                    {
                        colorText = "red";
                    }
                    else
                    {
						colorText = "white";
                    }
					hurtString = string.Format ("<color={0}>{1}</color>", colorText, hurt);
				break;
			case HurtType.Magical:
					if (this is BattlePlayerController)
                    {
						colorText = "cyan";
                    }
                    else
                    {
						colorText = "lightblue";
                    }
					hurtString = string.Format ("<color={0}>{1}</color>", colorText, hurt);
				break;

			}
             
            // 加入到伤害文本队列
			AddHurtTextToQueue (hurtString,textTowards);

		}

		/// <summary>
		/// 回血并播放回血文字动画
		/// </summary>
		/// <param name="gain">Gain.</param>
		public void AddHealthGainAndShow(int gain){

			agent.health += gain;

			string healthGainString = string.Format ("<color=green><size=55>{0}</size></color>", gain);

			AddTintTextToQueue (healthGainString);

		}

		/// <summary>
        /// 回魔并播放回魔文字动画
        /// </summary>
        /// <param name="gain">Gain.</param>
		public void AddManaGainAndShow(int gain){

			agent.mana += gain;

			string manaGainString = string.Format("<color=blue><size=55>{0}</size></color>", gain);

			AddTintTextToQueue(manaGainString);

		}

        /// <summary>
        /// 设置角色骨骼层级
        /// </summary>
        /// <param name="order">Order.</param>
		public virtual void SetSortingOrder(int order){
			armatureCom.sortingOrder = order;
		}
        

        /// <summary>
        /// 设置角色动画时间比例
        /// </summary>
        /// <param name="scale">Scale.</param>
		public void SetRoleAnimTimeScale(float scale){         
			armatureCom.animation.timeScale = scale;         
		}

        /// <summary>
        /// 获得角色动画的时间比例
        /// </summary>
        /// <returns>The role animation time scaler.</returns>
		public float GetRoleAnimTimeScaler(){
			return armatureCom.animation.timeScale;
		}
			
        /// <summary>
        /// 监听到的关键帧消息
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="eventObject">Event object.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		protected void KeyFrameMessage<T>(string key,T eventObject){

			EventObject frameObject = eventObject as EventObject;

			if (frameObject.name == "hit") {
				AgentExcuteHitEffect ();
			} else {
				Debug.LogError ("事件帧消息名称必须是hit");
			}

		}

        // 执行技能逻辑
		protected abstract void AgentExcuteHitEffect ();

        //设置朝向
		public abstract void TowardsLeft(bool andWait = true);
		public abstract void TowardsRight(bool andWait = true);
		public abstract void TowardsUp (bool andWait = true);
		public abstract void TowardsDown (bool andWait = true);

        /// <summary>
        /// 判断是否在橘色动画播放过程中
        /// </summary>
        /// <returns><c>true</c>, if in animation playing was ised, <c>false</c> otherwise.</returns>
		public bool IsInAnimPlaying()
        {
            return armatureCom.animation.isPlaying;
        }
        
        /// <summary>
        /// 移除角色身上指定触发型被动技能的执行器
        /// </summary>
        /// <param name="skill">Skill.</param>
		public void RemoveTriggeredSkillExcutor(TriggeredPassiveSkill skill){

            // 非战斗状态下不执行
            if(!isInFight){
                return;
            }

            // 更具技能的触发时机查询移除技能执行器
			if (skill.beforeFightTrigger) {
				int index = beforeFightTriggerExcutors.FindIndex (delegate(TriggeredSkillExcutor obj) {
					return obj.triggeredSkill == skill;
				});
				beforeFightTriggerExcutors.RemoveAt (index);
			}

			if (skill.attackTrigger) {
				int index = attackTriggerExcutors.FindIndex (delegate(TriggeredSkillExcutor obj) {
					return obj.triggeredSkill == skill;
				});
				attackTriggerExcutors.RemoveAt (index);
			}

			if (skill.hitTrigger) {
				int index = hitTriggerExcutors.FindIndex (delegate(TriggeredSkillExcutor obj) {
					return obj.triggeredSkill == skill;
				});
				hitTriggerExcutors.RemoveAt (index);
			}

			if (skill.beAttackTrigger) {
				int index = beAttackedTriggerExcutors.FindIndex (delegate(TriggeredSkillExcutor obj) {
					return obj.triggeredSkill == skill;
				});
				beAttackedTriggerExcutors.RemoveAt (index);
			}

			if (skill.beHitTrigger) {
				int index = beHitTriggerExcutors.FindIndex (delegate(TriggeredSkillExcutor obj) {
					return obj.triggeredSkill == skill;
				});
				beHitTriggerExcutors.RemoveAt (index);
			}

			if (skill.fightEndTrigger) {
				int index = fightEndTriggerExcutors.FindIndex (delegate(TriggeredSkillExcutor obj) {
					return obj.triggeredSkill == skill;
				});
				fightEndTriggerExcutors.RemoveAt (index);
			}

		}


		/// <summary>
		/// 攻击间隔时间后触发指定技能
		/// </summary>
		/// <param name="skill">计时结束后使用的技能</param>
		protected IEnumerator InvokeAttack(ActiveSkill skill){
            
			float timePassed = 0;

			float acutalAttackInterval = agent.attackInterval / armatureCom.animation.timeScale;

			while (timePassed < acutalAttackInterval) {

				timePassed += Time.deltaTime;

				yield return null;

			}

			UseSkill (skill);

		}
        
        /// <summary>
        /// 检查战斗是否结束
        /// </summary>
        /// <returns><c>true</c>, if fight end was checked, <c>false</c> otherwise.</returns>
		public abstract bool CheckFightEnd ();
        
        /// <summary>
        /// 播放角色被攻击时的轻微抖动动画
        /// </summary>
		public void PlayShakeAnim(){
			//如果角色已经死亡，则不执行抖动动画
			if (agent.health <= 0) {
				return;
			}
			IEnumerator playerShakeCoroutine = PlayAgentShake();
			StartCoroutine (playerShakeCoroutine);
		}

        /// <summary>
		/// 位置调整
        /// </summary>
        /// <param name="pos">Position.</param>
        /// <param name="callBack">Call back.</param>
		public void FixPosTo(Vector3 pos, CallBack callBack = null)
        {
			float distance = (transform.position - pos).magnitude;

			float duration = distance * 0.3f;

            transform.DOMove(pos, duration).OnComplete(() =>
            {
                if (callBack != null)
                {
                    callBack();
                }

            });

        }

		/// <summary>
		/// 角色被攻击时的抖动动画
		/// </summary>
		/// <returns>The agent shake.</returns>
		private IEnumerator PlayAgentShake(){

			float backwardTime = 0.05f;
			float forwardTime = 0.05f;

			float timer = 0f;

			float deltaX = 0.15f;

			float backwardSpeed = deltaX / backwardTime;
			float forwardSpeed = deltaX / forwardTime;

			Vector3 originPos = modelActive.transform.position;

			int directionSeed = armatureCom.armature.flipX ? -1 : 1;

			Vector3 targetPos = new Vector3 (modelActive.transform.position.x - deltaX * directionSeed, transform.position.y);

			while (timer < backwardTime) {

				modelActive.transform.position = Vector3.MoveTowards (modelActive.transform.position, targetPos, backwardSpeed * Time.deltaTime);

				timer += Time.deltaTime;

				yield return null;
			}

			timer = 0f;

			while (timer < forwardTime) {

				modelActive.transform.position = Vector3.MoveTowards (modelActive.transform.position, originPos, forwardSpeed * Time.deltaTime);

				timer += Time.deltaTime;

				yield return null;

			}

			if (this is BattlePlayerController) {
				modelActive.transform.localPosition = Vector3.zero;	
			} else {
				modelActive.transform.position = originPos;
			}
		}

		/// <summary>
		/// 播放角色动画方法.
		/// </summary>
		/// <param name="animName">播放的动画名称</param>
		/// <param name="playTimes">播放次数 [-1: 使用动画数据默认值, 0: 无限循环播放, [1~N]: 循环播放 N 次]</param>
		/// <param name="cb">动画完成回调.</param>
		public virtual void PlayRoleAnim (string animName, int playTimes, CallBack cb)
		{
         
			isIdle = animName == CommonData.roleIdleAnimName;

            // 死亡时只能播放死亡动画，其余动画请求全部直接返回
			if(isDead && animName != CommonData.roleDieAnimName){
				return;
			}

			// 如果还有等待上个角色动作结束的协程存在，则结束该协程
			if (waitRoleAnimEndCoroutine != null) {
				StopCoroutine (waitRoleAnimEndCoroutine);
			}
				

			// 播放新的角色动画
			armatureCom.animation.Play (animName,playTimes);
				
			this.animEndCallBack = cb;

			// 如果有角色动画结束后要执行的回调，则开启一个新的等待角色动画结束的协程，等待角色动画结束后执行回调
			if (cb != null) {
				waitRoleAnimEndCoroutine = ExcuteCallBackAtEndOfRoleAnim (cb);
				StartCoroutine(waitRoleAnimEndCoroutine);
			}
		}


		/// <summary>
		/// 当前动画结束之后播发角色等待动画（战斗结束时，等待攻击动作完成后再播放等待动画）
		/// </summary>
		public void ResetToWaitAfterCurrentRoleAnimEnd(){

			waitRoleAnimEndCoroutine = ExcuteCallBackAtEndOfRoleAnim (delegate {
                PlayRoleAnim(CommonData.roleIdleAnimName,0,null);  
				exploreManager.EnableExploreInteractivity();
			});

			if(waitRoleAnimEndCoroutine != null){
				StartCoroutine(waitRoleAnimEndCoroutine);
			}         
		}
			


		/// <summary>
		/// 设置角色特效动画，trigger 型触发器
		/// </summary>
		/// <param name="animName">触发器名称</param>
		public void SetEffectAnim(string effectName,CallBack cb = null,int playTimes = 1,float duration = 0,bool isProtectedBeforeEnd = false){

			if (effectName == string.Empty || exploreManager == null) {
				return;
			}
            
				
			EffectAnim skillEffect = exploreManager.newMapGenerator.GetEffectAnim (effectName,effectAnimContainer);

			skillEffect.isProtectedBeforeEnd = isProtectedBeforeEnd;

			if (skillEffect != null)
            {

    			skillEffect.gameObject.SetActive(true);

                // 所有的特效播放名称都是default
				skillEffect.PlayAnim ("default", cb, effectAnimYScaler, playTimes, duration);
			}

		}
			
        
		// 使用技能
		public abstract void UseSkill (ActiveSkill skill);

        // 角色死亡
		public abstract void AgentDie ();

        // 退出战斗
		public abstract void QuitFight ();

        // 战斗结束后停止所有协程
		protected virtual void StopCoroutinesWhenFightEnd (){         
			StopAllCoroutines ();         
		}

        /// <summary>
        /// 所有特效动画放入缓存池中
        /// </summary>
		public void AllEffectAnimsIntoPool(){

			for (int i = 0; i < effectAnimContainer.childCount; i++) {

				EffectAnim effectAnim = effectAnimContainer.GetChild (i).GetComponent<EffectAnim>();

				if(effectAnim.isProtectedBeforeEnd){
					continue;
				}

				exploreManager.newMapGenerator.AddEffectAnimToPool(effectAnim);

                i--;
			}

			effectAnimContainer.DetachChildren ();

		}

        // 更新角色状态面板
		public abstract void UpdateStatusPlane ();
        
        /// <summary>
        /// 执行战斗前被动技能回调
        /// </summary>
        /// <param name="enemy">Enemy.</param>
		public void ExcuteBeforeFightSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beforeFightTriggerExcutors.Count; i++) {
				SkillCallBack cb = beforeFightTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		/// <summary>
        /// 执行攻击时被动技能回调
        /// </summary>
        /// <param name="enemy">Enemy.</param>
		public void ExcuteAttackSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < attackTriggerExcutors.Count; i++) {
				SkillCallBack cb = attackTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		/// <summary>
        /// 执行击中时被动技能回调
        /// </summary>
        /// <param name="enemy">Enemy.</param>
		public void ExcuteHitSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beHitTriggerExcutors.Count; i++) {
				SkillCallBack cb = beHitTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		/// <summary>
        /// 执行被攻击时被动技能回调
        /// </summary>
        /// <param name="enemy">Enemy.</param>
		public void ExcuteBeAttackedSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beAttackedTriggerExcutors.Count; i++) {
				SkillCallBack cb = beAttackedTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		/// <summary>
        /// 执行被击中时被动技能回调
        /// </summary>
        /// <param name="enemy">Enemy.</param>
		public void ExcuteBeHitSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beHitTriggerExcutors.Count; i++) {
				SkillCallBack cb = beHitTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}
			

		/// <summary>
        /// 执行战斗结束时被动技能回调
        /// </summary>
        /// <param name="enemy">Enemy.</param>
		public void ExcuteFightEndCallBacks(BattleAgentController enemy){
			for (int i = 0; i < fightEndTriggerExcutors.Count; i++) {
				SkillCallBack cb = fightEndTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}
      
        /// <summary>
        /// 清除所有被动技能的执行器
        /// </summary>
		public void ClearAllSkillCallBacks(){

			beforeFightTriggerExcutors.Clear ();
			attackTriggerExcutors.Clear ();
			hitTriggerExcutors.Clear ();
			beAttackedTriggerExcutors.Clear ();
			beHitTriggerExcutors.Clear ();
			fightEndTriggerExcutors.Clear ();

		}


		/// <summary>
		/// 清除角色身上所有的战斗回调和触发型技能效果
		/// </summary>
		public void ClearAllEffectStatesAndSkillCallBacks(){

			ClearAllSkillCallBacks ();
		}

		/// <summary>
		/// 等待角色动画完成后执行回调
		/// </summary>
		/// <returns>The call back at end of animation.</returns>
		/// <param name="cb">Cb.</param>
		protected IEnumerator ExcuteCallBackAtEndOfRoleAnim(CallBack cb){
			yield return new WaitUntil (() => armatureCom.animation.isCompleted);
			cb ();
		}
			

	}



}