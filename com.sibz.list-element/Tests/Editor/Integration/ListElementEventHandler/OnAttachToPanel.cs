using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;

namespace Sibz.ListElement.Tests.Integration.ListElementEventHandlerTests
{
    public class OnAttachToPanel
    {
        private WindowFixture.TestWindow TestWindow => WindowFixture.Window;
        private ListElement listElement;
        private SerializedProperty property;

        [SetUp]
        public void SetUp()
        {
            property = TestHelpers.GetProperty();
        }

        [TearDown]
        public void TearDown()
        {
            if (TestWindow.rootVisualElement.Contains(listElement))
            {
                TestWindow.rootVisualElement.Remove(listElement);
            }
        }

        [Test]
        public void ShouldReset()
        {
            listElement = new ListElement(true);

            listElement.BindProperty(property);

            TestWindow.rootVisualElement.Add(listElement);

            Assert.Greater(listElement.Controls.ItemsSection.childCount, 0);
        }
    }
}