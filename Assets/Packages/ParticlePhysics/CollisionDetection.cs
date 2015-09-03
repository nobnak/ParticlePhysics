using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using MergeSort;

namespace ParticlePhysics {
	public interface ICollisionDetection : System.IDisposable {
		ComputeBuffer Collisions { get; }
		void Detect(float distance);
		void SetBuffer(ComputeShader c, int kernel);
		
	}
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Collision {
		public uint count;
		public fixed uint colliders[ShaderConst.COLLIDER_CAPACITY];
	}

	public class HashGrid : ICollisionDetection {
		public ComputeBuffer Collisions { get; private set; }

		readonly Grid _grid;
		readonly ComputeShader _compute;
		readonly LifeService _lifes;
		readonly PositionService _positions;

		readonly int _kernelInit, _kernelSolve;
		readonly ComputeBuffer _keys;
		readonly ComputeBuffer _hashes;
		readonly ComputeBuffer _hashGridStart, _hashGridEnd;
		readonly BitonicMergeSort _sort;

		public HashGrid(ComputeShader compute, ComputeShader computeSort, LifeService l, PositionService p, Grid g) {
			var capacity = l.Lifes.count;
			_compute = compute;
			_lifes = l;
			_positions = p;
			_grid = g;

			_kernelInit = compute.FindKernel(ShaderConst.KERNEL_INIT_COLLISION_DETECTION);
			_kernelSolve = compute.FindKernel (ShaderConst.KERNEL_SOVLE_COLLISION_DETECTION);
			_sort = new BitonicMergeSort(computeSort);
			_keys = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(uint)));
			_hashes = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(int)));
			Collisions = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(Collision)));

			var gridCellCount = g.nx * g.ny;
			_hashGridStart = new ComputeBuffer(gridCellCount, Marshal.SizeOf(typeof(uint)));
			_hashGridEnd = new ComputeBuffer(gridCellCount, Marshal.SizeOf(typeof(uint)));
		}

		public void Detect(float distance) {
			int x = _lifes.SimSizeX, y = _lifes.SimSizeY, z = _lifes.SimSizeZ;
			SetParams(_compute);
			_compute.SetBuffer(_kernelInit, ShaderConst.BUF_LIFE, _lifes.Lifes);
			_compute.SetBuffer(_kernelInit, ShaderConst.BUF_POSITION, _positions.P0);
			_compute.SetBuffer(_kernelInit, ShaderConst.BUF_HASHES, _hashes);
			_compute.SetBuffer(_kernelInit, ShaderConst.BUF_HASH_START, _hashGridStart);
			_compute.SetBuffer(_kernelInit, ShaderConst.BUF_HASH_END, _hashGridEnd);
			_compute.Dispatch(_kernelInit, x, y, z);

			_sort.Init(_keys);
			_sort.Sort(_keys, _hashes);

			SetParams(_compute);
			_compute.SetFloat(ShaderConst.PROP_BROADPHASE_SQR_DISTANCE, distance * distance);
			_compute.SetBuffer(_kernelSolve, ShaderConst.BUF_HASHES, _hashes);
			_compute.SetBuffer(_kernelSolve, ShaderConst.BUF_BROADPHASE_KEYS, _keys);
			_compute.SetBuffer(_kernelSolve, ShaderConst.BUF_POSITION, _positions.P0);
			SetBuffer(_compute, _kernelSolve);
			_compute.Dispatch(_kernelSolve, x, y, z);
		}
		public void SetParams(ComputeShader c) {
			c.SetInt(ShaderConst.PROP_HASH_GRID_CAPACITY, _grid.nx * _grid.ny);
			c.SetInt(ShaderConst.PROP_HASH_GRID_NX, _grid.nx);
			c.SetInt(ShaderConst.PROP_HASH_GRID_NY, _grid.ny);
			c.SetFloat(ShaderConst.PROP_HASH_GRID_DX, _grid.w / _grid.nx);
			c.SetFloat(ShaderConst.PROP_HASH_GRID_DY, _grid.h / _grid.ny);
			c.SetFloat(ShaderConst.PROP_HASH_GRID_W, _grid.w);
			c.SetFloat(ShaderConst.PROP_HASH_GRID_H, _grid.h);
		}
		public void SetBuffer(ComputeShader c, int kernel) {
			c.SetBuffer(kernel, ShaderConst.BUF_COLLISIONS, Collisions);
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
			if (_sort != null)
				_sort.Dispose();
			if (_keys != null)
				_keys.Dispose();
			if (_hashes != null)
				_hashes.Dispose();
			if (_hashGridStart != null)
				_hashGridStart.Dispose();
			if (_hashGridEnd != null)
				_hashGridEnd.Dispose();
			if (Collisions != null)
				Collisions.Dispose();
		}
		#endregion
	}
}