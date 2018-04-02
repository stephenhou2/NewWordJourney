using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class UnlockScrollDetailHUD : MonoBehaviour {

		public Transform unlockScrollContainer;
		public Image unlockedItemIcon;
		public Text unlockedItemName;
		public Text statusText;
		public Text unlockedItemDescription;

		private bool quitWhenClickBackground = true;
		private CallBack quitCallBack;
		private CallBack unlockCallBack;
		private CallBack resolveCallBack;

		public Button unlockButton;
		public Button resolveButton;

//		[HideInInspector]public UnlockScroll unlockScroll;

		private float zoomInDuration = 0.2f;
		private IEnumerator zoomInCoroutine;

		/// <summary>
		/// quitWhenClickBackground 表示点击背景空白处是否可以退出物品详细页
		/// quitCallBack回调是在关闭物品详细页的逻辑中执行，所以回调中 不要 再次 关闭物品详细页
		/// </summary>
		public void InitUnlockScrollDetailHUD(bool quitWhenClickBackground,CallBack quitCallBack,CallBack unlockCallBack,CallBack resolveCallBack){
			this.quitWhenClickBackground = quitWhenClickBackground;
			this.quitCallBack = quitCallBack;
			this.unlockCallBack = unlockCallBack;
			this.resolveCallBack = resolveCallBack;
		}

//		public void SetUpUnlockScrollDetailHUD(Item item){
//
//			gameObject.SetActive (true);
//
//			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Paper");
//
//			Time.timeScale = 0;
//
//			UnlockScroll unlockScrollInBag = item as UnlockScroll;
//
//			this.unlockScroll = unlockScrollInBag;
//
//			EquipmentModel itemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(EquipmentModel obj) {
//				return obj.itemId == unlockScrollInBag.unlockedItemId;
//			});
//
//			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
//				return obj.name == itemModel.spriteName;
//			});
//
//			unlockedItemIcon.sprite = itemSprite;
//
//			unlockedItemIcon.enabled = itemSprite != null;
//
//			unlockedItemName.text = itemModel.itemName;
//
//			bool hasScrollUnlocked = false;
//
//			List<UnlockScroll> sameUnlockScrollsInBag = Player.mainPlayer.allUnlockScrollsInBag.FindAll (delegate(UnlockScroll obj) {
//				return obj.itemId == unlockScroll.itemId;
//			});
//
//			for (int i = 0; i < sameUnlockScrollsInBag.Count; i++) {
//				if (sameUnlockScrollsInBag [i].unlocked) {
//					hasScrollUnlocked = true;
//					break;
//				}
//			}
//				
//			statusText.text = hasScrollUnlocked ? "<color=green>已解锁</color>" : "<color=red>未解锁</color>";
//
//			unlockedItemDescription.text = itemModel.itemDescription;
//
//			unlockButton.gameObject.SetActive (!hasScrollUnlocked);
//			resolveButton.gameObject.SetActive (hasScrollUnlocked);
//
//			unlockScrollContainer.localScale = new Vector3 (0.1f, 0.1f, 1);
//
//
//
//			zoomInCoroutine = UnlockScrollHUDZoomIn ();
//
//			StartCoroutine (zoomInCoroutine);
//
//		}

//		private IEnumerator UnlockScrollHUDZoomIn(){
//
//			float scale = unlockScrollContainer.transform.localScale.x;
//
//			float zoomInSpeed = (1 - scale) / zoomInDuration;
//
//			float lastFrameRealTime = Time.realtimeSinceStartup;
//
//			while (scale < 1) {
//
//				yield return null;
//
//				scale += zoomInSpeed * (Time.realtimeSinceStartup - lastFrameRealTime);
//
//				lastFrameRealTime = Time.realtimeSinceStartup;
//
//				unlockScrollContainer.transform.localScale = new Vector3 (scale, scale, 1);
//
//			}
//
//			unlockScrollContainer.transform.localScale = Vector3.one;
//
//		}

		public void OnBackgroundClicked(){
			if (quitWhenClickBackground) {
				QuitUnlockScrollDetailHUD ();
			}
		}

		public void OnUnlockButtonClick(){

			if (unlockCallBack != null) {
				unlockCallBack ();
			}

			QuitUnlockScrollDetailHUD ();
		}

		public void OnResolveButtonClick(){

			if (resolveCallBack != null) {
				resolveCallBack ();
			}

			QuitUnlockScrollDetailHUD ();
		}
			

		public void QuitUnlockScrollDetailHUD(){

			if (quitCallBack != null) {
				quitCallBack ();
			}

			unlockButton.gameObject.SetActive (false);
			resolveButton.gameObject.SetActive (false);

			if (zoomInCoroutine != null) {
				StopCoroutine (zoomInCoroutine);
			}

			gameObject.SetActive (false);

			if (TransformManager.FindTransform ("BagCanvas").GetComponent<Canvas> ().isActiveAndEnabled) {
				return;
			}

			Time.timeScale = 1f;

		}


	}
}
