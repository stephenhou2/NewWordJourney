using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney{

	[System.Serializable]
	public class MiniMapRecord
    {

		public int mapIndex;

		public int mapWidth;

		public int mapHeight;

        // 记录所有已走过的点
		public int[] recordArray;

		public MiniMapRecord(int mapIndex,int width,int height){
			this.mapIndex = mapIndex;
			this.mapWidth = width;
			this.mapHeight = height;
			this.recordArray = new int[width*height];
		}


    }

}

