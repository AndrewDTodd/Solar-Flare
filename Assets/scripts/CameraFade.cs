using UnityEngine;
using System.Collections;

public class CameraFade : MonoBehaviour 
{
	//material used to blurr or fade the screen 
	private Material fadeMaterial = null;

	//levle of fade, sets alfa value of material used for fade
	private float _currentFade = 1.0f;

	//fade function that alters the fade value over time
	public void faidIn(float duration = 1.0f)
	{
		StartCoroutine (DoFadeIn (duration));
	}

	private IEnumerator DoFadeIn(float duration)
	{
		float timer = 0.0f;

		while (timer < duration && duration > 0.01f) 
		{
			timer += Time.deltaTime;

			_currentFade = 1 - (timer/duration);

			yield return null;
		}

		_currentFade = 0.0f;
	}

	public void faidOut(float duration = 1.0f, float delay = 0.0f)
	{
		StartCoroutine (DoFadeOut (duration, delay));
	}

	public IEnumerator DoFadeOut (float duration, float delay)
	{
		yield return new WaitForSeconds (delay);

		float timer = 0.0f;

		while (timer < duration && duration > 0.01f) 
		{
			timer += Time.deltaTime;

			_currentFade = timer/duration;

			yield return null;
		}

		_currentFade = 1.0f;
	}

	//called after the camera has rendered scene
	void OnPostRender()
	{
		//if we hav enaot made the fade material
		if (!fadeMaterial) 
		{
			fadeMaterial = new Material( "Shader \"Hidden/CameraFade\" {" +
			                             "Properties { _Color (\"Main Color\", Color) = (1,1,1,0) }" +
			                             "SubShader {" +
			                             "    Pass {" +
			                             "        ZTest Always Cull Off ZWright Off "+
			                             "        Blen SrcAlpha OneMinusSrcAlpha "+
			                             "        Color [_Color]" +
			                             "    }" +
			                             "}" +
			                             "}"
			                             );

		}

		//set color of material
		fadeMaterial.SetColor ("_Color", new Color (0.0f, 0.0f, 0.0f, _currentFade));

		GL.PushMatrix ();

		GL.LoadOrtho ();

		for (var i = 0; i < fadeMaterial.passCount; ++i) 
		{
			fadeMaterial.SetPass(i);

			GL.Begin(GL.QUADS);

			GL.Vertex3 (0,0,0.1f);
			GL.Vertex3 (1,0,0.1f);
			GL.Vertex3 (1,1,0.1f);
			GL.Vertex3 (0,1,0.1f);

			GL.End();
		}

		GL.PopMatrix ();

	}
}
