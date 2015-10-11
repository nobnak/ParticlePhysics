using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class OutlinePolygonCollider : PolygonCollider {
		public Vector2[] vertices;

		void OnDrawGizmos() {
			ManualUpdate();
			DrawGizmos ();
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