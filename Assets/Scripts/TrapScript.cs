using UnityEngine;
using System.Collections;

public class TrapScript : MonoBehaviour {

	GameController gameController;

	void Start() {
		GameObject controllerObject = GameObject.FindGameObjectWithTag ("GameController");
		gameController = controllerObject.GetComponent<GameController> ();
	}

	void OnTriggerEnter(Collider other) {
		AudioSource audioSource = this.gameObject.GetComponent<AudioSource> ();
		audioSource.Play ();
		Explode ();
	}

	void Explode() {
		var exp = GetComponent<ParticleSystem>();
		exp.Play();
		Destroy(gameObject, exp.duration);
		gameController.Lose ();
	}
}