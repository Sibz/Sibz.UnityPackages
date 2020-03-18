using NUnit.Framework;
using Sibz.ListElement.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Integration.ListElementTests
{
    public class Initialise
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
        public void ShouldSendListResetEvent()
        {
            listElement = new ListElement(true);
            TestWindow.rootVisualElement.Add(listElement);

            listElement.RegisterCallback<ListResetEvent>((e) => Assert.Pass());
            listElement.BindProperty(property);

            Assert.Fail("ListResetEvent Callback Not Called");
        }

        [Test]
        public void ShouldCreateControls()
        {
            listElement = new ListElement(property);
                        
            Assert.IsNotNull(listElement.Controls);
        }
        
        [Test]
        public void ShouldClearTheControlContents()
        {
            VisualElement testElement = new VisualElement();
            listElement = new ListElement(true);
            listElement.Add(testElement);
            listElement.BindProperty(property);
            
            Assert.IsFalse(listElement.Contains(testElement));
        }
        
        [Test]
        public void ShouldHaveContents()
        {
            listElement = new ListElement(true);
            TestWindow.rootVisualElement.Add(listElement);
            listElement.BindProperty(property);
            
            Assert.Greater(listElement.childCount, 0);
        }
        
        [Test]
        public void ShouldResetContents()
        {
            listElement = new ListElement(true);
            TestWindow.rootVisualElement.Add(listElement);
            listElement.BindProperty(property);
            Assert.Greater(listElement.Controls.ItemsSection.childCount, 0);
        }
    }
}