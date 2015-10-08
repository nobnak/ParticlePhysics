using UnityEngine;
using System.Collections;

namespace InputTracking {
	public class MouserTrackerTest : MonoBehaviour {
		MouseTracker _tracker;

		void Start () {
			_tracker = new MouseTracker();
			_tracker.OnDrag += OnDrag;
			_tracker.OnClick += OnClick;
			_tracker.OnDoubleClick += OnDClick;
		}
		
		void Update () {
			_tracker.Update();
		}

		void OnDrag() {
			Debug.Log("On Drag");
		}
		void OnClick() {
			Debug.Log("On Click");
		}
		void OnDClick() {
			Debug.Log("On Double Click");
		}
	}
}