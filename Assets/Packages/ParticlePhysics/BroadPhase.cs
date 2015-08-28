using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using MergeSort;

namespace ParticlePhysics {
	public class BroadPhase : System.IDisposable {
		public ComputeBuffer Keys { get; private set; }
		public ComputeBuffer Bands { get; private set; }

		readonly ComputeShader _compute;
		readonly ComputeBuffer _ys;
		readonly LifeService _lifes;
		readonly PositionService _positions;
		readonly BitonicMergeSort _sort;
		readonly int _kernelInitYs;

		public BroadPhase(ComputeShader compute, ComputeShader computeSort, LifeService l, PositionService p) {
			var capacity = l.Lifes.count;
			_lifes = l;
			_positions = p;
			_kernelInitYs = compute.FindKernel(ShaderConst.KERNEL_INIT_BROADPHASE);
			_compute = compute;
			_sort = new BitonicMergeSort(computeSort);
			Keys = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(uint)));
			_ys = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(float)));
			Bands = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(Band)));
		}

		public void FindBand(float distance) {
			_compute.SetBuffer(_kernelInitYs, ShaderConst.BUF_LIFE, _lifes.Lifes);
			_compute.SetBuffer(_kernelInitYs, ShaderConst.BUF_POSITION, _positions.P0);
			_compute.SetBuffer(_kernelInitYs, ShaderConst.BUF_Y, _ys);
			_compute.Dispatch(_kernelInitYs, _lifes.SimSizeX, _lifes.SimSizeY, _lifes.SimSizeZ);

			_sort.Init(Keys);
			_sort.Sort(Keys, _ys);


		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Band {
			public uint start;
			public uint end;
		}

		#region IDisposable implementation
		public void Dispose () {
			if (_sort != null)
				_sort.Dispose();
			if (Keys != null)
				Keys.Dispose();
			if (_ys != null)
				_ys.Dispose();
			if (Bands != null)
				Bands.Dispose();
		}
		#endregion
	}
}