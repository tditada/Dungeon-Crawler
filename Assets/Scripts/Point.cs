using System;
public class Point {
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