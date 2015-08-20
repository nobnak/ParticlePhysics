using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	public class VelocitySimulation : System.IDisposable {
		readonly ComputeShader _compute;
		readonly VelocityService _velocities;
		readonly int _kernelSimulate;

		public VelocitySimulation(ComputeShader compute, VelocityService v) {
			_kernelSimulate = compute.FindKernel(ShaderConst.KERNEL_SIMULATE_VELOCITY);
			_compute = compute;
			_velocities = v;
		}

		public void Simulate(float dt) {
			_compute.SetFloat(ShaderConst.PROP_DELTA_TIME, Time.deltaTime);
			_velocities.SetBuffer(_compute, _kernelSimulate);
			_compute.Dispatch(_kernelSimulate, _velocities.SimSizeX, _velocities.SimSizeY, _velocities.SimSizeZ);
			_velocities.Swap();
		}
		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion
	}
}
