using UnityEngine;
using System.Collections;

public class MysteryProjectile : MonoBehaviour
{
	public float Speed = 100.0f;
	public SphereCollider BlastRadiusTrigger = null;
	private Collider _myCollider = null;
	private Vector3 _myVelocity = Vector3.zero;

	private bool _isActive = false;
	public bool isActive 
	{
		get{ return _isActive;}
	}

	private Renderer _myRenderer = null;
	private Rigidbody _myRigidbody = null;
	private SceneManager_gamescene _mySceneManager = null;
	private Material _myMaterial = null;

	private float _sizeMultiplier = 2.0f;

	void Start()
	{
		_isActive = false;
		_myVelocity = new Vector3 (0.0f, -Speed, 0.0f);
		_myRigidbody = GetComponent<Rigidbody>();
		_myRigidbody.velocity = Vector3.zero;
		_myRenderer = GetComponent<Renderer>();
		_myRenderer.enabled = _isActive;
		_myCollider = GetComponent<Collider>();
		_myCollider.enabled = _isActive;
		_mySceneManager = SceneManager_gamescene.instance;
		_myMaterial = GetComponent<Renderer>().material;

		if (BlastRadiusTrigger != null) 
		{
			BlastRadiusTrigger.enabled = false;
		}
	}

	public void Fire(Vector3 pos)
	{
		if (_isActive) 
		{
			return;
		}

        _myMaterial.color = Color.yellow;
        _sizeMultiplier = 2.0f;

		_myCollider.enabled = _myRenderer.enabled = true;

		_isActive = true;

		_myRigidbody.position = pos;
		_myRigidbody.velocity = _myVelocity;

		if (BlastRadiusTrigger != null) 
		{
			BlastRadiusTrigger.enabled = false;
		}


	}

	void OnTriggerEnter(Collider col)
	{
		if (col == null) 
		{
			return;
		}

		_myCollider.enabled = false;
		_myRenderer.enabled = false;

		if (col.gameObject.layer == LayerMask.NameToLayer ("bases")) 
		{
			col.gameObject.SetActive(false);

			if (BlastRadiusTrigger != null)
			{
				BlastRadiusTrigger.radius = Random.Range (0.5f, 1.3f * _sizeMultiplier);

				BlastRadiusTrigger.enabled = true;
			}

			if (_mySceneManager != null)
			{
				_mySceneManager.RegisterBaseHit(_myRigidbody.position);
			}

			Invoke ("DisableProjectile", 0.1f);
		}

		else
		{
			_mySceneManager.PlayerHit(_myRigidbody.position);

			DisableProjectile();
		}
	}

	void FixedUpdate()
	{
		if (_myRigidbody.position.y < -33)
		{
			DisableProjectile();
		}
	}

	void DisableProjectile()
	{
		_isActive = false;
		_myRenderer.enabled = false;
		_myCollider.enabled = false;
		_myRigidbody.velocity = Vector3.zero;

		if (BlastRadiusTrigger != null) 
		{
			BlastRadiusTrigger.enabled = false;
		}

		_mySceneManager.MysteryInstance.RegisterMysteryProjectile();
	}
}
