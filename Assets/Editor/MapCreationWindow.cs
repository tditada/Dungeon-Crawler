using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MapCreationWindow : EditorWindow
{	
	int seed = 0;
	int maxSize = 10;
	int minSteps = 1;
	int maxSteps = 10;
	double ramificationProbability = 0.5;
	double chamberProbability = 0.2;
	MapGenerator mapGenerator;
	//TwoWayCorridorChunk twoWayCorridorPrefab;
	//ThreeWayCorridorChunk threeWayCorridorPrefab;
	//FourWayCorridorChunk fourWayCorridorPrefab;
	//CornerChunk cornerPrefab;
	//ChamberChunk chamberPrefab;
	//CornerChamberChunk cornerChamberPrefab;
	//TwoWayChamberChunk twoWayChamberPrefab;
	//ThreeWayChamberChunk threeWayChamberPrefab;
	//FourWayChamberChunk fourWayChamberPrefab;
	Chunk twoWayCorridorPrefab;
	Chunk threeWayCorridorPrefab;
	Chunk fourWayCorridorPrefab;
	Chunk cornerPrefab;
	Chunk chamberPrefab;
	Chunk cornerChamberPrefab;
	Chunk twoWayChamberPrefab;
	Chunk threeWayChamberPrefab;
	Chunk fourWayChamberPrefab;
	double chunkSize = 34.8;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/MapCreation")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(MapCreationWindow));
	}
	
	void OnGUI()
	{	
		GUILayout.Label ("Map Generator Variables", EditorStyles.boldLabel);

		mapGenerator = EditorGUILayout.ObjectField("Map Script Algorithm: ", mapGenerator, typeof(MapGenerator), true) as MapGenerator;
		seed = EditorGUILayout.IntField ("Seed: ", seed);
		maxSize = EditorGUILayout.IntField("Maximum Map Size: ", maxSize);
		minSteps = EditorGUILayout.IntField("Minimum Steps to Exit: ", minSteps);
		maxSteps = EditorGUILayout.IntField("Maximum Steps to Exit: ", maxSteps);
		ramificationProbability = EditorGUILayout.DoubleField("Ramification Probability: ", ramificationProbability);
		chamberProbability = EditorGUILayout.DoubleField("Chamber Appearance Probability: ", chamberProbability);
		//twoWayCorridorPrefab = EditorGUILayout.ObjectField("Two Way Corridor Prefab: ", twoWayCorridorPrefab, typeof(TwoWayCorridorChunk), true) as TwoWayCorridorChunk;
		//threeWayCorridorPrefab = EditorGUILayout.ObjectField("ThreeWayCorridorPrefab: ", threeWayCorridorPrefab, typeof(ThreeWayCorridorChunk), true) as ThreeWayCorridorChunk;
		//fourWayCorridorPrefab = EditorGUILayout.ObjectField("FourWayCorridorPrefab: ", fourWayCorridorPrefab, typeof(FourWayCorridorChunk), true) as FourWayCorridorChunk;
		//cornerPrefab = EditorGUILayout.ObjectField("Corner Corridor Prefab: ", cornerPrefab, typeof(CornerChunk), true) as CornerChunk;
		//chamberPrefab = EditorGUILayout.ObjectField("Simple Chamber Prefab: ", chamberPrefab, typeof(ChamberChunk), true) as ChamberChunk;
		//cornerChamberPrefab = EditorGUILayout.ObjectField("Corner Chamber Prefab: ", cornerChamberPrefab, typeof(CornerChamberChunk), true) as CornerChamberChunk;
		//twoWayChamberPrefab = EditorGUILayout.ObjectField("Two Way Chamber Prefab: ", twoWayChamberPrefab, typeof(TwoWayChamberChunk), true) as TwoWayChamberChunk;
		//threeWayChamberPrefab = EditorGUILayout.ObjectField("Three Way Chamber Prefab: ", threeWayChamberPrefab, typeof(ThreeWayChamberChunk), true) as ThreeWayChamberChunk;
		//fourWayChamberPrefab = EditorGUILayout.ObjectField("Four Way Chamber Prefab: ", fourWayChamberPrefab, typeof(FourWayChamberChunk), true) as FourWayChamberChunk;
		twoWayCorridorPrefab = EditorGUILayout.ObjectField("Two Way Corridor Prefab: ", twoWayCorridorPrefab, typeof(TwoWayCorridorChunk), true) as Chunk;
		threeWayCorridorPrefab = EditorGUILayout.ObjectField("ThreeWayCorridorPrefab: ", threeWayCorridorPrefab, typeof(ThreeWayCorridorChunk), true) as Chunk;
		fourWayCorridorPrefab = EditorGUILayout.ObjectField("FourWayCorridorPrefab: ", fourWayCorridorPrefab, typeof(FourWayCorridorChunk), true) as Chunk;
		cornerPrefab = EditorGUILayout.ObjectField("Corner Corridor Prefab: ", cornerPrefab, typeof(CornerChunk), true) as Chunk;
		chamberPrefab = EditorGUILayout.ObjectField("Simple Chamber Prefab: ", chamberPrefab, typeof(ChamberChunk), true) as Chunk;
		cornerChamberPrefab = EditorGUILayout.ObjectField("Corner Chamber Prefab: ", cornerChamberPrefab, typeof(CornerChamberChunk), true) as Chunk;
		twoWayChamberPrefab = EditorGUILayout.ObjectField("Two Way Chamber Prefab: ", twoWayChamberPrefab, typeof(TwoWayChamberChunk), true) as Chunk;
		threeWayChamberPrefab = EditorGUILayout.ObjectField("Three Way Chamber Prefab: ", threeWayChamberPrefab, typeof(ThreeWayChamberChunk), true) as Chunk;
		fourWayChamberPrefab = EditorGUILayout.ObjectField("Four Way Chamber Prefab: ", fourWayChamberPrefab, typeof(FourWayChamberChunk), true) as Chunk;


		if (GUILayout.Button ("Create new map!")) {
			mapGenerator.twoWayCorridorPrefab = twoWayCorridorPrefab;
			mapGenerator.threeWayCorridorPrefab = threeWayCorridorPrefab;
			mapGenerator.fourWayCorridorPrefab = fourWayCorridorPrefab;
			mapGenerator.cornerPrefab = cornerPrefab;
			mapGenerator.chamberPrefab = chamberPrefab;
			mapGenerator.cornerChamberPrefab = cornerChamberPrefab;
			mapGenerator.twoWayChamberPrefab = twoWayChamberPrefab;
			mapGenerator.threeWayChamberPrefab = threeWayChamberPrefab;
			mapGenerator.fourWayChamberPrefab = fourWayChamberPrefab;
			
			mapGenerator.chamberProbability = chamberProbability;
			mapGenerator.ramificationProbability = ramificationProbability;
			Dictionary<Point, Chunk> map = mapGenerator.NewMap(maxSize, minSteps, maxSteps, seed);
		}
	}
}
