using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

    /// <summary>
    /// 地图事件触发记录
	/// 目前地图事件只记录 【金色宝箱】【直接在地图上的物品】【用钥匙开的门】【boss】【控制开关】[开关控制的门] 的触发记录
	/// 非地图事件只记录 【单词拼写是否完成】
	/// 触发记录只记录这些事件的位置，记录中有与查询的位置，则说明在这张地图上的该位置上的事件已经触发过
    /// </summary>
	[System.Serializable]
	public class MapEventsRecord
    {
        // 地图序号
		public int mapIndex;

		// 已触发过的事件位置记录表
		public List<Vector2> triggeredEventPositions = new List<Vector2>();

		public bool spellFinish;

		public bool diaryFinish;

		public MapEventsRecord(int mapIndex,List<Vector2> triggeredEventPositions,bool spellFinish,bool diaryFinish){
			this.mapIndex = mapIndex;
			this.triggeredEventPositions = triggeredEventPositions;
			this.spellFinish = spellFinish;
			this.diaryFinish = diaryFinish;
		}

       

        
        /// <summary>
        /// 判断制定地图上的指定位置上的事件是否已经触发过
        /// </summary>
        /// <returns><c>true</c>, if map event triggered was ised, <c>false</c> otherwise.</returns>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="eventPos">Event position.</param>
		public static bool IsMapEventTriggered(int mapIndex, Vector2 eventPos)
        {

            bool eventTriggered = false;

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate(MapEventsRecord obj)
			{
				return obj.mapIndex == mapIndex;

			});

            for (int i = 0; i < mapEventsRecord.triggeredEventPositions.Count; i++)
            {

                Vector2 triggeredPos = mapEventsRecord.triggeredEventPositions[i];

                if (MyTool.ApproximatelySameIntPosition2D(eventPos, triggeredPos))
                {
                    eventTriggered = true;
                    break;
                }

            }


            return eventTriggered;

        }


		public static bool IsSpellFinish(int mapIndex){
		
			bool spellFinish = false;

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate (MapEventsRecord obj)
            {
                return obj.mapIndex == mapIndex;            
            });
         
			spellFinish = mapEventsRecord.spellFinish;

			return spellFinish;

		}

		public static bool IsDiaryFinish(int mapIndex){
            
			bool diaryFinish = false;

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate (MapEventsRecord obj)
            {
               return obj.mapIndex == mapIndex;            
            });

			diaryFinish = mapEventsRecord.diaryFinish;

			return diaryFinish;

		}

		public static void AddEventTriggeredRecord(int mapIndex, Vector2 eventPos){

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate (MapEventsRecord obj)
            {
                return obj.mapIndex == mapIndex;
            });

			if(!mapEventsRecord.triggeredEventPositions.Contains(eventPos)){
				mapEventsRecord.triggeredEventPositions.Add(eventPos);
			}


		}

		public static void SpellFinishAtMapIndex(int mapIndex){

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate (MapEventsRecord obj)
            {
                return obj.mapIndex == mapIndex;
            });

			mapEventsRecord.spellFinish = true;

		}

		public static void DiaryFinishAtMapIndex(int mapIndex){

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate (MapEventsRecord obj)
            {
                return obj.mapIndex == mapIndex;
            });

			mapEventsRecord.diaryFinish = true;


		}
       
    }
}


