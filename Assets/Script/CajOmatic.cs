using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CajOmatic : MonoBehaviour {

	//Almacenador de cajas
	private GameObject[] almacenCajas;

	//Booleano para activar y desactivar la salida de las cajas
	private bool activar;
	//Boton para activar y desactivar la salida de las cajas
	public Text activarDesactivar;
	//Objeto
	public GameObject caja;

	//Numero de cajas activas
	private int Ncajas;

	//Temporizador
	private float timer;

	//Frecuencia de aparicion de Cajas 
	private float frecuenciaC;

	//Indice de Cajas
	private int indCajas;
	// Use this for initialization
	void Start () {
		//Inicializa las variables 
		//Aparecerá una caja cada 5 segundos
		activar = false;
		activarDesactivar.text = "Activar";
		frecuenciaC = 5f;
		//
		Ncajas =30;

		//Crea y llena el almacen de cajas con Ncajas
		almacenCajas = new GameObject[Ncajas];
		for (int i = 0; i < Ncajas; i++) {
			GameObject obj = (GameObject)Instantiate (caja);
			obj.name = (Mathf.Round(((Random.value+1)*5)*10f)/10f).ToString(); 
			obj.SetActive (false);
			obj.transform.parent = transform;
			almacenCajas [i] = obj;
		}


	}
	
	// Update is called once per frame
	void Update () {

		if (activar) {
			//temporizador para la frecuencia
			timer += Time.deltaTime;
			//si el tiempo es mayor que la frecuencia activa una caja y la pone en posicion de salida
			if (timer > frecuenciaC) {
				almacenCajas [indCajas].transform.position = transform.position;
				almacenCajas [indCajas].SetActive (true);
				timer = 0;
				if (indCajas < almacenCajas.Length - 1) {
					indCajas++;
				} else {
					indCajas = 0;
				}
			}
		}
			
	}

	public void ActivarDesactivar (){
	
		if (!activar) {
			activar = true;
			activarDesactivar.text = "Desactivar";
		} else {
			activar = false;
			activarDesactivar.text = "Activar";
		}

	}
}
