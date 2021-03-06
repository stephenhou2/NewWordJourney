﻿using System.Collections;
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

        // 拼写是否已经完成
		public bool spellFinish;

        // 日记是否已经完成
		public bool diaryFinish;

        /// <summary>
        /// 地图事件记录类
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="triggeredEventPositions">Triggered event positions.</param>
        /// <param name="spellFinish">If set to <c>true</c> spell finish.</param>
        /// <param name="diaryFinish">If set to <c>true</c> diary finish.</param>
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
        

        /// <summary>
        /// 判断拼写是否已经完成了
        /// </summary>
        /// <returns><c>true</c>, if spell finish was ised, <c>false</c> otherwise.</returns>
        /// <param name="mapIndex">Map index.</param>
		public static bool IsSpellFinish(int mapIndex){
		
			bool spellFinish = false;

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate (MapEventsRecord obj)
            {
                return obj.mapIndex == mapIndex;            
            });
         
			spellFinish = mapEventsRecord.spellFinish;

			return spellFinish;

		}

        /// <summary>
        /// 判断日记是否已经完成了
        /// </summary>
        /// <returns><c>true</c>, if diary finish was ised, <c>false</c> otherwise.</returns>
        /// <param name="mapIndex">Map index.</param>
		public static bool IsDiaryFinish(int mapIndex){
            
			bool diaryFinish = false;

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate (MapEventsRecord obj)
            {
               return obj.mapIndex == mapIndex;            
            });

			diaryFinish = mapEventsRecord.diaryFinish;

			return diaryFinish;

		}

        /// <summary>
        /// 添加事件触发记录
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="eventPos">Event position.</param>
		public static void AddEventTriggeredRecord(int mapIndex, Vector2 eventPos){

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate (MapEventsRecord obj)
            {
                return obj.mapIndex == mapIndex;
            });

			if(!mapEventsRecord.triggeredEventPositions.Contains(eventPos)){
				mapEventsRecord.triggeredEventPositions.Add(eventPos);
			}

            
		}

        /// <summary>
        /// 记录在指定地图序号上拼写已经完成【由于地图序号在随机后已经固定下来，所以指定地图序号就对应到指定关卡上】
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
		public static void SpellFinishAtMapIndex(int mapIndex){

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate (MapEventsRecord obj)
            {
                return obj.mapIndex == mapIndex;
            });

			mapEventsRecord.spellFinish = true;

		}

        /// <summary>
        /// 记录日记已在指定地图序号上完成
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
		public static void DiaryFinishAtMapIndex(int mapIndex){

			MapEventsRecord mapEventsRecord = GameManager.Instance.gameDataCenter.mapEventsRecords.Find(delegate (MapEventsRecord obj)
            {
                return obj.mapIndex == mapIndex;
            });

			mapEventsRecord.diaryFinish = true;


		}
       
    }


    // 本层地图事件触发记录
	[System.Serializable]
	public class CurrentMapEventsRecord{

		// 地图序号
        public int mapIndex;

        // 已触发过的事件位置记录表
        public List<Vector2> triggeredEventPositions = new List<Vector2>();

        // npc位置信息数组
		public Vector2[] npcPosArray;

        // npc数组
		public HLHNPC[] npcArray;

        // 谜语门位置信息数组
		public Vector2[] puzzleDoorPosArray;
        
		public static bool CheckRecordValid(CurrentMapEventsRecord record){
			return record != null && record.mapIndex >= 0 && record.mapIndex <= CommonData.maxLevelIndex && record.triggeredEventPositions != null 
				   && record.npcPosArray != null && record.npcPosArray.Length > 0
				   && record.npcArray != null && record.npcArray.Length > 0 
				   && record.puzzleDoorPosArray != null && record.puzzleDoorPosArray.Length > 0;
		}
        
		public void Reset(){
			mapIndex = -1;
		}


		public CurrentMapEventsRecord(int mapIndex, List<Vector2> triggeredEventPositions)
        {
            this.mapIndex = mapIndex;
            this.triggeredEventPositions = triggeredEventPositions;
			npcPosArray = new Vector2[] { -Vector2.one, -Vector2.one, -Vector2.one, -Vector2.one};
			npcArray = new HLHNPC[4];
			puzzleDoorPosArray = new Vector2[] { -Vector2.one, -Vector2.one };
        }


		/// <summary>
        /// 判断制定地图上的指定位置上的事件是否已经触发过
        /// </summary>
        /// <returns><c>true</c>, if map event triggered was ised, <c>false</c> otherwise.</returns>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="eventPos">Event position.</param>
        public bool IsMapEventTriggered(int mapIndex, Vector2 eventPos)
        {

            bool eventTriggered = false;

			for (int i = 0; i <GameManager.Instance.gameDataCenter.currentMapEventsRecord.triggeredEventPositions.Count; i++)
            {

				Vector2 triggeredPos = GameManager.Instance.gameDataCenter.currentMapEventsRecord.triggeredEventPositions[i];

                if (MyTool.ApproximatelySameIntPosition2D(eventPos, triggeredPos))
                {
                    eventTriggered = true;
                    break;
                }

            }   

            return eventTriggered;         
        }

        /// <summary>
        /// 添加本层地图事件记录
        /// </summary>
        /// <param name="mapIndex">Map index.</param>
        /// <param name="eventPos">Event position.</param>
		public void AddEventTriggeredRecord(int mapIndex, Vector2 eventPos)
        {     
			if (!GameManager.Instance.gameDataCenter.currentMapEventsRecord.triggeredEventPositions.Contains(eventPos))
            {
				GameManager.Instance.gameDataCenter.currentMapEventsRecord.triggeredEventPositions.Add(eventPos);
            }         
        }      

	}


}


