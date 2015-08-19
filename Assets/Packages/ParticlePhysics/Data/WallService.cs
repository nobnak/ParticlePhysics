using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ParticlePhysics {
	public class WallService : System.IDisposable {
		public readonly int capacity;

		public ComputeBuffer Walls { get; private set; }

		List<Wall> _walls;

		public WallService(int capacity) {
			this.capacity = capacity;
			_walls = new List<Wall>(capacity);
			Walls = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(Wall)));
			Walls.SetData(_walls.ToArray());
		}
		public void Add(Wall w) {
			_walls.Add(w);
			Walls.SetData(_walls.ToArray());
		}
		public void SetBuffer(ComputeShader compute, int kernel) {
			compute.SetInt(ShaderConst.PROP_WALL_COUNT, _walls.Count);
			compute.SetBuffer(kernel, ShaderConst.BUF_WALL, Walls);
		}

		#region IDisposable implementation
		public void Dispose () {
			if (Walls != null)
				Walls.Dispose();
		}
		#endregion

		[StructLayout(LayoutKind.Sequential)]
		public struct Wall {
			public Vector2 n;
			public Vector2 t;
			public float dn;
			public float dt;
			public float w;
			public float h;
		}
	}
}