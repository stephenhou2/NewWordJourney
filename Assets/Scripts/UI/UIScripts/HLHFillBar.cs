﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class HLHFillBar : MonoBehaviour {

		public Image fillImage;

		public Text fillText;

		public float changeDuration = 0.2f;

		public bool updateWithAnim;

		private Tweener fillTweener;

		public void InitHLHFillBar(int maxValue,int value){
			mValue = value;
			mMaxValue = maxValue;
			if (fillTweener != null)
            {
                fillTweener.Kill();
            }         
			UpdateBarWithoutAnim ();
		}

		public int mMaxValue;
		public int maxValue{
			get{ return mMaxValue; }
			set{ 
				if (value < 1) {
					value = 1;
				}
				mMaxValue = value;
			}
		}

		public int mValue;
		public int value{
			get{ return mValue; }
			set{
				if (value < 0) {
					value = 0;
				}
				if (value > maxValue) {
					value = maxValue;
				}
				mValue = value;

				if (updateWithAnim) {
					UpdateBarWithAnim ();
				} else {
					UpdateBarWithoutAnim ();
				}
			}
		}

		private void UpdateBarWithoutAnim(){
			if(fillText != null){
				fillText.text = string.Format("{0} / {1}", value, maxValue);
			}     
			fillImage.fillAmount = (float)value / maxValue;
		}


		private void UpdateBarWithAnim(){
			if (fillText != null)
			{
				fillText.text = string.Format("{0} / {1}", mValue, mMaxValue);
                
			}
			if(fillTweener != null){
				fillTweener.Kill();
			}

			fillTweener = fillImage.DOFillAmount ((float)mValue / mMaxValue, changeDuration);
			fillTweener.SetUpdate(true);         
		}


	}
}
