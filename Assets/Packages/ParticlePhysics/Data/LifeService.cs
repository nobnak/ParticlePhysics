using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class LifeService : System.IDisposable {
		public const int INIT_CAPACITY = 2 * ShaderConst.WARP_SIZE;
		
		public readonly int SimSizeX, SimSizeY, SimSizeZ;
		public ComputeBuffer Lifes { get; private set; }

		readonly ComputeShader _compute;
		readonly int _kernelSimulate, _kernelUpload;
		readonly float[] _lifes;

		ComputeBuffer _uploader;

		public LifeService(ComputeShader compute, int capacity) {
			_compute = compute;
			_kernelSimulate = compute.FindKernel(ShaderConst.KERNEL_SIMULATE_LIFE);
			_kernelUpload = compute.FindKernel(ShaderConst.KERNEL_UPLOAD_LIFE);
			_lifes = new float[capacity];
			Lifes = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(float)));
			Lifes.SetData(_lifes);
			_uploader = new ComputeBuffer(INIT_CAPACITY, Marshal.SizeOf(typeof(float)));

			ShaderUtil.CalcWorkSize(capacity, out SimSizeX, out SimSizeY, out SimSizeZ);
		}

		public void Upload(int offset, float[] l) {
			if (_uploader.count < l.Length) {
				_uploader.Dispose();
				_uploader = new ComputeBuffer(ShaderUtil.AlignBufferSize(l.Length), Marshal.SizeOf(typeof(float)));
			}
			_uploader.SetData(l);
			
			_compute.SetInt(ShaderConst.PROP_UPLOAD_OFFSET, offset);
			_compute.SetInt(ShaderConst.PROP_UPLOAD_LENGTH, l.Length);
			_compute.SetBuffer(_kernelUpload, ShaderConst.BUF_UPLOADER_FLOAT, _uploader);
			_compute.SetBuffer(_kernelUpload, ShaderConst.BUF_LIFE, Lifes);
			
			int x, y, z;
			ShaderUtil.CalcWorkSize(l.Length, out x, out y, out z);
			_compute.Dispatch(_kernelUpload, x, y, z);
		}
		public void Simulate() {
			SetBuffer(_compute, _kernelSimulate);
			_compute.Dispatch(_kernelSimulate, SimSizeX, SimSizeY, SimSizeZ);
		}
		public float[] Download() {
			Lifes.GetData(_lifes);
			return _lifes;
		}
		public void SetBuffer(ComputeShader compute, int kernel) {
			compute.SetBuffer(kernel, ShaderConst.BUF_LIFE, Lifes);
		}
		public void SetGlobal() {
			Shader.SetGlobalBuffer(ShaderConst.BUF_LIFE, Lifes);
		}

		#region IDisposable implementation
		public void Dispose () {
			if (Lifes != null)
				Lifes.Dispose();
			if (_uploader != null)
				_uploader.Dispose();
		}
		#endregion
	}
}