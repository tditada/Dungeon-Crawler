using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public TwoWayCorridorChunk twoWayCorridorPrefab;
	public ThreeWayCorridorChunk threeWayCorridorPrefab;
	public FourWayCorridorChunk fourWayCorridorPrefab;
	public CornerChunk cornerPrefab;
	public ChamberChunk chamberPrefab;
	public CornerChamberChunk cornerChamberPrefab;
	public TwoWayChamberChunk twoWayChamberPrefab;
	public ThreeWayChamberChunk threeWayChamberChunk;
	public FourWayChamberChunk fourWayChamberChunk;

	private Dictionary<Point, Chunk> map = new Dictionary<Point, Chunk>();
	private Point startPoint;
	private Point endPoint;
	private System.Random random;

	public void newMap(int maxSize, int minSteps, int maxSteps, int seed, double ramificationProbability) {
		if (minSteps < maxSteps) { throw new ArgumentException(); }
		if (minSteps > maxSize) { throw new ArgumentException(); }
		if (maxSteps > maxSize) { throw new ArgumentException(); }
		if (ramificationProbability < 0.0 || ramificationProbability > 1.0) { throw new ArgumentException(); }

		startPoint = new Point(random.Next(0, maxSize), random.Next(0, maxSize));
		endPoint = GetEndPoint(startPoint, minSteps, maxSteps, maxSize);

		if (endPoint == null) {
			return;
		}

		map.Add(startPoint, Instantiate(chamberPrefab) as Chunk);
		map.Add(endPoint, Instantiate(chamberPrefab) as Chunk);

		GenerateTrivialPath(startPoint, endPoint);

		Ramify(ramificationProbability, maxSize);
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

	void Ramify (double ramificationProbability, int maxSize) {
		foreach (var chunk in map) {
			RamifyChunk(null, chunk.Key, ramificationProbability, maxSize);
		}
	}

	bool RamifyChunk (Point previousPosition, Point currentPosition, double ramificationProbability, int maxSize) {
		if (!map.ContainsKey(currentPosition) && random.NextDouble() < ramificationProbability) {
			var ramificationPoints = GetRamificationPoints(currentPosition, maxSize);
			for (int i = 0; i < ramificationPoints.Count; i++) {
				map.Add(ramificationPoints[i], null);
				if (!RamifyChunk(currentPosition, ramificationPoints[i], ramificationProbability, maxSize)) {
					ramificationPoints.Remove(ramificationPoints[i]);
					i--;
				}
			}
			DecideChunk(previousPosition, currentPosition, ramificationPoints);
			return true;
		}
		return false;
	}

	void DecideChunk (Point previousPosition, Point currentPosition, List<Point> ramificationPoints) {
		var chunk = map[currentPosition];
		switch (ramificationPoints.Count) {
		case 0:
			if (chunk == null) {
				chunk = PutChamber(previousPosition, currentPosition);
			}
			break;
		case 1:
			if (chunk == null) {
				chunk = PutTwoWayChunk(currentPosition, previousPosition, ramificationPoints[0]);
			} else {
				Destroy(chunk);
				chunk = PutThreeWayChunk(currentPosition, previousPosition, ramificationPoints[0], ramificationPoints[1]);
				chunk = Instantiate(threeWayCorridorPrefab) as Chunk;
			}
			break;
		case 2:
			if (chunk == null) {
				chunk = PutThreeWayChunk(currentPosition, previousPosition, ramificationPoints[0], ramificationPoints[1]);
				chunk = Instantiate(threeWayCorridorPrefab) as Chunk;
			} else {
				Destroy(chunk);
				chunk = Instantiate(fourWayCorridorPrefab) as Chunk;
			}
			break;
		case 3:
			if (chunk != null) {
				Destroy(chunk);
			}
			chunk = Instantiate(fourWayCorridorPrefab) as Chunk;
			break;
		}
		map[currentPosition] = chunk;
	}

	Chunk PutChamber (Point previousPosition, Point currentPosition) {
		var chunk = Instantiate(chamberPrefab) as Chunk;
		chunk.Rotate(previousPosition.RelativeOrientationTo(currentPosition));
		return chunk;
	}

	Chunk PutTwoWayChunk (Point currentPosition, Point previousPosition, Point other) {
		Chunk chunk = null;
		if ((((int)previousPosition.RelativeOrientationTo(currentPosition) -
		      (int)other.RelativeOrientationTo(currentPosition)) % 2) == 0) {
			chunk = PutSraightTwoWayChunk(currentPosition, previousPosition, other);
		} else {
			chunk = PutCornerTwoWayChunk(currentPosition, previousPosition, other);
	    }
		return chunk;
	}

	Chunk PutSraightTwoWayChunk (Point currentPosition, Point previousPosition, Point other) {
		Chunk chunk = Instantiate(twoWayCorridorPrefab) as Chunk;
		if (previousPosition.RelativeOrientationTo(currentPosition) == Orientation.SOUTH ||
		    previousPosition.RelativeOrientationTo(currentPosition) == Orientation.NORTH) {
			chunk.Rotate(Orientation.NORTH);						
		} else {
			chunk.Rotate(Orientation.EAST);
		}
		return chunk;
	}

	Chunk PutCornerTwoWayChunk (Point currentPosition, Point previousPosition, Point other) {
		Chunk chunk = Instantiate(cornerPrefab) as Chunk;
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

	Chunk PutThreeWayChunk (Point currentPosition, Point previousPosition, Point other1, Point other2) {
		Chunk chunk = Instantiate(threeWayCorridorPrefab) as Chunk;
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

		/**
		 * This point position relative to the @other point given.
		 * i.e.: If this point is at (1,0) and the @other point is at
		 * (2,0), then the expected result is NORTH, since this object is 
		 * at the NORTH to the @other point. 
		 * This method will return a value iff the points are colinears in 
		 * some way. That means that they must be either (X1, Y) and (X2, Y)
		 * or (X, Y1) and (X, Y2), but cannot be (X1, Y1) and (X2, Y2), where
		 * X1 != X2 and Y1 != Y2.
		 */

		public Orientation RelativeOrientationTo (Point other) {
			if (other == null) { throw new ArgumentNullException(); }
			if (other.X == this.X) {
				if (this.Y < other.Y) {
					return Orientation.NORTH;
				} else {
					return Orientation.SOUTH;
				}
			} else if (other.Y == this.Y) {
				if (this.X < other.X) {
					return Orientation.WEST;
				} else {
					return Orientation.EAST;
				}
			}
			throw new NotSupportedException();
		}
	}
}
