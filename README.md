# Unity3D-ResourceManager
Strings management for Unity using Android format

# Usage
* Place your xml string files using [Android syntax](http://developer.android.com/intl/es/guide/topics/resources/string-resource.html) in [Resources](http://wiki.unity3d.com/index.php/Special_Folder_Names_in_your_Assets_Folder#.22Resources.22)/Strings/values[-iso_code] folders. 
  * Default folder should be named `values`. 
  * Language specific folders should be named `values-iso_language_code`.
  * You can name the xml file as you wish
  * You can have as many files as you want
  * You can use multiple Resource folders
* Copy Assets folder in this repository to your project
* Add ResouceManager.cs script to a GameObject in your scene
* In your code, call `ResourceManager.GetString(Strings.yourStringKey)` to get your localized resource.

## Composites
String concatenation generates garbage. In order to avoid it, instead of concatenate strings at runtime, you can create them on a file called composites.xml in [Resources](http://wiki.unity3d.com/index.php/Special_Folder_Names_in_your_Assets_Folder#.22Resources.22)/Strings folder, using {{key}} to refer to an existing string key. So they can be used as any other string resource.

For example, if you have this `strings.xml` file:
```
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <string name="hello">Hello</string>
    <string name="how_are_you">How are you?</string>
</resources>
```

And this `composites.xml` file:
```
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <string name="hello_how_are_you">{{hello}}. {{how_are_you}}</string>
</resources>
```

Then, `ResourceManager.GetString(Strings.helloHowAreYou)` output will be: *Hello. How are you?*
