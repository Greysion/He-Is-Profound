using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

/* --------------------------------------------------------------------------------------------------------------------------------------------------------- //
	Armour Class
// --------------------------------------------------------------------------------------------------------------------------------------------------------- */

[System.Serializable]
[XmlRoot("ArmourDatabase")]
public class ArmourDatabase {

	[XmlArray("ArmourInDatabase")]
	[XmlArrayItem("Armour")]
	public List<ArmourLevel> _lAllArmours = new List <ArmourLevel> ();

}

[System.Serializable]
public class ArmourLevel {

	[XmlAttribute("Level")]
	public int _itLevel;

	[XmlArray("ArmourInLevel")]
	[XmlArrayItem("Armour")]
	public List<Armour> _lArmours = new List <Armour> ();

}

[System.Serializable]
public class Armour {

	[XmlAttribute("Name")]
	public string _stName = "";

	[XmlElement("Damage")]
	public int _itDefence = 0;

	[XmlElement("Value")]
	public int _itValue = 0;

}