using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace Baviux {

public class StringResourceUtils {

	[MenuItem("Utils/Check Missing Strings")]
	static void CheckMissingStrings(){
        List<string> missingStrings = new List<string>();
		CheckMissingStrings(missingStrings);

        string msg = "";
		for (int i=0; i<missingStrings.Count; i++){
			msg += string.Format("Missing string: {0}", missingStrings[i]) + "\n";
		}

        EditorUtility.DisplayDialog("", string.IsNullOrEmpty(msg) ? "No missing strings" : msg, "Close");
    }

    public static void CheckMissingStrings(List<string> missingStrings){
		missingStrings.Clear();

		StringResourceManager resourceManager = new StringResourceManager();
		List<string> providedLanguages = GetProvidedIsoCodes();
		resourceManager.LoadStrings("", false, false);
		string[] stringKeys = new string[resourceManager.GetStringCount()];
		resourceManager.GetStringKeys(stringKeys);

		foreach(string language in providedLanguages){
			resourceManager.ClearStrings();
			resourceManager.LoadStrings(language, false, false);
			foreach(string stringKey in stringKeys){
				if (!resourceManager.ContainsString(stringKey)){
					missingStrings.Add(string.Format("{0} ({1})", stringKey, language));
				}
			}
		}
	}

	public static List<string> GetProvidedIsoCodes(){
		List<string> providedIsoCodes = new List<string>();
		providedIsoCodes.Add(""); // Default language
		Array allLanguages = Enum.GetValues(typeof(SystemLanguage));
		for (int i=0, size=allLanguages.Length; i<size; i++){
			string langIsoCode = StringResourceManager.GetISOCodeFromLanguage( (SystemLanguage)allLanguages.GetValue(i) );
			if (!providedIsoCodes.Contains(langIsoCode)){ // Avoid duplicate entries
				TextAsset stringsFileAsset = Resources.Load<TextAsset>(string.Format("Strings/values-{0}/strings", langIsoCode));
				if (stringsFileAsset != null){
					providedIsoCodes.Add( langIsoCode );
				}
			}
		}

		return providedIsoCodes;
	}

}

}