using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class WallCollisionSolver : System.IDisposable {
		readonly ComputeShader _compute;
		readonly VelocityService _velocities;
		readonly PositionService _positions;
		readonly WallService _walls;
		readonly int _kernel;

		public WallCollisionSolver(ComputeShader compute, VelocityService v, PositionService p, WallService w) {
			_kernel = compute.FindKernel(ShaderConst.KERNEL_SOLVE_WALL_COLLISION);
			_compute = compute;
			_velocities = v;
			_positions = p;
			_walls = w;
		}
		public void Solve() {
			_velocities.SetBuffer(_compute, _kernel);
			_positions.SetBuffer(_compute, _kernel);
			_walls.SetBuffer(_compute, _kernel);
			_compute.Dispatch(_kernel, _velocities.SimSizeX, _velocities.SimSizeY, _velocities.SimSizeZ);
		}

		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion

	}
}