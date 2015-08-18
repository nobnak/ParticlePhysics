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
			_compute.SetBuffer(_kernelSimulate, ShaderConst.BUF_VELOCITY, _velocities.V0);

			int x, y, z;
			ShaderUtil.CalcWorkSize(_velocities.V0.count, out x, out y, out z);
			_compute.Dispatch(_kernelSimulate, x, y, z);
		}
		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion
	}
}
