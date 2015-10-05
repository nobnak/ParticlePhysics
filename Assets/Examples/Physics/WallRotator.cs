using UnityEngine;
using System.Collections;

public class WallRotator : MonoBehaviour {
	public Vector3 speed;

	void Update () {
		transform.localRotation *= Quaternion.Euler(speed * Time.deltaTime);
	}
}
