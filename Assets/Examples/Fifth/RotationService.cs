using UnityEngine;
using System.Collections;
using ParticlePhysics;
using System.Runtime.InteropServices;
using ParticlePhysics.Extension;

public class RotationService : System.IDisposable {
	public const string KERNEL_VELOCITY_BASED_ROTATE = "RotateOnVelocity";
	public const string BUF_ROTATION = "Rotations";

	public readonly int Count;
	public readonly int SimSizeX, SimSizeY, SimSizeZ;

	readonly ComputeShader _compute;
	readonly VelocityService _velocities;
	readonly LifeService _lifes;
	readonly int _kernelVelocityBasedRotate;
	readonly Vector4[] _data;
	readonly ComputeBuffer _rotations;

	public RotationService(ComputeShader c, VelocityService v, LifeService l) {
		Count = v.V0.count;
		_compute = c;
		_velocities = v;
		_lifes = l;
		_kernelVelocityBasedRotate = c.FindKernel(KERNEL_VELOCITY_BASED_ROTATE);
		_data = new Vector4[Count];
		_rotations = new ComputeBuffer(Count, Marshal.SizeOf(typeof(Vector4)));

		for (var i = 0; i < Count; i++)
			_data[i] = QuaternionExtension.IDENTITY;
		Upload();
		ShaderUtil.CalcWorkSize(Count, out SimSizeX, out SimSizeY, out SimSizeZ);
	}
	public void VelocityBasedRotate(float dt) {
		_compute.SetFloat(ShaderConst.PROP_DELTA_TIME, dt);
		_velocities.SetBuffer(_compute, _kernelVelocityBasedRotate);
		_lifes.SetBuffer(_compute, _kernelVelocityBasedRotate);
		_compute.SetBuffer(_kernelVelocityBasedRotate, BUF_ROTATION, _rotations);
		_compute.Dispatch(_kernelVelocityBasedRotate, SimSizeX, SimSizeY, SimSizeZ);
	}
	public void SetQuaternion(Quaternion[] q) {
		for (var i = 0; i < Count; i++)
			_data[i] = q[i].Convert();
		Upload();
	}
	public void Upload() {
		_rotations.SetData(_data);
	}
	
	#region IDisposable implementation
	public void Dispose () {
		if (_rotations != null)
			_rotations.Dispose();
	}
	#endregion
}
