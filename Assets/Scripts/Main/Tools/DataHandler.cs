using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


namespace WordJourney
{

    /// <summary>
    /// 数据存取助手
    /// </summary>
	public static class DataHandler{

		// 数据转模型
		public static T[] LoadDataToModelsWithPath<T>(string fileName){

			string jsonStr = LoadDataString (fileName);

			bool decode = StringEncryption.isEncryptionOn && !(jsonStr.StartsWith("{") && jsonStr.EndsWith("}"));

            if (decode)
            {
                jsonStr = StringEncryption.Decode(jsonStr);
            }

			T[] dataArray = null;

			//模型转换
			try{
				dataArray = JsonHelper.FromJson<T> (jsonStr);
			}catch(System.Exception e){
				Debug.Log (e.Message);
			}
			return dataArray;
		}

        // 数据转单个模型
		public static T LoadDataToSingleModelWithPath<T>(string fileName){
         
			string jsonStr = LoadDataString (fileName);

			bool decode = StringEncryption.isEncryptionOn && !(jsonStr.StartsWith("{") && jsonStr.EndsWith("}"));

			if (decode)
            {
                jsonStr = StringEncryption.Decode(jsonStr);
            }

			T instance = default(T);

			if (jsonStr == string.Empty) {
				return instance;
			}

			//模型转换
			try{
				instance = JsonUtility.FromJson<T> (jsonStr);
			}catch(System.Exception e){
				Debug.Log (e.Message);
			}
			return instance;

		}


		// 加载指定路径的文件数据
		public static string LoadDataString(string fileName){
			
			StreamReader sr = null;

			if (!File.Exists (fileName)) {
				Debug.Log(string.Format("can not find file {0}",fileName));
				return string.Empty;
			}

			//读取文件
			try{
				sr = File.OpenText (fileName);
				string dataString = sr.ReadToEnd ();
				sr.Dispose();
				return dataString;

			}catch(System.Exception e){
				Debug.Log (e.Message);
				return null;
			}

		}

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="fileName">File name.</param>
		public static void SaveDataString(string data, string fileName){

			StreamWriter sw = new StreamWriter(fileName, false);
         
            //写文件
            try
            {
				sw.Write(data);
                sw.Dispose();

            }
            catch (Exception e)
            {
				Debug.Log(e);
				sw.Dispose();
            }

		}
			
        /// <summary>
        /// 保存模型数据到文件
        /// </summary>
        /// <param name="instance">Instance.</param>
        /// <param name="filePath">File path.</param>
        /// <param name="encrypt">If set to <c>true</c> encrypt.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void SaveInstanceDataToFile<T>(T instance,string filePath,bool encrypt = false){

			StreamWriter sw = new StreamWriter(filePath, false);

			try{

				string stringData = JsonUtility.ToJson(instance);

				if(encrypt && StringEncryption.isEncryptionOn){
					stringData = StringEncryption.Encode(stringData);
				}
            
				sw.Write(stringData);

				sw.Dispose();

			}catch(Exception e){

				sw.Dispose();

				Debug.Log (e);

			}

		}

        /// <summary>
        /// 保存多个模型数据到文件
        /// </summary>
        /// <param name="instances">Instances.</param>
        /// <param name="filePath">File path.</param>
        /// <param name="encrypt">If set to <c>true</c> encrypt.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void SaveInstanceListToFile<T>(List<T> instances,string filePath, bool encrypt = false){

			StreamWriter sw = new StreamWriter(filePath, false);

			try{

				MyArrayJsonUtilityHelper<T> arrayJsonUtilityHelper = new MyArrayJsonUtilityHelper<T>();

				arrayJsonUtilityHelper.Items = instances;

				string stringData = JsonUtility.ToJson(arrayJsonUtilityHelper);            

				if (encrypt && StringEncryption.isEncryptionOn)
                {
                    stringData = StringEncryption.Encode(stringData);
                }

				sw.Write(stringData);

				sw.Dispose();

			}catch(Exception e){

				sw.Dispose();

				Debug.Log (e);

			}


		}
			
        /// <summary>
        /// 辅助转换为jsonutility类可读的json格式
        /// </summary>
		private class MyArrayJsonUtilityHelper<T>{

			public List<T> Items;

		}

        // 检查指定路径的文件是否存在
		public static bool FileExist(string filePath){
			return File.Exists (filePath);
		}

        // 检查指定路径的文件夹是否存在
		public static bool DirectoryExist(string dirPath){
			return Directory.Exists (dirPath);
		}

        // 创建新的文件夹
		public static void CreateDirectory(string dirPath){
			Directory.CreateDirectory (dirPath);
		}

        /// <summary>
        /// 完整复制文件夹
        /// </summary>
        /// <param name="sourcePath">原始路径</param>
        /// <param name="destPath">目标路径</param>
        /// <param name="deleteOriDirectoryIfExist">是否删除原文件夹</param>
		public static void CopyDirectory(string sourcePath,string destPath,bool deleteOriDirectoryIfExist){

			DirectoryInfo destDirectoryInfo = new DirectoryInfo (destPath);

			if (deleteOriDirectoryIfExist && destDirectoryInfo.Exists) {
				DeleteDirectory (destPath);
			}

			if (!destDirectoryInfo.Exists) {
				destDirectoryInfo.Create ();
			}

			DirectoryInfo sourceDirectoryInfo = new DirectoryInfo (sourcePath);

			FileInfo[] fiArray = sourceDirectoryInfo.GetFiles ();

            for (int i = 0; i < fiArray.Length; i++)
            {
                FileInfo fi = fiArray[i];
                string newPath = Path.Combine(destPath, fi.Name);
                if (!fi.Extension.Equals(".meta")) { 
                    fi.CopyTo(newPath);
                }

			}

			DirectoryInfo[] diArray = sourceDirectoryInfo.GetDirectories ();

			for (int i = 0; i < diArray.Length; i++) {
				DirectoryInfo di = diArray [i];
				string newDestPath = Path.Combine (destPath, di.Name);
				CopyDirectory (di.FullName, newDestPath,deleteOriDirectoryIfExist);
			}

		}

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourceFileName">源文件路径</param>
        /// <param name="destFileName">目标路径</param>
		public static void CopyFile(string sourceFileName,string destFileName){

			try{
				if(File.Exists(destFileName)){
					File.Delete(destFileName);
				}
				File.Copy (sourceFileName, destFileName);
			}catch(Exception e){
				Debug.Log(e);
			}

		}
        
        /// <summary>
        /// 删除指定路径的文件夹
        /// </summary>
        /// <param name="directoryPath">Directory path.</param>
		public static void DeleteDirectory(string directoryPath){

			DirectoryInfo di = new DirectoryInfo (directoryPath);

			if (!di.Exists) {
				Debug.LogError (string.Format("{0} doesn't exist!", directoryPath));
			}

			FileInfo[] fiArray = di.GetFiles ();

			for (int i = 0; i < fiArray.Length; i++) {
				FileInfo fi = fiArray [i];
				fi.Delete ();
			}

			DirectoryInfo[] diArray = di.GetDirectories ();

			for (int i = 0; i < diArray.Length; i++) {
				DirectoryInfo subDi = diArray [i];
				DeleteDirectory (subDi.FullName);
			}
		}

        /// <summary>
        /// 删除指定路径的文件
        /// </summary>
        /// <param name="filePath">File path.</param>
		public static void DeleteFile(string filePath){
			if(File.Exists(filePath)){
				try{
					File.Delete (filePath);
				}catch(Exception e){
					Debug.Log (e);
				}
			}
		}

	}
}

