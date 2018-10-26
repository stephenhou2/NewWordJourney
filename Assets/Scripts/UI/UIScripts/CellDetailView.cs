using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	interface CellDetailViewInterface{

		void SetUpCellDetailView (object data);
	}

	public abstract class CellDetailView : MonoBehaviour,CellDetailViewInterface {
			
		public abstract void SetUpCellDetailView(object data);

	}
}