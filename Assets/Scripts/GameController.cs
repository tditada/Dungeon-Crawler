using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public void Pause(){
		if (Time.timeScale==1) {
			Time.timeScale = 0;
//			uiController.PauseMenu(true);
		} else {
			Time.timeScale =1;
//			uiController.PauseMenu(false);
		}
	}
}
