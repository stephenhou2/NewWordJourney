using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class MyVerticalScrollView : MonoBehaviour,IDragHandler,IBeginDragHandler {

		private int totalModelCount;//数据条数

		private float cellHeight;//单个cell的高度
		private float scrollViewPortHeight;//scrollView的可视窗口高度
		private float topOffsetY;//顶部间距
//		private float bottomOffsetY;//底部间距
		private float paddingY;//scrollViewcell之间的间距
		private int maxCellsVisible;//最多同时显示的cell数量
//		private int maxPreloadCellCount;//预加载cell数量

		private int currentMinCellIndex;//当前最上部（包括不可见）cell的序号
		private int currentMaxCellIndex;//当前最底部（包括不可见）cell的序号

		private List<object> allDataList;
		//private Equipment compareEquipment;//用来比较的equipment

		public Transform scrollContentContainer;//cell容器

		private InstancePool cellPool;//cell缓存池
		private Transform cellModel;//cell模型

		// 拖拽过程中的cell重用计数（底部cell移至顶部count++，顶部cell移至底部count--）
		// 可以为负数
		// scrollRect在拖拽过程中，content的localPosition由onDrag传入的PointerEventData计算而来，
		// 无法直接更改localPosition（更改后也会被从PointerEventData计算的position替换掉），因此需要自己传入PointerEventData
		// 记录拖拽过程的重用计数，获取原始PointerEventData后根据count更改其中的position后传入onDrag接口
		// 只有拖拽过程中会有content的localposition无法更改的问题 自由滑动过程中没有这个问题
		private int reuseCount;


		void Awake(){
			GetComponent<ScrollRect> ().onValueChanged.AddListener (ReuseCellAndUpdateContentPosition);
		}

		public void InitVerticalScrollViewData (List<object> dataList, InstancePool cellPool, Transform cellModel){

			this.allDataList = dataList;
			this.cellPool = cellPool;
			this.cellModel = cellModel;

			totalModelCount = dataList.Count;

			scrollViewPortHeight = (transform as RectTransform).rect.height;

			cellHeight = (cellModel as RectTransform).rect.height;

			VerticalLayoutGroup layout = scrollContentContainer.GetComponent<VerticalLayoutGroup> ();

			paddingY = layout.spacing;

			topOffsetY = layout.padding.top;

//			bottomOffsetY = layout.padding.bottom;

			maxCellsVisible = (int)(scrollViewPortHeight / (cellHeight + paddingY) + 2);


		}

		public void SetUpScrollView(){

			cellPool.AddChildInstancesToPool (scrollContentContainer);

			scrollContentContainer.localPosition = new Vector3 (scrollContentContainer.localPosition.x, 0, 0);

			//如果当前选中类的所有装备数量小于预加载数量，则只加载实际装备数量的cell
			int maxCount = maxCellsVisible < allDataList.Count ? maxCellsVisible : allDataList.Count;

			for(int i =0;i<maxCount;i++){

				object data = allDataList[i];

				Transform itemDetail = cellPool.GetInstance<Transform> (cellModel.gameObject,scrollContentContainer);

				itemDetail.GetComponent<CellDetailView> ().SetUpCellDetailView (allDataList [i]);

			}

			//初始化当前顶部和底部的equipment序号
			currentMinCellIndex = 0;
			currentMaxCellIndex = maxCount < 1 ? 0 : maxCount - 1;

		}


		/// <summary>
		/// 装备更换页面开始进行拖拽时，重置拖拽过程中cell重用计数
		/// </summary>
		public void OnBeginDrag(PointerEventData data){

			reuseCount = 0;

			if (currentMinCellIndex == 0 || currentMaxCellIndex == totalModelCount - 1) {
				GetComponent<ScrollRect> ().movementType = ScrollRect.MovementType.Clamped;
			} else {
				GetComponent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
			}

		}


		public void OnDrag(PointerEventData data){

			if (currentMinCellIndex == 0 || currentMaxCellIndex == totalModelCount - 1) {
				GetComponent<ScrollRect> ().movementType = ScrollRect.MovementType.Clamped;
			} else {
				GetComponent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
			}


			// content的位置移动（cell重用数量 * （cell高度+cell间距））
			// 为了方便这里计算，需要设定cell间距和scrollView顶部间距保持一致
			float offset = reuseCount * (cellHeight + paddingY);

			data.position = new Vector2 (data.position.x, 
				data.position.y - offset/CommonData.scalerToPresetResulotion);

			// 传入更新后的PointerEventData
			if (data != null) {
				GetComponent<ScrollRect> ().OnDrag (data);
			}

		}



		/// <summary>
		/// scrollView滑动过程中根据位置回收重用cell
		/// </summary>
		private void ReuseCellAndUpdateContentPosition(Vector2 pos){

			// 获得content的localPosition
			float scrollRectPosY = scrollContentContainer.localPosition.y;
			// 获得content的滚动速度
			float velocityY = GetComponent<ScrollRect> ().velocity.y;

			// 向下滚动
			if (velocityY > 0) {

				// 如果最底部equipment是当前选中类型equipments中的最后一个
				if (currentMaxCellIndex >= totalModelCount - 1) {
					Debug.Log ("所有数据加载完毕");
					return;
				}

				// 判断最顶部cell是否已经不可见
				bool firstCellInvisible = (int)(scrollRectPosY / (cellHeight + topOffsetY))  >= 1;

				// 顶部cell移至底部并更新显示数据
				if (firstCellInvisible) {

					// 获得顶部cell
					Transform cell = scrollContentContainer.GetChild (0);

					// 移至底部
					cell.SetAsLastSibling ();

					// 获得将显示的euipment数据
					object data = allDataList [currentMaxCellIndex + 1];

					// 更新该cell的显示数据
					cell.GetComponent<CellDetailView> ().SetUpCellDetailView (data);

					// 计算content新的位置信息
					float newPosY = scrollContentContainer.localPosition.y - cellHeight - paddingY;

					// 移动content，确保屏幕显示和cell重用前一致
					//（cell重用时由于autoLayout，cell位置会产生变化，整体移动content的位置确保每个cell回到重用前的位置）
					scrollContentContainer.localPosition = new Vector3 (0, newPosY, 0);

					// 最顶部和最底部equipment的序号++
					currentMaxCellIndex++;
					currentMinCellIndex++;

					// 重用数量++
					reuseCount++;

				} 

			} else if (velocityY < 0) {//向下滚动

				// 如果最顶部equipment是当前选中类型equipments中的第一个
				if (currentMinCellIndex <= 0) {
//					Debug.Log ("所有物品加载完毕");
					return;
				}

				// 判断最底部cell是否可见
				bool lastCellInvisble = (maxCellsVisible - 1) * (cellHeight + paddingY) + topOffsetY - scrollRectPosY >= scrollViewPortHeight;

				if (lastCellInvisble) {

					Transform cell = scrollContentContainer.GetChild (scrollContentContainer.childCount - 1);

					cell.SetAsFirstSibling ();

					object data = allDataList [currentMinCellIndex - 1];

					cell.GetComponent<CellDetailView> ().SetUpCellDetailView (data);

					float newPosY = scrollContentContainer.localPosition.y + cellHeight + paddingY;

					scrollContentContainer.localPosition = new Vector3 (0, newPosY, 0);

					currentMaxCellIndex--;
					currentMinCellIndex--;

					reuseCount--;

				}
			}
		}


	}
}
