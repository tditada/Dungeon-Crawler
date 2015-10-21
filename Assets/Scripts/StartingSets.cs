using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartingSets : MonoBehaviour {

	Chunk startPoint;
	Chunk endPoint;
	public GameObject treasure;

	private void setMap(Dictionary<Point, Chunk> map){		
		foreach (KeyValuePair<Point, Chunk> kvp in map) {
			if(kvp.Value.isStartChunk){
				startPoint=kvp.Value;
			}else if(kvp.Value.isEndChunk){
				endPoint=kvp.Value;
			}
		}
	}

	private void setPlayer(){
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = startPoint.gameObject.transform.position;
	}

	private void setTreasure(){
		Debug.Log ("set treasure");
		Instantiate (treasure,endPoint.gameObject.transform.position, endPoint.gameObject.transform.rotation);
	}

	public void startingGame(Dictionary<Point, Chunk> map){
		setMap (map);
		setPlayer ();
		setTreasure ();
	}

}