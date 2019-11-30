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
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            ListVisualElement listVisualElement = new ListVisualElement
            {
                bindingPath = nameof(ChildSubset.Children),
                Label = "Children",
                HideAddButton = true,
                HideDeleteAllButton = true,
                HideReorderItemButtons = true,
                HideDeleteItemButton = true,
                DisablePropertyLabel = true
            };
            
            listVisualElement.OnItemCreated += (item) =>
            {
                item.Q<PropertyField>()?.SetEnabled(false);
                Button pingButton = new Button(()=> {
                    EditorGUIUtility.PingObject(item.Q<ObjectField>().value);
                })
                {
                    text = "!"
                };
                item.Add(pingButton);
            };

            root.Add(listVisualElement);
            root.Bind(serializedObject);
            listVisualElement.BindProperty(serializedObject.FindProperty(nameof(ChildSubset.Children)));

            return root;
        }
    }
}
