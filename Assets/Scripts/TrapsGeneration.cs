using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TrapsGeneration : MonoBehaviour {
	
	private Dictionary<Point, Chunk> map = new Dictionary<Point, Chunk>();
	public int probability_room;
	public List<GameObject> traps = new List<GameObject>();
	private bool flag=true;
	public List<GameObject> chunks;

	public void setMap(Dictionary<Point, Chunk> map){
		this.map = map;
	}

	public void generateTraps(){
		foreach (KeyValuePair<Point, Chunk> kvp in map) {
			Chunk value = kvp.Value;
			if(Random.value < probability_room){
				Debug.Log (value);
				List<Vector3> possibleTraps = value.returnPossibleTraps();
				// revisar la linea 25 que da error al correr
				Vector3 trapPosition = possibleTraps[Random.Range(0,possibleTraps.Count)] + new Vector3(kvp.Key.X, kvp.Key.Y); 
				GameObject p = traps[Random.Range(0,traps.Count-1)];
				Instantiate(p,trapPosition,p.transform.rotation);
			}
		}
	}

	void Update(){
		if (flag) {
			flag = false;
			foreach (GameObject chunk in chunks) {
				map.Add(new Point((int)chunk.gameObject.transform.position.x,(int)chunk.gameObject.transform.position.y), chunk.GetComponent<Chunk>());
				generateTraps();
			}
		}
	}

}