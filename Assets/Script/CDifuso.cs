using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AForge.Fuzzy;

//La librería usada para ejecutar lógica difusa es Aforge

public class CDifuso : MonoBehaviour {

	//Declaración para mandar información a la conexión con labview
	TCPConnection masterControl;
	public GameObject masterControlO;

	//Variables para leer los sensores
	PosSensor posSensor;
	public GameObject posSensorO;

	//Declaración de tiendas
	public Tiendas regionA ;
	public Tiendas regionB ;
	public Tiendas regionC;

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

	//Sistema de reglas para el Fuzzy
	private InferenceSystem ISL;

	//Recibe señal de la detección de la caja
	public bool A;
	public bool B;
	public bool C;

	//Definición del sistema de lógica difusa
	void InitFuzzyEngine(){

		//Funciones de Pertenencia
		//Demanda
		FuzzySet fsBaja = new FuzzySet ("Baja", new TrapezoidalFunction (2f, 4f, TrapezoidalFunction.EdgeType.Right));
		FuzzySet fsMedia = new FuzzySet ("Media", new TrapezoidalFunction (3f, 5f, 7f));
		FuzzySet fsAlta = new FuzzySet ("Alta", new TrapezoidalFunction (6f,8f, TrapezoidalFunction.EdgeType.Left));
		//Disponible
		FuzzySet fsPoco = new FuzzySet ("Poco", new TrapezoidalFunction (4f, 8f, TrapezoidalFunction.EdgeType.Right));
		FuzzySet fsNormal = new FuzzySet ("Normal", new TrapezoidalFunction (6f, 10f, 14f));
		FuzzySet fsMucho = new FuzzySet ("Mucho", new TrapezoidalFunction (12f, 16f, TrapezoidalFunction.EdgeType.Left));
		//Fuerzas
		FuzzySet fsPbaja = new FuzzySet ("Pbaja", new TrapezoidalFunction ( 0.1f, 0.2f, TrapezoidalFunction.EdgeType.Right));
		FuzzySet fsPmedia = new FuzzySet ("Pmedia", new TrapezoidalFunction ( 0.3f, 0.5f, 0.7f));
		FuzzySet fsPalta = new FuzzySet ("Palta", new TrapezoidalFunction (0.6f, 0.8f, TrapezoidalFunction.EdgeType.Left));


		//Entradas
		//Altura
		LinguisticVariable lvDemanda = new LinguisticVariable ("Demanda", 0, 10);
		lvDemanda.AddLabel (fsBaja);
		lvDemanda.AddLabel (fsMedia);
		lvDemanda.AddLabel (fsAlta);
		//Velocidades
		LinguisticVariable lvDisponible = new LinguisticVariable ("Disponible", 0, 20);
		lvDisponible.AddLabel (fsPoco);
		lvDisponible.AddLabel (fsNormal);
		lvDisponible.AddLabel (fsMucho);
		//Fuerzas
		LinguisticVariable lvFuerza = new LinguisticVariable ("Prioridad", 0, 1);
		lvFuerza.AddLabel (fsPbaja);
		lvFuerza.AddLabel (fsPmedia);
		lvFuerza.AddLabel (fsPalta);

		//La base de datos
		Database fuzzyDb = new Database();
		fuzzyDb.AddVariable (lvDemanda);
		fuzzyDb.AddVariable (lvFuerza);
		fuzzyDb.AddVariable (lvDisponible);

		//Sistema de interferencia


		ISL = new InferenceSystem (fuzzyDb, new CentroidDefuzzifier(800));

		//Definicion de reglas

		ISL.NewRule ("Rule 1", "IF Demanda IS Baja AND Disponible IS Poco THEN Prioridad IS Pmedia" );
		ISL.NewRule ("Rule 2", "IF Demanda IS Baja AND Disponible IS Normal THEN Prioridad IS Pbaja" );
		ISL.NewRule ("Rule 3", "IF Demanda IS Baja AND Disponible IS Mucho THEN Prioridad IS Pbaja" );

		ISL.NewRule ("Rule 4", "IF Demanda IS Media AND Disponible IS Poco THEN Prioridad IS Palta" );
		ISL.NewRule ("Rule 5", "IF Demanda IS Media AND Disponible IS Normal THEN Prioridad IS Pbaja" );
		ISL.NewRule ("Rule 6", "IF Demanda IS Media AND Disponible IS Mucho THEN Prioridad IS Pbaja" );

		ISL.NewRule ("Rule 7", "IF Demanda IS Alta AND Disponible IS Poco THEN Prioridad IS Palta" );
		ISL.NewRule ("Rule 8", "IF Demanda IS Alta AND Disponible IS Normal THEN Prioridad IS Pmedia" );
		ISL.NewRule ("Rule 9", "IF Demanda IS Alta AND Disponible IS Mucho THEN Prioridad IS Pbaja" );


	}


	// Use this for initialization
	void Start () {
		//Inicializacion region A
		regionA = new Tiendas(1,5,10,0);
		//Inicializacion region B
		regionB = new Tiendas(2,5,10,0);
		//Inicializacion region C
		regionC = new Tiendas(3,5,10,0);

		//Asignación de los objetos para su posterior modificación
		masterControl = masterControlO.GetComponent<TCPConnection> ();
		posSensor = posSensorO.GetComponent<PosSensor> ();

	}

		
	void Update () {
		//Ejecuta el motor de lógica difusa

		InitFuzzyEngine ();

		//Evaluacion de la prioridad para cada tienda utilizando una función
		regionA.demanda = demandaA.value;
		regionA.prioridad = EvaluarFuzzy(regionA.demanda, regionA.cDisponible);
		regionB.demanda = demandaB.value;
		regionB.prioridad = EvaluarFuzzy(regionB.demanda, regionB.cDisponible);
		regionC.demanda = demandaC.value;
		regionC.prioridad = EvaluarFuzzy(regionC.demanda, regionC.cDisponible);


		//Función para retornar los pitones en caso de que se encuentren totalmente extendidos
		RetornoDePiston ();
		//Si estan llenas todas las tiendas se envía un mensaje de que todas estan llenas y no se ejecuta ningun movimiento
		//De lo contrario evalua quien tiene mayor prioridad y envía una señal para accionar la valvula
		if (regionA.cDisponible >= cDisponibleA.maxValue && regionB.cDisponible >= cDisponibleB.maxValue && regionC.cDisponible >= cDisponibleC.maxValue  ) {
			Debug.Log ("Estan llenas todas las tiendas");

		} else {

			if (A == true && regionA.prioridad >= regionB.prioridad && regionA.prioridad >= regionC.prioridad && regionA.cDisponible < cDisponibleA.maxValue) {
				masterControl.sensorP [0] = 1; //Señal para accionar la valvula
				A = false;
			}

			if (B == true && regionB.prioridad >= regionC.prioridad && regionB.cDisponible < cDisponibleB.maxValue) {
				masterControl.sensorP [1] = 1; //Señal para accionar la valvula
				B = false;
			}

			if (C == true && regionC.cDisponible < cDisponibleC.maxValue) {
				masterControl.sensorP [2] = 1; //Señal para accionar la valvula
				C = false;
			}


			//Si son iguales entonces se le mandaran paquetes al que tenga menos en almacen
			if (regionA.prioridad == regionB.prioridad && regionC.prioridad == regionB.prioridad) {
				
				if (A == true && regionA.cDisponible < regionB.cDisponible && regionA.cDisponible < regionC.cDisponible && regionA.cDisponible < cDisponibleA.maxValue) {
					masterControl.sensorP [0] = 1; //Señal para accionar la valvula
					A = false;
				}

				if (B == true && regionB.cDisponible < regionC.cDisponible && regionB.cDisponible < cDisponibleB.maxValue) {
					masterControl.sensorP [1] = 1; //Señal para accionar la valvula
					B = false;
				}

				if (C == true && regionC.cDisponible < cDisponibleC.maxValue) {
					masterControl.sensorP [2] = 1; //Señal para accionar la valvula
					C = false;
				}
			
			}
            


		}


		//Asigna la prioridad en pantalla (Cambiar a un script diferente para que este quede lo mas parecido a la implementación del arduino)
		prioridadA.value = regionA.prioridad;
		prioridadB.value = regionB.prioridad;
		prioridadC.value = regionC.prioridad;

		cDisponibleA.value = regionA.cDisponible;
		cDisponibleB.value = regionB.cDisponible;
		cDisponibleC.value = regionC.cDisponible;

	}

	//Función no utilizada para evaluar la lógica difusa
	float EvaluarFuzzy(float eDemanda, float eDisponibles){
		
		ISL.SetInput ("Demanda", eDemanda);
		ISL.SetInput ("Disponible", eDisponibles);
		return ISL.Evaluate("Prioridad");
	}

	//Función para retornar los pistones
	void RetornoDePiston(){

		if (posSensor.a2 == true) {
			masterControl.sensorP [0] = 0;
		}

		if (posSensor.b2 == true) {
			masterControl.sensorP [1] = 0;
		}

		if (posSensor.c2 == true) {
			masterControl.sensorP [2] = 0;
		}

	
	}



}
