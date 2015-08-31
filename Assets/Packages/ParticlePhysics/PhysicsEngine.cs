using UnityEngine;
using System.Collections;
using System.Text;

namespace ParticlePhysics {
	public class PhysicsEngine : MonoBehaviour {
		
		public const string PROP_ID = "_Id";
		
		public int header = 0;
		public int capacity = 1024;
		public ComputeShader compute;
		public ComputeShader computeSort;
		public Transform[] wallColliders;
		public ConstantService.ConstantData constants;
		
		public GameObject containerfab;
		public GameObject[] particlefabs;
		
		public PositionService Positions { get; private set; }
		public VelocityService Velocities { get; private set; }
		public LifeService Lifes { get; private set; }
		public WallService Walls { get; private set; }
		public ConstantService Constants { get; private set; }
		public VelocitySimulation VelSimulation { get; private set; }
		public PositionSimulation PosSimulation { get; private set; }
		public WallCollisionSolver WallSolver { get; private set; }
		public ParticleCollisionSolver ParticleSolver { get; private set; }
		public BoundsChecker BoundsChecker { get; private set; }
		public BroadPhase Broadphase { get; private set; }
		public MeshCombiner Combiner { get; private set; }
		
		void Start () {
			Positions = new PositionService(compute, capacity);
			Velocities = new VelocityService(compute, capacity);
			Lifes = new LifeService(compute, capacity);
			Constants = new ConstantService(constants);
			VelSimulation = new VelocitySimulation(compute, Velocities);
			PosSimulation = new PositionSimulation(compute, Velocities, Positions);
			Broadphase = new BroadPhase(compute, computeSort, Lifes, Positions);
			ParticleSolver = new ParticleCollisionSolver(compute, Velocities, Positions, Lifes, Broadphase);
			BoundsChecker = new BoundsChecker(compute, Lifes, Positions);
			Walls = BuildWalls(wallColliders);
			WallSolver = new WallCollisionSolver(compute, Velocities, Positions, Walls);
			
			var particles = new GameObject[capacity];
			foreach (var p in particlefabs)
				p.transform.localScale = 2f * constants.radius * Vector3.one;
			for (var i = 0; i < capacity; i++)
				particles[i] = particlefabs[Random.Range(0, particlefabs.Length)];
			Combiner = new MeshCombiner(containerfab);
			Combiner.Rebuild(particles);
			Combiner.SetParent(transform, false);
			
			UpdateConstantData();
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
			if (Walls != null)
				Walls.Dispose();
			if (VelSimulation != null)
				VelSimulation.Dispose();
			if (PosSimulation != null)
				PosSimulation.Dispose();
			if (Broadphase != null)
				Broadphase.Dispose();
			if (WallSolver != null)
				WallSolver.Dispose();
			if (ParticleSolver != null)
				ParticleSolver.Dispose();
			if (Combiner != null)
				Combiner.Dispose();
		}
		
		void Update () {
			UpdateConstantData();
			Positions.SetGlobal();
			Lifes.SetGlobal();
		}
		void FixedUpdate() {
			Constants.SetConstants(compute, Time.fixedDeltaTime);
			VelSimulation.Simulate();
			Broadphase.FindBand(2f * constants.radius);
			for(var i = 0; i < 4; i++) {
				ParticleSolver.Solve();
				WallSolver.Solve();
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
		
		void UpdateConstantData() {
			var center = (Vector2)Camera.main.transform.position;
			var h = 2f * Camera.main.orthographicSize;
			var w = Camera.main.aspect * h;
			constants.bounds = new Vector4(center.x - w, center.y - h, center.x + w, center.y + h);
		}
		static WallService BuildWalls(Transform[] wallColliders) {
			var walls = new WallService (wallColliders.Length);
			foreach (var collider in wallColliders)
				walls.Add (collider);
			return walls;
		}
	}
}