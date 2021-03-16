using UnityEngine;
using UnityEditor;

class ImportProcessor
{
    [InitializeOnLoadMethod]
    static void OnProjectLoadedInEditor()
    {
        // We need Android API Level >= 19
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;

        // We need .NET >= 4.x
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);

        Debug.Log("Kiln Plugin imported.");
    }
}