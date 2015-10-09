using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class OutlinePolygonCollider : PolygonCollider {
		public Vector2[] vertices;

		void OnDrawGizmos() {
			ManualUpdate();
			if (!_effective)
				return;


			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(_bounds.center, _bounds.size);
			Gizmos.color = Color.white;
			for (var i = 0; i < _segments.Length; i++) {
				var s = _segments[i];
				if (s.Validate()) {
					Gizmos.DrawRay(s.from, s.length * s.t);
				} else {
					Gizmos.color = Color.red;
					#if UNITY_EDITOR
					var size = UnityEditor.HandleUtility.GetHandleSize(s.from);
					Gizmos.DrawSphere(s.from, 0.2f * size);
					#endif
				}
			}
			Gizmos.color = Color.green;
			for (var i = 0; i < _segments.Length; i++) {
				var s = _segments[i];
				Gizmos.DrawRay(s.from + 0.5f * s.length * s.t, s.n);
			}
		}

		public override void ManualUpdate() {
			UpdateSegments();
		}

		void UpdateSegments () {
			_effective = false;
			if (vertices == null)
				return;
			if (_segments == null || _segments.Length != vertices.Length)
				_segments = new PolygonColliderService.Segment[vertices.Length];
			if (_segments.Length < 2)
				return;

			var from = transform.TransformPoint (vertices [vertices.Length - 1]);
			InitBoundary(from);
			for (var i = 0; i < vertices.Length; i++) {
				var to = transform.TransformPoint (vertices [i]);
				_segments[i] = PolygonColliderService.Segment.Generate (from, to);
				ExpandBoundary(to);
				from = to;
			}
			_effective = true;
		}
	}
}