using UnityEngine;
using System.Collections;
using ParticlePhysics;
using System.Text;

public class First : MonoBehaviour {
	public const string PROP_ID = "_Id";

	public int header = 0;
	public int capacity = 1024;
	public ComputeShader compute;
	public GameObject particlefab;
	public KeyCode keyAdd = KeyCode.A;
	public KeyCode keyReadPosition = KeyCode.P;
	public KeyCode keyReadLife = KeyCode.L;

	PositionService _positions;
	LifeService _lifes;

	void Start() {
		_positions = new PositionService (compute, capacity);
		_lifes = new LifeService(compute, capacity);
	}
	void OnDestroy() {
		if (_positions != null)
			_positions.Dispose();
		if (_lifes != null)
			_lifes.Dispose();
	}

	void Update() {
		if (Input.GetKeyDown(keyAdd)) {
			var inst = (GameObject)Instantiate(particlefab);
			inst.transform.SetParent(transform, false);
			var mat = inst.GetComponent<Renderer>().material;
			mat.SetInt(PROP_ID, header % capacity);
			_positions.Upload(header, new Vector2[]{ new Vector2(0.2f * header, 0f) });
			_lifes.Upload(header, new float[]{ 10f });
			header++;
		}
		if (Input.GetKeyDown (keyReadPosition)) {
			var buf = new StringBuilder("Positions:");
			foreach (var p in _positions.Download()) {
				buf.AppendFormat("{0},", p);
			}
			Debug.Log(buf);
		}
		if (Input.GetKeyDown(keyReadLife)) {
			var buf = new StringBuilder("Lifes:");
			foreach(var l in _lifes.Download())
				buf.AppendFormat("{0},", l);
			Debug.Log(buf);
		}

		_lifes.Simulate(Time.deltaTime);

		_positions.SetGlobal();
		_lifes.SetGlobal();
	}
}
