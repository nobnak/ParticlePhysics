using UnityEngine;
using System.Collections;
using ParticlePhysics;
using System.Runtime.InteropServices;
using ParticlePhysics.Extension;

public class Fifth : MonoBehaviour {
	public const string KERNEL_ROTATE_ON_UNIFORM_VELOCITY = "RotateOnUniformVelocity";
	public const string PROP_UNIFORM_SPEED = "uniformSpeed";
	public const string PROP_UNIFORM_FORWARD = "uniformForward";
	public const string BUF_POSITION = "Positions";
	public const string BUF_ROTATION = "Rotations";

	public int count = 100;
	public float radius = 10f;
	public ComputeShader computeRotation;
	public GameObject container;
	public GameObject particlefab;
	public Transform controller;

	int _kernelRotateOnUniformVelocity;
	MeshCombiner _combiner;
	Vector3[] _positions;
	Vector4[] _rotations;
	ComputeBuffer _positionBuf;
	ComputeBuffer _rotationBuf;
	Quaternion _rotation;
	Vector3 _prevPosition;

	void Start () {
		var particles = new GameObject[count];
		for (var i = 0; i < count; i++)
			particles[i] = (GameObject)Instantiate(particlefab);
		_combiner = new MeshCombiner(container);
		_combiner.Rebuild(particles);
		_combiner.SetParent(transform, false);
		for (var i = 0; i < count; i++)
			Destroy(particles[i]);

		_kernelRotateOnUniformVelocity = computeRotation.FindKernel(KERNEL_ROTATE_ON_UNIFORM_VELOCITY);
		_positions = new Vector3[count];
		_positionBuf = new ComputeBuffer(count, Marshal.SizeOf(typeof(Vector3)));
		_rotations = new Vector4[count];
		_rotationBuf = new ComputeBuffer(count, Marshal.SizeOf(typeof(Vector4)));
		for (var i = 0; i < count; i++) {
			_positions[i] = radius * Random.insideUnitCircle;
			_rotations[i] = Random.rotationUniform.Convert();
		}
		_positionBuf.SetData(_positions);
		_rotationBuf.SetData(_rotations);
	}
	void OnDestroy() {
		if (_combiner != null)
			_combiner.Dispose();
		if (_positionBuf != null)
			_positionBuf.Dispose();
		if (_rotationBuf != null)
			_rotationBuf.Dispose();
	}
	void Update () {
		var move = controller.position - _prevPosition;
		move.z = 0f;
		_prevPosition = controller.position;

		int x, y, z;
		ShaderUtil.CalcWorkSize(count, out x, out y, out z);
		var speed = move.magnitude;
		var forward = (1f / speed) * move;
		computeRotation.SetFloat(ShaderConst.PROP_DELTA_TIME, Time.deltaTime);
		computeRotation.SetFloat(PROP_UNIFORM_SPEED, speed);
		computeRotation.SetVector(PROP_UNIFORM_FORWARD, forward);
		computeRotation.SetBuffer(_kernelRotateOnUniformVelocity, BUF_ROTATION, _rotationBuf);
		computeRotation.Dispatch(_kernelRotateOnUniformVelocity, x, y, z);

		Shader.SetGlobalBuffer(BUF_POSITION, _positionBuf);
		Shader.SetGlobalBuffer(BUF_ROTATION, _rotationBuf);
	}
}
