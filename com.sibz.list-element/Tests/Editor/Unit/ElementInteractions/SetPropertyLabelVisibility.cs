using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
{
    public class SetPropertyLabelVisibility
    {
        [Test]
        public void WhenSettingPropertyLabelVisibilityWithOptionTrue_ShouldNotRemoveClass()
        {
            VisualElement itemSection = new VisualElement();
            itemSection.AddToClassList(UxmlClassNames.HidePropertyLabel);
            Handler.SetPropertyLabelVisibility(itemSection, true);
            Assert.IsTrue(itemSection.ClassListContains(UxmlClassNames.HidePropertyLabel));
        }    
        
        [Test]
        public void WhenSettingPropertyLabelVisibilityWithOptionFalse_ShouldRemoveClass()
        {
            VisualElement itemSection = new VisualElement();
            itemSection.AddToClassList(UxmlClassNames.HidePropertyLabel);
            Handler.SetPropertyLabelVisibility(itemSection, false);
            Assert.IsFalse(itemSection.ClassListContains(UxmlClassNames.HidePropertyLabel));
        }
    }
}