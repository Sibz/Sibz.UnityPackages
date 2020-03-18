using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
{
    public class SetReorderButtonVisibility
    {
        [Test]
        public void WhenOptionTrue_ShouldNotAddClass()
        {
            VisualElement itemSection = new VisualElement();
            Handler.SetReorderButtonVisibility(itemSection, true);
            Assert.IsFalse(itemSection.ClassListContains(UxmlClassNames.HideReorderButtons));
        }

        [Test]
        public void WhenOptionFalse_ShouldAddClass()
        {
            VisualElement itemSection = new VisualElement();
            Handler.SetReorderButtonVisibility(itemSection, false);
            Assert.IsTrue(itemSection.ClassListContains(UxmlClassNames.HideReorderButtons));
        }

        [Test]
        public void WhenElementIsNull_ShouldFailSilently()
        {
            Handler.SetReorderButtonVisibility(null, false);
        }
    }
}