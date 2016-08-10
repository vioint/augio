using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour
{

	public bool keepX = false;
	public bool keepZ = false;

	void Update ()
	{
		Vector3 v = Camera.main.transform.position - transform.position;
		v.z = keepZ ? v.z : 0f;
		v.x = keepX ? v.x : 0f;
		transform.LookAt (Camera.main.transform.position - v); 
		transform.Rotate (0, 180, 0);
	}
}