using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class SimpleItemDetail : MonoBehaviour {

		public Text itemName;
		public Text itemDescription;
		public Text attachedDescription;
		public Image itemIcon;

		public float flyDuration;
		public float stayDuration;
		public Vector3 itemDetailMoveStart;
		public int itemDetailMoveEndX;

		private IEnumerator simpleItemDetailCoroutine;

		public void SetupSimpleItemDetail(Item item){
			ClearItemDetail ();
			itemName.text = item.itemName;
			itemDescription.text = item.itemDescription;
			if (item.itemType == ItemType.Equipment) {
				Equipment eqp = item as Equipment;
				attachedDescription.text = eqp.attachedPropertyDescription;
			}
			Sprite s = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);
			itemIcon.sprite = s;
			itemIcon.enabled = s != null;

			if (simpleItemDetailCoroutine != null) {
				StopCoroutine (simpleItemDetailCoroutine);
			}

			simpleItemDetailCoroutine = ShowSimpleItemDetail ();

			StartCoroutine (simpleItemDetailCoroutine);


		}

		private IEnumerator ShowSimpleItemDetail(){

			transform.localPosition = itemDetailMoveStart;

			transform.DOLocalMoveX (itemDetailMoveEndX, flyDuration);

			yield return new WaitForSeconds (flyDuration + stayDuration);

			transform.DOLocalMoveX (itemDetailMoveStart.x, flyDuration);

		}

		private void ClearItemDetail(){
			itemName.text = string.Empty;
			itemDescription.text = string.Empty;
			attachedDescription.text = string.Empty;
			itemIcon.sprite = null;
			itemIcon.enabled = false;

		}

	}
}
