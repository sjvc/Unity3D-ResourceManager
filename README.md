# Unity3D-ResourceManager
Strings management for Unity using Android format

# Usage

* Place your xml string files using [Android syntax](http://developer.android.com/intl/es/guide/topics/resources/string-resource.html) in [Resources](http://wiki.unity3d.com/index.php/Special_Folder_Names_in_your_Assets_Folder#.22Resources.22)/Strings folder. 
  * Default language file should be named `strings.xml`. 
  * Language specific files should be named `strings-iso_language_code.xml`. 
* Copy Assets folder in this repository to your project
* Add ResouceManager.cs script to a GameObject in your scene
* In your code, call `ResourceManager.GetString(Strings.yourStringKey)` to get your localized resource.
