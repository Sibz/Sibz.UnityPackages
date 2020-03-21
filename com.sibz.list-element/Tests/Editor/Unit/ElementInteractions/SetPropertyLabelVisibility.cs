using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractionsTests
{
    public class SetPropertyLabelVisibility
    {
        [Test]
        public void WhenSettingPropertyLabelVisibilityWithOptionFalse_ShouldNotRemoveClass()
        {
            VisualElement itemSection = new VisualElement();
            itemSection.AddToClassList(UxmlClassNames.HidePropertyLabel);
            Handler.SetPropertyLabelVisibility(itemSection, false);
            Assert.IsTrue(itemSection.ClassListContains(UxmlClassNames.HidePropertyLabel));
        }

        [Test]
        public void WhenSettingPropertyLabelVisibilityWithOptionTrue_ShouldRemoveClass()
        {
            VisualElement itemSection = new VisualElement();
            itemSection.AddToClassList(UxmlClassNames.HidePropertyLabel);
            Handler.SetPropertyLabelVisibility(itemSection, true);
            Assert.IsFalse(itemSection.ClassListContains(UxmlClassNames.HidePropertyLabel));
        }

        [Test]
        public void WhenElementNull_ShouldNotThrowError()
        {
            Handler.SetPropertyLabelVisibility(null, true);
        }
    }
}