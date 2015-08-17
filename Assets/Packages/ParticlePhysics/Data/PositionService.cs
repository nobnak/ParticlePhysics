using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {	
	public class PositionService : System.IDisposable {
		readonly ComputeShader _compute;

		Vector2[] _positions;
		ComputeBuffer _p0, _p1;

		public PositionService(ComputeShader compute, int capasity) {
			_compute = compute;
			_positions = new Vector2[capasity];
			_p0 = new ComputeBuffer(capasity, Marshal.SizeOf(_positions[0]));
			_p1 = new ComputeBuffer(capasity, Marshal.SizeOf(_positions[1]));
		}

		#region IDisposable implementation
		public void Dispose () {
			if (_p0 != null)
				_p0.Dispose();
			if (_p1 != null)
				_p1.Dispose();
		}
		#endregion		
	}
}
