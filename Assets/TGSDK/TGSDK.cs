using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Together
{
	public enum TGPayingUser
	{
		TGNonPayingUser,
		TGSmallPaymentUser,
		TGMediumPaymentUser,
		TGLargePaymentUser
	};

    public class TGSDK
    {
#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
        private static extern string _TGSDK_sdkVersion();
		[DllImport("__Internal")]
        private static extern bool _TGSDK_checkSDKVersion(string version);
		[DllImport("__Internal")]
		private static extern void _TGSDK_setDebugModel(bool Debug);
		[DllImport("__Internal")]
		private static extern void _TGSDK_enableTestServer();
		[DllImport("__Internal")]
		private static extern void _TGSDK_setSDKConfig(string key, string val);
		[DllImport("__Internal")]
		private static extern string _TGSDK_getSDKConfig(string key);
		[DllImport("__Internal")]
		private static extern void _TGSDK_initialize(string appid);
		[DllImport("__Internal")]
		private static extern void _TGSDK_initializeWithChannel(string appid, string channelid);
		[DllImport("__Internal")]
		private static extern void _TGSDK_initializeWithInfoPList();
        [DllImport("__Internal")]
        private static extern void _TGSDK_setScreenSize(double width, double height);
        [DllImport("__Internal")]
        private static extern void _TGSDK_userPlatformRegister(string userName, string userPassword);
        [DllImport("__Internal")]
        private static extern void _TGSDK_userPlatformLogin(string userName, string userPassword);
        [DllImport("__Internal")]
        private static extern void _TGSDK_userPartnerRegister(string puid, string partner);
        [DllImport("__Internal")]
        private static extern void _TGSDK_userPartnerLogin(string puid, string partner);
        [DllImport("__Internal")]
        private static extern void _TGSDK_userPartnerBind(string puid, string partner);
		[DllImport("__Internal")]
		private static extern int _TGSDK_isWIFI();
		[DllImport("__Internal")]
		private static extern void _TGSDK_preloadAdOnlyWIFI();
        [DllImport("__Internal")]
        private static extern void _TGSDK_preloadAd();
        [DllImport("__Internal")]
		private static extern void _TGSDK_showAd(string scene);
        [DllImport("__Internal")]
		private static extern void _TGSDK_showAdWithSDK(string scene, string sdk);
		[DllImport("__Internal")]
        private static extern string _TGSDK_getAdStatus(string scene);
		[DllImport("__Internal")]
		private static extern bool _TGSDK_couldShowAd(string scene);
		[DllImport("__Internal")]
		private static extern bool _TGSDK_couldShowAdWithSDK(string scene, string sdk);
		[DllImport("__Internal")]
        private static extern void _TGSDK_showTestView(string scene);
		[DllImport("__Internal")]
		private static extern void _TGSDK_setBannerConfig(string scene, string type, float x, float y, float width, float height, int interval);
		[DllImport("__Internal")]
		private static extern void _TGSDK_closeBanner(string scene);
		[DllImport("__Internal")]
		private static extern void _TGSDK_reportAdRejected(string scene);
		[DllImport("__Internal")]
		private static extern void _TGSDK_showAdScene(string scene);
		[DllImport("__Internal")]
		private static extern string _TGSDK_getCPImagePath(string scene);
		[DllImport("__Internal")]
		private static extern void _TGSDK_showCPView(string scene);
		[DllImport("__Internal")]
		private static extern void _TGSDK_reportCPClick(string scene);
		[DllImport("__Internal")]
		private static extern void _TGSDK_reportCPClose(string scene);
		[DllImport("__Internal")]
		private static extern void _TGSDK_setCustomUserData(string userData);
		[DllImport("__Internal")]
        private static extern string _TGSDK_getStringParameterFromAdScene(string scene, string key, string def);
		[DllImport("__Internal")]
        private static extern int _TGSDK_getIntParameterFromAdScene(string scene, string key, int def);
		[DllImport("__Internal")]
        private static extern float _TGSDK_getFloatParameterFromAdScene(string scene, string key, float def);
		[DllImport("__Internal")]
		private static extern void _TGSDK_sendCounter(string name, string jsondata);
		[DllImport("__Internal")]
		private static extern void _TGSDK_paymentCounter(
			string productId,
			string method,
			string transId,
			string currency,
			float price,
			int quantity,
			float amount,
			int goodsAmount
		);
		[DllImport("__Internal")]
		private static extern void _TGSDK_tagPayingUser(string user, string currency, float currentAmount, float totalAmount);
		[DllImport("__Internal")]
        private static extern string _TGSDK_getUserGDPRConsentStatus();
		[DllImport("__Internal")]
        private static extern void _TGSDK_setUserGDPRConsentStatus(string status);
		[DllImport("__Internal")]
        private static extern string _TGSDK_getIsAgeRestrictedUser();
		[DllImport("__Internal")]
        private static extern void _TGSDK_setIsAgeRestrictedUser(string status);
		[DllImport("__Internal")]
        private static extern string _TGSDK_getSceneNameById(string scene);

#elif UNITY_ANDROID && !UNITY_EDITOR
		private static AndroidJavaClass _jc = null;
		private static AndroidJavaClass jc()
		{
			if (_jc == null) {
				_jc = new AndroidJavaClass("com.soulgame.sgsdk.tgsdklib.unity.TGSDKUnityMethods");
			}
			return _jc;
		}
        private static string _TGSDK_sdkVersion()
        {
		    return jc().CallStatic<string>("sdkVersion");
        }
        private static bool _TGSDK_checkSDKVersion(string version)
        {
			return jc().CallStatic<bool>("checkSDKVersion", version);
        }
		private static void _TGSDK_setDebugModel(bool debug)
		{
			jc().CallStatic("setDebugModel", debug);
		}
		private static void _TGSDK_enableTestServer()
		{
			jc().CallStatic("enableTestServer");
		}
		private static void _TGSDK_setSDKConfig(string key, string val)
		{
			jc().CallStatic("setSDKConfig", key, val);
		}
		private static string _TGSDK_getSDKConfig(string key)
		{
			return jc().CallStatic<string>("getSDKConfig", key);
		}
		private static void _TGSDK_initializeWithChannel(string appid, string channelid)
		{
			jc().CallStatic("initialize", appid, channelid);
		}
		private static void _TGSDK_initialize(string appid)
		{
			jc().CallStatic("initialize", appid);
		}
		private static void _TGSDK_initializeWithInfoPList()
		{
			jc().CallStatic("initialize");
		}
        private static void _TGSDK_setScreenSize(double width, double height) {}
		private static void _TGSDK_userPlatformRegister(string userName, string userPassword) 
		{
			jc().CallStatic("userPlatformRegister", userName, userPassword);
		}
		private static void _TGSDK_userPlatformLogin(string userName, string userPassword) 
		{
			jc().CallStatic("userPlatformLogin", userName, userPassword);
		}
		private static void _TGSDK_userPartnerRegister(string puid, string partner) 
		{
			jc().CallStatic("userPartnerRegister", puid, partner);
		}
		private static void _TGSDK_userPartnerLogin(string puid, string partner) 
		{
			jc().CallStatic("userPlatformLogin", puid, partner);
		}
        private static void _TGSDK_userPartnerBind(string puid, string partner)
        {
			jc().CallStatic("userPartnerBind", puid, partner);
        }
		private static void _TGSDK_preloadAd() 
		{
			jc().CallStatic("preloadAd");
		}
		private static int _TGSDK_isWIFI()
		{
			return jc().CallStatic<int>("isWIFI");
		}
		private static void _TGSDK_preloadAdOnlyWIFI()
		{
			jc().CallStatic("preloadAdOnlyWIFI");
		}
		private static void _TGSDK_showAd(string scene) 
		{
			jc().CallStatic("showAd", scene);
		}
		private static void _TGSDK_showAdWithSDK(string scene, string sdk) 
		{
			jc().CallStatic("showAd", scene, sdk);
		}
		private static void _TGSDK_showTestView(string scene) 
		{
			jc().CallStatic("showTestView", scene);
		}
        private static void _TGSDK_setBannerConfig(string scene, string type, float x, float y, float width, float height, int interval){
            jc().CallStatic("setBannerConfig", scene, type, x, y, width, height, interval);
        }
        private static void _TGSDK_closeBanner(string scene){
            jc().CallStatic("closeBanner", scene);
        }
		private static void _TGSDK_reportAdRejected(string scene)
		{
			jc().CallStatic("reportAdRejected", scene);
		}
		private static void _TGSDK_showAdScene(string scene)
		{
			jc().CallStatic("showAdScene", scene);
		}
        private static string _TGSDK_getAdStatus(string scene)
        {
			return jc().CallStatic<string>("getAdScene", scene);
        }
		private static bool _TGSDK_couldShowAd(string scene)
		{
			return jc().CallStatic<bool>("couldShowAd", scene);
		}
		private static bool _TGSDK_couldShowAdWithSDK(string scene, string sdk)
		{
			return jc().CallStatic<bool>("couldShowAd", scene, sdk);
		}
        private static string _TGSDK_getStringParameterFromAdScene(string scene, string key, string def)
        {
			return jc().CallStatic<string>("getStringParameterFromAdScene", scene, key, def);
        }
        private static int _TGSDK_getIntParameterFromAdScene(string scene, string key, int def)
        {
			return jc().CallStatic<int>("getIntParameterFromAdScene", scene, key, def);
        }
        private static float _TGSDK_getFloatParameterFromAdScene(string scene, string key, float def)
        {
			return jc().CallStatic<float>("getFloatParameterFromAdScene", scene, key, def);
        }
		private static void _TGSDK_sendCounter(string name, string jsondata)
		{
			jc().CallStatic("sendCounter", name, jsondata);
		}
		private static string _TGSDK_getCPImagePath(string scene)
		{
			return jc().CallStatic<string>("getCPImagePath", scene);
		}
		private static void _TGSDK_showCPView(string scene)
		{
			jc().CallStatic("showCPView", scene);
		}
		private static void _TGSDK_reportCPClick(string scene)
		{
			jc().CallStatic("reportCPClick", scene);
		}
		private static void _TGSDK_reportCPClose(string scene)
		{
			jc().CallStatic("reportCPClose", scene);
		}
		private static void _TGSDK_setCustomUserData(string userData)
		{
			jc().CallStatic("setCustomUserData", userData);
		}
		private static void _TGSDK_paymentCounter(
			string productId,
			string method,
			string transId,
			string currency,
			float price,
			int quantity,
			float amount,
			int goodsAmount
		)
		{
			jc().CallStatic("paymentCounter", productId, method, transId, currency, price, quantity, amount, goodsAmount);
		}
		private static void _TGSDK_tagPayingUser(string user, string currency, float currentAmount, float totalAmount)
		{
			jc().CallStatic("tagPayingUser", user, currency, currentAmount, totalAmount);
		}
        private static string _TGSDK_getUserGDPRConsentStatus()
        {
            return jc().CallStatic<string>("getUserGDPRConsentStatus");
        }
        private static void _TGSDK_setUserGDPRConsentStatus(string status)
        {
            jc().CallStatic("setUserGDPRConsentStatus", status);
        }
        private static string _TGSDK_getIsAgeRestrictedUser()
        {
            return jc().CallStatic<string>("getIsAgeRestrictedUser");
        }
        private static void _TGSDK_setIsAgeRestrictedUser(string status)
        {
            jc().CallStatic("setIsAgeRestrictedUser", status);
        }
		private static string _TGSDK_getSceneNameById(string scene)
		{
			return jc().CallStatic<string>("getSceneNameById", scene);
		}
#else
        private static string _TGSDK_sdkVersion() {return null;}
        private static bool _TGSDK_checkSDKVersion(string version) {return false;}
		private static void _TGSDK_setDebugModel (bool debug) {}
		private static void _TGSDK_enableTestServer () {}
		private static void _TGSDK_setSDKConfig (string key, string val) {}
		private static string _TGSDK_getSDKConfig (string key) {
            return "";
        }
		private static void _TGSDK_initialize(string appid) {}
		private static void _TGSDK_initializeWithChannel(string appid, string channelid) {}
		private static void _TGSDK_initializeWithInfoPList() {}
        private static void _TGSDK_setScreenSize(double width, double height) {}
        private static void _TGSDK_userPlatformRegister(string userName, string userPassword) {}
        private static void _TGSDK_userPlatformLogin(string userName, string userPassword) {}
        private static void _TGSDK_userPartnerRegister(string puid, string partner) {}
        private static void _TGSDK_userPartnerLogin(string puid, string partner) {}
        private static void _TGSDK_userPartnerBind(string puid, string partner) {}
		private static int _TGSDK_isWIFI() {
			return 2;
		}
		private static void _TGSDK_preloadAdOnlyWIFI() {
		}
        private static void _TGSDK_preloadAd() {}
        private static void _TGSDK_showAd(string scene) {}
        private static void _TGSDK_showAdWithSDK(string scene, string sdk) {}
        private static void _TGSDK_showTestView(string scene) {}
		private static void _TGSDK_setBannerConfig(string scene, string type, float x, float y, float width, float height, int interval) {}
		private static void _TGSDK_closeBanner(string scene) {}
        private static string _TGSDK_getAdStatus(string scene) {return "";}
		private static bool _TGSDK_couldShowAd(string scene) {return false;}
		private static bool _TGSDK_couldShowAdWithSDK(string scene, string sdk) {return false;}
		private static void _TGSDK_sendCounter(string name, string jsondata) {}
		private static void _TGSDK_reportAdRejected(string scene) {}
		private static void _TGSDK_showAdScene(string scene) {}
		private static string _TGSDK_getCPImagePath(string scene) {return null;}
		private static void _TGSDK_showCPView(string scene) {}
		private static void _TGSDK_reportCPClick(string scene) {}
		private static void _TGSDK_reportCPClose(string scene) {}
		private static void _TGSDK_setCustomUserData(string userData) {}
        private static string _TGSDK_getStringParameterFromAdScene(string scene, string key, string def) { return def; }
        private static int _TGSDK_getIntParameterFromAdScene(string scene, string key, int def) { return def; }
        private static float _TGSDK_getFloatParameterFromAdScene(string scene, string key, float def) { return def; }
		private static void _TGSDK_paymentCounter(
			string productId,
			string method,
			string transId,
			string currency,
			float price,
			int quantity,
			float amount,
			int goodsAmount
		)
		{
		}
		private static void _TGSDK_tagPayingUser(string user, string currency, float currentAmount, float totalAmount) {}
        private static string _TGSDK_getUserGDPRConsentStatus() { return ""; }
        private static void _TGSDK_setUserGDPRConsentStatus(string status) {}
        private static string _TGSDK_getIsAgeRestrictedUser() { return ""; }
        private static void _TGSDK_setIsAgeRestrictedUser(string status) {}
		private static string _TGSDK_getSceneNameById(string scene) { return ""; }
#endif
		public static Action<string> SDKInitFinishedCallback = null;

        public static Action<string> PlatformRegisterSuccessCallback = null;
		public static Action<string> PlatformRegisterFailedCallback = null;

        public static Action<string> PlatformLoginSuccessCallback = null;
		public static Action<string> PlatformLoginFailedCallback = null;

		public static Action<string> PartnerRegisterSuccessCallback = null;
		public static Action<string> PartnerRegisterFailedCallback = null;

		public static Action<string> PartnerLoginSuccessCallback = null;
		public static Action<string> PartnerLoginFailedCallback = null;

        public static Action<string> PartnerBindSuccessCallback = null;
		public static Action<string> PartnerBindFailedCallback = null;

		public static Action<string> PreloadAdSuccessCallback = null;
		public static Action<string> PreloadAdFailedCallback = null;
		public static Action<string> CPAdLoadedCallback = null;
		public static Action<string> VideoAdLoadedCallback = null;

		public static Action<string> AdShowSuccessCallback = null;
		public static Action<string> AdShowFailedCallback = null;
		public static Action<string> AdCompleteCallback = null;
		public static Action<string> AdCloseCallback = null;
		public static Action<string> AdClickCallback = null;

		public static Action<string> AdRewardSuccessCallback = null;
		public static Action<string> AdRewardFailedCallback = null;

		public static Action<string, string> BannerLoadedCallback = null;
		public static Action<string, string, string> BannerFailedCallback = null;
		public static Action<string, string> BannerClickCallback = null;
		public static Action<string, string> BannerCloseCallback = null;

        public static string SDKVersion()
        {
            try {
                return _TGSDK_sdkVersion();
            }
			catch (Exception e) {
				Debug.LogWarning (e);
			}
            return "";
        }
        public static bool CheckSDKVersion(string version)
        {
            try {
                return _TGSDK_checkSDKVersion(version);
            }
			catch (Exception e) {
				Debug.LogWarning (e);
			}
            return false;
        }
		public static void SetDebugModel(bool debug)
		{
			try {
				_TGSDK_setDebugModel(debug);
			}
			catch (Exception e) {
				Debug.LogWarning (e);
			}
		}

		public static void EnableTestServer()
		{
			try {
				_TGSDK_enableTestServer();
			}
			catch (Exception e)
			{
				Debug.LogWarning (e);
			}
		}

        public static void SetSDKConfig(string key, string val)
        {
            try {
                _TGSDK_setSDKConfig(key, val);
            }
            catch (Exception e)
            {
                Debug.LogWarning (e);
            }
        }

        public static string GetSDKConfig(string key)
        {
            try {
                return _TGSDK_getSDKConfig(key);
            }
            catch (Exception e)
            {
                Debug.LogWarning (e);
                return "";
            }
        }

        private static bool init = false;
		public static void Initialize(string appid)
		{
			if (init)
				return;
			TGGameObject.Instance ();
			init = true;
			try {
                _TGSDK_setScreenSize((double)Screen.width, (double)Screen.height);
				_TGSDK_initialize(appid);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
		}
		public static void Initialize(string appid, string channelid)
		{
			if (init)
				return;
			TGGameObject.Instance ();
			init = true;
			try {
                _TGSDK_setScreenSize((double)Screen.width, (double)Screen.height);
				_TGSDK_initializeWithChannel(appid, channelid);
			}
			catch (Exception e) {
				Debug.LogWarning (e);
			}
		}
        public static void Initialize()
        {
            if (init)
                return;
            TGGameObject.Instance ();
            init = true;
            try {
                _TGSDK_setScreenSize((double)Screen.width, (double)Screen.height);
                _TGSDK_initializeWithInfoPList();
            }
            catch (Exception e) {
                Debug.LogWarning (e);
            }
        }

        public static void PlatformRegister(string userName, string userPassword)
        {
            if (!init) return;
            try
            {
                _TGSDK_userPlatformRegister(userName, userPassword);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        public static void PlatformLogin(string userName, string userPassword)
        {
            if (!init) return;
            try
            {
                _TGSDK_userPlatformLogin(userName, userPassword);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        [Obsolete("use PartnerBind")]
        public static void PartnerRegister(string puid, string partner)
        {
            if (!init) return;
            try
            {
                _TGSDK_userPartnerRegister(puid, partner);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        [Obsolete("use PartnerBind")]
        public static void PartnerLogin(string puid, string partner)
        {
            if (!init) return;
            try
            {
                _TGSDK_userPartnerLogin(puid, partner);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
        
        public static void PartnerBind(string puid, string partner)
        {
            if (!init) return;
            try
            {
                _TGSDK_userPartnerBind(puid, partner);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        public static void PreloadAd()
        {
            if (!init) return;
            try
            {
                _TGSDK_preloadAd();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

		public static int IsWIFI()
		{
			if (!init) return -1;
			try
			{
				return _TGSDK_isWIFI();
			}
			catch (Exception e)
			{
				Debug.LogWarning (e);
				return 2;
			}
		}

		public static void PreloadAdOnlyWIFI()
        {
            if (!init) return;
            try
			{
				_TGSDK_preloadAdOnlyWIFI();
			}
			catch (Exception e)
			{
				Debug.LogWarning (e);
			}
		}

        public static void ShowAd(string scene)
        {
            if (!init) return;
            try
            {
                _TGSDK_showAd(scene);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        public static void ShowAd(string scene, string sdk)
        {
            if (!init) return;
            try
            {
                _TGSDK_showAdWithSDK(scene, sdk);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        public static void ShowTestView(string scene)
        {
            if (!init) return;
            try
            {
                _TGSDK_showTestView(scene);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

		public static void SetBannerConfig(string scene, string type, float x, float y, float width, float height, int interval)
		{
			if (!init) return;
			try
			{
				_TGSDK_setBannerConfig(scene, type, x, y, width, height, interval);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
		}

		public static void CloseBanner(string scene)
		{
			if (!init) return;
			try
			{
				_TGSDK_closeBanner(scene);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
		}

        public static string GetAdStatus(string scene)
        {
            if (!init) return "TGSDKNotInitialize";
            try
            {
                return _TGSDK_getAdStatus(scene);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            return "";
        }

		public static bool CouldShowAd(string scene)
		{
			if (!init) return false;
            try
            {
                return _TGSDK_couldShowAd (scene);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            return false;
        }

		public static bool CouldShowAd(string scene, string sdk)
		{
			if (!init) return false;
            try
            {
                return _TGSDK_couldShowAdWithSDK (scene, sdk);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            return false;
        }

		public static void ReportAdRejected(string scene)
        {
            if (!init) return;
            try
            {
                _TGSDK_reportAdRejected(scene);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

		public static void ShowAdScene(string scene)
        {
            if (!init) return;
            try
            {
                _TGSDK_showAdScene (scene);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

		public static void SendCounter(string name, string jsondata)
        {
            if (!init) return;
            try
            {
                _TGSDK_sendCounter (name, jsondata);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

		public static string GetCPImagePath(string scene)
        {
            if (!init) return "";
            try
            {
                return _TGSDK_getCPImagePath (scene);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            return "";
        }

		public static void ShowCPView(string scene)
        {
            if (!init) return;
            try
            {
                _TGSDK_showCPView (scene);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

		public static void ReportCPClick(string scene)
        {
            if (!init) return;
            try
            {
                _TGSDK_reportCPClick (scene);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

		public static void ReportCPClose(string scene)
        {
            if (!init) return;
            try
            {
                _TGSDK_reportCPClose (scene);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

		public static void SetCustomUserData(string userData)
		{
			if (!init) return;
			try
			{
				_TGSDK_setCustomUserData (userData);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}
		}

        public static string GetStringParameterFromAdScene(string scene, string key, string def = "")
        {
            if (!init) return null;
            try
            {
                return _TGSDK_getStringParameterFromAdScene(scene, key, def);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            return def;
        }

        public static int GetIntParameterFromAdScene(string scene, string key, int def = 0)
        {
            if (!init) return 0;
            try
            {
                return _TGSDK_getIntParameterFromAdScene(scene, key, def);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            return def;
        }

        public static float GetFloatParameterFromAdScene(string scene, string key, float def = 0)
        {
            if (!init) return 0;
            try
            {
                return _TGSDK_getFloatParameterFromAdScene(scene, key, def);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            return def;
        }

		public static void PaymentCounter(
			string productId,
			string method,
			string transId,
			string currency,
			float price,
			int quantity,
			float amount,
			int goodsAmount)
        {
            if (!init) return;
            try {
				_TGSDK_paymentCounter(
					productId,
					method,
					transId,
					currency,
					price,
					quantity,
					amount,
					goodsAmount
				);
			} catch (Exception e) {
				Debug.LogWarning (e);
			}
		}

		public static void TagPayingUser(TGPayingUser user, string currency = "", float currentAmount = 0, float totalAmount = 0)
		{
			if (!init)
				return;
			try {
				string payingUser = null;
				switch (user) {
				case TGPayingUser.TGNonPayingUser:
					payingUser = "none";
					break;
				case TGPayingUser.TGSmallPaymentUser:
					payingUser = "small";
					break;
				case TGPayingUser.TGMediumPaymentUser:
					payingUser = "medium";
					break;
				case TGPayingUser.TGLargePaymentUser:
					payingUser = "large";
					break;
				default:
					payingUser = "";
					break;
				}
				_TGSDK_tagPayingUser(payingUser, currency, currentAmount, totalAmount);
			} catch (Exception e) {
				Debug.LogWarning (e);
			}
		}

        public static string GetUserGDPRConsentStatus()
        {
            try {
                return _TGSDK_getUserGDPRConsentStatus();
			} catch (Exception e) {
				Debug.LogWarning (e);
                return "";
			}
        }

        public static void SetUserGDPRConsentStatus(string status)
        {
            try {
                _TGSDK_setUserGDPRConsentStatus(status);
			} catch (Exception e) {
				Debug.LogWarning (e);
            }
        }

        public static string GetIsAgeRestrictedUser()
        {
            try {
                return _TGSDK_getIsAgeRestrictedUser();
			} catch (Exception e) {
				Debug.LogWarning (e);
                return "";
			}
        }

        public static void SetIsAgeRestrictedUser(string status)
        {
            try {
                _TGSDK_setIsAgeRestrictedUser(status);
			} catch (Exception e) {
				Debug.LogWarning (e);
            }
        }

		public static string GetSceneNameById(string scene)
		{
			try {
				return _TGSDK_getSceneNameById(scene);
			} catch (Exception e) {
				Debug.LogWarning (e);
				return "";
			}
		}
    }
}
