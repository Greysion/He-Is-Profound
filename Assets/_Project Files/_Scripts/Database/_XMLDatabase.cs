using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using UnityEngine;

/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
   Author: 			Hayden Reeve
   File:			XMLDatabase.cs
   Description: 	This script operates, manages, and creates all of the necessary components for the XML Database, including Weapons, Armour, and Opponents. 
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

public enum DatabaseType {_weapon, _armour, _opponent, _all};
public enum DatabaseAction {_load, _save};

public class _XMLDatabase : MonoBehaviour {

	// ---- Static Variables

	public static _XMLDatabase _xml;

	// ---- Inspector Variables

	[Header("Operation Control")]
	public bool _isLoading = false;
	public bool _isSaving = false;

	[Header("Database Editor")]
	public WeaponDatabase _weaponDB;
	public ArmourDatabase _armourDB;
	public OpponentDatabase _opponentDB;

	// ---- On Awake, Load / Save according to the Editor Preferances.

	void Awake () {

		_xml = this;

		if (_isLoading) {
			AccessDatabase (DatabaseType._all, DatabaseAction._load);
		}

		if (_isSaving) {
			AccessDatabase (DatabaseType._all, DatabaseAction._save);
		}

	}

	// ---- On Application Quit, we should attempt to save all of our databases if save is ticked.

	void OnApplicationQuit() {

		if (_isSaving) {
			AccessDatabase (DatabaseType._all, DatabaseAction._save);
		}

	}

	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	public void AccessDatabase (DatabaseType dbType, DatabaseAction dbAction) {

		switch (dbType) {

		case (DatabaseType._weapon):

			if (dbAction == DatabaseAction._save) {
				SaveWeaponDatabase ();
			} else if (dbAction == DatabaseAction._load) {
				LoadWeaponDatabase ();
			}

			break;

		case (DatabaseType._armour):
			
			if (dbAction == DatabaseAction._save) {
				SaveArmourDatabase ();
			} else if (dbAction == DatabaseAction._load) {
				LoadArmourDatabase ();
			}

			break;

		case (DatabaseType._opponent):
			
			if (dbAction == DatabaseAction._save) {
				SaveOpponentDatabase ();
			} else if (dbAction == DatabaseAction._load) {
				LoadOpponentDatabase ();
			}

			break;

		case (DatabaseType._all):

			if (dbAction == DatabaseAction._save) {
				SaveWeaponDatabase ();
				SaveArmourDatabase ();
				SaveOpponentDatabase ();
			} else if (dbAction == DatabaseAction._load) {
				LoadWeaponDatabase ();
				LoadArmourDatabase ();
				LoadOpponentDatabase ();
			}

			break;

		}

	}

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		WEAPON Save / Load Functions
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	public void SaveWeaponDatabase () {

		XmlSerializer xmlSerializer = new XmlSerializer (typeof(WeaponDatabase));
		StreamWriter xmlStream = new StreamWriter (Application.dataPath + "/StreamingAssets/Weapons.xml", false, System.Text.Encoding.GetEncoding ("UTF-8"));

		xmlSerializer.Serialize (xmlStream, _weaponDB);
		xmlStream.Close ();
		
	}

	public void LoadWeaponDatabase () {

		XmlSerializer xmlSerializer = new XmlSerializer (typeof(WeaponDatabase));
		FileStream xmlStream = new FileStream (Application.dataPath + "/StreamingAssets/Weapons.xml", FileMode.Open);

		_weaponDB = xmlSerializer.Deserialize (xmlStream) as WeaponDatabase;
		xmlStream.Close ();

	}

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		ARMOUR Save / Load Functions
	// -------------------------------------------------------------------------------------------------------------------------------------------------------- */

	public void SaveArmourDatabase () {

		XmlSerializer xmlSerializer = new XmlSerializer (typeof(ArmourDatabase));
		StreamWriter xmlStream = new StreamWriter (Application.dataPath + "/StreamingAssets/Armour.xml", false, System.Text.Encoding.GetEncoding ("UTF-8"));

		xmlSerializer.Serialize (xmlStream, _armourDB);
		xmlStream.Close ();

	}

	public void LoadArmourDatabase() {

		XmlSerializer xmlSerializer = new XmlSerializer (typeof(ArmourDatabase));
		FileStream xmlStream = new FileStream (Application.dataPath + "/StreamingAssets/Armour.xml", FileMode.OpenOrCreate);

		_armourDB = xmlSerializer.Deserialize (xmlStream) as ArmourDatabase;
		xmlStream.Close ();

	}

	/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
		OPPONENT Save / Load Functions
	// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

	public void SaveOpponentDatabase () {

		XmlSerializer xmlSerializer = new XmlSerializer (typeof(OpponentDatabase));
		StreamWriter xmlStream = new StreamWriter (Application.dataPath + "/StreamingAssets/Opponents.xml", false, System.Text.Encoding.GetEncoding ("UTF-8"));

		xmlSerializer.Serialize (xmlStream, _opponentDB);
		xmlStream.Close ();

	}

	public void LoadOpponentDatabase() {

		XmlSerializer xmlSerializer = new XmlSerializer (typeof(OpponentDatabase));
		FileStream xmlStream = new FileStream (Application.dataPath + "/StreamingAssets/Opponents.xml", FileMode.OpenOrCreate);

		_opponentDB = xmlSerializer.Deserialize (xmlStream) as OpponentDatabase;
		xmlStream.Close ();

	}

}