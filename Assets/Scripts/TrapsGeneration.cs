using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TrapsGeneration : MonoBehaviour {

	public int probability_room;
	public List<GameObject> traps = new List<GameObject>();
	private bool flag=true;

	public void generateTraps(Dictionary<Point, Chunk> map){
		Debug.Log ("generate trap");
		foreach (KeyValuePair<Point, Chunk> kvp in map) {
			Chunk value = kvp.Value;
			List<Vector3> possibleTraps = value.returnPossibleTraps();
			Debug.Log (possibleTraps.Count);
			int i = Random.Range (0,possibleTraps.Count);
			while(i>0){
				if(Random.value < probability_room){
					Vector3 trapPosition = possibleTraps[Random.Range(0,possibleTraps.Count)]; 
					Debug.Log (trapPosition);
					GameObject p = traps[Random.Range(0,traps.Count)];
					GameObject go = Instantiate(p,Vector3.zero,p.transform.rotation) as GameObject;
					go.gameObject.transform.parent=value.gameObject.transform;
					go.gameObject.transform.localPosition=trapPosition;
				}
				i--;
			}
		}
	}

}