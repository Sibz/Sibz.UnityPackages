using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using RowGen = Sibz.ListElement.RowGenerator;

namespace Sibz.ListElement.Tests.Unit.RowGenerator
{
    public class NewRow
    {
        private RowGen rowGen;
        private SerializedProperty property;

        [SetUp]
        public void TestSetup()
        {
            property = TestHelpers.GetProperty();
            rowGen = new RowGen(new ListOptions().ItemTemplateName);
        }

        [TearDown]
        public void TearDown()
        {
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