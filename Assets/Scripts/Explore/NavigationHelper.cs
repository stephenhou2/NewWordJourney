using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace WordJourney
{
	public class NavigationHelper : MonoBehaviour {

		// 地图上可行走位置的二维数组
		// -2表示地图中的镂空区域，不可到达
		// －1表示墙，不可到达
		// 0表示障碍物,不可行走
		// 1表示正常地面
		// 10表示有陷阱（陷阱的消耗值为10，即如果为了绕过陷阱要多走10步以上时，直接走陷阱，小于10步时，则选择绕路【陷阱消耗可以根据需要进行修改】）
		// 1和10同时也是寻路参数中的G值
		public int[,] mapWalkableInfoArray;

		// 可以用来检测的点阵
		private List<PointIn2D> openList = new List<PointIn2D>();

		// 已关闭的点
		private List<PointIn2D> closedList = new List<PointIn2D>();

		// 自动寻路路径上的点集
		private List<Vector3> pathPos;

		// 地图宽度
		private int mapWidth;
		// 地图高度
		private int mapHeight;

		/// <summary>
		/// 寻路接口
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		/// <param name="mapWalkableInfoArray">Map walkable info array.</param>
		public List<Vector3> FindPath(Vector3 startPos,Vector3 endPos,int[,] mapWalkableInfoArray){


			PointIn2D startPoint = new PointIn2D(startPos);
			PointIn2D endPoint = new PointIn2D(endPos);

			pathPos = new List<Vector3> ();

//			bool isEndPosArrivable = mapWalkableInfoArray [endPoint.x, endPoint.y] == 1;
//
//			if (!isEndPosArrivable) {
//				return pathPos;
//			}

			// 每次自动探测路径前将待检测点集和已关闭点集和路径点集清空
			openList.Clear ();
			closedList.Clear ();
//			pathPos.Clear ();

			// 拿到全地图信息（包括是否可行走和每个点上的行走消耗）
			this.mapWalkableInfoArray = mapWalkableInfoArray;

			this.mapHeight = mapWalkableInfoArray.GetLength (1);
			this.mapWidth = mapWalkableInfoArray.GetLength(0);

			// 起点和终点重合，则将该点加入到路径点集中并直接返回
			if (startPoint.Equals (endPoint)) {
				pathPos.Add (endPos);
				return pathPos;
			}

			// 如果终点是不可行走点
			if (mapWalkableInfoArray [endPoint.x, endPoint.y] < 0) {
				Debug.Log ("点击在不可到达区域");
				return pathPos;
			}

			// 记录起始点
			PointIn2D currentPoint = startPoint;

			// 将起始点加入待检测点集
			openList.Add (startPoint);

			// 待检测点集内如果有待检测点，就遍历下去
			while (openList.Count != 0) {

				// 移除当前点
				openList.Remove (currentPoint);

				// 当前点加入到关闭点集中
				closedList.Add (currentPoint);

				// 拿到当前点的周围点集（上下左右）
				List<PointIn2D> surroundedPoints = currentPoint.GetSurroundedPoints ();

				// 遍历周围点集
				for(int i = 0; i<surroundedPoints.Count; i++) {
					
					PointIn2D p = surroundedPoints[i];

					if (p.x >= mapWidth || p.y >= mapHeight || p.x < 0 || p.y < 0) {
						continue;
					}

					// 如果周围点集中有终点，则停止检测，并利用fatherPoint属性逐级获取路径中的上级点
					if (p.Equals (endPoint)) {

//						Debug.LogFormat ("{0}监测到终点", p);
						p.fatherPoint = currentPoint;

						while (p.fatherPoint != null) {
							pathPos.Add (new Vector3 (p.x, p.y, 0));
							p = p.fatherPoint;

						}

						pathPos.Reverse ();

//						pathPos.RemoveAt (0);

						return pathPos;
					}

					// 如果周围点集中有不可行走点或者是已关闭点或者已经在待检测点集中，则这个点什么都不做
					if (UnwalkableOrClosedOrExistInOpen (p)) {
//						Debug.LogFormat ("{0}不可行走或已关闭或在待检测点集,可行走信息：{1}", p,mapWalkableInfoArray[p.x,p.y]);
						continue;
					}

					// 从地图信息中获取当前周围点集中可行走待检测点的行走耗费
					int G = mapWalkableInfoArray [p.x, p.y];

					// 计算F，G，H
					p.CaculateFGH (currentPoint, endPoint, G);

					// 设置父级节点
					p.fatherPoint = currentPoint;

					// 将可行走待检测点加入到待检测点集中
					openList.Add (p);
//					Debug.LogFormat ("将点{0}放入待检测点集中", p);

				}

				// 获取待检测点集中F值做小的点
				PointIn2D minFPoint = GetMinFPoint (openList);
//				Debug.LogFormat ("点{0}是F最小的点", minFPoint);


				if (minFPoint != null) {

					// 将F值最小的点设置为当前点（基点，周围点是由基点找出来的）
					currentPoint = minFPoint;

				}
					
			}

			// 走到这里说明没有有效路径，返回的pathPos内部节点数量为0
			return pathPos;
		}

		/// <summary>
		/// 获取待检测点集中F值最小的点
		/// </summary>
		/// <returns>The minimum F point.</returns>
		/// <param name="openList">Open list.</param>
		private PointIn2D GetMinFPoint(List<PointIn2D> openList){

			// 如果待检测点集中点的数量不为0，则对待检测点集中的点根据F值进行升序排列，返回F最小的点
			if (openList.Count != 0) {

				openList = openList.OrderBy (p => p.F).ToList ();

				return openList [0];

			} 

			// 待检测点集中没有点，返回null
			return null;

		}

		/// <summary>
		/// 点是否可行走或者是否已关闭或者已经在待检测点集中
		/// </summary>
		/// 地图外部或者障碍物或者不可到达点或者是已经关闭或者已经在待检测点集中返回true，其余返回false
		/// <param name="p">P.</param>
		private bool UnwalkableOrClosedOrExistInOpen(PointIn2D p){

//			if (mapWalkableInfoArray [p.x, p.y] == 0 || mapWalkableInfoArray [p.x, p.y] == -1) {
//				return true;
//			}


			if (mapWalkableInfoArray [p.x, p.y] <= 0) {
				return true;
			}

			foreach (PointIn2D closedPoint in closedList) {

				if (closedPoint.Equals (p)) {
					return true;
				}
			}

			foreach (PointIn2D openPoint in openList) {

				if (openPoint.Equals (p)) {
					return true;
				}
			}

			return false;
		}

		void OnDestroy(){
//			mapWalkableInfoArray = null;
		}

			
	}

	[System.Serializable]
	public class PointIn2D{

		public PointIn2D fatherPoint;

		public int x;

		public int y;

		public int F{ get; set;}

		private int H{ get; set;}

		private int G{ get; set;}

		public PointIn2D(int x,int y){
			this.x = x;
			this.y = y;
		}

		public PointIn2D(Vector3 positon){
			this.x = Mathf.RoundToInt(positon.x);
			this.y = Mathf.RoundToInt(positon.y);
		}

		public void CaculateFGH(PointIn2D fatherPoint,PointIn2D endPoint,int GofNewPoint){

			this.G = fatherPoint.G + GofNewPoint;

			this.H = Mathf.Abs (x - endPoint.x) + Mathf.Abs (y - endPoint.y);

			this.F = this.H + this.G;

		}

		public bool Equals(PointIn2D p){

			if (this.x == p.x && this.y == p.y) {
				return true;
			}
			return false;

		}

		public bool Equals(Vector3 position){
			return this.x == (int)(position.x) && this.y == (int)(position.y);
		}

		/// <summary>
		/// 获得当前点的周围所有点（上下左右）
		/// </summary>
		/// <returns>The surrounded optimize point.</returns>
		/// <param name="currentPoint">Current point.</param>
		public List<PointIn2D> GetSurroundedPoints(){

			List<PointIn2D> surroundedPoints = new List<PointIn2D> ();

			PointIn2D upPoint = new PointIn2D (x, y + 1);

			PointIn2D downPoint = new PointIn2D (x, y - 1);

			PointIn2D leftPoint = new PointIn2D (x - 1, y);

			PointIn2D rightPoint = new PointIn2D (x + 1, y);


			surroundedPoints.Add (upPoint);
			surroundedPoints.Add (downPoint);
			surroundedPoints.Add (leftPoint);
			surroundedPoints.Add (rightPoint);

			return surroundedPoints;

		}
			
		public override string ToString ()
		{
			return string.Format ("[Point: [{0},{1}],F={2}]", x, y, F);
		}



	}
}
