#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildPreProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
    {
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            Debug.LogWarning($"<size=20>Kiln only supports Android for the moment.</size>");
        }
    }
}
#endif