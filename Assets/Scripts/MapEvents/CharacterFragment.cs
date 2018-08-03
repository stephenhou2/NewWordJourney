using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using DG.Tweening;

	public delegate void ObtainCharacterFragmentCallBack(char character);

	public class CharacterFragment : MonoBehaviour
    {

		public float floatingInterval = 2f;
        public float floatingDistance = 0.1f;

		public BoxCollider2D boxCollider;

		public float oriPosY;

		public Transform fragmentTrans;

        private Sequence floatingSequence;

		public float fragmentFlyDuration = 0.5f;

		private char character;

		private Vector2 oriPos;

		private InstancePool characterFragmentPool;

		private ObtainCharacterFragmentCallBack obtainCharacterFragmentCallBack;

		public CharacterSmallDetect characterSmallDetect;

		private void Awake()
		{
			characterSmallDetect.InitSmallDetect(delegate
			{
				CharacterFragmentFlyToPlayer(ExploreManager.Instance.battlePlayerCtr);
			});
		}

		public void SetPool(InstancePool pool){
			this.characterFragmentPool = pool;
		}
        
		public void GenerateCharacterFragment(char character,Vector3 position,ObtainCharacterFragmentCallBack obtainCharacterFragmentCallBack){

			this.gameObject.SetActive(true);

			this.character = character;

			this.transform.position = position;

			oriPos = position;

			this.obtainCharacterFragmentCallBack = obtainCharacterFragmentCallBack;

			this.boxCollider.enabled = true;

			characterSmallDetect.SetBoxColliderEnable(true);

			//oriPosY = fragmentTrans.localPosition.y;

			fragmentTrans.GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(position.y);

			BeginFloating();

		}


		public void AddToPool(InstancePool pool)
		{

			this.boxCollider.enabled = false;

			this.gameObject.SetActive(false);

			int posX = Mathf.RoundToInt(oriPos.x);
			int posY = Mathf.RoundToInt(oriPos.y);
            
			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[posX, posY] = 1;

			characterSmallDetect.SetBoxColliderEnable(false);

			pool.AddInstanceToPool(this.gameObject);

		}

		public void OnTriggerEnter2D(Collider2D col)
		{
			BattlePlayerController battlePlayer = col.transform.GetComponent<BattlePlayerController>();

			if(battlePlayer == null){
				return;
			}

			StopFloating();

			characterSmallDetect.SetBoxColliderEnable(false);

			CharacterFragmentFlyToPlayer(battlePlayer);

			//boxCollider.enabled = false;

		}

		private void BeginFloating()
        {

            floatingSequence = DOTween.Sequence();


            float floatingTop = oriPosY + floatingDistance;

            floatingSequence.Append(fragmentTrans.DOLocalMoveY(floatingTop, floatingInterval))
                .Append(fragmentTrans.DOLocalMoveY(oriPosY, floatingInterval));

            floatingSequence.SetLoops(-1);
            floatingSequence.Play();
        }

        private void StopFloating()
        {

            floatingSequence.Kill(false);

            fragmentTrans.localPosition = new Vector3(fragmentTrans.localPosition.x, oriPosY, fragmentTrans.localPosition.z);

        }

		private void CharacterFragmentFlyToPlayer(BattlePlayerController battlePlayer){

			IEnumerator fragmentFlyCoroutine = FlyToPlayer(battlePlayer, delegate
			{
				obtainCharacterFragmentCallBack(character);
			});

			StartCoroutine(fragmentFlyCoroutine);

		}

		private IEnumerator FlyToPlayer(BattlePlayerController battlePlayer,CallBack cb)
        {

            yield return new WaitUntil(() => Time.timeScale == 1);

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

            float passedTime = 0;

			float leftTime = fragmentFlyDuration - passedTime;
            

			float distance = Mathf.Sqrt(Mathf.Pow((battlePlayer.transform.position.x - transform.position.x), 2.0f)
			                            + Mathf.Pow((battlePlayer.transform.position.y - transform.position.y), 2.0f));

            while (distance > 0.5f)
            {
                
                if (leftTime <= 0)
                {
                    break;
                }

				Vector3 rewardVelocity = new Vector3((battlePlayer.transform.position.x - transform.position.x) / leftTime,
				                                     (battlePlayer.transform.position.y - transform.position.y) / leftTime, 0);

                Vector3 newRewardPos = new Vector3(transform.position.x + rewardVelocity.x * Time.deltaTime,
                    transform.position.y + rewardVelocity.y * Time.deltaTime);

                transform.position = newRewardPos;

                passedTime += Time.deltaTime;

				leftTime = fragmentFlyDuration - passedTime;

				distance = Mathf.Sqrt(Mathf.Pow((battlePlayer.transform.position.x - transform.position.x), 2.0f)
				                      + Mathf.Pow((battlePlayer.transform.position.y - transform.position.y), 2.0f));

                yield return null;

            }

            if (cb != null)
            {
                cb();
            }

			AddToPool(characterFragmentPool);

        }



	}

}

