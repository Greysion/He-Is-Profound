using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Linq;

/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
   Author: 			Hayden Reeve
   File:			MenuWatcher.cs
   Description: 	This script contains the functions used by the main menu to control the camera, change scenes, and manipulate the options settings. 
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

public enum Turn {_input,_choice,_game};

[RequireComponent(typeof(_Story))]
public class PrintToGame : MonoBehaviour {

	// ---- Inspector Variables

	[Header("Gameplay Options")]

	[Tooltip("Whether or not to skip the Time-To-Wait before the game begins. Developer option, use for testing, turn off when building.")]
	[SerializeField] private bool _isSkipping;

	[Header("Animation Text Component")]

	[Tooltip("The most recent text element visable on the screen. This is the element to be animated.")]
	[SerializeField] private Text _txLineCurrent;

	[Tooltip("The list of text elements that display the game's dialogue backlog.")]
	[SerializeField] private List<Text> _ltxConsole;

	[Header("Music Components")]

	[Tooltip("The ambience music track gameobject that plays")]
	[SerializeField] private GameObject _gmMusic;

	[Tooltip("The credits overlay.")]
	[SerializeField] private GameObject _gmCredits;

	[Header("SFX Component")]

	[FMODUnity.EventRef]
	[SerializeField] private string _stKeystrokeAudio;

	[FMODUnity.EventRef]
	[SerializeField] private string _stSpacebarAudio;

	[FMODUnity.EventRef]
	[SerializeField] private string _stGunshotAudio;

//	[Header("Music Component")]
//
//	[FMODUnity.EventRef]
//	[SerializeField] private string _stAmbientMusic;
//
//	[FMODUnity.EventRef]
//	[SerializeField] private string _stCreditMusic;

	[Header("Timing Components")]

	// Controls the delays between certain events in Typespace.
	[SerializeField] private float _flChar; // Character
	[SerializeField] private float _flPunc; // Punctuation
	[SerializeField] private float _flLine; // Line
	[SerializeField] private float _flWait; // Wait Character
	[SerializeField] public float _flCuts;  // Cutscene

	[SerializeField] private float _flSkip; // Shift Skipping

	[Header("Text Colour Components")]

	// Controls the colours available to be printed in Typespace.
	[SerializeField] private Color _clHumanity;
	[SerializeField] private Color _clProfundus;
	[SerializeField] private Color _clAdmit;
	[SerializeField] private Color _clAvert;
	[SerializeField] private Color _clAdmonish;
	[SerializeField] private Color _clMorgan;

	[Header("Background Colours")]

	[SerializeField] public Color _clFlash;

	// ---- Hidden Variables

	[HideInInspector] public Turn _enTurn = Turn._game; // Whether it's the Game's turn, or the player's turn.

	[HideInInspector] public List<string> _lstDiagHist = new List<string> (); // The dialogue history of everything printed to the screen.

	private WaitForSeconds _wsChar; // Wait this many seconds between characters.
	private WaitForSeconds _wsPunc; // Wait this many seconds between punctuation.
	private WaitForSeconds _wsLine; // Wait this many seconds between Lines.
	private WaitForSeconds _wsWait; // Wait this many seconds for special event pauses, indicated by a $ in a string.
	private WaitForSeconds _wsCuts; // Wait this many seconds for cutscenes.

	public WaitForSeconds _wsUpdate = new WaitForSeconds(0.005f); // We don't need to check constantly. It's a text based game, we can save processing by running it at intervals.

	private int _itCurLine = -1; // Stores the line the program is currently printing in the backlog.
	private bool _isTyping = false; // Whether the system is currently typing. Used to control coroutines.
	private bool _wasTyping = false; // Whether the system was typing in the previous set of events, or whether it was the User's turn. Used to control sound.
	private bool _isInstant = false; // If we're skipping all delays and immediately printing text.

	private string[] _astPunctuation = new string[] { ".", "!", "?", ":", "," }; // Punctuation Characters that trigger additional delays within Typespace. Will be printed.
	private string[] _astSpecial = new string[] { "$", "[", "]", "^", "@", "*" }; // Special Characters that trigger events withing Typespace. Will not be printed.

	private bool _isPunctuating = false; // Is there an additional delay queued up in the system due to punctuation?

	[HideInInspector] public bool _isChoosing = false; // Whether the player is Choosing an answer, or Typing an Answer when it's their turn.

	[HideInInspector] public bool _isCutscene = false; // If the player should be watching a cutscene.
	[HideInInspector] public int _itCutscene = 0; // Which cutscene the player should be watching.

	[HideInInspector] public int _itTutorial = 0; // Whether the tutorial is currently in effect '1', or past '2'. The player needs to press SHIFT in order to continue playing.

	[HideInInspector] public bool _isCleaning = false; // Whether the original colour is being restored.
	[HideInInspector] public bool _isFlashing = false; // Whether the original colour is being flashed with white temporarily.

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// Use this for initialization.
	void Start () {

		// Assign WaitForSeconds.
		_wsChar = new WaitForSeconds ( _flChar );
		_wsPunc = new WaitForSeconds ( _flPunc );
		_wsLine = new WaitForSeconds ( _flLine );
		_wsWait = new WaitForSeconds ( _flWait );
		_wsCuts = new WaitForSeconds ( _flCuts );

		// Begin the coroutines and refresh functions.
		StartCoroutine (CheckForNewLines ());

	}

	void Update () {

		// Stop the tutorial bullshit after we're far enough into the game.
		if (_lstDiagHist.Count > 30) {
			_itTutorial = 0;
		}

		// If we're pressing Shift, we need to speed up the whole program.
		if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift) ) {
			
			_wsChar = new WaitForSeconds ( _flSkip );
			_wsPunc = new WaitForSeconds ( _flSkip );
			_wsLine = new WaitForSeconds ( _flSkip );
			_wsWait = new WaitForSeconds ( _flSkip );

			// If we're doing the tutorial, we need to replace the text display to visualise the player's ability to speed up the game.
			if (_lstDiagHist.Count > 8 && _itTutorial != 0) {

				if (_itTutorial == 1) {
					_lstDiagHist [9] = _lstDiagHist [9].Replace ("Hold to enable.","Enabled.");
					_itTutorial = 2;
				} else {
					_lstDiagHist [9] = _lstDiagHist [9].Replace ("Disabled.","Enabled.");
				}
				
				UpdateTextElements ();

			}

		}

		if (Input.GetKeyUp (KeyCode.LeftShift) || Input.GetKeyUp (KeyCode.RightShift)) {
			
			_wsChar = new WaitForSeconds (_flChar);
			_wsPunc = new WaitForSeconds (_flPunc);
			_wsLine = new WaitForSeconds (_flLine);
			_wsWait = new WaitForSeconds (_flWait);

			if (_lstDiagHist.Count > 8 && _itTutorial != 0) {
				_lstDiagHist [9] = _lstDiagHist [9].Replace ("Enabled","Disabled");
				UpdateTextElements ();
			}

		}

	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// This coroutine constantly checks the current backlog and requests each line backlogged to be printed out one at a time.
	IEnumerator CheckForNewLines() {

		// Opening Sequence Delay.
		_enTurn = Turn._game;
		if (!_isSkipping) { yield return new WaitForSeconds (4f); }

		while (true) {

			// If we aren't at the most recent addition to the Dialogue History, and aren't currently typing, update the text elements and start a new typing animation.
			if ( (_lstDiagHist.Count-1) > _itCurLine && !_isTyping) {

				// Since we're still typing, it must be the game's turn.
				_enTurn = Turn._game;

				_itCurLine++;
				UpdateTextElements ();
				StartCoroutine (TypeLine());

			} else if ( (_lstDiagHist.Count-1) <= _itCurLine && !_isTyping && _itTutorial != 1) {

				// If we're up to date, that must mean that it's the player's turn as the AI fires off all responces instantly.
				_wasTyping = false;

				// First, are we in a cutscene?
				if (_isCutscene) {
					_enTurn = Turn._game;

					// We only want to do something if the Cutscene Value is 1. If it's 2, it's handled elsewhere and we don't want to do anything here.
					if (_itCutscene == 1) {

						_gmMusic.SetActive (false);
						FMODUnity.RuntimeManager.PlayOneShot (_stGunshotAudio);
						yield return new WaitForSeconds (_flChar);

						ClearTextLog ();
						_isFlashing = true;

						yield return _wsCuts;

						_gmMusic.SetActive (true);
						_isCutscene = false;
						_itCutscene = 0;

					} else if (_itCutscene == 2) {

						_gmCredits.SetActive (true);
						yield return new WaitForSeconds (112f);
						Application.Quit ();

					}

				// If not, are we choosing between three options?
				} if (_isChoosing) {
					_enTurn = Turn._choice;

				// If not, then we must be waiting for a manual text entry.
				} else { 
					_enTurn = Turn._input; }

			}
			
			yield return _wsUpdate;
			
		}

	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// This coroutine prints out the current line in the dialogue backlog to the screen.
	IEnumerator TypeLine() {

		// Announce that we're now typing and clear the current line.
		_isTyping = true;
		_txLineCurrent.text = "";

		// If we just finished typing a line, play the line break sound.
		if (_wasTyping) { FMODUnity.RuntimeManager.PlayOneShot (_stSpacebarAudio); }

		// Briefly pause if we're not printing options.
		if (!_isInstant) { yield return _wsLine; }
		_isInstant = false;

		// Allocate the correct line to be printed.
		string stPrinting = _lstDiagHist [_itCurLine];

		// Check to make sure there is something to print in the first place.
		if (stPrinting != "__") {

			// Print out the current DialogueHistory string one character at a time by iterating through the string length.
			for (int it = 0; it < (stPrinting.Length + 1 - 2) && !_isInstant; it++) {

				// Seperate the Pre-Text and the Text components from the string.
				string stPre = stPrinting.Substring (0, 2);
				string stTex = stPrinting.Substring (2, it);
				string stCur;

				// Make sure we're not trying to SubString an invalid index.
				if (stTex != "") {
					stCur = stTex.Substring (stTex.Length - 1, 1);
				} else {
					stCur = "";
				}

				// If we're not looking at a special character...
				if (!_astSpecial.Contains(stCur)) {

					// Play the correct audio file, whether it's a keystroke or a spacebar.
					if (stTex != "") {
						if (stCur == " ") {
							FMODUnity.RuntimeManager.PlayOneShot (_stSpacebarAudio);
						} else {
							FMODUnity.RuntimeManager.PlayOneShot (_stKeystrokeAudio);
						}
					}

					// Remove any special characters that have already been processed previously.
					foreach (string st in _astSpecial) {
						stTex = stTex.Replace (st, "");
					}

					// Wait until the next character delay depending on the last character inputted.
					if (_isPunctuating) {
						_isPunctuating = false;
						yield return _wsPunc;
					} else {
						yield return _wsChar;
					}

					// If we have certain punctuation, we want to wait longer after the next character.
					if (_astPunctuation.Contains (stCur)) {
						_isPunctuating = true;
					}

					// Print the text and add a text charat.
					_txLineCurrent.text = ColourText (stPre, stTex) + "<b>|</b>";
					
				} else {

					// Work through the corresponding action
					switch (stCur) {

					// This is a special pause character. The script will take an extended delay before printing the next character.
					case ("$"):
						yield return _wsWait;
						break;

					case ("^"):
						_isInstant = true;
						break;

					case ("@"):
						_isCleaning = true;
						break;

					case ("*"):
						_isInstant = true;
						_isCutscene = true;
						_itCutscene = 1;
						break;
					
					default:
						break;

					}
					
				}

			}

		} else {
			
			_txLineCurrent.text = "<b>|</b>";

		}

		// Make sure we've finished printing the line.
		stPrinting = _lstDiagHist [_itCurLine];
		stPrinting = ColourText (stPrinting.Substring (0,2), stPrinting.Remove (0,2));

		foreach (string st in _astSpecial) {
			stPrinting = stPrinting.Replace (st, "");
		}

		_txLineCurrent.text = stPrinting + "<b>|</b>";

		// Pause on the line break.
		if (!_isInstant) { yield return _wsLine; }

		// Remove the typing caret.
		_txLineCurrent.text = _txLineCurrent.text.Substring (0, _txLineCurrent.text.Length - 8);
		_isPunctuating = false;
		_isTyping = false;
		_wasTyping = true;

	}
		
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// This function simply updates the text on the screen to reflect the current dialogue backlog.
	void UpdateTextElements() {

		// First we need to find the amount of lines currently /not/ queued for animation, and not actually shown on the screen.
		int itAnimDiff = ( (_itCurLine+1) - _lstDiagHist.Count) * -1;

		// Then, for each element, we need to determine where we currently are in the simulation and print out the remaining text elements in accordance.
		for (int it = 0; it < (_ltxConsole.Count); it++) {

			if (it + 1 < _lstDiagHist.Count-itAnimDiff) {

				string stPrinting = _lstDiagHist [_lstDiagHist.Count - (it + 2 + itAnimDiff)];

				// Colour the text, then modify the Alpha Hex Value depending on how far the list we are.
				stPrinting = ColourText (stPrinting.Substring (0,2), stPrinting.Remove (0,2));
				stPrinting = stPrinting.Replace ("FF>", ColorUtility.ToHtmlStringRGBA (_ltxConsole [it].color).Remove(0,6) + ">");

				// Remove special characters and apply modifiers to code.
				stPrinting = stPrinting.Replace ( "[", string.Format ("</color>{0}", stPrinting.Substring (0,8+6) + "FF>") );
				stPrinting = stPrinting.Replace ( "]", "</color>" + stPrinting.Substring (0,9+8) );

				foreach (string st in _astSpecial) {
					stPrinting = stPrinting.Replace (st, "");
				}

				_ltxConsole [it].text = stPrinting;

			} else {
				
				_ltxConsole [it].text = "";

			}
			
		}
		
	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// This function colours the text depending on the first two characters that are given.
	string ColourText(string stPre, string stTex) {

		string stColoured;

		// Check the string for any possible matches, and change the colour accordingly. Otherwise, the colour is set to the default.
		switch (stPre) {

		case ("__"): // Profundus
			stColoured = string.Format ("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA (_clProfundus), stTex);
			break;

		case ("T_"): // Humanity
			stColoured = string.Format ("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA (_clHumanity), stTex); //"<color=#b9b0feff>" + stTex + "</color>";
			break;

		case ("1_"): // Good
			stColoured = string.Format ("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA (_clAdmit), stTex); // "<color=#678befff>" + stTex + "</color>";
			break;

		case ("2_"): // Neutral
			stColoured = string.Format ("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA (_clAvert), stTex); // "<color=#e5f17bff>" + stTex + "</color>";
			break;

		case ("3_"): // Bad
			stColoured = string.Format ("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA (_clAdmonish), stTex); // "<color=#cd2b2bff>" + stTex + "</color>";
			break;

		case ("M_"): // Lie
			stColoured = string.Format ("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA (_clMorgan), stTex); // "[Lie]  <color=#e01919ff>" + stTex + "</color>";
			break;

		default:
			stColoured = string.Format ("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA (_clProfundus), stTex);
			Debug.LogError ("You should not be triggering a Default Switch in ColourText.");
			break;

		}

		return stColoured;

	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// This functiuon refreshes the current dialogue box and resets the chat history.
	public void ClearTextLog() {

		_lstDiagHist.Clear ();
		_itCurLine = -1;

		UpdateTextElements ();

	}

}
