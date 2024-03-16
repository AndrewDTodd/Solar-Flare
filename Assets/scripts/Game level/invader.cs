using UnityEngine;
using System.Collections;


public class invader : MonoBehaviour {

	public GameObject frame1 = null; // mesh to be used on frame 1 of two frame animation cycle
	public GameObject frame2 = null; // mesh to be used on frame 2 of two frame animation cycle
	public int points = 20; // points awarded upon player killing this invader

	public int Column = 0;
	public bool Active = true;

	private SceneManager_gamescene _mySceneManager = null;
	private Transform _myTransform = null;
	// Use this for initialization
	void Start () {
		//activate invader upon level startup
		gameObject.SetActive (true);
		Active = true;

		//activate frame1's mesh, set frame2 mesh to de-active
		if (frame1 != null)
			frame1.SetActive (true);
		if (frame2 != null)
			frame2.SetActive (false);


		_mySceneManager = SceneManager_gamescene.instance;
		_myTransform = transform;
	}

	//sets mech to be displayed on this frame of animation to active, previouse to de-active
	//to be called by scene manager
	public void UpdateFrame(){

		if (frame1 != null)
						frame1.SetActive (!frame1.activeSelf);
		if (frame2 != null)
						frame2.SetActive (!frame2.activeSelf);

	}

	void OnDisable()
	{
		if (_mySceneManager != null) 
		{
			_mySceneManager.RegisterInvaderHit (Column, points, _myTransform.position);
		}
		Active = false;
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("bases"))
		{
			this.gameObject.SetActive (false);

            col.gameObject.SetActive(false);

			GameObject blast = new GameObject("ImpactBlast");

			blast.transform.parent = _myTransform;
			blast.transform.localPosition = Vector3.zero;

			SphereCollider BlastRadiusTrigger = blast.AddComponent<SphereCollider>();

			blast.AddComponent<BlastRadius>();


            if (BlastRadiusTrigger != null)
            {
				BlastRadiusTrigger.radius = 5f;

                BlastRadiusTrigger.enabled = true;
            }

            if (_mySceneManager != null)
            {
                _mySceneManager.RegisterBaseHit(_myTransform.position);
            }
        }
		else if(col.tag == "GroundPlane")
		{
			_mySceneManager.InvadersWin();
		}
    }
}



