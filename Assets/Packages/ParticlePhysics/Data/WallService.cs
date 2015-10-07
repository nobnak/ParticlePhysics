using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ParticlePhysics {
	public class WallService : System.IDisposable {
		public readonly int capacity;

		public int Count { get { return _colliders.Count; } }
		public ComputeBuffer Walls { get; private set; }

		List<Transform> _colliders;
		Wall[] _walls;

		public WallService(int capacity) {
			this.capacity = capacity;
			_colliders = new List<Transform>(capacity);
			_walls = new Wall[capacity];
			Walls = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(Wall)));
			Walls.SetData(_walls);
		}
		public void Add(Transform collider) { _colliders.Add(collider); }
		public void Clear() { _colliders.Clear(); }
		public void Update() {
			for (var i = 0; i < _colliders.Count; i++)
				_walls[i] = Convert(_colliders[i]);
			Walls.SetData(_walls);
		}
		public void SetBuffer(ComputeShader compute, int kernel) {
			compute.SetInt(ShaderConst.PROP_WALL_COUNT, _colliders.Count);
			compute.SetBuffer(kernel, ShaderConst.BUF_WALL, Walls);
		}
		public Wall[] Download() {
			var walls = new Wall[Walls.count];
			Walls.GetData(walls);
			return walls;
		}

		public static Wall Convert(Transform collider) {
			var n = ((Vector2)collider.up).normalized;
			var t = ((Vector2)collider.right).normalized;
			var p = (Vector2)collider.position;
			var w = collider.lossyScale.x * 0.5f;
			var h = collider.lossyScale.y * 0.5f;
			return new WallService.Wall () {
				n = n, t = t, dn = Vector2.Dot (n, p), dt = Vector2.Dot (t, p), w = w, h = h
			};
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

			public override string ToString () {
				return string.Format("n={0},Dn={1},t={2},Dt={3},WxH={4}x{5}", n, dn, t, dt, w, h);
			}
		}
	}
}