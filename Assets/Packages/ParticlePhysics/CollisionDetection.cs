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

	public class SweepAndPrune : ICollisionDetection {
		public ComputeBuffer Collisions { get; private set; }

		readonly ComputeShader _compute;
		readonly ComputeBuffer _keys;
		readonly ComputeBuffer _ys;
		readonly LifeService _lifes;
		readonly PositionService _positions;
		readonly BitonicMergeSort _sort;
		readonly int _kernelInitYs, _kernelSolve;

		public SweepAndPrune(ComputeShader compute, ComputeShader computeSort, LifeService l, PositionService p) {
			var capacity = l.Lifes.count;
			_lifes = l;
			_positions = p;
			_kernelInitYs = compute.FindKernel(ShaderConst.KERNEL_INIT_BROADPHASE);
			_kernelSolve = compute.FindKernel (ShaderConst.KERNEL_SOVLE_BROADPHASE);
			_compute = compute;
			_sort = new BitonicMergeSort(computeSort);
			_keys = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(uint)));
			_ys = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(float)));
			Collisions = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(Collision)));
		}

		public void Detect(float distance) {
			int x = _lifes.SimSizeX, y = _lifes.SimSizeY, z = _lifes.SimSizeZ;
			_compute.SetBuffer(_kernelInitYs, ShaderConst.BUF_LIFE, _lifes.Lifes);
			_compute.SetBuffer(_kernelInitYs, ShaderConst.BUF_POSITION, _positions.P0);
			_compute.SetBuffer(_kernelInitYs, ShaderConst.BUF_Y, _ys);
			_compute.Dispatch(_kernelInitYs, x, y, z);

			_sort.Init(_keys);
			_sort.Sort(_keys, _ys);

			_compute.SetFloat(ShaderConst.PROP_BROADPHASE_SQR_DISTANCE, distance * distance);
			_compute.SetBuffer(_kernelSolve, ShaderConst.BUF_Y, _ys);
			_compute.SetBuffer(_kernelSolve, ShaderConst.BUF_BROADPHASE_KEYS, _keys);
			_compute.SetBuffer(_kernelSolve, ShaderConst.BUF_POSITION, _positions.P0);
			SetBuffer(_compute, _kernelSolve);
			_compute.Dispatch(_kernelSolve, x, y, z);
		}
		public void SetBuffer(ComputeShader c, int kernel) {
			c.SetBuffer(kernel, ShaderConst.BUF_COLLISIONS, Collisions);
		}

		#region IDisposable implementation
		public void Dispose () {
			if (_sort != null)
				_sort.Dispose();
			if (_keys != null)
				_keys.Dispose();
			if (_ys != null)
				_ys.Dispose();
			if (Collisions != null)
				Collisions.Dispose();
		}
		#endregion
	}
}