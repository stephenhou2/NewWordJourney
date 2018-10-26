

namespace WordJourney{

    /// <summary>
    /// 小地图探索记录数据模型
    /// </summary>
	[System.Serializable]
	public class MiniMapRecord
    {
        // 地图序号
		public int mapIndex;
        // 地图宽度
		public int mapWidth;
        // 地图高度
		public int mapHeight;

        // 记录所有已走过的点
		public int[] recordArray;
       
        // 构造函数
		public MiniMapRecord(int mapIndex,int width,int height){
			this.mapIndex = mapIndex;
			this.mapWidth = width;
			this.mapHeight = height;
			this.recordArray = new int[width*height];
		}


    }

}

