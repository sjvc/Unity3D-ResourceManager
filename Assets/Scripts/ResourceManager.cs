using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Text.RegularExpressions;

/**
	IMPORTANT: Set this script to be executed before the default time (Script Execution Order)
	so strings are loaded as fast as possible
**/

/// <summary>
/// For text resources, it expects Android xml values file format
/// </summary>
public class ResourceManager : MonoBehaviour {

	[HideInInspector]
	public static ResourceManager instance;
	private static Regex placeHoldersRegEx = new Regex(@"%(\d+)\$.");
	private static string[] placeHolderGroups = new string[20];
	
	protected Dictionary<string, string> strings = new Dictionary<string, string>();
	
	void Awake(){
		instance = this;

		for (int i=0; i<placeHolderGroups.Length; i++){
			placeHolderGroups[i] = string.Format("{{0}}", i);
		}

		LoadStrings(null); // Load default language strings
		LoadStrings(GetISOCodeFromSystemLanguage()); // Override system language existing strings
	}

	public void LoadStrings(string lang){
		TextAsset stringsFileAsset = Resources.Load<TextAsset>("strings" + (String.IsNullOrEmpty(lang) ? "" : ("-" + lang)));

		if (stringsFileAsset != null){
			XmlDocument xmlStringsDoc = new XmlDocument();
			xmlStringsDoc.LoadXml(stringsFileAsset.text);

			for(int i=0, size=xmlStringsDoc.DocumentElement.ChildNodes.Count; i<size; i++){
				XmlNode xmlNode = xmlStringsDoc.DocumentElement.ChildNodes[i];
				strings[xmlNode.Attributes["name"].Value] = Regex.Unescape(xmlNode.InnerText);
			}
		}
	}

	public static string GetString(string key){
		string str;
		if (instance.strings.TryGetValue(key, out str)){
			return ConvertPlaceHolders(str);
		}

		Logger.WriteWarning("Strings array doesn't contain the key: " + key);
		return "[" + key + "]";
	}

	/// <summary>
	/// Converts placeholders from Android format to mono format
	/// </summary>
	private static string ConvertPlaceHolders(string str){
		return placeHoldersRegEx.Replace(str, m => placeHolderGroups[ int.Parse(m.Groups[1].Value) - 1 ]);
	}

	public static string GetString(string key, object arg0){
		return string.Format (GetString(key), arg0);
	}

	public static string GetString(string key, object arg0, object arg1){
		return string.Format (GetString(key), arg0, arg1);
	}

	public static string GetString(string key, object arg0, object arg1, object arg2){
		return string.Format (GetString(key), arg0, arg1, arg2);
	}

	public static string GetString(string key, params object[] args){
		return string.Format(GetString(key), args);
	}

	public static string GetISOCodeFromSystemLanguage() {
		SystemLanguage lang = Application.systemLanguage;
		string res = "en";
		switch (lang) {
			case SystemLanguage.Afrikaans: res = "af"; break;
			case SystemLanguage.Arabic: res = "ar"; break;
			case SystemLanguage.Basque: res = "eu"; break;
			case SystemLanguage.Belarusian: res = "by"; break;
			case SystemLanguage.Bulgarian: res = "bg"; break;
			case SystemLanguage.Catalan: res = "ca"; break;
			case SystemLanguage.Chinese: res = "zh_CN"; break;
			case SystemLanguage.ChineseSimplified: res = "zh_CN"; break;
			case SystemLanguage.ChineseTraditional: res = "zh_TW"; break;
			case SystemLanguage.Czech: res = "cs"; break;
			case SystemLanguage.Danish: res = "da"; break;
			case SystemLanguage.Dutch: res = "nl"; break;
			case SystemLanguage.English: res = "en"; break;
			case SystemLanguage.Estonian: res = "et"; break;
			case SystemLanguage.Faroese: res = "fo"; break;
			case SystemLanguage.Finnish: res = "fi"; break;
			case SystemLanguage.French: res = "fr"; break;
			case SystemLanguage.German: res = "de"; break;
			case SystemLanguage.Greek: res = "el"; break;
			case SystemLanguage.Hebrew: res = "iw"; break;
			case SystemLanguage.Hungarian: res = "hu"; break;
			case SystemLanguage.Icelandic: res = "is"; break;
			case SystemLanguage.Indonesian: res = "in"; break;
			case SystemLanguage.Italian: res = "it"; break;
			case SystemLanguage.Japanese: res = "ja"; break;
			case SystemLanguage.Korean: res = "ko"; break;
			case SystemLanguage.Latvian: res = "lv"; break;
			case SystemLanguage.Lithuanian: res = "lt"; break;
			case SystemLanguage.Norwegian: res = "no"; break;
			case SystemLanguage.Polish: res = "pl"; break;
			case SystemLanguage.Portuguese: res = "pt"; break;
			case SystemLanguage.Romanian: res = "ro"; break;
			case SystemLanguage.Russian: res = "ru"; break;
			case SystemLanguage.SerboCroatian: res = "sh"; break;
			case SystemLanguage.Slovak: res = "sk"; break;
			case SystemLanguage.Slovenian: res = "sl"; break;
			case SystemLanguage.Spanish: res = "es"; break;
			case SystemLanguage.Swedish: res = "sv"; break;
			case SystemLanguage.Thai: res = "th"; break;
			case SystemLanguage.Turkish: res = "tr"; break;
			case SystemLanguage.Ukrainian: res = "uk"; break;
			case SystemLanguage.Unknown: res = "en"; break;
			case SystemLanguage.Vietnamese: res = "vi"; break;
		}

		return res;
	}
}