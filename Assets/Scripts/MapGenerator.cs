using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	//public TwoWayCorridorChunk twoWayCorridorPrefab;
	//public ThreeWayCorridorChunk threeWayCorridorPrefab;
	//public FourWayCorridorChunk fourWayCorridorPrefab;
	//public CornerChunk cornerPrefab;
	//public ChamberChunk chamberPrefab;
	//public CornerChamberChunk cornerChamberPrefab;
	//public TwoWayChamberChunk twoWayChamberPrefab;
	//public ThreeWayChamberChunk threeWayChamberPrefab;
	//public FourWayChamberChunk fourWayChamberPrefab;
	public Chunk twoWayCorridorPrefab;
	public Chunk threeWayCorridorPrefab;
	public Chunk fourWayCorridorPrefab;
	public Chunk cornerPrefab;
	public Chunk chamberPrefab;
	public Chunk cornerChamberPrefab;
	public Chunk twoWayChamberPrefab;
	public Chunk threeWayChamberPrefab;
	public Chunk fourWayChamberPrefab;
	public double chunkSize = 34.8;

	public double ramificationProbability = 0.5;
	public double chamberProbability = 0.2;

	private Dictionary<Point, Chunk> map;
	private Point startPoint;
	private Point endPoint;
	private System.Random random;

	public Dictionary<Point, Chunk> NewMap(int maxSize, int minSteps, int maxSteps, int seed) {
		if (minSteps < maxSteps) { throw new ArgumentException(); }
		if (minSteps > maxSize) { throw new ArgumentException(); }
		if (maxSteps > maxSize) { throw new ArgumentException(); }
		if (ramificationProbability < 0.0 || ramificationProbability > 1.0) { throw new ArgumentException(); }
		if (chamberProbability < 0.0 || ramificationProbability > 1.0) { throw new ArgumentException(); }

		map = new Dictionary<Point, Chunk>();
		startPoint = new Point(random.Next(0, maxSize), random.Next(0, maxSize));
		endPoint = GetEndPoint(startPoint, minSteps, maxSteps, maxSize);

		if (endPoint == null) {
			return null;
		}

		map.Add(startPoint, Instantiate(chamberPrefab) as Chunk);
		map.Add(endPoint, Instantiate(chamberPrefab) as Chunk);

		GenerateTrivialPath(startPoint, endPoint);

		Ramify(ramificationProbability, chamberProbability, maxSize);

		GenerateMap();
		return map;
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

		map[leftmostPoint].Rotate(Orientation.EAST);
		if (rightmostPoint.Equals(topmostPoint)) {
			map[rightmostPoint].Rotate(Orientation.SOUTH);
		} else {
			map[rightmostPoint].Rotate(Orientation.NORTH);
		}

		GenerateHorizontalPath(leftmostPoint, rightmostPoint);
		GenerateVerticalPath(topmostPoint, bottomostPoint, rightmostPoint);
		GenerateCorner(leftmostPoint, rightmostPoint);
	}

	void GenerateHorizontalPath (Point leftmostPoint, Point rightmostPoint) {
		for (int i = leftmostPoint.X + 1; i < rightmostPoint.X; i++) {
			var chunk = Instantiate(twoWayCorridorPrefab) as Chunk;
			chunk.Rotate(Orientation.EAST);
			map.Add(new Point(i, leftmostPoint.Y), chunk);
		}
	}

	void GenerateVerticalPath (Point topmostPoint, Point bottomostPoint, Point rightmostPoint) {
		for (int j = topmostPoint.Y + 1; j < bottomostPoint.Y; j++) {
			var chunk = Instantiate(twoWayCorridorPrefab);
			chunk.Rotate(Orientation.NORTH);
			map.Add(new Point(rightmostPoint.X, j), chunk);
		}
	}

	void GenerateCorner (Point leftmostPoint, Point rightmostPoint) {
		var cornerPoint = new Point(rightmostPoint.X, leftmostPoint.Y);
		if (!map.ContainsKey(cornerPoint)) {
			var chunk = Instantiate(cornerPrefab) as Chunk;
			if (rightmostPoint.Y > leftmostPoint.Y) {
				chunk.Rotate(Orientation.WEST);
			} else {
				chunk.Rotate(Orientation.SOUTH);
			}
			map.Add(cornerPoint, chunk);
		}
	}

	void Ramify (double ramificationProbability, double chamberProbability, int maxSize) {
		foreach (var chunk in map) {
			RamifyChunk(null, chunk.Key, ramificationProbability, chamberProbability, maxSize);
		}
	}

	bool RamifyChunk (Point previousPosition, Point currentPosition, double ramificationProbability, double chamberProbability, int maxSize) {
		if (!map.ContainsKey(currentPosition) && random.NextDouble() < ramificationProbability) {
			var ramificationPoints = GetRamificationPoints(currentPosition, maxSize);
			for (int i = 0; i < ramificationPoints.Count; i++) {
				map.Add(ramificationPoints[i], null);
				if (!RamifyChunk(currentPosition, ramificationPoints[i], ramificationProbability, chamberProbability, maxSize)) {
					ramificationPoints.Remove(ramificationPoints[i]);
					i--;
				}
			}
			DecideChunk(previousPosition, currentPosition, chamberProbability, ramificationPoints);
			return true;
		}
		return false;
	}

	void DecideChunk (Point previousPosition, Point currentPosition, double chamberProbability, List<Point> ramificationPoints) {
		var chunk = map[currentPosition];
		switch (ramificationPoints.Count) {
		case 0:
			if (chunk == null) {
				chunk = PutChamber(previousPosition, currentPosition);
			}
			break;
		case 1:
			if (chunk == null) {
				chunk = PutTwoWayChunk(currentPosition, previousPosition, ramificationPoints[0], chamberProbability);
			} else {
				Destroy(chunk);
				chunk = PutThreeWayChunk(currentPosition, previousPosition, ramificationPoints[0], ramificationPoints[1], chamberProbability);
			}
			break;
		case 2:
			if (chunk == null) {
				chunk = PutThreeWayChunk(currentPosition, previousPosition, ramificationPoints[0], ramificationPoints[1], chamberProbability);
			} else {
				Destroy(chunk);
				chunk = PutFourWayChunk(chamberProbability);
			}
			break;
		case 3:
			if (chunk != null) {
				Destroy(chunk);
			}
			chunk = PutFourWayChunk(chamberProbability);
			break;
		}
		map[currentPosition] = chunk;
	}

	Chunk PutChamber (Point previousPosition, Point currentPosition) {
		var chunk = Instantiate(chamberPrefab) as Chunk;
		chunk.Rotate(previousPosition.RelativeOrientationTo(currentPosition));
		return chunk;
	}

	Chunk PutTwoWayChunk (Point currentPosition, Point previousPosition, Point other, double chamberProbability) {
		Chunk chunk = null;
		if ((((int)previousPosition.RelativeOrientationTo(currentPosition) -
		      (int)other.RelativeOrientationTo(currentPosition)) % 2) == 0) {
			chunk = PutSraightTwoWayChunk(currentPosition, previousPosition, other, chamberProbability);
		} else {
			chunk = PutCornerTwoWayChunk(currentPosition, previousPosition, other, chamberProbability);
	    }
		return chunk;
	}

	Chunk PutSraightTwoWayChunk (Point currentPosition, Point previousPosition, Point other, double chamberProbability) {
		Chunk chunk = InstantiateChunk(twoWayCorridorPrefab, twoWayChamberPrefab, chamberProbability); 
		if (previousPosition.RelativeOrientationTo(currentPosition) == Orientation.SOUTH ||
		    previousPosition.RelativeOrientationTo(currentPosition) == Orientation.NORTH) {
			chunk.Rotate(Orientation.NORTH);						
		} else {
			chunk.Rotate(Orientation.EAST);
		}
		return chunk;
	}

	Chunk PutCornerTwoWayChunk (Point currentPosition, Point previousPosition, Point other, double chamberProbability) {
		Chunk chunk = InstantiateChunk(cornerPrefab, cornerChamberPrefab, chamberProbability);
		if ((previousPosition.RelativeOrientationTo(currentPosition) == Orientation.NORTH &&
		     other.RelativeOrientationTo(currentPosition) == Orientation.EAST) ||
		    (previousPosition.RelativeOrientationTo(currentPosition) == Orientation.EAST &&
		 other.RelativeOrientationTo(currentPosition) == Orientation.NORTH)) {
			chunk.Rotate(Orientation.NORTH);
		} else if ((previousPosition.RelativeOrientationTo(currentPosition) == Orientation.EAST &&
		            other.RelativeOrientationTo(currentPosition) == Orientation.SOUTH) ||
		           (previousPosition.RelativeOrientationTo(currentPosition) == Orientation.SOUTH &&
		 other.RelativeOrientationTo(currentPosition) == Orientation.EAST)) {
			chunk.Rotate(Orientation.EAST);
		} else if ((previousPosition.RelativeOrientationTo(currentPosition) == Orientation.SOUTH &&
		            other.RelativeOrientationTo(currentPosition) == Orientation.WEST) ||
		           (previousPosition.RelativeOrientationTo(currentPosition) == Orientation.WEST &&
		 other.RelativeOrientationTo(currentPosition) == Orientation.SOUTH)) {
			chunk.Rotate(Orientation.SOUTH);
		} else if ((previousPosition.RelativeOrientationTo(currentPosition) == Orientation.WEST &&
		            other.RelativeOrientationTo(currentPosition) == Orientation.NORTH) ||
		           (previousPosition.RelativeOrientationTo(currentPosition) == Orientation.NORTH &&
		 other.RelativeOrientationTo(currentPosition) == Orientation.WEST)) {
			chunk.Rotate(Orientation.WEST);
		}
		return chunk;
	}

	Chunk PutThreeWayChunk (Point currentPosition, Point previousPosition, Point other1, Point other2, double chamberProbability) {
		Chunk chunk = InstantiateChunk(threeWayCorridorPrefab, threeWayChamberPrefab, chamberProbability);
		var orientationSum = (int)previousPosition.RelativeOrientationTo(currentPosition) +
			(int)other1.RelativeOrientationTo(currentPosition) + (int)other2.RelativeOrientationTo(currentPosition);
		if (orientationSum % 2 == 0) {
			if (previousPosition.RelativeOrientationTo(currentPosition) == Orientation.EAST ||
			    other1.RelativeOrientationTo(currentPosition) == Orientation.EAST ||
			    other2.RelativeOrientationTo(currentPosition) == Orientation.EAST) {
				chunk.Rotate(Orientation.EAST);
			} else {
				chunk.Rotate(Orientation.WEST);
			}
		} else {
			if (previousPosition.RelativeOrientationTo(currentPosition) == Orientation.NORTH ||
			    other1.RelativeOrientationTo(currentPosition) == Orientation.NORTH ||
			    other2.RelativeOrientationTo(currentPosition) == Orientation.NORTH) {
				chunk.Rotate(Orientation.NORTH);
			} else {
				chunk.Rotate(Orientation.SOUTH);
			}
		}
		return chunk;
	}

	Chunk PutFourWayChunk(double chamberProbability) {
		return InstantiateChunk(fourWayCorridorPrefab, fourWayChamberPrefab, chamberProbability);
	}

	Chunk InstantiateChunk(Chunk corridorChunk, Chunk chamberChunk, double chamberProbability) {
		if (random.NextDouble() < chamberProbability) {
			return Instantiate(chamberChunk) as Chunk;
		} else {
			return Instantiate(corridorChunk) as Chunk;
		}
	}

	List<Point> GetRamificationPoints (Point center, int maxSize) {
		var ramificationPoints = new List<Point>();
		var xs = new int[] {1, 0, -1, 0};
		var ys = new int[] {0, 1, 0, -1};
		for (int i = 0; i < xs.Length; i++) {
			var point = new Point(center.X + xs[i], center.Y + ys[i]);
			if (UsablePoint(point, maxSize)) {
				ramificationPoints.Add(point);
			}
		}
		return ramificationPoints;
	}

	bool UsablePoint (Point p, int maxSize) {
		return p.X >=0 && p.X < maxSize && p.Y >= 0 && p.Y < maxSize && !map.ContainsKey(p);
	}
	
	void GenerateMap() {
		foreach (Point p in map.Keys) {
			Chunk c = map[p];
			c.gameObject.transform.position = new Vector3((float)(p.Y * chunkSize), 0.0f, (float)(p.X * chunkSize));
			c.enabled = true;
		}
	}
}
