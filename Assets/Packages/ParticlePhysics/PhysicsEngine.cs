using UnityEngine;
using System.Collections;
using System.Text;

namespace ParticlePhysics {
	public class PhysicsEngine : MonoBehaviour {
		public const string PROP_ID = "_Id";

		// Params
		public bool particleCollisionEnabled = false;
		public int capacity = 1024;
		public ConstantService.ConstantData constants;

		public int header = 0;
		public ComputeShader compute;
		public ComputeShader computeSort;
		public PolygonCollider[] polygonColliders;

		public Camera targetCamera;
		public GameObject containerfab;
		public GameObject[] particlefabs;

		public bool Initialized { get { return _initialized; } }
		public PositionService Positions { get; private set; }
		public VelocityService Velocities { get; private set; }
		public LifeService Lifes { get; private set; }
		public PolygonColliderService Polygons { get; private set; }
		public ConstantService Constants { get; private set; }
		public VelocitySimulation VelSimulation { get; private set; }
		public PositionSimulation PosSimulation { get; private set; }
		public PolygonCollisionSolver PolygonSolver { get; private set; }
		public ParticleCollisionSolver ParticleSolver { get; private set; }
		public BoundsChecker BoundsChecker { get; private set; }
		public ICollisionDetection Collisions { get; private set; }
		public MeshCombiner Combiner { get; private set; }

		bool _initialized = false;
		
		void Start () {
			capacity = ShaderUtil.PowerOfTwo(capacity);

			Positions = new PositionService(compute, capacity);
			Velocities = new VelocityService(compute, capacity);
			Lifes = new LifeService(compute, capacity);
			Constants = new ConstantService(capacity, constants);
			VelSimulation = new VelocitySimulation(compute, Velocities, Constants);
			PosSimulation = new PositionSimulation(compute, Velocities, Positions);
			Collisions = new HashGrid(compute, computeSort, Lifes, Positions, GenerateGrid());
			ParticleSolver = new ParticleCollisionSolver(compute, Velocities, Positions, Lifes, Collisions);
			BoundsChecker = new BoundsChecker(compute, Lifes, Positions);
			Polygons = new PolygonColliderService();
			PolygonSolver = new PolygonCollisionSolver(compute, Velocities, Positions, Lifes, Polygons);
			
			var particles = new GameObject[capacity];
			foreach (var pfab in particlefabs)
				pfab.transform.localScale = 2f * constants.radius * Vector3.one;
			Profiler.BeginSample("Instantiate Particles");
			for (var i = 0; i < capacity; i++) {
				var pfab = particlefabs[Random.Range(0, particlefabs.Length)];
				particles[i] = (GameObject)Instantiate(pfab, Vector3.zero, Random.rotationUniform);
			}
			Profiler.EndSample();
			Combiner = new MeshCombiner(containerfab);
			Combiner.Rebuild(particles);
			Combiner.SetParent(transform, false);
			for (var i = 0; i < capacity; i++)
				Destroy(particles[i]);
			
			UpdateConstantData();
			_initialized = true;
		}
		void OnDestroy() {
			if (Positions != null)
				Positions.Dispose();
			if (Velocities != null)
				Velocities.Dispose();
			if (Lifes != null)
				Lifes.Dispose();
			if (Constants != null)
				Constants.Dispose();
			if (Polygons != null)
				Polygons.Dispose();
			if (VelSimulation != null)
				VelSimulation.Dispose();
			if (PosSimulation != null)
				PosSimulation.Dispose();
			if (Collisions != null)
				Collisions.Dispose();
			if (PolygonSolver != null)
				PolygonSolver.Dispose();
			if (ParticleSolver != null)
				ParticleSolver.Dispose();
			if (Combiner != null)
				Combiner.Dispose();
		}
		
		void Update () {
			UpdateConstantData();
			Positions.SetGlobal();
			Lifes.SetGlobal();
			Collisions.SetGlobal ();
			Polygons.UpdatePolygons(polygonColliders);
		}
		void FixedUpdate() {
			Constants.SetConstants(compute, constants.FixedDeltaTime);
			VelSimulation.Simulate();
			if (particleCollisionEnabled)
				Collisions.Detect(2f * constants.radius);
			for(var i = 0; i < 10; i++) {
				PolygonSolver.Solve();
				if (particleCollisionEnabled)
					ParticleSolver.Solve();
				Velocities.ClampMagnitude();
			}
			PosSimulation.Simulate();
			BoundsChecker.Check();
			Lifes.Simulate();
		}

		public void AddParticle (Vector2[] positions, float life) {
			var l = new float[positions.Length];
			for (var i = 0; i < positions.Length; i++)
				l[i] = life;
			Velocities.Upload (header, new Vector2[positions.Length]);
			Positions.Upload (header, positions);
			Lifes.Upload (header, l);
			header = (header + positions.Length) % capacity;
		}
		public GridService.Grid GenerateGrid() {
			var h = 2f * targetCamera.orthographicSize;
			var w = h * targetCamera.aspect;
			var diam = 2f * constants.radius;
			var nx = Mathf.FloorToInt(w / diam);
			var ny = Mathf.FloorToInt (h / diam);
			return new GridService.Grid(){ nx = nx, ny = ny, w = w, h = h };
		}
		void UpdateConstantData() {
			var center = (Vector2)targetCamera.transform.position;
			var h = 2f * targetCamera.orthographicSize;
			var w = targetCamera.aspect * h;
			constants.bounds = new Vector4(center.x - w, center.y - h, center.x + w, center.y + h);
		}
	}
}