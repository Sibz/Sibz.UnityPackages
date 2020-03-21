using System;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;

namespace Sibz.ListElement.Tests.Unit.ListElementTests
{
    public class Constructor
    {
        private ListElement listElement;
        private SerializedProperty property;

        [SetUp]
        public void SetUp()
        {
            property = TestHelpers.GetProperty();
        }

        [Test]
        public void WhenNoParams_ShouldNotInitialise()
        {
            listElement = new ListElement();
            Assert.IsFalse(listElement.IsInitialised);
        }

        [Test]
        public void WhenNoParams_ShouldNotBeEmpty()
        {
            listElement = new ListElement();
            Assert.Greater(listElement.childCount, 0);
        }

        [Test]
        public void WhenEmptySetTrue_ShouldBeEmpty()
        {
            listElement = new ListElement(true);
            Assert.AreEqual(listElement.childCount, 0);
        }

        [Test]
        public void WhenEmptySetFalse_ShouldNotBeEmpty()
        {
            listElement = new ListElement(false);
            Assert.Greater(listElement.childCount, 0);
        }

        [Test]
        public void WhenLabelSet_ShouldSetOption()
        {
            const string testString = "TEST123$$$";
            listElement = new ListElement(property, testString);
            Assert.AreEqual(testString, listElement.Options.Label);
        }

        [Test]
        public void WhenPropertyIsNull_ShouldThrowArgumentNullException()
        {
            try
            {
                listElement = new ListElement(null);
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("property", e.ParamName);
                return;
            }

            Assert.Fail($"{nameof(ArgumentNullException)} not thrown");
        }

        [Test]
        public void WhenOptionsIsNull_ShouldNotThrowError()
        {
            ListOptions options = null;
            listElement = new ListElement(property, options);
        }

        [Test]
        public void WhenPropertyIsSet_ShouldContainControls()
        {
            listElement = new ListElement(property);
            Assert.IsNotNull(listElement.Controls);
        }

        [Test]
        public void WhenPropertyIsSet_ShouldNotBeEmpty()
        {
            listElement = new ListElement(property);
            Assert.Greater(listElement.childCount, 0);
        }

        [Test]
        public void WhenPropertySet_ShouldBindProperty()
        {
            listElement = new ListElement(property);
            Assert.AreEqual(property.propertyPath, listElement.bindingPath);
        }

        [Test]
        public void WhenPropertyBoundAfter_ShouldInitialise()
        {
            listElement = new ListElement();
            listElement.BindProperty(property);
            Assert.IsTrue(listElement.IsInitialised);
        }
    }
}