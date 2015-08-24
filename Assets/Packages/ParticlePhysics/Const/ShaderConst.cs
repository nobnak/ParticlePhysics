using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	public static class ShaderConst {
		public const int WARP_SIZE = 64;
		public const int MAX_THREAD_GROUPS = 1024;
		public const int MAX_X_THREADS = WARP_SIZE * MAX_THREAD_GROUPS;

		public const string KERNEL_UPLOAD_VELOCITY = "UploadVelocity";
		public const string KERNEL_UPLOAD_POSITION = "UploadPosition";
		public const string KERNEL_UPLOAD_LIFE = "UploadLife";
		public const string KERNEL_SIMULATE_VELOCITY = "SimulateVelocity";
		public const string KERNEL_SIMULATE_POSITION = "SimulatePosition";
		public const string KERNEL_SIMULATE_LIFE = "SimulateLife";
		public const string KERNEL_SOLVE_WALL_COLLISION = "SolveWallCollision";
		public const string KERNEL_SOLVE_PARTICLE_COLLISION = "SolveParticleCollision";

		public const string PROP_UPLOAD_OFFSET = "uploadOffset";
		public const string PROP_UPLOAD_LENGTH = "uploadLength";
		public const string PROP_DELTA_TIME = "dt";
		public const string PROP_WALL_COUNT = "wallCount";
		public const string PROP_ELASTICS = "elastics";

		public const string BUF_VELOCITY_CURR = "VelocitiesCurr";
		public const string BUF_VELOCITY_NEXT = "VelocitiesNext";
		public const string BUF_POSITION = "Positions";
		public const string BUF_UPLOADER_FLOAT2 = "Uploader";
		public const string BUF_UPLOADER_FLOAT = "UploaderFloat";
		public const string BUF_WALL = "Walls";
		public const string BUF_LIFE = "Lifes";
	}
}