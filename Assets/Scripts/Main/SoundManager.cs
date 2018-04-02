using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{


	public class SoundManager : MonoBehaviour {

		private static SoundManager mInstance;
		public static SoundManager Instance{
			get{
				if (mInstance == null) {
					mInstance = TransformManager.FindTransform ("SoundManager").GetComponent<SoundManager> ();
				}
				return mInstance;
			}
		}

	
		public AudioSource bgmAS;

		public AudioSource pronunciationAS;

		public List<AudioSource> audioSourceList = new List<AudioSource> ();

		private Dictionary<string,int> audioClipInfoDic = new Dictionary<string, int> (); 

		public void UpdateVolume(){
			float newVolume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;
			bgmAS.volume = newVolume;
			for (int i = 0; i < audioSourceList.Count; i++) {
				audioSourceList [i].volume = newVolume;
			}
		}

		public void PlayBgmAudioClip(string bgmName,bool isLoop = true){

			string fullPath = "";

			if (!bgmName.StartsWith ("Audio/BGM/")) {
				fullPath = "Audio/BGM/" + bgmName;
			}


			AudioClip bgm = GetAudioClip (fullPath);

			bgmAS.clip = bgm;

			bgmAS.loop = isLoop;

			bgmAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;

			bgmAS.Play ();

		}




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

			int idleASIndex = GetIdleAudioSourceIndex ();

//			audioClipInfoDic.Add (clipName, idleASIndex);

			AudioSource audioSource = audioSourceList [idleASIndex];

			audioSource.clip = clip;

			audioSource.volume = volume == -1 ? GameManager.Instance.gameDataCenter.gameSettings.systemVolume : volume;

			audioSource.Play ();


		}

		public void PauseBgm(){
			bgmAS.Pause ();
		}

		public void ResumeBgm(){
			bgmAS.Play ();
		}

		public void StopBgm(){
			bgmAS.Stop ();
		}

//		public void PlayExploreBackgroundMusic(){
//			bgmAS.clip = exploreBackground;
//			bgmAS.Play ();
//		}
//
//		public void PlayWordPronunciation(AudioClip pronunciation){
//
//			pronunciationAS.clip = pronunciation;
//
//			pronunciationAS.Play ();
//
//		}
//
//		public void PlaySkillEffectClips(string audioClipName){
//
//			AudioClip skillClip = skillEffectAudioClips.Find (delegate(AudioClip obj) {
//				return obj.name == audioClipName;
//			});
//
//
//			if (skillClip == null) {
//				Debug.LogError(string.Format("名字为{0}的音频文件不存在",audioClipName));
//			}
//
//			effectAS.clip = skillClip;
//
//			effectAS.pitch = Random.Range (lowPitchRange, highPitchRange);
//
//			effectAS.Play ();
//
//		}
//
//		public void PlayFootStepClips(){
//
//			AudioClip footStepClip = RandomAudioClip (footStepAudioClips);
//
//			footSoundAS.clip = footStepClip;
//
//			footSoundAS.pitch = Random.Range (lowPitchRange, highPitchRange);
//
//			footSoundAS.Play ();
//
//		}
//
//		public void PlayMapEffectClips(string audioClipName){
//
//			AudioClip clip = mapEffectAudioClips.Find (delegate(AudioClip obj) {
//				return obj.name == audioClipName;
//			});
//
//			if (clip == null) {
//				Debug.LogError(string.Format("名字为{0}的音频文件不存在",audioClipName));
//			}
//
//			effectAS.clip = clip;
//
//			effectAS.pitch = Random.Range (lowPitchRange, highPitchRange);
//
//			effectAS.Play ();
//
//
//		}


			
	}
}
