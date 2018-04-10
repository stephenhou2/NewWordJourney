using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class CraftingRecipesHUD : MonoBehaviour {

		public Transform craftingItemAndRecipesContainer;
		public Image craftingItemIcon;
		public Text craftingItemName;
		public Text craftingItemDescription;

		public Image centralLine;

		public Transform recipesItemModel;
		public InstancePool recipesItemPool;
		public Transform recipesItemsContainer;

		private bool quitWhenClickBackground = true;
		private CallBack quitCallBack;
		private CallBack craftCallBack;

		public Button craftButton;

		[HideInInspector]public CraftingRecipe craftingRecipe;

		private float zoomInDuration = 0.2f;
		private IEnumerator zoomInCoroutine;

		private int widthX = 520;


		public void InitCraftingRecipesHUD(bool quitWhenClickBackground,CallBack quitCallBack,CallBack craftCallBack){

//			if (recipesItemPool == null) {
//				recipesItemPool = InstancePool.GetOrCreateInstancePool ("RecipesItemPool", CommonData.bagCanvasPoolContainerName);
//			}
			this.quitWhenClickBackground = quitWhenClickBackground;
			this.quitCallBack = quitCallBack;
			this.craftCallBack = craftCallBack;

		}

//		public void SetUpCraftingRecipesHUD(Item item){
//
//			gameObject.SetActive (true);
//
//			e.PlayAudioClip ("UI/sfx_UI_Paper");
//
//			Time.timeScale = 0;
//			
//			craftingRecipe = item as CraftingRecipe;
//
//			EquipmentModel craftingItem = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(EquipmentModel obj) {
//				return obj.itemId == craftingRecipe.craftItemId;
//			});
//
//			Sprite craftingItemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
//				return obj.name == craftingItem.spriteName;
//			});
//
//			craftingItemIcon.sprite = craftingItemSprite;
//
//			craftingItemIcon.enabled = craftingItemSprite != null;
//
//			craftingItemName.text = craftingItem.itemName;
//
//			craftingItemDescription.text = craftingItem.itemDescription;
//
//			EquipmentModel.ItemInfoForProduce[] itemInfosForProduce = craftingItem.itemInfosForProduce;
//
//			if (itemInfosForProduce.Length == 2) {
//				centralLine.gameObject.SetActive (false);
//			} else if (itemInfosForProduce.Length == 3) {
//				centralLine.gameObject.SetActive (true);
//			}
//
//			SetUpRecipesItems (itemInfosForProduce);
//
//			craftingItemAndRecipesContainer.localScale = new Vector3 (0.1f, 0.1f, 1);
//
//
//
//			zoomInCoroutine = CraftingRecipesHUDZoomIn ();
//
//			StartCoroutine (zoomInCoroutine);
//		}

//		private float GetMiddleLineWidth(int itemCount){
//
//			HorizontalLayoutGroup layout = recipesItemsContainer.GetComponent<HorizontalLayoutGroup> ();
//
//			float recipesItemWidth = recipesItemModel.GetComponent<RectTransform> ().rect.width;
//
//			return (itemCount - 1) * (layout.spacing + recipesItemWidth);
//
//		}

//		private void SetUpRecipesItems(EquipmentModel.ItemInfoForProduce[] itemInfosForProduce){
//
////			int horizontalFix = itemInfosForProduce.Length % 2 == 0 ? spacingX : 0;
//
//			int spaceX = widthX / (itemInfosForProduce.Length > 1 ? itemInfosForProduce.Length - 1 : 1);
//
//			recipesItemPool.AddChildInstancesToPool(recipesItemsContainer);
//
//			bool canCraft = true;
//
//			for (int i = 0; i < itemInfosForProduce.Length; i++) {
//
//				EquipmentModel item = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(EquipmentModel obj) {
//					return obj.itemId == itemInfosForProduce [i].itemId;
//				});
//
//				int itemCountForProduce = itemInfosForProduce [i].itemCount;
//
//				Transform recipesItem = recipesItemPool.GetInstance<Transform> (recipesItemModel.gameObject, recipesItemsContainer);
//
//				int localPosX = i * spaceX;
//
//				recipesItem.localPosition = new Vector3 (localPosX, 0, 0);
//
////				Debug.Log (recipesItem.localPosition);
//
//				Image recipesItemIcon = recipesItem.Find ("ItemIcon").GetComponent<Image> ();
//				Text recipesItemName = recipesItem.Find ("ItemName").GetComponent<Text> ();
//				Text recipesItemEnoughText = recipesItem.Find ("ItemEnoughText").GetComponent<Text> ();
//
//				Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
//					return obj.name == item.spriteName;
//				});
//
//				recipesItemIcon.sprite = itemSprite;
//
//				recipesItemIcon.enabled = itemSprite != null;
//
//				recipesItemName.text = item.itemName;
//
//				int itemInBagCount = 0;
//
//				List<Item> sameItemsInBag = Player.mainPlayer.allItemsInBag.FindAll (delegate(Item obj) {
//					return obj.itemId == item.itemId;
//				});
//
//				for(int j = 0; j < Player.mainPlayer.allEquipedEquipments.Length; j++){
//
//					Equipment equipment = Player.mainPlayer.allEquipedEquipments [j];
//
//					if (equipment.itemId < 0) {
//						continue;
//					}
//
//					if (equipment.itemId == item.itemId) {
//						itemInBagCount++;
//					}
//
//				}
//					
//				for (int j = 0; j < sameItemsInBag.Count; j++) {
//
//					itemInBagCount += sameItemsInBag [j].itemCount;
//
//				}
//
//
//				bool recipeItemEnough = itemInBagCount >= itemCountForProduce;
//
//				canCraft = canCraft && recipeItemEnough;
//
//				string color = recipeItemEnough ? "green" : "red";
//
//				recipesItemEnoughText.text = string.Format ("<color={0}>{1}/{2}</color>", color, itemInBagCount, itemCountForProduce);
//
//
//			}
//
//			craftButton.interactable = canCraft;
//
//
//		}

		private IEnumerator CraftingRecipesHUDZoomIn(){

			float scale = craftingItemAndRecipesContainer.transform.localScale.x;

			float zoomInSpeed = (1 - scale) / zoomInDuration;

			float lastFrameRealTime = Time.realtimeSinceStartup;

			while (scale < 1) {

				yield return null;

				scale += zoomInSpeed * (Time.realtimeSinceStartup - lastFrameRealTime);

				lastFrameRealTime = Time.realtimeSinceStartup;

				craftingItemAndRecipesContainer.transform.localScale = new Vector3 (scale, scale, 1);

			}

			craftingItemAndRecipesContainer.transform.localScale = Vector3.one;

		}




		public void OnBackgroundClick(){
			if (quitWhenClickBackground) {
				QuitCraftingRecipesHUD ();
			}
		}


		public void QuitCraftingRecipesHUD(){

			if (quitCallBack != null) {
				quitCallBack ();
			}

			if (zoomInCoroutine != null) {
				StopCoroutine (zoomInCoroutine);
			}

			gameObject.SetActive (false);

			if (TransformManager.FindTransform ("BagCanvas").GetComponent<Canvas> ().isActiveAndEnabled) {
				return;
			}

			Time.timeScale = 1f;

		}


		public void OnCraftButtonClick(){

			if (craftCallBack != null) {
				craftCallBack ();
			}

			QuitCraftingRecipesHUD ();

		}

	}
}
