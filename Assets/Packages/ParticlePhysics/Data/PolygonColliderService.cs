using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

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
			Init(0, DEFAULT_POLYGON_COUNT, DEFAULT_SEGMENT_COUNT);
		}

		public void UpdatePolygons(PolygonCollider[] colliders) {
			_nPolygons = colliders.Length;
			for (var i = 0; i < _nPolygons; i++)
				colliders[i].ManualUpdate();

			var nSegments = 0;
			for (var i = 0; i < _nPolygons; i++)
				nSegments += colliders[i].Segments.Length;
			if (_Polygons.count < _nPolygons || _Segments.count < nSegments) {
				Release();
				Init (_nPolygons, _nPolygons, nSegments);
			}

			var segmentIndex = 0;
			for (var i = 0; i < _nPolygons; i++) {
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

		void Init (int polygonCount, int polygonCapacity, int segmentsCapacity) {
			polygonCapacity = Mathf.Max(polygonCapacity, DEFAULT_POLYGON_COUNT);
			segmentsCapacity = Mathf.Max(segmentsCapacity, DEFAULT_SEGMENT_COUNT);

			_nPolygons = polygonCount;
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
			public Vector2 n;
			public Vector2 t;
			public float length;

			public Segment(Vector2 from, float length, Vector2 n, Vector2 t) {
				this.from = from; this.length = length; this.n = n; this.t = t;
			}

			public bool Validate() {
				return length > float.Epsilon;
			}
			
			public static Segment Generate(Vector2 from, Vector2 to) {
				var dir = to - from;
				var length = dir.magnitude;
				var t = dir.normalized;
				var n = new Vector2(-t.y, t.x);
				return new Segment(from, length, n, t);
			}

			public override string ToString () {
				return string.Format("<from={0}, length={1}, n={2}, t={3}>",
				                     from, length, n, t);
			}
		}

	}
}
