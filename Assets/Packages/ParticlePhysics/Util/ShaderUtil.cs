using UnityEngine;
using System.Collections;


namespace ParticlePhysics {	
	public static class ShaderUtil {

		public static void CalcWorkSize(int length, out int x, out int y, out int z) {
			if (length <= ShaderConst.MAX_X_THREADS) {
				x = (length - 1) / ShaderConst.WARP_SIZE + 1;
				y = z = 1;
			} else {
				x = ShaderConst.MAX_THREAD_GROUPS;
				y = (length - 1) / ShaderConst.MAX_X_THREADS + 1;
				z = 1;
			}
			//Debug.LogFormat("WorkSize {0}x{1}x{2}", x, y, z);
		}
		public static int AlignBufferSize(int length) {
			return ((length - 1) / ShaderConst.WARP_SIZE + 1) * ShaderConst.WARP_SIZE;
		}
	}
}
