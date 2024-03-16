using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEditor;
using UnityEngine.SceneManagement;

//class containing info about game levels
[System.Serializable]
public class levelInfo
{
	//name of level
	public string Name = null;

	//delay between ailien movement
	public float StartMoveDelay = 55.0f;

	//how much the delay is decremented after each killed alien
	public float MoveDelayDecrement = 1.0f;

	//speed per update of aliens on the x axis
	public float HorizStartSpeed = 2.5f;

	//factor that deturmins by what rate horizontal speed is increased after each killed alien
	public float HorizSpeedMultiplyer = 1.0f;

	//virtical desention speed
	public float VirticalSpeed = 8.0f;

	//persentage chance that an alien will fire upon update
	public int FireChance = 25;

	//max num of alien projectiles to be fired at one time
	public int MaxAlienProjectiles = 2;

	//chance that mystery invader will be spawn on this update
	public int MysteryInvaderChance = 5;

	//min delay between alien movement
	public float MoveDelayClamp = 1.0f;


}

//high score table
[System.Serializable]
public class HighScore
{
	//name of player
	public string Name = null;

	//score of player
	public uint Score = 0;

	//temp marker for playing character score
	public bool Marker = false;

	//time stamp of score
	public long TimeStamp = 0;
}

public class GameManager : MonoBehaviour
{
	//list of game levels
	[SerializeField]
	private List<levelInfo> levels = new List<levelInfo> (); 

	//high score table list object
	[SerializeField]
	private List<HighScore> HighScores = new List<HighScore> ();

	private HighScore _currentPlayerScore = null;
	public ref HighScore CurrentPlayerScore
	{
		get => ref _currentPlayerScore;
	}

	//list of Audio clips
	[SerializeField]
	private List<AudioClip>	MusicClips = new List<AudioClip>();

	//used to cache audio source component
	private AudioSource _music = null;

	//current audio clip playing, by index
	private int _currentClip = -1;

	//lives
	private int _lives = 3;
	public int lives
	{
		get
		{
			return _lives;
		}
	}
	//score
	private int _currentScore  = 0;
	public int currentScore
	{
		get 
		{
				return _currentScore;
		}
	}
	//level
	[SerializeField]
	private int _level = 0;
	public int level 
	{
		get 
		{
			return _level;
		}
	}

	//list of acsess functions to the level details
	public string CurrentLevelName 				{get {return levels[_level].Name;}}
	public float CurrentStartMoveDelay    		{get {return levels[_level].StartMoveDelay;}}
	public float CurrentMoveDelayDecrement		{get {return levels[_level].MoveDelayDecrement;}}
	public float CurrentHorizStartSpeed			{get {return levels[_level].HorizStartSpeed;}}
	public float CurrentHorizSpeedMultiplyer	{get {return levels[_level].HorizSpeedMultiplyer;}}
	public float CurrentVerticalSpeed			{get {return levels[_level].VirticalSpeed;}}
	public int CurrentFireChance				{get {return levels[_level].FireChance;}}
	public int CurrentMaxAlienProjectiles		{get {return levels[_level].MaxAlienProjectiles;}}
	public int CurrentMysteryInvaderChance		{get {return levels[_level].MysteryInvaderChance;}}
	public float CurrentMoveDelayClamp			{get {return levels[_level].MoveDelayClamp;}}

	//used to store the pathway to the high score table on disk
	private string _HighScorePath;

	//signiture of the high score table that will verify its the right file when being loaded
	private string _FileSigniture = "High Score Table Verification";

	//singleton game manager object
	private static GameManager _instance = null;
	public static GameManager instance 
	{

		get
		{
			if (_instance == null)
			{

				_instance = (GameManager)FindObjectOfType(typeof(GameManager));
			}
			return _instance;
		}
	}

	void Awake()
	{
		//app global object that will not be destroyed from load to load
		DontDestroyOnLoad(gameObject);

		//inti game data
		_lives = 3;
		_currentScore = 0;
		_level = 0;

		//pathway of the high scores table
		_HighScorePath = Application.dataPath + "/HighScoreTable";

		//loads high score table
		LoadHighScores ();
	}

	//start function that starts music
	void Start()
	{
		//cache the audio
		_music = GetComponent<AudioSource>();
	
		//if an audio source is active
		if (_music) 
		{
			//ensure that audio will not play on awake
			//volume is set to 0
			//audio is inactive
			_music.playOnAwake = false;
			_music.volume = 0;
			_music.Stop();
		}

	}

    void LateUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
			EditorApplication.isPlaying = false;
        }
#else
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

#endif
    }

    //loads High Score Table or creates it if it dousnt exist
    public void LoadHighScores()
	{
		//check to see if the path exists
		bool DirectoryExists = Directory.Exists(_HighScorePath);

		//check to see if the file exists
		bool FileExists = File.Exists(_HighScorePath+"/HighScoreTable.dat");

		if (!DirectoryExists || !FileExists) 
		{
			//creates new directory 
			if (!DirectoryExists) 
			{
				Directory.CreateDirectory(_HighScorePath);
			}

			//creates new table
			if (!FileExists)
			{
				CreateNewHighScoreTable("HighScoreTable.dat");
			}
		}

		//directory and file exist
		LoadHighScores("HighScoreTable.dat");

	}

	//make the high score table for the first time
	void CreateNewHighScoreTable (string FileName)
	{
		//create time stamp 
		DateTime Epoch = new DateTime (1970, 1, 1, 0, 0, 0);
		long timestamp = (long) ((DateTime.UtcNow - Epoch).TotalSeconds);

		using (BinaryWriter writer = new BinaryWriter(File.Open(_HighScorePath + "/" + FileName, FileMode.Create))) 
		{
			//write out the file signiture
			writer.Write (_FileSigniture);

			//write eight high score entries
			for (int i = 0; i < 8 ; i++)
			{
				writer.Write ("---");
				writer.Write (000);

				//write time stamp
				writer.Write(timestamp);


			}
		}
	}

	//saves the high score table
	public void SaveHighScores()
	{
		//nothing to save
		if (HighScores == null || HighScores.Count == 0)
						return;

		//sort
		SortHighScores();

		//stor file pathway and name
		string filename = _HighScorePath + "/HighScoreTable.dat";

		using (BinaryWriter Writer = new BinaryWriter (File.Open(filename, FileMode.Open))) 
		{
			//write signiture
			Writer.Write(_FileSigniture);

			//write high scores
			for(int i = 0; i < 8; i++ )
			{
				//if for some reason there isnt enough scores
				if(i >= HighScores.Count)
				{
					Writer.Write ("---");
					Writer.Write (000);
					Writer.Write ( (long) 0 );
				}

				else
				{
					//fetch next high score
					HighScore hse = HighScores[i];

					//write data to file
					if (hse != null && hse.Name != null)
					{
						Writer.Write(hse.Name);
						Writer.Write(hse.Score);
						Writer.Write (hse.TimeStamp);
					}

					else
					{
						Writer.Write ("---");
						Writer.Write(000);
						Writer.Write ((long)0);
					}

				}
			}
		}
	}

    public IEnumerator GameOverTimer(float time)
    {
        yield return new WaitForSeconds(time);

        GameOver();
    }

	//game over function
	public void GameOver()
	{
		if (_lives == 0)
		{
			//reset level and lives
			_lives = 3;
			_level = 0;
		}
		else
		{
			_level++;
		}

		//start fading in menu
		PlayMusic (0, 3);
		
		//score position on high score table
		int PlayerTablePosition = -1;

		//loop through the scores to test if the player has made a high score
		for (int i = 0; i < HighScores.Count; i++) 
		{
			//get entry to examine
			HighScore hse = HighScores[i];

			if (hse != null)
			{
				//if current score is greater
				if(_currentScore >= hse.Score)
				{
					//this is the score to replase
					PlayerTablePosition = i;

					//stop serching for posible score slot
					break;
				}
			}
		}

		//have we got on the tabe 
		if (PlayerTablePosition != -1) 
		{
			//create timestamp
			DateTime epoch = new DateTime (1970, 1, 1, 0, 0, 0);
			long Timestamp = (long)((DateTime.UtcNow - epoch).TotalSeconds);

			//create new high score
			HighScore nhse = new HighScore();

			nhse.Name = "___";

			nhse.Score = (uint)_currentScore;

			nhse.TimeStamp = Timestamp;

			nhse.Marker = true;

			//insert high score into high score table
			HighScores.Insert(PlayerTablePosition, nhse);

			//remove lowest score
			HighScores.RemoveAt(HighScores.Count-1);

			_currentPlayerScore = nhse;

			//load in high score entry screen
			SceneManager.LoadScene("HighScoreEntry");

		}
		//no high score
		else 
		{
			//Application.LoadLevel("mainmenu");
			SceneManager.LoadScene("mainmenu");
		}
	}

	//loads the high score table
	private bool LoadHighScores (string FileName)
	{
		int i;

		using (BinaryReader reader = new BinaryReader (File.Open(_HighScorePath+"/"+FileName, FileMode.Open)))
		{
			//read signiture
			string signiture = reader.ReadString();

			//if its the wrong signiture its not the right file
			if (signiture != _FileSigniture)
			{
				Debug.Log("Load Error: Invalid High Score File");
				return false;
			}

			//clear resident high scores
			HighScores.Clear ();

			//read new data into list
			for (i = 0; i < 8; i++)
			{
				//create new entry
				HighScore hs = new HighScore();

				hs.Name = reader.ReadString();
				hs.Score = reader.ReadUInt32();
				hs.TimeStamp = reader.ReadInt64();

				HighScores.Add (hs);
			}
		}
		SortHighScores();
		return true;

	}

	//geter function for the high score data
	public List<HighScore> GetHighScores ()
	{
		SortHighScores();
		return HighScores;
	}

	public void GetHigestScore(out string name, out uint score)
	{
		//set for defalt in case of error
		name = "ERR";
		score = 0;

		//if high score table is not working return error
		if (HighScores == null || HighScores.Count < 1 || HighScores [0] == null || HighScores [0].Name == null)
						return;

		name = HighScores [0].Name;
		score = HighScores [0].Score;
	}

	//sorts high scores
	public void SortHighScores()
	{
		HighScores.Sort (SortFunc);
	}

	private int SortFunc(HighScore h1, HighScore h2)
	{
		//if the two scores are equal use timestamp to sourt instead
		if (h2.Score.CompareTo (h1.Score) == 0)
				return h2.TimeStamp.CompareTo (h1.TimeStamp);
		else
				return h2.Score.CompareTo (h1.Score);
	}

	//playes audio clip located at the passed index of the musicClips holder
	public void PlayMusic (int clip, float fade)
	{
		//check to validate that there is audio atached to game object
		if (!_music) 
		{
			return;
		}
		//ensure that the clip index pased into function is valid
		if (MusicClips == null || MusicClips.Count <= clip || MusicClips [clip] == null) 
		{
			return;
		}
		//if clip requested is already playing
		if (_currentClip == clip && _music.isPlaying) 
		{
			return;
		}
		//change _currentClip value to clip
		_currentClip = clip;

		//start corouting to fade in music
		StartCoroutine (FadeInMusic (clip, fade));
	}

	//stops the currently playing audio clip
	public void StopMusic(float fade = 2.0f)
	{
		//if thereis no audio component return
		if (!_music) 
		{
			return;
		}
		//set currentClip var to -1 (no clip)
		_currentClip = -1;

		//fade out the currently playing clip
		StartCoroutine (FadeOutMusic (fade));

	}
	//fades the currently playing audio as it closes
	private IEnumerator FadeOutMusic(float fade)
	{
		//minimum fade time
		if (fade < 1.0f) 
		{
			fade = 1.0f;
		}

		//check that there is and audio scorce than preform fade
		if (_music)
		{
			//force volume to max
			_music.volume = 1.0f;

			//reset deltaTime
			float timer = 0.0f;

			//decrease volume by factor of the deltaTime/fadetime ratio
			while(timer < fade)
			{
				timer += Time.deltaTime;

				_music.volume = 1.0f - (timer/fade);

				yield return null; 
			}

			//set volume to 0, and stop music
			_music.volume = 0.0f;
			_music.Stop();

		}

	}
	//coroutine function that handles the actual fading of the audio
	private IEnumerator FadeInMusic(int clip, float fade)
	{
		//min fade value of 0.1 sec
		if (fade < 0.1f) 
		{
			fade = 0.1f;
		}
		//cant procede if there is no audio source, so check that there is
		if (_music) 
		{
			// if there is music curently playing, stop it
			//class assumes that previous music would have been faded by external source
			_music.volume = 0.0f;
			_music.Stop ();

			//now assine new clip to the audio source and play it
			_music.clip = MusicClips[clip];
			_music.Play();

			//declare a timer to keep track of delta time from start of play
			float timer = 0.0f;

			//while deltaTime is less than the desired fade time
			while (timer <= fade)
			{
				//record deltatime into "deltaTime" variable
				timer += Time.deltaTime;

				//calculate volume amplitude
				_music.volume = timer/fade;

				yield return null;
			}

			//fade is compleate
			_music.volume = 1.0f;

		}
	}

	//starts a new game
	public void StartNewGame()
	{
		/*
		//reset score, lives, etx...
		_currentScore = 0;
		_level = 0;
		_lives = 3;
		*/

		//stop any music that is playing and fade it out
		StopMusic (3);

		//load game scene
		SceneManager.LoadScene("game");

	}

	//quits game
	public void QuitGame()
	{
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	//function to be played at end of level before moving on
	public void LevelComplete()
	{
		//increase level
		if (_level < levels.Count - 1)
						_level++;

		//load game scene
		SceneManager.LoadScene("game");
	}

	//decreases the life count
	public int DecrementLives()
	{
		if (_lives > 0)
						_lives--;
		return _lives;
	}

	public void KillPlayer()
	{
		_lives = 0;
	}

	//increases player score
	public void IncreaseScore(int points)
	{
		_currentScore += points;
	}

}