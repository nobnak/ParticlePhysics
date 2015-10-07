using UnityEngine;
using System.Collections;
using System.Text;

namespace ParticlePhysics {
	public class PolygonColliderTest : MonoBehaviour {
		public PolygonCollider[] colliders;

		PolygonColliderService _service;

		void Start() {
			_service = new PolygonColliderService();
		}
		void OnDestroy() {
			if (_service != null)
				_service.Dispose();
		}
		void Update () {
			for (var i = 0; i < colliders.Length; i++)
				colliders[i].ManualUpdate();
			_service.UpdatePolygons(colliders);

			_service.GetData();
			var plogger = new StringBuilder();
			var slogger = new StringBuilder();
			plogger.Append("Polygons : ");
			slogger.Append("Segments : ");
			var polygons = _service.Polygons;
			var segments = _service.Segments;
			for (var i = 0; i < _service.Polygons.Length; i++) {
				var p = polygons[i];
				plogger.Append(p);
				for (var j = 0; j < p.segmentCount; j++) {
					var s = segments[j + p.segmentIndex];
					slogger.Append(s);
				}
			}
			Debug.Log(plogger);
			Debug.Log(slogger);
		}
	}
}