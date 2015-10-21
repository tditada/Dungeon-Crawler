using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

	public Button play;
	public Button quit;
	public Button instructions;
	public Text instructionText;
	public Button back;

	public void onClickStart(){
		Debug.Log ("clic");
		Application.LoadLevel (2);
	}

	public void onClickInstructions(){
		play.gameObject.SetActive (false);
		quit.gameObject.SetActive (false);
		instructions.gameObject.SetActive (false);
		instructionText.gameObject.SetActive (true);
		back.gameObject.SetActive (true);
	}

	public void onClickQuit(){
		Application.Quit ();
	}

	public void onClickBack(){
		Debug.Log ("clic");
		play.gameObject.SetActive (true);
		quit.gameObject.SetActive (true);
		instructions.gameObject.SetActive (true);
		instructionText.gameObject.SetActive (false);
		back.gameObject.SetActive (false);
	}
}
