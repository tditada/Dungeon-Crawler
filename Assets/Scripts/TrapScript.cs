using UnityEngine;
using System.Collections;

public class TrapScript : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		AudioSource audioSource = this.gameObject.GetComponent<AudioSource> ();
		audioSource.Play ();
		Explode ();
	}
//
//	void onTriggerEnter(){
//		Explode ();
//	}
	
	void Explode() {
		var exp = GetComponent<ParticleSystem>();
		exp.Play();
		Destroy(gameObject, exp.duration);
	}
}