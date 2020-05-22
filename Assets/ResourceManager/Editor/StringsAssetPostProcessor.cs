using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Baviux {

public class StringsAssetPostProcessor : AssetPostprocessor {
	private const string STRINGS_XML_PATH_REGEXP = @"^Assets(/.*)*/Resources/Strings/values/.+\.xml$";
	private const string COMPOSITES_XML_PATH_REGEXP = @"^Assets/Resources/Strings/composites\.xml$";
	private const string STRINGS_CS_PATH = "Scripts/Strings.cs";

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths){
		foreach (string asset in importedAssets){
			if (Regex.IsMatch(asset, STRINGS_XML_PATH_REGEXP) || Regex.IsMatch(asset, COMPOSITES_XML_PATH_REGEXP)){
				UpdateStringsCsFile();
				return;
			}
		}
	}

	[MenuItem("Assets/Generate Strings.cs file", false, 10000)]
	public static void UpdateStringsCsFile(){
		string fileContent = "public static class Strings {\n\n";
		fileContent += GetStringVars(Resources.LoadAll<TextAsset>("Strings/values/"));
		fileContent += GetStringVars(Resources.Load<TextAsset>("Strings/composites"));
		fileContent += "\n}";

		File.WriteAllText( Path.Combine(Application.dataPath, STRINGS_CS_PATH), fileContent );

		Debug.Log(STRINGS_CS_PATH + " file updated");
	}

	private static string GetStringVars(TextAsset stringsFileAsset){
		return GetStringVars(new TextAsset[]{stringsFileAsset});
	}

	private static string GetStringVars(TextAsset[] stringsFileAssets){
		string output = "";

		foreach(TextAsset stringsFileAsset in stringsFileAssets){
			if (stringsFileAsset != null){
				XmlDocument xmlStringsDoc = new XmlDocument();
				xmlStringsDoc.LoadXml(stringsFileAsset.text);

				for(int i=0, size=xmlStringsDoc.DocumentElement.ChildNodes.Count; i<size; i++){
					XmlNode xmlNode = xmlStringsDoc.DocumentElement.ChildNodes[i];
					string stringKey = xmlNode.Attributes["name"].Value;
					output += "\tpublic static string " + Camelize(stringKey) + " = \"" + stringKey + "\";\n";
				}
			}
		}

		return output;
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

}
