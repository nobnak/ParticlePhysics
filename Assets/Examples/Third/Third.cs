using UnityEngine;
using System.Collections;
using ParticlePhysics;
using System.Text;

public class Third : MonoBehaviour {
	public KeyCode keyAdd = KeyCode.A;
	public KeyCode keyReadLifes = KeyCode.L;
	public KeyCode keyReadVelocities = KeyCode.V;
	public KeyCode keyReadPositions = KeyCode.P;
	public KeyCode keyReadWalls = KeyCode.W;

	public PhysicsEngine physics;
	public Transform[] emitters;
	public float accumulationInterval;

	bool _iterativeAcumulation = false;

	void Start () {
		StartCoroutine(ParticleAccumulator());
	}
	
	void Update () {
		if (Input.GetKeyDown(keyAdd))
			_iterativeAcumulation = !_iterativeAcumulation;
		if (Input.GetKeyDown (keyReadPositions)) {
			var buf = new StringBuilder("Positions:");
			foreach (var p in physics.Positions.Download()) {
				buf.AppendFormat("{0},", p);
			}
			Debug.Log(buf);
		}
		if (Input.GetKeyDown (keyReadVelocities)) {
			var buf = new StringBuilder("Velocities:");
			foreach (var v in physics.Positions.Download()) {
				buf.AppendFormat("{0},", v);
			}
			Debug.Log(buf);
		}
		if (Input.GetKeyDown(keyReadLifes)) {
			var buf = new StringBuilder("Lifes:");
			foreach (var l in physics.Lifes.Download())
				buf.AppendFormat("{0},", l);
			Debug.Log(buf);
		}
		if (Input.GetKeyDown(keyReadWalls)) {
			var buf = new StringBuilder("Walls:");
			foreach (var w in physics.Walls.Download())
				buf.AppendFormat("{0},", w.ToString());
			Debug.Log(buf);
		}

	}
	IEnumerator ParticleAccumulator() {
		while (true) {
			yield return new WaitForSeconds(accumulationInterval);
			if (_iterativeAcumulation) {
				var posLocal = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
				var tr = emitters[Random.Range(0, emitters.Length)];
				var pos = tr.TransformPoint(posLocal);
				physics.AddParticle(new Vector2[]{ (Vector2)pos} , 1000f);
			}
		}
	}
}
