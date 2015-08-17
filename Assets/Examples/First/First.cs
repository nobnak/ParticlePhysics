using UnityEngine;
using System.Collections;
using ParticlePhysics;

public class First : MonoBehaviour {
	public const string PROP_ID = "Id";

	public int header = 0;
	public int capasity = 1024;
	public ComputeShader compute;
	public GameObject particlefab;
	public KeyCode keyAdd;

	PositionService _positions;

	void Start() {
		_positions = new PositionService (compute, capasity);
	}
	void OnDestroy() {
		if (_positions != null)
			_positions.Dispose();
	}

	void Update() {
		if (Input.GetKeyDown(keyAdd)) {
			var inst = (GameObject)Instantiate(particlefab);
			inst.transform.SetParent(transform, false);
			var mat = inst.GetComponent<Renderer>().material;
			mat.SetInt(PROP_ID, header);
			_positions.Upload(header, new Vector2[]{ new Vector2(header, 0f) });
			header++;
		}


		_positions.SetGlobal();
	}
}
