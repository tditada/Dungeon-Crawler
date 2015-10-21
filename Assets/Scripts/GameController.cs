using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public float timer = 300; 
	public Text guiText;
	public bool end = false;
	private Mecanim_Control_melee player;

	public void Start(){
		GameObject controllerObject = GameObject.FindGameObjectWithTag ("Player");
		player = controllerObject.GetComponent<Mecanim_Control_melee> ();
	}

	public void Update()
	{	
		timer -= Time.deltaTime; 		
		if (timer > 0 && !end) {
			string minutes = Mathf.Floor (timer / 60).ToString ("00");
			string seconds = (timer % 60).ToString ("00");
			guiText.text = minutes + ":" + seconds;
		} else if (!end) {
			guiText.text = "TIME OVER\nPress X to restart"; 	
			player.canControl = false;
			Pause ();
			end = true;
		} else {
			if (Input.GetKeyDown ("x")) { 
				Application.LoadLevel (Application.loadedLevel); // reload the same level
			}
		}
	}

	public void Pause(){
		if (Time.timeScale==1) {
			Time.timeScale = 0;
//			uiController.PauseMenu(true);
		} else {
			Time.timeScale =1;
//			uiController.PauseMenu(false);
		}
	}

	public void onClickMainMenu(){
		Application.LoadLevel (0);
	}

	public void Win(){
		end = true;
		player.canControl = false;
		Pause ();
		guiText.text = "You Won!\n Press X to start a new game";
	}

	public void Lose(){
		end = true;
		player.canControl = false;
		Pause ();
		guiText.text = "You Lose\n Press X to start a new game";
	}

}
