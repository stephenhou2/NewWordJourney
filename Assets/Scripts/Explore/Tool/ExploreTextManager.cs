using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;
    
    /// <summary>
    /// 探索中文字模型【主要用于属性变化提示数字，文字】
    /// </summary>
	public class ExploreText{
		// 文本
		public string text;
        // 朝向
		public MyTowards towards;
        // 文字动画运动的原始位置
		public Vector3 basePosition;
        // 在待显示文字提示的等待列表中的序号
		public int indexInList;

        // 构造函数
		public ExploreText(string text,MyTowards towards,Vector3 basePosition){
			this.text = text;
			this.towards = towards;
			this.basePosition = basePosition;
		}
	}

    /// <summary>
	/// 探索中文字控制器
    /// </summary>
	public class ExploreTextManager : MonoBehaviour {

        // 缓存池
		private InstancePool exploreTextPool;
        // 模型
		private Transform exploreTextModel;
        // 容器
		private Transform exploreTextContainer;
        
        // 伤害文字动画间隔
		private float hurtTextInterval = 0.1f;
        // 提示文字动画间隔
		private float tintTextInterval = 0.3f;


        // 等待显示的伤害文本列表
		private List<ExploreText> hurtTextList = new List<ExploreText> ();
        // 等待显示的提示文本列表
		private List<ExploreText> hintTextList = new List<ExploreText> ();

        
        /// <summary>
        /// 初始化文本动画控制器
        /// </summary>
        /// <param name="exploreTextPool">Explore text pool.</param>
        /// <param name="exploreTextModel">Explore text model.</param>
        /// <param name="exploreTextContainer">Explore text container.</param>
		public void InitExploreTextManager(InstancePool exploreTextPool,Transform exploreTextModel,Transform exploreTextContainer){
			this.exploreTextPool = exploreTextPool;
			this.exploreTextModel = exploreTextModel;
			this.exploreTextContainer = exploreTextContainer;
		}


		/// <summary>
		/// 添加伤害文本到显示队列中
		/// </summary>
		/// <param name="exploreText">Explore text.</param>
		public void AddHurtText(ExploreText exploreText){
			exploreText.indexInList = hurtTextList.Count;
			hurtTextList.Add (exploreText);
			IEnumerator showNewHurtTextCoroutine = ShowANewHurtText(exploreText);
			StartCoroutine (showNewHurtTextCoroutine);
		}

		/// <summary>
		/// 添加纵向屏幕文字到显示队列中
		/// </summary>
		/// <param name="exploreText">Explore text.</param>
		public void AddHintText(ExploreText exploreText){
			exploreText.indexInList = hintTextList.Count;
			hintTextList.Add (exploreText);
			IEnumerator showNewHintTextCoroutine = ShowANewTintText(exploreText);
			StartCoroutine (showNewHintTextCoroutine);
		}

		private IEnumerator ShowANewHurtText(ExploreText exploreText){

			// 如果 exploreText 不在显示队列的队首，则一直等待
			while (exploreText.indexInList > 0) {
				yield return null;
			}

			// exploreText现在在显示队列的队首，则等待显示间隔事件后显示
			yield return new WaitForSeconds (hurtTextInterval);

			PlayHurtTextAnim (exploreText);

			// 将exploreText从显示队列中移除
			hurtTextList.RemoveAt (0);

			// 显示队列中的其他 exploreText 在队列中整体左移（由于exploreText的队列序号是在exploreText内部存储，所以需要手动移动整个队列中的exploreText）
			for (int i = 0; i < hurtTextList.Count; i++) {
				hurtTextList [i].indexInList--;
			}

		}

		private IEnumerator ShowANewTintText(ExploreText exploreText){

			// 如果 exploreText 不在显示队列的队首，则一直等待
			while (exploreText.indexInList > 0) {
				yield return null;
			}

			// exploreText现在在显示队列的队首，则等待显示间隔事件后显示
			yield return new WaitForSeconds (tintTextInterval);

			PlayTintTextAnim (exploreText);

			// 将exploreText从显示队列中移除
			hintTextList.RemoveAt (0);

			// 显示队列中的其他 exploreText 在队列中整体左移（由于exploreText的队列序号是在exploreText内部存储，所以需要手动移动整个队列中的exploreText）
			for (int i = 0; i < hintTextList.Count; i++) {
				hintTextList [i].indexInList--;
			}

		}
			
		/// <summary>
		/// 横向跳动文本动画（用于显示伤害文本动画）
		/// </summary>
		/// <param name="hurtString">Hurt string.</param>
		public void PlayHurtTextAnim(ExploreText et){

			// 从缓存池获取文本模型
			Text hurtText = exploreTextPool.GetInstance<Text> (exploreTextModel.gameObject, exploreTextContainer);

            // 伤害文本的原始位置
			Vector3 originHurtPos = Vector3.zero;
            // 伤害文本的第一次跳动终点
			Vector3 firstHurtPos = Vector3.zero;
            // 伤害文本的第二次跳动终点
			Vector3 secondHurtPos = Vector3.zero;
            // 提示文本的原始位置【伤害可能附带暴击，闪避等提示文字】
			Vector3 originTintPos = Vector3.zero;
			// 提示文本的终点位置【伤害可能附带暴击，闪避等提示文字】
			Vector3 finalTintPos = Vector3.zero;


            // 下面设置文本的动画路径
			switch(et.towards){
			case MyTowards.Left:
			case MyTowards.Up:
				originHurtPos = et.basePosition + new Vector3 (-50f, 50f, 0);
				firstHurtPos = originHurtPos + new Vector3 (-Random.Range(80,100), Random.Range(0,10), 0);
				secondHurtPos = firstHurtPos + new Vector3 (-Random.Range(20,30), Random.Range(0,2), 0);
				originTintPos = originHurtPos + new Vector3 (-100f, 100f, 0);
				break;
			case MyTowards.Right:
			case MyTowards.Down:
					originHurtPos = et.basePosition + new Vector3 (50f, 50f, 0);
				firstHurtPos = originHurtPos + new Vector3 (Random.Range(80,100), Random.Range(0,10), 0);
				secondHurtPos = firstHurtPos + new Vector3 (Random.Range(20,30), Random.Range(0,2), 0);
				originTintPos = originHurtPos + new Vector3 (100f, 100f, 0);
				break;
			}

			hurtText.transform.localPosition = originHurtPos;
         
			hurtText.text = et.text;

			hurtText.GetComponentInChildren<Image>().enabled = false;

			hurtText.gameObject.SetActive (true);

			float firstJumpPower = Random.Range (100f, 120f);

			// 伤害文本跳跃动画
			hurtText.transform.DOLocalJump (firstHurtPos, firstJumpPower, 1, 0.4f).OnComplete(()=>{

				float secondJumpPower = Random.Range(20f,30f);

				// 伤害文本二次跳跃
				hurtText.transform.DOLocalJump (secondHurtPos, secondJumpPower, 1, 0.15f).OnComplete(()=>{
					hurtText.text = "";
					hurtText.gameObject.SetActive(false);
					exploreTextPool.AddInstanceToPool(hurtText.gameObject);
				});

			});

		}


		/// <summary>
		/// 纵向提示文本动画（用于显示血量提升，特殊攻击结果（暴击，闪避），其他屏幕提示）
		/// </summary>
		/// <param name="tintStr">Tint string.</param>
		/// <param name="originPos">Origin position.</param>
		private void PlayTintTextAnim(ExploreText et){

			Text tintText = exploreTextPool.GetInstance<Text> (exploreTextModel.gameObject, exploreTextContainer);

			tintText.transform.localPosition = et.basePosition + new Vector3 (0, 260, 0);

			tintText.text = string.Format("<size=35>{0}</size>",et.text);

			tintText.gameObject.SetActive(true);
         
            // 提示文本做一次缩放
			tintText.transform.DOScale(new Vector3(1.2f,1.2f,1f),0.3f).OnComplete (() => {

				tintText.text = "";

				tintText.transform.localScale = Vector3.one;

				tintText.gameObject.SetActive(false);

				exploreTextPool.AddInstanceToPool(tintText.gameObject);

			});
		}

		/// <summary>
		/// 所有的屏幕探索文字全部清除并添加到缓存池中
		/// </summary>
		public void AllExploreTextClearAndIntoPool(){

			for (int i = 0; i < exploreTextContainer.childCount; i++) {
				Text exploreText = exploreTextContainer.GetChild (i).GetComponent<Text> ();
				exploreText.text = "";
				exploreTextPool.AddInstanceToPool (exploreText.gameObject);
			}

		}

	}
		

}
