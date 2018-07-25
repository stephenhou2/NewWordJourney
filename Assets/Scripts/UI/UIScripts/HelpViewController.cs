
namespace WordJourney
{
	using UnityEngine.UI;

	public class HelpViewController : ZoomHUD
    {
		public Text helpText_1;
		public Text helpText_2;
		public Text helpText_3;
      
		public Text pageText;

		private int currentHelpPage;
		//private int totalHelpPage = 3;

		public void SetUpHelpView(){

			this.gameObject.SetActive(true);

			currentHelpPage = 0;

			UpdateHelpDisplay();

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);
		}


		public void OnNextPageButtonClick(){

			if(currentHelpPage >= 2){
				return;
			}

			currentHelpPage++;

			UpdateHelpDisplay();
         
		}

		public void OnLastPageButtonClick(){

			if (currentHelpPage <= 0)
            {
                return;
            }

			currentHelpPage--;

            UpdateHelpDisplay();

		}

       
		public void UpdateHelpDisplay(){
			
			pageText.text = string.Format("{0}/3", currentHelpPage + 1);

			switch(currentHelpPage){
				case 0:
					helpText_1.enabled = true;
					helpText_2.enabled = false;
					helpText_3.enabled = false;
					break;
				case 1:
					helpText_1.enabled = false;
					helpText_2.enabled = true;
                    helpText_3.enabled = false;
					break;
				case 2:
					helpText_1.enabled = false;
                    helpText_2.enabled = false;
					helpText_3.enabled = true;
					break;
			}
		}


		public void QuitHelpHUD(){

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut();

			StartCoroutine(zoomCoroutine);

		}

       
    }

}

