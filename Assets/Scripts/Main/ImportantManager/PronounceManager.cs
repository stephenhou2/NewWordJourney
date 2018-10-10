using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{


	public class PronounceManager : MonoBehaviour
	{

		private class Pronunciation
		{
			public HLHWord word;
			public AudioClip pronunciation;

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

		private enum PronounceErrorCode
		{
			ErrorWithURL_1,
			ErrorWithURL_2,
			Unknown
		}

		private WWW pronunciationWWW;

		private string currentConnectingUrl;

		// 下载发音的超时时长
		public float wwwTimeOutInterval = 2f;


		// 读音缓存
		private List<Pronunciation> pronunciationCache;

		private IEnumerator waitDownloadFinishCoroutine;

		private HLHWord wordToPronounce;

		private List<PronounceErrorRecord> errorRecords = new List<PronounceErrorRecord>();


		//private string pronunciationBaseURL;

		void Awake()
		{
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


		public void DownloadPronounceCache(HLHWord word)
		{

			wordToPronounce = word;

			Pronunciation pro = GetPronunciationFromCache(word);

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
							break;
						case PronounceErrorCode.Unknown:
							break;
					}
				}

				if (pronounceUrl != string.Empty)
				{
					try
					{

						if (string.Equals(pronounceUrl, currentConnectingUrl))
						{
							return;
						}

						//CancelInProgressPronounce();

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

			Pronunciation pro = GetPronunciationFromCache(word);

			if (pro == null)
			{


				string pronounceUrl = string.Empty;

				PronounceErrorRecord errorRecord = CheckPronounceErrorExist(word);
				//PronounceErrorRecord errorRecord = new PronounceErrorRecord(wordToPronounce.wordId, PronounceErrorCode.ErrorWithURL_1);

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
							break;
						case PronounceErrorCode.Unknown:
							break;
					}
				}

				if (pronounceUrl != string.Empty)
				{
					try
					{

						if (string.Equals(pronounceUrl, currentConnectingUrl))
						{
							return;
						}

						CancelInProgressPronounce();

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
				GameManager.Instance.soundManager.PlayPronuncitaion(pro.pronunciation, false);
				//Debug.Log("hun cun fa yin");
			}

		}



		public void PronunceWordFromURL(string url)
		{

			try
			{
				if (string.Equals(url, currentConnectingUrl))
				{
					return;
				}

				CancelInProgressPronounce();

				waitDownloadFinishCoroutine = PlayPronunciationWhenFinishDownloading(url);

				StartCoroutine(waitDownloadFinishCoroutine);
			}
			catch (System.Exception e)
			{
				Debug.Log(e);
			}


		}

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

		public void ClearPronunciationCache()
		{
			//for (int i = 0; i < pronunciationCache.Count; i++) {
			//	pronunciationCache [i].Clear ();
			//}
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

						//Debug.LogFormat("缓存完成：{0}", url);

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

		private void AddPronounceCache(Pronunciation pro){

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
						AudioClip pronunciationClip = downloadWww.GetAudioClip(false, false, AudioType.MPEG);

						//Debug.LogFormat("缓存完成：{0}", url);

						if (pronunciationClip == null)
						{
							downloadWww.Dispose();
						}
						else
						{

							Pronunciation pro = new Pronunciation(word, pronunciationClip);

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
