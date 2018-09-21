using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class PlayRecordView : MonoBehaviour
    {

		public PlayRecordSimpleCell simpleCellModel;

		public InstancePool simpleRecordCellPool;

		public Transform simpleRecordCellContainer;

		public Text pageText;

		public Text noDataHintText;

		public Button nextPageButton;

		public Button lastPageButton;

		public PlayRecordDetailHUD playRecordDetailHUD; 

		private List<PlayRecord> playRecords;

		public void InitPlayRecordView(List<PlayRecord> playRecords){
			this.playRecords = playRecords;
		}

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

