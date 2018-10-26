using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Together;
using System;

namespace WordJourney
{

	//枚举广告的类型
	public enum MyAdType
	{
		CPAd,
		RewardedVideoAd
	}

	public delegate void AdCallBack(MyAdType adType);
	//public delegate void AdCallBackFailure(MyAdType adType);

	public class TGController : MonoBehaviour
	{
        
		//广告回到Unity，重新获得焦点后的回调函数
		public static AdCallBack adSuccessCallBack;
		public static AdCallBack adFailureCallBack;

		public static AdCallBack adRewardFailCallBack;

		//判断广告sdk是否已经初始化了
		public static bool _isAdInited = false;
		public static bool IsAdInited
		{
			get { return _isAdInited; }
			private set { _isAdInited = value; }
		}

		//判断cp静态广告是否能用了
		public static bool _isCPAdReady = false;
		public static bool IsCPAdReady
		{
			get {
				return IsAdInited && TGSDK.CouldShowAd(cpAdId);
			}
		}

		//判断视频广告是否能用了
		public static bool _isVideoAdReady = false;
		public static bool IsVideoAdReady
		{
			get { return _isVideoAdReady; }
			private set { _isVideoAdReady = value; }
		}

		//判断激励视频广告是否能用了
		public static bool _isRewardedVideoAdReady = false;
		public static bool IsRewardedVideoAdReady
		{
			get {
				return IsAdInited && TGSDK.CouldShowAd(rewardedVideoId); 
			}
		}


		//判断当前走的是什么广告的回调，根据不同广告给予奖励
		private static MyAdType currentAdType;

		//应用的appid和渠道id
		private static string myAppId = "Y43pIgs8uHf8qh7KvFs2";
		private static string channelId = "10053";//tatap 渠道10053，各个渠道需要添加不同的渠道标识
												  //可以通过网络连接来获取场景的广告
		private static string cpAdId = "kBnbx4WqkCvshcuP1Na";
		//private static string videoId = "Fg3ZXpKOhE4UsZDmNxL";
		private static string rewardedVideoId = "TqqYs3RAxtmhR1XhqPR";

		//实际使用时可以把下面的字符串给注释掉
		//public static string adMsg = "当前的状态是";

		private static bool isRewardVedioComplete;

		private static bool isRewardVedioSuccuss;


		//唤起的时候进行初始化
		private void Awake()
		{
#if UNITY_ANDROID
			InitAdSDK();
#endif

		}

		//初始化整个sdk
		private static void InitAdSDK()
		{
			//判断网络连接的状态
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				Debug.Log("the internet is not reachable");
				//adMsg = "当前网络状态不可用";
				IsAdInited = false;
				return;
			}
			//设置当前为debug模式
			//TGSDK.SetDebugModel(true);
			//初始化结束后的回调
			TGSDK.SDKInitFinishedCallback = (string msg) =>
			{
				//设定gdpr的一些策略
				TGSDK.SetUserGDPRConsentStatus("yes");
				TGSDK.SetIsAgeRestrictedUser("yes");
				Debug.Log(msg);
				Debug.Log("广告的sdk初始化完成了");
				//adMsg = "广告的sdk初始化完成了" + msg;
				IsAdInited = true;

				//广告预加载
				PreloadAd();
			};


#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            TGSDK.Initialize(myAppId,channelId);
        }
        catch (Exception err)
        {
            Debug.Log("链接初始化广告时发生了错误" + err);
            IsAdInited = false;
        }

#endif
		}




		//预加载
		private static void PreloadAd()
		{
			//添加广告预先加载的回调函数
			TGSDK.PreloadAdSuccessCallback = (string msg) =>
			{
				Debug.Log("广告预加载 success的信息是" + msg);
				//adMsg = "PreloadAdSuccessCallback" + msg;
			};

			TGSDK.PreloadAdFailedCallback = (string msg) =>
			{
				Debug.Log("广告预加载失败" + msg);
				//adMsg = "PreloadAdFailedCallback" + msg;
			};

			TGSDK.CPAdLoadedCallback = (string msg) =>
			{
				Debug.Log("静态插屏广告已经加载完毕" + msg);
				//adMsg = "CPAdLoadedCallback" + msg;

			};

			TGSDK.VideoAdLoadedCallback = (string msg) =>
			{
				Debug.Log("视频广告已经加载完毕了" + msg);
				//adMsg = "VideoAdLoadedCallback" + msg;

			};

			//广告播放过程中的回调函数
			TGSDK.AdShowSuccessCallback = (string msg) =>
			{
				Debug.Log("AdShowSuccessCallback : " + msg);
				//adMsg = "AdShowSuccessCallback" + msg;
			};
			TGSDK.AdShowFailedCallback = (string msg) =>
			{
				Debug.Log("AdShowFailedCallback : " + msg);

				isRewardVedioSuccuss = false;
				isRewardVedioComplete = false;

				//展示失败的时候设置回调的函数
				if (adFailureCallBack.Method != null)
				{
					adFailureCallBack(currentAdType);
				}

			};

			TGSDK.AdCompleteCallback = (string msg) =>
			{
				Debug.Log("AdCompleteCallback : " + msg);
 
				isRewardVedioComplete = true;
			};

			TGSDK.AdCloseCallback = (string msg) =>
			{
				Debug.Log("AdCloseCallback : " + msg);

				//根据当前不同的广告类型给与奖励
				if (currentAdType == MyAdType.CPAd)
				{
					//执行插屏广告的奖励回调
					if (adSuccessCallBack.Method != null)
					{
						adSuccessCallBack(currentAdType);
					}

				} else if(currentAdType == MyAdType.RewardedVideoAd){

					if(isRewardVedioSuccuss || isRewardVedioComplete){

						if(adSuccessCallBack != null){
							adSuccessCallBack(currentAdType);
						}

					}else{

						if(adRewardFailCallBack != null){
							adRewardFailCallBack(currentAdType);
						}

					}
    			}                 
			};

			TGSDK.AdClickCallback = (string msg) =>
			{
				Debug.Log("AdClickCallback : " + msg);
			};

			TGSDK.AdRewardSuccessCallback = (string msg) =>
			{
				Debug.Log("AdRewardSuccessCallback : " + msg);

				isRewardVedioSuccuss = true;            
			};

			TGSDK.AdRewardFailedCallback = (string msg) =>
			{
				Debug.Log("AdRewardFailedCallback : " + msg);

				isRewardVedioSuccuss = false;            
			};


			//加载广告
			TGSDK.PreloadAd();
		}

		//额外去加载下一次的广告
		static public void QuickRereshAd()
		{
			if (!IsAdInited)
			{
				//可能因为种种问题没有初始化
				InitAdSDK();
				Debug.Log("在refresh-----------------------这个部分才开始初始化了sdk");
				return;
			}
		}


		//广告进行展示
		static public void ShowAd(MyAdType myAdType,AdCallBack successCallBack,AdCallBack failCallBack, AdCallBack adRewardFailCallBack)
		{

            // 传入当前广告的播放回调
			TGController.adSuccessCallBack = successCallBack;
			TGController.adFailureCallBack = failCallBack;
			TGController.adRewardFailCallBack = adRewardFailCallBack;

			isRewardVedioSuccuss = false;
			isRewardVedioComplete = false;
                     
			string sceneId = "";
			switch (myAdType)
			{
				case MyAdType.CPAd:
					sceneId = cpAdId;
					break;
				case MyAdType.RewardedVideoAd:
					sceneId = rewardedVideoId;
					break;
			}

			Debug.Log(TGSDK.CouldShowAd(sceneId));

            // 如果可以播放广告，则播放广告
			if (TGSDK.CouldShowAd(sceneId))
			{
				currentAdType = myAdType;
				TGSDK.ShowAd(sceneId);
			}
			else
			{
				if(failCallBack != null){
					failCallBack(myAdType);
				}
				Debug.Log(sceneId);
			}


		}


      
	}
}
