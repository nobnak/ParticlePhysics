using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	public static class ShaderConst {
		public const int WARP_SIZE = 64;
		public const int MAX_THREAD_GROUPS = 1024;
		public const int MAX_X_THREADS = WARP_SIZE * MAX_THREAD_GROUPS;

		public const string KERNEL_UPLOAD_VELOCITY = "UploadVelocity";
		public const string KERNEL_UPLOAD_POSITION = "UploadPosition";
		public const string KERNEL_SIMULATE_VELOCITY = "SimulateVelocity";
		public const string KERNEL_SIMULATE_POSITION = "SimulatePosition";

		public const string PROP_UPLOAD_OFFSET = "uploadOffset";
		public const string PROP_UPLOAD_LENGTH = "uploadLength";
		public const string PROP_DELTA_TIME = "dt";

		public const string BUF_VELOCITY = "Velocities";
		public const string BUF_POSITION = "Positions";
		public const string BUF_UPLOADER = "Uploader";
	}
}