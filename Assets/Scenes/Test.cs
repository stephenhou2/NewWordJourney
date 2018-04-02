using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DragonBones;

public class Test : MonoBehaviour {

	public GameObject go;

	void Start(){

		go.GetComponent<UnityArmatureComponent> ().animation.Play ("attack", 0);

	}
}

