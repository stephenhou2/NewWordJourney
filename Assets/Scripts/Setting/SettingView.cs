using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney{
	public class SettingView : MonoBehaviour {

		public Slider volumeControl;

		public ToggleGroup tg;

		public Transform settingPlane;

		public Toggle cet46;
		public Toggle nmet;
		public Toggle gre;
		public Toggle toefl;

		public Transform queryChangeWordHUD;

		public Image pronounceOnImage;
		public Image pronounceOffImage;

		public void SetUpSettingView(GameSettings settings){

			volumeControl.value = (int)(settings.systemVolume * 100);

			UpdatePronounceControl (settings.isAutoPronounce);

			tg.SetAllTogglesOff ();

			switch (settings.wordType) {
			case WordType.CET46:
				cet46.isOn = true;
				break;
			case WordType.NMET:
				nmet.isOn = true;
				break;
			case WordType.GRE:
				gre.isOn = true;
				break;
			case WordType.TOEFL:
				toefl.isOn = true;
				break;
			}

			GetComponent<Canvas> ().enabled = true;

		}

		public void UpdatePronounceControl(bool enable){

			if (!enable) {
				pronounceOnImage.gameObject.SetActive (false);
				pronounceOffImage.gameObject.SetActive (true);
			} else {
				pronounceOnImage.gameObject.SetActive (true);
				pronounceOffImage.gameObject.SetActive (false);
			}

		}



		public void ShowAlertHUD(){
			queryChangeWordHUD.gameObject.SetActive (true);
		}

		public void QuitAlertHUD(){
			queryChangeWordHUD.gameObject.SetActive (false);
		}


		public void QuitSettingView(){

			GetComponent<Canvas> ().enabled = false;

		}

		void OnDestroy(){
//			queryChangeWordHUD = null;
		}

	}
}
