using NUnit.Framework;
using NUnit.Framework.Constraints;
using Sibz.ListElement.Internal;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.Controls
{
    public class ControlsTestBase
    {
        protected Internal.Controls Controls;
        protected Internal.Controls ControlsForObjectList;
        protected ListOptions Options;
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
            Options = new ListOptions();
            template = SingleAssetLoader.Load<VisualTreeAsset>(Options.TemplateName);
        }

        [SetUp]
        public void ControlSetup()
        {
            element = new VisualElement();
            elementForObjectList = new VisualElement();
            template.CloneTree(element);
            template.CloneTree(elementForObjectList);
            Controls = new Internal.Controls(element);
            ControlsForObjectList = new Internal.Controls(elementForObjectList);

            AddItemRows(element, TestHelpers.GetProperty());
            AddItemRows(elementForObjectList, TestHelpers.GetProperty(nameof(TestHelpers.TestComponent.myCustomList)));
        }

        private void AddItemRows(VisualElement root, SerializedProperty property)
        {
            Sibz.ListElement.RowGenerator rowGenerator = new Sibz.ListElement.RowGenerator(Options.ItemTemplateName);
            for (int i = 0; i < property.arraySize; i++)
            {
                root.Q<VisualElement>(null, UxmlClassNames.ItemsSectionClassName).Add(
                    rowGenerator.NewRow(i, property)
                );
            }
        }
    }
}