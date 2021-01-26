using UnityEditor;
using UnityEngine;

namespace Kiln
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Kiln Settings Editor"))
            {
                SettingsWindow.Init();
            }
        }
    }

}