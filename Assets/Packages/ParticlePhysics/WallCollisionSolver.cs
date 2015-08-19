using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class WallCollisionSolver : System.IDisposable {
		readonly ComputeShader _compute;
		readonly VelocityService _velocities;
		readonly WallService _walls;

		public WallCollisionSolver(ComputeShader compute, VelocityService v, WallService w) {
			_compute = compute;
			_velocities = v;
			_walls = w;
		}

		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion

	}
}