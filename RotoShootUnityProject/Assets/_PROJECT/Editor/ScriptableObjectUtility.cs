using UnityEngine;
using UnityEditor;
using System.IO;


/// http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
/// This is a method to easily create a new asset file instance of a ScriptableObject-derived class. The asset is uniquely named and placed in the currently selected project path; this mimics the way Unity's built-in assets are created....
/// 
public static class ScriptableObjectUtility
{
  /// <summary>
  //	This makes it easy to create, name and place unique new ScriptableObject asset files.
  /// </summary>
  public static void CreateAsset<T>() where T : ScriptableObject
  {
    T asset = ScriptableObject.CreateInstance<T>();

    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    if (path == "")
    {
      path = "Assets";
    }
    else if (Path.GetExtension(path) != "")
    {
      path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
    }

    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

    AssetDatabase.CreateAsset(asset, assetPathAndName);

    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
    EditorUtility.FocusProjectWindow();
    Selection.activeObject = asset;
  }
}