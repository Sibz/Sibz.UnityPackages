using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Sibz.UXMLList;

namespace Sibz.ChildSubset
{
    [CustomEditor(typeof(ChildSubset))]
    public class ChildSubsetInspector : Editor
    {
        public virtual VisualTreeAsset MainContent => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("ChildSubsetInspector t:VisualTreeAsset")[0]));
        public virtual VisualTreeAsset PingButton => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("ChildSubsetPingButton t:VisualTreeAsset")[0]));

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            MainContent.CloneTree(root);

            ListVisualElement listVisualElement = root.Q<ListVisualElement>();

            listVisualElement.OnItemCreated += (item) =>
            {
                item.Q<PropertyField>()?.SetEnabled(false);
                Button pingButton = PingButton.CloneTree().Q<Button>();
                pingButton.clicked += () =>
                 {
                     EditorGUIUtility.PingObject(item.Q<ObjectField>().value);
                 };                
                pingButton.AddToClassList("sibz-child-subset-ping-button");
                item.Add(pingButton);
            };

            root.Bind(serializedObject);

            return root;
        }
    }
}
