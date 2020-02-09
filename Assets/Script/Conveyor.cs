using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour {


	private float speed = 23;
	private Vector2 offset;
	private Renderer renderizo;



	// Use this for initialization
	void Start () {
		renderizo = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		offset += new Vector2(0,0.3f) * Time.deltaTime;
		renderizo.material.SetTextureOffset ("_MainTex", offset);
	}
	//Cuando un objeto este en contacto con la cinta transportadora tomara una velocidad
	void OnCollisionStay(Collision caja){
		float beltVelocity = speed * Time.deltaTime;
		caja.rigidbody.velocity = beltVelocity * -transform.forward;
	}
}
