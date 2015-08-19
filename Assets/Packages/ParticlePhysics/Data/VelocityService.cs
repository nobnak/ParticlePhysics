using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class VelocityService : System.IDisposable {
		public const int INITIAL_CAP = 1024;

		public ComputeBuffer V0 { get; private set; }

		readonly ComputeShader _compute;
		readonly int _kernelUpload;

		Vector2[] _velocities;
		ComputeBuffer _uploader;

		public VelocityService(ComputeShader compute, int capasity) {
			_kernelUpload = compute.FindKernel(ShaderConst.KERNEL_UPLOAD_VELOCITY);
			_compute = compute;
			_velocities = new Vector2[capasity];
			V0 = new ComputeBuffer(capasity, Marshal.SizeOf(_velocities[0]));
			V0.SetData(_velocities);
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
			_compute.SetBuffer(_kernelUpload, ShaderConst.BUF_UPLOADER, _uploader);
			_compute.SetBuffer(_kernelUpload, ShaderConst.BUF_VELOCITY, V0);

			int x, y, z;
			ShaderUtil.CalcWorkSize(v.Length, out x, out y, out z);
			_compute.Dispatch(_kernelUpload, x, y, z);
		}
		public Vector2[] Download() {
			V0.GetData (_velocities);
			return _velocities;
		}
		public void SetGlobal() { Shader.SetGlobalBuffer (ShaderConst.BUF_VELOCITY, V0); }
		public void SetBuffer(ComputeShader compute, int kernel) {
			compute.SetBuffer(kernel, ShaderConst.BUF_VELOCITY, V0);
		}

		#region IDisposable implementation
		public void Dispose () {
			if (V0 != null)
				V0.Dispose();
			if (_uploader != null)
				_uploader.Dispose();
		}
		#endregion
	}
}
