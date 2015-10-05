using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParticlePhysics {
	public class PolygonColliderService : System.IDisposable {
		public const int DEFAULT_POLYGON_COUNT = 1;
		public const int DEFAULT_SEGMENT_COUNT = 64;
		int _nPolygons = 0;
		Polygon[] _polygonData;
		Segment[] _segmentData;
		ComputeBuffer _Polygons;
		ComputeBuffer _Segments;

		public int PolygonCount { get { return _nPolygons; } }
		public Polygon[] Polygons { get { return _polygonData; } }
		public Segment[] Segments { get { return _segmentData; } }

		public PolygonColliderService() {
			Init(DEFAULT_POLYGON_COUNT, DEFAULT_SEGMENT_COUNT);
		}

		public void UpdatePolygons(PolygonCollider[] colliders) {
			for (var i = 0; i < colliders.Length; i++)
				colliders[i].ManualUpdate();

			_nPolygons = colliders.Length;
			var nSegments = 0;
			for (var i = 0; i < colliders.Length; i++)
				nSegments += colliders[i].Segments.Length;
			if (_Polygons.count < _nPolygons || _Segments.count < nSegments) {
				Release();
				Init (_nPolygons, nSegments);
			}

			var segmentIndex = 0;
			for (var i = 0; i < colliders.Length; i++) {
				var c = colliders[i];
				var segments = c.Segments;
				_polygonData[i] = Polygon.Generate(c.BoundingBox, segmentIndex, segments.Length);
				for (var j = 0; j < segments.Length; j++)
					_segmentData[segmentIndex++] = segments[j];
			}
			SetData();
		}
		public void SetBuffer(ComputeShader compute, int kernel) {
			compute.SetInt(ShaderConst.PROP_POLYGON_COUNT, _nPolygons);
			compute.SetBuffer(kernel, ShaderConst.BUF_POLYGONS, _Polygons);
			compute.SetBuffer(kernel, ShaderConst.BUF_SEGMENTS, _Segments);
		}
		public void SetData() {
			_Polygons.SetData(_polygonData);
			_Segments.SetData(_segmentData);
		}
		public void GetData() {
			_Polygons.GetData(_polygonData);
			_Segments.GetData(_segmentData);
		}

		void Release() {
			if (_Polygons != null)
				_Polygons.Release();
			if (_Segments != null)
				_Segments.Release();
		}

		void Init (int polygonCapacity, int segmentsCapacity) {
			_polygonData = new Polygon[polygonCapacity];
			_Polygons = new ComputeBuffer (polygonCapacity, Marshal.SizeOf (typeof(Polygon)));
			_segmentData = new Segment[segmentsCapacity];
			_Segments = new ComputeBuffer (segmentsCapacity, Marshal.SizeOf (typeof(Segment)));
			SetData();
		}

		#region IDisposable implementation
		public void Dispose () {
			Release();
		}
		#endregion

		[StructLayout(LayoutKind.Sequential)]
		public struct Polygon {
			public Vector4 bounds;
			public int segmentIndex;
			public int segmentCount;

			public static Polygon Generate(Rect boundingBox, int segmentIndex, int segmentCount) {
				var center = boundingBox.center;
				var expand = 0.5f * boundingBox.size;
				return new Polygon() {
					bounds = new Vector4(center.x, center.y, expand.x, expand.y),
					segmentIndex = segmentIndex,
					segmentCount = segmentCount
				};
			}

			public override string ToString () {
				return string.Format("<bounds={0}, segmentIndex={1}, segmentCount={2}>",
				                     bounds, segmentIndex, segmentCount);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Segment {
			public Vector2 from;
			public float length;
			public Vector2 n;
			public Vector2 t;
			
			public Segment(Vector2 from, float length, Vector2 n, Vector2 t) {
				this.from = from; this.length = length; this.n = n; this.t = t;
			}
			
			public static bool Generate(Vector2 from, Vector2 to, out Segment segment) {
				var dir = to - from;
				var length = dir.magnitude;
				var t = dir.normalized;
				var n = new Vector2(-t.y, t.x);
				if (length <= float.Epsilon) {
					Debug.LogErrorFormat("Length is zero : {0}", dir);
					segment = default(Segment);
					return false;
				}
				segment = new Segment(from, length, n, t);
				return true;
			}

			public override string ToString () {
				return string.Format("<from={0}, length={1}, n={2}, t={3}>",
				                     from, length, n, t);
			}
		}

	}
}
