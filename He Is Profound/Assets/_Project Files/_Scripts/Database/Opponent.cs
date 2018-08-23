using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
	Opponent Class
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

[System.Serializable]
[XmlRoot("OpponentDatabase")]
public class OpponentDatabase {

	[XmlArray("OpponentsInDatabase")]
	[XmlArrayItem("Opponent")]
	public List<OpponentLevel> _lAllOpponents = new List <OpponentLevel> ();

}

[System.Serializable]
public class OpponentLevel {

	[XmlAttribute("Level")]
	public int _itLevel;

	[XmlArray("OpponentsAtLevel")]
	[XmlArrayItem("Opponent")]
	public List<Opponent> _lOpponents = new List <Opponent> ();

}

[System.Serializable]
public class Opponent {

	[XmlAttribute("Name")]
	public string _stName = "";

	[XmlElement("Battlecry")]
	public string _stBattlecry = "";

	[XmlElement("Health")]
	public int _itHealth = 0;
	public int _itHealthCur = 0;

	[XmlElement("Damage")]
	public int _itDamage = 0;

	[XmlElement("Speed")]
	public int _itSpeed = 0;

	[XmlElement("Defence")]
	public int _itDefence = 0;

}