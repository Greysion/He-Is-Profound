using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour {

	[Header("Speed")]
	[SerializeField] private float _flCreditRollSpeed;
	private float _flStartTime;
	private float _flTransition;

	[Header("Destination")]
	[SerializeField] private Transform _trGoal;
	private Vector3 _v3Original;

	// Init
	void Start() {

		_v3Original = transform.position;
		_flStartTime = Time.time;

	}

	// Update is called once per frame
	void Update () {

		_flTransition += Time.deltaTime * _flCreditRollSpeed;
		transform.position = Vector3.Lerp (_v3Original, _trGoal.position, _flTransition);

	}
}
