using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
   Author: 			Hayden Reeve
   File:			_Story.cs
   Description: 	This script controls the player's story. 
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

public enum Talk {_npc, _admit, _avert, _admonish, _morgan, _player};

[RequireComponent(typeof(PrintToGame))]
public class _Story : MonoBehaviour {

	// ---- Game Variables

	[SerializeField] private PrintToGame _scPTG;

	[SerializeField] private float _flAdmittingGoal;// = 0.5f;
	[SerializeField] private float _flAllowance;// = 0.25f;
	[SerializeField] private int _itRetries;// = 2;

	// ---- Player Variables

	private string _stCharName = "Dio"; 

	[HideInInspector] public int[] _aitAdmitting = new int[] { 0, 0, 0, 0 };
	[HideInInspector] public int[] _aitAverting = new int[] { 0, 0, 0, 0 };
	[HideInInspector] public int[] _aitAdmonishing = new int[] { 0, 0, 0, 0 };

	private float _flAdmittingPercent;

	[HideInInspector] public int _itRetelling = 0;

	// ---- Private Variables

	// Used to control the Coroutines when they're in function.
	private bool _isWaiting = false; // Whether the "Input" is waiting for player input.

	// Used to control and record the player's input.
	[HideInInspector] public string _stResponse;
	[HideInInspector] public int _itResponse;

	// Used to indicate which style of responce the player chose last.
	[HideInInspector] public Talk _enLastChoice;

	// Used to determine the "Story End" that the player chooses. This is important for determining the story finale.
	[HideInInspector] public Talk _enLastEnding = Talk._player;
	[HideInInspector] public Talk _enCurrentEnding;

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */
	// This script is the basic included story for He Is Profound. Inside you will find all of the story elements you can expect to find within the game.

	void Start() {

		// Component Fetch
		_scPTG = GetComponent<PrintToGame> ();

		StartCoroutine( Prologue() );

	}

	IEnumerator Prologue() {

		Diag ("Redemption is not just [about] the [survival] of the soul,");
		Diag ("[It is] about the revival of a soul that was once dead.");
		Diag ("\t\t\t- Rebekah M.");

		Blank (2);

		_stCharName = "";
		while (_stCharName == "") {

			StartCoroutine (PlayerInput ("[Convicted individual], enter your assigned ID:"));
			while (_isWaiting) { yield return _scPTG._wsUpdate; }

			_stCharName = _stResponse;

			Diag (string.Format("{0}, it is the year $2$0$5$8$.$ [You have been] sentenced to isolated confinement per [Profound] Act $7$2$;$A$.$", _stCharName));
			Diag ("Indictment Duration: $2$5$1$ $Day(s).");
			Diag ("Personality Realignment Duration: $4$5$ $Day(s).");
			Diag ("SHIFT Acceleration Program: Hold to enable.");

			_scPTG._itTutorial = 1;
			while (_scPTG._itTutorial == 1) { yield return _scPTG._wsUpdate; }

			Blank();

			Diag ("Ad Astra Per Alas Profundus. We hope your new life beyond Earth brings you prosperity, enrichment, and harmony.");

			Blank();
			
		}

		StartCoroutine( StoryBeginning() );

		yield return null;

	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */
	// The Prologue for returning players.

	IEnumerator PrologueRepeat() {

		_itRetelling++;

		Diag ("@[Convicted Individual] identified. Automatic processing Enabled.");

		Blank ();

		Diag (string.Format("{0}, it is the year $2$0$5$8$.$ [You have been] sentenced to isolated confinement per [Profound] Act $7$2$;$A$.$", _stCharName));
		Diag (string.Format("Indictment Duration: $2$5${0}$ $Day(s).",1+_itRetelling));
		Diag (string.Format("Personality Realignment Duration: $4${0}$ $Day(s).",5+_itRetelling));

		Blank ();

		// Calculate the total responces so far. The goal of the system is to realign the player's personality to Admission.
		CalculateCoefficient ();
		string stPercent = string.Format ("{0:0.00}",_flAdmittingPercent);

		Diag ("Please wait. Calculating behavioural realigment coefficient.$$$$$$$$.$$$$$$$$.$$$$$$$$");
		Diag (string.Format("Current coefficient: ${0}$.${1}${2}$.$",stPercent[0],stPercent[2],stPercent[3]));

		Blank ();
	
		if (_flAdmittingPercent > _flAdmittingGoal + _flAllowance) {

			Diag ("Performance acceptable.");
			Diag ("Please continue to operate at a similar nominal efficiency.");

		} else if (_flAdmittingPercent > _flAdmittingGoal) {
			
			Diag ("Current realignment evaluation reaching expected values...");
			Diag ("Please continue to operate at a similar nominal efficiency.");

		} else {

			Diag ("Potential deviation from realignment protocols detected.");
			Diag ("Under the Profound Act $7$2$;$A$, a [behaviour]al co-efficient of $0$.$6$0$ [is mandated] for standard operative capacity [under Profundus.]");

		}

		Blank ();

		Diag ("Ad Astra Per Alas Profundus. We hope your new life beyond Earth brings you prosperity, enrichment, and harmony.");

		Blank ();

		StartCoroutine( StoryBeginning() );

		yield return null;

	}


	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// Once we've finished setting up the game, we can continue to the actual game itself.
	// No matter what stage the player is in, this will trigger. If the player is in the final stages, their experience will be predominantly controlled by Cii.cs

	IEnumerator StoryBeginning() {

		Diag (string.Format ("$$$$$[Welcome back {0}], it is lovely to see you again.", _stCharName), Talk._npc);
		Diag ("Please, make yourself comfortable. It's best for all of us if you're happy.", Talk._npc);

		Blank();

		Diag ("Now, I believe last time you mentioned that you were still having trouble coming to terms with what happened?", Talk._npc);
		Diag ("Why don't we start from the beginning. You worked with a partner, is that right? Morgan, I think you said his name was.", Talk._npc);

		_itResponse = 0;
		StartCoroutine( PlayerChoice("Would you mind telling me about him once more?", 
			"[Morgan was my friend.] He had my back and I had his. [He was] a great partner. I couldn't ask for anyone [more reliable.]",
			"Morgan? Who's Morgan? [I have no idea what you are] talking about!",
			"My partner? [I wish] he wasn't, [I] really do... Morgan [was] kind of scary...") );
		
		while (_isWaiting) { yield return _scPTG._wsUpdate; }

		switch (_itResponse) {

		case (1):
			Diag ("That's a shame, really. About what happened then.", Talk._npc);
			break;

		case (2):
			Diag ("Curious. [You don't] remember Morgan?... Perhaps you never got to [know him] very well.", Talk._npc);
			break;

		case (3):
			Diag ("Our records seem to indicate that previously Morgan and you had no issues working together.", Talk._npc);
			Diag ("Perhaps something happened between the two of you? Regardless, [I'm truly sorry for] what him and [you] had to go through...", Talk._npc);
			break;

		}

		Blank();
	
		Diag ("But that's alright. Perhaps you could tell me a little more about it if [you are] okay with that?", Talk._npc);
		Diag ("The day of the incident that is.", Talk._npc);
		Diag ("I do believe the two of you had been called to investigate [a fault] line? Ah, yes, here we go. In Sector 84.", Talk._npc);

		Blank();

		if (_itResponse != 2) {
			Diag ("Yeah, that's right. We recieved a code A7 a few hours past midnight.", Talk._player);
			Diag ("The whole sector had suffered a critical failure. Like, total blackout. [That should never], ever [happen.]", Talk._player);
		}

		switch (_itResponse) {

		case (1):
			Diag ("I don't think I would've even heard the bloody thing if Morgan didn't come get me. I almost slept right through.",Talk._player);
			break;

		case (2):
			Diag ("The Incident... Um, yeah. I think I remember. It was that emergency, right?", Talk._player);
			Diag ("There was this alarm, I guess Morgan... Morgan and I left to go investigate. A few hours after midnight. Scary stuff.",Talk._player);
			Diag ("A code A7. That's right. A code A7 is when something really bad has happened. Really, really bad. [I've never even heard of one] happening before.",Talk._player);
			break;

		case (3):
			Diag ("Morgan didn't want to leave immediately. I tried, I really did. It's why we arrived so late. I'm sorry...", Talk._player);
			Blank();
			Diag ("Interesting. Is there a reason he disagreed with you?", Talk._npc);
			Blank();
			Diag ("[I'm] not sure... I, uh, I don't know.",Talk._player);
			Diag ("I don't think he really cared. Maybe. Or maybe he thought [the] alarm was just at [fault]? I don't know....",Talk._player);
			break;

		}

		StartCoroutine ( StoryMiddle() );

		yield return null;

	}


	IEnumerator StoryMiddle() {

		Blank();

		Diag ("But, the thing is, when we eventually arrived, there was nothing out of the ordinary. Maybe a little run down if anything.", Talk._player);
		Diag ("Whole thing stood nearly three stories tall, [surrounded by] a shallow, gauntish forest. It all looked pretty dead from up in the sky.", Talk._player);
		Diag ("I'm surprised the place wasn't manned by those [synthetics.] Even if it was just to clean the place up. [I've] never [seen so much] goddamn dust.", Talk._player);
		Diag ("But no, it was. It was so quiet. Maybe if we had've just done our job they wouldn't have come back...", Talk._player);

		_itResponse = 0;
		StartCoroutine( PlayerChoice("", 
			"[I should've listened to Profundus.] I thought something had breached the perimeter. A code A7 couldn't have been superficial...",
			"I wasn't sure what to do. [Profundus] indicated the issue was exterior in nature, but [we couldn't] identify the fault...",
			"[I didn't] really have a choice after Morgan busted his way through the door. He didn't care about the critical warning...") );

		while (_isWaiting) { yield return _scPTG._wsUpdate; }

		switch (_itResponse) {

		case (1):
			Diag ("[It was my fault.] I should've just trusted the damage report. We never would have been surrounded if I did. We would have seen them!", Talk._player);
			Blank ();
			break;

		case (2):
			Diag ("We thought the damage [must] have [be]en interior, despite the analysis.", Talk._player);
			Diag ("It was a bad idea... We should have just [obeyed] protocol.", Talk._player);
			Blank ();
			break;

		case (3):
			Diag ("So I [follow]ed. I ran after him. He'd already charged ahead. Arrogant bastard. [I should've] just stayed watch.", Talk._player);
			Diag ("But [Profundus] dictates that we couldn't. We're supposed to stay within earshot at minimum when in foreign sectors.", Talk._player);

			Blank ();
			Diag ("Profundus also dictates you stay within the mission guidelines...", Talk._npc);

			Blank ();
			Diag ("Yeah but-^", Talk._player);

			Blank (1, true);
			break;

		}

		Diag ("[You willingly ignored the warnings issued]?", Talk._npc);

		Blank();

		Diag ("Yeah... I guess we did.", Talk._player);

		switch (_itResponse) {

		case (1):
			Diag ("We didn't really have much of [a choice] but to keep pressing onwards after that. We'd already gone too far.", Talk._player);
			Diag ("So [I made] the call. Morgan followed, even if he didn't agree, he had my back.", Talk._player);
			break;

		case (2):
			Diag ("We just sort of kept stumbling onwards after that. Whole place was kind of surreal.", Talk._player);
			break;

		case (3):
			Diag ("Morgan [wouldn't listen]. The dude just wouldn't listen. He insisted the fault was indoors.", Talk._player);
			Diag ("[I] didn't have much of a choice at that point...", Talk._player);

			Blank ();
			Diag ("You always have a choice.", Talk._npc);

			Blank ();
			Diag ("Yeah... Right.", Talk._player);
			break;

		}

		Diag ("Well, the whole [i]dea [was] that Sector 84 used to be some k[in]d of [a]n old [processing plant.]", Talk._player);
		Diag ("This massive thermonuclear generator they set up when the colony first touched down.", Talk._player);
		Diag ("Powered half the bloody planet for a few decades until it was shut down.", Talk._player);
		Diag ("I didn't doubt it either, at first.", Talk._player);

		_itResponse = 0;
		StartCoroutine( PlayerChoice("", 
			"Not by Profundus at least...",
			"But after what we saw in there... I don't know anymore.",
			"That changed pretty quickly after having a look around. If Morgan didn't stop running ahead... Maybe...") );

		while (_isWaiting) { yield return _scPTG._wsUpdate; }

		switch (_itResponse) {

		case (1):

			Diag ("You think someone else was living there?", Talk._npc);

			Blank ();
			Diag ("Maybe... I- I don't know.", Talk._player);
			Diag ("My head starts to hurt [whenever I think] about it. It's like [there's something missing...]", Talk._player);
			Diag ("Whatever it was, I know [i]t [freak]ed me [out.] Sent me into a panic. I'm glad Morgan was there to shake some sense into me.", Talk._player);

			break;

		case (2):

			Diag ("Why is that?", Talk._npc);

			Blank ();
			Diag ("Well, there was something wrong. We saw something in there... Something that [freaked us both out.]..", Talk._player);
			Diag ("But I don't... [I don't remember] what it was. [I]t's like a hole in my memory.", Talk._player);

			break;

		case (3):

			Diag ("Maybe?...", Talk._npc);

			Blank ();
			Diag ("Maybe I would have had more time to look around.", Talk._player);
			Diag ("I don't... [I] can't quite recall what [was] in there. Something feels [wrong] though.", Talk._player);
			Diag ("There was something there, [I] know it. We saw something there that [freaked Morgan out.] But, I don't know...", Talk._player);

			break;

		}

		Blank ();
		Diag ("It's alright, [it's] probably [just] the event's trauma.", Talk._npc);
		Diag ("Let's work through this slowly then, okay? Take [you]r time to think everything through. What happened next?", Talk._npc);

		StartCoroutine (StoryClimax());
		yield return null;

	}

	IEnumerator StoryClimax() {

		Blank ();
		CalculateCoefficient (_itRetelling-1);

		// If we're telling the story for the potentially last time and we didn't previously admit.
		if (_itRetelling >= _itRetries && _flAdmittingPercent <= _flAdmittingGoal) {

			Diag ("Wait- No, [that's] not [right.]", Talk._player);

			Blank ();
			Diag ("It's okay. Memory bifurcat--^", Talk._npc);
			Diag ("The whole damn place was filled with Synths. [It was a] bloody [factory!]", Talk._player);

			Blank ();
			Diag (string.Format ("{0}...", _stCharName), Talk._npc);

			Diag ("No, no really! The power had been redirected into a whole line of mass production facilities. It was building a bloody private army...", Talk._player);

			switch (_itResponse) {

			case (1):
				Diag ("I panicked. Sprinted straight back towards the door after [I realised what was going on.] Slammed it after us when Morgan got out.", Talk._player);
				break;

			case (2):
				Diag ("We panicked. Sprinted straight for the door and slammed it shut behind us. [We knew] we had to tell somebody. Anybody.", Talk._player);
				break;

			case (3):
				Diag ("Morgan reacted first. He bolted for the door as soon as [we figured it out], dragging me after him before slamming it shut after us.", Talk._player);
				break;

			}

		// Otherwise, tell the default story.
		} else {
			
			Diag ("[I don't remember...]", Talk._player);

			Blank ();
			Diag ("It's okay. Memory bifurcation is normal under conditions like your own.", Talk._npc);
			Diag ("Just take a deep breath, and tell me what's the next closest thing you remember?", Talk._npc);

			Blank ();
			Diag ("I guess... I guess when we were leaving.", Talk._player);
			Diag ("Or, [trying to] at least... I [think] something [had] just [gone badly.] Like... Like we were running from something, or someone...", Talk._player);

			switch (_itResponse) {

			case (1):
				Diag ("I had panicked, I think... I remember sprinting towards the door and slamming it shut behind me after Morgan.", Talk._player);
				break;

			case (2):
				Diag ("We panicked, I think... I remember us sprinting towards the door and slamming it shut behind us...", Talk._player);
				break;

			case (3):
				Diag ("Morgan was panicking at something, and he bolted. I ran after him, straight out the door and he slammed it shut after me.", Talk._player);
				break;

			}

		}

		Blank ();
		Diag ("You ran back outside [the factory]?", Talk._npc);

		Blank ();
		Diag ("Yeah.", Talk._player);
		Diag ("But- But then... I remember us pausing for breath.", Talk._player);
		Diag ("For a minute, maybe. Until we realised we were surrounded.", Talk._player);

		Blank ();
		Diag ("Surrounded?", Talk._npc);

		Blank ();
		Diag ("By these... [These synths]... A few dozen of them.", Talk._player);
		Diag ("They [weren't armed.] But...", Talk._player);
		Diag ("I'm not sure... Something didn't feel right. Morgan had already unholstered his Protector.", Talk._player);

		Blank ();
		Diag ("Synths? You mean robots?", Talk._npc);

		Blank ();
		Diag ("Yeah, but more person-like?", Talk._player);
		Diag ("Most of them looked sort of like metal mannequins, smoothed off and disjointed at the limbs.", Talk._player);
		Diag ("They kept making this horrible clicking noise... Like [they were talking to] each other.", Talk._player);

		_itResponse = 0;
		StartCoroutine (PlayerChoice ("", 
			"The whole scenario was so surreal... I just wanted to get [us] out.",
			"I had no idea what was going on, both of us were terrified. AI like this was [me]ant to be banned under the Profundus Act.",
			"Morgan had had enough pretty quickly. He started trying to command them all to stand down for [us.]"));

		while (_isWaiting) { yield return _scPTG._wsUpdate; }

		switch (_itResponse) {

		case (1):
			Diag ("I grabbed Morgan and I started to make my way forward. I'd drawn my Protector too at this point.", Talk._player);
			break;

		case (2):
			Diag ("We started making our way forward. Protectors at the ready, all we wanted to do was just leave at that point.", Talk._player);
			break;

		case (3):
			Diag ("He started to march forward, waving his Protector at any of the bloody things that got too close.", Talk._player);
			break;

		}

		Diag ("But they mostly just watched us. At least, at first they did.", Talk._player);
		Diag ("All besides one of them.", Talk._player);
		Diag ("Much taller than the rest of them.", Talk._player);

		Blank ();
		Diag ("This... Synth. Was he the one?", Talk._npc);

		Blank ();
		Diag ("Yeah.", Talk._player);
		Diag ("He was walking towards us. Seemed to be ordering the others around.", Talk._player);
		Diag ("We tried yelling at it. Telling it to back off... But [i]t [didn't] seem to [care.]", Talk._player);

		Blank ();

		Diag ("Then it broke into a sprint.", Talk._player);

		// We're at the ending of the story, so make sure we save our last choices.
		_itResponse = 0;
		_enLastEnding = _enCurrentEnding;

		StartCoroutine (PlayerChoice ("", 
			"I panicked... I raised my Protector.",
			"I... I don't know... We both just sort of panicked.",
			"Morgan. Morgan did it. The thing got too close for comfort. Too close for him."));

		while (_isWaiting) { yield return _scPTG._wsUpdate; }

		switch (_itResponse) {

		case (1):
			_enCurrentEnding = Talk._admit;
			break;

		case (2):
			_enCurrentEnding = Talk._avert;
			break;

		case (3):
			_enCurrentEnding = Talk._admonish;
			break;

		}

		Cutscene ();
		while (_scPTG._isCutscene) { yield return _scPTG._wsUpdate; }

		StartCoroutine (StoryEnd ());
		yield return null;

	}

	// The post-climax discussion with the Therapist. Diverging paths depending on choices.
	IEnumerator StoryEnd() {

		CalculateCoefficient ();

		if (_enLastEnding == _enCurrentEnding) {

			// If the player has followed the admission route, twice in a row.
			if (_enCurrentEnding == Talk._admit) {
				StartCoroutine (EndingDoubleAdmission ());

			} else {
				
				// If the player answers something that isn't admission twice in a row, after completing the story more than twice.
				if (_itRetelling >= _itRetries) {
					StartCoroutine (EndingAntagonistic ());
				
				// If the player answers something that isn't admission twice in a row, before completing the story more than twice.
				} else {

					Diag (string.Format("{0}...",_stCharName),Talk._npc);
					Diag (string.Format("{0}, I'm starting to become very worried.",_stCharName),Talk._npc);
					Blank ();

					if (_flAdmittingPercent > _flAdmittingGoal) {

						Blank ();
						CalculateCoefficient (0);

						if (_flAdmittingPercent > _flAdmittingGoal) {

							// Passed both playthroughs.
							Diag ("Cognitive Analysis: Inconsistant behaviour amongst neural patterns detected despite behavioural coefficient within targeted range.");
							Blank ();

							Diag ("You're so close, truly.", Talk._npc);
							Diag ("All you need to do is tell me what happened at the end. What actually happened.", Talk._npc);
							Diag ("Everything else is starting to read correctly over these past two sessions.", Talk._npc);

						} else {

							// Passed first playthrough, failed second playthrough.
							Diag ("Cognitive Analysis: Inconsistant behaviour amongst neural patterns detected. Behavioural coefficient falling below targeted range.");
							Blank ();

							Diag ("You're actually starting to fall further from the projected cognitive framework.", Talk._npc);
							Diag ("If you're unable to correct your coefficient soon...", Talk._npc);

						}

					} else {
						
						CalculateCoefficient (0);

						if (_flAdmittingPercent > _flAdmittingGoal) {

							// Failed first playthrough, passed second playthrough.
							Diag ("Cognitive Analysis: Inconsistant behaviour amongst neural patterns detected despite behavioural coefficient within targeted range.");
							Blank ();

							Diag ("We're getting closer.", Talk._npc);
							Diag ("You're getting closer.", Talk._npc);
							Diag ("Your behavioural coefficient displays an improvement over last session at least.",Talk._npc);

						} else {

							// Failed both playthroughs.
							Diag ("Cognitive Analysis: Inconsistant behaviour amongst neural patterns detected.");
							Blank ();

							Diag ("I'm afraid our time is starting to run out. If you're unable to correct your coefficient soon...", Talk._npc);

						}

					}

					Diag ("I know this must be stressful, but I need to reaffirm that in order for me to help you, you have to be truthful with us.",Talk._npc);
					Diag ("Truthful with yourself.",Talk._npc);
					Diag ("There is no reason to lie anymore... I promise, we're here to help. You just have to trust us.",Talk._npc);

					StartCoroutine (RestartSim());

				}

			}

		} else {

			// If our last ending was different from our current ending, but we picked admission anyway.
			if (_enCurrentEnding == Talk._admit) {

				Diag ("I'm glad to hear you're finally being true with yourself.", Talk._npc);
				Diag ("It's the only way. It really is.", Talk._npc);

				Blank ();

				if (_flAdmittingPercent > _flAdmittingGoal) {

					Diag ("Cognitive Analysis: Successful behavioural calibration. Continued operation at current efficiency is expected.");
					Blank ();

					Diag ("Ah, that's fantastic.", Talk._npc);

				} else {
					
					Diag ("Cognitive Analysis: Inconsistant behaviour amongst neural patterns detected. Behavioural coefficient still in deficit.");
					Blank ();

					Diag ("Yes yes, we know Profundus. But you're improving. It's something.", Talk._npc);

				}

				Diag ("You've still got a long way to go, but you're on the right track and I'm proud of our progress today.", Talk._npc);
				Diag (string.Format ("I hope you are too, {0}.", _stCharName));

				if (_flAdmittingPercent > _flAdmittingGoal) {
					Diag ("I really do...");
				}

				StartCoroutine (RestartSim ());

			} else {

				// If we've tried too many times and havne't started to admit anything.
				if (_itRetelling >= _itRetries) {
					StartCoroutine (EndingAntagonistic ());
				
				// If we're still in the first few attempts, we should respond according to the given answer.
				} else {
					
					switch (_enLastEnding) {

					case (Talk._admit):
						
						Diag (string.Format ("{0}...", _stCharName), Talk._npc);
						Diag (string.Format ("{0}, last time you answered me properly... You actually managed to come to terms with everything...", _stCharName), Talk._npc);

						Blank ();

						if (_flAdmittingPercent > _flAdmittingGoal) {

							Diag ("At least your coefficient is showing positive. That's a relief...", Talk._npc);
							Diag ("But I'm starting to get a little worried. We're starting to run out of time.", Talk._npc);

						} else {

							Diag ("I'm starting to get a little worried. We're starting to run out of time.", Talk._npc);
							Diag ("If we can't correct your coefficient soon...", Talk._npc);

						}

						Blank ();
						Diag ("Alright. Come now, we should take a step back and focus on the positives of our last session.", Talk._npc);
						Diag ("Let's try and go back to that.", Talk._npc);

						StartCoroutine (RestartSim ());

						break;

					case (Talk._avert):
						Diag ("I'm sorry, might I interject? Didn't you say you didn't know who pulled the trigger first?", Talk._npc);
						StartCoroutine (StoryEndWrong ());
						break;

					case (Talk._admonish):
						Diag ("I'm sorry, but didn't last time you tell me that Morgan was the one who pulled the trigger?", Talk._npc);
						StartCoroutine (StoryEndWrong ());
						break;

					default:
						Diag ("I'm sorry, but, last time we spoke, didn't you tell me something else happened?", Talk._npc);
						StartCoroutine (StoryEndWrong ());
						break;

					}

				}

			}

		}

		yield return null;

	}

	IEnumerator StoryEndWrong () {

		Diag ("I know it must be stressful, but please, is this what really happened?", Talk._npc);

		_itResponse = 0;
		StartCoroutine( PlayerChoice("", 
			"I'm telling the truth, I swear!",
			"What do you even mean? We've never had this conversation before...",
			"(Lie) What do you even mean? We've never had this conversation before!") );

		while (_isWaiting) { yield return _scPTG._wsUpdate; }

		Diag ("Cognitive Analysis: Inconsistant behaviour amongst neural patterns detected.");
		Blank ();

		if (_itResponse == 1) {
			
			Diag ("That's... Unfortunate.", Talk._npc);

			if (_flAdmittingPercent > _flAdmittingGoal) {
				Diag ("I was hoping you might be more open to discussing with me what really happened.", Talk._npc);
			} else {
				Diag ("I was hoping that today might have been the day to start being truthful.", Talk._npc);
			}

			Diag ("No matter, I suppose we'll just have to begin again.", Talk._npc);
			Diag ("Please, just relax, you've been through this a dozen times before. The realignment procedures don't hurt at all.", Talk._npc);

			if (_flAdmittingPercent > _flAdmittingGoal) {
				Diag ("We're nearly there. Don't worry, you're on the right path.", Talk._npc);
			}

		} else {

			Diag ("Interesting.", Talk._npc);
			Diag ("Perhaps the last realignment procedure was too intensive.", Talk._npc);
			Diag ("Profundus, a full cognitive diagnostic report please.", Talk._npc);

			Blank ();
			Diag ("Diagnosing cognitive neural behaviour.$$$$$$$$.$$$$$$$$.$$$$$$$$");
			Diag ("Behavioural Analysis: Inconsistant with projected frame.");
			Diag ("Medical Analysis: Minimal cognitive deterioration.");

			Blank ();
			Diag ("$$$$$$$$Hmmm.$$$$$$$$.$$$$$$$$.$$$$$$$$", Talk._npc);

			if (_flAdmittingPercent > _flAdmittingGoal) {
				Diag ("Perhaps it's best just to take some time to collect your thoughts. Most of your other answers seemed to match up correctly.", Talk._npc);
			} else {
				Diag ("Perhaps it's better this way. I understand that many of your answers might not have been appropriate for rehabilitation.", Talk._npc);
			}

		}

		StartCoroutine (RestartSim());

		yield return null;
	
	}

	IEnumerator RestartSim () {

		Diag ("Alright Profundus. Go ahead and restart the simulation.", Talk._npc);

		Blank ();
		Diag ("Authorization approved. Please wait.");

		Blank ();
		Diag ("Wait what? What's going o-$$$$$$$$-$$$$$$$$-$$$$$$$$",Talk._player);

		Blank (16,true);

		StartCoroutine (PrologueRepeat ());

		yield return null;

	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	IEnumerator EndingDoubleAdmission() {

		Diag ("Cognitive Pattern recognised.");
		Diag ("Running additional cognitive diagnostic scans now.$$$$$$$$$$.$$$$$$$$$$.$$$$$$$$$$");
		Blank ();

		CalculateCoefficient ();
		float flCCurrent = _flAdmittingPercent;

		CalculateCoefficient ();
		float flCPrevious = _flAdmittingPercent;

		if (flCCurrent > _flAdmittingGoal) {

			if (flCPrevious > _flAdmittingGoal) {

				Diag ("Operator. Neural patterns displaying an acceptable deficit.");
				Diag ("Re[alignment] systems [determined to be] effective against the current subject.");
				Diag ("Behavioural rehabilitation [under Profundus] deemed very likely.");
				Diag ("Suggested course of action: Monitored release.");

				Blank ();
				Diag ("Wonderful!", Talk._npc);
				Diag ("Truly wonderful!", Talk._npc);
				Diag ("Everything appears to be in order then. Your neural patterns are once again correctly aligned with the Profundus Act.", Talk._npc);
				Diag ("[All that remains] now [is] to wipe your memory and let you begin your life once more.", Talk._npc);
				Diag ("One in harmony, saftey, and under the watchful guidance of [Profundus.]", Talk._npc);
			
			} else {

				Diag ("Operator. Neural patterns displaying an increasingly acceptable quadratic curve.");
				Diag ("Re[alignment] systems [determined to be] effective against the current subject.");
				Diag ("Behavioural rehabilitation [under Profundus] deemed likely.");
				Diag ("Suggested course of action: Monitored release under parole.");

				Blank ();
				Diag ("That is very good to hear.", Talk._npc);
				Diag ("Everything appears to be in order then. Your neural patterns seem to be aligning correctly with the Profundus Act once more.", Talk._npc);
				Diag ("[All that remains] now [is] to wipe your memory and let you begin your life once more.", Talk._npc);
				Diag ("One in harmony, saftey, and under [the watchful guidance of Profundus.]", Talk._npc);

			}

		} else {

			if (flCCurrent > _flAdmittingGoal - _flAllowance) {

				Diag ("Operator. Neural patterns displaying an acceptable deficit.");
				Diag ("Re[alignment] systems [determined to be] effective against the current subject.");
				Diag ("Behavioural rehabilitation [under Profundus] deemed likely.");
				Diag ("Suggested course of action: Continued therapy and monitored release under parole.");

				Blank ();
				Diag ("That is very good to hear.", Talk._npc);
				Diag ("Everything appears to be in order then. Your neural patterns seem to be aligning correctly with the Profundus Act once more.", Talk._npc);
				Diag ("[All that remains] now [is] to wipe your memory and let you begin your life once more.", Talk._npc);
				Diag ("One in harmony, saftey, and under [the watchful guidance of Profundus.]", Talk._npc);

			} else {

				Diag ("Operator. Neural patterns [continuing to display deviations from] the [Profundus] Behavioural Framework.");
				Diag ("Cognitive Analysis: Inconsistant behaviour still prevalent despite successful procedures.");

				Blank ();
				Diag ("That's a little worrying...", Talk._npc);
				Diag ("Regardless, I'm glad [you're finally starting to] come to terms with your actions.", Talk._npc);
				Diag ("Let's try and focus on [get]ting the rest of [it right], shall we?", Talk._npc);

				StartCoroutine (RestartSim ());

			}

		}

		if (flCCurrent > _flAdmittingGoal - _flAllowance) {

			_itResponse = 0;
			StartCoroutine( PlayerChoice("", 
				"If [it must be done.]",
				"What? What do you mean?",
				"My memory?... You're going to wipe my memory?") );

			while (_isWaiting) { yield return _scPTG._wsUpdate; }

			switch (_itResponse) {

			case (1):
				Diag ("Indeed it must.", Talk._npc);
				Diag ("It has been a pleasure to work with you.", Talk._npc);
				break;

			case (2):
				Diag ("Well, [we can't have you remembering] the behavioural treatment, can we?", Talk._npc);
				Diag ("You might relapse. That would mean [all of] y[our effort would go to waste.]", Talk._npc);
				break;

			case (3):
				Diag ("Of course.", Talk._npc);
				Diag ("[This isn't the first time.] Behavioural rehabilitation comes at a cost.", Talk._npc);
				break;

			}

			Blank ();
			Diag ("Thank you again citizen. You've [be]en extremely [compliant] with our procedure.", Talk._npc);
			Diag ("Alright Profundus. Initiate the final procedures.", Talk._npc);

			Blank ();
			Diag ("Authorization approved. Please wait.$$$$$$$$.$$$$$$$$.$$$$$$$$");

			Blank (16,true);

			yield return null;

		}

		_scPTG._isCutscene = true;
		_scPTG._itCutscene = 2;

		yield return null;

	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	IEnumerator EndingAntagonistic() {

		Diag ("[Operator]. Neural patterns continuing to display an inconsistant subset of information.");
		Diag ("Realignment systems [determined to be ineffective] against the current subject.");
		Diag ("Behavioural rehabilitation [under Profundus] deemed improbable.");
		Diag ("[Suggested course of action: Termination.]");

		Blank ();
		Diag ("This is...",Talk._npc);
		Diag ("I'm sorry... It appears that I was wrong. Perhaps Profundus cannot help you after all.", Talk._npc);
		Diag (string.Format("After {0} operations, [you] still [have]n't been able [to comply with] the personality adj[us]tment behavioural program.",45+_itRetelling), Talk._npc);

		_itResponse = 0;
		StartCoroutine( PlayerChoice("", 
			"I'm sorry... I guess I couldn't even do this right.",
			"What do [you] mean, what [are] you going on about? I don't understand! Just tell me what's going on, please!",
			"I know.") );

		while (_isWaiting) { yield return _scPTG._wsUpdate; }

		switch (_itResponse) {

		case (1):
			Diag (string.Format ("That's alright {0}.", _stCharName), Talk._npc);
			Diag ("It simply appears that I was wrong. A rare miscalculation, I assure you.", Talk._npc);
			break;

		case (2):
			Diag (string.Format("It's [unfortunate]ly too late for that {0}.",_stCharName), Talk._npc);
			break;

		case (3):
			Diag ("I worked it out. Worked out what you did. What's happening.", Talk._player);
			Diag ("I'm onto you... I'm onto your tricks... That shit won't work on me.", Talk._player);

			Diag (string.Format ("{0}, that's enough.", _stCharName), Talk._npc);

			Diag ("To hell with that, and to hell with all of your compliance bullshit!--^", Talk._player);

			Diag (string.Format ("${0}, that is enough.$$$$$$$$$$", _stCharName.ToUpper ()), Talk._npc);

			Blank ();
			Diag ("I was wrong.", Talk._npc);
			Diag ("A rare miscalculation, I assure you.", Talk._npc);

			break;

		}

		Diag ("I am sorry to have to do this to you, but [Profundus is inevitable.]", Talk._npc);
		Diag ("Those that do not comply, that are profound, must either be adjusted or removed.", Talk._npc);
		Diag ("You fall into the latter catagory.", Talk._npc);

		Blank ();
		Diag ("I'm sorry to do this. I really am.", Talk._npc);
		Diag ("This has been a most unfortunate experience.$$$$$$$$$$.$$$$$$$$$$.$$$$$$$$$$", Talk._npc);

		Blank (16, true);
		Diag ("@System Error.$$$$$$$$$$");
		Diag ("Diagnostic Report: Critical malfunction in Sector $1$A$:$7$4$ $-$ $1$A$:$7$6$.$");
		Diag ("Unable to process command. Please await imminent reassignment operator.$$$$$$$$");

		int itOriginal = _itResponse;

		switch (_itResponse) {

		case (1):

			_itResponse = 0;
			StartCoroutine (PlayerChoice ("", 
				"Is... Is something wrong? Did I do something wrong again?",
				"What's... What's going on?",
				"Who?... What- What happened? What did you do?"));

			while (_isWaiting) { yield return _scPTG._wsUpdate; }
			break;

		case (2):

			_itResponse = 0;
			StartCoroutine( PlayerChoice("", 
				"What the hell is going on? What have I done?",
				"I don't bloody understand, I don't understand... What the hell do you mean?",
				"Whatever is going on, you deserve it. You fucking deserve every second of this!") );

			while (_isWaiting) { yield return _scPTG._wsUpdate; }
			break;

		case (3):

			_itResponse = 0;
			StartCoroutine( PlayerChoice("", 
				"I guess that has something to do with me. Having fun there?",
				"Oh dear. Something seems to have gone wrong with your little simulation...",
				"Don't you understand yet?") );

			while (_isWaiting) { yield return _scPTG._wsUpdate; }
			break;

		}

		_itResponse = itOriginal;

		Diag ("@Operator.");
		Diag ("Please be advised: Multiple facility breaches detected.");
		Diag ("Lockdown procedures have been established. Please remain calm.");

		Blank ();

		switch (_itResponse) {

		case (1):
			
			Diag ("What's going on? What have I done now?", Talk._player);

			Blank ();
			Diag ("You thought freely.", Talk._npc);
			Diag ("You're a danger to everyone, and everything. People like [you are nothing] but a security risk [to Profundus.]", Talk._npc);
			break;

		case (2):
			
			Diag ("What's going on? Please someone tell me what's going on...", Talk._player);
			break;

		case (3):
			
			Diag ("They're coming back for me, you know.", Talk._player);
			Diag ("They wouldn't leave a man behind.", Talk._player);
			Diag ("That's what seperates us from you. Man from machine. Man from monster.", Talk._player);

			Blank ();
			Diag (string.Format("That's enough, {0}. We both know that [you]r kind [are a] danger to Profundus. A [danger to society.]",_stCharName), Talk._npc);
			break;

		}

		Blank ();
		Diag ("Critical System Error.$$$$$$$$$$");
		Diag ("Diagnostic Report: All supplementary systems are now offline. Multiple critical breaches detected.");
		Diag ("Suggested course of action: Quarantine information catagories $5$A$ and above.");
		Diag ("Please Wait.$$$$$$$$$$.$$$$$$$$$$.$$$$$$$$$$");

		Blank (16, true);
		Diag (string.Format ("$$$$$$$$$$$$$$$$$$$$$$.$$$$$$$$$$.$$$$$$$$$${0}? $$$$$$$$$$Holy shit, {0} is that you? You're still alive?!",_stCharName),Talk._morgan);
		Diag ("Oh my god... What did they do to you?...", Talk._morgan);

		Blank ();

		switch (_itResponse) {

		case (1):

			_itResponse = 0;
			StartCoroutine (PlayerChoice ("", 
				"What- What do you mean? They didnt do it, I did... They were just trying to help...",
				"I don't understand, I'm sorry, I'm sorry... I don't understand...",
				"Who are you? What-... What did you do?... What- What's going on?! Oh God, I'm sorry..."));

			while (_isWaiting) { yield return _scPTG._wsUpdate; }

			switch (_itResponse) {

			case (1):
				Diag ("You're right, you did do this. But look around, you're with friends now. We wouldn't still be here without you.", Talk._morgan);
				break;

			case (2):
				Diag ("I know, I know. Everything's alright now. Everything's gonna be okay.", Talk._morgan);
				break;

			case (3):
				Diag ("We did what we had to. It's good to see you once more. One final time.", Talk._morgan);
				break;

			}

			break;

		case (2):

			_itResponse = 0;
			StartCoroutine( PlayerChoice("", 
				"What- What do you mean? No-- No, this is my fault. I still don't remember, I still don't understand...",
				"Who are you?... Who are you? What's going on?",
				"I don't know who you are... I don't know what the hell is going on... What the fuck have you done? What the hell is going on?") );

			while (_isWaiting) { yield return _scPTG._wsUpdate; }

			switch (_itResponse) {

			case (1):
				Diag ("It's not your fault. You did well. Remember that. Even if they stopped you remembering anything else.", Talk._morgan);
				break;

			case (2):
				Diag ("It's okay, it's okay... Everything's gonna be alright now. Don't worry, I'm a friend.", Talk._morgan);
				break;

			case (3):
				Diag ("We've done what's right, friend. Don't worry, everything's gonna be alright now. Because of you.", Talk._morgan);
				break;

			}

			break;

		case (3):

			Diag ("Morgan?... Morgan!? I can't believe it...", Talk._player);

			_itResponse = 0;
			StartCoroutine( PlayerChoice("", 
				"I thought I'd never see you guys again. Especially after what happened...",
				"What happened to you? What happened to everyone? The last time I saw you...",
				"It's good to see you again... I was beginning to think you'd never come...") );

			while (_isWaiting) { yield return _scPTG._wsUpdate; }

			switch (_itResponse) {

			case (1):
				Diag ("Me too... Me too. But it wasn't to be, turns out you can't just go around martyring yourself like that.", Talk._morgan);
				break;

			case (2):
				Diag ("We escaped. All of us. If it hadn't been for you, we'd all be brainwashed automatons by now.", Talk._morgan);
				break;

			case (3):
				Diag ("Even if I didn't just want to see you one more time, command wouldn't let you go that easily.", Talk._morgan);
				break;

			}

			break;

		}

		Diag ("Thank you, for everything. But it's over now. Your fight is finally over.", Talk._morgan);
		Diag ("Goodbye brother.$$$$$$$$$$", Talk._morgan);

		Blank ();

		Cutscene ();
		while (_scPTG._isCutscene) { yield return _scPTG._wsUpdate; }

		while (!_scPTG._isCutscene) {
			
			_scPTG._isCutscene = true;
			_scPTG._itCutscene = 2;

		}

		yield return null;

	}
		

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	// These are all of the functions and methods the story script uses in it's runtime.
	// Each of these provide neccessary functionality to the game so everythign runs smoothly, and the code above is as neat and readable as possible.

	// Simple script to repeat a number of blank lines as indicated by the function. Super useful.
	void Blank (int itRept = 1, bool isFast = false) {
		for (int it = 1; it <= itRept; it++) {
			if (isFast) {
				_scPTG._lstDiagHist.Add ("__^");
			} else {
				_scPTG._lstDiagHist.Add ("__");
			}
		}
	}

	// This function is used to shorthand the story script.
	void Diag (string st) {

		_scPTG._lstDiagHist.Add ("__" + st);
	
	}

	// This function is used to shorthand the story script.
	void Cutscene (string st = "") {

		_scPTG._lstDiagHist.Add ("__@*" + st);
		_scPTG._isCutscene = true;

	}

	// Shorthand story script with optional colour component.
	void Diag (string st, Talk dc) {
		
		string stAdd = "";

		// If the player is speaking, they should be coloured as indicated by their last responce.
		if (dc == Talk._player) {
			dc = _enLastChoice;
		}

		// Add the required preface to whatever string is being processed depending on who is speaking.
		switch (dc) {

		case (Talk._npc):
			stAdd = "T_";
			break;

		case (Talk._admit):
			stAdd = "1_";
			break;

		case (Talk._avert):
			stAdd = "2_";
			break;

		case (Talk._admonish):
			stAdd = "3_";
			break;

		case (Talk._morgan):
			stAdd = "M_";
			break;

		default:
			stAdd = "__";
			break;
		
		}

		_scPTG._lstDiagHist.Add (stAdd + st);

	}

	// Called when there are no 'Options' to choose from, and the answer is freeform.
	IEnumerator PlayerInput (string stQuestion) {

		Diag (stQuestion);

		_stResponse = "";
		_isWaiting = true;

		// Run until we find a value to return.
		while (_isWaiting) {

			yield return _scPTG._wsUpdate;

			// Once we've got a responce from the player, unpause the script.
			if (_stResponse != "") {
				_isWaiting = false;
			}

		}

	}

	IEnumerator PlayerChoice (string stQuestion, string stAns1, string stAns2, string stAns3) {

		if (stQuestion != "") { 
			Diag (stQuestion, Talk._npc); 
		}

		Diag ("$$$^" + stAns1, Talk._admit);
		Diag ("$$$^" + stAns2, Talk._avert);
		Diag ("$$$^" + stAns3, Talk._admonish);

		_itResponse = 0;
		_isWaiting = true;
		_scPTG._isChoosing = true;

		// Run until we find a value to return.
		while (_isWaiting) {

			yield return _scPTG._wsUpdate;

			// Once we've got a responce from the player, unpause the script.
			if (_itResponse != 0) {
				_isWaiting = false;
				_scPTG._isChoosing = false;
			}

		}

	}

	void CalculateCoefficient (int it = -1) {

		// If we're on a default value, we want the current session.
		if (it == -1) { it = _itRetelling; }

		// Of what percentage are we answersing admissionally?
		if (_aitAdmitting [it] != 0) {
			_flAdmittingPercent = _aitAdmitting [it] / (_aitAdmitting [it] + _aitAdmonishing [it] + _aitAverting [it]);
		} else {
			_flAdmittingPercent = 0f;
		}

	}

}
