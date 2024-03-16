using UnityEngine;
using System.Collections;

public class renderflasher : MonoBehaviour
{
	//how fast the object will flash
	public float FlickerRate = 0.5f;

	//cache the renderer component
	private Renderer _myRenderer = null;

	//internal timer
	private float _timer = 0.0f;

	void OnEnable()
	{
		//cahce the rederer component of object
		_myRenderer = GetComponent<Renderer>();

		//reser timer
		_timer = 0.0f;

	}

	void OnDisable ()
	{	
		//reactivate any renderer
		if (_myRenderer != null) 
		{
			_myRenderer.enabled = true;
		}
	}

	void Update()
	{
		//increment timer
		_timer += Time.deltaTime;

		//if time greater than flicker time
		if (_timer > FlickerRate) 
		{
			//reset timer
			_timer = 0.0f;

			//toggle the renderer
			if (_myRenderer != null)
			{
				_myRenderer.enabled = !_myRenderer.enabled;

			}
		}
	}
}
