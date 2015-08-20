using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	public class ConstantService : System.IDisposable {
		readonly ConstantData _data;

		public ConstantService(ConstantData d) {
			_data = d;
		}

		public void SetConstants(ComputeShader compute) {
			compute.SetFloat(ShaderConst.PROP_ELASTICS, _data.elastics);
		}

		#region IDisposable implementation
		public void Dispose () {
		}
		#endregion

		[System.Serializable]
		public class ConstantData {
			public float elastics;
		}
	}
}