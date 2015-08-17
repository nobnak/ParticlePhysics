using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class VelocityService : System.IDisposable {
		public const int INITIAL_CAP = 1024;

		readonly ComputeShader _compute;
		readonly int _kernel;

		Vector2[] _velocities;
		ComputeBuffer _v0, _v1, _uploader;

		public VelocityService(ComputeShader compute, int capasity) {
			_kernel = compute.FindKernel(ShaderConst.KERNEL_UPLOAD_VELOCITY);
			_compute = compute;
			_velocities = new Vector2[capasity];
			_v0 = new ComputeBuffer(capasity, Marshal.SizeOf(_velocities[0]));
			_v1 = new ComputeBuffer(capasity, Marshal.SizeOf(_velocities[0]));
			_uploader = new ComputeBuffer(INITIAL_CAP, Marshal.SizeOf(_velocities[0]));
		}

		public void Upload(int offset, Vector2[] v) {
			if (_uploader.count < v.Length) {
				_uploader.Dispose();
				_uploader = new ComputeBuffer(ShaderUtil.AlignBufferSize(v.Length), Marshal.SizeOf(_velocities[0]));
			}
			_uploader.SetData(v);

			_compute.SetInt(ShaderConst.PROP_UPLOAD_OFFSET, offset);
			_compute.SetInt(ShaderConst.PROP_UPLOAD_LENGTH, v.Length);
			_compute.SetBuffer(_kernel, ShaderConst.BUF_UPLOADER, _uploader);
			_compute.SetBuffer(_kernel, ShaderConst.BUF_VELOCITY, _v0);

			int x, y, z;
			ShaderUtil.CalcWorkSize(v.Length, out x, out y, out z);
			_compute.Dispatch(_kernel, x, y, z);
		}

		#region IDisposable implementation
		public void Dispose () {
			if (_v0 != null)
				_v0.Dispose();
			if (_v1 != null)
				_v1.Dispose();
			if (_uploader != null)
				_uploader.Dispose();
		}
		#endregion
	}
}
