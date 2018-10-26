using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

    /// <summary>
    /// 音频控制器
    /// </summary>
	public class SoundManager : MonoBehaviour {

        // 背景音乐音源
		public AudioSource bgmAS;
        // 发音音源
		public AudioSource pronunciationAS;
		// 所有音效音源列表【不包括背景音乐音源和发音音源，主要用于其他音效的播放】
		public List<AudioSource> audioSourceList = new List<AudioSource> ();

		//private Dictionary<string,int> audioClipInfoDic = new Dictionary<string, int> (); 

        // 更新所有音源的音量
		public void UpdateVolume(){
			float newVolume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;
			bgmAS.volume = newVolume;
			for (int i = 0; i < audioSourceList.Count; i++) {
				audioSourceList [i].volume = newVolume;
			}
		}

        /// <summary>
        /// 播放指定名称的背景音乐
        /// </summary>
        /// <param name="bgmName">名称</param>
        /// <param name="isLoop">是否循环播放</param>
		public void PlayBgmAudioClip(string bgmName,bool isLoop = true){

			string fullPath = "";

            // 检查音频路径，如果只是音频名称的话，补全路径
			if (!bgmName.StartsWith ("Audio/BGM/")) {
				fullPath = "Audio/BGM/" + bgmName;
			}

            // 如果已经在播放这个背景音乐，则直接返回【否则会从头播放】
			if(bgmAS.isPlaying && bgmAS.clip.name == bgmName){
				return;
			}


			AudioClip bgm = GetAudioClip (fullPath);

			Debug.Log(bgm.name);

			bgmAS.clip = bgm;

			bgmAS.loop = isLoop;

			bgmAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume * 0.5f;

			bgmAS.Play ();

		}



        /// <summary>
        /// 获取指定路径的音频clip
        /// </summary>
        /// <returns>The audio clip.</returns>
        /// <param name="path">Path.</param>
		private AudioClip GetAudioClip(string path){

			AudioClip audioClip = Resources.Load (path) as AudioClip;

			return audioClip;

		}


		/// <summary>
		/// 返回首个空闲状态中的音源，如果都在播放，则返回音源列表的首项
		/// </summary>
		/// <returns>The valid audio source index.</returns>
		private int GetIdleAudioSourceIndex(){

			int idleASIndex = 0;

			for (int i = 0; i < audioSourceList.Count; i++) {
				if (!audioSourceList [i].isPlaying) {
					idleASIndex = i;
					break;
				}
			}

			return idleASIndex;

		}

        /// <summary>
        /// 播放发音
        /// </summary>
        /// <param name="clip">Clip.</param>
        /// <param name="isLoop">If set to <c>true</c> is loop.</param>
		public void PlayPronuncitaion(AudioClip clip,bool isLoop = false){

			pronunciationAS.clip = clip;

			pronunciationAS.volume = 1f;

			pronunciationAS.Play ();

		}

		/// <summary>
		/// 播放指定名称的音乐
		/// </summary>
		/// <param name="clipName">音效路径名称，方法内已提前添加了“Audio/”,不需要再加“Audio/”</param>
		/// <param name="isLoop">If set to <c>true</c> is loop.</param>
		/// <param name="volume">音量设置为-1，则使用设置中的音量</param>
		public void PlayAudioClip(string audioPath,bool isLoop = false,float volume = -1f){
			
			string fullPath = "";

			if (!audioPath.StartsWith ("Audio/")) {
				fullPath = "Audio/" + audioPath;
			}

			AudioClip clip = GetAudioClip (fullPath);

			if (clip == null) {
				Debug.LogFormat ("未找到名为{0}的音乐", audioPath);
				return;
			}


            // 获取一个可用的音源
			int idleASIndex = GetIdleAudioSourceIndex ();
            
			AudioSource audioSource = audioSourceList [idleASIndex];

			audioSource.clip = clip;

			audioSource.volume = volume == -1 ? GameManager.Instance.gameDataCenter.gameSettings.systemVolume : volume;

			audioSource.Play ();


		}

        // 暂停背景音乐
		public void PauseBgm(){
			bgmAS.Pause ();
		}

        // 重新开始播放背景音乐
		public void ResumeBgm(){
			bgmAS.Play ();
		}
        // 停止播放背景音乐
		public void StopBgm(){
			bgmAS.Stop ();
		}      
			
	}
}
