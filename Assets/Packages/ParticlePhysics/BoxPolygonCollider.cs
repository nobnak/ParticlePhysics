using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class BoxPolygonCollider : PolygonCollider {
		public static readonly Vector2[] CORNERS = new Vector2[]{
			new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, -0.5f)
		};

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
			if (_segments == null)
				_segments = new PolygonColliderService.Segment[4];

			var from = transform.TransformPoint(CORNERS[3]);
			InitBoundary(from);
			for (var i = 0; i < CORNERS.Length; i++) {
				var to = transform.TransformPoint (CORNERS[i]);
				_segments[i] = PolygonColliderService.Segment.Generate (from, to);
				ExpandBoundary(to);
				from = to;
			}
			_effective = true;
		}
	}
}