using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumidor : MonoBehaviour {

	CDifuso controlDifuso;

	public GameObject objectControlDifuso;

	private float TimerA;
	private float TimerB;
	private float TimerC;
	// Use this for initialization
	void Start () {

		controlDifuso = objectControlDifuso.GetComponent<CDifuso> ();
	}


	// Update is called once per frame
	void Update () {
		TimerA += Time.deltaTime;
		TimerB += Time.deltaTime;
		TimerC += Time.deltaTime;

		if (TimerA >= (120 / controlDifuso.regionA.demanda) && (controlDifuso.regionA.cDisponible > 0 )) {
			controlDifuso.regionA.cDisponible -= 1;
			TimerA = 0;
		}
		if (TimerB >= (120 / controlDifuso.regionB.demanda) && (controlDifuso.regionB.cDisponible > 0)) {
			controlDifuso.regionB.cDisponible -= 1;
			TimerB = 0;
		}
		if (TimerC >= (120 / controlDifuso.regionC.demanda) && (controlDifuso.regionC.cDisponible > 0)) {
			controlDifuso.regionC.cDisponible -= 1;
			TimerC = 0;
		}
	}


}
