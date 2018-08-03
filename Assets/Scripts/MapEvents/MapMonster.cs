using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using System.Data;
	using DragonBones;

	public class MapMonster : MapWalkableEvent {

		public MonsterAlertArea[] alertAreas;

		public UnityArmatureComponent alertTint;

		public LayerMask collisionLayer;
        
		private float alertIconOffsetX;
		private float alertIconOffsetY;

		//private float wordOffsetX;
		//private float wordOffsetY;

		public bool isReadyToFight;

		// 触发机关的位置【如果没有触发的机关，则设置为（-1，-1，-1）】
		public Vector3 pairEventPos;

		public Vector2 oriPos;

		private int mapHeight;

		public int mapIndex;

		public bool hasReward = true;

		private MyTowards boneTowards;

		public void SetPosTransferSeed(int mapHeight){
			this.mapHeight = mapHeight;
		}


		protected override void Awake ()
		{

			base.Awake();

			alertIconOffsetX = alertTint.transform.localPosition.x;
			alertIconOffsetY = alertTint.transform.localPosition.y;

			//if(tmPro != null){
			//	wordOffsetX = tmPro.transform.localPosition.x + 1000;
   //             wordOffsetY = tmPro.transform.localPosition.y;
			//}

		}

		public override void AddToPool (InstancePool pool)
		{
			StopMoveImmidiately ();
			StopCoroutine ("DelayedMovement");
			//StopCoroutine("WordShowAndHide");
			DisableAllDetect ();
			isReadyToFight = false;
			HideAllAlertAreas ();
			bc2d.enabled = false;
			gameObject.SetActive (false);
			pool.AddInstanceToPool (this.gameObject);

		}


		public override void ResetWhenDie(){

			StopAllCoroutines ();
			HideAllAlertAreas ();
			alertTint.gameObject.SetActive(false);

		}


		public override void QuitFightAndDelayMove(int delay){

			StopMoveImmidiately ();

			isInAutoWalk = false;
			isTriggered = false;
			isReadyToFight = false;

			HideAllAlertAreas ();

			bc2d.enabled = true;

			StopCoroutine ("DelayedMovement");

			StartCoroutine ("DelayedMovement",delay);

		}


		private IEnumerator DelayedMovement(int delay){

			yield return new WaitForSeconds (delay);

			StartMove ();

		}


		public void ShowAlertAreaTint(){
			MyTowards towards = baCtr.towards;
			for (int i = 0; i < alertAreas.Length; i++) {
				if (i == (int)towards) {
					alertAreas [i].ShowAlerAreaTint ();
				} else {
					alertAreas [i].HideAlertAreaTint ();
				}
			}
		}

		public void DisableAllDetect(){

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].DisableAlertDetect ();
			}

			bc2d.enabled = false;

		}

		/// <summary>
		/// 隐藏所有的警示区域
		/// </summary>
		public void HideAllAlertAreas(){
			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].HideAlertAreaTint ();
			}
		}




		public void OnTriggerEnter2D (Collider2D col){

			BattlePlayerController bp = col.GetComponent<BattlePlayerController> ();

			if (bp == null) {
				return;
			}

			if (bp.fadeStepsLeft > 0)
            {
                return;
            }


			if (bp.isInEvent) {
				return;
			}

			if (bp.isInFight) {
				return;
			}

			if (baCtr.isDead) {
				return;
			}

			if (bp.isInPosFixAfterFight) {
				return;
			}

			isReadyToFight = true;

			//EnterMapEvent (bp);
			DetectPlayer(bp);
		}

		public override void EnterMapEvent (BattlePlayerController bp)
		{         
			if (isInMoving) {
				RefreshWalkableInfoWhenTriggeredInMoving ();
			}

			bp.isInEvent = true;

			ExploreManager.Instance.MapWalkableEventsStopAction ();

			StopMoveImmidiately ();

			bp.StopMoveAndWait ();

			ExploreManager.Instance.EnterFight (this.transform);

			MapEventTriggered (false, bp);
		}

		public void DetectPlayer(BattlePlayerController bp){

			if(bp.fadeStepsLeft > 0){
				return;
			}

			if (bp.escapeFromFight) {
				return;
			}

			bp.isInEvent = true;

			ExploreManager.Instance.DisableExploreInteractivity ();

			if (isInMoving) {
				RefreshWalkableInfoWhenTriggeredInMoving ();
			}

			ExploreManager.Instance.MapWalkableEventsStopAction ();

			StopMoveImmidiately ();

			bp.StopMoveAtEndOfCurrentStep ();

			ExploreManager.Instance.EnterFight (this.transform);

			//AlertTintSpark();

			MapEventTriggered (false, bp);

		}

		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			this.mapIndex = mapIndex;

			this.oriPos = attachedInfo.position;

			this.moveOrigin = attachedInfo.position;

			this.moveDestination = attachedInfo.position;

			canMove = bool.Parse(KVPair.GetPropertyStringWithKey("canMove",attachedInfo.properties));

			string pairEventPosString = KVPair.GetPropertyStringWithKey ("pairEventPos", attachedInfo.properties);

			if (pairEventPosString != string.Empty && pairEventPosString != "-1") {

				string[] posXY = pairEventPosString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

				int posX = int.Parse (posXY [0]);
				int posY = mapHeight - int.Parse (posXY [1]) - 1;

				pairEventPos = new Vector3 (posX, posY, transform.position.z);
			} else {
				pairEventPos = -Vector3.one;
			}

			this.gameObject.SetActive(true);

			for (int i = 0; i < alertAreas.Length; i++) {
				alertAreas [i].InitializeAlertArea ();
			}

			HideAllAlertAreas ();

            RandomTowards();

			if (canMove) {
				ShowAlertAreaTint ();
			}

			transform.position = attachedInfo.position;

			gameObject.SetActive (true);

			GetComponent<Monster> ().ResetBattleAgentProperties (true);

			baCtr.SetAlive();

			//StartMove ();

			bc2d.enabled = true;
			isReadyToFight = false;

			isTriggered = false;

			baCtr.isIdle = false;
			isInMoving = false;
			isInAutoWalk = false;
         
			alertTint.gameObject.SetActive(false);
         
		}

		private void RandomTowards(){

			int towardsIndex = Random.Range (0, 4);

			boneTowards = MyTowards.Right;

			switch (towardsIndex) {
			case 0:
				baCtr.TowardsRight ();
				boneTowards = MyTowards.Right;

				break;
			case 1:
				baCtr.TowardsLeft ();
				boneTowards = MyTowards.Left;
				break;
			case 2:
				baCtr.TowardsUp ();
				boneTowards = (baCtr as BattleMonsterController).GetMonsterBoneTowards();               
				break;
			case 3:
				baCtr.TowardsDown ();
				boneTowards = (baCtr as BattleMonsterController).GetMonsterBoneTowards();         
				break;
			}




		}


		/// <summary>
		/// 怪物头上红色感叹号闪烁动画
		/// </summary>
		/// <returns>The to fight icon shining.</returns>
		private void AlertTintSpark(){

			switch (boneTowards)
            {
                case MyTowards.Right:
                    alertTint.transform.localPosition = new Vector3(alertIconOffsetX, alertIconOffsetY, 0);
                    break;
                case MyTowards.Left:
                    alertTint.transform.localPosition = new Vector3(-alertIconOffsetX, alertIconOffsetY, 0);
                    break;
            }
         
			alertTint.gameObject.SetActive(true);

			StartCoroutine("AlertTintLatelyHide");
		}

		private IEnumerator AlertTintLatelyHide(){

			yield return new WaitForSeconds(0.1f);

			alertTint.animation.Play("default", 1);

			yield return new WaitUntil(() => alertTint.animation.isCompleted);

			alertTint.gameObject.SetActive(false);

		}
			

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			if (isTriggered) {
				return;
			}

			//Debug.Log("monster is not in triggered");

			bp.escapeFromFight = false;
			bp.isInEscaping = false;

			if (!isReadyToFight) {            
				ExploreManager.Instance.PlayerStartFight ();
			}

			//StartCoroutine ("AlertToFightIconShining");

			StartCoroutine ("ResetPositionAndStartFight", bp);

			isTriggered = true;

		}

		//private IEnumerator WordShowAndHide(){

		//	HLHWord word = null;

  //          bool hasValidWord = false;
  //          for (int i = 0; i < wordsArray.Length; i++)
  //          {
  //              if (wordsArray[i].spell.Length <= 12)
  //              {
  //                  hasValidWord = true;
  //                  word = wordsArray[i];
  //                  break;
  //              }
  //          }

  //          if (!hasValidWord)
  //          {
  //              word = GetAValidWord();            
  //          }


  //          if (tmPro != null)
  //          {
  //              tmPro.text = word.spell;
  //          }
  //          yield return new WaitForSeconds(1f);

  //          if (tmPro != null)
  //          {
  //              tmPro.text = string.Empty;
  //          }

		//}
			

		private IEnumerator ResetPositionAndStartFight(BattlePlayerController battlePlayerCtr){

			yield return new WaitUntil (() => isReadyToFight);

			AlertTintSpark();
         
			baCtr.PlayRoleAnim (CommonData.roleIdleAnimName, 0, null);

			bool playerNeedResetAttack = battlePlayerCtr.NeedResetAttack();

			yield return new WaitForSeconds (0.4f);

			HideAllAlertAreas ();
			DisableAllDetect ();

            Vector3 playerOriPos = battlePlayerCtr.transform.position;
            Vector3 monsterOriPos = transform.position;

            int playerPosX = Mathf.RoundToInt(playerOriPos.x);
            int playerPosY = Mathf.RoundToInt(playerOriPos.y);
            int monsterPosX = Mathf.RoundToInt(monsterOriPos.x);
            int monsterPosY = Mathf.RoundToInt(monsterOriPos.y);

            int monsterLayerOrder = -monsterPosY;

			int posOffsetX = playerPosX - monsterPosX; 
            int posOffsetY = playerPosY - monsterPosY;

			Vector3 monsterRunPos = Vector3.zero;
			Vector3 monsterFightPos = Vector3.zero;
            Vector3 playerFightPos = new Vector3(playerPosX, playerPosY, 0);

            int minX = 0;
            int maxX = ExploreManager.Instance.newMapGenerator.columns - 1;

			HLHRoleAnimInfo playerCurrentAnimInfo = battlePlayerCtr.GetCurrentRoleAnimInfo ();
         
			if (posOffsetX > 0) {
				
				battlePlayerCtr.TowardsLeft(!battlePlayerCtr.isInFight);
            
				if (playerPosX - 1 >= minX && playerPosX - 1 == monsterPosX && playerPosY == monsterPosY) {
					monsterRunPos = new Vector3(playerPosX - 1f, playerPosY, 0);
					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
				} else if (playerPosX - 1 >= minX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX - 1, playerPosY] == 1) {
					monsterRunPos = new Vector3(playerPosX - 0.5f, playerPosY, 0);
					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
                } else if (playerPosX - 1 >= minX && playerPosX - 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
					monsterRunPos = new Vector3(playerPosX - 1f, playerPosY, 0);
					monsterFightPos = new Vector3(playerPosX - 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
				} else {
                    if (posOffsetY > 0)
                    {     
						monsterRunPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);
                        monsterFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);
                        monsterLayerOrder = -playerPosY + 1;

                        playerFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY + 1);

                    }
                    else
                    {             
						monsterRunPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);
                        monsterFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);
                        monsterLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY - 1);
                    }
				}

			} else if (posOffsetX == 0) {

                if (playerPosX + 1 <= maxX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX + 1, playerPosY] == 1) {

					battlePlayerCtr.TowardsRight (!battlePlayerCtr.isInFight);

					monsterRunPos = new Vector3(playerPosX + 0.5f, playerPosY, 0);

					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;

                } else if (playerPosX + 1 <= maxX && playerPosX + 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
                    
					battlePlayerCtr.TowardsRight(!battlePlayerCtr.isInFight);

					monsterRunPos = new Vector3(playerPosX + 0.5f, playerPosY, 0);

					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;

                } else if (playerPosX - 1 >= minX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [playerPosX - 1, playerPosY] == 1) {

					battlePlayerCtr.TowardsLeft (!battlePlayerCtr.isInFight);

					monsterRunPos = new Vector3(playerPosX - 0.5f, playerPosY, 0);

					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;

                } else if (playerPosX - 1 >= minX && playerPosX - 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
                    
					battlePlayerCtr.TowardsLeft (!battlePlayerCtr.isInFight);

					monsterRunPos = new Vector3(playerPosX - 0.5f, playerPosY, 0);

					monsterFightPos = new Vector3 (playerPosX - 1, playerPosY, 0);

                    monsterLayerOrder = -playerPosY;

				} else {

                    if(posOffsetY > 0){

						monsterRunPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        monsterLayerOrder = -playerPosY + 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY + 1);

						battlePlayerCtr.TowardsRight(!battlePlayerCtr.isInFight);

                    }else{

						monsterRunPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

                        monsterLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);

                        baCtr.SetSortingOrder(-playerPosY - 1);

						battlePlayerCtr.TowardsRight(!battlePlayerCtr.isInFight);
                  
                    }
					
				}

			} else if (posOffsetX < 0) {

				battlePlayerCtr.TowardsRight (!battlePlayerCtr.isInFight);
            
				if (playerPosX + 1 <= maxX && playerPosX + 1 == monsterPosX && playerPosY == monsterPosY) {
					monsterRunPos = new Vector3(playerPosX + 1f, playerPosY, 0);
					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
                } else if (playerPosX + 1 <= maxX && ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray[playerPosX + 1, playerPosY] == 1) {
					monsterRunPos = new Vector3(playerPosX + 0.5f, playerPosY, 0);
					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
                } else if (playerPosX + 1 <= maxX && playerPosX + 1 == Mathf.RoundToInt (moveDestination.x) && playerPosY == Mathf.RoundToInt (moveDestination.y)) {
					monsterRunPos = new Vector3(playerPosX + 1f, playerPosY, 0);
					monsterFightPos = new Vector3 (playerPosX + 1, playerPosY, 0);
                    monsterLayerOrder = -playerPosY;
				} else {
                    if (posOffsetY > 0)
                    {

						monsterRunPos = new Vector3(playerPosX + 0.25f, playerPosY -0.15f, 0);
                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY - 0.15f, 0);

                        monsterLayerOrder = -playerPosY + 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY + 0.15f, 0);
                  
                        baCtr.SetSortingOrder(-playerPosY + 1);

                    }
                    else
                    {
						monsterRunPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);
                        monsterFightPos = new Vector3(playerPosX + 0.25f, playerPosY + 0.15f, 0);

                        monsterLayerOrder = -playerPosY - 1;

                        playerFightPos = new Vector3(playerPosX - 0.25f, playerPosY - 0.15f, 0);
                  
                        baCtr.SetSortingOrder(-playerPosY - 1);
                    }
				}
			}
            
			if(playerNeedResetAttack){
				battlePlayerCtr.ResetAttack(playerCurrentAnimInfo);
				ExploreManager.Instance.expUICtr.UpdateActiveSkillButtons();
			}
         

            battlePlayerCtr.FixPosTo(playerFightPos, null);
				
			RunToPosition (monsterRunPos, delegate {
            
				if(baCtr.isDead){
					return;
				}

				baCtr.FixPosTo(monsterFightPos, null);

				if(!battlePlayerCtr.escapeFromFight){

					if(transform.position.x <= ExploreManager.Instance.battlePlayerCtr.transform.position.x){
						baCtr.TowardsRight();
						boneTowards = MyTowards.Right;
					}else{
						baCtr.TowardsLeft();
						boneTowards = MyTowards.Left;
					}

					if (!battlePlayerCtr.isInEscaping && !battlePlayerCtr.isInFight) {
						ExploreManager.Instance.PlayerAndMonsterStartFight ();  
					} else {
						ExploreManager.Instance.MonsterStartFight ();

					}
				}else{
					bool monsterDie = baCtr.agent.health <= 0;
					RefreshWalkableInfoWhenQuit(monsterDie);
					QuitFightAndDelayMove(5);
					battlePlayerCtr.escapeFromFight = false;
				}
            },monsterLayerOrder);

		}


		public override void WalkToPosition(Vector3 position,CallBack cb,bool showAlertArea = true){

			baCtr.PlayRoleAnim (CommonData.roleWalkAnimName, 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);


			moveOrigin = new Vector3 (oriPosX, oriPosY, 0);
            moveDestination = new Vector3(targetPosX, targetPosY, 0);

			RefreshWalkableInfoWhenStartMove ();

			if (targetPosY == oriPosY) {
				if (targetPosX >= oriPosX) {
					baCtr.TowardsRight ();
					boneTowards = MyTowards.Right;
					alertTint.transform.localPosition = new Vector3 (alertIconOffsetX, alertIconOffsetY, 0);
				} else {
					baCtr.TowardsLeft ();
					boneTowards = MyTowards.Left;
				}
			} 
			else if(targetPosX == oriPosX){

				if (targetPosY >= oriPosY) {
					baCtr.TowardsUp ();
				} else {
					baCtr.TowardsDown ();
				}
			}

			if (moveCoroutine != null) {
				StopCoroutine (moveCoroutine);
			}

			float timeScale = 3f;

			if (showAlertArea) {
				ShowAlertAreaTint ();
			}

			moveCoroutine = MoveTo (position,timeScale,delegate{
				
				baCtr.PlayRoleAnim(CommonData.roleIdleAnimName,0,null);

				if(cb != null){
					cb();
				}

				SetSortingOrder (-Mathf.RoundToInt (position.y));
			});

			StartCoroutine (moveCoroutine);

		}



		protected override void RunToPosition(Vector3 position,CallBack cb,int layerOrder){

			baCtr.PlayRoleAnim (CommonData.roleRunAnimName, 0, null);

			int oriPosX = Mathf.RoundToInt (transform.position.x);
			int oriPosY = Mathf.RoundToInt (transform.position.y);

			int targetPosX = Mathf.RoundToInt (position.x);
			int targetPosY = Mathf.RoundToInt (position.y);


			moveOrigin = new Vector3 (oriPosX, oriPosY, 0);
			moveDestination = new Vector3 (targetPosX, targetPosY, 0);

//			Debug.LogFormat ("MOVE ORIGIN:{0}++++++MOVE DESTINATION:{1}", moveOrigin, moveDestination);

			RefreshWalkableInfoWhenStartMove ();

			float timeScale = 0.8f;

			if (position.x >= transform.position.x + 0.2f) {
				baCtr.TowardsRight ();
			} else if(position.x <= transform.position.x - 0.2f){
				baCtr.TowardsLeft ();
			}


			moveCoroutine = MoveTo (position,timeScale,delegate{

				this.transform.position = position;

				baCtr.PlayRoleAnim(CommonData.roleIdleAnimName,0,null);

				if(cb != null){
					cb();
				}

                SetSortingOrder (layerOrder);
			});

			StartCoroutine (moveCoroutine);

		}


		public override void SetSortingOrder (int order)
		{
			baCtr.SetSortingOrder (order);
		}
			

		private HLHWord GetAValidWord()
        {

            MySQLiteHelper mySql = MySQLiteHelper.Instance;

            mySql.GetConnectionWith(CommonData.dataBaseName);

            string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();

			string[] condition = { "wordLength <= 12 ORDER BY RANDOM() LIMIT 1" };

            IDataReader reader = mySql.ReadSpecificRowsOfTable(currentWordsTableName, null, condition, true);

            reader.Read();

            int wordId = reader.GetInt32(0);

            string spell = reader.GetString(1);

            string phoneticSymble = reader.GetString(2);

            string explaination = reader.GetString(3);

            string sentenceEN = reader.GetString(4);

            string sentenceCH = reader.GetString(5);

            string pronounciationURL = reader.GetString(6);

            int wordLength = reader.GetInt16(7);

            int learnedTimes = reader.GetInt16(8);

            int ungraspTimes = reader.GetInt16(9);

			bool isFamiliar = reader.GetInt16(10) == 1;

            HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes, isFamiliar);

            
            return word;
        }


          public Item GenerateRewardItem()
        {
			Monster monster = GetComponent<Monster>();

            Item rewardItem = null;

			if(monster.isBoss)
            {
				if(!hasReward){
					return null;
				}
				
                int index = 0;
				if(monster.monsterId % 2 == 0){
					
                    index = (Player.mainPlayer.currentLevelIndex / 5 + 1) * 1000;

                    List<EquipmentModel> ems = GameManager.Instance.gameDataCenter.allEquipmentModels.FindAll(delegate (EquipmentModel obj)
                    {
                        return obj.equipmentGrade == index;
                    });

                    int randomSeed = Random.Range(0, ems.Count);

                    rewardItem = new Equipment(ems[randomSeed], 1);
                }else{
					index = (Player.mainPlayer.currentLevelIndex / 5 + 2);
					if(index == 10){
						index = 9;
					}

                    List<EquipmentModel> ems = GameManager.Instance.gameDataCenter.allEquipmentModels.FindAll(delegate (EquipmentModel obj)
                    {
                        return obj.equipmentGrade == index;
                    });

                    int randomSeed = Random.Range(0, ems.Count);
               
                    rewardItem = new Equipment(ems[randomSeed], 1);

					(rewardItem as Equipment).SetToGoldQuality();
                }

            }
            else
            {

                int randomSeed = Random.Range(0, 100);

				int dropItemSeed = 0;

				switch(Player.mainPlayer.luckInMonsterTreasure){
					case 0:
						dropItemSeed = 5 + Player.mainPlayer.extraLuckInMonsterTreasure;
						break;
					case 1:
						dropItemSeed = 10 + Player.mainPlayer.extraLuckInMonsterTreasure;
						break;
				}

				if (randomSeed >= 0 && randomSeed < dropItemSeed)
                {
					randomSeed = Random.Range(0, 10);

                    // 掉落物品是30%的概率掉落装备
                    if (randomSeed <= 2)
                    {

                        int index = Player.mainPlayer.currentLevelIndex / 5 + 1;
                        
                        if (index == 10)
                        {
                            index = 9;
                        }

                        List<EquipmentModel> ems = GameManager.Instance.gameDataCenter.allEquipmentModels.FindAll(delegate (EquipmentModel obj)
                        {
                            return obj.equipmentGrade == index;
                        });
                        randomSeed = Random.Range(0, ems.Count);
                        rewardItem = new Equipment(ems[randomSeed], 1);
                    }
                    else
                    {
						int consumablesGrade = Player.mainPlayer.currentLevelIndex / 10;

						if(consumablesGrade >= 4){
							consumablesGrade = 3;
						}

						List<ConsumablesModel> cms = GameManager.Instance.gameDataCenter.allConsumablesModels.FindAll(delegate (ConsumablesModel obj)
						{
							return obj.consumablesGrade == consumablesGrade;
						});

						randomSeed = Random.Range(0, cms.Count);

						rewardItem = new Consumables(cms[randomSeed], 1);
                    }
                }else{
				    rewardItem = null;
			    }
                   
            }            

            return rewardItem;
        }

       

		
	}
}
