using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
	public TextMesh CurrentScore;
	public TextMesh HiScore;
	public TextMesh Panel_line1;
	public TextMesh Panel_line2;

	public GameObject Panel;
	private Material PanelMaterial;

	private GameManager _gameManager = null;
	private GameObject _myGameObject;

	private string _highScoreName = null;
	private uint _highScore = 0;

	void Start()
	{
		_myGameObject = gameObject;
		_gameManager = GameManager.instance;

		if (Panel != null && Panel.GetComponent<Renderer>() != null)
		{
			PanelMaterial = Panel.GetComponent<Renderer>().material;
		}

		if (_gameManager != null) 
		{
			_gameManager.GetHigestScore(out _highScoreName, out _highScore);
		}
		if (HiScore != null && _gameManager != null) 
		{
			HiScore.text = "High Score" + "(" + _highScoreName + ")" + " : " + _highScore;
		}

		if (CurrentScore != null && _gameManager != null)
		{
			CurrentScore.text = "Current Score : " + _gameManager.currentScore;
		}

		Reset (false);
	}

	public void Reset (bool active)
	{
		Color col;
		if (PanelMaterial != null) 
		{
			col = PanelMaterial.color;
			col.a = 0.0f;
			PanelMaterial.color = col;
		}

		if (Panel_line1 != null)
		{
			col = Panel_line1.color;
			col.a = 0.0f;
			Panel_line1.color = col;
		}

		if (Panel_line2 != null) 
		{
			col = Panel_line2.color;
			col.a = 0.0f;
			Panel_line2.color = col;
		}

		if (_myGameObject != null) 
		{
			_myGameObject.SetActive (active);
		}

		if (_gameManager && HiScore != null && _gameManager.currentScore > _highScore) 
		{
			HiScore.color = Color.red;
		}
	}

	public void UpdateScore()
	{
		if (_gameManager != null) 
		{
			if (CurrentScore != null)
			{
				CurrentScore.text = "Current Score : " + _gameManager.currentScore;
			}

			if (HiScore != null && _gameManager.currentScore > _highScore)
			{
				HiScore.color = Color.red;
			}
		}
	}

	public void SetPanelText (string line1, string line2)
	{
		if (Panel_line1) 
		{
			Panel_line1.text = line1;
		}
		if (Panel_line2) 
		{
			Panel_line2.text = line2;
		}
	}

	public IEnumerator FadeIn (float duration)
	{
		if (PanelMaterial && Panel_line1 && Panel_line2)
		{
			Reset (true);

			float timer = 0;
			Color panelColor = PanelMaterial.color;
			Color panelLine1Color = Panel_line1.color;
			Color panelLine2Color = Panel_line2.color;

			if (duration < 0.0001f)
			{
				duration = 1.0f;
				timer = duration - 0.0001f;
			}

			while (timer < duration)
			{
				timer += Time.deltaTime;


				panelColor.a = (0.92f / duration) * timer;
				panelLine1Color.a = (1.0f / duration) * timer;
				panelLine2Color.a = (1.0f / duration) * timer;

				PanelMaterial.color = panelColor;
				Panel_line1.color = panelLine1Color;
				Panel_line2.color = panelLine2Color;

				yield return null;
			}
		}
	}
}

