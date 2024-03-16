using UnityEngine;
using System.Collections;

public class RotateAnimator : MonoBehaviour {

	//component cache
	private Transform _MyTransform = null;

	//rotation factors describing how mutch we rotate
	public Vector3 _eulers = new Vector3(1.0f, 0.0f, 5.0f);

	//called prior to update to cache the transform object
	void Start () 
	{
		_MyTransform = transform;
	}
	
	//called every frame to rotate the object that is attatched to this script
	void Update () 
	{
		//apply rotation to the transform object
		_MyTransform.Rotate (_eulers * Time.deltaTime);
	}
}
