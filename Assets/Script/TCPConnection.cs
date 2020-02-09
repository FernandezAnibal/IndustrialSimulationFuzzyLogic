using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class TCPConnection : MonoBehaviour {


	//El nombre de la conexión por si se tiene mas de una
	public string conName = "Localhost";

	//Dirección ip a la cual se debe conectar;
	private string conHost = "127.0.0.1";

	//puerto para la conexión
	private int conPort = 27015;

	//a true/false para ele stado de la conexión
	public bool socketReady = false;

	//Temporizador para regular la velocidad de envio de mensaje
	float timer;

	//Variables para el uso de la conexión tcp
	TcpClient mySocket;
	NetworkStream theStream;
	StreamWriter theWriter;
	StreamReader theReader;

	//Para guardar la cadena que viene de labview 
	private string Temp;
	//Almacenará la posicion de los pistones
	private float[] Pistones = new float[3];
	//Almacenara el peso y la posicion de las cajas
	public float[] pesoYpos = new float[6];
	//Almacenara el estado de los sensores de posicion
	private bool[] sensorPosPistones = new bool[6];

	//Objetos donde se reflejara la posicion de cada pistón
	public GameObject Piston1;
	public GameObject Piston2;
	public GameObject Piston3;

	//Estado de los detectores, cuando uno de los detectores se activa manda la orden al actuador
	public float[] sensorP = new float[3];

	//Variables para el estado de los sensores
	PosSensor posSensor;
	public GameObject posSensorO;

	void Start(){
		//Inicialización de variables
		posSensor = posSensorO.GetComponent<PosSensor> ();
		pesoYpos = new float[]{0,0,0,0,0,0 };
		sensorPosPistones = new bool[] {true, false, true, false, true, false};
		sensorP[0] = 0;
		sensorP[1] = 0;
		sensorP[2] = 0;


	}

	//Configuración para abrir la conexión
	public void setupSocket() {
		try {
			mySocket = new TcpClient(conHost, conPort);
			theStream = mySocket.GetStream();
			theWriter = new StreamWriter(theStream);
			theReader = new StreamReader(theStream);
			socketReady = true;
		}
		catch (Exception e) {
			Debug.Log("Socket error:" + e);
		}
	}

	//Enviar mensajes al servidor
	public void writeSocket(string theLine) {
		if (!socketReady)
			return;
		String tmpString = theLine + "\r\n";
		theWriter.Write(tmpString);
		theWriter.Flush();
	}

	//Recibir mensajes del servidor
	public string readSocket() {
		String result = "";
		if (mySocket.Available > 0) {
			Byte[] inStream = new Byte[mySocket.SendBufferSize];
			theStream.Read (inStream, 0 , inStream.Length); 
			result += System.Text.Encoding.UTF8.GetString(inStream);

		}
		return result;
	}

	//Descinectar del servidor
	public void closeSocket() {
		if (!socketReady)
			return;
		theWriter.Close();
		theReader.Close();
		mySocket.Close();
		socketReady = false;
	}

	//Mantener la conexión
	public void maintainConnection(){
		if(!theStream.CanRead) {
			setupSocket();
		}
	}

	//Función para leer la cadena de caracteres recibida por la conexión tcp en formato JSON, se divide y se tranforma en numeros reales
	//para ser introducidos en la matriz de posicion de los pistones
	float[] JsonToArrayP(string JsonP, float[] arrayPiston2){

		char[] c = new char[]{'"',':',',','{','}' };
		string[] s = JsonP.Split (c, StringSplitOptions.RemoveEmptyEntries);
		float[] pistonesPos = new float[s.Length/2];
		for (int i = 0; i < s.Length-1; i ++ ){
			if (i % 6 == 0){
				pistonesPos [i / 6] = float.Parse(s [i+1]);
			}
		}

		for (int i = 0; i < arrayPiston2.Length; i++){
			arrayPiston2 [i] = pistonesPos [i];
		}

		return arrayPiston2;
	
	}

	//Función para leer la cadena de caracteres recibida por la conexión tcp en formato JSON, se divide y se tranforma en variables booleanas
	//para ser introducidos en la matriz de sensores de posicion
	bool[] JsonToArraySPP(string JsonP, bool[] sensores){
	
		char[] c = new char[]{'"',':',',','{','}' };
		string[] s = JsonP.Split (c, StringSplitOptions.RemoveEmptyEntries);
		bool[] sensoresPos = new bool[s.Length/2];
		for (int i = 0; i < s.Length-1; i ++ ){
			if (i % 6 == 0){
				sensoresPos [(i / 6)*2] = Convert.ToBoolean(s [i+3]);
				sensoresPos [((i / 6)*2)+1] = Convert.ToBoolean(s [i+5]);
			}
		}

		for (int i = 0; i < sensores.Length; i++){
			sensores [i] = sensoresPos [i];
		}
		return sensores;
	}

	void Update (){
		//Extrae la cadena de texto de Labview
		if (socketReady) {
			Temp = readSocket ();
			if (Temp != ""){
				//Se llama a la funcion para llenar la matriz de posicion de pistones
				Pistones = JsonToArrayP (Temp, Pistones);
				sensorPosPistones = JsonToArraySPP (Temp, sensorPosPistones);
			}
		}
		//Actualizar sensores
		posSensor.a1 = sensorPosPistones[0];
		posSensor.a2 = sensorPosPistones[1];

		posSensor.b1 = sensorPosPistones[2];
		posSensor.b2 = sensorPosPistones[3];

		posSensor.c1 = sensorPosPistones[4];
		posSensor.c2 = sensorPosPistones[5];

		//Le da la posicion a cada piston que viene de labview
		Piston1.transform.position = new Vector3 (-Pistones[0], Piston1.transform.position.y, Piston1.transform.position.z );
		Piston2.transform.position = new Vector3 (-Pistones[1], Piston2.transform.position.y, Piston2.transform.position.z);
		Piston3.transform.position = new Vector3 (-Pistones[2], Piston3.transform.position.y, Piston3.transform.position.z);
		timer += Time.fixedDeltaTime;

		//0.07 fue el tiempo de actualización para mandar la información a labview y sea leida correctamente
		if (timer >= 0.07f) {
			timer = 0;

			//Si se activa la valvula para regresar se le envia esa información a labview para regresar el piston

			if (sensorP [0] == 0 && posSensor.a2) {
				MensajeOall ();
			}
			if (sensorP [1] == 0 && posSensor.b2 ) {
				MensajeOall ();
			}
			if (sensorP [2] == 0 && posSensor.c2) {
				MensajeOall ();
			}


			//Si se activa la valvula para extender se le envía esa informaciñon a labview para que haga la simulación respectiva
			if(sensorP[0]==1 && posSensor.a1 || sensorP[1]==1 && posSensor.b1 || sensorP[2]==1 && posSensor.c1 ){
				MensajeOall ();

			}


		}
			


	}

	//Se utiliza para mandar información a labview en formato JSON para que pueda ser decodificada correctamente
	public void MensajeO(string mms1,string mms2, string mms3, string mms4, string mms5, string mms6, string mms7, string mms8, string mms9){

		//
		string Jsonf = "{\"1\":"+mms1+",\"2\":"+mms2+",\"3\":"+mms3+",\"Pos1\":"+mms4+",\"Peso1\":"+mms5+",\"Pos2\":"+mms6+",\"Peso2\":"+mms7+",\"Pos3\":"+mms8+",\"Peso3\":"+mms9+ "}";
		int tama = Jsonf.Length;
		writeSocket(tama.ToString());
		writeSocket (Jsonf);

	}

	public void MensajeOall (){
		MensajeO (sensorP[0].ToString (), sensorP[1].ToString (), sensorP[2].ToString (), pesoYpos[0].ToString(), pesoYpos[1].ToString(), pesoYpos[2].ToString(), pesoYpos[3].ToString(), pesoYpos[4].ToString(), pesoYpos[5].ToString());
		
	}

}

