using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	public class ParticleCollisionSolver : System.IDisposable {
		readonly ComputeShader _compute;
		readonly VelocityService _velocities;
		readonly PositionService _positions;
		readonly LifeService _lifes;
		readonly CollisionDetection _broadphase;
		readonly int _kernel;
		
		public ParticleCollisionSolver(ComputeShader compute, VelocityService v, PositionService p, LifeService l, CollisionDetection b) {
			_kernel = compute.FindKernel(ShaderConst.KERNEL_SOLVE_PARTICLE_COLLISION);
			_compute = compute;
			_velocities = v;
			_positions = p;
			_lifes = l;
			_broadphase = b;
		}
		public void Solve() {
			_velocities.SetBuffer(_compute, _kernel);
			_positions.SetBuffer(_compute, _kernel);
			_lifes.SetBuffer(_compute, _kernel);
			_broadphase.SetBuffer(_compute, _kernel);
			_compute.Dispatch(_kernel, _velocities.SimSizeX, _velocities.SimSizeY, _velocities.SimSizeZ);
			_velocities.Swap();
		}
		
		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion
	}
}