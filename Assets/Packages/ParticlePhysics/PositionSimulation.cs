using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	public class PositionSimulation : System.IDisposable {
		readonly ComputeShader _compute;
		readonly VelocityService _velocities;
		readonly PositionService _positions;
		readonly int _kernelSimulate;

		public PositionSimulation(ComputeShader compute, VelocityService v, PositionService p) {
			_kernelSimulate = compute.FindKernel(ShaderConst.KERNEL_SIMULATE_POSITION);
			_compute = compute;
			_velocities = v;
			_positions = p;
		}

		public void Simulate(float dt) {
			_compute.SetFloat(ShaderConst.PROP_DELTA_TIME, Time.deltaTime);
			_velocities.SetBuffer(_compute, _kernelSimulate);
			_positions.SetBuffer(_compute, _kernelSimulate);

			int x, y, z;
			ShaderUtil.CalcWorkSize(_positions.P0.count, out x, out y, out z);
			_compute.Dispatch(_kernelSimulate, x, y, z);
		}
		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion
	}
}
