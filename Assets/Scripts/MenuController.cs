using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

	public Button play;
	public Button quit;
	public Button instructions;
	public Text instructionText;
	public Button back;
	public Button hard;
	public Button medium;
	public Button easy;

	public void onClickMedium(){		
		Application.LoadLevel (2);
	}

	public void onClickHard(){		
		Application.LoadLevel (3);
	}

	public void onClickEasy(){
		Application.LoadLevel (1);
	}

	public void onClickPlay(){
		play.gameObject.SetActive (false);
		quit.gameObject.SetActive (false);
		instructions.gameObject.SetActive (false);
		back.gameObject.SetActive (true);
		easy.gameObject.SetActive (true);
		medium.gameObject.SetActive (true);
		hard.gameObject.SetActive (true);
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
		play.gameObject.SetActive (true);
		quit.gameObject.SetActive (true);
		instructions.gameObject.SetActive (true);
		instructionText.gameObject.SetActive (false);
		back.gameObject.SetActive (false);
		easy.gameObject.SetActive (false);
		medium.gameObject.SetActive (false);
		hard.gameObject.SetActive (false);
	}
}
