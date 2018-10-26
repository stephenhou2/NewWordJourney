using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

/// <summary>
/// 单例类【非mono】
/// </summary>
public class Singleton<T> where T:class
{
	private static T instance;

	private static object objectLock = new object();

	public static readonly Type[] ms_EmptyTypes = new Type[0];

	public static T Instance{

		get{
			if (instance == null) {
				lock (objectLock)    
				{   
//					if (instance == null)    
//						instance = new T(); 
					ConstructorInfo ci = typeof(T).GetConstructor(
						BindingFlags.NonPublic | BindingFlags.Instance, null, ms_EmptyTypes, null);

					if (ci == null)
					{
						throw new InvalidOperationException("class must contain a private constructor");
					}

					instance = (T)ci.Invoke(null);
				}   
			}
			return instance;
		}
	}

}

/// <summary>
/// 单例类【mono】
/// </summary>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour  
{  
	private static volatile T instance;  
	private static object objectLock = new System.Object();  
	public static T Instance  
	{  
		get  
		{  
			if (instance == null)  
			{  
				lock (objectLock)  
				{  
					T[] instances = FindObjectsOfType<T>();  
					if (instances != null)  
					{  
						for (var i = 0; i < instances.Length; i++)  
						{  
							Destroy(instances[i].gameObject);  
						}  
					}  
					GameObject go = new GameObject();  
					go.name = typeof(T).Name;  
					instance = go.AddComponent<T>(); 
					DontDestroyOnLoad (instance);
				}  
			}  
			return instance;  
		}  
	}  
}  
