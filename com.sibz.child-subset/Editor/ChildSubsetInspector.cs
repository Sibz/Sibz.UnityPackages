using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Sibz.UXMLList;

namespace Sibz.ChildSubset
{
    [CustomEditor(typeof(ChildSubset))]
    public class ChildSubsetInspector : Editor
    {
        protected virtual VisualTreeAsset MainContent => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("ChildSubsetInspector t:VisualTreeAsset")[0]));

        protected virtual VisualTreeAsset PingButton => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("ChildSubsetPingButton t:VisualTreeAsset")[0]));

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            MainContent.CloneTree(root);

            AttachHandlersForItemCreation(root.Q<ListVisualElement>());

            root.Bind(serializedObject);

            return root;
        }

        private void AttachHandlersForItemCreation(ListVisualElement listVisualElement)
        {
            listVisualElement.OnItemCreated += DisablePropertyField;
            listVisualElement.OnItemCreated += AddPingButtonWithEventHandler;
        }

        private static void DisablePropertyField(VisualElement itemSection)
        {
            itemSection.Q<PropertyField>()?.SetEnabled(false);
        }

        private void AddPingButtonWithEventHandler(VisualElement itemSection)
        {
            Button pingButton = PingButton.CloneTree().Q<Button>();

            pingButton.clicked += () => { EditorGUIUtility.PingObject(itemSection.Q<ObjectField>().value); };

            pingButton.AddToClassList("sibz-child-subset-ping-button");
            itemSection.Add(pingButton);
        }
    }
}
