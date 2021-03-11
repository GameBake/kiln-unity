#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace Kiln
{
    public static class Utils
    {   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static string GetFolderPath(string folder)
        {
            string[] directories = Directory.GetDirectories(Application.dataPath, folder, SearchOption.AllDirectories);
            string path = "";
            foreach (var item in directories)
            {
                path = $"{item.Substring(Application.dataPath.Length + 1)}";
                break;
            }

            if (path == "") { return null; }

            return $"Assets/{path}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetPluginPath()
        {
            return GetFolderPath("Kiln");
        }
    }

    public static class Constants
    {
        /// <summary>
        /// Relative paths
        /// </summary>
        public static class Folders
        {
            public const string Settings = "SDK/Resources";
        }
    }
}
#endif