using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	public class RewardInMap : MonoBehaviour {

		private Transform mBattlePlayerTrans;
		private Transform battlePlayerTrans{
			get{
				if (mBattlePlayerTrans == null) {
					mBattlePlayerTrans = Player.mainPlayer.transform.Find ("BattlePlayer");
				}
				return mBattlePlayerTrans;
			}
		}
			
		public SpriteRenderer sr;

		private Item reward;

		public float rewardFlyDuration = 0.5f;

		public void SetUpRewardInMap(Item reward, Vector3 rewardPosition){

			this.reward = reward;

			sr.sprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (reward);

			transform.position = new Vector3 (rewardPosition.x, rewardPosition.y + 1f, rewardPosition.z);

//			sr.sortingOrder = -(int)rewardPosition.y;

			gameObject.SetActive (true);

		}

		public void RewardFlyToPlayer(CallBack cb){
			StartCoroutine ("FlyToPlayer",cb);
		}

		private IEnumerator FlyToPlayer(CallBack cb){

			yield return new WaitUntil (()=>Time.timeScale == 1);

			float rewardUpAndDownSpeed = 0.5f;

			float timer = 0;
			while (timer < 0.5f) {
				Vector3 moveVector = new Vector3 (0, rewardUpAndDownSpeed * Time.deltaTime, 0);
				transform.position += moveVector;
				timer += Time.deltaTime;
				yield return null;
			}
			while (timer < 1f) {
				Vector3 moveVector = new Vector3 (0, -rewardUpAndDownSpeed * Time.deltaTime, 0);
				transform.position += moveVector;
				timer += Time.deltaTime;
				yield return null;
			}

			float passedTime = 0;

			float leftTime = rewardFlyDuration - passedTime;

//			BattlePlayerController bpCtr = battlePlayerTrans.GetComponent<BattlePlayerController> ();

			float distance = Mathf.Sqrt (Mathf.Pow ((battlePlayerTrans.position.x - transform.position.x), 2.0f) 
				+ Mathf.Pow ((battlePlayerTrans.position.y - transform.position.y), 2.0f));

			while (distance > 0.5f) {

				if (leftTime <= 0) {
					break;
				}

				Vector3 rewardVelocity = new Vector3 ((battlePlayerTrans.position.x - transform.position.x) / leftTime, 
					(battlePlayerTrans.position.y - transform.position.y) / leftTime, 0);

				Vector3 newRewardPos = new Vector3 (transform.position.x + rewardVelocity.x * Time.deltaTime, 
					transform.position.y + rewardVelocity.y * Time.deltaTime);

				transform.position = newRewardPos;

				passedTime += Time.deltaTime;

				leftTime = rewardFlyDuration - passedTime;

				distance = Mathf.Sqrt (Mathf.Pow ((battlePlayerTrans.position.x - transform.position.x), 2.0f) 
					+ Mathf.Pow ((battlePlayerTrans.position.y - transform.position.y), 2.0f));

				yield return null;

			}


			if (Player.mainPlayer.CheckBagFull (reward)) {
				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
					TransformManager.FindTransform ("BagCanvas").GetComponent<BagViewController> ().AddBagItemWhenBagFull (reward);
				}, false, true);
			} else {

				ExploreManager.Instance.ObtainReward (reward);
			}

			if (cb != null) {
				cb ();
			}

		}
			
	}
}
