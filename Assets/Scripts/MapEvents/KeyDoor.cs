using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public enum KeyType{
		Iron,
        Brass,
        Gold
	}


	public class KeyDoor : Door
    {

		private KeyType keyType;

		//private int mapIndex;

		private void ToolSelectCallBack(Item tool)
        {
			OpenTheDoor();
			Player.mainPlayer.RemoveItem(tool, 1);
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
        }


		public override void OpenTheDoor(bool playAudio = true)
		{
			base.OpenTheDoor(playAudio);

			int posX = Mathf.RoundToInt(this.transform.position.x);
            int posY = Mathf.RoundToInt(this.transform.position.y);

            MapEventsRecord.AddEventTriggeredRecord(mapIndex, new Vector2(posX, posY));
			MapEventsRecord.AddEventTriggeredRecord(mapIndex, pairDoorPos);

		}

		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
        {
			this.mapIndex = mapIndex;

            transform.position = attachedInfo.position;

            isWordTrigger = false;
            isOpen = false;

            direction = int.Parse(KVPair.GetPropertyStringWithKey("direction", attachedInfo.properties));

            keyType = (KeyType)int.Parse(KVPair.GetPropertyStringWithKey("type", attachedInfo.properties));

			string pairDoorPosString = KVPair.GetPropertyStringWithKey("pairDoorPos", attachedInfo.properties);

            string[] posXY = pairDoorPosString.Split(new char[] { '_' }, System.StringSplitOptions.RemoveEmptyEntries);

            int posX = int.Parse(posXY[0]);
            int posY = mapHeight - int.Parse(posXY[1]) - 1;

            pairDoorPos = new Vector3(posX, posY, transform.position.z);


			if (MapEventsRecord.IsMapEventTriggered(mapIndex, attachedInfo.position))
            {
				mapItemRenderer.sprite = null;
                isOpen = true;
                //ExploreManager.Instance.newMapGenerator.ChangeMapEventStatusAtPosition(pairDoorPos);
            }

            if (!isOpen)
            {
                mapItemRenderer.enabled = true;
                mapItemRenderer.sprite = doorCloseSprites[direction];
            }
            else
            {
                mapItemRenderer.sprite = null;
                mapItemRenderer.enabled = false;
            }

           
            bc2d.enabled = true;
            SetSortingOrder(-(int)transform.position.y);
                     
        }


        /// <summary>
        /// 查找人物身上对应的钥匙
        /// </summary>
        /// <returns>The key for key type.</returns>
        /// <param name="keyType">Key type.</param>
		private List<SpecialItem> FindKeyForKeyType(KeyType keyType){

			List<SpecialItem> keys = new List<SpecialItem>();

			SpecialItem key = null;

            switch (keyType)
            {
				case KeyType.Iron:
					key = Player.mainPlayer.allSpecialItemsInBag.Find(delegate (SpecialItem obj)
                    {
                        return obj.specialItemType == SpecialItemType.TieYaoShi;
                    });               
                    break;
				case KeyType.Brass:
                    key = Player.mainPlayer.allSpecialItemsInBag.Find(delegate (SpecialItem obj)
                    {
                        return obj.specialItemType == SpecialItemType.TongYaoShi;
                    });               
                    break;
				case KeyType.Gold:
                    key = Player.mainPlayer.allSpecialItemsInBag.Find(delegate (SpecialItem obj)
                    {
                        return obj.specialItemType == SpecialItemType.JinYaoShi;
                    });
                    break;
            }

			if(key != null){
				keys.Add(key);
			}


			key = Player.mainPlayer.allSpecialItemsInBag.Find(delegate (SpecialItem obj)
            {
				return obj.specialItemType == SpecialItemType.WanNengYaoShi;

            });
			if(key != null){
				keys.Add(key);
			}

			key = Player.mainPlayer.allSpecialItemsInBag.Find(delegate (SpecialItem obj)
            {
				return obj.specialItemType == SpecialItemType.QiaoZhen;

            });
            if (key != null)
            {
                keys.Add(key);
            }         
			return keys;
		}

		public override void EnterMapEvent(BattlePlayerController bp)
        {
           
			if (isOpen)
            {
                MapEventTriggered(true, bp);
                return;
            }

			List<SpecialItem> keys = FindKeyForKeyType(keyType);

            if (keys.Count == 0)
            {

                string keyName = string.Empty;

                switch (keyType)
                {
                    case KeyType.Iron:
                        keyName = "铁钥匙";
                        break;
                    case KeyType.Brass:
                        keyName = "铜钥匙";
                        break;
                    case KeyType.Gold:
                        keyName = "金钥匙";
                        break;
                }
                string tint = string.Format("缺少{0}x1", keyName);
                ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD(tint);
                bp.isInEvent = false;            
            }
            else
            {
                ExploreManager.Instance.expUICtr.SetUpToolSelectView(keys, ToolSelectCallBack);
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.keyAudioName);
            }
        }

		public override void MapEventTriggered(bool isSuccess, BattlePlayerController bp)
		{
			if (isOpen)
			{

				Vector3 continueMovePos = Vector3.zero;

				switch (bp.towards)
				{
					case MyTowards.Up:
						continueMovePos = pairDoorPos + new Vector3(0, 1, 0);
						break;
					case MyTowards.Down:
						continueMovePos = pairDoorPos + new Vector3(0, -1, 0);
						break;
					case MyTowards.Left:
						continueMovePos = pairDoorPos + new Vector3(-1, 0, 0);
						break;
					case MyTowards.Right:
						continueMovePos = pairDoorPos + new Vector3(1, 0, 0);
						break;
				}

				ExploreManager.Instance.expUICtr.DisplayTransitionMaskAnim(delegate
				{
					// 直接在门外的位置，没有移动动画
					bp.transform.position = continueMovePos;
					bp.singleMoveEndPos = continueMovePos;
					bp.moveDestination = continueMovePos;

					bp.SetSortingOrder(-Mathf.RoundToInt(continueMovePos.y));
					ExploreManager.Instance.newMapGenerator.miniMapPlayer.localPosition = (continueMovePos);
                    ExploreManager.Instance.newMapGenerator.ClearMiniMapMaskAround(continueMovePos);
                    ExploreManager.Instance.newMapGenerator.MiniMapCameraLatelySleep();
                    ExploreManager.Instance.expUICtr.UpdateMiniMapDisplay(continueMovePos);
					bp.isInEvent = false;
				});

			}


		}      
	



	}
}

