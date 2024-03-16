using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager_HighScoreEntry : MonoBehaviour
{
    private HighScore currentScore = null;

    [SerializeField]
    private TMPro.TMP_InputField nameInput = null;
    [SerializeField]
    private TMPro.TextMeshProUGUI scoreText = null;
    [SerializeField]
    private TMPro.TextMeshProUGUI timeText = null;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentScore = GameManager.instance.CurrentPlayerScore;

        if(scoreText)
        {
            scoreText.text = currentScore.Score.ToString();
        }
        if(timeText) 
        {
            timeText.text = currentScore.TimeStamp.ToString();
        }
    }

    public void OnConfirmNameButtonClicked()
    {
        if (currentScore != null) 
        {
            if(!string.IsNullOrEmpty(nameInput.text))
            {
                currentScore.Name = nameInput.text;
            }
            else
            {
                currentScore.Name = "___";
            }
        }

        GameManager.instance.SaveHighScores();

        SceneManager.LoadScene("mainmenu");
    }
}
