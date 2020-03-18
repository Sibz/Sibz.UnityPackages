using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
{
    public class SetRemoveButtonVisibility
    {
        [Test]
        public void WhenOptionTrue_ShouldNotAddClass()
        {
            VisualElement listElement = new VisualElement();
            Handler.SetRemoveButtonVisibility(listElement, true);
            Assert.IsFalse(listElement.ClassListContains(UxmlClassNames.HideRemoveButtons));
        }

        [Test]
        public void WhenOptionFalse_ShouldAddClass()
        {
            VisualElement listElement = new VisualElement();
            Handler.SetRemoveButtonVisibility(listElement, false);
            Assert.IsTrue(listElement.ClassListContains(UxmlClassNames.HideRemoveButtons));
        }

        [Test]
        public void WhenElementIsNull_ShouldFailSilently()
        {
            Handler.SetRemoveButtonVisibility(null, false);
        }
    }
}