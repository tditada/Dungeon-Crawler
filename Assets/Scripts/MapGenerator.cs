using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public GameObject startingChunkPrefab;
	public GameObject endingChunkPrefab;
	public GameObject twoWayCorridorPrefab;
	public GameObject threeWayCorridorPrefab;
	public GameObject fourWayCorridorPrefab;
	public GameObject cornerPrefab;
	public GameObject chamberPrefab;

	private Dictionary<Point, GameObject> map = new Dictionary<Point, GameObject>();
	private System.Random random;

	public void newMap(int maxSize, int minSteps, int maxSteps, int seed, int ramificationCoefficient, double ramificationProbability) {
		if (minSteps < maxSteps) { throw new ArgumentException(); }
		if (minSteps > maxSize) { throw new ArgumentException(); }
		if (maxSteps > maxSize) { throw new ArgumentException(); }
		if (ramificationProbability < 0.0 || ramificationProbability > 1.0) { throw new ArgumentException(); }

		var startPoint = new Point(random.Next(0, maxSize), random.Next(0, maxSize));
		var endPoint = GetEndPoint(startPoint, minSteps, maxSteps, maxSize);

		if (endPoint == null) {
			return;
		}

		map.Add(startPoint, Instantiate(startingChunkPrefab));
		map.Add(endPoint, Instantiate(endingChunkPrefab));

		GenerateTrivialPath(startPoint, endPoint);

		Ramify(ramificationCoefficient, ramificationProbability);
	}

	Chunk InstantiateChunk(Chunk chunk) {
		return (Chunk)Instantiate(chunk);
	}

	Point GetEndPoint (Point startPoint, int minSteps, int maxSteps, int maxSize) {
		var endPoint = new Point(0, 0);
		var maxTryes = maxSteps * maxSteps;

		while (maxTryes > 0 && startPoint.Distance(endPoint) < minSteps && startPoint.Distance(endPoint) >= maxSteps) {
			endPoint = new Point(random.Next(0, maxSize), random.Next(0, maxSize));
			maxTryes--;
		}

		if (maxTryes == 0) {
			return null;
		}

		return endPoint;
	}

	void GenerateTrivialPath (Point startingPoint, Point endingPoint) {
		var leftmostPoint = startingPoint.X < endingPoint.X ? startingPoint : endingPoint;
		var rightmostPoint = startingPoint.X > endingPoint.X ? startingPoint : endingPoint;
		var topmostPoint = startingPoint.Y < endingPoint.Y ? startingPoint : endingPoint;
		var bottomostPoint = startingPoint.Y > endingPoint.Y ? startingPoint : endingPoint;

		for (int i = leftmostPoint.X + 1; i < rightmostPoint.X; i++) {
			map.Add(new Point(i, leftmostPoint.Y), Instantiate(twoWayCorridorPrefab));
		}

		for (int j = topmostPoint.Y + 1; j < bottomostPoint.Y; j++) {
			map.Add(new Point(rightmostPoint.X, j), Instantiate(twoWayCorridorPrefab));
		}

		var cornerPoint = new Point(rightmostPoint.X, leftmostPoint.Y);
		if (!map.ContainsKey(cornerPoint)) {
			map.Add(cornerPoint, Instantiate(cornerPrefab));
		}
	}

	void Ramify (int ramificationCoefficient, double ramificationProbability) {
		foreach (var chunk in map) {
			if (random.NextDouble() < ramificationProbability) {
				RamifyChunk(chunk, ramificationCoefficient, ramificationProbability);
			}
		}
	}

	void RamifyChunk (KeyValuePair<Point, GameObject> chunk, int ramificationCoefficient, double ramificationProbability) {

		throw new NotImplementedException ();
	}

	private class Point {
		public int X { get; private set; }
		public int Y { get; private set; }

		public Point(int x, int y) {
			this.X = x;
			this.Y = y;
		}

		public int Distance (Point p) {
			if (p == null) { throw new ArgumentNullException(); }
			return Math.Abs(this.X - p.X) + Math.Abs(this.Y - p.Y);
		}
	}
}
