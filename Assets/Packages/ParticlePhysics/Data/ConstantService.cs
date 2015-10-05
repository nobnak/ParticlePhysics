using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class ConstantService : System.IDisposable {
		readonly ConstantData _data;

		float _lastDragCoeff = -1f;
		float[] _dragCoeffData;
		ComputeBuffer _dragCoeffs;

		public ConstantService(ConstantData d) {
			_data = d;
			_dragCoeffData = new float[ShaderConst.WARP_SIZE];
			_dragCoeffs = new ComputeBuffer (ShaderConst.WARP_SIZE, Marshal.SizeOf (typeof(float)));
			CheckDragCoeffs();
		}

		public void SetConstants(ComputeShader compute, float dt) {
			CheckDragCoeffs ();
			compute.SetFloat(ShaderConst.PROP_DELTA_TIME, dt);
			compute.SetFloat(ShaderConst.PROP_ELASTICS, _data.elastics);
			compute.SetFloat(ShaderConst.PROP_FRICTION, _data.friction);
			compute.SetFloat(ShaderConst.PROP_PARTICLE_RADIUS, _data.radius);
			compute.SetFloat(ShaderConst.PROP_PENETRATION_BIAS, _data.penetrationBias);
			compute.SetVector(ShaderConst.PROP_BOUNDS, _data.bounds);
		}
		public void SetBuffer(ComputeShader compute, int kernel) {
			compute.SetBuffer (kernel, ShaderConst.BUF_DRAG_COEFFS, _dragCoeffs);
		}

		void CheckDragCoeffs() {
			if (_lastDragCoeff != _data.dragCoeff) {
				_lastDragCoeff = _data.dragCoeff;
				for (var i = 0; i < _dragCoeffData.Length; i++) {
					var d = _data.dragCoeff * (1f + _data.dragCoeffDeviation * Random.value);
					_dragCoeffData[i] = (d > 0f ? d : 0f);
				}
				_dragCoeffs.SetData(_dragCoeffData);
			}
		}

		#region IDisposable implementation
		public void Dispose () {
			if (_dragCoeffs != null) {
				_dragCoeffs.Dispose();
			}
		}
		#endregion

		[System.Serializable]
		public class ConstantData {
			public float timeScale = 1f;
			public float elastics = 0f;
			public float friction = 0.2f;
			public float dragCoeff = 0.01f;
			public float dragCoeffDeviation = 0.1f;
			public float radius = 1f;
			public float penetrationBias = 0.1f;
			public Vector4 bounds = new Vector4(-100f, -100f, 100f, 100f);

			public float FixedDeltaTime { get { return Time.fixedDeltaTime * timeScale; } }
			public float DeltaTime { get { return Time.deltaTime * timeScale; } }
		}
	}
}