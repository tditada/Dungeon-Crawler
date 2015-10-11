using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunckScript : MonoBehaviour {

	public List<Vector3> mountPoints = new List<Vector3>(); //doors and other 
	public List<Vector3> deathTraps = new List<Vector3>(); 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public List<Vector3> returnPossibleTraps(){
		return deathTraps;
	}
}
