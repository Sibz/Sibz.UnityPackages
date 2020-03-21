using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.ElementInteractionsTests
{
    public class SetAddSectionVisibility
    {
        [Test]
        public void WhenFalse_ShouldAddClass()
        {
            VisualElement listElement = new VisualElement();
            ElementInteractions.SetAddSectionVisibility(listElement, false);
            Assert.IsTrue(listElement.ClassListContains(UxmlClassNames.HideAddSection));
        }

        [Test]
        public void WhenTrue_ShouldNotAddClass()
        {
            VisualElement listElement = new VisualElement();
            ElementInteractions.SetAddSectionVisibility(listElement, true);
            Assert.IsFalse(listElement.ClassListContains(UxmlClassNames.HideAddSection));
        }

        [Test]
        public void WhenElementIsNull_ShouldNotThrowError()
        {
            ElementInteractions.SetAddSectionVisibility(null, false);
        }
    }
}