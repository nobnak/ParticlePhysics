using UnityEngine;
using System.Collections.Generic;
using ParticlePhysics;

public class Fourth : MonoBehaviour {
	public const float CUBIC_ROOT = 1f / 3;

	public GameObject containerfab;
	public int count = 1024;
	public float radius = 10f;
	public GameObject[] particlefabs;
	public Transform wasted;

	MeshCombiner _combiner;

	void Start () {
		_combiner = new MeshCombiner(containerfab);
		var particles = new List<GameObject>();
		var willbedestroyed = new List<GameObject>();
		for (var i = 0; i < count; i++) {
			var fab = particlefabs[Random.Range(0, particlefabs.Length)];
			var pos = radius * Mathf.Pow(Random.value, CUBIC_ROOT) * Random.onUnitSphere;
			var inst = (GameObject)Instantiate(fab, pos, Random.rotationUniform);
			if (wasted != null)
				inst.transform.SetParent(wasted, false);
			else
				willbedestroyed.Add(inst);
			particles.Add(inst);
		}
		_combiner.Rebuild(particles);
		_combiner.SetParent(transform, false);

		foreach (var go in willbedestroyed)
			Destroy(go);
	}
	void OnDestroy() {
		if (_combiner != null)
			_combiner.Dispose();
	}
}
