using UnityEngine;
using System.Collections;

public class ChestController : MonoBehaviour {

	GameController gameController;
	
	void Start() {
		GameObject controllerObject = GameObject.FindGameObjectWithTag ("GameController");
		gameController = controllerObject.GetComponent<GameController> ();
	}
	
	void OnTriggerEnter(Collider other) {
		gameController.Win ();
	}

}
