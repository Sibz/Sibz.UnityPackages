using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit
{
    public class RowGeneratorTests
    {
        private RowGenerator rowGen;
        private SerializedProperty property;
        private GameObject testGameObject;
        private SerializedObject testSerializedGameObject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testGameObject = Object.Instantiate(new GameObject());
        }

        [SetUp]
        public void TestSetup()
        {
            testGameObject.AddComponent<TestHelpers.TestComponent>();
            testSerializedGameObject =
                new SerializedObject(testGameObject.GetComponent<TestHelpers.TestComponent>());
            property = testSerializedGameObject.FindProperty(nameof(TestHelpers.TestComponent.myList));
            rowGen = new RowGenerator(new ListElementOptions().ItemTemplateName);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testGameObject.GetComponent<TestHelpers.TestComponent>());
            testSerializedGameObject = null;
            property = null;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Object.DestroyImmediate(testGameObject);
        }

        [Test]
        public void ShouldContainElements()
        {
            Assert.Less(0, rowGen.NewRow(0, property).childCount);
        }

        [Test]
        public void ShouldContainPropertyField()
        {
            Assert.IsNotNull(
                rowGen.NewRow(0, property)
                    .Q<PropertyField>());
        }

        [Test]
        public void ShouldHaveSetPropertyFieldsPropertyPath()
        {
            Assert.IsFalse(
                string.IsNullOrEmpty(
                    rowGen.NewRow(0, property)
                        .Q<PropertyField>().bindingPath));
        }

        [Test]
        public void ShouldDisableOnlyFirstMoveUpButton([Values(0, 1, 2)] int row)
        {
            Button moveUp = new Button();
            RowGenerator.AdjustReorderButtonsState(moveUp, null, row, 3);
            if (row == 0)
            {
                Assert.IsFalse(moveUp.enabledSelf);
            }
            else
            {
                Assert.IsTrue(moveUp.enabledSelf);
            }
        }

        [Test]
        public void ShouldDisableOnlyLastMoveDownButton([Values(0, 1)] int row)
        {
            Button moveDown = new Button();
            RowGenerator.AdjustReorderButtonsState(null, moveDown, row, 3);
            if (row == 2)
            {
                Assert.IsFalse(moveDown.enabledSelf);
            }
            else
            {
                Assert.IsTrue(moveDown.enabledSelf);
            }
        }
    }
}