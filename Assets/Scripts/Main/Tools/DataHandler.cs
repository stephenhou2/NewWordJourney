using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


namespace WordJourney
{
	public static class DataHandler{

		// 数据转模型
		public static T[] LoadDataToModelsWithPath<T>(string fileName){

			string jsonStr = LoadDataString (fileName);

			bool decode = !StringEncryption.isEncryptionOn && !(jsonStr.StartsWith("{") && jsonStr.EndsWith("}"));

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

		public static T LoadDataToSingleModelWithPath<T>(string fileName){
         
			string jsonStr = LoadDataString (fileName);

			bool decode = !StringEncryption.isEncryptionOn && !(jsonStr.StartsWith("{") && jsonStr.EndsWith("}"));

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
			

		private class MyArrayJsonUtilityHelper<T>{

			public List<T> Items;

		}

		public static bool FileExist(string filePath){
			return File.Exists (filePath);
		}

		public static bool DirectoryExist(string dirPath){
			return Directory.Exists (dirPath);
		}

		public static void CreateDirectory(string dirPath){
			Directory.CreateDirectory (dirPath);
		}

		public static void  CopyDirectory(string sourcePath,string destPath,bool deleteOriDirectoryIfExist){

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

