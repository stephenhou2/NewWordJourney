using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transform = UnityEngine.Transform;
using DragonBones;


namespace WordJourney
{

	public delegate void SkillCallBack(BattleAgentController selfBaCtr,BattleAgentController enemyBaCtr);

	public class TriggeredSkillExcutor{
		public TriggeredPassiveSkill triggeredSkill;
		public SkillCallBack triggeredCallback;

		public TriggeredSkillExcutor(TriggeredPassiveSkill ts,SkillCallBack callBack){
			this.triggeredSkill = ts;
			this.triggeredCallback = callBack;
		}

		public void OnClear(){
			triggeredSkill = null;
			triggeredCallback = null;
		}
	}

	[RequireComponent(typeof(NormalAttack))]
	public abstract class BattleAgentController : MonoBehaviour {

		// 控制的角色
		[HideInInspector]public Agent agent;

//		public AgentPropertyCalculator probpertyCalculator;

		protected ExploreUICotroller expUICtr;

		public BattleAgentController enemy;

		protected BattleAgentUIController agentUICtr;

		public bool isIdle;

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
		// 等待技能动画结束协程
		public List<IEnumerator> allSkillEffectReuseCoroutines = new List<IEnumerator>();

		// 角色的普通攻击技能
		public NormalAttack normalAttack;

		protected ActiveSkill currentUsingActiveSkill;

//		public List<ActiveSkill> activeSkills= new List<ActiveSkill>();

//		[HideInInspector]public bool isAttackActionFinish;

		// 移动速度
		public float moveDuration{ get{return 1 / (agent.moveSpeed * 0.025f + 2.5f); }}

		// 骨骼动画控制器
		protected UnityArmatureComponent armatureCom{
			get{
				return modelActive.GetComponent<UnityArmatureComponent> ();
			}
		}


//		protected ExploreManager mExploreManager;
//		protected ExploreManager exploreManager{
//			get{
//				if (mExploreManager == null) {
//					mExploreManager = ExploreManager.Instance.GetComponent<ExploreManager>();
//				}
//				return mExploreManager;
//			}
//		}

		protected ExploreManager exploreManager{get{ return ExploreManager.Instance; }}


		public List<string> currentTriggeredEffectAnim = new List<string>();

		public MyTowards towards;

		public Transform effectAnimContainer;


		protected virtual void Awake(){

			boxCollider = GetComponent <BoxCollider2D> ();

			ListenerDelegate<EventObject> keyFrameListener = KeyFrameMessage;

			if (this is BattleMonsterController ) {
				armatureCom.AddEventListener (DragonBones.EventObject.FRAME_EVENT, keyFrameListener);
			} else if (this is BattlePlayerController) {
				UnityArmatureComponent playerArmature = transform.Find ("PlayerSide").GetComponent<UnityArmatureComponent> ();
				playerArmature.AddEventListener(DragonBones.EventObject.FRAME_EVENT, keyFrameListener);
			}

			isIdle = true;


		}

		/// <summary>
		/// 初始化角色身上的触发型被动技能的回调
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="enemy">Enemy.</param>
		public void InitTriggeredPassiveSkillCallBacks(BattleAgentController self,BattleAgentController enemy){
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

			Vector3 basePosition = MyTool.ToPointInCanvas (transform.position);

//			MyTowards textAnimTowards = GetReversedTowards ();

			ExploreText ft = new ExploreText (text,towards,basePosition);

			agentUICtr.exploreTextManager.AddHurtText (ft);
		}


		/// <summary>
		/// 添加纵向文字动画到队列
		/// </summary>
		/// <param name="text">Text.</param>
		public void AddTintTextToQueue (string text)
		{
			Vector3 basePosition = MyTool.ToPointInCanvas (transform.position);

			ExploreText ft = new ExploreText (text,MyTowards.Left,basePosition);

			agentUICtr.exploreTextManager.AddTintText (ft);
		}


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

			string hurtString = string.Empty;
			if (agent.shenLuTuTengScaler == 0) {
				agent.health -= hurt;
			}else{

				if ((float)agent.mana / agent.maxMana > 0.3f) {

					int manaValid = agent.mana - (int)(agent.maxMana * 0.3f);

					if (manaValid >= hurt / 2) {
						agent.mana -= hurt / 2;
					} else {
						agent.mana = (int)(agent.maxMana * 0.3f);
						int healthChange = hurt - manaValid * 2;
						agent.health -= healthChange;
					}
				} else {
					agent.health -= hurt;
				}
			}

			switch (type) {
			case HurtType.Physical:
				hurtString = string.Format ("<color=red>{0}</color>", hurt);
				break;
			case HurtType.Magical:
				hurtString = string.Format ("<color=blue>{0}</color>", hurt);
				break;

			}

			AddHurtTextToQueue (hurtString,textTowards);

		}

		/// <summary>
		/// 回血并播放回血文字动画
		/// </summary>
		/// <param name="gain">Gain.</param>
		public void AddHealthGainAndShow(int gain){

			agent.health += gain;

			string hurtString = string.Format ("<color=green>{0}</color>", gain);

			AddTintTextToQueue (hurtString);

		}


		public virtual void SetSortingOrder(int order){
			armatureCom.sortingOrder = order;
		}

//		public abstract void InitFightTextDirectionTowards (BattleAgentController enemy);
//		public abstract void InitFightTextDirectionTowards (Vector3 position);


		public void SetRoleAnimTimeScale(float scale){

			armatureCom.animation.timeScale = scale;

		}
			

		protected void KeyFrameMessage<T>(string key,T eventObject){

			EventObject frameObject = eventObject as EventObject;

			if (frameObject.name == "hit") {
				AgentExcuteHitEffect ();
//				Debug.LogFormat ("hit---{0}", agent.name);
			} else {
				Debug.LogError ("事件帧消息名称必须是hit");
			}

		}

		protected abstract void AgentExcuteHitEffect ();


		public abstract void TowardsLeft(bool andWait = true);
		public abstract void TowardsRight(bool andWait = true);
		public abstract void TowardsUp (bool andWait = true);
		public abstract void TowardsDown (bool andWait = true);


		public void RemoveTriggeredSkillExcutor(TriggeredPassiveSkill skill){

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
				beforeFightTriggerExcutors.RemoveAt (index);
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
		/// 角色攻击间隔计时器
		/// </summary>
		/// <param name="skill">计时结束后使用的技能</param>
		protected IEnumerator InvokeAttack(ActiveSkill skill){

			float timePassed = 0;

			while (timePassed < agent.attackInterval) {

				timePassed += Time.deltaTime;

				yield return null;

			}

			UseSkill (skill);

		}

		public abstract bool CheckFightEnd ();

		public void PlayShakeAnim(){
			StartCoroutine ("PlayAgentShake");
		}

		/// <summary>
		/// 角色被攻击时的抖动动画
		/// </summary>
		/// <returns>The agent shake.</returns>
		private IEnumerator PlayAgentShake(){

			float backwardTime = 0.1f;
			float forwardTime = 0.1f;

			float timer = 0f;

			float deltaX = 0.2f;

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
			}
				
		}

		/// <summary>
		/// 播放角色动画方法.
		/// </summary>
		/// <param name="animName">播放的动画名称</param>
		/// <param name="playTimes">播放次数 [-1: 使用动画数据默认值, 0: 无限循环播放, [1~N]: 循环播放 N 次]</param>
		/// <param name="cb">动画完成回调.</param>
		public void PlayRoleAnim (string animName, int playTimes, CallBack cb)
		{


			isIdle = animName == "wait";

			// 如果还有等待上个角色动作结束的协程存在，则结束该协程
			if (waitRoleAnimEndCoroutine != null) {
				StopCoroutine (waitRoleAnimEndCoroutine);
			}
				

			// 播放新的角色动画
			armatureCom.animation.Play (animName,playTimes);
				
			// 如果有角色动画结束后要执行的回调，则开启一个新的等待角色动画结束的协程，等待角色动画结束后执行回调
			if (cb != null) {
				waitRoleAnimEndCoroutine = ExcuteCallBackAtEndOfRoleAnim (cb);
				StartCoroutine(waitRoleAnimEndCoroutine);
			}
		}


		/// <summary>
		/// 设置角色特效动画，trigger 型触发器
		/// </summary>
		/// <param name="animName">触发器名称</param>
		public void SetEffectAnim(string triggerName,CallBack cb = null){

			if(triggerName != string.Empty && exploreManager != null){

				IEnumerator playEffectAnimCoroutine = LatelyPlayEffectAnim (triggerName, cb);

				StartCoroutine (playEffectAnimCoroutine);

			}
		}

		private IEnumerator LatelyPlayEffectAnim(string triggerName,CallBack cb){

			yield return new WaitUntil (() => Time.timeScale == 1);

			yield return null;

			if (agent.health > 0) {
				
				Transform skillEffect = null;
				Animator skillEffectAnim = null;

				skillEffect = exploreManager.GetComponent<MapGenerator> ().GetEffectAnim (transform);

				skillEffectAnim = skillEffect.GetComponent<Animator> ();

				skillEffectAnim.transform.SetParent (effectAnimContainer);

				skillEffectAnim.SetTrigger (triggerName);

				IEnumerator skillEffectReuseCoroutine = AddSkillEffectToPoolAfterAnimEnd (skillEffect.transform, cb);

				allSkillEffectReuseCoroutines.Add (skillEffectReuseCoroutine);

				StartCoroutine (skillEffectReuseCoroutine);
			}
		}




		/// <summary>
		/// 技能特效动画结束后将特效显示器重置后（带SkillEffectAnimtor的游戏体）加入缓存池
		/// </summary>
		/// <returns>The skill effect to pool after animation end.</returns>
		/// <param name="effectInfo">Effect info.</param>
		protected IEnumerator AddSkillEffectToPoolAfterAnimEnd(Transform skillEffectTrans,CallBack cb){

			yield return null;

			Animator animator = skillEffectTrans.GetComponent<Animator> ();

			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (0);

//			Debug.Log (stateInfo.IsName ("Empty") + "----------------------");

			while (stateInfo.normalizedTime < 1) {
//				Debug.Log (stateInfo.normalizedTime);
				yield return null;
				stateInfo = animator.GetCurrentAnimatorStateInfo (0);
			}
//			yield return new WaitForSeconds (stateInfo.length);

			animator.SetTrigger ("Empty");

			yield return null;

			exploreManager.GetComponent<MapGenerator> ().AddEffectAnimToPool (animator.transform);

			if (cb != null) {
				cb ();
			}

//			skillEffectDic.Remove (effectInfo.triggerName);

//			animator.ResetTrigger (triggerName);

//			Debug.LogFormat ("{0}回收技能特效", agent.agentName);

		}
			


		/// <summary>
		/// 使用技能
		/// </summary>
		/// <param name="skill">Skill.</param>
		public abstract void UseSkill (ActiveSkill skill);

		/// <summary>
		/// 角色默认的战斗方法
		/// </summary>
		public abstract void Fight ();

		public void ResetAgent(){
			StopCoroutinesWhenFightEnd ();
			this.armatureCom.animation.Stop ();
			CancelInvoke ();
		}

		public abstract void AgentDie ();

		public abstract void QuitFight ();

		protected virtual void StopCoroutinesWhenFightEnd (){
			
//			if (attackCoroutine != null) {
//				StopCoroutine (attackCoroutine);
//			}
//
//			if (waitRoleAnimEndCoroutine != null) {
//				StopCoroutine (waitRoleAnimEndCoroutine);
//			}
//
//			for (int i = 0; i < allSkillEffectReuseCoroutines.Count; i++) {
//				IEnumerator skillEffectReuseCoroutine = allSkillEffectReuseCoroutines [i];
//				if (skillEffectReuseCoroutine != null) {
//					StopCoroutine (skillEffectReuseCoroutine);
//				}
//
//			}

			StopAllCoroutines ();

			CancelInvoke ();

			AllEffectAnimsInfoPool ();

			allSkillEffectReuseCoroutines.Clear ();

//			StopCoroutine ("PlayAgentShake");

//			modelActive.transform.localPosition = Vector3.zero;
		}

		private void AllEffectAnimsInfoPool(){

			for (int i = 0; i < effectAnimContainer.childCount; i++) {

				Transform effectAnim = effectAnimContainer.GetChild (i);

				exploreManager.GetComponent<MapGenerator> ().AddEffectAnimToPool (effectAnim);

				i--;

			}

			effectAnimContainer.DetachChildren ();

		}

		public abstract void UpdateStatusPlane ();

		public void ExcuteBeforeFightSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beforeFightTriggerExcutors.Count; i++) {
				SkillCallBack cb = beforeFightTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		public void ExcuteAttackSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < attackTriggerExcutors.Count; i++) {
				SkillCallBack cb = attackTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		public void ExcuteHitSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beHitTriggerExcutors.Count; i++) {
				SkillCallBack cb = beHitTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		public void ExcuteBeAttackedSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beAttackedTriggerExcutors.Count; i++) {
				SkillCallBack cb = beAttackedTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

		public void ExcuteBeHitSkillCallBacks(BattleAgentController enemy){
			for (int i = 0; i < beHitTriggerExcutors.Count; i++) {
				SkillCallBack cb = beHitTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}
			

		public void ExcuteFightEndCallBacks(BattleAgentController enemy){
			for (int i = 0; i < fightEndTriggerExcutors.Count; i++) {
				SkillCallBack cb = fightEndTriggerExcutors [i].triggeredCallback;
				cb (this, enemy);
			}
		}

//		public void RemoveTriggeredSkillEffect(){
//			agent.AddPropertyChangeFromOther (
//				-propertyCalculator.maxHealthChangeFromTriggeredSkill,
//				-propertyCalculator.hitChangeFromTriggeredSkill,
//				-propertyCalculator.attackChangeFromTriggeredSkill,
//				-propertyCalculator.attackSpeedChangeFromTriggeredSkill,
//				-propertyCalculator.manaChangeFromTriggeredSkill,
//				-propertyCalculator.armorChangeFromTriggeredSkill,
//				-propertyCalculator.magicResistChangeFromTriggeredSkill,
//				-propertyCalculator.dodgeChangeFromTriggeredSkill,
//				-propertyCalculator.critChangeFromTriggeredSkill,
//				-propertyCalculator.maxHealthChangeScalerFromTriggeredSkill,
//				-propertyCalculator.hitChangeScalerFromTriggeredSkill,
//				-propertyCalculator.attackChangeScalerFromTriggeredSkill,
//				-propertyCalculator.attackSpeedChangeScalerFromTriggeredSkill,
//				-propertyCalculator.manaChangeScalerFromTriggeredSkill,
//				-propertyCalculator.armorChangeScalerFromTriggeredSkill,
//				-propertyCalculator.magicResistChangeScalerFromTriggeredSkill,
//				-propertyCalculator.dodgeChangeScalerFromTriggeredSkill,
//				-propertyCalculator.critChangeScalerFromTriggeredSkill,
//				-propertyCalculator.physicalHurtScalerChangeFromTriggeredSkill,
//				-propertyCalculator.magicalHurtScalerChangeFromTriggeredSkill,
//				-propertyCalculator.critChangeScalerFromTriggeredSkill);
//
//		}

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

//			propertyCalculator.ClearSkillsOfType<TriggeredPassiveSkill> ();
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