using UnityEngine;
using System.Collections;
using ParticlePhysics;
using System.Runtime.InteropServices;

public class Fifth : MonoBehaviour {
	public const string PROP_ROTATION = "_Rotation";
	public const string BUF_POSITION = "Positions";
	public const string BUF_QUATERNION = "Quaternions";

	public int count = 100;
	public float radius = 10f;
	public GameObject container;
	public GameObject particlefab;
	public Transform controller;

	MeshCombiner _combiner;
	Vector3[] _positions;
	Vector4[] _quaternions;
	ComputeBuffer _positionBuf;
	ComputeBuffer _quaternionBuf;

	void Start () {
		var particles = new GameObject[count];
		for (var i = 0; i < count; i++)
			particles[i] = (GameObject)Instantiate(particlefab);
		_combiner = new MeshCombiner(container);
		_combiner.Rebuild(particles);
		_combiner.SetParent(transform, false);
		for (var i = 0; i < count; i++)
			Destroy(particles[i]);

		_positions = new Vector3[count];
		_positionBuf = new ComputeBuffer(count, Marshal.SizeOf(typeof(Vector3)));
		_quaternions = new Vector4[count];
		_quaternionBuf = new ComputeBuffer(count, Marshal.SizeOf(typeof(Vector4)));
		for (var i = 0; i < count; i++) {
			_positions[i] = radius * Random.insideUnitCircle;
			_quaternions[i] = Random.rotationUniform.Convert();
		}
		_positionBuf.SetData(_positions);
		_quaternionBuf.SetData(_quaternions);
	}
	void OnDestroy() {
		if (_combiner != null)
			_combiner.Dispose();
		if (_positionBuf != null)
			_positionBuf.Dispose();
		if (_quaternionBuf != null)
			_quaternionBuf.Dispose();
	}
	void Update () {
		Shader.SetGlobalVector(PROP_ROTATION, controller.rotation.Convert());
		Shader.SetGlobalBuffer(BUF_POSITION, _positionBuf);
		Shader.SetGlobalBuffer(BUF_QUATERNION, _quaternionBuf);
	}
}

public static class Extension {
	public static Vector4 Convert(this Quaternion q) {
		return new Vector4(q.x, q.y, q.z, q.w);
	}
}
