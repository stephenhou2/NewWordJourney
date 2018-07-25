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
		public Image itemIconBackground;
		public Text priceText;

		public Sprite grayFrame;
		public Sprite blueFrame;
		public Sprite goldFrame;
		public Sprite purpleFrame;

		public float flyDuration;
		public float stayDuration;
		public Vector3 itemDetailMoveStart;
		public int itemDetailMoveEndX;

		private IEnumerator simpleItemDetailCoroutine;

		public void SetupSimpleItemDetail(Item item){
			
			ClearItemDetail ();

			itemName.text = item.itemName;

			itemIconBackground.sprite = grayFrame;

			switch(item.itemType){
			case ItemType.Equipment:
				itemDescription.text = item.itemDescription;
				Equipment eqp = item as Equipment;
				attachedDescription.text = eqp.attachedPropertyDescription;
					switch(eqp.quality){
						case EquipmentQuality.Gray:
							itemIconBackground.sprite = grayFrame;
							break;
						case EquipmentQuality.Blue:
							itemIconBackground.sprite = blueFrame;
							break;
						case EquipmentQuality.Gold:
							itemIconBackground.sprite = goldFrame;
							break;
						case EquipmentQuality.Purple:
							itemIconBackground.sprite = purpleFrame;
							break;
					}
				break;
			case ItemType.Consumables:
				attachedDescription.text = item.itemDescription;
				break;
			case ItemType.PropertyGemstone:
				attachedDescription.text = item.itemDescription;
					break;
			case ItemType.SkillScroll:
				//SkillScroll skillScroll = item as SkillScroll;
				//Skill skill = GameManager.Instance.gameDataCenter.allSkills.Find(delegate (Skill obj)
				//{
				//	return obj.skillId == skillScroll.skillId;
				//});
				attachedDescription.text = item.itemDescription;
				break;
			case ItemType.SpecialItem:
				attachedDescription.text = item.itemDescription;
				break;
			}

			Sprite s = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);
			itemIcon.sprite = s;
			itemIcon.enabled = s != null;

			priceText.text = (item.price / 8).ToString();

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
			priceText.text = string.Empty;
		}

	}
}
