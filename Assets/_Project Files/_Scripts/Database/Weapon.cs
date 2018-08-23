using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
	Weapon Class
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

[System.Serializable]
[XmlRoot("WeaponDatabase")]
public class WeaponDatabase {

	[XmlArray("WeaponsInDatabase")]
	[XmlArrayItem("Weapon")]
	public List<WeaponLevel> _lAllWeapons = new List <WeaponLevel> ();

}

[System.Serializable]
public class WeaponLevel {

	[XmlAttribute("Level")]
	public int _itLevel;

	[XmlArray("WeaponsInLevel")]
	[XmlArrayItem("Weapon")]
	public List<Weapon> _lWeapons = new List <Weapon> ();

}

[System.Serializable]
public class Weapon {

	[XmlAttribute("Name")]
	public string _stName = "";

	[XmlElement("Damage")]
	public int _itDamage = 0;

	[XmlElement("Speed")]
	public int _itSpeed = 0;

	[XmlElement("Type")]
	public string _stType = "";

	[XmlElement("Value")]
	public int _itValue = 0;

}

