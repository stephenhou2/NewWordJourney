using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.UI;

	public class TintHUD : MonoBehaviour {

		public Transform tintTextModel;
		public Transform tintTextContainer;
		public InstancePool tintTextPool;
        
		public Text goldTintText;
		public Image goldIcon;

		private List<Text> singleTextTints = new List<Text>();
        
		public float tintHUDStayDuration;
		public float tintHUDFloatDuration;
		public int singleTintTextFloatSpeed;

		private float goldTintStayDuration = 1f;
		//private IEnumerator tintHUDCoroutine;

		public void SetUpSingleTextTintHUD(string tint){

			//if (tintHUDCoroutine != null) {
			//	StopCoroutine (tintHUDCoroutine);
			//}

			IEnumerator tintHUDCoroutine = SingleTextFloatAndDisappear (tint);

			StartCoroutine (tintHUDCoroutine);
		}

		public void SetUpGoldTintHUD(int goldGain){
   
			//if (tintHUDCoroutine != null) {
			//	StopCoroutine (tintHUDCoroutine);
			//}

			IEnumerator tintHUDCoroutine = GoldTintLatelyDisappear (goldGain);

			StartCoroutine (tintHUDCoroutine);

		}
			
		private IEnumerator GoldTintLatelyDisappear(int goldCount){

			goldIcon.enabled = true;

			goldTintText.text = string.Format("+{0}", goldCount);

			goldTintText.enabled = true;

			yield return new WaitForSeconds(goldTintStayDuration);

			goldIcon.enabled = false;

			goldTintText.text = string.Empty;

			goldTintText.enabled = false;
		}

		public IEnumerator SingleTextFloatAndDisappear(string tint){
			
			Text tintText = tintTextPool.GetInstance<Text>(tintTextModel.gameObject, tintTextContainer);

			singleTextTints.Add(tintText);

			tintText.text = tint;

			tintText.color = Color.white;

			tintText.transform.localPosition = Vector3.zero;

			yield return new WaitForSecondsRealtime(tintHUDStayDuration);
         
			float timer = 0;

			float tintTextAlphaChangeSpeed = 1 / tintHUDFloatDuration;

            while (timer < tintHUDFloatDuration)
            {
				tintText.transform.localPosition = new Vector3(0, singleTintTextFloatSpeed * Time.unscaledDeltaTime, 0) + tintText.transform.localPosition;

				tintText.color = new Color(1, 1, 1, 1 - tintTextAlphaChangeSpeed * timer);

				timer += Time.unscaledDeltaTime;

				yield return null;
            }



			tintTextPool.AddInstanceToPool(tintText.gameObject);  

			singleTextTints.Remove(tintText);

		}

		public void QuitTintHUD(){
			StopAllCoroutines();
			for (int i = 0; i < singleTextTints.Count;i++){
				singleTextTints[i].text = string.Empty;
			}
			goldTintText.text = string.Empty;
			goldIcon.enabled = false;         
		}
	}
}
