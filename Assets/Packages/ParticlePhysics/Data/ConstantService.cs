using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	public class ConstantService : System.IDisposable {
		readonly ConstantData _data;

		public ConstantService(ConstantData d) {
			_data = d;
		}

		public void SetConstants(ComputeShader compute, float dt) {
			compute.SetFloat(ShaderConst.PROP_DELTA_TIME, dt);
			compute.SetFloat(ShaderConst.PROP_ELASTICS, _data.elastics);
			compute.SetFloat(ShaderConst.PROP_FRICTION, _data.friction);
			compute.SetFloat(ShaderConst.PROP_DRAG, _data.dragCoeff);
			compute.SetFloat(ShaderConst.PROP_PARTICLE_RADIUS, _data.radius);
			compute.SetVector(ShaderConst.PROP_BOUNDS, _data.bounds);
		}

		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion

		[System.Serializable]
		public class ConstantData {
			public float elastics = 0f;
			public float friction = 0.2f;
			public float dragCoeff = 0.01f;
			public float radius = 1f;
			public Vector4 bounds = new Vector4(-100f, -100f, 100f, 100f);
		}
	}
}