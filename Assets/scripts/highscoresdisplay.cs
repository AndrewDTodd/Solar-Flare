using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

//desplayes the high score data
public class highscoresdisplay : MonoBehaviour 
{
	//reference to the name object of the high score
	public TextMesh name = null;

	//reference to the score of the high score
	public TextMesh score = null;

	//time that the high scores screen will be desplayed 
	public float DisplayTime = 6.0f;

	//internal storage for a timer to the displaytime var
	public float _timer = 0.0f;

	void OnEnable()
	{
		//set timer to 0
		_timer = 0.0f;

		//cache a reference to  the gamemanager
		GameManager gameManager = GameManager.instance;


		//can only procede if the gamemanager is valid
		if (gameManager != null) 
		{

			//colect the list of high scores
			List<HighScore> highScores = gameManager.GetHighScores();

			//ensure that the gamemanager returned data
			if (highScores != null)
			{
				//allocate two string builder objects
				StringBuilder namesText = new StringBuilder();
				StringBuilder scoreText = new StringBuilder();

				//loop through up to eight high scores returned by the gamemanager
				for (int i = 0; i < Mathf.Min (highScores.Count,8) ; i++)
				{
					//collect the current high score to process
					HighScore hse = highScores[i];

					//check that the name and score are valid
					if (hse != null && hse.Name != null)
					{
						//add name to the nametext 
						namesText.Append(hse.Name).Append ("\n");

						//add score to the scoretext
						scoreText.Append(hse.Score).Append ("\n");
					}
				}

				//if there is a valid reference to a text mesh
				if (name != null)
				{
					//converth the sring builder to a sting and set the names text to the string
					name.text = namesText.ToString();

				}

				//set the score in the string builder to the score text
				if (score != null)
				{
					score.text = scoreText.ToString();
				}
			}
		}
	}

	// update function that counts the desplay time
	void Update()
	{
		//increase timer
		_timer += Time.deltaTime;

		//if we are at the main menu we need to change back to the other screen after 6 seconds
		if (scenemanager_mainmenu.instance != null) 
		{
			//if 6 sec is past switch screens
			if (_timer > DisplayTime)
			{
				scenemanager_mainmenu.instance.NextScreen();
			}
		}
	}
}
