using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using MergeSort;
using System.Text;

namespace ParticlePhysics {
	public interface ICollisionDetection : System.IDisposable {
		ComputeBuffer Collisions { get; }
		void Detect(float distance);
		void SetBuffer(ComputeShader c, int kernel);
		void SetGlobal();
		
	}
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Collision {
		public uint count;
		public fixed uint colliders[ShaderConst.COLLIDER_CAPACITY];
	}

	public class HashGrid : ICollisionDetection {
		public ComputeBuffer Collisions { get; private set; }
		public readonly Collision[] CollisionData;

		readonly ComputeShader _compute;
		readonly LifeService _lifes;
		readonly PositionService _positions;

		readonly int _kernelSolve;
		readonly ComputeBuffer _keys;
		readonly HashService _hashes;
		readonly BitonicMergeSort _sort;
		readonly GridService _grid;

		public HashGrid(ComputeShader compute, ComputeShader computeSort, LifeService l, PositionService p, GridService.Grid g) {
			var capacity = l.Lifes.count;
			_compute = compute;
			_lifes = l;
			_positions = p;

			_kernelSolve = compute.FindKernel (ShaderConst.KERNEL_SOVLE_COLLISION_DETECTION);
			_sort = new BitonicMergeSort(computeSort);
			_keys = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(uint)));
			CollisionData = new Collision[capacity];
			Collisions = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(Collision)));
			_hashes = new HashService(compute, l, p);
			_grid = new GridService(compute, g, _hashes);
		}

		public void Detect(float distance) {
			int x = _lifes.SimSizeX, y = _lifes.SimSizeY, z = _lifes.SimSizeZ;

			_hashes.Init(_grid);

			_sort.Init(_keys);
			_sort.SortInt(_keys, _hashes.Hashes);

			_grid.Construct(_keys);

			_grid.SetParams(_compute);
			_compute.SetFloat(ShaderConst.PROP_BROADPHASE_SQR_DISTANCE, distance * distance);
			_compute.SetBuffer(_kernelSolve, ShaderConst.BUF_BROADPHASE_KEYS, _keys);
			_compute.SetBuffer(_kernelSolve, ShaderConst.BUF_POSITION, _positions.P0);
			_grid.SetBuffer(_compute, _kernelSolve);
			SetBuffer(_compute, _kernelSolve);
			_compute.Dispatch(_kernelSolve, x, y, z);
		}

		public void SetBuffer(ComputeShader c, int kernel) {
			c.SetBuffer(kernel, ShaderConst.BUF_COLLISIONS, Collisions);
		}
		public void SetGlobal() {
			Shader.SetGlobalBuffer (ShaderConst.BUF_COLLISIONS, Collisions);
		}
		public void Download() {
			Collisions.GetData (CollisionData);
		}


		#region IDisposable implementation
		public void Dispose () {
			if (_sort != null)
				_sort.Dispose();
			if (_keys != null)
				_keys.Dispose();
			if (_hashes != null)
				_hashes.Dispose();
			if (_grid != null)
				_grid.Dispose ();
			if (Collisions != null)
				Collisions.Dispose();
		}
		#endregion
	}
}