using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

    /// <summary>
    /// 通关记录界面控制器
    /// </summary>
	public class PlayRecordViewController : MonoBehaviour
	{
        // 通关记录
		private List<PlayRecord> playRecords;
        // 当前页面序号
		private int currentPageIndex;
        // 一页最多显示的通关记录数量
		public int maxCellCountInOnePage;
        // 通关记录显示界面
		public PlayRecordView playRecordView;

        // 最大页数
		private int maxPageIndex
		{
			get
			{
				return (playRecords.Count - 1) / maxCellCountInOnePage;
			}
		}


        /// <summary>
        /// 初始化通关记录界面
        /// </summary>
		public void SetUpPlayerRecordView()
		{

            // 获取通关记录
			playRecords = GameManager.Instance.gameDataCenter.allPlayRecords;
         
            // 对通关记录排序
			SortPlayRecordsOrder();

            // 通关数据传入通关显示界面
			playRecordView.InitPlayRecordView(playRecords);

            // 重置显示页数，从第0页开始显示
			currentPageIndex = 0;
         
            // 本页最小记录序号
			int minCellIndex = maxCellCountInOnePage * currentPageIndex;

			// 本页理论通关记录最大序号
			int idealMaxCellIndex = maxCellCountInOnePage * (currentPageIndex + 1) - 1;

            // 本页实际通关记录最大序号
			int actualMaxCellIndex = idealMaxCellIndex > playRecords.Count - 1 ? playRecords.Count - 1 : idealMaxCellIndex;

			playRecordView.SetUpPlayerRecordView(currentPageIndex, maxPageIndex, minCellIndex, actualMaxCellIndex);

		}
        
        /// <summary>
        /// 通关记录排序
        /// </summary>
		private void SortPlayRecordsOrder()
		{
			if(playRecords == null || playRecords.Count == 0){
				return;
			}

			for (int i = 0; i < playRecords.Count - 1;i++){
				
				for (int j = i; j < playRecords.Count - 1;j++){
					
					PlayRecord formerRecord = playRecords[j];
					PlayRecord laterRecord = playRecords[j + 1];

					if(laterRecord.maxExploreLevel > formerRecord.maxExploreLevel){

						playRecords[j] = laterRecord;

						playRecords[j + 1] = formerRecord;

					}else if(laterRecord.maxExploreLevel == formerRecord.maxExploreLevel){

						if(laterRecord.evaluatePoint > formerRecord.evaluatePoint){

							playRecords[j] = laterRecord;

                            playRecords[j + 1] = formerRecord;

						}

					}               
				}
			}


		}


        /// <summary>
        /// 用户点击了下一页按钮
        /// </summary>
		public void OnNextPageButtonClick()
		{

			if (currentPageIndex >= maxPageIndex)
			{
				return;
			}

            // 页数记录++
			currentPageIndex++;

            // 重算最大最小记录序号
			int minCellIndex = maxCellCountInOnePage * currentPageIndex;

			int idealMaxCellIndex = maxCellCountInOnePage * (currentPageIndex + 1) - 1;

			int actualMaxCellIndex = idealMaxCellIndex > playRecords.Count - 1 ? playRecords.Count - 1 : idealMaxCellIndex;

            // 更新通关记录显示界面
			playRecordView.SetUpPlayerRecordView(currentPageIndex, maxPageIndex, minCellIndex, actualMaxCellIndex);

      
		}

        /// <summary>
        /// 上一页按钮点击响应
        /// </summary>
		public void OnLastPageButtonClick(){

			if (currentPageIndex <= 0)
            {
                return;
            }

            currentPageIndex--;

            int minCellIndex = maxCellCountInOnePage * currentPageIndex;

			int idealMaxCellIndex = maxCellCountInOnePage * (currentPageIndex + 1) - 1;

            int actualMaxCellIndex = idealMaxCellIndex > playRecords.Count - 1 ? playRecords.Count - 1 : idealMaxCellIndex;

            playRecordView.SetUpPlayerRecordView(currentPageIndex, maxPageIndex, minCellIndex, actualMaxCellIndex);


		}

        /// <summary>
        /// 退出通关记录界面
        /// </summary>
		public void QuitPlayRecordView(){

			GameManager.Instance.UIManager.RemoveCanvasCache("PlayRecordCanvas");

		}


		public void DestroyInstances(){

			GetComponent<Canvas>().enabled = false;

			MyResourceManager.Instance.UnloadAssetBundle(CommonData.playRecordCanvasBundleName, true);

            Destroy(this.gameObject, 0.3f);

		}

        
    }
}

