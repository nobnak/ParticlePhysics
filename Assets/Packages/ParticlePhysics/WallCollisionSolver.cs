using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class WallCollisionSolver : System.IDisposable {
		readonly ComputeShader _compute;
		readonly VelocityService _velocities;
		readonly WallService _walls;
		readonly int _kernel;

		public WallCollisionSolver(ComputeShader compute, VelocityService v, WallService w) {
			_kernel = compute.FindKernel(ShaderConst.KERNEL_SOLVE_WALL_COLLISION);
			_compute = compute;
			_velocities = v;
			_walls = w;
		}
		public void Solve() {
			_velocities.SetBuffer(_compute, _kernel);
			_walls.SetBuffer(_compute, _kernel);
		}

		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion

	}
}