using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour {

	[SerializeField] protected GameObject successParticles;

	void OnTriggerEnter(Collider c) { if (c.attachedRigidbody.name=="Pinball Wizard") Win(); }

	void Win() { 
		var o = Instantiate(successParticles); 
		o.transform.position = transform.position; 
		o.transform.rotation = transform.rotation;
		Destroy(gameObject); 
	}

}
