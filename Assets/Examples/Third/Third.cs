using UnityEngine;
using System.Collections;
using ParticlePhysics;
using System.Text;

public class Third : MonoBehaviour {
	public const string PROP_ID = "_Id";

	public int header = 0;
	public int capacity = 1024;
	public ComputeShader compute;
	public ComputeShader computeSort;
	public Transform[] emitters;
	public Transform[] wallColliders;
	public ConstantService.ConstantData constants;

	public GameObject containerfab;
	public GameObject[] particlefabs;

	public KeyCode keyAdd = KeyCode.A;
	public KeyCode keyReadLifes = KeyCode.L;
	public KeyCode keyReadVelocities = KeyCode.V;
	public KeyCode keyReadPositions = KeyCode.P;
	public KeyCode keyReadWalls = KeyCode.W;
	public float accumulationInterval;

	PositionService _positions;
	VelocityService _velocities;
	LifeService _lifes;
	WallService _walls;
	ConstantService _constants;
	VelocitySimulation _velSimulation;
	PositionSimulation _posSimulation;
	WallCollisionSolver _wallSolver;
	ParticleCollisionSolver _particleSolver;
	BoundsChecker _boundsChecker;
	BroadPhase _broadphase;
	MeshCombiner _combiner;
	bool _iterativeAcumulation = false;

	void Start () {
		_positions = new PositionService(compute, capacity);
		_velocities = new VelocityService(compute, capacity);
		_lifes = new LifeService(compute, capacity);
		_constants = new ConstantService(constants);
		_velSimulation = new VelocitySimulation(compute, _velocities);
		_posSimulation = new PositionSimulation(compute, _velocities, _positions);
		_broadphase = new BroadPhase(compute, computeSort, _lifes, _positions);
		_particleSolver = new ParticleCollisionSolver(compute, _velocities, _positions, _lifes, _broadphase);
		_boundsChecker = new BoundsChecker(compute, _lifes, _positions);
		_walls = BuildWalls(wallColliders);
		_wallSolver = new WallCollisionSolver(compute, _velocities, _positions, _walls);

		var particles = new GameObject[capacity];
		foreach (var p in particlefabs)
			p.transform.localScale = 2f * constants.radius * Vector3.one;
		for (var i = 0; i < capacity; i++)
			particles[i] = particlefabs[Random.Range(0, particlefabs.Length)];
		_combiner = new MeshCombiner(containerfab);
		_combiner.Rebuild(particles);
		_combiner.SetParent(transform, false);

		UpdateConstantData();
		StartCoroutine(ParticleAccumulator());
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
		if (_broadphase != null)
			_broadphase.Dispose();
		if (_wallSolver != null)
			_wallSolver.Dispose();
		if (_particleSolver != null)
			_particleSolver.Dispose();
		if (_combiner != null)
			_combiner.Dispose();
	}
	
	void Update () {
		if (Input.GetKeyDown(keyAdd))
			_iterativeAcumulation = !_iterativeAcumulation;
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

		UpdateConstantData();
		_positions.SetGlobal();
		_lifes.SetGlobal();
	}
	void FixedUpdate() {
		_constants.SetConstants(compute, Time.fixedDeltaTime);
		_velSimulation.Simulate();
		_broadphase.FindBand(2f * constants.radius);
		for(var i = 0; i < 4; i++) {
			_particleSolver.Solve();
			_wallSolver.Solve();
			_velocities.ClampMagnitude();
		}
		_posSimulation.Simulate();
		_boundsChecker.Check();
		_lifes.Simulate();
	}

	IEnumerator ParticleAccumulator() {
		while (true) {
			yield return new WaitForSeconds(accumulationInterval);
			if (_iterativeAcumulation) {
				var posLocal = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
				var tr = emitters[Random.Range(0, emitters.Length)];
				var pos = tr.TransformPoint(posLocal);
				AddParticle(new Vector2[]{ (Vector2)pos} , 1000f);
			}
		}
	}
	void UpdateConstantData() {
		var center = (Vector2)Camera.main.transform.position;
		var h = 2f * Camera.main.orthographicSize;
		var w = Camera.main.aspect * h;
		constants.bounds = new Vector4(center.x - w, center.y - h, center.x + w, center.y + h);
	}
	void AddParticle (Vector2[] positions, float life) {
		var l = new float[positions.Length];
		for (var i = 0; i < positions.Length; i++)
			l[i] = life;
		_velocities.Upload (header, new Vector2[positions.Length]);
		_positions.Upload (header, positions);
		_lifes.Upload (header, l);
		header = (header + positions.Length) % capacity;
	}

	static WallService BuildWalls(Transform[] wallColliders) {
		var walls = new WallService (wallColliders.Length);
		foreach (var collider in wallColliders)
			walls.Add (collider);
		return walls;
	}
}
