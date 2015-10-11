using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapsGeneration : MonoBehaviour {
	
	private Dictionary<Vector2, GameObject> map = new Dictionary<Vector2, GameObject>();
	public int probability_room;

	void setMap(Dictionary<Vector2, GameObject> map){
		this.map = map;
	}

	void generateTraps(){
		foreach (KeyValuePair<Vector2, GameObject> kvp in map) {
			GameObject value = kvp.Value.gameObject;
			ChunckScript cs = value.GetComponent<ChunckScript>();
			if(Random.value > probability_room){
				List<Vector3> v3 = cs.returnPossibleTraps();
				Vector3 trap = v3[Random.Range(0,v3.Count)]; //TODO: activate trap
			}

		}
	}
}
