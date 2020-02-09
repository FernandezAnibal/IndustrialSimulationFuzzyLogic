using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AForge.Fuzzy;

public class FuzzyTest : MonoBehaviour {

	//Para controlar este rigidbody
	Rigidbody thebox;
	public Slider SlideAltura;
	//private InferenceSystem ISL;

	private float fuerza;
	private float deltaAltura;
	private float velocidad;

	InferenceSystem ISL;

	void InitFuzzyEngine(){

		//Sunciones de Pertenencia
		//Alturas
		FuzzySet fsBaja = new FuzzySet ("Baja", new TrapezoidalFunction (-1f, 0f, TrapezoidalFunction.EdgeType.Right));
		FuzzySet fsMedia = new FuzzySet ("Media", new TrapezoidalFunction (-1f, -0.1f, 0.1f, 1f));
		FuzzySet fsAlta = new FuzzySet ("Alta", new TrapezoidalFunction (0f,1f, TrapezoidalFunction.EdgeType.Left));
		//Velocidades
		FuzzySet fsMn = new FuzzySet ("Negativa", new TrapezoidalFunction (-1f, -0.5f, TrapezoidalFunction.EdgeType.Right));
		FuzzySet fsZero = new FuzzySet ("Media", new TrapezoidalFunction (-1f, -0.5f, 0.5f, 1f));
		FuzzySet fsMp = new FuzzySet ("Positiva", new TrapezoidalFunction (0.5f,1f, TrapezoidalFunction.EdgeType.Left));
		//Fuerzas
		FuzzySet fsZf = new FuzzySet ("PocaFuerza", new TrapezoidalFunction ( 0f, 1f, TrapezoidalFunction.EdgeType.Right));
		FuzzySet fsPf = new FuzzySet ("ZeroFuerza", new TrapezoidalFunction ( 0.5f, 1f, 9f, 10f));
		FuzzySet fsMf = new FuzzySet ("MuchaFuerza", new TrapezoidalFunction (9.5f,16f, TrapezoidalFunction.EdgeType.Left));


		//Entradas
		//Altura
		LinguisticVariable lvAltura = new LinguisticVariable ("Altura", -15, 15);
		lvAltura.AddLabel (fsBaja);
		lvAltura.AddLabel (fsMedia);
		lvAltura.AddLabel (fsAlta);
		//Velocidades
		LinguisticVariable lvVelocidad = new LinguisticVariable ("Velocidad", -25, 25);
		lvVelocidad.AddLabel (fsMn);
		lvVelocidad.AddLabel (fsZero);
		lvVelocidad.AddLabel (fsMp);
		//Fuerzas
		LinguisticVariable lvFuerza = new LinguisticVariable ("Fuerza", 0, 16);
		lvFuerza.AddLabel (fsPf);
		lvFuerza.AddLabel (fsZf);
		lvFuerza.AddLabel (fsMf);

		//La base de datos
		Database fuzzyDb = new Database();
		fuzzyDb.AddVariable (lvAltura);
		fuzzyDb.AddVariable (lvFuerza);
		fuzzyDb.AddVariable (lvVelocidad);

		//Sistema de interferencia


		ISL = new InferenceSystem (fuzzyDb, new CentroidDefuzzifier(1000));

		ISL.NewRule ("Rule 1", "IF Altura IS Baja AND Velocidad IS Negativa THEN Fuerza IS MuchaFuerza" );
		ISL.NewRule ("Rule 2", "IF Altura IS Baja AND Velocidad IS Media THEN Fuerza IS MuchaFuerza" );
		ISL.NewRule ("Rule 3", "IF Altura IS Baja AND Velocidad IS Positiva THEN Fuerza IS MuchaFuerza" );

		ISL.NewRule ("Rule 4", "IF Altura IS Media AND Velocidad IS Negativa THEN Fuerza IS MuchaFuerza" );
		ISL.NewRule ("Rule 5", "IF Altura IS Media AND Velocidad IS Media THEN Fuerza IS ZeroFuerza" );
		ISL.NewRule ("Rule 6", "IF Altura IS Media AND Velocidad IS Positiva THEN Fuerza IS ZeroFuerza" );

		ISL.NewRule ("Rule 7", "IF Altura IS Alta AND Velocidad IS Negativa THEN Fuerza IS ZeroFuerza" );
		ISL.NewRule ("Rule 8", "IF Altura IS Alta AND Velocidad IS Media THEN Fuerza IS ZeroFuerza" );
		ISL.NewRule ("Rule 9", "IF Altura IS Alta AND Velocidad IS Positiva THEN Fuerza IS ZeroFuerza" );



	}

	// Use this for initialization
	void Start () {
		
		thebox = gameObject.GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void Update () {
		InitFuzzyEngine ();

		//Entradas

		deltaAltura = gameObject.transform.position.y - SlideAltura.value; 
		velocidad = thebox.velocity.y;
		ISL.SetInput ("Velocidad", velocidad);
		ISL.SetInput ("Altura", deltaAltura);
		ISL.SetInput ("Fuerza", fuerza);

		Debug.Log (velocidad);

		try{


			fuerza = ISL.Evaluate("Fuerza");
		}
		catch (Exception) 
		{

		}

		thebox.AddForce (Vector3.up*fuerza);
	}

}
