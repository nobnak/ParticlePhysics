using UnityEngine;
using UnityEditor;
using System.Collections;
using InputTracking;

namespace ParticlePhysics {
	[CustomEditor(typeof(PolygonCollider))]
	public class PolygonColliderEditor : Editor {
		PolygonCollider _polygon;

		void OnEnable() {
			_polygon = target as PolygonCollider;
		}

		public void OnSceneGUI() {
			if (_polygon == null || _polygon.Segments == null)
				return;

			EditorGUI.BeginChangeCheck();

			var vertices = _polygon.vertices;
			for (var i = 0; i < vertices.Length; i++) {
				var v = vertices[i];
				var worldPos = _polygon.transform.TransformPoint(v);
				worldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
				vertices[i] = _polygon.transform.InverseTransformPoint(worldPos);
			}

			if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(_polygon);
		}
	}
}