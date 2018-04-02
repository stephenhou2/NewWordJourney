using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using cn.sharesdk.unity3d.sdkporter;
using cn.sharesdk.unity3d;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class ShareSDKPostProcessBuild 
{
	//[PostProcessBuild]
	[PostProcessBuildAttribute(88)]
	public static void onPostProcessBuild(BuildTarget target,string targetPath)
	{
		string unityEditorAssetPath = Application.dataPath;

		if (target != BuildTarget.iOS) 
		{
			Debug.LogWarning ("Target is not iPhone. XCodePostProcess will not run");
			return;
		}

		XCProject project = new XCProject (targetPath);
		//var files = System.IO.Directory.GetFiles( unityEditorAssetPath, "*.projmods", System.IO.SearchOption.AllDirectories );
		var files = System.IO.Directory.GetFiles( unityEditorAssetPath + "/ShareSDKiOSAutoPackage/Editor/SDKPorter", "*.projmods", System.IO.SearchOption.AllDirectories);
		foreach( var file in files ) 
		{
			project.ApplyMod( file );
		}

		//如需要预配置Xocode中的URLScheme 和 白名单,请打开下两行代码,并自行配置相关键值
		string projPath = Path.GetFullPath (targetPath);
		EditInfoPlist (projPath);

		//Finally save the xcode project
		project.Save();
	}
	private static void EditInfoPlist(string projPath)
	{

		XCPlist plist = new XCPlist (projPath);
		//URL Scheme 添加
		string PlistAdd = @"  
            <key>CFBundleURLTypes</key>
			<array>
				<dict>
					<key>CFBundleURLSchemes</key>
					<array>
					<string>wx4868b35061f87885</string>
					<string>wb568898243</string>
					</array>
				</dict>
			</array>";

		//白名单添加
		string LSAdd = @"
		<key>LSApplicationQueriesSchemes</key>
			<array>
			<string>weibosdk</string>
			<string>sinaweibohd</string>
			<string>sinaweibo</string>
			<string>sinaweibohdsso</string>
			<string>sinaweibosso</string>
			<string>wechat</string>
			<string>weixin</string>
            <string>weibosdk2.5</string>
		</array>";

        //添加浏览照片资源库的
        string PhotoAdd = @"
        <key>NSPhotoLibraryUsageDescription</key>
        <string>App 需要您的同意才能读取媒体资料库</string>
        ";

        //自定义网络安全策略
        string AppSecurity = @"
        <key>App Transport Security Settings</key>
        <dict>
                <key>Exception Domains</key>
                <dict>
                    <key>weibo.com</key>
                    <dict>
                        <key>NSExceptionMinimumTLSVersion</key>
                        <string>TLSv1.0</string>
                        <key>NSIncludesSubdomains</key>
                        <true/>
                        <key>NSExceptionRequiresForwardSecrecy</key>
                        <false/>
                        <key>NSExceptionAllowsInsecureHTTPLoads</key>
                        <true/>
                    </dict>
                    <key>weibo.cn</key>
                    <dict>
                        <key>NSExceptionMinimumTLSVersion</key>
                        <string>TLSv1.0</string>
                        <key>NSIncludesSubdomains</key>
                        <true/>
                        <key>NSExceptionRequiresForwardSecrecy</key>
                        <false/>
                        <key>NSExceptionAllowsInsecureHTTPLoads</key>
                        <true/>
                    </dict>
                    <key>sina.com.cn</key>
                    <dict>
                        <key>NSExceptionMinimumTLSVersion</key>
                        <string>TLSv1.0</string>
                        <key>NSIncludesSubdomains</key>
                        <true/>
                        <key>NSExceptionRequiresForwardSecrecy</key>
                        <false/>
                        <key>NSExceptionAllowsInsecureHTTPLoads</key>
                        <true/>
                    </dict>
                </dict>
        </dict>

        ";



		//在plist里面增加一行
		plist.AddKey(PlistAdd);
		plist.AddKey (LSAdd);
        plist.AddKey(PhotoAdd);
        plist.AddKey(AppSecurity);




		 ShareSDKConfig theConfig;
		 try
		 {
		 	string filePath = Application.dataPath + "/Plugins/ShareSDK/Editor/ShareSDKConfig.bin";
		 	BinaryFormatter formatter = new BinaryFormatter();
		 	Stream destream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		 	ShareSDKConfig config = (ShareSDKConfig)formatter.Deserialize(destream);
		 	destream.Flush();
		 	destream.Close();
		 	theConfig = config;
		 }
		 catch(Exception)
		 {
		 	theConfig = new ShareSDKConfig ();
		 }
		
		string AppKey = @"<key>MOBAppkey</key> <string>" + theConfig.appKey + "</string>";
		string AppSecret = @"<key>MOBAppSecret</key> <string>" + theConfig.appSecret + "</string>";

		//在plist里面增加一行
		plist.AddKey(AppKey);
		plist.AddKey(AppSecret);

		plist.Save();
	}


}