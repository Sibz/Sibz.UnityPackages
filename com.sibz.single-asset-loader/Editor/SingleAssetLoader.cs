using UnityEditor;
using UnityEngine;

namespace Sibz.SingleAssetLoader
{
    public static class SingleAssetLoader
    {
        public static Object Load(string name, System.Type type)
        {
            return AssetDatabase.LoadAssetAtPath<Object>(AssetPathFromName(name, type.Name));
        }

        public static T Load<T>(string name) where T : Object
        {
            return Load(name, typeof(T)) as T;
        }

        private static string AssetPathFromName(string name, string typeName = null)
        {
            string filter = string.IsNullOrEmpty(typeName) ? name : $"{name} t:{typeName}";
            var assets = AssetDatabase.FindAssets(filter);
            
            if (assets.Length == 0)
            {
                throw new System.Exception($"Unable to load asset: {name}");
            }

            return AssetDatabase.GUIDToAssetPath(assets[0]);
        }
    }
}