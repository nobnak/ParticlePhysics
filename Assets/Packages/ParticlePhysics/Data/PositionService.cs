using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {	
	public class PositionService : System.IDisposable {
		public const int INITIAL_CAP = 1024;

		public ComputeBuffer P0 { get; private set; }

		readonly ComputeShader _compute;
		readonly int _kernelUpload;

		Vector2[] _positions;
		ComputeBuffer _uploader;

		public PositionService(ComputeShader compute, int capasity) {
			_kernelUpload = compute.FindKernel(ShaderConst.KERNEL_UPLOAD_POSITION);
			_compute = compute;
			_positions = new Vector2[capasity];
			P0 = new ComputeBuffer(capasity, Marshal.SizeOf(_positions[0]));
			P0.SetData(_positions);
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
			_compute.SetBuffer(_kernelUpload, ShaderConst.BUF_UPLOADER, _uploader);
			_compute.SetBuffer(_kernelUpload, ShaderConst.BUF_POSITION, P0);
			
			int x, y, z;
			ShaderUtil.CalcWorkSize(p.Length, out x, out y, out z);
			_compute.Dispatch(_kernelUpload, x, y, z);
		}
		public Vector2[] Download() {
			P0.GetData (_positions);
			return _positions;
		}
		public void SetGlobal() { Shader.SetGlobalBuffer (ShaderConst.BUF_POSITION, P0); }

		#region IDisposable implementation
		public void Dispose () {
			if (P0 != null)
				P0.Dispose();
			if (_uploader != null)
				_uploader.Dispose();
		}
		#endregion		
	}
}
