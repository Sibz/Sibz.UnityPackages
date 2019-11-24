using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    public class ListVisualElement : BindableElement
    {
        public string Label { get; set; }

        private Label LabelElement = new Label();
        public static readonly string ussClassName = "sibz-list-field";

        private SerializedObject m_SO;

        public VisualElement ListContents;

        public new class UxmlFactory : UxmlFactory<ListVisualElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_PropertyPath;
            UxmlStringAttributeDescription m_Label;

            public UxmlTraits()
            {
                m_PropertyPath = new UxmlStringAttributeDescription { name = "binding-path" };
                m_Label = new UxmlStringAttributeDescription { name = "label" };
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
                            field.style.display = DisplayStyle.None;
                            field.RegisterValueChangedCallback(UpdateList);
                            ListContents.Add(field);
                            break;

                        default:
                            ListContents.Add(new PropertyField(prop));
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