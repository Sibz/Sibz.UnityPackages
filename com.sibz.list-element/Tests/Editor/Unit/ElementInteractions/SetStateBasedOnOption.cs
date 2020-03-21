using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.ElementInteractionsTests
{
    public class SetStateBasedOnOption
    {
        [Test]
        public void WhenTrue_ShouldSetEnabledTrue()
        {
            VisualElement element = new VisualElement();
            ElementInteractions.SetStateBasedOnOption(element, true);

            Assert.IsTrue(element.enabledSelf);
        }

        [Test]
        public void WhenFalse_ShouldSetEnabledFalse()
        {
            VisualElement element = new VisualElement();
            ElementInteractions.SetStateBasedOnOption(element, false);

            Assert.IsFalse(element.enabledSelf);
        }

        [Test]
        public void WhenButtonIsNull_ShouldNotThrowError()
        {
            ElementInteractions.SetStateBasedOnOption(null, true);
        }
    }
}