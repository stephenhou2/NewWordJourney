using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Together;
using System.Text.RegularExpressions;

public class ExampleScript : MonoBehaviour {
	public InputField sceneId;
    public Text logField;

	private string[] scenes;
	private int sceneIndex = 0;
	private string[] sceneNames;

	void Awake (){ 
        TGSDK.SetDebugModel(true);
		TGSDK.SDKInitFinishedCallback = (string msg) => {
			TGSDK.TagPayingUser(TGPayingUser.TGMediumPaymentUser, "CNY", 0, 0);
			Log ("TGSDK finished : " + msg);
            Debug.Log ("TGSDK GetUserGDPRConsentStatus = " + TGSDK.GetUserGDPRConsentStatus ());
		    TGSDK.SetUserGDPRConsentStatus ("yes");
		    Debug.Log ("TGSDK GetIsAgeRestrictedUser = " + TGSDK.GetIsAgeRestrictedUser ());
		    TGSDK.SetIsAgeRestrictedUser ("no");
            float bannerHeight = (float)(Screen.height) * 0.123f;
			TGSDK.SetBannerConfig("banner0", "TGBannerNormal", 0, Display.main.systemHeight - bannerHeight, Display.main.systemWidth, bannerHeight, 30);
			TGSDK.SetBannerConfig("banner1", "TGBannerNormal", 0, Display.main.systemHeight - 2*bannerHeight, Display.main.systemWidth, bannerHeight, 30);
			TGSDK.SetBannerConfig("banner2", "TGBannerNormal", 0, Display.main.systemHeight - 3*bannerHeight, Display.main.systemWidth, bannerHeight, 30);
		};
#if UNITY_IOS && !UNITY_EDITOR
		TGSDK.Initialize ("hP7287256x5z1572E5n7");
#elif UNITY_ANDROID && !UNITY_EDITOR
		TGSDK.Initialize ("59t5rJH783hEQ3Jd7Zqr");
#endif
	}

    public void Log(string message)
    {
        Debug.Log("[TGSDK-Unity]  "+message);
        if(logField != null)
        {
			if (logField.text.Length > 100) {
				logField.text = message;
			} else {
            	logField.text = logField.text + "\n" + message;
			}
        }
    }

    public void PreloadAd()
    {
		TGSDK.PreloadAdSuccessCallback = (string msg) => {
			Log ("PreloadAdSuccessCallback : " + msg);
            scenes = Regex.Split(msg, ",", RegexOptions.IgnoreCase);
			sceneNames = new string[scenes.Length];
			for (int i = 0; i < scenes.Length; i++) {
                string scene = scenes[i];
				string sceneName = TGSDK.GetSceneNameById(scene);
				sceneNames[i] = sceneName+"("+scene.Substring(0, 4)+")";
            }
			RefreshSceneId();
		};
		TGSDK.PreloadAdFailedCallback = (string msg) => {
			Log ("PreloadAdFailedCallback : " + msg);
		};
		TGSDK.CPAdLoadedCallback = (string msg) => {
			Log ("CPAdLoadedCallback : " + msg);
		};
		TGSDK.VideoAdLoadedCallback = (string msg) => {
			Log ("VideoAdLoadedCallback : " + msg);
		};
		TGSDK.AdShowSuccessCallback = (string msg) => {
			Log ("AdShowSuccessCallback : " + msg);
		};
		TGSDK.AdShowFailedCallback = (string msg) => {
			Log ("AdShowFailedCallback : " + msg);
		};
		TGSDK.AdCompleteCallback = (string msg) => {
			Log ("AdCompleteCallback : " + msg);
		};
		TGSDK.AdCloseCallback = (string msg) => {
			Log ("AdCloseCallback : " + msg);
		};
		TGSDK.AdClickCallback = (string msg) => {
			Log ("AdClickCallback : " + msg);
		};
		TGSDK.AdRewardSuccessCallback = (string msg) => {
			Log ("AdRewardSuccessCallback : " + msg);
		};
		TGSDK.AdRewardFailedCallback = (string msg) => {
			Log ("AdRewardFailedCallback : " + msg);
		};
		TGSDK.BannerLoadedCallback = (string scene, string ad) => {
			Log ("BannerLoadedCallback : "+scene+", "+ad);
		};
		TGSDK.BannerFailedCallback = (string scene, string ad, string err) => {
			Log ("BannerFailedCallback : "+scene+", "+ad+", "+err);
		};
		TGSDK.BannerClickCallback = (string scene, string ad) => {
			Log ("BannerClickCallback : "+scene+", "+ad);
		};
		TGSDK.BannerCloseCallback = (string scene, string ad) => {
			Log ("BannerCloseCallback : "+scene+", "+ad);
		};
        TGSDK.PreloadAd();
    }

	private void RefreshSceneId() {
		if (scenes != null && scenes.Length > 0) {
			sceneId.text = sceneNames [sceneIndex];
		}
	}

	public void LastScene() {
		if (sceneIndex > 0) {
			sceneIndex--;
			RefreshSceneId();
		}
	}

	public void NextScene() {
		if (sceneIndex < scenes.Length-1) {
			sceneIndex++;
			RefreshSceneId();
		}
	}

    public void ShowAd()
    {
		string sceneid = scenes[sceneIndex];
		if (TGSDK.CouldShowAd (sceneid)) {
			TGSDK.ShowAd(sceneid);
		} else {
			Log("Scene "+sceneid+" could not to show");
		}
	}

	public void ShowTestView()
	{
		string sceneid = scenes[sceneIndex];
		TGSDK.ShowTestView (sceneid);
	}

	public void CloseBanner()
	{
		string sceneid = scenes[sceneIndex];
		TGSDK.CloseBanner(sceneid);
	}
}
