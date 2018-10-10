

namespace WordJourney
{
	
	[System.Serializable]
    public class ApplicationInfo
    {
		private static ApplicationInfo mInstance;
		public static ApplicationInfo Instance
        {
            get
            {
                if (mInstance == null)
                {
					mInstance = DataHandler.LoadDataToSingleModelWithPath<ApplicationInfo>(CommonData.applicationInfoFilePath);
                }

                return mInstance;
            }
        }


		// 记录当前版本信息,用于版本比对【格式：x.xx  例如：1.01 代表1.01版，  版本更新时版本号需比上一版大】
        public float currentVersion;

        // IOS上记录是否已经同意过隐私策略
        // 该属性暂时没有用处
		public bool agreePrivacyStrategyOnIos;


    }
}

