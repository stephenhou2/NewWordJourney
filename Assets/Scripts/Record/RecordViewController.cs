using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class RecordViewController : MonoBehaviour {


		public RecordView recordView;

		private int currentTabIndex;


		/// <summary>
		/// 初始化学习记录界面
		/// </summary>
		public void SetUpRecordView(){
			currentTabIndex = 0;
			recordView.SetUpRecordView (LearningInfo.Instance);
//			e.PlayAudioClip ("UI/sfx_UI_Click");
//			StartCoroutine ("SetUpViewAfterDataReady");
		}

//		private IEnumerator SetUpViewAfterDataReady(){
//
//			bool dataReady = false;
//
//			while (!dataReady) {
//
//				dataReady = GameManager.Instance.gameDataCenter.CheckDatasReady (new GameDataCenter.GameDataType[] {
//					GameDataCenter.GameDataType.LearnInfo,
//				});
//
//				yield return null;
//
//			}
//
//			recordView.SetUpRecordView (LearningInfo.Instance);
//
//
//		}
			

		public void OnGeneralRecordButtonClick(){
			if (currentTabIndex == 0) {
				return;
			}
			currentTabIndex = 0;
			recordView.SetUpGeneralLearningInfo ();
		}


		public void OnWrongWordsButtonClick(){
			if (currentTabIndex == 1) {
				return;
			}
			currentTabIndex = 1;
			recordView.SetUpAllUngraspedWords ();
		}

		/// <summary>
		/// 退出学习记录界面
		/// </summary>
		public void QuitRecordPlane(){

			GameManager.Instance.pronounceManager.ClearPronunciationCache ();

			recordView.QuitRecordPlane ();

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.homeCanvasBundleName, "HomeCanvas", () => {
				TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
			});

			GameManager.Instance.UIManager.RemoveCanvasCache ("RecordCanvas");

		}

		/// <summary>
		/// 清理内存
		/// </summary>
		public void DestroyInstances(){

//			learnInfo = null;

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.recordCanvasBundleName, true);

			Destroy (this.gameObject,0.3f);

		}

		void OnDestroy(){
//			recordView = null;
		}

	}
}
