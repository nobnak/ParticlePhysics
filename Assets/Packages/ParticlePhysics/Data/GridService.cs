using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class GridService : System.IDisposable {
		public readonly uint[] StartData, EndData;

		readonly ComputeShader _compute;
		readonly Grid _grid;
		readonly HashService _hashes;
		readonly int _kernelInit, _kernelConstruct;
		readonly ComputeBuffer _hashGridStart, _hashGridEnd;

		public GridService(ComputeShader compute, Grid g, HashService h) {
			_compute = compute;
			_grid = g;
			_hashes = h;
			_kernelInit = compute.FindKernel(ShaderConst.KERNEL_INIT_GRID);
			_kernelConstruct = compute.FindKernel(ShaderConst.KERNEL_CONSTRUCT_GRID);
			var gridCellCount = g.nx * g.ny;

			StartData = new uint[gridCellCount];
			EndData = new uint[gridCellCount];
			_hashGridStart = new ComputeBuffer(gridCellCount, Marshal.SizeOf(typeof(uint)));
			_hashGridEnd = new ComputeBuffer(gridCellCount, Marshal.SizeOf(typeof(uint)));
			_hashGridStart.SetData (StartData);
			_hashGridEnd.SetData (EndData);
		}
		public void Construct(ComputeBuffer keys) {
			int x, y, z;
			SetParams(_compute);
			ShaderUtil.CalcWorkSize(_hashGridStart.count, out x, out y, out z);
			SetBuffer(_compute, _kernelInit);
			_compute.Dispatch(_kernelInit, x, y, z);

			ShaderUtil.CalcWorkSize(_hashes.Hashes.count, out x, out y, out z);
			SetParams(_compute);
			_compute.SetBuffer(_kernelConstruct, ShaderConst.BUF_BROADPHASE_KEYS, keys);
			SetBuffer(_compute, _kernelConstruct);
			_hashes.SetBuffer(_compute, _kernelConstruct);
			_compute.Dispatch(_kernelConstruct, x, y, z);
		}
		public void SetParams(ComputeShader c) {
			c.SetInt(ShaderConst.PROP_HASH_GRID_CAPACITY, _grid.nx * _grid.ny);
			c.SetInt(ShaderConst.PROP_HASH_GRID_NX, _grid.nx);
			c.SetInt(ShaderConst.PROP_HASH_GRID_NY, _grid.ny);
			c.SetVector(ShaderConst.PROP_HASH_GRID_PARAMS, new Vector4(_grid.w, _grid.h, _grid.nx / _grid.w, _grid.ny / _grid.h));
		}
		public void SetBuffer(ComputeShader c, int kernel) {
			c.SetBuffer(kernel, ShaderConst.BUF_GRID_START, _hashGridStart);
			c.SetBuffer(kernel, ShaderConst.BUF_GRID_END, _hashGridEnd);
		}
		public void Download(){
			_hashGridStart.GetData (StartData);
			_hashGridEnd.GetData (EndData);
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Grid {
			public int nx;
			public int ny;
			public float w;
			public float h;
		}

		#region IDisposable implementation
		public void Dispose () {
			if (_hashGridStart != null)
				_hashGridStart.Dispose ();
			if (_hashGridEnd != null)
				_hashGridEnd.Dispose ();
		}
		#endregion
	}
}