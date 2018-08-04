using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{   
	//public struct CanvasInfo{
	//	public int defaultSortingOrder;
	//	public bool fixSortingOrder;
	//	public CallBack loadCallBack;
	//	public bool keepCanvas;
	//	public string bundleName;
	//	public string canvasName;
	//}

	//public struct CanvasDiplayInfo{
	//	public Transform canvas;
 //       public int defaultSortingOrder;
 //       public bool fixSortingOrder;
	//}

	public class UIManager:MonoBehaviour{

		public Transform canvasContainer;

		public Dictionary<string,Transform> UIDic = new Dictionary<string, Transform> ();

        
		public void SetUpCanvasWith(string bundleName,string canvasName,CallBack cb, bool isSync = false,bool keepBackCanvas = true,bool setVisible = true){

			if (!UIDic.ContainsKey (canvasName)) {
				
				IDictionaryEnumerator dicEnumerator = UIDic.GetEnumerator ();

				if (isSync) {

					GameObject[] assets = MyResourceManager.Instance.LoadAssets<GameObject> (bundleName);

					if (!keepBackCanvas) {
						while (dicEnumerator.MoveNext ()) {
							Canvas canvas = (dicEnumerator.Value as Transform).GetComponent<Canvas> ();
							canvas.enabled = false;
						}
					}

					Canvas c = null;

					foreach (GameObject asset in assets) {
						GameObject obj = GameObject.Instantiate (asset);
						obj.name = asset.name;
						if (obj.name == canvasName) {
							c = obj.GetComponent<Canvas> ();
							if(setVisible){
								c.enabled = true;
							}
						}
					}

					if (cb != null) {
						cb ();
					}

					c.transform.SetParent (canvasContainer);
					c.transform.SetAsLastSibling ();               

					UIDic.Add (canvasName, c.transform);

				} else {

					IEnumerator canvasCoroutine = SetUpCanvasAsync (bundleName, canvasName, cb, keepBackCanvas, setVisible);

					StartCoroutine (canvasCoroutine);

				}

			} else {

				IDictionaryEnumerator dicEnumerator = UIDic.GetEnumerator ();
				
				while (dicEnumerator.MoveNext ()) {

					Canvas c = (dicEnumerator.Value as Transform).GetComponent<Canvas> ();

					if (dicEnumerator.Key as string == canvasName) {
						c.enabled = true;
						c.transform.SetAsLastSibling ();
//						ResetCanvasToSafeArea (c.transform);
					} else {
						c.enabled = c.enabled && keepBackCanvas;
					}
						
				}

				if (cb != null) {
					cb ();
				}

			}

		}
      
		private IEnumerator SetUpCanvasAsync(string bundleName,string canvasName,CallBack cb, bool keepBackCanvas = true,bool setVisible = true){

			AssetBundleRequest requeset = MyResourceManager.Instance.LoadAssetAsync<GameObject> (bundleName);

			yield return requeset;

			Object[] assets = requeset.allAssets;

			IDictionaryEnumerator dicEnumerator = UIDic.GetEnumerator ();

			if (!keepBackCanvas) {
				while (dicEnumerator.MoveNext ()) {
					Canvas canvas = (dicEnumerator.Value as Transform).GetComponent<Canvas> ();
					canvas.enabled = false;
				}
			}

			Canvas c = null;

			foreach (Object asset in assets) {
				GameObject obj = Instantiate (asset as GameObject);
				obj.name = asset.name;
				if (obj.name == canvasName) {
					c = obj.GetComponent<Canvas> ();
					if(setVisible){
						c.enabled = true;
					}
					//if (cb == null) {
					//	c.enabled = true;
					//}
				}
			}

			if (cb != null) {
				cb ();
			}

			c.transform.SetParent(canvasContainer);
			c.transform.SetAsLastSibling ();

			//ResetCanvasesSortingOrder ();

			UIDic.Add (canvasName, c.transform);

		}


			
		//private void ResetCanvasesSortingOrder(){
		//	for (int i = 0; i < canvasContainer.childCount; i++) {
		//		Canvas canvas = canvasContainer.GetChild (i).GetComponent<Canvas>();
		//		canvas.sortingOrder = i;
		//	}
		//}

		public void HideCanvas(string canvasName){

			if(UIDic.ContainsKey(canvasName)){
				UIDic [canvasName].GetComponent<Canvas> ().enabled = false;
			}

		}


		public void RemoveMultiCanvasCache(string[] canvasNames){

			for (int i = 0; i < canvasNames.Length; i++) {
				RemoveCanvasCache (canvasNames [i]);
			}

		}

		public void RemoveCanvasCache(string canvasName){

			if (!UIDic.ContainsKey (canvasName)) {
				return;
			}
	
			switch (canvasName) {
    			case "HomeCanvas":
    				UIDic [canvasName].GetComponent<HomeViewController> ().DestroyInstances ();
    				break;
    			case "BagCanvas":
    				UIDic [canvasName].GetComponent<BagViewController> ().DestroyInstances ();
    				break;
    			case "RecordCanvas":
    				UIDic [canvasName].GetComponent<RecordViewController> ().DestroyInstances ();
    				break;
    			case "SettingCanvas":
    				UIDic [canvasName].GetComponent<SettingViewController> ().DestroyInstances ();
    				break;
    			case "ExploreCanvas":
    				UIDic [canvasName].GetComponent<ExploreUICotroller> ().DestroyInstances ();
    				break;
    			case "GuideCanvas":
    				UIDic[canvasName].GetComponent<GuideViewController>().DestroyInstances();
    				break;
    			case "LoadingCanvas":
    				UIDic[canvasName].GetComponent<LoadingViewController>().DestroyInstances();
    				break;
    			case "NPCCanvas":
    				UIDic[canvasName].GetComponent<NPCViewController>().DestroyInstances();
    				break;
				case "ShareCanvas":
					UIDic[canvasName].GetComponent<ShareViewController>().DestroyInstances();
					break;
				case "FinalChapterCanvas":
					UIDic[canvasName].GetComponent<FinalChapterViewControlller>().DestroyInstances();
					break;

			}

			UIDic.Remove (canvasName);

			Debug.Log ("移除画布" + canvasName);
		}
	}
}
