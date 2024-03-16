using UnityEngine;
using System.Collections;

public class scenemanager_mainmenu : MonoBehaviour 
{
	public GameObject screen1 = null;
	public GameObject screen2 = null;

	private CameraFade _camerafade = null;

	private float _timer = 0.0f;
	private bool _quiting = false;

	//singleton class
	private static scenemanager_mainmenu _instance = null;
	public static scenemanager_mainmenu instance
	{

		get
		{
			if (_instance == null)
			{
				_instance = (scenemanager_mainmenu) FindObjectOfType (typeof(scenemanager_mainmenu));
			}

			return _instance;
		}
	}

	//start function
	void Start()
	{
		if (screen1)
						screen1.SetActive (true);
		if (screen2)
						screen2.SetActive (false);

		_camerafade = FindObjectOfType<CameraFade> ();
		if (_camerafade)
						_camerafade.faidIn (1.5f);

		Cursor.visible = false;
	}

	//calls next screen in scene
	public void NextScreen()
	{
		if (screen1)
						screen1.SetActive (!screen1.activeSelf);
		if (screen2)
						screen2.SetActive (!screen1.activeSelf);
	}

	void Update()
	{
		if (_quiting)
						return;
		_timer += Time.deltaTime;

		if (_timer < 1.0f)
						Input.ResetInputAxes ();

		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			_quiting = true;

			if (GameManager.instance)
				GameManager.instance.StopMusic(2);

			if (_camerafade)
				_camerafade.faidOut(3.5f);

			Invoke("QuitMenu", 2.2f);
		}

		else
			if (Input.GetKeyDown (KeyCode.Space))
		{
			if (GameManager.instance)
				GameManager.instance.StartNewGame();
		}
	}

	void QuitMenu()
	{
		if (GameManager.instance)
						GameManager.instance.QuitGame ();
	}
}
