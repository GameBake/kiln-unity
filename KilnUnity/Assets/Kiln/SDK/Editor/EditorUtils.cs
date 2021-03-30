using System;
using UnityEditor;
using UnityEngine;

namespace Kiln
{
    public static class EditorUtils 
    {
        public static string TextFieldWithPlaceholder(Rect rect, string text, string placeholder) 
        {
            string newText = EditorGUI.TextField(rect, text);

            if (String.IsNullOrEmpty(text)) 
            {
                Color guiColor = GUI.color;
                GUI.color = Color.grey;
                EditorGUI.LabelField(rect, placeholder);
                GUI.color = guiColor;
            }
            
            return newText;
        }
    
        public static float FloatFieldWithPlaceholder(Rect rect, float value, string placeholder) 
        {
            float newValue = EditorGUI.FloatField(rect, value);

            if (value == 0f) 
            {
                Color guiColor = GUI.color;
                GUI.color = Color.grey;
                
                GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
                EditorGUI.LabelField(rect, placeholder, style);
                GUI.color = guiColor;
            }
            
            return newValue;
        }
    }
}
