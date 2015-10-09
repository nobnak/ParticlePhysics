using UnityEngine;
using UnityEditor;
using System.Collections;
using InputTracking;

namespace ParticlePhysics {
	[CustomEditor(typeof(PolygonCollider))]
	public class PolygonColliderEditor : Editor {
		PolygonCollider _polygon;
		Vector2[] _vertices;

		void OnEnable() {
			_polygon = target as PolygonCollider;
		}

		public void OnSceneGUI() {
			if (_polygon == null || _polygon.Segments == null)
				return;

			Load();			
			EditorGUI.BeginChangeCheck();
			for (var i = 0; i < _vertices.Length; i++) {
				var v = _vertices[i];
				var worldPos = _polygon.transform.TransformPoint(v);
				worldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
				_vertices[i] = _polygon.transform.InverseTransformPoint(worldPos);
			}
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(_polygon, "Move Segment");
				EditorUtility.SetDirty(_polygon);
				Save();
			}
		}

		void Load () {
			if (_vertices == null || _vertices.Length != _polygon.vertices.Length)
				_vertices = new Vector2[_polygon.vertices.Length];
			System.Array.Copy (_polygon.vertices, _vertices, _polygon.vertices.Length);
		}
		void Save () {
			System.Array.Copy (_vertices, _polygon.vertices, _vertices.Length);
		}
	}
}