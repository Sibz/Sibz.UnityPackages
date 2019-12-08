using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sibz.ListElement
{
    /// <summary>
    /// TODO
    /// Must initialise when constructed with SP
    /// Must initialise after values loaded from bag
    /// Must reset the list when SP changes
    /// On Initialise must load Templates and style
    /// On Initialise must insert main template and apply style
    /// On Initialise must bind events to main buttons (Add/Clear List/Yes/Cancel)
    /// On Reset, insert SP bound row, with bound buttons (Delete/MoveUp/MoveDown)
    ///  
    /// </summary>
    public class ListElement : BindableElement
    {
        private const string DefaultTemplateName = "Sibz.ListElement.Template";
        private const string DefaultItemTemplateName = "Sibz.ListElement.ItemTemplate";
        private const string DefaultStyleSheetName = "Sibz.ListElement.Template";
        private VisualTreeAsset itemTemplate;
        private StyleSheet styleSheet;
        private VisualTreeAsset template;
        
        public bool HideLabel { get; set; }
        public string Label { get; set; } = "";
        public string TemplateName { get; set; } = DefaultTemplateName;
        public string ItemTemplateName { get; set; } = DefaultItemTemplateName;
        public string StyleSheetName { get; set; } = DefaultStyleSheetName;

        public bool IsInitialised { get; private set; }
        public event Action OnReset;


        public ListElement() : this(null, null)
        {
        }

        public ListElement(SerializedProperty property) : this(property, string.Empty)
        {
        }

        public ListElement(SerializedProperty property, string label)
        {
            if (property is null)
            {
                return;
            }
            
            this.BindProperty(property);
            Initialise();
        }

      
        private void Initialise()
        {
            // LoadAssets();
            // CloneTemplate();
            IsInitialised = true;
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            Reset();
            // Don't allow the binding of `this` to continue because `this` is not
            // the actually bound field, it is just a container.
            evt.StopPropagation();
        }

        private void Reset()
        {
            OnReset?.Invoke();
        }

        public new class UxmlFactory : UxmlFactory<ListElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
/*            private UxmlBoolAttributeDescription hideLabel;
            private UxmlStringAttributeDescription itemTemplateName;
            private UxmlStringAttributeDescription label;
            private UxmlStringAttributeDescription styleSheetName;
            private UxmlStringAttributeDescription templateName;*/

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is ListElement le)
                {
                    le.Initialise();
                }
            }
        }
    }
}