using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
   Author: 			Hayden Reeve
   File:			MenuWatcher.cs
   Description: 	This script allows image and text UI elements to fade in and out of existance in a chained format. 
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

public class Fade : MonoBehaviour {

	// The settings that are unique to each object! You must set these individually for every object in a chain.
	[Header("Unique Fade Settings")]

	[Tooltip("Do not set this as true for the first object in a chain! Checking this makes the current object inherit from the linked object.")] 
	[SerializeField] private bool _isAChain = false;

	[Tooltip("If the object is a chain, select the object that came FIRST in the chain.")]
	[SerializeField] private GameObject _gmChainPrimary;

	[Tooltip("What number is this object in the chain, if it is chained? Take into consideration, the ChainPrimary is number 0.")]
	[SerializeField] private float _flNumberInChain = 0f;

	[Tooltip("Stops the object from fading out.")] 
	[SerializeField] private bool _isNotFading = false;

	[Tooltip("If the fade is a Unity UI Text Element, or a Unity UI Image Element.")]
	[SerializeField] private bool _isText = true;

	[Tooltip("Stops the object from being deleted. This does not stop it from fading out! Useful if the object needs to be used later on, but you still want it to fade.")]
	[SerializeField] private bool _isPermanent = false;

	// The settings that are -overwritten- by the previous object if it is a chain. These must be set by the first object in a chain!
	[Header("Virtual Fade Settings")]

	[Tooltip("When the object should fade in. This is inherited from the object previous if it is in a chain.")]
	[SerializeField] private float _flFadeInAt = -1f;

	[Tooltip("How long the object should remain on the screen for. This is inherited from the object previous if it is in a chain.")]
	[SerializeField] private float _flDuration = 0f;

	[Tooltip("How long should the object take to fade in and out from the screen? This is inherited from the object previous if it is in a chain.")]
	[SerializeField] private float _flFadeSpeed = 2f;

	[Tooltip("If the object is in a chain, how long should the delay between further chains. This is inherited from the object previous if it is in a chain.")]
	[SerializeField] private float _flChainWait = 1f;

	// Component Variables
	private Text _txToFade;
	private Image _imToFade;
	private Fade _gmChainedScript;

	// Tracking Variables
	private bool _hasFadedIn = false;
	private bool _hasFadedOut = false;

	private float _flRuntime;
	private float _flCurtime;
	private float _flDestime;

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- //

	// Use this for initialization.
	void Start () {

		// Set the current time, and grab the Object the script is attached to.
		_flRuntime = Time.time;

		if (_isText) {
			_txToFade = gameObject.GetComponent<Text> ();
		} else {
			_imToFade = gameObject.GetComponent<Image> ();
		}

		// If this script is a chain in a set of fade scripts, inherit settings from the script before it.
		if (_isAChain) {
			_gmChainedScript = _gmChainPrimary.GetComponent<Fade> ();

			// Transfer variables across, and modify them according to the chain
			_flFadeInAt = (_gmChainedScript._flDuration + _gmChainedScript._flFadeSpeed + _gmChainedScript._flChainWait) * _flNumberInChain + _gmChainedScript._flFadeInAt;
			_flDuration = _gmChainedScript._flDuration;
			_flFadeSpeed = _gmChainedScript._flFadeSpeed;
			_flChainWait = _gmChainedScript._flChainWait;
		}

		// If the user wants to have the text fade in after the scene has begun, make sure it's invisible to start with.
		if (_flFadeInAt > 0) {

			if (_isText) {
				_txToFade.enabled = true;
				_txToFade.canvasRenderer.SetAlpha (0f);
			} else {
				_imToFade.enabled = true;
				_imToFade.canvasRenderer.SetAlpha (0f);
			}
		}

	}
	
	// Update is called once per frame.
	void Update () {
	
		// Update current time.
		_flCurtime = Time.time - _flRuntime;

		// Handle the "Fade In" script once the user's set time has been reached.
		if (_flCurtime > _flFadeInAt && !_hasFadedIn) {
			_hasFadedIn = true;

			if (_isText) {
				_txToFade.CrossFadeAlpha (1.0f, _flFadeSpeed, false);
			} else {
				_imToFade.CrossFadeAlpha (1.0f, _flFadeSpeed, false);
			}

			// If the manual override, "willNotFade" has been checked, stop the script here.
			if (_isNotFading == true) {
				gameObject.GetComponent<Fade> ().enabled = false;
			}
		}
			
		// Handle the "Fade Out" script once the user's set time has been reached.
		if (_flCurtime > (_flFadeInAt + _flDuration) && !_hasFadedOut && !_isNotFading ) {
			_flDestime = _flCurtime + (_flFadeSpeed);
			_hasFadedOut = true;

			if (_isText) {
				_txToFade.CrossFadeAlpha (0.0f, _flFadeSpeed, false);
			} else {
				_imToFade.CrossFadeAlpha (0.0f, _flFadeSpeed, false);
			}

		}

		// Once the object has been faded out, destroy it if it's not permanent, or disable it if it is.
		if ( _flCurtime > _flDestime && _hasFadedOut && !_isPermanent) {
			Destroy (gameObject);
		} else if ( _flCurtime > _flDestime && _hasFadedOut && _isPermanent) {
			_flRuntime = 0f;
			_hasFadedIn = false; _hasFadedOut = false;
			gameObject.GetComponent<Fade> ().enabled = false;
		}

	}
}
