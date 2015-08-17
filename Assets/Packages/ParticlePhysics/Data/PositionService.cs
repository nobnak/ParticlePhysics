using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {	
	public class PositionService : System.IDisposable {
		public const int INITIAL_CAP = 1024;

		readonly ComputeShader _compute;
		readonly int _kernel;

		Vector2[] _positions;
		ComputeBuffer _p0, _p1, _uploader;

		public PositionService(ComputeShader compute, int capasity) {
			_kernel = compute.FindKernel(ShaderConst.KERNEL_UPLOAD_POSITION);
			_compute = compute;
			_positions = new Vector2[capasity];
			_p0 = new ComputeBuffer(capasity, Marshal.SizeOf(_positions[0]));
			_p1 = new ComputeBuffer(capasity, Marshal.SizeOf(_positions[0]));
			_uploader = new ComputeBuffer(INITIAL_CAP, Marshal.SizeOf(_positions[0]));
		}		
		public void Upload(int offset, Vector2[] p) {
			if (_uploader.count < p.Length) {
				_uploader.Dispose();
				_uploader = new ComputeBuffer(ShaderUtil.AlignBufferSize(p.Length), Marshal.SizeOf(_positions[0]));
			}
			_uploader.SetData(p);
			
			_compute.SetInt(ShaderConst.PROP_UPLOAD_OFFSET, offset);
			_compute.SetInt(ShaderConst.PROP_UPLOAD_LENGTH, p.Length);
			_compute.SetBuffer(_kernel, ShaderConst.BUF_UPLOADER, _uploader);
			_compute.SetBuffer(_kernel, ShaderConst.BUF_VELOCITY, _p0);
			
			int x, y, z;
			ShaderUtil.CalcWorkSize(p.Length, out x, out y, out z);
			_compute.Dispatch(_kernel, x, y, z);
		}

		#region IDisposable implementation
		public void Dispose () {
			if (_p0 != null)
				_p0.Dispose();
			if (_p1 != null)
				_p1.Dispose();
			if (_uploader != null)
				_uploader.Dispose();
		}
		#endregion		
	}
}
