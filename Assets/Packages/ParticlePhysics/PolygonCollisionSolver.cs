using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	public class PolygonCollisionSolver : System.IDisposable {
		readonly ComputeShader _compute;
		readonly int _kernel;
		readonly VelocityService _velocities;
		readonly PositionService _positions;
		readonly LifeService _lifes;
		readonly PolygonColliderService _polygons;

		public PolygonCollisionSolver(ComputeShader c, VelocityService v, PositionService p, LifeService l, PolygonColliderService poly) {
			_compute = c;
			_kernel = c.FindKernel(ShaderConst.KERNEL_SOLVE_POLYGON_COLLISION);
			_velocities = v;
			_positions = p;
			_lifes = l;
			_polygons = poly;
		}
		public void Solve() {
			if (_polygons.PolygonCount == 0)
				return;

			_velocities.SetBuffer(_compute, _kernel);
			_positions.SetBuffer(_compute, _kernel);
			_lifes.SetBuffer(_compute, _kernel);
			_polygons.SetBuffer(_compute, _kernel);
			_compute.Dispatch(_kernel, _velocities.SimSizeX, _velocities.SimSizeY, _velocities.SimSizeZ);
			_velocities.Swap();

		}

		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion
	}
}