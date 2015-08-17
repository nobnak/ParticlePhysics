using UnityEngine;
using System.Collections;


namespace ParticlePhysics {	
	public static class ShaderUtil {

		public static void CalcWorkSize(int length, out int x, out int y, out int z) {
			if (length <= ShaderConst.MAX_X_THREADS) {
				x = (length - 1) / ShaderConst.WARP_SIZE + 1;
				y = z = 1;
				return;
			}

			x = ShaderConst.MAX_THREAD_GROUPS;
			y = (length - 1) / ShaderConst.MAX_X_THREADS + 1;
			z = 1;
		}
	}
}
