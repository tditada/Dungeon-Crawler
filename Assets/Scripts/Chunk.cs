using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Orientation {
	NORTH = 0,
	EAST = 1,
	SOUTH = 2,
	WEST = 3
}

public class Chunk : MonoBehaviour {
	
	public Orientation chunkOrientation = Orientation.NORTH;

	public List<Vector3> mountingPoints = new List<Vector3>();

	public List<Vector3> deathTraps = new List<Vector3>(); 
		
	public bool isStartChunk = false;

	public bool isEndChunk = false;

	public Chunk Rotate(Orientation orientation) {
		while (chunkOrientation != orientation) {
			gameObject.transform.Rotate(0.0f, 90.0f, 0.0f);
			chunkOrientation++;
		}
		return this;
	}

	public bool IsStartChunk {
		get { return isStartChunk; }
		set { 
			if (value && IsEndChunk) {
				throw new ArgumentException();
			}
			isStartChunk = value;
		}
	}

	bool IsEndChunk {
		get { return isEndChunk; }
		set {
			if (value && IsStartChunk) {
				throw new ArgumentException();
			}
			isEndChunk = value;
		}
	}

	public List<Vector3> returnPossibleTraps(){
		return deathTraps;
	}
}