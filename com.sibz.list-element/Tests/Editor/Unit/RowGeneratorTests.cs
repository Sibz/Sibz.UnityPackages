using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using RowGen = Sibz.ListElement.RowGenerator;

namespace Sibz.ListElement.Tests.Unit.RowGenerator
{
    public class NewRow
    {
        private RowGen rowGen;
        private SerializedProperty property;
        private SerializedObject testSerializedGameObject;

        [SetUp]
        public void TestSetup()
        {
            testSerializedGameObject =
                new SerializedObject(new GameObject().AddComponent<TestHelpers.TestComponent>());
            property = testSerializedGameObject.FindProperty(nameof(TestHelpers.TestComponent.myList));
            rowGen = new RowGen(new ListElementOptions().ItemTemplateName);
        }

        [TearDown]
        public void TearDown()
        {
            testSerializedGameObject = null;
            property = null;
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
    }
}