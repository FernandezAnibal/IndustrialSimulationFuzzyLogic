using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Se crea una clase para manejar la información de cada tienda como un objeto con propiedades
public class Tiendas  {

	public float id;
	public float demanda;// Demanda va 1 a 10 cada 60 segundos
	public float cDisponible; //1 a 20
	public float prioridad; //0 a 1;

	public Tiendas(float newId, float newDemanda, float newCdisponible, float newPrioridad) {

		id = newId;
		demanda = newDemanda;
		cDisponible = newCdisponible;
		prioridad = newPrioridad;
	}

}
