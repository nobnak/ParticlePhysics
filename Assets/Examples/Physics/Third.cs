using UnityEngine;
using System.Collections;
using ParticlePhysics;
using System.Text;

public class Third : MonoBehaviour {
	public bool fall = true;
	public float particleGenerationSpeed = 100f;
	public float particleLife = 100f;
	public Vector2 angularSpeedRange = new Vector2(0f, 1f);

	public PhysicsEngine physics;
	public ComputeShader computeRotation;
	public Transform[] emitters;

	float _reservedParticles = 0f;
	RotationService _rotation;

	IEnumerator Start() {
		while (!physics.Initialized)
			yield return null;
		_rotation = new RotationService(computeRotation, physics.Velocities, physics.Lifes, 
		                                physics.constants.radius, angularSpeedRange);
	}
	void OnDestroy() {
		if (_rotation != null)
			_rotation.Dispose();
	}
	void Update () {
		if (_rotation == null)
			return;

		GenerateParticles();

		_rotation.VelocityBasedRotate(Time.deltaTime);
		_rotation.SetGlobal();
	}
	void GenerateParticles() {
		if (fall)
			_reservedParticles += particleGenerationSpeed * Time.deltaTime;

		var bulk = 0;
		while ((bulk = Mathf.Min((int)_reservedParticles, ShaderConst.WARP_SIZE)) > 1) {
			_reservedParticles -= bulk;

			var posisions = new Vector2[bulk];
			for (var i = 0; i < posisions.Length; i++) {
				var posLocal = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
				var tr = emitters[Random.Range(0, emitters.Length)];
				posisions[i] = tr.TransformPoint(posLocal);
			}
			physics.AddParticle(posisions , particleLife);
		}
	}
}
