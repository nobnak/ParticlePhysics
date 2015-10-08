using UnityEngine;
using System.Collections;

namespace InputTracking {

	[System.Serializable]
	public class MouseTracker {
		public enum StateEnum { Default = 0, Drag, FirstDown, FirstUp, SecondDown }
		public const int MOUSE_BUTTON = 0;

		public float thresholdOfTime = 0.2f;
		public float thresholdOfPos = 1f;

		StateEnum _state = StateEnum.Default;
		float _enterTime;
		Vector3 _enterPos;

		public void Update() {
			var down = Input.GetMouseButton(MOUSE_BUTTON);
			var time = Time.timeSinceLevelLoad;
			var pos = Input.mousePosition;
			var sqrPosThresh = thresholdOfPos * thresholdOfPos;

			switch (_state) {
			default:
				if (down)
					NextState(StateEnum.FirstDown);
				_enterTime = time;
				_enterPos = pos;
				break;
			case StateEnum.FirstDown:
				if (!down) {
					NextState(StateEnum.FirstUp);
					_enterTime = time;
				} else if ((pos - _enterPos).sqrMagnitude > sqrPosThresh)
					NextState(StateEnum.Drag);
				break;
			case StateEnum.FirstUp:
				if (down) {
					NextState(StateEnum.SecondDown);
					_enterPos = pos;
				} else if ((time - _enterTime) > thresholdOfTime) {
					NextState(StateEnum.Default);
					Click ();
				}
				break;
			case StateEnum.SecondDown:
				if (!down) {
					NextState(StateEnum.Default);
					DClick();
				} else if ((pos - _enterPos).sqrMagnitude > sqrPosThresh)
					NextState(StateEnum.Drag);
				break;
			case StateEnum.Drag:
				if (down)
					Drag();
				else
					NextState(StateEnum.Default);
				break;
			}
		}

		void NextState(StateEnum next) { _state = next; }

		void Drag() {
			if (OnDrag != null)
				OnDrag();
		}
		void Click() {
			if (OnClick != null)
				OnClick();
		}
		void DClick() {
			if (OnDoubleClick != null)
				OnDoubleClick();
		}

		public event System.Action OnClick;
		public event System.Action OnDoubleClick;
		public event System.Action OnDrag;
	}
}