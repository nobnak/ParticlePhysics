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
			DrawGizmos ();
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