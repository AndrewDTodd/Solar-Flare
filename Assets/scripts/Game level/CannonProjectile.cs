using UnityEngine;
using System.Collections;

public class CannonProjectile : MonoBehaviour
{
	public float speed = 100.0f;
	public SphereCollider BlastRadiusTrigger = null;


	private Vector3 _myVelocity = Vector3.zero;
	private Renderer _myRenderer = null;
	private Rigidbody _myRigidBody = null;
	private Transform _myTransform = null;
	private SceneManager_gamescene _mySceneManager = null;

	private bool _isActive = false;
	public bool isActive
	{
		get
		{
			return _isActive;
		}
	}

	private Collider _myCollider = null;

	void Start()
	{
		_isActive = false;
		_myVelocity = new Vector3 (0.0f, speed, 0.0f);
		_myRigidBody = GetComponent<Rigidbody>();
		_myRigidBody.velocity = Vector3.zero;
		_myRenderer = GetComponent<Renderer>();
		_myRenderer.enabled = _isActive;
		_myCollider = GetComponent<Collider>();
		_myCollider.enabled = _isActive;
		_myTransform = transform;
		_mySceneManager = SceneManager_gamescene.instance;

		if (BlastRadiusTrigger != null) 
		{
			BlastRadiusTrigger.enabled = false;
		}
	}

	void FixedUpdate()
	{
		if (_myRigidBody.position.y > 140) 
		{
			DisableProjectile();
		}
	}

	public void Fire(Vector3 pos)
	{
		if (_isActive) 
		{
			return;
		}

		_myCollider.enabled = _myRenderer.enabled = true;

		_isActive = true;

		_myTransform.position = pos;

		_myRigidBody.velocity = _myVelocity;

		if (BlastRadiusTrigger != null) 
		{
			BlastRadiusTrigger.enabled = false;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		col.gameObject.SetActive (false);

		_myCollider.enabled = false;
		_myRenderer.enabled = false;

		if (col.gameObject.layer == LayerMask.NameToLayer ("bases")) 
		{
			if (BlastRadiusTrigger != null)
			{
				BlastRadiusTrigger.radius = Random.Range (0.5f, 1.3f);
				BlastRadiusTrigger.enabled = true;
			}

			if (_mySceneManager != null)
			{
				_mySceneManager.RegisterBaseHit(_myTransform.position);
			}
		}

		Invoke ("DisableProjectile", 0.1f);
	}

	void DisableProjectile()
	{
		_isActive = false;
		_myRenderer.enabled = false;
		_myCollider.enabled = false;
		_myRigidBody.velocity = Vector3.zero;
		if (BlastRadiusTrigger != null) 
		{
			BlastRadiusTrigger.enabled = false;
		}
	}

}
