using UnityEngine;
using System.Collections;
using ParticlePhysics;
using System.Text;

public class Second : MonoBehaviour {
	public const string PROP_ID = "_Id";

	public int header = 0;
	public int capacity = 1024;
	public ComputeShader compute;
	public GameObject particlefab;
	public KeyCode keyAdd;
	public KeyCode keyReadVelocities;
	public KeyCode keyReadPositions;

	PositionService _positions;
	VelocityService _velocities;
	LifeService _lifes;
	VelocitySimulation _velSimulation;
	PositionSimulation _posSimulation;

	void Start () {
		_positions = new PositionService(compute, capacity);
		_velocities = new VelocityService(compute, capacity);
		_lifes = new LifeService(compute, capacity);
		_velSimulation = new VelocitySimulation(compute, _velocities);
		_posSimulation = new PositionSimulation(compute, _velocities, _positions);
	}
	void OnDestroy() {
		if (_positions != null)
			_positions.Dispose();
		if (_velocities != null)
			_velocities.Dispose();
		if (_lifes != null)
			_lifes.Dispose();
		if (_velSimulation != null)
			_velSimulation.Dispose();
		if (_posSimulation != null)
			_posSimulation.Dispose();
	}
	
	void Update () {
		if (Input.GetKeyDown(keyAdd)) {
			var inst = (GameObject)Instantiate(particlefab);
			inst.transform.SetParent(transform, false);
			var mat = inst.GetComponent<Renderer>().material;
			mat.SetInt(PROP_ID, header % capacity);
			_velocities.Upload(header, new Vector2[]{ new Vector2(0f, 0f) });
			_positions.Upload(header, new Vector2[]{ new Vector2(0.2f * header, 0f) });
			_lifes.Upload(header, new float[]{ 1f });
			header++;
		}
		if (Input.GetKeyDown (keyReadPositions)) {
			var buf = new StringBuilder("Positions:");
			foreach (var p in _positions.Download()) {
				buf.AppendFormat("{0},", p);
			}
			Debug.Log(buf);
		}
		if (Input.GetKeyDown (keyReadVelocities)) {
			var buf = new StringBuilder("Velocities:");
			foreach (var v in _positions.Download()) {
				buf.AppendFormat("{0},", v);
			}
			Debug.Log(buf);
		}

		_velSimulation.Simulate(Time.deltaTime);
		_posSimulation.Simulate(Time.deltaTime);
		_lifes.Simulate(Time.deltaTime);

		_positions.SetGlobal();
		_lifes.SetGlobal();
	}
}
