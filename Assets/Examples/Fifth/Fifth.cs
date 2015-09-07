using UnityEngine;
using System.Collections;

public class Fifth : MonoBehaviour {
	public const string PROP_QUATERNION = "_Quaternion";

	public GameObject qrot;
	public Transform controller;

	Material _mat;

	void Start () {
		_mat = qrot.GetComponent<Renderer> ().sharedMaterial;
	}
	void Update () {
		var rot = controller.rotation;
		var quaternion = new Vector4 (rot.x, rot.y, rot.z, rot.w);
		_mat.SetVector (PROP_QUATERNION, quaternion);
	}
}
