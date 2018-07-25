using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using cn.sharesdk.unity3d;
	using UnityEngine.UI;
	using System.IO;




	public class MyShare : MonoBehaviour {


	    

		public Transform shareHUD;

		public RectTransform shareContainer;

		public Button shareButton;

		private ShareType currentShareType;
      
	    
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




	}
}
