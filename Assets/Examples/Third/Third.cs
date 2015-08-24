using UnityEngine;
using System.Collections;
using ParticlePhysics;
using System.Text;

public class Third : MonoBehaviour {
	public const string PROP_ID = "_Id";

	public int header = 0;
	public int capacity = 1024;
	public ComputeShader compute;
	public GameObject particlefab;
	public Transform emitter;
	public Transform[] wallColliders;
	public ConstantService.ConstantData constants;
	public KeyCode keyAdd = KeyCode.A;
	public KeyCode keyReadLifes = KeyCode.L;
	public KeyCode keyReadVelocities = KeyCode.V;
	public KeyCode keyReadPositions = KeyCode.P;
	public KeyCode keyReadWalls = KeyCode.W;

	PositionService _positions;
	VelocityService _velocities;
	LifeService _lifes;
	WallService _walls;
	ConstantService _constants;
	VelocitySimulation _velSimulation;
	PositionSimulation _posSimulation;
	WallCollisionSolver _wallSolver;
	ParticleCollisionSolver _particleSolver;

	void Start () {
		_positions = new PositionService(compute, capacity);
		_velocities = new VelocityService(compute, capacity);
		_lifes = new LifeService(compute, capacity);
		_constants = new ConstantService(constants);
		_velSimulation = new VelocitySimulation(compute, _velocities);
		_posSimulation = new PositionSimulation(compute, _velocities, _positions);
		_particleSolver = new ParticleCollisionSolver(compute, _velocities, _positions, _lifes);

		_walls = BuildWalls(wallColliders);
		_wallSolver = new WallCollisionSolver(compute, _velocities, _positions, _walls);
	}
	void OnDestroy() {
		if (_positions != null)
			_positions.Dispose();
		if (_velocities != null)
			_velocities.Dispose();
		if (_lifes != null)
			_lifes.Dispose();
		if (_constants != null)
			_constants.Dispose();
		if (_walls != null)
			_walls.Dispose();
		if (_velSimulation != null)
			_velSimulation.Dispose();
		if (_posSimulation != null)
			_posSimulation.Dispose();
		if (_wallSolver != null)
			_wallSolver.Dispose();
		if (_particleSolver != null)
			_particleSolver.Dispose();
	}
	
	void Update () {
		if (Input.GetKeyDown(keyAdd)) {
			var inst = (GameObject)Instantiate(particlefab);
			inst.transform.SetParent(transform, false);
			inst.transform.localScale = (2f * constants.radius) * Vector3.one;
			var mat = inst.GetComponent<Renderer>().material;
			mat.SetInt(PROP_ID, header % capacity);
			_velocities.Upload(header, new Vector2[]{ new Vector2(0f, 0f) });
			_positions.Upload(header, new Vector2[]{ (Vector2)emitter.position });
			_lifes.Upload(header, new float[]{ 1000f });
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
		if (Input.GetKeyDown(keyReadLifes)) {
			var buf = new StringBuilder("Lifes:");
			foreach (var l in _lifes.Download())
				buf.AppendFormat("{0},", l);
			Debug.Log(buf);
		}
		if (Input.GetKeyDown(keyReadWalls)) {
			var buf = new StringBuilder("Walls:");
			foreach (var w in _walls.Download())
				buf.AppendFormat("{0},", w.ToString());
			Debug.Log(buf);
		}

		_constants.SetConstants(compute);
		_velSimulation.Simulate(Time.deltaTime);
		for(var i = 0; i < 3; i++) {
			_particleSolver.Solve();
			_wallSolver.Solve();
		}
		_posSimulation.Simulate(Time.deltaTime);
		_lifes.Simulate(Time.deltaTime);

		_positions.SetGlobal();
		_lifes.SetGlobal();
	}

	static WallService BuildWalls(Transform[] wallColliders) {
		var walls = new WallService (wallColliders.Length);
		foreach (var collider in wallColliders)
			walls.Add (collider);
		return walls;
	}
}
