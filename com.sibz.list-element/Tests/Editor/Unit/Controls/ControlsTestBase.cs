using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.Controls
{
    public class ControlsTestBase
    {
        protected Internal.Controls Controls;
        protected Internal.Controls ControlsForObjectList;
        protected ListElementOptions Options;
        private VisualElement element;
        private VisualElement elementForObjectList;
        private VisualTreeAsset template;

        protected class IsElementWithClassName<T> : Constraint
            where T : VisualElement
        {
            private readonly string className;

            public IsElementWithClassName(string className)
            {
                this.className = className;
            }

            public override ConstraintResult ApplyTo(object actual)
            {
                if (!(actual is T element))
                {
                    Description = typeof(T).Name;
                    return new ConstraintResult(this, actual?.GetType().Name, ConstraintStatus.Failure);
                }

                if (element.ClassListContains(className))
                {
                    return new ConstraintResult(this, actual, ConstraintStatus.Success);
                }

                Description = $"Class name should contain '{className}'";
                return new ConstraintResult(this, string.Join(" ", element.GetClasses()),
                    ConstraintStatus.Failure);
            }

            public override string Description { get; protected set; }
        }

        [OneTimeSetUp]
        public void OneTime()
        {
            Options = new ListElementOptions();
            template = SingleAssetLoader.Load<VisualTreeAsset>(Options.TemplateName);
        }

        [SetUp]
        public void ControlSetup()
        {
            element = new VisualElement();
            elementForObjectList = new VisualElement();
            template.CloneTree(element);
            template.CloneTree(elementForObjectList);
            Controls = new Internal.Controls(element, Options);
            ControlsForObjectList = new Internal.Controls(elementForObjectList, Options);
            SerializedObject serializedObject =
                new SerializedObject(new GameObject().AddComponent<TestHelpers.TestComponent>());

            AddItemRows(element, serializedObject, nameof(TestHelpers.TestComponent.myList));
            AddItemRows(elementForObjectList, serializedObject, nameof(TestHelpers.TestComponent.myCustomList));
        }

        private void AddItemRows(VisualElement root, SerializedObject serializedObject, string propertyName)
        {
            SerializedProperty serializedProperty = serializedObject.FindProperty(propertyName);
            Sibz.ListElement.RowGenerator rowGenerator = new Sibz.ListElement.RowGenerator(Options.ItemTemplateName);
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                root.Q<VisualElement>(null, Options.ItemsSectionClassName).Add(
                    rowGenerator.NewRow(i, serializedProperty)
                );
            }
        }
    }
}