using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    public class ListVisualElement : BindableElement
    {
        public string Label { get; set; }
        public bool ShowSize { get; set; }
        public bool DisableLabelContextMenu { get; set; }
        public bool DisablePropertyLabel { get; set; }

        private Label LabelElement = new Label();
        public static readonly string ussClassName = "sibz-list-field";

        private SerializedObject m_SO;

        public VisualElement ListContents;

        public new class UxmlFactory : UxmlFactory<ListVisualElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_PropertyPath;
            UxmlStringAttributeDescription m_Label;
            UxmlBoolAttributeDescription m_DisableLabelContextMenu;
            UxmlBoolAttributeDescription m_DisablePropertyLabel;
            //UxmlBoolAttributeDescription m_ShowSize;

            public UxmlTraits()
            {
                m_PropertyPath = new UxmlStringAttributeDescription { name = "binding-path" };
                m_Label = new UxmlStringAttributeDescription { name = "label" };
                m_DisableLabelContextMenu = new UxmlBoolAttributeDescription { name = "disable-label-context-menu" };
                m_DisablePropertyLabel = new UxmlBoolAttributeDescription { name = "disable-property-label" };
                //m_ShowSize = new UxmlBoolAttributeDescription { name = "show-size" };
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (!(ve is ListVisualElement field))
                {
                    return;
                }

                string propPath = m_PropertyPath.GetValueFromBag(bag, cc);

                if (!string.IsNullOrEmpty(propPath))
                {
                    field.bindingPath = propPath;
                }

                string label = m_Label.GetValueFromBag(bag, cc);

                field.DisableLabelContextMenu = m_DisableLabelContextMenu.GetValueFromBag(bag, cc);
                field.DisablePropertyLabel = m_DisablePropertyLabel.GetValueFromBag(bag, cc);
                //field.ShowSize = m_ShowSize.GetValueFromBag(bag, cc);
                field.Label = label;
                field.LabelElement.text = label;
                if (string.IsNullOrEmpty(label))
                {
                    field.LabelElement.style.display = DisplayStyle.None;
                }
                else
                {
                    field.LabelElement.style.display = DisplayStyle.Flex;
                }
            }
        }

        public override VisualElement contentContainer => ListContents ?? base.contentContainer;

        public ListVisualElement() : this(null, string.Empty) { }
        public ListVisualElement(SerializedProperty property) : this(property, string.Empty) { }
        public ListVisualElement(SerializedProperty property, string label)
        {
            AddToClassList(ussClassName);

            Add(LabelElement);

            VisualElement listContents = new VisualElement { name = "list-contents" };
            Add(listContents);
            ListContents = listContents;

            if (property == null)
            {
                return;
            }

            m_SO = property.serializedObject;

            bindingPath = property.propertyPath;
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            System.Type type = evt.GetType();
            if (type.Name == "SerializedPropertyBindEvent"
                &&
                type.GetProperty("bindProperty").GetValue(evt) is SerializedProperty property)
            {


                Reset(property);

                m_SO = property.serializedObject;

                // Don't allow the binding of `this` to continue because `this` is not
                // the actually bound field, it is just a container.
                evt.StopPropagation();
            }
        }

        private void Reset(SerializedProperty prop)
        {
            ListContents.Clear();
            if (prop.isArray)
            {

                var endProperty = prop.GetEndProperty();

                prop.NextVisible(true);
                do
                {

                    if (SerializedProperty.EqualContents(prop, endProperty))
                    {
                        break;
                    }

                    switch (prop.propertyType)
                    {
                        case SerializedPropertyType.ArraySize:
                            var field = new IntegerField { bindingPath = prop.propertyPath };
                            field.SetValueWithoutNotify(prop.intValue); // This avoids the OnValueChanged/Rebind feedback loop.
                            field.style.display = ShowSize ? DisplayStyle.Flex : DisplayStyle.None;
                            field.RegisterValueChangedCallback(UpdateList);
                            field.label = "Size";
                            ListContents.Add(field);
                            break;

                        default:
                            var f = new PropertyField(prop);
                            ListContents.Add(f);
                            if (DisablePropertyLabel)
                            {
                                f.RegisterCallback<AttachToPanelEvent>((e) =>
                                    {

                                        if (f.Q<Label>() is Label)
                                        {
                                            f.Q<Label>().style.display = DisplayStyle.None;
                                        }
                                        //Debug.Log(f.childCount);
                                    });
                            }

                            if (!DisablePropertyLabel && DisableLabelContextMenu)
                            {
                                f.RegisterCallback<MouseUpEvent>((e) =>
                                    {
                                        //Debug.Log(((Label)e.target).parent?.parent.GetType());
                                        if (e.target is Label && ((Label)e.target).parent?.parent == f)
                                        {
                                            e.StopPropagation();
                                        }
                                    }, TrickleDown.TrickleDown);
                            }

                            break;
                    }

                } while (prop.NextVisible(false));

                prop.Reset();

            }
            else
            {
                ListContents.Add(new Label("Error, Bound item is not a list or array"));
            }
        }

        private void UpdateList(ChangeEvent<int> changeEvent)
        {
            this.Unbind();
            m_SO.UpdateIfRequiredOrScript();
            m_SO.ApplyModifiedProperties();
            this.Bind(m_SO);
            changeEvent.StopImmediatePropagation();
        }
    }
}