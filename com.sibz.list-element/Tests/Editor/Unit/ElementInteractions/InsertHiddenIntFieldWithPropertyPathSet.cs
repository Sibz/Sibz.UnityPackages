using System;
using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.ElementInteractionsTests
{
    public class InsertHiddenIntFieldWithPropertyPathSet
    {
        [Test]
        public void ShouldAddIntField()
        {
            VisualElement root = new VisualElement();
            ElementInteractions.InsertHiddenIntFieldWithPropertyPathSet(root, "");
            Assert.IsNotNull(root.Q<IntegerField>());
        }

        [Test]
        public void ShouldSetPath()
        {
            VisualElement root = new VisualElement();
            ElementInteractions.InsertHiddenIntFieldWithPropertyPathSet(root, "test");
            Assert.AreEqual("test", root.Q<IntegerField>().bindingPath);
        }

        [Test]
        public void ShouldNotBeVisible()
        {
            VisualElement root = new VisualElement();
            ElementInteractions.InsertHiddenIntFieldWithPropertyPathSet(root, "test");
            Assert.AreEqual(DisplayStyle.None, root.Q<IntegerField>().style.display.value);
        }

        [Test]
        public void WhenElementIsNull_ShouldThrowArgumentNullException()
        {
            try
            {
                ElementInteractions.InsertHiddenIntFieldWithPropertyPathSet(null, "test");
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("element", e.ParamName);
                return;
            }

            Assert.Fail($"{nameof(ArgumentNullException)} was not thrown");
        }
    }
}