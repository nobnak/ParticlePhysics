using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class BoundsChecker : System.IDisposable {
		readonly ComputeShader _compute;
		readonly LifeService _lifes;
		readonly PositionService _positions;
		readonly int _kernel;

		public BoundsChecker(ComputeShader compute, LifeService l, PositionService p) {
			_kernel = compute.FindKernel(ShaderConst.KERNEL_CHECK_BOUNDS);
			_compute = compute;
			_lifes = l;
			_positions = p;
		}
		public void Check() {
			_lifes.SetBuffer(_compute, _kernel);
			_positions.SetBuffer(_compute, _kernel);
			_compute.Dispatch(_kernel, _positions.SimSizeX, _positions.SimSizeY, _positions.SimSizeZ);
		}

		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion

	}
}