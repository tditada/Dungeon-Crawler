using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
		if (minSteps > maxSteps) { throw new ArgumentException(); }
		if (minSteps > maxSize) { throw new ArgumentException(); }
		if (maxSteps > maxSize) { throw new ArgumentException(); }
		if (ramificationProbability < 0.0 || ramificationProbability > 1.0) { throw new ArgumentException(); }
		if (chamberProbability < 0.0 || ramificationProbability > 1.0) { throw new ArgumentException(); }
				
		Debug.Log("Generating New Map");

		map = new Dictionary<Point, Chunk>();
		random = new System.Random(seed);

		startPoint = new Point(random.Next(0, maxSize), random.Next(0, maxSize));

		Debug.LogFormat("Starting Point: " + startPoint);

		endPoint = GetEndPoint(startPoint, minSteps, maxSteps, maxSize);

		Debug.LogFormat("Ending Point: " + endPoint);

		if (endPoint == null) { return null; }

		map.Add(startPoint, Instantiate(chamberPrefab) as Chunk);
		map.Add(endPoint, Instantiate(chamberPrefab) as Chunk);

		GenerateTrivialPath(startPoint, endPoint);

		Ramify(ramificationProbability, chamberProbability, maxSize);

		GenerateMap();
		return map;
	}

	Point GetEndPoint (Point startPoint, int minSteps, int maxSteps, int maxSize) {
		Point endPoint = null;
		var maxTryes = maxSteps * maxSteps;
		var distance = 0;

		do {
			endPoint = new Point(random.Next(0, maxSize), random.Next(0, maxSize));
			maxTryes--;
			distance = startPoint.Distance(endPoint);
		} while (maxTryes > 0 && (distance < minSteps || distance >= maxSteps));

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
		Debug.Log ("Generating Trivial Path");
		Debug.Log ("Leftmost: " + leftmostPoint);
		Debug.Log ("Rightmost: " + rightmostPoint);
		Debug.Log ("Topmost: " + topmostPoint);
		Debug.Log ("Bottomost: " + bottomostPoint);

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
		Debug.Log ("Generating Horizontal Path");
		var previousPoint = new Point(leftmostPoint.X, leftmostPoint.Y);
		for (int i = leftmostPoint.X + 1; i < rightmostPoint.X; i++) {
			var currentPoint = new Point(i , leftmostPoint.Y);
			var nextPoint = new Point(i + 1, leftmostPoint.Y);
			map.Add(currentPoint, PutTwoWayChunk(currentPoint, previousPoint, nextPoint, chamberProbability));
		}
	}

	void GenerateVerticalPath (Point topmostPoint, Point bottomostPoint, Point rightmostPoint) {
		Debug.Log ("Generating Vertical Path");
		var previousPoint = new Point(rightmostPoint.X, topmostPoint.Y);
		for (int j = topmostPoint.Y + 1; j < bottomostPoint.Y; j++) {
			var currentPoint = new Point(rightmostPoint.X, j);
			var nextPoint = new Point(rightmostPoint.X, j + 1);
			map.Add(currentPoint, PutTwoWayChunk(currentPoint, previousPoint, nextPoint, chamberProbability));
			previousPoint = currentPoint;
		}
	}

	void GenerateCorner (Point leftmostPoint, Point rightmostPoint) {
		var cornerPoint = new Point(rightmostPoint.X, leftmostPoint.Y);
		if (!map.ContainsKey(cornerPoint)) {
			Debug.Log ("Putting Corner");
			var chunk = PutTwoWayChunk(cornerPoint, rightmostPoint, leftmostPoint, chamberProbability);
			map.Add(cornerPoint, chunk);
		}
	}

	void Ramify (double ramificationProbability, double chamberProbability, int maxSize) {
		Debug.Log ("Starting Ramification");
		foreach (var point in map.Keys.ToList()) {
			RamifyChunk(null, point, ramificationProbability, chamberProbability, maxSize);
		}
	}

	void RamifyChunk (Point previousPosition, Point currentPosition, double ramificationProbability, double chamberProbability, int maxSize) {
		var r = random.NextDouble();
		Debug.Log ("Current Point: " + currentPosition + " r = " + r);
		var ramificationPoints = new List<Point>();
		if (r < ramificationProbability) {
			Debug.Log ("Ramifying position: " + currentPosition);
			ramificationPoints.AddRange(GetRamificationPoints(currentPosition, maxSize));
			Debug.Log ("Total points to ramify = " + ramificationPoints.Count);
			foreach (var ramificationPoint in ramificationPoints) {
				map.Add (ramificationPoint, null);
			}
			foreach (var ramificationPoint in ramificationPoints) {
				RamifyChunk(currentPosition, ramificationPoint, ramificationProbability, chamberProbability, maxSize);
			}
		}
		DecideChunk(previousPosition, currentPosition, chamberProbability, ramificationPoints);
	}

	void DecideChunk (Point parentPoint, Point currentPoint, double chamberProbability, List<Point> ramificationPoints) {
		Debug.Log("Deciding Chunk at " + currentPoint);
		var chunk = map[currentPoint];
		switch (ramificationPoints.Count) {
		case 0:
			if (chunk == null) {
				chunk = PutChamber(currentPoint, parentPoint);
			}
			break;
		case 1:
			if (chunk == null) {
				chunk = PutTwoWayChunk(currentPoint, parentPoint, ramificationPoints[0], chamberProbability);
			} else {
				Point previoustPoint = null, nextPoint = null;
				switch (chunk.chunkOrientation) {
				case Orientation.NORTH:
					previoustPoint = new Point(currentPoint.X, currentPoint.Y + 1);
					nextPoint = new Point(currentPoint.X, currentPoint.Y - 1);
					break;
				case Orientation.EAST:
					previoustPoint = new Point(currentPoint.X - 1, currentPoint.Y);
					nextPoint = new Point(currentPoint.X + 1 , currentPoint.Y);
					break;
				case Orientation.SOUTH:
					previoustPoint = new Point(currentPoint.X - 1, currentPoint.Y);
					nextPoint = new Point(currentPoint.X, currentPoint.Y + 1);
					break;
				case Orientation.WEST:
					previoustPoint = new Point(currentPoint.X - 1, currentPoint.Y);
					nextPoint = new Point(currentPoint.X, currentPoint.Y - 1);
					break;
				}
				DestroyImmediate(chunk);
				chunk = PutThreeWayChunk(currentPoint, previoustPoint, ramificationPoints[0], nextPoint, chamberProbability);
			}
			break;
		case 2:
			if (chunk == null) {
				chunk = PutThreeWayChunk(currentPoint, parentPoint, ramificationPoints[0], ramificationPoints[1], chamberProbability);
			} else {
				DestroyImmediate(chunk);
				chunk = PutFourWayChunk(chamberProbability);
			}
			break;
		case 3:
			if (chunk != null) {
				DestroyImmediate(chunk);
			}
			chunk = PutFourWayChunk(chamberProbability);
			break;
		}
		map[currentPoint] = chunk;
	}

	Chunk PutChamber (Point currentPoint, Point parentPoint) {
		var chunk = Instantiate(chamberPrefab) as Chunk;
		chunk.Rotate(parentPoint.RelativeOrientationTo(currentPoint));
		Debug.Log ("Putting Chamber at " + currentPoint + " with orientation towards " + chunk.chunkOrientation);
		return chunk;
	}

	Chunk PutTwoWayChunk (Point currentPoint, Point parentPoint, Point otherPoint, double chamberProbability) {
		Chunk chunk = null;
		if ((((int)parentPoint.RelativeOrientationTo(currentPoint) -
		      (int)otherPoint.RelativeOrientationTo(currentPoint)) % 2) == 0) {
			chunk = PutSraightTwoWayChunk(currentPoint, parentPoint, otherPoint, chamberProbability);
		} else {
			chunk = PutCornerTwoWayChunk(currentPoint, parentPoint, otherPoint, chamberProbability);
	    }
		return chunk;
	}

	Chunk PutSraightTwoWayChunk (Point currentPoint, Point parentPoint, Point otherPoint, double chamberProbability) {
		Chunk chunk = InstantiateChunk(twoWayCorridorPrefab, twoWayChamberPrefab, chamberProbability); 
		if (parentPoint.RelativeOrientationTo(currentPoint) == Orientation.SOUTH ||
		    parentPoint.RelativeOrientationTo(currentPoint) == Orientation.NORTH) {
			chunk.Rotate(Orientation.NORTH);						
		} else {
			chunk.Rotate(Orientation.EAST);
		}
		Debug.Log ("Putting TwoWayChunk at " + currentPoint + " with orientation towards " + chunk.chunkOrientation);
		return chunk;
	}

	Chunk PutCornerTwoWayChunk (Point currentPoint, Point parentPoint, Point otherPoint, double chamberProbability) {
		Chunk chunk = InstantiateChunk(cornerPrefab, cornerChamberPrefab, chamberProbability);
		if ((parentPoint.RelativeOrientationTo(currentPoint) == Orientation.NORTH &&
		     otherPoint.RelativeOrientationTo(currentPoint) == Orientation.EAST) ||
		    (parentPoint.RelativeOrientationTo(currentPoint) == Orientation.EAST &&
		 otherPoint.RelativeOrientationTo(currentPoint) == Orientation.NORTH)) {
			chunk.Rotate(Orientation.NORTH);
		} else if ((parentPoint.RelativeOrientationTo(currentPoint) == Orientation.EAST &&
		            otherPoint.RelativeOrientationTo(currentPoint) == Orientation.SOUTH) ||
		           (parentPoint.RelativeOrientationTo(currentPoint) == Orientation.SOUTH &&
		 otherPoint.RelativeOrientationTo(currentPoint) == Orientation.EAST)) {
			chunk.Rotate(Orientation.EAST);
		} else if ((parentPoint.RelativeOrientationTo(currentPoint) == Orientation.SOUTH &&
		            otherPoint.RelativeOrientationTo(currentPoint) == Orientation.WEST) ||
		           (parentPoint.RelativeOrientationTo(currentPoint) == Orientation.WEST &&
		 otherPoint.RelativeOrientationTo(currentPoint) == Orientation.SOUTH)) {
			chunk.Rotate(Orientation.SOUTH);
		} else if ((parentPoint.RelativeOrientationTo(currentPoint) == Orientation.WEST &&
		            otherPoint.RelativeOrientationTo(currentPoint) == Orientation.NORTH) ||
		           (parentPoint.RelativeOrientationTo(currentPoint) == Orientation.NORTH &&
		 otherPoint.RelativeOrientationTo(currentPoint) == Orientation.WEST)) {
			chunk.Rotate(Orientation.WEST);
		}
		Debug.Log ("Putting CornerChunk at " + currentPoint + " with orientation towards " + chunk.chunkOrientation);
		return chunk;
	}

	Chunk PutThreeWayChunk (Point currentPoint, Point parentPoint, Point otherPoint1, Point otherPoint2, double chamberProbability) {
		Chunk chunk = InstantiateChunk(threeWayCorridorPrefab, threeWayChamberPrefab, chamberProbability);
		var orientationSum = (int)parentPoint.RelativeOrientationTo(currentPoint) +
			(int)otherPoint1.RelativeOrientationTo(currentPoint) + (int)otherPoint2.RelativeOrientationTo(currentPoint);
		if (orientationSum % 2 == 0) {
			if (parentPoint.RelativeOrientationTo(currentPoint) == Orientation.NORTH ||
			    otherPoint1.RelativeOrientationTo(currentPoint) == Orientation.NORTH ||
			    otherPoint2.RelativeOrientationTo(currentPoint) == Orientation.NORTH) {
				chunk.Rotate(Orientation.NORTH);
			} else {
				chunk.Rotate(Orientation.SOUTH);
			}
		} else {
			if (parentPoint.RelativeOrientationTo(currentPoint) == Orientation.EAST ||
			    otherPoint1.RelativeOrientationTo(currentPoint) == Orientation.EAST ||
			    otherPoint2.RelativeOrientationTo(currentPoint) == Orientation.EAST) {
				chunk.Rotate(Orientation.EAST);
			} else {
				chunk.Rotate(Orientation.WEST);
			}
		}
		Debug.Log ("Putting ThreeWayChunk at " + currentPoint + " with orientation towards " + chunk.chunkOrientation);
		return chunk;
	}

	Chunk PutFourWayChunk(double chamberProbability) {
		Debug.Log ("Putting FourWayChunk");
		return InstantiateChunk(fourWayCorridorPrefab, fourWayChamberPrefab, chamberProbability);
	}

	Chunk InstantiateChunk(Chunk corridorChunk, Chunk chamberChunk, double chamberProbability) {
		if (random.NextDouble() < chamberProbability) {
			return Instantiate(chamberChunk) as Chunk;
		} else {
			return Instantiate(corridorChunk) as Chunk;
		}
	}

	Point GetNextPositionOnTrivialPath (Point parentPoint, Point currentPoint) {
		if (parentPoint.X == currentPoint.X) {
			if (parentPoint.Y < currentPoint.Y) {
				return new Point(currentPoint.X, currentPoint.Y + 1);
			} else {
				return new Point(currentPoint.X, currentPoint.Y - 1);
			}
		} else {
			if (parentPoint.X < currentPoint.X) {
				return new Point(currentPoint.X + 1, currentPoint.Y);
			} else {
				return new Point(currentPoint.X - 1, currentPoint.Y);
			}
		}
	}

	List<Point> GetRamificationPoints (Point center, int maxSize) {
		var ramificationPoints = new List<Point>();
		var xs = new int[] {1, 0, -1, 0};
		var ys = new int[] {0, 1, 0, -1};
		Debug.Log ("Checking Ramification Points for center: " + center);
		for (int i = 0; i < xs.Length; i++) {
			var point = new Point(center.X + xs[i], center.Y + ys[i]);
			if (UsablePoint(point, maxSize)) {
				Debug.Log ("Point " + point + " accepted");
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
			//c.enabled = true;
			c.gameObject.SetActive(true);
			if (p.Equals(startPoint)) {
				c.isStartChunk = true;
			}
			if (p.Equals(endPoint)) {
				c.isEndChunk = true;
			}
		}
	}
}
