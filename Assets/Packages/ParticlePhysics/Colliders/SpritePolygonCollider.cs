using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace ParticlePhysics {
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpritePolygonCollider : PolygonCollider {
		public float degenerationDistance = 0.1f;

		SpriteRenderer _spriteRend;
		ushort[] _outlines;
		
		void OnDrawGizmos() {
			_spriteRend = GetComponent<SpriteRenderer> ();
			ManualUpdate();
			DrawGizmos ();
		}
		void OnEnable() {
			_spriteRend = GetComponent<SpriteRenderer> ();
			UpdateSegment ();
		}

		#region implemented abstract members of PolygonCollider
		public override void ManualUpdate () {
			UpdateSegment ();
		}
		#endregion

		void UpdateSegment() {
			_effective = false;

			if (_spriteRend == null || _spriteRend.sprite == null)
				return;
			if (_outlines == null)
				_outlines = FindOutlines (_spriteRend.sprite.vertices, _spriteRend.sprite.triangles);
			var len = _outlines.Length;
			if (len == 0)
				return;
			if (_segments == null || _segments.Length != len)
				_segments = new PolygonColliderService.Segment[len];

			var sprite = _spriteRend.sprite;
			var vertices = sprite.vertices;
			var t0 = _outlines [0];
			var from = transform.TransformPoint(vertices[t0]);
			InitBoundary (from);
			for (var i = 0; i < len; i++) {
				from = transform.TransformPoint(vertices [t0]);
				var t1 = _outlines[(i+1) % len];
				var to = transform.TransformPoint(vertices[t1]);
				t0 = t1;
				_segments [i] = PolygonColliderService.Segment.Generate (from, to);
				ExpandBoundary(from);
				ExpandBoundary(to);
			}

			_effective = true;
		}

		ushort[] FindOutlines(Vector2[] vertices, ushort[] triangles) {
			var halfEdges = new HashSet<HalfEdge> ();
			for (var i = 0; i < triangles.Length; i+=3) {
				for (var j = 0; j < 3; j++) {
					var t0 = triangles[j + i];
					var t1 = triangles[(j + 1) % 3 + i];
					var h = new HalfEdge(t0, t1);
					halfEdges.Add(h);
				}
			}
			var outlines = new Dictionary<ushort, HalfEdge> ();
			var t = (ushort)0;
			foreach (var h0 in halfEdges) {
				var h1 = h0.Opposite();
				if (!halfEdges.Contains(h1))
					outlines.Add(t = h0.t0, h0);
			}

			var result = new List<ushort> (outlines.Count * 2);
			while (outlines.Count > 0 && outlines.ContainsKey(t)) {
				var h = outlines[t];
				outlines.Remove(t);
				result.Add(h.t0);
				t = h.t1;
			}

			var k = 0;
			while (k < result.Count) {
				var t0 = result[k];
				var t1 = result[(k + 1) % outlines.Count];
				var d = vertices[t0] - vertices[t1];
				if (d.sqrMagnitude < (degenerationDistance * degenerationDistance))
					result.RemoveAt(k);
				else
					k++;
			}

			return result.ToArray ();
		}

		public class HalfEdge {
			public ushort t0;
			public ushort t1;

			public HalfEdge(ushort t0, ushort t1) {
				this.t0 = t0;
				this.t1 = t1;
			}

			public HalfEdge Opposite() {
				return new HalfEdge (t1, t0);
			}

			public override bool Equals (object obj) {
				if (!(obj is HalfEdge))
					return false;
				var b = (HalfEdge)obj;
				return b.t0 == t0 && b.t1 == t1;
			}
			public override int GetHashCode () {
				return t0 + t1 * 787;
			}
		}
	}
}