using UnityEngine;
using System.Collections;

public class cannon : MonoBehaviour
{
	public float speed = 100.0f;
	public GameObject ProjectilePrefab  = null;
	public AudioClip ShootingAudioClip = null;

	private Transform _myTransform = null;
	private Vector3 _myPosition = Vector3.zero;
	private Vector3 _startPosition = Vector3.zero;
	private AudioSource ShootingAudioSource = null;
	private bool _canFire = true;

	private CannonProjectile _cannonProjectileScript = null;

	void Start()
	{
		_myTransform = transform;
		_myPosition = _startPosition = _myTransform.position;

		if (ProjectilePrefab != null) 
		{
			GameObject go = Instantiate (ProjectilePrefab) as GameObject;

			if (go != null)
			{
				_cannonProjectileScript = go.GetComponent<CannonProjectile>();
			}
		}

		ShootingAudioSource = gameObject.AddComponent<AudioSource> ();

		ShootingAudioSource.clip = ShootingAudioClip;

		ShootingAudioSource.playOnAwake = false;
	}

    public IEnumerator ResetAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Reset();
    }

	public void Reset()
	{
		gameObject.SetActive (true);

		_myTransform.position = _startPosition;

		_myPosition = _startPosition;

        _canFire = true;
	}

	void Update()
	{
		float delta = Input.GetAxis ("Horizontal") * speed;

		_myPosition.x = Mathf.Clamp (_myPosition.x + (delta * Time.deltaTime), -115.0f, 115.0f);

		_myTransform.position = _myPosition;

		if (_canFire && (Input.GetKeyDown (KeyCode.Space) || Input.GetMouseButton (0))) 
		{
			_cannonProjectileScript.Fire(_myPosition);

			ShootingAudioSource.Play();

			StartCoroutine(LockFire());
		}
	}

	IEnumerator LockFire()
	{
		_canFire = false;
		yield return new WaitForSeconds (0.35f);
		_canFire = true;
	}
}
