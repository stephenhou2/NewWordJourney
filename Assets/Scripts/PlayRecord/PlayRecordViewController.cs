using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public class PlayRecordViewController : MonoBehaviour
	{

		private List<PlayRecord> playRecords;

		private int currentPageIndex;

		public int maxCellCountInOnePage;

		public PlayRecordView playRecordView;

		private int maxPageIndex
		{
			get
			{
				return (playRecords.Count - 1) / maxCellCountInOnePage;
			}
		}

		public void SetUpPlayerRecordView()
		{

			playRecords = GameManager.Instance.gameDataCenter.allPlayRecords;
         
			SortPlayRecordsOrder();

			playRecordView.InitPlayRecordView(playRecords);

			currentPageIndex = 0;
         
			int minCellIndex = maxCellCountInOnePage * currentPageIndex;

			int idealMaxCellIndex = maxCellCountInOnePage * (currentPageIndex + 1) - 1;

			int actualMaxCellIndex = idealMaxCellIndex > playRecords.Count - 1 ? playRecords.Count - 1 : idealMaxCellIndex;

			playRecordView.SetUpPlayerRecordView(currentPageIndex, maxPageIndex, minCellIndex, actualMaxCellIndex);

		}
        
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

		public void OnNextPageButtonClick()
		{

			if (currentPageIndex >= maxPageIndex)
			{
				return;
			}

			currentPageIndex++;

			int minCellIndex = maxCellCountInOnePage * currentPageIndex;

			int idealMaxCellIndex = maxCellCountInOnePage * (currentPageIndex + 1) - 1;

			int actualMaxCellIndex = idealMaxCellIndex > playRecords.Count - 1 ? playRecords.Count - 1 : idealMaxCellIndex;

			playRecordView.SetUpPlayerRecordView(currentPageIndex, maxPageIndex, minCellIndex, actualMaxCellIndex);

      
		}

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

