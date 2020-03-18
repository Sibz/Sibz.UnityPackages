using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
{
    public class SetAddFieldVisibility
    {
        [Test]
        public void WhenTypeIsObject_ShouldAddClass()
        {
            VisualElement itemSection = new VisualElement();
            Handler.SetAddFieldVisibility(itemSection, typeof(TestHelpers.TestObject), false);
            Assert.IsTrue(itemSection.ClassListContains(UxmlClassNames.UseObjectField));
        }
        
        [Test]
        public void WhenTypeIsObjectAndOptionSet_ShouldNotAddClass()
        {
            VisualElement itemSection = new VisualElement();
            Handler.SetAddFieldVisibility(itemSection, typeof(TestHelpers.TestObject), true);
            Assert.IsFalse(itemSection.ClassListContains(UxmlClassNames.UseObjectField));
        }
        
        [Test]
        public void WhenTypeIsString_ShouldNotAddClass()
        {
            VisualElement itemSection = new VisualElement();
            Handler.SetAddFieldVisibility(itemSection, typeof(string), false);
            Assert.IsFalse(itemSection.ClassListContains(UxmlClassNames.UseObjectField));
        }
          
        [Test]
        public void WhenElementIsNull_ShouldNotThrowError()
        {
            Handler.SetAddFieldVisibility(null, typeof(TestHelpers.TestObject), false);
        }
    }
}