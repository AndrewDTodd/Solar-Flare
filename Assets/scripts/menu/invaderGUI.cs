using UnityEngine;
using System.Collections;

public class invaderGUI : MonoBehaviour {


	public GameObject frame1 = null;
	public GameObject frame2 = null;

	public GameObject PointsText = null;

	public float StartDelay = 2.0f;

	public float speed = 1.0f;

	public float FrameSpeed = 0.25f;

	//used to signle the end of the screen
	public bool SequenceTurminator = false;

	public Vector3 _target = Vector3.zero;
	public Vector3 _start = Vector3.zero;
	public Vector3 _end = Vector3.zero;

	private Vector3 _current = Vector3.zero;

	//cache transform
	private Transform _myTransform;

	private float _interpolator = 0.0f;

	private bool _sequenceRuning = false;

	private float _timer = 0.0f;
	private float _OldTimer = 0.0f;

	//should the object animate
	private bool _animate = true;



	// Use this for initialization
	void Awake () 
	{
		_myTransform = transform;
		_target = _myTransform.localPosition;
	}

	void OnEnable()
	{
		_start = _end = _target;

		//ofset the start and end keyframes
		_start.x = _target.x + 3.0f;
		_end.x = _target.x - 3.0f;

		_myTransform.localPosition = _start;

		//ensure that points text is off
		if (PointsText != null)
						PointsText.SetActive (false);

		//initialize internal var
		_timer = 0;
		_OldTimer = 0;
		_animate = true;
		_interpolator = 0.0f;
		_sequenceRuning = false;

	}
	
	// Update is called once per frame
	void Update () 
	{
		//update the timer
		_timer += Time.deltaTime;

		//start animation
		if (!_sequenceRuning && _timer > StartDelay)
		{
			StartCoroutine(AnimatedSequence());
		}

		//animate
		if (frame1 != null && frame2 != null && (_animate || !frame1.activeSelf))
		{
			//check that the animation time constrant has been reaced
			if(_timer - _OldTimer > FrameSpeed)
			{
				//record this time stamp
				_OldTimer = _timer;

				frame1.SetActive(!frame1.activeSelf);
				frame2.SetActive(!frame2.activeSelf);

			}
		}

	}

	//horizontal animation function
	private IEnumerator AnimatedSequence()
	{
		_sequenceRuning = true;

		_myTransform.localPosition = _start;

		_interpolator = 0.0f;
		_animate = true;

		//enable renderers
		if (frame1 && frame1.GetComponent<Renderer>()) { frame1.GetComponent<Renderer>().enabled = true;}
		if (frame2 && frame2.GetComponent<Renderer>()) { frame2.GetComponent<Renderer>().enabled = true;}
		if (GetComponent<Renderer>())
						GetComponent<Renderer>().enabled = true;

		while (_interpolator < 1.0f)
		{
			//increase the interpolator based on anim speed
			_interpolator += Time.deltaTime * speed;

			//interpolate between start and end position to get current 
			_current = Vector3.Lerp (_start, _target, _interpolator);

			_myTransform.localPosition = _current;

			yield return null;

		}
		//shut down the animation
		_animate = false;

		//desplay the point value
		if (PointsText != null) {PointsText.SetActive(true);}

		//Stop and wait for 6 seconds
		yield return new WaitForSeconds (6.0f);

		//turn off points text
		if (PointsText != null) {PointsText.SetActive(false);}

		//start animator again
		_animate = true;

		//reset interpolator
		_interpolator = 0.0f;

		//animate off screen
		while (_interpolator < 1.0) 
		{
			_interpolator += Time.deltaTime * speed;

			_current = Vector3.Lerp(_target, _end, _interpolator);

			_myTransform.localPosition = _current;

			yield return null;
		}

		//change to the next screen on the last ship pass
		if (SequenceTurminator && scenemanager_mainmenu.instance) 
		{
			//shut off the screen and change to the next one
			_sequenceRuning = false;

			scenemanager_mainmenu.instance.NextScreen();
		}

		//disable renderers
		if (frame1 && frame1.GetComponent<Renderer>())
						frame1.GetComponent<Renderer>().enabled = false;
		if (frame2 && frame2.GetComponent<Renderer>())
						frame2.GetComponent<Renderer>().enabled = false;
		if (GetComponent<Renderer>())
						GetComponent<Renderer>().enabled = false;


		yield return new WaitForSeconds(5);

		_sequenceRuning = false;
	}

	//disabler function
	void OnDisable()
	{
		_sequenceRuning = false;
	}

}
