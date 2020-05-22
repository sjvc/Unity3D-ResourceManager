using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

/// <summary>
/// For text resources, it expects Android xml values file format
/// </summary>
public class StringResourceManager {
	private static Regex placeHoldersRegEx = new Regex(@"%(\d+)\$.");
	private static string[] placeHolderGroups = new string[20];
	
	private Dictionary<string, string> strings = new Dictionary<string, string>();

	public StringResourceManager(){
		for (int i=0; i<placeHolderGroups.Length; i++){
			placeHolderGroups[i] = string.Format("{{{0}}}", i);
		}
	}

	public bool LoadStrings(string langIsoCode, bool loadAllFiles = true, bool loadComposites = true){
		bool loaded = false;
		TextAsset[] stringFileAssets;
		string stringsFolder = string.Format("Strings/values{0}/", String.IsNullOrEmpty(langIsoCode) ? "" : string.Format("-{0}", langIsoCode));

		if (loadAllFiles){
			stringFileAssets = Resources.LoadAll<TextAsset>(stringsFolder);
		} else{
			stringFileAssets = new TextAsset[]{
				Resources.Load<TextAsset>(string.Format("{0}strings", stringsFolder))
			};
		}
		
		for (int r=0; r<stringFileAssets.Length; r++){ 
			if (stringFileAssets[r] != null){
				XmlDocument xmlStringsDoc = new XmlDocument();
				xmlStringsDoc.LoadXml(stringFileAssets[r].text);

				for(int i=0, size=xmlStringsDoc.DocumentElement.ChildNodes.Count; i<size; i++){
					XmlNode xmlNode = xmlStringsDoc.DocumentElement.ChildNodes[i];
					strings[xmlNode.Attributes["name"].Value] = Regex.Unescape(xmlNode.InnerText);
				}

				loaded = true;
			}
		}

		if (loaded && loadComposites){
			LoadComposites();
		}

		if (!loaded && langIsoCode != null && langIsoCode.Contains("-")){
			return LoadStrings(langIsoCode.Substring(0, langIsoCode.LastIndexOf("-")), loadAllFiles, loadComposites);
		}

		return loaded;
	}

	private void LoadComposites(){
		TextAsset stringsFileAsset = Resources.Load<TextAsset>("Strings/composites");

		if (stringsFileAsset != null){
			XmlDocument xmlStringsDoc = new XmlDocument();
			xmlStringsDoc.LoadXml(stringsFileAsset.text);

			for(int i=0, size=xmlStringsDoc.DocumentElement.ChildNodes.Count; i<size; i++){
				XmlNode xmlNode = xmlStringsDoc.DocumentElement.ChildNodes[i];
				string text = Regex.Unescape(xmlNode.InnerText);
				strings[xmlNode.Attributes["name"].Value] = Regex.Replace(text, @"{{[^}}]+}}", m => strings[m.Value.ToString().Replace("{{","").Replace("}}", "")] );
			}
		}
	}

	public void ClearStrings(){
		strings.Clear();
	}

	public void GetStringKeys(string[] keys){
		strings.Keys.CopyTo(keys, 0);
	}

	public int GetStringCount(){
		return strings.Keys.Count;
	}

	public bool ContainsString(string key){
		return strings.ContainsKey(key);
	}

	public string GetString(string key){
		string str;
		if (strings.TryGetValue(key, out str)){
			return ConvertPlaceHolders(str);
		}

		Logger.WriteWarning(string.Format("Strings array doesn't contain the key: {0}", key));
		return "[" + key + "]";
	}

	public string GetString(string key, object arg0){
		return string.Format (GetString(key), arg0);
	}

	public string GetString(string key, object arg0, object arg1){
		return string.Format (GetString(key), arg0, arg1);
	}

	public string GetString(string key, object arg0, object arg1, object arg2){
		return string.Format (GetString(key), arg0, arg1, arg2);
	}

	public string GetString(string key, params object[] args){
		return string.Format(GetString(key), args);
	}

	/// <summary>
	/// Converts placeholders from Android format to mono format
	/// </summary>
	private static string ConvertPlaceHolders(string str){
		return placeHoldersRegEx.Replace(str, m => placeHolderGroups[ int.Parse(m.Groups[1].Value) - 1 ]);
	}

	public static string GetISOCodeFromSystemLanguage() {
		return GetISOCodeFromLanguage(Application.systemLanguage);
	}

	public static string GetISOCodeFromLanguage(SystemLanguage language){
		string code = "en";
		switch (language) {
			case SystemLanguage.Afrikaans: code = "af"; break;
			case SystemLanguage.Arabic: code = "ar"; break;
			case SystemLanguage.Basque: code = "eu"; break;
			case SystemLanguage.Belarusian: code = "by"; break;
			case SystemLanguage.Bulgarian: code = "bg"; break;
			case SystemLanguage.Catalan: code = "ca"; break;
			case SystemLanguage.Chinese: code = "zh"; break;
			case SystemLanguage.ChineseSimplified: code = "zh"; break;
			case SystemLanguage.ChineseTraditional: code = "zh-rTW"; break;
			case SystemLanguage.Czech: code = "cs"; break;
			case SystemLanguage.Danish: code = "da"; break;
			case SystemLanguage.Dutch: code = "nl"; break;
			case SystemLanguage.English: code = "en"; break;
			case SystemLanguage.Estonian: code = "et"; break;
			case SystemLanguage.Faroese: code = "fo"; break;
			case SystemLanguage.Finnish: code = "fi"; break;
			case SystemLanguage.French: code = "fr"; break;
			case SystemLanguage.German: code = "de"; break;
			case SystemLanguage.Greek: code = "el"; break;
			case SystemLanguage.Hebrew: code = "iw"; break;
			case SystemLanguage.Hungarian: code = "hu"; break;
			case SystemLanguage.Icelandic: code = "is"; break;
			case SystemLanguage.Indonesian: code = "in"; break;
			case SystemLanguage.Italian: code = "it"; break;
			case SystemLanguage.Japanese: code = "ja"; break;
			case SystemLanguage.Korean: code = "ko"; break;
			case SystemLanguage.Latvian: code = "lv"; break;
			case SystemLanguage.Lithuanian: code = "lt"; break;
			case SystemLanguage.Norwegian: code = "no"; break;
			case SystemLanguage.Polish: code = "pl"; break;
			case SystemLanguage.Portuguese: code = "pt"; break;
			case SystemLanguage.Romanian: code = "ro"; break;
			case SystemLanguage.Russian: code = "ru"; break;
			case SystemLanguage.SerboCroatian: code = "sh"; break;
			case SystemLanguage.Slovak: code = "sk"; break;
			case SystemLanguage.Slovenian: code = "sl"; break;
			case SystemLanguage.Spanish: code = "es"; break;
			case SystemLanguage.Swedish: code = "sv"; break;
			case SystemLanguage.Thai: code = "th"; break;
			case SystemLanguage.Turkish: code = "tr"; break;
			case SystemLanguage.Ukrainian: code = "uk"; break;
			case SystemLanguage.Unknown: code = "en"; break;
			case SystemLanguage.Vietnamese: code = "vi"; break;
		}

		return code;
	}
}
