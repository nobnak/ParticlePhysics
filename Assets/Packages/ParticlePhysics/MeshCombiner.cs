using UnityEngine;
using System.Collections.Generic;

namespace ParticlePhysics {
	public class MeshCombiner : System.IDisposable {
		public const int VERTEX_LIMIT = 65534;

		public GameObject[] Combined { get; private set; }

		readonly GameObject _container;

		public MeshCombiner(GameObject container) {
			_container = container;
		}

		public void Rebuild(IEnumerable<GameObject> particles) {
			Release();

			var combined = new List<GameObject>();
			foreach (var group in SplitInGroups(Convert(particles))) {
				var go = (GameObject)GameObject.Instantiate(_container);
				go.GetComponent<MeshFilter>().sharedMesh = Combine(group);
				combined.Add(go);
			}
			Combined = combined.ToArray();
		}
		public void SetParent(Transform parent, bool worldPositionStays) {
			foreach (var go in Combined)
				go.transform.SetParent(parent, worldPositionStays);
		}
		public void Release() {
			if (Combined == null)
				return;

			foreach (var go in Combined) {
				var mesh = go.GetComponent<MeshFilter>().sharedMesh;
				GameObject.Destroy(mesh);
				GameObject.Destroy(go);
			}
			Combined = null;
		}
		public static IEnumerable<Source> Convert(IEnumerable<GameObject> gos) {
			var id = 0;
			foreach (var go in gos) {
				var mesh = go.GetComponent<MeshFilter>().sharedMesh;
				var source = new Source(id++, go.transform, mesh);
				yield return source;
			}
		}
		public static IEnumerable<Source[]> SplitInGroups(IEnumerable<Source> sources) {
			var result = new List<Source[]>();
			var group = new List<Source>();
			var vertexCount = 0;
			foreach (var source in sources) {
				if ((vertexCount + source.mesh.vertexCount) > VERTEX_LIMIT) {
					vertexCount = 0;
					result.Add(group.ToArray());
					group.Clear();
					continue;
				}
				vertexCount += source.mesh.vertexCount;
				group.Add(source);
			}
			if (group.Count > 0)
				result.Add(group.ToArray());
			return result;
		}
		public static Mesh Combine(Source[] sources) {
			var totalVertices = 0;
			var totalTriangles = 0;
			foreach (var source in sources) {
				totalVertices += source.mesh.vertexCount;
				totalTriangles += source.mesh.triangles.Length;
			}

			var vertices = new Vector3[totalVertices];
			var normals = new Vector3[totalVertices];
			var uvs = new Vector2[totalVertices];
			var uv2s = new Vector2[totalVertices];
			var triangles = new int[totalTriangles];

			var vertexCount = 0;
			var triangleCount = 0;
			foreach (var source in sources) {
				var mesh = source.mesh;

				var tri = mesh.triangles;
				for (var j = 0; j < tri.Length; j++)
					triangles[triangleCount++] = tri[j] + vertexCount;

				var vs = mesh.vertices;
				var ns = mesh.normals;
				var uv = mesh.uv;
				var uv2 = new Vector2(source.id, 0f);
				var mat = source.transform.localToWorldMatrix;
				var matit = mat.inverse.transpose;
				for (var i = 0; i < mesh.vertexCount; i++) {
					vertices[vertexCount] = mat.MultiplyPoint3x4(vs[i]);
					normals[vertexCount] = matit.MultiplyVector(ns[i]);
					uvs[vertexCount] = uv[i];
					uv2s[vertexCount] = uv2;
					vertexCount++;
				}
			}

			var combined = new Mesh();
			combined.name = "CombinedMesh";
			combined.vertices = vertices;
			combined.normals = normals;
			combined.uv = uvs;
			combined.uv2 = uv2s;
			combined.triangles = triangles;
			combined.RecalculateBounds();
			return combined;
		}

		#region IDisposable implementation
		public void Dispose () {
			Release();
		}
		#endregion

		public struct Source {
			public int id;
			public Transform transform;
			public Mesh mesh;
			
			public Source(int id, Transform transform, Mesh mesh) {
				this.id = id;
				this.transform = transform;
				this.mesh = mesh;
			}
		}
	}
}