using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using cn.sharesdk.unity3d;
	using UnityEngine.UI;
	using System.IO;
	using System;

	public enum ShareType
	{
		Weibo,
		WeChat
	}


	public class ShareViewController : ZoomHUD
	{

		public Text learnedDaysText;

		public Text learnedWordCountText;

		public Text correctPercentageText;

		public Text exploredLevelCountText;

		public Button shareButton;
		public Text shareTo;

		private ShareType shareType;

		//定义分享对象
		private ShareSDK ssdk;

		//public TintHUD tintHUD;

		public Transform shareShotcutRect;

		private CallBack shareSucceedCallBack;

		private CallBack shareFailedCallBack;

		private CallBack quitShareCallBack;

		// Use this for initialization
		void Start()
		{
			ssdk = Camera.main.GetComponent<ShareSDK>();
			//处理回调函数
			ssdk.shareHandler = ShareResultHandler;

		}
        
		/// <summary>
		/// 初始化分享界面
		/// </summary>
		public void SetUpShareView(ShareType shareType, CallBack shareSucceedCallBack, CallBack shareFailedCallBack,CallBack quitShareCallBack)
		{
			if(ExploreManager.Instance != null){
				ExploreManager.Instance.UpdateWordDataBase();
			}
         
			this.shareType = shareType;

			this.shareSucceedCallBack = shareSucceedCallBack;

			this.shareFailedCallBack = shareFailedCallBack;

			this.quitShareCallBack = quitShareCallBack;

			DateTime now = DateTime.Now;

			DateTime installDate = Convert.ToDateTime(GameManager.Instance.gameDataCenter.gameSettings.installDateString);

			TimeSpan timeSpan = now.Subtract(installDate);

			//Debug.LogFormat("安装日期：{0}", installDate);
			//Debug.LogFormat("当前日期：{0}", now);

			learnedDaysText.text = string.Format("<size=60>{0}</size>  天", timeSpan.Days + 1);

			LearningInfo learningInfo = LearningInfo.Instance;

			int learnedWordCountOfCurrentType = learningInfo.learnedWordCount;

			int wrongWordCountOfCurrentType = learningInfo.ungraspedWordCount;

			learnedWordCountText.text = string.Format("<size=60>{0}</size>  个", learnedWordCountOfCurrentType);

			int correctPercentageMultiply100 = learnedWordCountOfCurrentType == 0 ? 0 : (learnedWordCountOfCurrentType - wrongWordCountOfCurrentType) * 100 / learnedWordCountOfCurrentType;

			correctPercentageText.text = string.Format("<size=60>{0}</size>  %", correctPercentageMultiply100);

			exploredLevelCountText.text = string.Format("<size=60>{0}</size>  层", Player.mainPlayer.maxUnlockLevelIndex);

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

			if (zoomCoroutine != null)
			{
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);
		}

		public void OnShareButtonClick()
		{

			shareButton.enabled = false;

			StartCoroutine("TrimScreenShotAndShare");
		}


		//分享的回调函数
		void ShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
		{

			shareButton.enabled = true;

			if (state == ResponseState.Success)
			{
				
				if (shareSucceedCallBack != null)
				{
					shareSucceedCallBack();
				}

				DataHandler.DeleteFile(Application.persistentDataPath + "/tempPics/shareImage.png");
			}
			else if (state == ResponseState.Fail)
			{       
				

				if (shareFailedCallBack != null)
				{
					shareFailedCallBack();
				}

				DataHandler.DeleteFile(Application.persistentDataPath + "/tempPics/shareImage.png");
			}
			else if (state == ResponseState.Cancel)
			{            
				DataHandler.DeleteFile(Application.persistentDataPath + "/tempPics/shareImage.png");
			}
		}


		private IEnumerator TrimScreenShotAndShare()
		{

			yield return new WaitForEndOfFrame();

			Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();

			Debug.LogFormat("截屏图片大小[{0},{1}]", texture.width, texture.height);

			int shareHUDWidth = (int)((shareShotcutRect.transform as RectTransform).rect.width / CommonData.scalerToPresetResulotion);
			int shareHUDHeight = (int)((shareShotcutRect.transform as RectTransform).rect.height / CommonData.scalerToPresetResulotion);

			int offsetYFix = (int)shareShotcutRect.localPosition.y;

			int offsetX = (texture.width - shareHUDWidth) / 2;
			int offsetYMin = (texture.height - shareHUDHeight) / 2 + offsetYFix;
			int offsetYMax = (texture.height + shareHUDHeight) / 2 + offsetYFix;

			Texture2D newT2d = new Texture2D(shareHUDWidth, shareHUDHeight);

			for (int i = offsetX; i < texture.width - offsetX; i++)
			{
				for (int j = offsetYMin; j < offsetYMax; j++)
				{

					Color c = texture.GetPixel(i, j);

					newT2d.SetPixel(i - offsetX, j - offsetYMin, c);

				}
			}

			newT2d.Apply();

			Texture2D resizedT2d = ScaleTexture(newT2d, shareHUDWidth, shareHUDHeight);

			byte[] trimImgData = resizedT2d.EncodeToPNG();

			if (!DataHandler.DirectoryExist(Application.persistentDataPath + "/tempPics"))
			{
				DataHandler.CreateDirectory(Application.persistentDataPath + "/tempPics");
			}

			string trimImgPath = Application.persistentDataPath + "/tempPics/shareImage.png";

			File.WriteAllBytes(trimImgPath, trimImgData);
            
            
#if UNITY_EDITOR
            
#elif UNITY_IOS || UNITY_ANDROID
			Share();
#endif

		}

		private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
		{
			Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

			for (int i = 0; i < result.height; ++i)
			{
				for (int j = 0; j < result.width; ++j)
				{
					Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
					result.SetPixel(j, i, newColor);
				}
			}

			result.Apply();
			return result;
		}


		private void Share()
		{

			string shareImgPath = Application.persistentDataPath + "/tempPics/shareImage.png";

			ShareContent content = new ShareContent();

			switch (shareType)
			{

				case ShareType.WeChat:
                               
					content.SetImagePath(shareImgPath);

					//设置分享的类型
					content.SetShareType(ContentType.Image);

                    //直接分享
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

