using NUnit.Framework;
using UnityEditor;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Events.ListElementEventHandler;

namespace Sibz.ListElement.Tests.Unit.ListElementEventHandler
{
    public class PopulateList
    {
        [Test]
        public void ShouldClearExistingItems()
        {
            ListElement listElement = new ListElement(TestHelpers.GetProperty());
            VisualElement testElement = new VisualElement();
            listElement.Controls.ItemsSection.Add(testElement);
            Handler.PopulateList(listElement);
            Assert.IsFalse(listElement.Controls.ItemsSection.Contains(testElement));
        }

        [Test]
        public void ShouldPopulateCorrectNumberOfItems()
        {
            SerializedProperty property = TestHelpers.GetProperty();
            ListElement listElement = new ListElement(property);
            Handler.PopulateList(listElement);
            Assert.AreEqual(property.arraySize, listElement.Controls.ItemsSection.childCount);
        }
    }
}