using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;


    /// <summary>
    /// 通关记录界面
    /// </summary>
	public class PlayRecordView : MonoBehaviour
    {

        // 简易通关记录模型
		public PlayRecordSimpleCell simpleCellModel;
        // 简易通关记录缓存池
		public InstancePool simpleRecordCellPool;
        // 简易通关记录容器
		public Transform simpleRecordCellContainer;
        // 页数
		public Text pageText;
        // 没有通关记录时的提示文本
		public Text noDataHintText;
        // 下一页按钮
		public Button nextPageButton;
        // 上一页按钮
		public Button lastPageButton;
		// 详细通关记录界面
		public PlayRecordDetailHUD playRecordDetailHUD; 

        // 通关记录
		private List<PlayRecord> playRecords;

        // 获取通关记录
		public void InitPlayRecordView(List<PlayRecord> playRecords){
			this.playRecords = playRecords;
		}
        
        /// <summary>
		/// 初始化通关记录界面【初始化UI】
        /// </summary>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="maxPageIndex">Max page index.</param>
        /// <param name="minRecordIndex">Minimum record index.</param>
        /// <param name="maxRecordIndex">Max record index.</param>
		public void SetUpPlayerRecordView(int pageIndex, int maxPageIndex, int minRecordIndex,int maxRecordIndex){
        

			if (playRecords == null || playRecords.Count == 0)
			{
				noDataHintText.enabled = true;
				nextPageButton.gameObject.SetActive(false);
				lastPageButton.gameObject.SetActive(false);
				pageText.enabled = false;
				return;
			}
			
			noDataHintText.enabled = false;
			nextPageButton.gameObject.SetActive(true);
			lastPageButton.gameObject.SetActive(true);
			pageText.enabled = true;
         
			pageText.text = string.Format("{0} / {1}", pageIndex + 1, maxPageIndex + 1);

			simpleRecordCellPool.AddChildInstancesToPool(simpleRecordCellContainer);

			for (int i = minRecordIndex; i <= maxRecordIndex;i++){
				
				if(i < 0 || i >= playRecords.Count){
					continue;
				}

				PlayRecord playRecord = playRecords[i];
				PlayRecordSimpleCell playRecordSimpleCell = simpleRecordCellPool.GetInstance<PlayRecordSimpleCell>(simpleCellModel.gameObject, simpleRecordCellContainer);

				int recordIndex = i;

				playRecordSimpleCell.SetUpPlayRecordSimpleCell(playRecord,recordIndex, delegate
				{               
					playRecordDetailHUD.SetUpPlayRecordDetailHUD(playRecord);
				});
                                   
			}

		}

        
    }
}

