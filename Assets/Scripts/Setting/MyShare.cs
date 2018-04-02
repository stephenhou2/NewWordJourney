using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using cn.sharesdk.unity3d;
	using UnityEngine.UI;
	using System.IO;


	public class MyShare : MonoBehaviour {

		private enum ShareType
		{
			Weibo,
			WeChat
		}

	    //定义分享对象
		private ShareSDK ssdk;

		public TintHUD tintHUD;

		public Transform shareHUD;

		public RectTransform shareContainer;

		public Button shareButton;

		private ShareType currentShareType;

		// Use this for initialization
		void Start () {
			ssdk = Camera.main.GetComponent<ShareSDK> ();
	        //处理回调函数
	        ssdk.shareHandler = ShareResultHandler;

		}

	


	    //分享的回调函数
		void ShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	    {
	        if (state == ResponseState.Success)
	        {
				QuitShareHUD ();
				string tintStr = "分享成功，获得水晶x30";
				tintHUD.SetUpTintHUD (tintStr, null);
				DataHandler.DeleteFile (Application.persistentDataPath + "/tempPics/shareImage.png");
	        }else if (state == ResponseState.Fail)
			{
				QuitShareHUD ();
				string tintStr = "打开客户端失败";
				tintHUD.SetUpTintHUD (tintStr, null);
				DataHandler.DeleteFile (Application.persistentDataPath + "/tempPics/shareImage.png");
	        }else if (state == ResponseState.Cancel)
	        {
				QuitShareHUD ();
				DataHandler.DeleteFile (Application.persistentDataPath + "/tempPics/shareImage.png");
	        }
	    }

	    //按钮点击截屏分享到微信
	    public void BtnShareToWechatOnClick(){

			currentShareType = ShareType.WeChat;

			ShowShareHUD ();

	    }

	    //按钮点击分享到微博-截屏的操作可以与微信的合并
	    public void BtnShareToWeiboOnClick()
	    {
			currentShareType = ShareType.Weibo;

			ShowShareHUD ();
	    }

		private void ShowShareHUD(){

			shareButton.gameObject.SetActive (true);

			shareHUD.gameObject.SetActive (true);

		}

		public void QuitShareHUD(){
			shareHUD.gameObject.SetActive (false);
		}


		public void OnShareButtonClick(){

			shareButton.gameObject.SetActive (false);

			StartCoroutine ("TrimScreenShotAndShare");

		}




		private IEnumerator TrimScreenShotAndShare(){

			yield return new WaitForEndOfFrame();

			Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture ();

			QuitShareHUD ();

			Debug.LogFormat ("截屏图片大小[{0},{1}]",texture.width,texture.height);

			int shareHUDWidth = (int)((shareContainer.transform as RectTransform).rect.width / CommonData.scalerToPresetResulotion);
			int shareHUDHeight = (int)((shareContainer.transform as RectTransform).rect.height/CommonData.scalerToPresetResulotion);

			int offsetX = (int)((texture.width - shareHUDWidth) / 2);
			int offsetY = (int)((texture.height - shareHUDHeight) / 2);

			Texture2D newT2d = new Texture2D (shareHUDWidth, shareHUDHeight);

			for (int i = offsetX; i < texture.width - offsetX; i++) {
				for (int j = offsetY; j < texture.height - offsetY; j++) {

					Color c = texture.GetPixel(i,j);

					newT2d.SetPixel (i - offsetX, j - offsetY, c);

				}
			}

			newT2d.Apply ();

			Texture2D resizedT2d = ScaleTexture (newT2d, texture.width, texture.height);

			byte[] trimImgData = resizedT2d.EncodeToPNG ();

			if (!DataHandler.DirectoryExist (Application.persistentDataPath + "/tempPics")) {
				DataHandler.CreateDirectory (Application.persistentDataPath + "/tempPics");
			}

			string trimImgPath = Application.persistentDataPath + "/tempPics/shareImage.png";

			File.WriteAllBytes (trimImgPath, trimImgData);


			#if UNITY_EDITORtemp
			#elif UNITY_IOS || UNITY_ANDROID
			Share ();
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


		private void Share(){

			string shareImgPath = Application.persistentDataPath + "/tempPics/shareImage.png";

			ShareContent content = new ShareContent ();

			switch(currentShareType){

			case ShareType.WeChat:

				content.SetText ("分享的内容");
				content.SetTitle ("分享的标题");
				content.SetAddress ("分享的位置");
				content.SetDesc ("分享的描述");
				content.SetComment ("分享的评论");

				content.SetImagePath (shareImgPath);

				//设置分享的类型
				content.SetShareType (ContentType.Auto);

				//直接分享
				ssdk.ShareContent (PlatformType.WeChatMoments, content);

				break;
			case ShareType.Weibo:

				string currentThoughts = "1234566789";
				content.SetText (currentThoughts);
				content.SetImagePath(shareImgPath);

				//设置分享的类型
				content.SetShareType(ContentType.Image);

				//直接分享
				ssdk.ShareContent(PlatformType.SinaWeibo, content);

				break;
			}


		}

		void OnDestroy(){

//			ssdk = null;
//			tintHUD = null;
//			shareHUD = null;
		}

	}
}
