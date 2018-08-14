using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.EventSystems;

	public delegate void CallBackWithFloat(float arg);
	
	public class HLHSlider : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
    {

		public int maxNum;

		public int mValue;
		public int value{
			get{
				return mValue;
			}
			set{
				float scaler = (float)value / maxNum;
				float sliderWidth = slider.rect.width;
				if(scaler < 0){
					scaler = 0;
				}else if(scaler > 1f){
					scaler = 1f;
				}
				handler.localPosition = new Vector3(sliderWidth * scaler, 0, 0);
				mValue = value;
				fillBar.value = value;

			}
		}

        // 滑块 【中心点必须设置在中心位置】[滑块的锚点设置在滑动条的中心点位置]
		public RectTransform handler;

        // 滑动条 【获取整个滑动区域信息】[slider的中心点设置必须在自身的左侧中点]
		public RectTransform slider;

        // 滑动区域内的填充
		public HLHFillBar fillBar;
      
        // 是否点击在滑块内部【初始点击位置不在滑块内部的话不能进行拖动操作】
		private bool clickInsideHandler;

		private CallBackWithFloat valueChangeCallBack;

		private float pixelScalerInWidth;

		private float handlerRectMaxX;
		private float handlerRectMinX;

		void Awake(){
			fillBar.maxValue = maxNum;
			pixelScalerInWidth = Camera.main.pixelWidth / 1080f;

		}

		public void InitHLHSlider(CallBackWithFloat valueChangeCallBack){
			this.valueChangeCallBack = valueChangeCallBack;

		}

		public void OnPointerDown(PointerEventData data){
   
			float handlerXMin = handler.position.x / pixelScalerInWidth - handler.rect.width / 2;
			float handlerXMax = handler.position.x / pixelScalerInWidth + handler.rect.width / 2;
            
			handlerRectMaxX = slider.position.x / pixelScalerInWidth + slider.rect.width;
			handlerRectMinX = slider.position.x / pixelScalerInWidth;
            
			clickInsideHandler = data.position.x / pixelScalerInWidth >= handlerXMin && data.position.x / pixelScalerInWidth <= handlerXMax;
            
			float scaler = (data.position.x - slider.position.x) / slider.rect.width / pixelScalerInWidth;

			value = (int)(scaler * maxNum);

		}

		public void OnPointerUp(PointerEventData data){         
			if(valueChangeCallBack != null){
				float scaler = (data.position.x - slider.position.x) / slider.rect.width / pixelScalerInWidth;
				valueChangeCallBack(scaler);
			}         
		}
        
		public void OnDrag(PointerEventData data){

			if(!clickInsideHandler){
				return;
			}

			if(data.position.x / pixelScalerInWidth >= handlerRectMaxX || data.position.x / pixelScalerInWidth <= handlerRectMinX){
				return;
			}

			float scaler = (data.position.x  - slider.position.x) / slider.rect.width / pixelScalerInWidth;

			value = (int)(scaler * maxNum);

			if(valueChangeCallBack != null){
                valueChangeCallBack(scaler);
            }         

		}
       

	}
}

