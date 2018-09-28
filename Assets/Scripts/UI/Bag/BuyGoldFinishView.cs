using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	using UnityEngine.UI;

	public class BuyGoldFinishView : ZoomHUD
    {

		public Image goldItemIcon;
		public Text buyResultHint;

		public Sprite goldIcon_500;
		public Sprite goldIcon_1600;
		public Sprite goldIcon_3500;
		public Sprite goldIcon_5000;


		public void SetUpBuyGoldFinishView(string productID){

			Sprite buyItemSprite = null;

			string buyResult = string.Empty;

			int goldBuy = 0;
			if(productID == PurchaseManager.gold_100_id){
				buyItemSprite = goldIcon_500;
                goldBuy = 100;
				buyResult = string.Format("获得奖励<color=orange>{0}</color>金币", goldBuy);
			}else if(productID == PurchaseManager.gold_500_id){
				buyItemSprite = goldIcon_500;
				goldBuy = 500;
				buyResult = string.Format("成功购买<color=orange>{0}</color>金币", goldBuy);
			}else if(productID == PurchaseManager.gold_1600_id){
				buyItemSprite = goldIcon_1600;
				goldBuy = 1600;
				buyResult = string.Format("成功购买<color=orange>{0}</color>金币", goldBuy);
			}else if(productID == PurchaseManager.gold_3500_id){
				buyItemSprite = goldIcon_3500;
				goldBuy = 3500;
				buyResult = string.Format("成功购买<color=orange>{0}</color>金币", goldBuy);
			}else if(productID == PurchaseManager.gold_5000_id){
				buyItemSprite = goldIcon_5000;
				goldBuy = 5000;
				buyResult = string.Format("成功购买<color=orange>{0}</color>金币", goldBuy);
            }
                     
			ExploreManager.Instance.expUICtr.UpdatePlayerGold();
                     
			goldItemIcon.sprite = buyItemSprite;
			buyResultHint.text = buyResult;

			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);

		}
      
		public void QuitBuyGoldFinishView(){

			if (inZoomingOut)
            {
                return;
            }

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut();

			StartCoroutine(zoomCoroutine);

		}

    }

}
