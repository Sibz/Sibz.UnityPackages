using System;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sibz.ListElement
{
    public class ListElement : BindableElement
    {
        private const string DefaultTemplateName = "Sibz.ListElement.Template";
        private const string DefaultItemTemplateName = "Sibz.ListElement.ItemTemplate";
        private const string DefaultStyleSheetName = "Sibz.ListElement.Template";

        public bool HideLabel { get; set; }
        public string Label { get; set; } = "";
        public string TemplateName { get; set; } = DefaultTemplateName;
        public string ItemTemplateName { get; set; } = DefaultItemTemplateName;
        public string StyleSheetName { get; set; } = DefaultStyleSheetName;


        private VisualTreeAsset template;
        private VisualTreeAsset itemTemplate;
        private StyleSheet styleSheet;

        public ListElement() : this(null, null)
        {
        }

        public ListElement(SerializedProperty property) : this(property, string.Empty)
        {
        }

        public ListElement(SerializedProperty property, string label)
        {
            Initialise();
        }

        private void Initialise()
        {
            LoadAssets();
            CloneTemplate();
        }

        private void LoadAssets()
        {
            template = LoadAsset<VisualTreeAsset>(TemplateName);
            itemTemplate = LoadAsset<VisualTreeAsset>(ItemTemplateName);
            styleSheet = LoadAsset<StyleSheet>(StyleSheetName);
        }
        
        private void CloneTemplate()
        {
            Clear();
            template.CloneTree(this);
        }

        private static T LoadAsset<T>(string assetName) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetPathFromName(assetName, typeof(T).Name));
        }

        private static string AssetPathFromName(string name, string typeName = null)
        {
            string filter = string.IsNullOrEmpty(typeName) ? name : $"{name} t:{typeName}";
            try
            {
                return AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(filter)[0]);
            }
            catch
            {
                throw new Exception($"Unable to load asset: {name}");
            }
        }
        
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
        }
        
        public new class UxmlFactory : UxmlFactory<ListElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private UxmlBoolAttributeDescription hideLabel;
            private UxmlStringAttributeDescription label;
            private UxmlStringAttributeDescription templateName;
            private UxmlStringAttributeDescription itemTemplateName;
            private UxmlStringAttributeDescription styleSheetName;
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                (ve as ListElement)?.Initialise();
            }
        }

      
    }
}