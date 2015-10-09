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
	}
}