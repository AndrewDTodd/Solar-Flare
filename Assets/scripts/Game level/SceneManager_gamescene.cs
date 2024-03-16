//#define PRINT_SCORE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Alien
{
	public GameObject AlienGameObject = null;

	public invader InvaderScript = null;

	public Transform AlienTransform = null;
}

public class SceneManager_gamescene : MonoBehaviour 
{
    public TextMesh currentScore;
    public TextMesh highScore;

    public GameObject Life1;
    public GameObject Life2;
    public GameObject Life3;

    public GameObject cannonObj;
    public cannon cannonIns;

    public GameObject GameOverHud;

	//list of the alien prefabs
	public List<GameObject> AlienPrefabs = new List<GameObject> ();
	public GameObject MysteryPrefab = null;
	public List<AudioClip> MovementSounds = new List<AudioClip> ();
	public List<AudioClip> ExplosionSounds = new List<AudioClip> ();
	public AudioClip InvaderHitClip = null;

	public GameObject AlienProjectilePrefab = null;

	public Transform BaseExploder = null;
	public ParticleSystem _baseExploderPS = null;
	public Transform InvaderExploder = null;
    public ParticleSystem _invaderExploderPS = null;

	private List<AlienProjectile> AlienProjectileInstances = new List<AlienProjectile> ();

	private List<Alien> _alienInstances = new List<Alien>();

	private mysteryInvader _mysteryInstance = null;
	public mysteryInvader MysteryInstance { get => _mysteryInstance; }

	private GameObject _waveObject = null;
	private Transform _waveTransform = null;
	private List<int> _AlienColCounts = new List<int> ();
	private GameManager _gameManager = null;

	private float _xStep = 2.5f;
	private float _xStepMultiplier = 1.001f;
	private float _yStep = 10.0f;
	private float _moveDelay = 55.0f;
	private float _moveDelayDecrement = 1.0f;
	private float _lowMoveClamp = 1;
	private int _maxAlienProjectiles = 2;
	private int _liveAlienProjectiles = 0;
	private int _mysteryChance = 2;
	private int _invaderFireChance = 10;

	private int XMin = -20;
	private int XMax = 20;
	private int _delayCounter = 0;
	private Vector3 _wavePos = Vector3.zero; 
	private int _movementClipIndex = 0;
	private AudioSource _moveAudioSource = null;
	private List<AudioSource> _explosions = new List<AudioSource> ();
	private AudioSource _invaderHit = null;

	private int _invaderCount = 55;

	private static SceneManager_gamescene _instance = null;
	public static SceneManager_gamescene instance 
	{
		get
		{
			if(_instance == null)
			{
				_instance = (SceneManager_gamescene) FindObjectOfType(typeof(SceneManager_gamescene));
			}
			return _instance;
		}
	}

	//creates the wave of invaders
	void Start()
	{
		_gameManager = GameManager.instance;

		if (_gameManager != null) 
		{
			_maxAlienProjectiles = _gameManager.CurrentMaxAlienProjectiles;
			_xStep = _gameManager.CurrentHorizStartSpeed;
			_xStepMultiplier = _gameManager.CurrentHorizSpeedMultiplyer;
			_yStep = _gameManager.CurrentVerticalSpeed;
			_mysteryChance = _gameManager.CurrentMysteryInvaderChance;
			_invaderFireChance = _gameManager.CurrentFireChance;
			_lowMoveClamp = _gameManager.CurrentMoveDelayClamp;
			_moveDelay = _gameManager.CurrentStartMoveDelay;
			_moveDelayDecrement = _gameManager.CurrentMoveDelayDecrement;

            string name = "";
            uint score = 0;

            _gameManager.GetHigestScore(out name, out score);

            print(name);
            print(score);

            highScore.text = "High Score (" + name + ") : " + score;
		}

		//location of the top left invader
		int x = -100;
		int y = 130;

		_waveObject = new GameObject ("Wave Container");

		_waveTransform = _waveObject.transform;

		//creates 5 rows of invaders
		for (int rows = 0; rows < 5; rows++) 
		{
			for (int cols = 0; cols < 11; cols++)
			{
				GameObject go = Instantiate(AlienPrefabs[rows], new Vector3 (x,y,0), Quaternion.identity) as GameObject;


				if (go != null)
				{
					go.transform.parent = _waveTransform;

					Alien alien = new Alien();

					alien.AlienGameObject = go;
					alien.AlienTransform = go.transform;
					alien.InvaderScript = go.GetComponent<invader>();

					alien.InvaderScript.Column = cols;

					_alienInstances.Add(alien);
				}

				x += 20;
			}

			x =-100;
			y -= 20;
		}

		for (int cols = 0; cols < 11; cols++) 
		{
			_AlienColCounts.Add(5);
		}

		_waveTransform.position = new Vector3 (XMin, 0, 0);
		_waveTransform.rotation = Quaternion.identity;


		_wavePos = _waveTransform.position;

		_moveAudioSource = gameObject.AddComponent<AudioSource> ();
		_moveAudioSource.playOnAwake = false;
		_movementClipIndex = 0;


		for (int i = 0; i < 4; i++) 
		{
			_explosions.Add (gameObject.AddComponent<AudioSource>());
		}

		_invaderHit = gameObject.AddComponent<AudioSource> ();
		_invaderHit.clip = InvaderHitClip;
		_invaderHit.playOnAwake = false;

		for (int i = 0; i < _maxAlienProjectiles; i++) 
		{
			GameObject go = Instantiate (AlienProjectilePrefab) as GameObject;
			if (go != null)
			{
				AlienProjectile instance = go.GetComponent<AlienProjectile>();

				if(instance != null)
				{
					AlienProjectileInstances.Add (instance);
				}
			}
		}
       
		if (BaseExploder != null) 
		{
			_baseExploderPS = BaseExploder.GetComponentInChildren<ParticleSystem>();
		}

		if (InvaderExploder != null) 
		{
			_invaderExploderPS  = InvaderExploder.GetComponentInChildren<ParticleSystem>();
		}
		

	}

	public void RegisterAlienProjectile()
	{
		_liveAlienProjectiles--;
	}

	void FixedUpdate()
	{
		if(_mysteryInstance == null && Random.value <= (_mysteryChance / 100))
		{
			Debug.LogWarning("Spawning Mystery Invader");

            //location to spawn mystery invader
            int x = -100;
            int y = 140;

			GameObject mystery = Instantiate(MysteryPrefab, new Vector3(x, y, 0), Quaternion.identity);

			_mysteryInstance = mystery.GetComponent<mysteryInvader>();
			_invaderCount++;
        }

        if (_invaderCount < 1)
        {
            print("Level Won");
            StartCoroutine(_gameManager.GameOverTimer(1.5f));
        }

        _delayCounter++;

		if (_delayCounter >= Mathf.Clamp (_moveDelay, _lowMoveClamp, 50.0f)) 
		{
			if (!_moveAudioSource.isPlaying)
			{
				_moveAudioSource.clip = MovementSounds[_movementClipIndex++];

				_moveAudioSource.Play();
			}

			if (_movementClipIndex > 3)
			{
				_movementClipIndex = 0;
			}

			_wavePos = _waveTransform.position;

			if((_wavePos.x >= XMax && _xStep > 0) || (_wavePos.x <= XMin && _xStep < 0))
			{
				_xStep = -_xStep;
				_wavePos.y -= _yStep;
			}

			else
			{
				_wavePos.x += _xStep;

			}
			_waveTransform.position = _wavePos;

			for (int i = 0; i < _alienInstances.Count; i++)
			{
				Alien alien = _alienInstances[i];

				if (alien != null && alien.InvaderScript != null && alien.AlienGameObject.activeSelf)
				{
					alien.InvaderScript.UpdateFrame();


				}
			}

			if (_liveAlienProjectiles < _maxAlienProjectiles && Random.Range (0, 100) < _invaderFireChance)
			{
				for (int i = 0; i < _maxAlienProjectiles; i++)
				{
					AlienProjectile instance = AlienProjectileInstances[i];

					if (instance != null && !instance.isActive)
					{
						int col = Random.Range (0,10);

						if (_AlienColCounts[col] == 0 )
						{
							continue;
						}

						for (int y = 4; y >= 0; y--)
						{
							Alien invader = _alienInstances[y*11+col];

							if (invader.AlienGameObject.activeSelf)
							{
								instance.Fire (invader.AlienTransform.position, false);

								_liveAlienProjectiles++;

								break;
							}
						}
						break;
					}
				}
			}

			_delayCounter = 0;
		}
	}

	public void RegisterBaseHit (Vector3 pos)
	{
		if (BaseExploder != null) 
		{
			BaseExploder.position = pos;

			if (_baseExploderPS)
				_baseExploderPS.Emit(50);
		}

		if (ExplosionSounds.Count > 0) 
		{
			for (int i = 0; i < _explosions.Count; i++)
			{
				AudioSource src = _explosions[i];

				if (src != null && !src.isPlaying)
				{
					int effectIndex = Random.Range (0, ExplosionSounds.Count);

					if (ExplosionSounds[effectIndex] != null)
					{
						src.clip = ExplosionSounds[effectIndex];

						src.volume = 0.25f;

						src.pitch = Random.value+0.8f;

						src.Play ();
					}
				}
			}
		}
	}

	public void RegisterInvaderHit(int colIndex, int points, Vector3 pos)
	{
		if (_gameManager != null) 
		{
			_gameManager.IncreaseScore (points);

#if PRINT_SCORE
			print(_gameManager.currentScore);
#endif

            currentScore.text = "Current Score: " + _gameManager.currentScore;
		}

		if (colIndex > -1)
		{
			_AlienColCounts[colIndex]--;

			_invaderCount--;

			_moveDelay -= _moveDelayDecrement;
			_xStep *= _xStepMultiplier;

			for (int i = 0; i < 11; i++)
			{
				if (_AlienColCounts[i] == 0)
				{
					XMin = -20 - ((i+1) *20);
				}
				else
					break;

			}

			for (int i = 10; i >= 0; i--)
			{
				if (_AlienColCounts[i] == 0)
				{
					XMax = 20 + ((11-i) * 20);
				}

				else
					break;
			}
		}

		_invaderHit.Play ();

		if (InvaderExploder != null)
		{
			InvaderExploder.position = pos;
			if (_invaderExploderPS)
			{
				_invaderExploderPS.Emit(50);
			}
		}
	}

    public void RegisterMysteryHit(int points, Vector3 pos)
    {
        if (_gameManager != null)
        {
            _gameManager.IncreaseScore(points);

#if PRINT_SCORE
			print(_gameManager.currentScore);
#endif

            currentScore.text = "Current Score: " + _gameManager.currentScore;
        }
		_invaderCount--;

        _invaderHit.Play();

        if (BaseExploder != null)
        {
            BaseExploder.position = pos;

            if (_baseExploderPS)
                _baseExploderPS.Emit(50);
        }

		GameObject.Destroy(_mysteryInstance.gameObject);
		_mysteryInstance = null;
    }

    public void PlayerHit(Vector3 pos)
	{
        print("Player Hit");

        _gameManager.DecrementLives();

        cannonObj.SetActive(false);
		
        if (BaseExploder != null)
        {
            BaseExploder.position = pos;

            if (_baseExploderPS)
                _baseExploderPS.Emit(50);
        }
		
        if (Life3.activeInHierarchy)
        {
            Life3.SetActive(false);
        }
        else if(Life2.activeInHierarchy)
        {
            Life2.SetActive(false);
        }
        else
        {
            Life1.SetActive(false);

            StartCoroutine(_gameManager.GameOverTimer(1.5f));
        }

        StartCoroutine(cannonIns.ResetAfterTime(.5f));
        
	}

	public void InvadersWin()
	{
        print("Invaders got to planet");

        _gameManager.KillPlayer();

        cannonObj.SetActive(false);
		if (BaseExploder != null)
		{
            BaseExploder.position = cannonObj.transform.position;

            if (_baseExploderPS)
                _baseExploderPS.Emit(50);
        }
        
        Life3.SetActive(false);
        
        Life2.SetActive(false);
       
        Life1.SetActive(false);

        StartCoroutine(_gameManager.GameOverTimer(1.5f));
    }
}
