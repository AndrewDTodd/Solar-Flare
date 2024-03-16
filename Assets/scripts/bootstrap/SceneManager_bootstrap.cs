using UnityEngine;
using System.Collections;

public class SceneManager_bootstrap : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		Invoke ("LoadMainMenu", 3.0f);

		if (GameManager.instance) 
		{
			GameManager.instance.PlayMusic (0, 5.0f);
		}
	}

	private void LoadMainMenu ()
	{
		Application.LoadLevel("mainmenu");
	}
}
