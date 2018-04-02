using System;
using System.Collections;
using UnityEngine;

namespace cn.sharesdk.unity3d
{
		[Serializable]
		public class ShareSDKConfig
		{
				public string appKey;
				public string appSecret;

				public ShareSDKConfig()
				{
                        this.appKey = "23d7f4268f228";
                        this.appSecret = "89b5123d45df123cb09c31b4507e27a8";
				}
		}		
				
}


