using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshValues : MonoBehaviour {



	//Declaración de objetos que definiran la demanda en cada tienda
	public Slider demandaA;
	public Slider demandaB;
	public Slider demandaC;

	//Declaración de objetos que definiran la cantidad de productos en inventario en cada tienda
	public Slider cDisponibleA;
	public Slider cDisponibleB;
	public Slider cDisponibleC;

	//Declaración de objetos que definiran la prioridad de cada tienda
	public Slider prioridadA;
	public Slider prioridadB;
	public Slider prioridadC;

	//Declaración de objetos que definiran la demanda en cada tienda
	public Text textdemandaA;
	public Text textdemandaB;
	public Text textdemandaC;

	//Declaración de objetos que definiran la cantidad de productos en inventario en cada tienda
	public Text textcDisponibleA;
	public Text textcDisponibleB;
	public Text textcDisponibleC;

	//Declaración de objetos que definiran la prioridad de cada tienda
	public Text textprioridadA;
	public Text textprioridadB;
	public Text textprioridadC;

	//Pra reportar resultado
	public Text trackingA;
	private float memoryA;
	public Text trackingB;
	private float memoryB;
	public Text trackingC;
	private float memoryC;
	public float contador;
	// Use this for initialization
	void Start () {
		memoryA = cDisponibleA.value;
		memoryB = cDisponibleB.value;
		memoryC = cDisponibleC.value;
		contador = 0;
	}
	
	// Update is called once per frame
	void Update () {

		textdemandaA.text = demandaA.value.ToString();
		textdemandaB.text = demandaB.value.ToString();
		textdemandaC.text = demandaC.value.ToString();

		textcDisponibleA.text = cDisponibleA.value.ToString();
		textcDisponibleB.text = cDisponibleB.value.ToString();
		textcDisponibleC.text = cDisponibleC.value.ToString();

		textprioridadA.text = prioridadA.value.ToString();
		textprioridadB.text = prioridadB.value.ToString();
		textprioridadC.text = prioridadC.value.ToString();

		if (memoryA != cDisponibleA.value) {
			trackingA.text = trackingA.text + "; " + cDisponibleA.value;
			memoryA = cDisponibleA.value;
			contador++;
		}

		if (memoryB != cDisponibleB.value) {
			trackingB.text = trackingB.text + "; " + cDisponibleB.value;
			memoryB = cDisponibleB.value;
			contador++;
		}

		if (memoryC != cDisponibleC.value) {
			trackingC.text = trackingC.text + "; " + cDisponibleC.value;
			memoryC = cDisponibleC.value;
			contador++;
		}



	}


}
