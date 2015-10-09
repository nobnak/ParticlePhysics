using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	public static class ShaderConst {
		public const int WARP_SIZE = 64;
		public const int MAX_THREAD_GROUPS = 1024;
		public const int MAX_X_THREADS = WARP_SIZE * MAX_THREAD_GROUPS;
		public const int COLLIDER_CAPACITY = 9 * 4;
		public const int MAX_WALL_COUNT = 64;

		public const string KERNEL_UPLOAD_VELOCITY = "UploadVelocity";
		public const string KERNEL_UPLOAD_POSITION = "UploadPosition";
		public const string KERNEL_UPLOAD_LIFE = "UploadLife";
		public const string KERNEL_SIMULATE_VELOCITY = "SimulateVelocity";
		public const string KERNEL_SIMULATE_POSITION = "SimulatePosition";
		public const string KERNEL_SIMULATE_LIFE = "SimulateLife";
		public const string KERNEL_SOLVE_WALL_COLLISION = "SolveWallCollision";
		public const string KERNEL_SOLVE_POLYGON_COLLISION = "SolvePolygonCollision";
		public const string KERNEL_SOLVE_PARTICLE_COLLISION = "SolveParticleCollision";
		public const string KERNEL_CLAMP_VELOCITY = "ClampVelocity";
		public const string KERNEL_CHECK_BOUNDS = "CheckBounds";
		public const string KERNEL_INIT_HASHES = "InitHashes";
		public const string KERNEL_INIT_GRID = "InitGrid";
		public const string KERNEL_CONSTRUCT_GRID = "ConstructGrid";
		public const string KERNEL_SOVLE_COLLISION_DETECTION = "SolveCollisionDetection";

		public const string PROP_UPLOAD_OFFSET = "uploadOffset";
		public const string PROP_UPLOAD_LENGTH = "uploadLength";
		public const string PROP_DELTA_TIME = "dt";
		public const string PROP_WALL_COUNT = "wallCount";
		public const string PROP_ELASTICS = "elastics";
		public const string PROP_FRICTION = "friction";
		public const string PROP_DRAG = "dragCoeff";
		public const string PROP_PARTICLE_RADIUS = "particleRadius";
		public const string PROP_PENETRATION_BIAS = "penetrationBias";
		public const string PROP_BOUNDS = "bounds";
		public const string PROP_BROADPHASE_SQR_DISTANCE = "broadphaseSqrDistance";
		public const string PROP_HASH_GRID_NX = "grid_nx";
		public const string PROP_HASH_GRID_NY = "grid_ny";
		public const string PROP_HASH_GRID_PARAMS = "gridParams"; // grid_w, grid_h, grid_dxdw, grid_dydh;
		public const string PROP_POLYGON_COUNT = "polygonCount";

		public const string BUF_VELOCITY_CURR = "VelocitiesCurr";
		public const string BUF_VELOCITY_NEXT = "VelocitiesNext";
		public const string BUF_POSITION = "Positions";
		public const string BUF_UPLOADER_FLOAT2 = "Uploader";
		public const string BUF_UPLOADER_FLOAT = "UploaderFloat";
		public const string BUF_WALL = "Walls";
		public const string BUF_LIFE = "Lifes";
		public const string BUF_HASHES = "Hashes";
		public const string BUF_GRID_START = "GridStarts";
		public const string BUF_GRID_END = "GridEnds";
		public const string BUF_COLLISIONS = "Collisions";
		public const string BUF_BROADPHASE_KEYS = "BroadphaseKeys";
		public const string BUF_DRAG_COEFFS = "DragCoeffs";
		public const string BUF_POLYGONS = "Polygons";
		public const string BUF_SEGMENTS = "Segments";
	}
}