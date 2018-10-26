using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

    /// <summary>
    /// 发音控制器类
    /// </summary>
	public class PronounceManager : MonoBehaviour
	{
        // 发音缓存模型
		private class Pronunciation
		{
			// 发音对应的单词对象
			public HLHWord word;
            // 发音音频clip
			public AudioClip pronunciation;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="word">Word.</param>
            /// <param name="pronunciation">Pronunciation.</param>
			public Pronunciation(HLHWord word, AudioClip pronunciation)
			{
				this.word = word;
				this.pronunciation = pronunciation;
			}

			public void Clear()
			{
				word = null;
				pronunciation = null;
			}
		}

        /// <summary>
        /// 发音错误记录类
        /// </summary>
		private struct PronounceErrorRecord
		{
			public int wordId;
			public PronounceManager.PronounceErrorCode errorCode;

			public PronounceErrorRecord(int wordId, PronounceManager.PronounceErrorCode errorCode)
			{
				this.wordId = wordId;
				this.errorCode = errorCode;
			}

		}

        //发音错误类型枚举
		private enum PronounceErrorCode
		{
			ErrorWithURL_1,
			ErrorWithURL_2,
			Unknown
		}

        // 发音网络请求
		private WWW pronunciationWWW;

        // 当前发音网络请求的url
		private string currentConnectingUrl;

		// 下载发音的超时时长
		public float wwwTimeOutInterval = 2f;


		// 读音缓存列表
		private List<Pronunciation> pronunciationCache;

        // 等待当前发音下载完成的协程
		private IEnumerator waitDownloadFinishCoroutine;

        // 发音请求对应的单词
		private HLHWord wordToPronounce;

        // 发音网络请求错误记录列表
		private List<PronounceErrorRecord> errorRecords = new List<PronounceErrorRecord>();


		void Awake()
		{
			// 初始化和重置
			pronunciationCache = new List<Pronunciation>();
			currentConnectingUrl = string.Empty;
		}

		/// <summary>
		/// 从缓存中读取单词发音
		/// </summary>
		/// <returns>The pronunciation from cache.</returns>
		/// <param name="word">Word.</param>
		private Pronunciation GetPronunciationFromCache(HLHWord word)
		{
			Pronunciation pro = pronunciationCache.Find(delegate (Pronunciation obj)
			{
				return obj.word.wordId == word.wordId;
			});
			return pro;
		}




        /// <summary>
        /// 检查请求单词之前的网络下载是否出现过错误
        /// </summary>
        /// <returns>The pronounce error exist.</returns>
        /// <param name="word">Word.</param>
		private PronounceErrorRecord CheckPronounceErrorExist(HLHWord word)
		{

			PronounceErrorRecord errorRecord = new PronounceErrorRecord(-1, PronounceErrorCode.Unknown);


			for (int i = 0; i < errorRecords.Count; i++)
			{

				PronounceErrorRecord tempRecord = errorRecords[i];

				if (word.wordId == errorRecord.wordId)
				{
					errorRecord = tempRecord;
					break;
				}

			}

			return errorRecord;
		}

        /// <summary>
        /// 下载单词发音【主要用于在创建地图时进行发音缓存】
        /// </summary>
        /// <param name="word">Word.</param>
		public void DownloadPronounceCache(HLHWord word)
		{

			wordToPronounce = word;
            // 从发音缓存查询是否有该单词的缓存
			Pronunciation pro = GetPronunciationFromCache(word);

            // 如果没有，则重新下载
			if (pro == null)
			{


				string pronounceUrl = string.Empty;

                // 检查该单词是否有过下载错误
				PronounceErrorRecord errorRecord = CheckPronounceErrorExist(word);

                // 如果没有错误，使用默认下载链接
				if (errorRecord.wordId == -1)
				{
					pronounceUrl = word.pronounciationURL;
				}
                // 如果出现过错误
				else if (errorRecord.wordId >= 0)
				{
					switch (errorRecord.errorCode)
					{
						// 如果默认发音地址出现过错误，则使用备用发音地址
						case PronounceErrorCode.ErrorWithURL_1:
							pronounceUrl = word.backupProuounciationURL;
							break;
						// 如果备用发音地址出现过错误，则使用默认发音地址
						case PronounceErrorCode.ErrorWithURL_2:
							pronounceUrl = word.pronounciationURL;
							break;
						case PronounceErrorCode.Unknown:
							break;
					}
				}

                // 有可用发音链接
				if (pronounceUrl != string.Empty)
				{
					try
					{

						if (string.Equals(pronounceUrl, currentConnectingUrl))
						{
							return;
						}

                        // 开启新的下载协程
						IEnumerator downloadCoroutine = CachePronouciationFinishDownloading(pronounceUrl,word);
						StartCoroutine(downloadCoroutine);
					}
					catch (System.Exception e)
					{
						Debug.Log(e);
					}

				}

			}
		}

		/// <summary>
		/// 如果缓存中有单词发音，则直接发音，如果没有，则下载完成后发音
		/// </summary>
		/// <param name="word">Word.</param>
		public void PronounceWord(HLHWord word)
		{

			wordToPronounce = word;

            // 获取发音缓存
			Pronunciation pro = GetPronunciationFromCache(word);

            // 如果没有缓存数据
			if (pro == null)
			{
            
				string pronounceUrl = string.Empty;

				PronounceErrorRecord errorRecord = CheckPronounceErrorExist(word);
            
				if (errorRecord.wordId == -1)
				{
					pronounceUrl = word.pronounciationURL;
				}
				else if (errorRecord.wordId >= 0)
				{
					switch (errorRecord.errorCode)
					{
						case PronounceErrorCode.ErrorWithURL_1:
							pronounceUrl = word.backupProuounciationURL;
							break;
						case PronounceErrorCode.ErrorWithURL_2:
							pronounceUrl = word.pronounciationURL;
							break;
						case PronounceErrorCode.Unknown:
							break;
					}
				}

                // 如果有有效url
				if (pronounceUrl != string.Empty)
				{
					try
					{
                        // 如果新的url和当前正在请求的url一致时，直接返回【玩家快速多次点击发音按钮，导致多个发音请求时，后面的发音请求直接忽略】
						if (string.Equals(pronounceUrl, currentConnectingUrl))
						{
							return;
						}

                        // 先停止原来发音下载协程【如果上一次发音下载没有完成，传入了新单词的发音请求，则停止前一个发音请求】
						CancelInProgressPronounce();
                        // 等待发音下载完成后发音
						waitDownloadFinishCoroutine = PlayPronunciationWhenFinishDownloading(pronounceUrl);
						StartCoroutine(waitDownloadFinishCoroutine);
					}
					catch (System.Exception e)
					{
						Debug.Log(e);
					}

				}

			}
			else
			{
				// 如果有缓存发音，则直接发音
				GameManager.Instance.soundManager.PlayPronuncitaion(pro.pronunciation, false);
			}

		}
        

        /// <summary>
        /// 通过给定的url直接下载并发音
        /// </summary>
        /// <param name="url">URL.</param>
		public void PronunceWordFromURL(string url)
		{

			try
			{
				// 如果传入的url和当前正在请求的url一样，直接返回
				if (string.Equals(url, currentConnectingUrl))
				{
					return;
				}

                // 停止当前发音请求
				CancelInProgressPronounce();

                // 开启新的下载并在下载完成后发音
				waitDownloadFinishCoroutine = PlayPronunciationWhenFinishDownloading(url);

				StartCoroutine(waitDownloadFinishCoroutine);
			}
			catch (System.Exception e)
			{
				Debug.Log(e);
			}


		}

        /// <summary>
        /// 停止当前发音请求
        /// </summary>
		public void CancelInProgressPronounce()
		{

			if (waitDownloadFinishCoroutine != null)
			{
				StopCoroutine(waitDownloadFinishCoroutine);
			}

			if (pronunciationWWW != null)
			{
				pronunciationWWW.Dispose();
			}

		}

        /// <summary>
        /// 清除发音缓存
        /// </summary>
		public void ClearPronunciationCache()
		{
			pronunciationCache.Clear();
		}

		/// <summary>
		/// 下载读音文件并在下载完成后播放单词读音的协程
		/// </summary>
		/// <returns>The pronunciation when finish downloading.</returns>
		/// <param name="www">Www.</param>
		private IEnumerator PlayPronunciationWhenFinishDownloading(string url)
		{

			float timer = 0;

			using (pronunciationWWW = new WWW(url))
			{

				currentConnectingUrl = url;

				timer += Time.deltaTime;

                // 下载超时退出下载
                if (timer >= wwwTimeOutInterval)
                {
                    pronunciationWWW.Dispose();
                    yield break;
                }

				yield return pronunciationWWW;


				// 出现异常退出下载
				if (!string.IsNullOrEmpty(pronunciationWWW.error))
				{
					Debug.Log(pronunciationWWW.error);
					pronunciationWWW.Dispose();
					yield break;
				}

				if (pronunciationWWW != null && pronunciationWWW.isDone)
				{

					try
					{
						AudioClip pronunciationClip = pronunciationWWW.GetAudioClip(false, false, AudioType.MPEG);
                  
						if (pronunciationClip == null)
						{
							pronunciationWWW.Dispose();
						}
						else
						{

							Pronunciation pro = new Pronunciation(wordToPronounce, pronunciationClip);

							AddPronounceCache(pro);

							GameManager.Instance.soundManager.PlayPronuncitaion(pronunciationClip, false);

							pronunciationWWW.Dispose();
						}
					}
					catch (System.Exception e)
					{

						Debug.Log(e);

						if (pronunciationWWW.url.Equals(wordToPronounce.pronounciationURL))
						{
							PronounceErrorRecord wdErr = new PronounceErrorRecord(wordToPronounce.wordId, PronounceErrorCode.ErrorWithURL_1);
							errorRecords.Add(wdErr);
						}
						else if (pronunciationWWW.url.Equals(wordToPronounce.backupProuounciationURL))
						{
							PronounceErrorRecord wdErr = new PronounceErrorRecord(wordToPronounce.wordId, PronounceErrorCode.ErrorWithURL_2);
							errorRecords.Add(wdErr);
						}

						pronunciationWWW.Dispose();

					}
				}

				currentConnectingUrl = string.Empty;

			}
		}

        /// <summary>
        /// 添加发音缓存
        /// </summary>
        /// <param name="pro">Pro.</param>
		private void AddPronounceCache(Pronunciation pro){

            // 检查是否已经有对应单词的发音缓存
			Pronunciation pronunciation = pronunciationCache.Find(delegate (Pronunciation obj)
			{
				return obj.word.wordId == pro.word.wordId;
			});

			if(pronunciation == null){
				pronunciationCache.Add(pro);
			}

		}

		/// <summary>
		/// 下载读音文件并在下载完成后播放单词读音的协程
		/// </summary>
		/// <returns>The pronunciation when finish downloading.</returns>
		/// <param name="www">Www.</param>
		private IEnumerator CachePronouciationFinishDownloading(string url,HLHWord word)
		{

			float timer = 0;

			using (WWW downloadWww = new WWW(url))
			{

				timer += Time.deltaTime;

                // 下载超时退出下载
                if (timer >= wwwTimeOutInterval)
                {
                    downloadWww.Dispose();
                    yield break;
                }

				yield return downloadWww;


				// 出现异常退出下载
				if (!string.IsNullOrEmpty(downloadWww.error))
				{
					Debug.Log(downloadWww.error);
					downloadWww.Dispose();
					yield break;
				}

				if (downloadWww != null && downloadWww.isDone)
				{

					try
					{
						// 从下载数据中获取发音clip
						AudioClip pronunciationClip = downloadWww.GetAudioClip(false, false, AudioType.MPEG);

						//Debug.LogFormat("缓存完成：{0}", url);

						if (pronunciationClip == null)
						{
							downloadWww.Dispose();
						}
						else
						{
                            // 创建新的发音缓存
							Pronunciation pro = new Pronunciation(word, pronunciationClip);
                            // 添加缓存
							AddPronounceCache(pro);
                            
							downloadWww.Dispose();
						}
					}
					catch (System.Exception e)
					{

						Debug.Log(e);

						if (downloadWww.url.Equals(wordToPronounce.pronounciationURL))
						{
							PronounceErrorRecord wdErr = new PronounceErrorRecord(wordToPronounce.wordId, PronounceErrorCode.ErrorWithURL_1);
							errorRecords.Add(wdErr);
						}
						else if (downloadWww.url.Equals(wordToPronounce.backupProuounciationURL))
						{
							PronounceErrorRecord wdErr = new PronounceErrorRecord(wordToPronounce.wordId, PronounceErrorCode.ErrorWithURL_2);
							errorRecords.Add(wdErr);
						}

						downloadWww.Dispose();

					}
				}
			}
		}

	}

}
