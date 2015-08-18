using UnityEngine;
using System.Collections;
using ParticlePhysics;
using System.Text;

public class First : MonoBehaviour {
	public const string PROP_ID = "_Id";

	public int header = 0;
	public int capasity = 1024;
	public ComputeShader compute;
	public GameObject particlefab;
	public KeyCode keyAdd;
	public KeyCode keyRead;

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
			mat.SetInt(PROP_ID, header % capasity);
			_positions.Upload(header, new Vector2[]{ new Vector2(0.2f * header, 0f) });
			header++;
		}
		if (Input.GetKeyDown (keyRead)) {
			var buf = new StringBuilder("Positions:");
			foreach (var p in _positions.Download()) {
				buf.AppendFormat("{0},", p);
			}
			Debug.Log(buf);
		}

		_positions.SetGlobal();
	}
}
