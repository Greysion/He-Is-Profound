using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
   Author: 			Hayden Reeve
   File:			MenuWatcher.cs
   Description: 	This script contains the functions used by the main menu to control the camera, change scenes, and manipulate the options settings. 
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

[RequireComponent(typeof(InputField))]

public class InputByUser : MonoBehaviour {

	// ---- Inspector Variables

	[Header("Scripts")]

	[Tooltip("Select the PrintToGame.cs script to allow the game manager to print the user's commands.")]
	[SerializeField] private PrintToGame _scPTG;

	[Tooltip("Select the Story script you wish to load, so we can communicate the player's responce to the script itself.")]
	[SerializeField] private _Story _scStory;

	[Tooltip("Select the desired rumbling objects.")]
	[SerializeField] private Rumble _scRumble;

	[Header("Visual Components")]

	[Tooltip("Select the TextHighlight object so we can enable a visual indicator that the user may type a responce.")]
	[SerializeField] private Image _imHighlight;

	[Tooltip("Select the main background image so that we may change the colours depending on the answers given by the player.")]
	[SerializeField] private Image _imBackground;

	// The three buttons that correspond to one of the three choices the player can make upon multiple choice junctions.
	[SerializeField] private Button _btAdmit;
	[SerializeField] private Button _btAvert;
	[SerializeField] private Button _btAdmonish;

	[Header("Audio Component")]

	[FMODUnity.EventRef]
	[SerializeField] private string _stKeystrokeAudio;

	[FMODUnity.EventRef]
	[SerializeField] private string _stSpacebarAudio;

	// ---- Private Variables

	private InputField _inUser; // Self attached Input Field.

	private float _flRGB = 0.00392156862f; // The division of 1 by 255.

	// Used to save and restore the original color of the background image.
	private Color _clOriginal;


	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */
	//Initialisation

	void Start () {

		_inUser = gameObject.GetComponent<InputField> ();

		_btAdmit.onClick.AddListener(Admit);
		_btAvert.onClick.AddListener(Avert);
		_btAdmonish.onClick.AddListener(Admonish);

		_clOriginal = _imBackground.color;

	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */
	// Main Responce Loop

	// Update is called once per frame.
	void Update () {

		if (_scPTG._isCleaning) {
			_imBackground.color = _clOriginal;
			_scPTG._isCleaning = false;
		}

		if (_scPTG._isFlashing) {
			StartCoroutine (FlashBackground ());
			_scPTG._isFlashing = false;
		}

		switch (_scPTG._enTurn) {

		// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

		case (Turn._input):

			if (!_inUser.isFocused || !_imHighlight.gameObject.activeInHierarchy) {
				_inUser.interactable = true;
				_imHighlight.gameObject.SetActive (true);
				_inUser.ActivateInputField ();
			}

			if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.Return)) {
				FMODUnity.RuntimeManager.PlayOneShot (_stSpacebarAudio);
			} else if (Input.anyKeyDown) {
				FMODUnity.RuntimeManager.PlayOneShot (_stKeystrokeAudio);
			}

			if (Input.GetKeyDown (KeyCode.Return)) {

				// Take the currently typed text, and add it to the dialogue backlog of the Game Manager. The commands will be interpreted by the Game Manager script.
				if (_inUser.text != "") {

					_scStory._stResponse = _inUser.text;
					_inUser.text = "";
					
				}

			}

			break;

		// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

		case (Turn._choice):

			if (!_btAdmit.gameObject.activeInHierarchy || !_btAvert.gameObject.activeInHierarchy || !_btAdmonish.gameObject.activeInHierarchy) {
				_btAdmit.gameObject.SetActive (true);
				_btAvert.gameObject.SetActive (true);
				_btAdmonish.gameObject.SetActive (true);
				FMODUnity.RuntimeManager.PlayOneShot (_stSpacebarAudio);
			}

			break;

		// --------------------------------------------------------------------------------------------------------------------------------------------------------- */
		
		case (Turn._game):

			// Player Input Interface
			_inUser.interactable = false;
			_imHighlight.gameObject.SetActive (false);

			// Player Choice Interface
			_btAdmit.gameObject.SetActive (false);
			_btAvert.gameObject.SetActive (false);
			_btAdmonish.gameObject.SetActive (false);

			break;

		}

	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */
	// Buttons

	void Admit() {

		_scPTG._lstDiagHist [_scPTG._lstDiagHist.Count - 2] = _scPTG._lstDiagHist [_scPTG._lstDiagHist.Count - 3];

		_scPTG._lstDiagHist [_scPTG._lstDiagHist.Count - 1] = "__";
		_scPTG._lstDiagHist [_scPTG._lstDiagHist.Count - 3] = "__";

		_scStory._itResponse = 1;
		_scStory._aitAdmitting [_scStory._itRetelling] += 1;

		_scStory._enLastChoice = Talk._admit;

		_imBackground.color = new Color (_imBackground.color.r, _imBackground.color.g, _imBackground.color.b + _flRGB);

	}

	void Avert() {

		_scPTG._lstDiagHist [_scPTG._lstDiagHist.Count - 1] = "__";
		_scPTG._lstDiagHist [_scPTG._lstDiagHist.Count - 3] = "__";

		_scStory._itResponse = 2;
		_scStory._aitAverting [_scStory._itRetelling] += 1;

		_scStory._enLastChoice = Talk._avert;

		_imBackground.color = new Color (_imBackground.color.r + _flRGB, _imBackground.color.g + _flRGB, _imBackground.color.b);

	}

	void Admonish() {

		_scPTG._lstDiagHist [_scPTG._lstDiagHist.Count - 2] = _scPTG._lstDiagHist [_scPTG._lstDiagHist.Count - 1];

		_scPTG._lstDiagHist [_scPTG._lstDiagHist.Count - 1] = "__";
		_scPTG._lstDiagHist [_scPTG._lstDiagHist.Count - 3] = "__";

		_scStory._itResponse = 3;
		_scStory._aitAdmonishing [_scStory._itRetelling] += 1;

		_scStory._enLastChoice = Talk._admonish;

		_imBackground.color = new Color (_imBackground.color.r + _flRGB, _imBackground.color.g, _imBackground.color.b);

	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */
	// Other Co-routines.

	IEnumerator FlashBackground() {

		_imBackground.color = Color.white;

		float flTimeElapsed = 0f;
		float flTime = _scPTG._flCuts * 2f;

		float flLerp = 0;
		float flLerpNum = 20f;

		_scRumble._flRumbling += 100f;

		do {

			_imBackground.color = Color.Lerp (_scPTG._clFlash, _clOriginal, (flTimeElapsed / flTime));
			flTimeElapsed += Time.deltaTime + flLerp;

			flLerp += Time.deltaTime / flLerpNum;
			flLerpNum++;

			yield return null;

		} while (flTimeElapsed <= flTime);

		_imBackground.color = _clOriginal;

		yield return null;

	}

}
