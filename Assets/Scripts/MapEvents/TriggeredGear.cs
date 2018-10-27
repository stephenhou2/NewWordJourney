using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
    /// 联动触发型地图物品
    /// </summary>
	public abstract class TriggeredGear : MapEvent {

		public abstract void ChangeStatus ();

	}
}
