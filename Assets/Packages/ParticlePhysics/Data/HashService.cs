using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class HashService : System.IDisposable {
		public readonly ComputeBuffer Hashes;
		public readonly int[] HashData;

		readonly ComputeShader _compute;
		readonly LifeService _lifes;
		readonly PositionService _positions;
		readonly int _kernelInitHashes;

		public HashService(ComputeShader compute, LifeService l, PositionService p) {
			_compute = compute;
			_lifes = l;
			_positions = p;
			_kernelInitHashes = compute.FindKernel(ShaderConst.KERNEL_INIT_HASHES);
			HashData = new int[l.Lifes.count];
			Hashes = new ComputeBuffer(l.Lifes.count, Marshal.SizeOf(typeof(int)));
			Hashes.SetData (HashData);
		}
		public void Init(GridService grid) {
			int x = _lifes.SimSizeX, y = _lifes.SimSizeY, z = _lifes.SimSizeZ;
			grid.SetParams(_compute);
			_compute.SetBuffer(_kernelInitHashes, ShaderConst.BUF_LIFE, _lifes.Lifes);
			_compute.SetBuffer(_kernelInitHashes, ShaderConst.BUF_POSITION, _positions.P0);
			SetBuffer(_compute, _kernelInitHashes);
			_compute.Dispatch(_kernelInitHashes, x, y, z);
		}
		public void SetBuffer(ComputeShader c, int kernel) {
			c.SetBuffer(kernel, ShaderConst.BUF_HASHES, Hashes);
		}
		public void Download() {
			Hashes.GetData (HashData);
		}

		#region IDisposable implementation
		public void Dispose () {
			if (Hashes != null)
				Hashes.Dispose();
		}
		#endregion		
	}
}