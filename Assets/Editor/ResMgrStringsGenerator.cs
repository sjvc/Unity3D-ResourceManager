using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class ResMgrStringsGenerator : AssetPostprocessor {

	private const string STRINGS_XML_PATH_REGEXP = @"^Assets/Resources/strings.xml$";
	private const string STRINGS_CS_PATH = "Scripts/Strings.cs";

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths){
		foreach (string asset in importedAssets){
			if (Regex.IsMatch(asset, STRINGS_XML_PATH_REGEXP)){
				UpdateStringsCsFile();
				return;
			}
		}
	}

	[MenuItem("Assets/Generate Strings.cs file", false, 10000)]
	public static void UpdateStringsCsFile(){
		TextAsset stringsFileAsset = Resources.Load<TextAsset>("strings");

		if (stringsFileAsset != null){
			XmlDocument xmlStringsDoc = new XmlDocument();
			xmlStringsDoc.LoadXml(stringsFileAsset.text);

			string fileContents = "public static class Strings {\n\n";

			for(int i=0, size=xmlStringsDoc.DocumentElement.ChildNodes.Count; i<size; i++){
				XmlNode xmlNode = xmlStringsDoc.DocumentElement.ChildNodes[i];
				string stringKey = xmlNode.Attributes["name"].Value;
				fileContents += "\tpublic static string " + Camelize(stringKey) + " = \"" + stringKey + "\";\n";
			}

			fileContents += "\n}";

			File.WriteAllText( Path.Combine(Application.dataPath, STRINGS_CS_PATH), fileContents );

			Debug.Log(STRINGS_CS_PATH + " file updated");
		}
	}

	private static string Camelize(string str){
		string result = "";

		string[] strArray = str.Split('_');
		foreach(string word in strArray) {
			result += (result == "" ? word.Substring(0, 1) : word.Substring(0, 1).ToUpper()) + word.Substring(1);
		}

		return result;
	}

}
