using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using System.Data;

	public class Door : TriggeredGear {

		protected int direction;

		// 关闭的门图片数组（0:上 1:下 2:左 3:右）
		public Sprite[] doorCloseSprites;

		public bool isOpen;

		public bool isWordTrigger;

		public Vector3 pairDoorPos;

		protected int mapHeight;

        private HLHWord word;

		private bool isSwitchControledDoor;

		protected int mapIndex;



		public void SetPosTransferSeed(int mapHeight){
			this.mapHeight = mapHeight;
		}

		public virtual void OpenTheDoor(bool playAudio = true){
			if(isOpen){
				return;
			}

			if (isSwitchControledDoor)
            {
                MapEventsRecord.AddEventTriggeredRecord(mapIndex, transform.position);
                MapEventsRecord.AddEventTriggeredRecord(mapIndex, pairDoorPos);
            }


			mapItemRenderer.sprite = null;
			isOpen = true;
			ExploreManager.Instance.newMapGenerator.ChangeMapEventStatusAtPosition (pairDoorPos);
			if(playAudio){
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.doorAudioName);
			}

		}

		//public void CloseTheDoor(){
		//	mapItemRenderer.enabled = true;
		//	mapItemRenderer.sprite = doorCloseSprites[direction];
		//	isOpen = false;
		//	ExploreManager.Instance.newMapGenerator.ChangeMapEventStatusAtPosition(pairDoorPos);
  //          GameManager.Instance.soundManager.PlayAudioClip(CommonData.doorAudioName);
		//}



		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);         
		}
        

		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			this.mapIndex = mapIndex;

			transform.position = attachedInfo.position;

			isWordTrigger = bool.Parse (KVPair.GetPropertyStringWithKey ("isWordTrigger", attachedInfo.properties));
			isOpen = bool.Parse (KVPair.GetPropertyStringWithKey ("isOpen", attachedInfo.properties));

			isSwitchControledDoor = !isWordTrigger && !isOpen;

			direction = int.Parse(KVPair.GetPropertyStringWithKey("direction",attachedInfo.properties));

			if(MapEventsRecord.IsMapEventTriggered(mapIndex,attachedInfo.position)){
				OpenTheDoor(false);
			}

			if (!isOpen) {
				mapItemRenderer.enabled = true;
				mapItemRenderer.sprite = doorCloseSprites [direction];
			}else{
				mapItemRenderer.sprite = null;
				mapItemRenderer.enabled = false;
			}

			string pairDoorPosString = KVPair.GetPropertyStringWithKey ("pairDoorPos", attachedInfo.properties);

			string[] posXY = pairDoorPosString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

			int posX = int.Parse (posXY [0]);
			int posY = mapHeight - int.Parse (posXY [1]) - 1;

			pairDoorPos = new Vector3 (posX, posY,transform.position.z);

			bc2d.enabled = true;
			SetSortingOrder (-(int)transform.position.y);

            if (isWordTrigger)
            {
                bool hasValidWord = false;
                for (int i = 0; i < wordsArray.Length; i++)
                {
                    if (wordsArray[i].spell.Length <= 7)
                    {
                        hasValidWord = true;
                        word = wordsArray[i];
                        break;
                    }
                }

                if (!hasValidWord)
                {
                    word = GetAValidWord();               
                }
            }

		}



		public override void EnterMapEvent(BattlePlayerController bp)
		{


			if (isOpen) {
				MapEventTriggered (true, bp);
				return;
			}

			if (!isOpen && !isWordTrigger)
            {
                ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD("隐约听到齿轮转动的声音\n需要通过机关才能打开");
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.lockedDoorAudioName);
                bp.isInEvent = false;
			}else{

				ExploreManager.Instance.ShowCharacterFillPlane(word);

			}

           

		}
		private HLHWord GetAValidWord(){
			
			MySQLiteHelper mySql = MySQLiteHelper.Instance;

			mySql.GetConnectionWith (CommonData.dataBaseName);

			string currentWordsTableName = LearningInfo.Instance.GetCurrentLearningWordsTabelName();

			string[] condition = new string[]{"wordLength <= 7 ORDER BY RANDOM() LIMIT 1"};

			IDataReader reader = mySql.ReadSpecificRowsOfTable (currentWordsTableName, null, condition, true);

			reader.Read ();

			int wordId = reader.GetInt32 (0);

			string spell = reader.GetString (1);

			string phoneticSymble = reader.GetString (2);

			string explaination = reader.GetString (3);

			string sentenceEN = reader.GetString (4);

			string sentenceCH = reader.GetString (5);

			string pronounciationURL = reader.GetString (6);

			int wordLength = reader.GetInt16 (7);

			int learnedTimes = reader.GetInt16 (8);

			int ungraspTimes = reader.GetInt16 (9);

			bool isFamiliar = reader.GetInt16(10) == 1;

            HLHWord word = new HLHWord(wordId, spell, phoneticSymble, explaination, sentenceEN, sentenceCH, pronounciationURL, wordLength, learnedTimes, ungraspTimes, isFamiliar);
         
			return word;
		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{

			if (isSuccess) {            
				
				if (isOpen) {

					bp.boxCollider.enabled = false;               

					Vector3 continueMovePos = Vector3.zero;

					switch (bp.towards) {
					case MyTowards.Up:
						continueMovePos = pairDoorPos + new Vector3 (0, 1, 0);
						break;
					case MyTowards.Down:
						continueMovePos = pairDoorPos + new Vector3 (0, -1, 0);
						break;
					case MyTowards.Left:
						continueMovePos = pairDoorPos + new Vector3 (-1, 0, 0);
						break;
					case MyTowards.Right:
						continueMovePos = pairDoorPos + new Vector3 (1, 0, 0);
						break;
					}
                    
               
					ExploreManager.Instance.expUICtr.DisplayTransitionMaskAnim (delegate {
						bp.transform.position = continueMovePos;
						bp.singleMoveEndPos = continueMovePos;
						bp.moveDestination = continueMovePos;

						bp.isInEvent = false;
						bp.boxCollider.enabled = bp.fadeStepsLeft == 0;
						ExploreManager.Instance.newMapGenerator.miniMapPlayer.localPosition = (continueMovePos);
						ExploreManager.Instance.newMapGenerator.ClearMiniMapMaskAround(continueMovePos);
						ExploreManager.Instance.newMapGenerator.MiniMapCameraLatelySleep();
						ExploreManager.Instance.expUICtr.UpdateMiniMapDisplay(continueMovePos);

					});

				} else {
					OpenTheDoor ();
					ExploreManager.Instance.battlePlayerCtr.isInEvent = false;

				}
			}
		}

		public override void ChangeStatus ()
		{         
			if (isOpen)
            {
                return;
            }

            if (isSwitchControledDoor)
            {
                MapEventsRecord.AddEventTriggeredRecord(mapIndex, transform.position);
                MapEventsRecord.AddEventTriggeredRecord(mapIndex, pairDoorPos);
            }


            mapItemRenderer.sprite = null;
            isOpen = true;
            ExploreManager.Instance.newMapGenerator.ChangeMapEventStatusAtPosition(pairDoorPos);
		}

	}
}
