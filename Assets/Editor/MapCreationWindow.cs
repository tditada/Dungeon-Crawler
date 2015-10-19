using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MapCreationWindow : EditorWindow
{	
	int seed = 0;
	int maxSize = 10;
	int minSteps = 1;
	int maxSteps = 10;
	MapGenerator mapGenerator;

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

		if (GUILayout.Button ("Create new map!")) {
			Dictionary<Point, Chunk> map = mapGenerator.NewMap(maxSize, minSteps, maxSteps, seed);
		}
	}
}
