using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Baviux {

public class UsedGlyphs {
	private static string defaultFontGlyphs = @"!""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~ ¡¢£¤¥¦§¨©ª«¬­®°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿ–—‘’‚“”„†‡•…‰‹›€™";

	private static Dictionary<string, List<char>> extraLanguageGlyphs = new Dictionary<string, List<char>>();

	private enum CharFormat{
		Char,
		Unicode
	}

	private static void SetExtraLanguageGlyphs(){
		extraLanguageGlyphs["ru"] = new List<char>();
		extraLanguageGlyphs["ru"].Add('₽'); // Russian money (ruble) symbol
	}

	[MenuItem("Utils/Generate Used Glyphs List")]
	static void GenerateUsedGlyphs(){
		SetExtraLanguageGlyphs();

		StringResourceManager resources = new StringResourceManager();
		List<string> languages = StringResourceUtils.GetProvidedIsoCodes();
		string filePath = System.IO.Path.Combine( System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "UnityProjectGlyphs.txt" );

		string fileContents = "";

		AddUnicodeArrayByLanguage(languages, resources, ref fileContents);
		AddAllCharsTogether(languages, resources, ref fileContents);

		System.IO.File.WriteAllText(filePath, fileContents);

		EditorUtility.DisplayDialog("Glyph list generated", "Glyph list generated at " + filePath, "Ok");
	}

	static void AddUnicodeArrayByLanguage(List<string> languages, StringResourceManager resources, ref string str){
		List<char> usedGlyphsList = new List<char>();

		str += "fontGlyphs = ['";
		foreach(char c in defaultFontGlyphs){
			AddGlyph(c, CharFormat.Unicode, c == defaultFontGlyphs[0] ? "" : "', '", ref str, ref usedGlyphsList);
		}

		str +=  "]\n\n";

		for (int i=0; i<languages.Count; i++){
			string language = languages[i];
			resources.LoadStrings( language );

			str += (language.Length == 0 ? "default" : language) + "Glyphs = ['";
			string[] stringKeys = new string[resources.GetStringCount()];
			resources.GetStringKeys(stringKeys);
			string languageGlyphs = "";
			foreach(string stringKey in stringKeys){
				string stringValue = resources.GetString(stringKey);
				foreach(char c in stringValue){
					AddGlyph(c, CharFormat.Unicode, "', '", ref languageGlyphs, ref usedGlyphsList);
				}
			}
			if (extraLanguageGlyphs.ContainsKey(language)){
				foreach(char c in extraLanguageGlyphs[language]){
					AddGlyph(c, CharFormat.Unicode, "', '", ref languageGlyphs, ref usedGlyphsList);
				}
			}
			str += languageGlyphs + "']\n\n";
		}
	}

	static void AddAllCharsTogether(List<string> languages, StringResourceManager resources, ref string str){
		List<char> usedGlyphsList = new List<char>();

		str += "All characters together" + "\n" + "-------------------" + "\n";

		foreach(char c in defaultFontGlyphs){
			AddGlyph(c, CharFormat.Char, "", ref str, ref usedGlyphsList);
		}

		for (int i=0; i<languages.Count; i++){
			string language = languages[i];
			resources.LoadStrings( language );

			string[] stringKeys = new string[resources.GetStringCount()];
			resources.GetStringKeys(stringKeys);
			foreach(string stringKey in stringKeys){
				string stringValue = resources.GetString(stringKey);
				foreach(char c in stringValue){
					AddGlyph(c, CharFormat.Char, "", ref str, ref usedGlyphsList);
				}
			}
			if (extraLanguageGlyphs.ContainsKey(language)){
				foreach(char c in extraLanguageGlyphs[language]){
					AddGlyph(c, CharFormat.Char, "", ref str, ref usedGlyphsList);
				}
			}
		}

		str +=  "\n\n";
	}

	static void AddGlyph(char c, CharFormat f, string join, ref string str, ref List<char> usedGlyphsList){
		if (c != ' ' && !usedGlyphsList.Contains(c)){
			str += ((str.Length == 0) ? "" : join) + (f == CharFormat.Char ? c.ToString() : ((int)c).ToString("X").PadLeft(4, '0'));
			usedGlyphsList.Add(c);
		}
	}
}

}
