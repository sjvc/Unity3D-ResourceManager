using UnityEngine;
using System;
using System.Collections.Generic;

/**
	IMPORTANT: Set this script to be executed before the default time (Script Execution Order)
	so strings are loaded as fast as possible
**/

namespace Baviux {

public class ResourceManager : MonoBehaviour {
	public string debugIsoCode = "";

	[HideInInspector]
	public static StringResourceManager stringResources;

	void Awake(){
		stringResources = new StringResourceManager();

		string isoCode = Debug.isDebugBuild && !String.IsNullOrEmpty(debugIsoCode) ? debugIsoCode : StringResourceManager.GetISOCodeFromSystemLanguage();
		stringResources.LoadStrings(null); // Load default language strings
		stringResources.LoadStrings(isoCode); // Override system language existing strings
	}

	public static string GetString(string key){
		return stringResources.GetString(key);
	}

	public static string GetString(string key, object arg0){
		return stringResources.GetString(key, arg0);
	}

	public static string GetString(string key, object arg0, object arg1){
		return stringResources.GetString(key, arg0, arg1);
	}

	public static string GetString(string key, object arg0, object arg1, object arg2){
		return stringResources.GetString(key, arg0, arg1, arg2);
	}

	public static string GetString(string key, params object[] args){
		return stringResources.GetString(key, args);
	}
}

}