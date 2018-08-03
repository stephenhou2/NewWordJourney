using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	public class RewardInMap : MapEvent {

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

		private InstancePool rewardPool;

		public override void InitializeWithAttachedInfo(int mapIndex, MapAttachedInfoTile attachedInfo)
		{
			
		}

		public void SetUpRewardInMap(Item reward, Vector3 rewardPosition,InstancePool rewardPool){

			this.reward = reward;

			this.rewardPool = rewardPool;

			sr.sprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (reward);
            
			transform.position = new Vector3 (rewardPosition.x, rewardPosition.y, rewardPosition.z);
			sr.transform.position = new Vector3(rewardPosition.x, rewardPosition.y + 0.5f, rewardPosition.z);

			sr.sortingOrder = -Mathf.RoundToInt(rewardPosition.y);

			gameObject.SetActive (true);

			bool bagFull = Player.mainPlayer.CheckBagFull(reward);

			bool npcCanvasOnTop = false;

			Canvas npcCanvas = TransformManager.FindTransform("NPCCanvas").GetComponent<Canvas>();

            npcCanvasOnTop = npcCanvas.enabled;
         
			if(bagFull){
				
				if(npcCanvasOnTop){
					npcCanvas.GetComponent<NPCViewController>().tintHUD.SetUpSingleTextTintHUD("背包已满");
				}else{
					ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD("背包已满");
				}

				bc2d.enabled = true;
				StartCoroutine("RewardFloat");
				ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[Mathf.RoundToInt(rewardPosition.x), Mathf.RoundToInt(rewardPosition.y)] = 2;
			}else{
                
				if(!npcCanvasOnTop){
					ExploreManager.Instance.ObtainReward(reward);               
				}

				bc2d.enabled = false;

				RewardFlyToPlayer(delegate
				{
					AddToPool(rewardPool);
					ExploreManager.Instance.expUICtr.UpdateBottomBar();
				},true);
			}

		}


		public override void AddToPool(InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool(this.gameObject);
		}

		public override void EnterMapEvent(BattlePlayerController bp)
		{
			bool bagFull = Player.mainPlayer.CheckBagFull(reward);

            if (bagFull)
            {
				
                ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD("背包已满");
                bc2d.enabled = true;
                //StartCoroutine("RewardFloat");
				//ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y - 1)] = 10;
			}
			else
            {

                ExploreManager.Instance.ObtainReward(reward);

				ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)] = 1;

                bc2d.enabled = false;

                RewardFlyToPlayer(delegate
                {
                    AddToPool(rewardPool);
                    ExploreManager.Instance.expUICtr.UpdateBottomBar();
				},false);
            }

			bp.isInEvent = false;
		}

		public override void MapEventTriggered(bool isSuccess, BattlePlayerController bp)
		{
			
		}


		public void RewardFlyToPlayer(CallBack cb,bool needFloat){
			IEnumerator rewardFlyCoroutine = FlyToPlayer(cb, needFloat);
			StartCoroutine(rewardFlyCoroutine);
		}

		private IEnumerator RewardFloat(){

			yield return new WaitUntil(() => Time.timeScale == 1f);

			while(true){
				
				float rewardUpAndDownSpeed = 0.3f;

                float timer = 0;
                while (timer < 1f)
                {
                    Vector3 moveVector = new Vector3(0, rewardUpAndDownSpeed * Time.deltaTime, 0);
					sr.transform.position += moveVector;
                    timer += Time.deltaTime;
                    yield return null;
                }
                while (timer < 2f)
                {
                    Vector3 moveVector = new Vector3(0, -rewardUpAndDownSpeed * Time.deltaTime, 0);
					sr.transform.position += moveVector;
                    timer += Time.deltaTime;
                    yield return null;
                }

			}
            

		}

		private IEnumerator FlyToPlayer(CallBack cb,bool needFloat){

			StopCoroutine("RewardFloat");

			if(needFloat){
				
				yield return new WaitUntil(() => Time.timeScale == 1f);

                float rewardUpAndDownSpeed = 0.5f;

                float timer = 0;
                while (timer < 0.5f)
                {
                    Vector3 moveVector = new Vector3(0, rewardUpAndDownSpeed * Time.deltaTime, 0);
                    transform.position += moveVector;
                    timer += Time.deltaTime;
                    yield return null;
                }
                while (timer < 1f)
                {
                    Vector3 moveVector = new Vector3(0, -rewardUpAndDownSpeed * Time.deltaTime, 0);
                    transform.position += moveVector;
                    timer += Time.deltaTime;
                    yield return null;
                }
			}


			float passedTime = 0;

			float leftTime = rewardFlyDuration - passedTime;

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




			if (cb != null) {
				cb ();
			}

		}
			
	}
}
