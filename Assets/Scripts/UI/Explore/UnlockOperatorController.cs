using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;



	public class UnlockOperatorController : MonoBehaviour {

        
		public RectTransform lockMovePane;      
		public RectTransform lockHoleVisableTrans;
		//public RectTransform lockHoleValidTrans;
		public RectTransform lockHoleValidZone;


		public RectTransform keyMovePane;
		public RectTransform keyTrans;

		public Image lockImage;
		public Image lockHoleImage;

		public Sprite lockSprite;
		public Sprite unlockSprite;

		public Sprite normalLockHoleSprite;
		public Sprite enterLockHoleSprite;
		public Sprite missLockHoleSprite;

		public Image keyImage;
		public Sprite ironKeySprite;
		public Sprite brassKeySprite;
		public Sprite goldKeySprite;


		public float keyMoveSpeedBase;
		public float lockMoveSpeedBase;


		private float lockHoleValidZoneEdgeLeft;      
        private float lockHoleValidZoneEdgeRight;

		private float keyValidZoneEdgeLeft;
		private float keyValidZoneEdgeRight;


		private CallBack unlockSuccessCallBack;

		private CallBack unlockFailCallBack;

		private CallBack latelyQuitCallBack;

		private IEnumerator keyMoveCoroutine;
		//private IEnumerator lockMoveCoroutine;
        
              
		public void StartUnlockCheck(KeyType keyType, int unlockDifficulty,CallBack unlockSuccessCallBack,CallBack unlockFailCallBack,CallBack latelyQuitCallBack){

			this.unlockSuccessCallBack = unlockSuccessCallBack;
			this.unlockFailCallBack = unlockFailCallBack;
			this.latelyQuitCallBack = latelyQuitCallBack;

			this.gameObject.SetActive(true);

         
			keyValidZoneEdgeLeft = 0;
			keyValidZoneEdgeRight = keyMovePane.rect.width;

			keyTrans.localPosition = Vector3.zero;
                     

			lockImage.sprite = lockSprite;

			switch(keyType){
				case KeyType.Iron:
					keyImage.sprite = ironKeySprite;
					break;
				case KeyType.Brass:
					keyImage.sprite = brassKeySprite;
					break;
				case KeyType.Gold:
					keyImage.sprite = goldKeySprite;
					break;
			}


			if(keyMoveCoroutine != null){
				StopCoroutine(keyMoveCoroutine);
			}

			//if(lockMoveCoroutine != null){
			//	StopCoroutine(lockMoveCoroutine);
			//}
            

			float keyMoveSpeedLevel = 1f;

			switch(keyType){
				case KeyType.Iron:
					keyMoveSpeedLevel = 4.5f;
					break;
				case KeyType.Brass:
					keyMoveSpeedLevel = 3.75f;
					break;
				case KeyType.Gold:
					keyMoveSpeedLevel = 3f;
					break;
			}


			SetLock(unlockDifficulty);

			keyMoveCoroutine = KeyMove(keyMoveSpeedLevel);
         
			//lockMoveCoroutine = LockMove(unlockDifficulty);

			StartCoroutine(keyMoveCoroutine);
			//StartCoroutine(lockMoveCoroutine);

		}
        

		private IEnumerator KeyMove(float speedLevel){

			int direction = 1;

			float keyMoveSpeed = speedLevel * keyMoveSpeedBase;

			while (true) {

				if (keyTrans.localPosition.x > keyValidZoneEdgeRight) {
					direction = -1;
				} else if (keyTrans.localPosition.x < keyValidZoneEdgeLeft) {
					direction = 1;
				}

				Vector3 keyPositionFix = new Vector3 (keyMoveSpeed * Time.deltaTime * direction, 0, 0);

				keyTrans.localPosition += keyPositionFix;

				yield return null;
			}

		}
        
		private void SetLock(int difficulty){

			lockHoleImage.sprite = normalLockHoleSprite;

			int lockHoleVisableHeight = 100;

			int lockHoleVisableWidth = 90 - difficulty * 7;

			int lockHoleValidWidth = 45 - difficulty * 3;

            lockHoleValidZoneEdgeLeft = lockHoleVisableTrans.rect.width/2;
            lockHoleValidZoneEdgeRight = lockMovePane.rect.width - lockHoleVisableTrans.rect.width/2;

			lockHoleValidZone.sizeDelta = new Vector2(lockHoleValidWidth, lockHoleVisableHeight);
         
			lockHoleVisableTrans.sizeDelta = new Vector2(lockHoleVisableWidth, lockHoleVisableHeight);

            lockHoleVisableTrans.localPosition = new Vector3(Random.Range(lockHoleValidZoneEdgeLeft, lockHoleValidZoneEdgeRight), 0, 0);

		}

		private IEnumerator LockMove(int speed){

			int direction = 1;

			float lockMoveSpeed = speed * lockMoveSpeedBase;

			while (true) {

				if (lockHoleVisableTrans.localPosition.x > lockHoleValidZoneEdgeRight) {
					direction = -1;
				} else if (lockHoleVisableTrans.localPosition.x < lockHoleValidZoneEdgeLeft) {
					direction = 1;
				}

				bool inKeyInLock = CheckKeyInLock ();

				if (inKeyInLock) {
					lockHoleImage.sprite = enterLockHoleSprite;
				} else {
					lockHoleImage.sprite = normalLockHoleSprite;
				}

				Vector3 attackZonePositionFix = new Vector3 (lockMoveSpeed * Time.deltaTime * direction, 0, 0);

				lockHoleVisableTrans.localPosition += attackZonePositionFix;

				yield return null;
			}

		}      

		private bool CheckKeyInLock(){

			Vector2 checkCenter = keyTrans.position;

//			Debug.LogFormat ("check center x:{0}", checkCenter.x);

			float lockHoleLeftEdgeX = lockHoleVisableTrans.transform.position.x - lockHoleValidZone.rect.width / 2;

			float lockHoleRightEdgeX = lockHoleVisableTrans.transform.position.x + lockHoleValidZone.rect.width / 2;

//			Debug.LogFormat ("attack zone left:{0}  attack zone right:{1}", attackZoneLeftEdge, attackZoneRightEdgeX);

			return checkCenter.x >= lockHoleLeftEdgeX && checkCenter.x <= lockHoleRightEdgeX;

		}


		public void UnlockDoor(){
			bool unlockSuccess = CheckKeyInLock();
			IEnumerator unlockResultCoroutine = StopKeyAndLockMoveAndQuit(unlockSuccess);
			StartCoroutine(unlockResultCoroutine);
		}

        
		private IEnumerator StopKeyAndLockMoveAndQuit(bool unlockSuccess){

			if(keyMoveCoroutine != null){
				StopCoroutine(keyMoveCoroutine);
			}
            
			//if(lockMoveCoroutine != null){
			//	StopCoroutine(lockMoveCoroutine);
			//}

			if(!unlockSuccess){
				lockHoleImage.sprite = missLockHoleSprite;
			}else{
				lockHoleImage.sprite = enterLockHoleSprite;
			}

         
			if(unlockSuccess){
				lockImage.sprite = unlockSprite;

				if(unlockSuccessCallBack != null){
					unlockSuccessCallBack();
                }
			}else{
				if (unlockFailCallBack != null)
                {
					unlockFailCallBack();
                }
			}

			yield return new WaitForSeconds(1f);

			this.gameObject.SetActive(false);

			if(latelyQuitCallBack != null){
				latelyQuitCallBack();
			}
         
		}
              
      
	}
}
