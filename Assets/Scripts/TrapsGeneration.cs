using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TrapsGeneration : MonoBehaviour {
	
	private Dictionary<Point, Chunk> map = new Dictionary<Point, Chunk>();
	public int probability_room;
	public List<GameObject> traps = new List<GameObject>();
	private bool flag=true;
	public GameObject chunk;

	void setMap(Dictionary<Point, Chunk> map){
		this.map = map;
	}

	void generateTraps(){
		foreach (KeyValuePair<Point, Chunk> kvp in map) {
			Chunk value = kvp.Value;
			if(Random.value < probability_room){
				Debug.Log ("im here");
				Debug.Log (value);
				List<Vector3> v3 = value.returnPossibleTraps();
				Vector3 trapPosition = v3[Random.Range(0,v3.Count)]; 
				GameObject p = traps[Random.Range(0,traps.Count)];
				Instantiate(p,trapPosition,p.transform.rotation);
			}

		}
	}

	void Update(){
		if (flag) {
			flag = false;
			map.Add(new Point(0,0), chunk.GetComponent<Chunk>());
			generateTraps();
		}
	}

}