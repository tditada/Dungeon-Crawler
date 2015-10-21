using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public float timer = 300; 
	public Text guiText;
	
	public void Update()
	{	
		timer -= Time.deltaTime; 		
		if (timer > 0)
		{
			string minutes = Mathf.Floor(timer / 60).ToString("00");
			string seconds = (timer % 60).ToString("00");
			guiText.text = minutes +":"+ seconds;
		} 
		else 
		{
			guiText.text = "TIME OVER\nPress X to restart"; 			
			if (Input.GetKeyDown("x")) 
			{ 
				Application.LoadLevel(Application.loadedLevel); // reload the same level
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
}
