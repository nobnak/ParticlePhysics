using UnityEngine;
using System.Collections;

namespace ParticlePhysics {
	[RequireComponent(typeof(Sprite))]
	public class SpritePolygonCollider : PolygonCollider {
		Sprite _sprite;

		void OnEnable() {
			_sprite = GetComponent<Sprite> ();
		}

		#region implemented abstract members of PolygonCollider
		public override void ManualUpdate () {
			UpdateSegment ();
		}
		#endregion

		void UpdateSegment() {

		}
	}
}