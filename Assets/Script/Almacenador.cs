using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Almacenador : MonoBehaviour {

	CDifuso controlDifuso;

	public GameObject objectControlDifuso;


	// Use this for initialization
	void Start () {

		controlDifuso = objectControlDifuso.GetComponent<CDifuso> ();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other){


		if (other.tag == "Caja") {
			
			if (int.Parse (gameObject.name) == 0) {
				if (controlDifuso.regionA.cDisponible < controlDifuso.cDisponibleA.maxValue)
					controlDifuso.regionA.cDisponible += 1;
			}
			if (int.Parse (gameObject.name) == 1) {
				if (controlDifuso.regionB.cDisponible < controlDifuso.cDisponibleB.maxValue)
					controlDifuso.regionB.cDisponible += 1;
			}
			if (int.Parse (gameObject.name) == 2) {
				if (controlDifuso.regionC.cDisponible < controlDifuso.cDisponibleC.maxValue)
					controlDifuso.regionC.cDisponible += 1;
			}

		}
	}
}

