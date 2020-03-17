using NUnit.Framework;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

// ReSharper disable HeapView.BoxingAllocation

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
{
    public class SetConfirmSectionVisibility
    {
        private class ElementPair
        {
            public readonly Button Button;
            public readonly VisualElement Section;
            
            public ElementPair(bool sectionShown = false)
            {
                Button = new Button();
                Section = new VisualElement();

                Button.style.display = sectionShown ?  DisplayStyle.None : DisplayStyle.Flex;
                Section.style.display = sectionShown ?  DisplayStyle.Flex :DisplayStyle.None;
            }
            
            public void AssertDisplayState(bool sectionShown = false)
            {
                Assert.AreEqual(sectionShown ?  DisplayStyle.None : DisplayStyle.Flex,
                    Button.style.display.value);

                Assert.AreEqual(sectionShown ?  DisplayStyle.Flex :DisplayStyle.None,
                    Section.style.display.value);
            }
        }
        
        [Test]
        public void WhenElementsAreNull_ShouldFailSilently()
        {
            Handler.SetConfirmSectionVisibility(null, null, false);
        }
        
        [Test]
        public void WhenOneElementIsNull_ShouldFailSilentlyAndNotChangeElement()
        {
            ElementPair elements = new ElementPair();

            Handler.SetConfirmSectionVisibility(elements.Button, null, true);
            Handler.SetConfirmSectionVisibility(null, elements.Section, true);

            elements.AssertDisplayState();
        }

        [Test]
        public void WhenSettingToTheSameState_ShouldNotChangeElements()
        {
            ElementPair elements = new ElementPair();

            Handler.SetConfirmSectionVisibility(elements.Button, elements.Section, false);

            elements.AssertDisplayState();
        }

        [Test]
        public void WhenShowIsTrue_ShouldChangeElements()
        {
            ElementPair elements = new ElementPair();

            Handler.SetConfirmSectionVisibility(elements.Button, elements.Section, true);

            elements.AssertDisplayState(true);
        }

        [Test]
        public void WhenShowIsFalse_ShouldChangeElements()
        {
            ElementPair elements = new ElementPair(true);

            Handler.SetConfirmSectionVisibility(elements.Button, elements.Section, false);

            elements.AssertDisplayState();
        }
    }
}