﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public enum MyTowards{
		Up,
		Down,
		Left,
		Right

	}

	public class ExploreText{
		public string text;
		public MyTowards towards;
		public Vector3 basePosition;
		public int indexInList;
		public ExploreText(string text,MyTowards towards,Vector3 basePosition){
			this.text = text;
			this.towards = towards;
			this.basePosition = basePosition;
		}
	}


	public class ExploreTextManager : MonoBehaviour {

		private InstancePool exploreTextPool;

		private Transform exploreTextModel;

		private Transform exploreTextContainer;

//		private Vector3 basePosition;

//		private MyTowards direction;

		private float exploreTextInterval = 0.1f;

//		private IEnumerator hurtTextCoroutine;
//
//		private IEnumerator tintTextCoroutine;

		private List<ExploreText> hurtTextList = new List<ExploreText> ();

		private List<ExploreText> tintTextList = new List<ExploreText> ();


		public void InitExploreTextManager(InstancePool exploreTextPool,Transform exploreTextModel,Transform exploreTextContainer){
			this.exploreTextPool = exploreTextPool;
			this.exploreTextModel = exploreTextModel;
			this.exploreTextContainer = exploreTextContainer;
		}

//		public void SetUpFightTextManager(Vector3 selfPos,MyTowards towards){
//
//			direction = towards;
//
//			basePosition = ToPointInCanvas (selfPos);
//
//		}

		/// <summary>
		/// 添加伤害文本到显示队列中
		/// </summary>
		/// <param name="exploreText">Explore text.</param>
		public void AddHurtText(ExploreText exploreText){
			exploreText.indexInList = hurtTextList.Count;
			hurtTextList.Add (exploreText);
			StartCoroutine ("ShowANewHurtText",exploreText);
		}

		/// <summary>
		/// 添加纵向屏幕文字到显示队列中
		/// </summary>
		/// <param name="exploreText">Explore text.</param>
		public void AddTintText(ExploreText exploreText){
			exploreText.indexInList = tintTextList.Count;
			tintTextList.Add (exploreText);
			StartCoroutine ("ShowANewTintText", exploreText);
		}

		private IEnumerator ShowANewHurtText(ExploreText exploreText){

			// 如果 exploreText 不在显示队列的队首，则一直等待
			while (exploreText.indexInList > 0) {
				yield return null;
			}

			// exploreText现在在显示队列的队首，则等待显示间隔事件后显示
			yield return new WaitForSeconds (exploreTextInterval);

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
			yield return new WaitForSeconds (exploreTextInterval);

			PlayTintTextAnim (exploreText);

			// 将exploreText从显示队列中移除
			tintTextList.RemoveAt (0);

			// 显示队列中的其他 exploreText 在队列中整体左移（由于exploreText的队列序号是在exploreText内部存储，所以需要手动移动整个队列中的exploreText）
			for (int i = 0; i < tintTextList.Count; i++) {
				tintTextList [i].indexInList--;
			}

		}
			
		/// <summary>
		/// 横向跳动文本动画（用于显示伤害文本动画）
		/// </summary>
		/// <param name="hurtString">Hurt string.</param>
		public void PlayHurtTextAnim(ExploreText et){

			// 从缓存池获取文本模型
			Text hurtText = exploreTextPool.GetInstance<Text> (exploreTextModel.gameObject, exploreTextContainer);

			Vector3 originHurtPos = Vector3.zero;
			Vector3 firstHurtPos = Vector3.zero;
			Vector3 secondHurtPos = Vector3.zero;
			Vector3 originTintPos = Vector3.zero;
			Vector3 finalTintPos = Vector3.zero;

			switch(et.towards){
			case MyTowards.Left:
				originHurtPos = et.basePosition + new Vector3 (-50f, 50f, 0);
				firstHurtPos = originHurtPos + new Vector3 (-Random.Range(80,100), Random.Range(0,10), 0);
				secondHurtPos = firstHurtPos + new Vector3 (-Random.Range(20,30), Random.Range(0,2), 0);
				originTintPos = originHurtPos + new Vector3 (-100f, 100f, 0);
				break;
			case MyTowards.Right:
				originHurtPos = et.basePosition + new Vector3 (50f, 50f, 0);
				firstHurtPos = originHurtPos + new Vector3 (Random.Range(80,100), Random.Range(0,10), 0);
				secondHurtPos = firstHurtPos + new Vector3 (Random.Range(20,30), Random.Range(0,2), 0);
				originTintPos = originHurtPos + new Vector3 (100f, 100f, 0);
				break;
			}

			hurtText.transform.localPosition = originHurtPos;

			hurtText.text = string.Format ("<b>{0}</b>", et.text);

			hurtText.gameObject.SetActive (true);

			float firstJumpPower = Random.Range (100f, 120f);

			// 伤害文本跳跃动画
			hurtText.transform.DOLocalJump (firstHurtPos, firstJumpPower, 1, 0.35f).OnComplete(()=>{

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
		/// 吸血文本动画
		/// </summary>
		/// <param name="gainStr">Gain string.</param>
		/// <param name="agentPos">Agent position.</param>
//		public void PlayGainTextAnim(ExploreText exploreText){
//
//			Vector3 pos = Vector3.zero;
//
//			switch (direction) {
//			case MyTowards.Left:
//				pos = basePosition + new Vector3 (-50f, 150f, 0);
//				break;
//			case MyTowards.Right:
//				pos = basePosition + new Vector3 (50f, 150f, 0);
//				break;
//			}
//
//			Text gainText = exploreTextPool.GetInstance<Text> (exploreTextModel.gameObject, exploreTextContainer);
//
//			gainText.transform.localPosition = pos;
//
//			gainText.text = exploreText.text;
//
//			gainText.gameObject.SetActive (true);
//
//			float endY = pos.y + 100f;
//
//			gainText.transform.DOLocalMoveY (endY, 1f).OnComplete(()=>{
//				gainText.text = "";
//				gainText.gameObject.SetActive(false);
//				exploreTextPool.AddInstanceToPool(gainText.gameObject);
//			});
//
//		}

		/// <summary>
		/// 纵向提示文本动画（用于显示血量提升，特殊攻击结果（暴击，闪避），其他屏幕提示）
		/// </summary>
		/// <param name="tintStr">Tint string.</param>
		/// <param name="originPos">Origin position.</param>
		private void PlayTintTextAnim(ExploreText et){

			Text tintText = exploreTextPool.GetInstance<Text> (exploreTextModel.gameObject, exploreTextContainer);

			tintText.transform.localPosition = et.basePosition + new Vector3 (0, 1, 0);

			tintText.text = et.text;

			tintText.gameObject.SetActive (true);

			tintText.transform.DOScale(new Vector3(1.2f,1.2f,1f),0.5f).OnComplete (() => {

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
