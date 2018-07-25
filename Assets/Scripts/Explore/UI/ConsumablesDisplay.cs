using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class ConsumablesDisplay : MonoBehaviour {

		public ConsumablesCell[] consCells;

		public Image changeConsumablesIcon;

		private int consumablesDisplayIndex;

		private int consumablesDisplayCount = 5;

		private CallBack refreshPlayerStatusPlane;

		public void InitConsumablesDisplay(CallBack refreshPlayerStatusPlane){
			this.refreshPlayerStatusPlane = refreshPlayerStatusPlane;
		}


		public void SetUpConsumablesButtons(){

			ClearConsumablesButtons ();

			int minIndex = consumablesDisplayCount * consumablesDisplayIndex;

			int validCount = 0;

			int indexInPanel = 0;

			for (int i = 0; i < Player.mainPlayer.allConsumablesInBag.Count; i++) {

				Consumables cons = Player.mainPlayer.allConsumablesInBag [i];

				if (cons.isShowInBagOnly) {
					continue;
				}
					
				if (validCount < minIndex) {
					validCount++;
					continue;
				}

				consCells [indexInPanel].SetUpConsumablesCell (cons, delegate {
					SetUpConsumablesButtons();
					refreshPlayerStatusPlane();

				});

				indexInPanel++;

				if (indexInPanel >= consumablesDisplayCount) {
					return;
				}

			}

			for (int i = 0; i < Player.mainPlayer.allSpecialItemsInBag.Count; i++)
            {

				SpecialItem specialItem = Player.mainPlayer.allSpecialItemsInBag[i];

				if (specialItem.isShowInBagOnly)
                {
                    continue;
                }

                if (validCount < minIndex)
                {
                    validCount++;
                    continue;
                }

				consCells[indexInPanel].SetUpConsumablesCell(specialItem, delegate {
                    SetUpConsumablesButtons();
                    refreshPlayerStatusPlane();

                });

                indexInPanel++;

                if (indexInPanel >= consumablesDisplayCount)
                {
                    return;
                }

            }

		}


		public void ClearConsumablesButtons(){
			for (int i = 0; i < consCells.Length; i++) {
				consCells [i].ClearConsumablesCell ();
			}
		}

		public void ChangeConsumablesDisplayIndex(){

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.bagAudioName);
			
			if (consumablesDisplayIndex == 0) {
				consumablesDisplayIndex = 1;
				changeConsumablesIcon.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 180));
			} else {
				consumablesDisplayIndex = 0;
				changeConsumablesIcon.transform.localRotation = Quaternion.identity;
			}
				
			SetUpConsumablesButtons ();
		}

	}
}
