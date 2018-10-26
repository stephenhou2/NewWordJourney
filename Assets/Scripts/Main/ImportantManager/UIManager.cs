using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{   
	using UnityEngine.UI;

    /// <summary>
    /// UI控制器
    /// </summary>
	public class UIManager:MonoBehaviour{

		public Transform canvasContainer;

		public Dictionary<string,Transform> UIDic = new Dictionary<string, Transform> ();

        /// <summary>
        /// 初始化画布
        /// </summary>
        /// <param name="bundleName">bundle名称</param>
        /// <param name="canvasName">画布名称</param>
        /// <param name="cb">加载完成回调</param>
        /// <param name="isSync">是否同步加载</param>
        /// <param name="keepBackCanvas">背后的画布是否可见</param>
        /// <param name="setVisible">加载完成后是否将该画布直接设为可见</param>
		public void SetUpCanvasWith(string bundleName,string canvasName,CallBack cb, bool isSync = false,bool keepBackCanvas = true,bool setVisible = true){

            // 画布记录中不包括想要初始化的画布
			if (!UIDic.ContainsKey (canvasName)) {
				// 画布字典遍历器
				IDictionaryEnumerator dicEnumerator = UIDic.GetEnumerator ();

                // 同步加载
				if (isSync) {

					// 如果背后的画布不保留，则遍历字典中的所有画布，全部设为不可见
                    if (!keepBackCanvas)
                    {
                        while (dicEnumerator.MoveNext())
                        {
                            Canvas canvas = (dicEnumerator.Value as Transform).GetComponent<Canvas>();
                            canvas.enabled = false;
                        }
                    }

                    // 从bundle中同步加载
					GameObject[] assets = MyResourceManager.Instance.LoadAssets<GameObject> (bundleName);
               
					Canvas c = null;

                    // 从bundle中加载出得资源中找到画布
					foreach (GameObject asset in assets) {
						// 实例化
						GameObject obj = Instantiate (asset);
						obj.name = asset.name;
						if (obj.name == canvasName) {
							c = obj.GetComponent<Canvas> ();
                            // 可见
							if(setVisible){
								c.enabled = true;
							}
						}
					}

                    // 加载完成回调
					if (cb != null) {
						cb ();
					}

                    // 将画布放置到场景中的画布容器下，方便管理
					c.transform.SetParent (canvasContainer);
					c.transform.SetAsLastSibling ();
               
                    // 判断当前屏幕的宽高比，小于1.7的进行高度适配，大于1.7的进行宽度适配
					if(CommonData.HWScalerOfCurrentScreen < 1.7f){
						c.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
					}else{
						c.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
					}

					UIDic.Add (canvasName, c.transform);

				} else {
                    // 异步加载的协程
					IEnumerator canvasCoroutine = SetUpCanvasAsync (bundleName, canvasName, cb, keepBackCanvas, setVisible);

					StartCoroutine (canvasCoroutine);

				}

			} else {
                // 如果字典中已经有该画布的缓存
				IDictionaryEnumerator dicEnumerator = UIDic.GetEnumerator ();
				// 设置其他画布是否可见
				while (dicEnumerator.MoveNext ()) {

					Canvas c = (dicEnumerator.Value as Transform).GetComponent<Canvas> ();

					if (dicEnumerator.Key as string == canvasName) {
						c.enabled = true;
						c.transform.SetAsLastSibling ();
					} else {
						c.enabled = c.enabled && keepBackCanvas;
					}
						
				}

				if (cb != null) {
					cb ();
				}

			}

		}
      
        /// <summary>
        /// 异步加载画布的协程
        /// </summary>
        /// <returns>The up canvas async.</returns>
        /// <param name="bundleName">Bundle name.</param>
        /// <param name="canvasName">Canvas name.</param>
        /// <param name="cb">Cb.</param>
        /// <param name="keepBackCanvas">If set to <c>true</c> keep back canvas.</param>
        /// <param name="setVisible">If set to <c>true</c> set visible.</param>
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
				}
			}

			if (cb != null) {
				cb ();
			}

			c.transform.SetParent(canvasContainer);
			c.transform.SetAsLastSibling ();
                     

			if (CommonData.HWScalerOfCurrentScreen < 1.7f)
            {
                c.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
            }
            else
            {
                c.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
            }

			UIDic.Add (canvasName, c.transform);

		}

        /// <summary>
        /// 隐藏指定名称的画布
        /// </summary>
        /// <param name="canvasName">Canvas name.</param>
		public void HideCanvas(string canvasName){

			if(UIDic.ContainsKey(canvasName)){
				UIDic [canvasName].GetComponent<Canvas> ().enabled = false;
			}

		}


        /// <summary>
        /// 移除指定画布的缓存
        /// </summary>
        /// <param name="canvasNames">Canvas names.</param>
		public void RemoveMultiCanvasCache(string[] canvasNames){

			for (int i = 0; i < canvasNames.Length; i++) {
				RemoveCanvasCache (canvasNames [i]);
			}

		}

        /// <summary>
        /// 移除指定画布的缓存【完全移除 bundle缓存和场景中该画布的游戏体都会完全移除】
        /// </summary>
        /// <param name="canvasName">Canvas name.</param>
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
				case "PlayRecordCanvas":
					UIDic[canvasName].GetComponent<PlayRecordViewController>().DestroyInstances();
					break;
				case "UpdateDataCanvas":
					UIDic[canvasName].GetComponent<UpdateDataViewController>().DestroyInstances();
					break;

			}

			UIDic.Remove (canvasName);

			Debug.Log ("移除画布" + canvasName);
		}
	}
}
