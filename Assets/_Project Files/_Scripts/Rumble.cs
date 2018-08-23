using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rumble : MonoBehaviour {

	// --- GAMEPLAY VARIABLES

	[Header("Camera Settings")]
	[SerializeField] public float _flCameraSpeed = 100f;

	[Header("Noise Variables")]
	[SerializeField] public bool _isRumbling = false;
	[SerializeField] private float _flRumbleMax = 10f;
	[SerializeField] private float _flRumbleLerp = 1f;

	[Header("Current Rumble")]
	[SerializeField] public float _flRumbling;

	// Update is called once per frame
	void LateUpdate () {

		Vector3 v3CameraPos = new Vector3 (0f,0f,0f);

		// Append the Camera Shake effect if it's in use.

		// Lerp into, and out of, the rumble effect for added smoothness.
		if (_isRumbling) {
			_flRumbling = Mathf.Lerp (_flRumbling, _flRumbleMax, Time.deltaTime * _flRumbleLerp * 3f);
		} else if (_flRumbling > 0) {
			_flRumbling = Mathf.Lerp (_flRumbling, 0f, Time.deltaTime * _flRumbleLerp);
		}

		// If we're at a non-zero rumble, apply the shake to the camera's trajectory.
		if (_flRumbling != 0) {
			v3CameraPos += new Vector3 (0.4f-Mathf.PerlinNoise (Random.insideUnitCircle.x,Random.insideUnitCircle.y), 0.4f-Mathf.PerlinNoise (Random.insideUnitCircle.x,Random.insideUnitCircle.y), 0f) * _flRumbling;
		}

		// Lerp to the final position.
		transform.position = Vector3.Lerp (transform.position, new Vector3 (v3CameraPos.x, v3CameraPos.y, transform.position.z), Time.deltaTime * _flCameraSpeed);

	}

}
