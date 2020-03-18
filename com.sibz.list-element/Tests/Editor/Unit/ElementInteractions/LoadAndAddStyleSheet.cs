using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
{
    public class LoadAndAddStyleSheet
    {
        [Test]
        public void WhenNameIsNotEqualToTemplateName_ShouldAddStylesheet()
        {
            VisualElement element = new VisualElement();
            Handler.LoadAndAddStyleSheet(element, TestHelpers.DefaultTestTemplateName, ListOptions.DefaultTemplateName);
            Assert.AreEqual(1, element.styleSheets.count);
        }

        [Test]
        public void WhenNameIsEqualToTemplateName_ShouldNotAddStyleSheet()
        {
            VisualElement element = new VisualElement();
            Handler.LoadAndAddStyleSheet(element, ListOptions.DefaultStyleSheetName, ListOptions.DefaultTemplateName);
            Assert.AreEqual(0, element.styleSheets.count);
        }

        [Test]
        public void WhenStylesheetDoesNotExist_ShouldShowWarning()
        {
            VisualElement element = new VisualElement();
            Handler.LoadAndAddStyleSheet(element, "TEST43325416436231", ListOptions.DefaultTemplateName);

            LogAssert.Expect(LogType.Warning, new Regex(".*\n*.*\n*.*\n*.*\n*.*\n*.*\n*.*\n*.*\n*.*"));
        }
    }
}