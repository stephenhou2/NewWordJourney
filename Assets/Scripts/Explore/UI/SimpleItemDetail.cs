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
		public Image attachedSkillIcon;
		public Text attachedSkillName;
		public Text priceText;

		public float flyDuration;
		public float stayDuration;
		public Vector3 itemDetailMoveStart;
		public int itemDetailMoveEndX;

		private IEnumerator simpleItemDetailCoroutine;

		public void SetupSimpleItemDetail(Item item){
			
			ClearItemDetail ();

			itemName.text = item.itemName;

			switch(item.itemType){
			case ItemType.Equipment:
				itemDescription.text = item.itemDescription;
				Equipment eqp = item as Equipment;
				attachedDescription.text = eqp.attachedPropertyDescription;
				//if (eqp.attachedSkillId > 0) {

				//	Skill attachedSkill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
				//		return obj.skillId == eqp.attachedSkillId;
				//	});

				//	Sprite skillSprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find (delegate (Sprite obj) {
				//		return obj.name == attachedSkill.skillIconName;
				//	});

				//	attachedSkillIcon.sprite = skillSprite;
				//	attachedSkillIcon.enabled = skillSprite != null;

				//	attachedSkillName.text = attachedSkill.skillName;

				//}
				break;
			case ItemType.Consumables:
				attachedDescription.text = item.itemDescription;
				break;
			case ItemType.PropertyGemstone:
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
			attachedSkillIcon.sprite = null;
			attachedSkillIcon.enabled = false;
			attachedSkillName.text = string.Empty;
			priceText.text = string.Empty;
		}

	}
}
