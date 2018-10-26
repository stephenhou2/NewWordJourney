using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using System;
	using System.Security.Cryptography;
	using System.Text;
    
    /// <summary>
    /// 字符串数据加密处理
    /// </summary>
	public class StringEncryption
    {

		public static bool isEncryptionOn;

		private static string key = "wordcastle2018091600000000000000";

		public static bool Validate(string source){
			return !source.StartsWith("{") && !source.EndsWith("}");
		}

        // 编码
		public static string Encode(string source){

			try{

				RijndaelManaged encoder = new RijndaelManaged();

				byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);

				encoder.Key = keyArray;

				encoder.Mode = CipherMode.ECB;

				encoder.Padding = PaddingMode.PKCS7;

				//生成加密锁  
				ICryptoTransform cTransform = encoder.CreateEncryptor();  
				byte[] _EncryptArray = UTF8Encoding.UTF8.GetBytes(source);  
                byte[] resultArray = cTransform.TransformFinalBlock(_EncryptArray, 0, _EncryptArray.Length);  
				return Convert.ToBase64String(resultArray, 0, resultArray.Length);  

			}catch(Exception e){
				Debug.Log(e);
				return source;
			}
		}

        // 反编码
		public static string Decode(string source){

			try
            {

                RijndaelManaged encoder = new RijndaelManaged();

                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);

                encoder.Key = keyArray;

				encoder.Mode = CipherMode.ECB;

                encoder.Padding = PaddingMode.PKCS7;

                //生成加密锁  
				ICryptoTransform cTransform = encoder.CreateDecryptor();
				byte[] _EncryptArray = Convert.FromBase64String(source);  
                byte[] resultArray = cTransform.TransformFinalBlock(_EncryptArray, 0, _EncryptArray.Length);  
                return UTF8Encoding.UTF8.GetString(resultArray);  

            }
            catch (Exception e)
            {
                Debug.Log(e);
                return source;
            }

		}



    }
}

