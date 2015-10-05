using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	public class VelocitySimulation : System.IDisposable {
		readonly ComputeShader _compute;
		readonly VelocityService _velocities;
		readonly ConstantService _constants;
		readonly int _kernelSimulate;

		public VelocitySimulation(ComputeShader compute, VelocityService v, ConstantService c) {
			_kernelSimulate = compute.FindKernel(ShaderConst.KERNEL_SIMULATE_VELOCITY);
			_compute = compute;
			_velocities = v;
			_constants = c;
		}

		public void Simulate() {
			_velocities.SetBuffer(_compute, _kernelSimulate);
			_constants.SetBuffer (_compute, _kernelSimulate);
			_compute.Dispatch(_kernelSimulate, _velocities.SimSizeX, _velocities.SimSizeY, _velocities.SimSizeZ);
			_velocities.Swap();
		}
		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion
	}
}
