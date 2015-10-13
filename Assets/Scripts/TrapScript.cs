using UnityEngine;
using System.Collections;

public class TrapScript : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		Explode ();
	}

	void onCollisionEnter(){
		Explode ();
	}
	
	void Explode() {
		var exp = GetComponent<ParticleSystem>();
		exp.Play();
		Destroy(gameObject, exp.duration);
	}
}
