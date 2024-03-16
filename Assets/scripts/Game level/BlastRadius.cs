using UnityEngine;
using System.Collections;

public class BlastRadius : MonoBehaviour 
{
	void OnTriggerEnter(Collider col)
	{
		if(col.tag != "ForceField")
			col.gameObject.SetActive (false);
	}
}
