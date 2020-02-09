using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarCaja : MonoBehaviour {




	//Declaración para modificar las variables del controlador difuso
	CDifuso controlDifuso;
	public GameObject goControlDifuso;
	//Declaración para modificar variables directamente a la comunicacion con labview
	TCPConnection conect;
	public GameObject conectO;

	// Use this for initialization
	void Start () {

		controlDifuso = goControlDifuso.GetComponent<CDifuso> ();
		conect = conectO.GetComponent<TCPConnection> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Cada uno de los siguientes codigos dependerá de donde este aplicado el script para mandar el mensaje
	//Si entra en el campo del sensor le dara el valor de true(Enviara el mensaje de tener una caja en frente)
	void OnTriggerEnter(Collider other){

		if(other.tag == "Caja"){
			if (int.Parse (gameObject.name) == 0) {
				controlDifuso.A = true;
				conect.pesoYpos [0] = (other.gameObject.transform.position.x + ((other.transform.localScale.x / 2) + 0.01f))*-1f;
				conect.pesoYpos [1] = float.Parse (other.gameObject.name);
			

			}
			if (int.Parse (gameObject.name) == 1) {
				controlDifuso.B = true;
				conect.pesoYpos [2] = (other.gameObject.transform.position.x + ((other.transform.localScale.x / 2) + 0.01f))*-1f;
				conect.pesoYpos [3] = float.Parse (other.gameObject.name);
			}
			if (int.Parse (gameObject.name) == 2) {
				controlDifuso.C = true;
				conect.pesoYpos [4] = (other.gameObject.transform.position.x + ((other.transform.localScale.x / 2) + 0.01f))*-1f;
				conect.pesoYpos [5] = float.Parse (other.gameObject.name);
			}
		}

	}

	//Si sale del campo del sensor le dara el valor de false (Enviara un mensaje de no tener nada al frente)
	void OnTriggerExit(Collider other){

		if (int.Parse (gameObject.name) == 0) {
			controlDifuso.A = false;
		}
		if (int.Parse (gameObject.name) == 1) {
			controlDifuso.B = false;
		}
		if (int.Parse (gameObject.name) == 2) {
			controlDifuso.C = false;
		}

	}
}
