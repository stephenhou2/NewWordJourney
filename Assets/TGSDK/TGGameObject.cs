using UnityEngine;
using System.Collections;

namespace Together
{
    public class TGGameObject : MonoBehaviour
    {
        private static TGGameObject instance = null;
        public static TGGameObject Instance()
        {
            if (instance == null)
			{
				GameObject go = new GameObject("TGSDK");
                go.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(go);

                instance = go.AddComponent<TGGameObject>();
            }
            return instance;
        }

		void initSDK(string msg)
		{
			if (TGSDK.SDKInitFinishedCallback != null)
				TGSDK.SDKInitFinishedCallback (msg);
		}

		void userPlatformRegisterSuccess(string msg)
        {
			if(TGSDK.PlatformRegisterSuccessCallback != null)
				TGSDK.PlatformRegisterSuccessCallback(msg);
        }

		void userPlatformRegisterFailed(string msg)
		{
			if(TGSDK.PlatformRegisterFailedCallback != null)
				TGSDK.PlatformRegisterFailedCallback(msg);
		}

		void userPlatformLoginSuccess(string msg)
        {
			if (TGSDK.PlatformLoginSuccessCallback != null)
				TGSDK.PlatformLoginSuccessCallback(msg);
        }

		void userPlatformLoginFailed(string msg)
		{
			if (TGSDK.PlatformLoginFailedCallback != null)
				TGSDK.PlatformLoginFailedCallback(msg);
		}

		void userPartnerRegisterSuccess(string msg)
		{
			if (TGSDK.PartnerRegisterSuccessCallback != null)
				TGSDK.PartnerRegisterSuccessCallback(msg);
		}
		
		void userPartnerRegisterFailed(string msg)
		{
			if (TGSDK.PartnerRegisterFailedCallback != null)
				TGSDK.PartnerRegisterFailedCallback(msg);
		}

		void userPartnerLoginSuccess(string msg)
		{
			if (TGSDK.PartnerLoginSuccessCallback != null)
				TGSDK.PartnerLoginSuccessCallback(msg);
		}
		
		void userPartnerLoginFailed(string msg)
		{
			if (TGSDK.PartnerLoginFailedCallback != null)
				TGSDK.PartnerLoginFailedCallback(msg);
		}

		void userPartnerBindSuccess(string msg)
        {
			if (TGSDK.PartnerBindSuccessCallback != null)
				TGSDK.PartnerBindSuccessCallback(msg);
        }

		void userPartnerBindFailed(string msg)
		{
			if (TGSDK.PartnerBindFailedCallback != null)
				TGSDK.PartnerBindFailedCallback(msg);
		}

        void onPreloadSuccess(string msg)
		{
			if (TGSDK.PreloadAdSuccessCallback != null)
				TGSDK.PreloadAdSuccessCallback (msg);
		}

		void onPreloadFailed(string msg)
		{
			if (TGSDK.PreloadAdFailedCallback != null)
				TGSDK.PreloadAdFailedCallback (msg);
		}

		void onCPADLoaded(string msg)
		{
			if (TGSDK.CPAdLoadedCallback != null)
				TGSDK.CPAdLoadedCallback (msg);
		}

		void onVideoADLoaded(string msg)
		{
			if (TGSDK.VideoAdLoadedCallback != null)
				TGSDK.VideoAdLoadedCallback (msg);
		}

		void onADShowSuccess(string msg)
		{
			if (TGSDK.AdShowSuccessCallback != null)
				TGSDK.AdShowSuccessCallback (msg);
		}

		void onADShowFailed(string msg)
		{
			if (TGSDK.AdShowFailedCallback != null)
				TGSDK.AdShowFailedCallback (msg);
		}

		void onADComplete(string msg)
		{
			if (TGSDK.AdCompleteCallback != null)
				TGSDK.AdCompleteCallback (msg);
		}

		void onADClose(string msg)
		{
			if (TGSDK.AdCloseCallback != null)
				TGSDK.AdCloseCallback (msg);
		}

		void onADClick(string msg)
		{
			if (TGSDK.AdClickCallback != null)
				TGSDK.AdClickCallback (msg);
		}

		void onADRewardSuccess(string msg)
		{
			if (TGSDK.AdRewardSuccessCallback != null)
				TGSDK.AdRewardSuccessCallback (msg);
		}

		void onADRewardFailed(string msg)
		{
			if (TGSDK.AdRewardFailedCallback != null)
				TGSDK.AdRewardFailedCallback (msg);
		}

		void onBannerLoaded(string msg)
		{
			if (TGSDK.BannerLoadedCallback != null) {
				string[] args = msg.Split('|');
				TGSDK.BannerLoadedCallback(args[0], args[1]);
			}
		}

		void onBannerFailed(string msg)
		{
			if (TGSDK.BannerFailedCallback != null) {
				string[] args = msg.Split('|');
				TGSDK.BannerFailedCallback(args[0], args[1], args[2]);
			}
		}

		void onBannerClick(string msg) {
			if (TGSDK.BannerClickCallback != null) {
				string[] args = msg.Split('|');
				TGSDK.BannerClickCallback(args[0], args[1]);
			}
		}

		void onBannerClose(string msg) {
			if (TGSDK.BannerCloseCallback != null) {
				string[] args = msg.Split('|');
				TGSDK.BannerCloseCallback(args[0], args[1]);
			}
		}

    }
}