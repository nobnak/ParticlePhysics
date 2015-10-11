using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public abstract class PolygonCollider : MonoBehaviour {
		protected bool _effective = false;
		protected Rect _bounds;
		protected PolygonColliderService.Segment[] _segments;

		public virtual bool Effective { get { return _effective; } }
		public virtual Rect BoundingBox { get { return _bounds; } }
		public virtual PolygonColliderService.Segment[] Segments { get { return _segments; } }

		public abstract void ManualUpdate();

		protected virtual void InitBoundary(Vector2 p) { _bounds.Set(p.x, p.y, 0f, 0f); }
		protected virtual void ExpandBoundary(Vector2 p) {
			if (p.x < _bounds.xMin)
				_bounds.xMin = p.x;
			if (p.x > _bounds.xMax)
				_bounds.xMax = p.x;
			if (p.y < _bounds.yMin)
				_bounds.yMin = p.y;
			if (p.y > _bounds.yMax)
				_bounds.yMax = p.y;
		}
		protected virtual void DrawGizmos() {
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
	}
}