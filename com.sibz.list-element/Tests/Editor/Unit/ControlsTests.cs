using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit
{
    public class ControlsTests
    {
        private Controls controls;
        private Controls controlsForObjectList;
        private GameObject gameObject;
        private ListElementOptions options;
        private VisualElement element;
        private VisualElement elementForObjectList;

        private class IsElementWithClassName<T> : Constraint
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
            gameObject = Object.Instantiate(new GameObject());
            gameObject.AddComponent<ControlsMono>();
        }
        
        [SetUp]
        public void ControlSetup()
        {
            options = new ListElementOptions();
            element = new VisualElement();
            elementForObjectList = new VisualElement();
            SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>(options.TemplateName).CloneTree(element);
            SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>(options.TemplateName).CloneTree(elementForObjectList);
            controls = new Controls(element, options);
            controlsForObjectList = new Controls(elementForObjectList, options);
            SerializedObject serializedObject = new SerializedObject(gameObject.GetComponent<ControlsMono>());
            AddItemRows(element, serializedObject);
            AddItemRows(elementForObjectList, serializedObject);
        }

        private void AddItemRows(VisualElement root, SerializedObject serializedObject)
        {
            SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(ControlsMono.stringList));
            RowGenerator rowGenerator = new RowGenerator(options.ItemTemplateName);
            for (int i = 0; i < gameObject.GetComponent<ControlsMono>().stringList.Count; i++)
            {
                root.Q<VisualElement>(null, options.ItemsSectionClassName).Add(
                    rowGenerator.NewRow(i, serializedProperty)
                );
            }
        }

        [System.Serializable]
        public class ControlsMono : MonoBehaviour
        {
            public List<string> stringList = new List<string> { "1", "2", "3"};
        }

        [Test]
        public void ShouldGetHeaderLabel()
        {
            Assert.That(
                controls.HeaderLabel,
                new IsElementWithClassName<Label>(options.HeaderLabelClassName));
        }

        [Test]
        public void ShouldGetAddButton()
        {
            Assert.That(
                controls.Add,
                new IsElementWithClassName<Button>(options.AddItemButtonClassName));
        }

        [Test]
        public void ShouldGetClearListButton()
        {
            Assert.That(
                controls.ClearList,
                new IsElementWithClassName<Button>(options.ClearListButtonClassName));
        }

        [Test]
        public void ShouldGetClearListConfirmButton()
        {
            Assert.That(
                controls.ClearListConfirm,
                new IsElementWithClassName<Button>(options.ClearListConfirmButtonClassName));
        }

        [Test]
        public void ShouldGetClearListCancelButton()
        {
            Assert.That(
                controls.ClearListCancel,
                new IsElementWithClassName<Button>(options.ClearListCancelButtonClassName));
        }

        [Test]
        public void ShouldGetAddObjectField()
        {
            Assert.That(
                controls.AddObjectField,
                new IsElementWithClassName<ObjectField>(options.AddItemObjectFieldClassName));
        }

        [Test]
        public void ShouldGetAddObjectFieldLabel()
        {
            Assert.IsNotNull(controls.AddObjectFieldLabel);
            Assert.IsTrue(
                controls.AddObjectFieldLabel.GetFirstAncestorOfType<ObjectField>()
                    .ClassListContains(options.AddItemObjectFieldClassName));
        }

        [Test]
        public void ShouldGetHeaderSection()
        {
            Assert.That(
                controls.HeaderSection,
                new IsElementWithClassName<VisualElement>(options.HeaderSectionClassName));
        }

        [Test]
        public void ShouldGetClearListConfirmSection()
        {
            Assert.That(
                controls.ClearListConfirmSection,
                new IsElementWithClassName<VisualElement>(options.ClearListConfirmSectionClassName));
        }

        [Test]
        public void ShouldGetAddSection()
        {
            Assert.That(
                controls.AddSection,
                new IsElementWithClassName<VisualElement>(options.AddItemSectionClassName));
        }

        [Test]
        public void ShouldGetItemSection()
        {
            Assert.That(
                controls.ItemsSection,
                new IsElementWithClassName<VisualElement>(options.ItemsSectionClassName));
        }

        [Test]
        public void ShouldGetRowMoveUpButton()
        {
            Assert.That(
                controls.Row[0].MoveUp,
                new IsElementWithClassName<Button>(options.MoveItemUpButtonClassName));
        }

        [Test]
        public void ShouldGetRowMoveDownButton()
        {
            Assert.That(
                controls.Row[0].MoveDown,
                new IsElementWithClassName<Button>(options.MoveItemDownButtonClassName));
        }

        [Test]
        public void ShouldGetRowRemoveButton()
        {
            Assert.That(
                controls.Row[0].RemoveItem,
                new IsElementWithClassName<Button>(options.RemoveItemButtonClassName));
        }

        [Test]
        public void ShouldGetRowPropertyField()
        {
            Assert.IsNotNull(controls.Row[0].PropertyField);
            Assert.IsNotNull(
                controls.Row[0].PropertyField.GetFirstAncestorOfType<ListRowElement>());
        }

        [Test]
        public void ShouldGetRowPropertyFieldLabel()
        {
            Assert.IsNotNull(controls.Row[0].PropertyFieldLabel);
            Assert.IsNotNull(
                controls.Row[0].PropertyFieldLabel.GetFirstAncestorOfType<ListRowElement>());
        }
        
        [Test]
        public void ShouldGetRowPropertyFieldLabelForObjectList()
        {
            Assert.IsNotNull(controlsForObjectList.Row[0].PropertyFieldLabel);
            Assert.IsNull(
                controlsForObjectList.Row[0].PropertyFieldLabel.GetFirstAncestorOfType<ObjectField>());
            Assert.IsNotNull(
                controlsForObjectList.Row[0].PropertyFieldLabel.GetFirstAncestorOfType<ListRowElement>());
        }

        [Test]
        public void RowFieldsBelongToCorrectRow([Values(0, 1, 2)] int row)
        {
            Assert.IsTrue(new[]
            {
                controls.Row[row].MoveUp.GetFirstAncestorOfType<ListRowElement>().Index,
                controls.Row[row].MoveDown.GetFirstAncestorOfType<ListRowElement>().Index,
                controls.Row[row].RemoveItem.GetFirstAncestorOfType<ListRowElement>().Index,
                controls.Row[row].PropertyField.GetFirstAncestorOfType<ListRowElement>().Index,
                controls.Row[row].PropertyFieldLabel.GetFirstAncestorOfType<ListRowElement>().Index
            }.All(x => x == row));
        }
    }
}