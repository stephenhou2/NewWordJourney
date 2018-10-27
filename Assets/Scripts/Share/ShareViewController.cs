using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using cn.sharesdk.unity3d;
	using UnityEngine.UI;
	using System.IO;
	using System;

    // 分享类型【目前只有微信朋友圈分享通道】
	public enum ShareType
	{
		Weibo,
		WeChat
	}

    //分享结果
	public enum ShareResult
	{
		Succeed,
		Faild,
		Canceled
	}


    /// <summary>
    /// 分享控制器
    /// </summary>
	public class ShareViewController : ZoomHUD
	{

		public RectTransform sharePlane;

		public Text learnedDaysText;

		public Text learnedWordCountText;

		public Image codeIcon;

		public Text downloadHintText;

		public Sprite iosCodeSprite;

		public Sprite androidCodeSprite;

		public Button shareButton;
		public Text shareTo;

		private ShareType shareType;

		// share sdk
		private ShareSDK ssdk;

        // 分享截图截取的范围
		public Transform shareShotcutRect;
        // 分享成功的回调
		private CallBack shareSucceedCallBack;
        // 分享失败的回调
		private CallBack shareFailedCallBack;
        // 退出分享的回调
		private CallBack quitShareCallBack;
        // 分享结果
		private ShareResult shareResult;

        /// <summary>
        /// 一些初始化设置
        /// </summary>
		void Start()
		{
			ssdk = Camera.main.GetComponent<ShareSDK>();
			//处理回调函数
			ssdk.shareHandler = ShareResultHandler;

			bool ssdkInitialized = ssdk.CheckInitialization();

			if (!ssdkInitialized)
			{
				ssdk.InitializeShareSDK();
			}
		}

		/// <summary>
		/// 初始化分享界面
		/// </summary>
		public void SetUpShareView(ShareType shareType, CallBack shareSucceedCallBack, CallBack shareFailedCallBack, CallBack quitShareCallBack)
		{
			// 更新单词数据库，确保分享数据是正确的
			if (ExploreManager.Instance != null)
			{
				ExploreManager.Instance.UpdateWordDataBase();
			}

            // 初始化分享类型和分享回调
			this.shareType = shareType;

			shareResult = ShareResult.Canceled;

			this.shareSucceedCallBack = shareSucceedCallBack;

			this.shareFailedCallBack = shareFailedCallBack;

			this.quitShareCallBack = quitShareCallBack;

            // 初始化UI
            // 计算学习天数
			DateTime now = DateTime.Now;

			DateTime installDate = Convert.ToDateTime(GameManager.Instance.gameDataCenter.gameSettings.installDateString);

			TimeSpan timeSpan = now.Subtract(installDate);

			learnedDaysText.text = string.Format("<size=60>{0}</size>  天", timeSpan.Days + 1);

			LearningInfo learningInfo = LearningInfo.Instance;

			int learnedWordCountOfCurrentType = learningInfo.learnedWordCount;

			int wrongWordCountOfCurrentType = learningInfo.ungraspedWordCount;

			learnedWordCountText.text = string.Format("<size=60>{0}</size>  个", learnedWordCountOfCurrentType);

			int correctPercentageMultiply100 = learnedWordCountOfCurrentType == 0 ? 0 : (learnedWordCountOfCurrentType - wrongWordCountOfCurrentType) * 100 / learnedWordCountOfCurrentType;

         
			switch (shareType)
			{
				case ShareType.WeChat:
					shareTo.text = "分享到微信";
					break;
				case ShareType.Weibo:
					shareTo.text = "分享到微博";
					break;
			}

			shareButton.enabled = true;

			GetComponent<Canvas>().enabled = true;

#if UNITY_IOS
			codeIcon.sprite = iosCodeSprite;
            downloadHintText.text = "前往AppStore\n进行下载";
#elif UNITY_ANDROID
			codeIcon.sprite = androidCodeSprite;
			downloadHintText.text = "前往TapTap\n进行下载";
#elif UNITY_EDITOR
			UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

                switch (buildTarget) {
                case UnityEditor.BuildTarget.Android:
        			codeIcon.sprite = androidCodeSprite;
                    downloadHintText.text = "前往TapTap\n进行下载";   
        			break;
                case UnityEditor.BuildTarget.iOS:
        			codeIcon.sprite = iosCodeSprite;
                    downloadHintText.text = "前往AppStore\n进行下载";
                    break;
                }
#endif


			if (zoomCoroutine != null)
			{
				StopCoroutine(zoomCoroutine);
			}

            // 
			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);
		}

        /// <summary>
        /// 点击了分享按钮
        /// </summary>
		public void OnShareButtonClick()
		{

			shareButton.enabled = false;

			IEnumerator trimAndShareCoroutine = TrimScreenShotAndShare();
			StartCoroutine(trimAndShareCoroutine);
		}


		//分享的回调函数
		void ShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
		{
                
			shareButton.enabled = true;

			Debug.LogFormat("share result :{0}",state);

			if (state == ResponseState.Success)
			{
				shareResult = ShareResult.Succeed;
			}
			else if (state == ResponseState.Fail)
			{            
				shareResult = ShareResult.Faild;
			}
			else if (state == ResponseState.Cancel)
			{
				shareResult = ShareResult.Canceled;
			}

			DataHandler.DeleteFile(Application.persistentDataPath + "/tempPics/shareImage.jpg");

			QuitShareView();
		}
        
        /// <summary>
        /// 截屏并截取分享部分的图片
        /// </summary>
        /// <returns>The screen shot and share.</returns>
		private IEnumerator TrimScreenShotAndShare()
		{
                 
			yield return new WaitForEndOfFrame();
           
            // 截屏
			Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();

			Debug.LogFormat("截屏图片大小[{0},{1}]", texture.width, texture.height);

			float transferScaler = 1f;

            // 按照分辨率来确认转换系数
			if(Camera.main.pixelWidth < 1080f){
				if(CommonData.HWScalerOfCurrentScreen < 1.7f){
					transferScaler = CommonData.scalerToPresetH;
				}else{
					transferScaler = CommonData.scalerToPresetW;
				}

			}

            // 获取分享区域的大小
			int shareHUDWidth = (int)((shareShotcutRect.transform as RectTransform).rect.width * transferScaler);
			int shareHUDHeight = (int)((shareShotcutRect.transform as RectTransform).rect.height * transferScaler);


			Debug.LogFormat("实际图片大小：[{0},{1}]", shareHUDWidth, shareHUDHeight);

            // 分享区域在y方向的偏移
			int sharePlaneFixY = (int)(sharePlane.localPosition.y * transferScaler);

			Debug.LogFormat("3y:{0}",sharePlane.localPosition.y);

            // 分享区域的X方向offset和Y方向offset
			int offsetYFix = (int)(shareShotcutRect.localPosition.y * transferScaler);         
			int offsetX = (texture.width - shareHUDWidth) / 2;

            // 实际分享区域的最小y值
			int offsetYMin = (texture.height - shareHUDHeight) / 2 + offsetYFix + sharePlaneFixY;
			// 实际分享区域的最大y值
			int offsetYMax = (texture.height + shareHUDHeight) / 2 + offsetYFix + sharePlaneFixY;


			//int offsetYMin = offsetYFix;
			//int offsetYMax = offsetYFix + shareHUDHeight;

			Debug.LogFormat("实际最小y{0},最大y{1}", offsetYMin, offsetYMax);

            // 按照分享图片的大小创建新的空纹理
			Texture2D newT2d = new Texture2D(shareHUDWidth, shareHUDHeight);

            // 像素处理
			for (int i = offsetX; i < texture.width - offsetX; i++)
			{
				for (int j = offsetYMin; j < offsetYMax; j++)
				{

					Color c = texture.GetPixel(i, j);

					newT2d.SetPixel(i - offsetX, j - offsetYMin, c);

				}
			}
         

            // 纹理应用
			newT2d.Apply();

            // 纹理转化为jpg格式二进制数据
			byte[] trimImgData = newT2d.EncodeToJPG();

            // 检查临时分享文件夹是否存在，不存在创建文件夹
			if (!DataHandler.DirectoryExist(Application.persistentDataPath + "/tempPics"))
			{
				DataHandler.CreateDirectory(Application.persistentDataPath + "/tempPics");
			}

            // 临时图片保存位置
			string trimImgPath = Application.persistentDataPath + "/tempPics/shareImage.jpg";

            // 保存图片
			File.WriteAllBytes(trimImgPath, trimImgData);

            // 清理工作
			Destroy(texture);
			Destroy(newT2d);

			texture = null;
			newT2d = null;
			trimImgData = null;

			Resources.UnloadUnusedAssets();
			GC.Collect();

#if UNITY_EDITOR
			//DataHandler.DeleteFile(Application.persistentDataPath + "/tempPics/shareImage.jpg");
			QuitShareView();
#elif UNITY_IOS || UNITY_ANDROID
			Share();
#endif

		}
      
        /// <summary>
        /// 分享逻辑
        /// </summary>
		private void Share()
		{

			string shareImgPath = Application.persistentDataPath + "/tempPics/shareImage.jpg";

			ShareContent content = new ShareContent();

			switch (shareType)
			{

				case ShareType.WeChat:

					content.SetImagePath(shareImgPath);

					// 设置分享的类型
					content.SetShareType(ContentType.Image);

					// 直接分享
					ssdk.ShareContent(PlatformType.WeChatMoments, content);

                    break;
                case ShareType.Weibo:
					
                    content.SetImagePath(shareImgPath);
               
                    //设置分享的类型
                    content.SetShareType(ContentType.Image);

                    //直接分享
                    ssdk.ShareContent(PlatformType.SinaWeibo, content);

                    break;
            }


        }

        /// <summary>
        /// 退出分享界面
        /// </summary>
		public void QuitShareView(){

			if(inZoomingOut){
				return;
			}

			inZoomingOut = true;

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut(delegate {
				GameManager.Instance.UIManager.RemoveCanvasCache("ShareCanvas"); 
				switch(shareResult){
					case ShareResult.Succeed:
						if(shareSucceedCallBack != null){
							shareSucceedCallBack();
						}                  
						break;
					case ShareResult.Faild:
						if(shareFailedCallBack != null){
							shareFailedCallBack();
						}
						break;
					case ShareResult.Canceled:
						//if(shareFailedCallBack != null){
						//	shareFailedCallBack();
						//}
						break;
				}
			});

			StartCoroutine(zoomCoroutine);

			if(quitShareCallBack != null){
				quitShareCallBack();
			}

		}

		public void DestroyInstances(){

			MyResourceManager.Instance.UnloadAssetBundle(CommonData.shareCanvasBundleName, true);

			Destroy(this.gameObject, 0.3f);

		}
       
    }
}

