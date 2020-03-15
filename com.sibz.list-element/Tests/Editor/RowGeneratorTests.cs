using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests
{
    public class RowGeneratorTests
    {
        private ListElement listElement;
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
            testGameObject.AddComponent<ListElementTestBase.MyTestObject>();
            testSerializedGameObject =
                new SerializedObject(testGameObject.GetComponent<ListElementTestBase.MyTestObject>());
            property = testSerializedGameObject.FindProperty(nameof(ListElementTestBase.MyTestObject.myList));
            listElement = new ListElement(property);
            rowGen = new RowGenerator("TestItemTemplate");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testGameObject.GetComponent<ListElementTestBase.MyTestObject>());
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
        public void ShouldContainTestElement()
        {
            Assert.IsNotNull(
                rowGen.NewRow(0, property)
                    .Q("TestItemTemplateCheck"));
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
        public void ShouldDisableFirstMoveUpButton()
        {
            const int row = 0;
            rowGen.PostInsert(listElement.Controls.Row[row], row, 3);
            Assert.False(listElement.Controls.Row[row].MoveUp.enabledSelf);
        }

        [Test]
        public void ShouldDisableOnlyFirstMoveUpButton([Values(1, 2)] int row)
        {
            rowGen.PostInsert(listElement.Controls.Row[row], row, 3);
            Assert.IsTrue(listElement.Controls.Row[row].MoveUp.enabledSelf);
        }

        [Test]
        public void ShouldDisableLastMoveDownButton()
        {
            const int row = 2;
            rowGen.PostInsert(listElement.Controls.Row[row], row, 3);
            Assert.False(listElement.Controls.Row[row].MoveDown.enabledSelf);
        }

        [Test]
        public void ShouldDisableOnlyLastMoveDownButton([Values(0, 1)] int row)
        {
            rowGen.PostInsert(listElement.Controls.Row[row], row, 3);
            Assert.IsTrue(listElement.Controls.Row[row].MoveDown.enabledSelf);
        }
    }
}