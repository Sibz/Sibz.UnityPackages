using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.ElementInteractionsTests
{
    public class SetDefaultStyle
    {
        [Test]
        public void WhenElementIsNull_ShouldNotThrowError()
        {
            ElementInteractions.SetDefaultStyle(null);
        }

        [Test]
        public void ShouldAddClassToList()
        {
            VisualElement ve = new VisualElement();
            ElementInteractions.SetDefaultStyle(ve);
            Assert.IsTrue(ve.ClassListContains(UxmlClassNames.DefaultStyle));
        }
    }
}