using UnityEditor;
using UnityEngine;

public class MapCreationWindow : EditorWindow
{	
	private int rooms=1;
	MapGeneration mapScript;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/MapCreation")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(MapCreationWindow));
	}
	
	void OnGUI()
	{	
		GameObject go = GameObject.FindGameObjectWithTag ("generator");
		mapScript = go.GetComponent<MapGeneration> ();

		GUILayout.Label ("Map Variables", EditorStyles.boldLabel);
		rooms = EditorGUILayout.IntField ("number of rooms", rooms);

		if (GUILayout.Button ("Create new map!")) {
			mapScript.newMap(rooms);
		}


	}
}
