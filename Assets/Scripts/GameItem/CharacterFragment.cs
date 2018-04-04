using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	#warning 这个类应该没有用了
	// 字母碎片的id=10000
	public class CharacterFragment : Item {

		public char character;

		public CharacterFragment(char character,int count = 1){
			this.itemType = ItemType.CharacterFragment;
			this.itemId = 10000;
			this.character = character;
			itemName = character.ToString();
			spriteName = string.Format ("character_{0}", character);
			itemCount = count;
		}



		public override string GetItemTypeString ()
		{
			return "字母碎片";
		}

		public override string ToString ()
		{
			return string.Format ("[CharacterFragment]");
		}
	}
}
