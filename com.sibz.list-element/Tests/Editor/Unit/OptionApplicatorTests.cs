using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit
{
    [SuppressMessage("ReSharper", "ConvertToNullCoalescingCompoundAssignment")]
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class OptionApplicatorTests
    {
        private readonly Internal.ListElementOptions options = new Internal.ListElementOptions();
        
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
     
        [Test]
        public void WhenSettingDeletionButtonVisibilityWithOptionTrue_ShouldNotAddClass()
        {
            VisualElement listElement = new VisualElement();
            Handler.SetDeletionButtonVisibility(listElement, true);
            Assert.IsFalse(listElement.ClassListContains(UxmlClassNames.HideRemoveButtons));
        }    
        
        [Test]
        public void WhenSettingDeletionButtonVisibilityWithOptionFalse_ShouldAddClass()
        {
            VisualElement listElement = new VisualElement();
            Handler.SetDeletionButtonVisibility(listElement, false);
            Assert.IsTrue(listElement.ClassListContains(UxmlClassNames.HideRemoveButtons));
        }

        [Test]
        public void WhenSettingReorderButtonVisibilityWithOptionTrue_ShouldNotAddClass()
        {
            VisualElement itemSection = new VisualElement();
            Handler.SetReorderButtonVisibility(itemSection, true);
            Assert.IsFalse(itemSection.ClassListContains(UxmlClassNames.HideReorderButtons));
        }    
        
        [Test]
        public void WhenSettingReorderButtonVisibilityWithOptionFalse_ShouldAddClass()
        {
            VisualElement itemSection = new VisualElement();
            Handler.SetReorderButtonVisibility(itemSection, false);
            Assert.IsTrue(itemSection.ClassListContains(UxmlClassNames.HideReorderButtons));
        }

        [Test]
        public void WhenSettingAddObjectFieldVisibilityWithObjectAsType_ShouldAddClass()
        {
            VisualElement itemSection = new VisualElement();
            Handler.SetAddFieldVisibility(itemSection, typeof(TestHelpers.TestObject), false);
            Assert.IsTrue(itemSection.ClassListContains(UxmlClassNames.UseObjectField));
        }
        
        [Test]
        public void WhenSettingAddObjectFieldVisibilityWithObjectAsTypeAndOptionSet_ShouldNotAddClass()
        {
            VisualElement itemSection = new VisualElement();
            Handler.SetAddFieldVisibility(itemSection, typeof(TestHelpers.TestObject), true);
            Assert.IsFalse(itemSection.ClassListContains(UxmlClassNames.UseObjectField));
        }
        
        [Test]
        public void WhenSettingAddObjectFieldVisibilityWithStringsType_ShouldNotAddClass()
        {
            VisualElement itemSection = new VisualElement();
            Handler.SetAddFieldVisibility(itemSection, typeof(string), false);
            Assert.IsFalse(itemSection.ClassListContains(UxmlClassNames.UseObjectField));
        }
          
        [Test]
        public void WhenSettingAddObjectFieldVisibilityWithNullElement_ShouldNotThrowError()
        {
            Handler.SetAddFieldVisibility(null, typeof(string), false);
        }

        [Test]
        public void WhenReplacingObjectFieldLabelAndFieldIsNull_ShouldNotThrowError()
        {
            Handler.InsertLabelInObjectField(null, "Test");
        }
        
        [Test]
        public void WhenReplacingObjectFieldLabelAndFieldIsEmpty_ShouldNotThrowError()
        {
            ObjectField objectField = new ObjectField();
            objectField.hierarchy.RemoveAt(0);
            Handler.InsertLabelInObjectField(objectField, "Test");
        }
        
        [Test]
        public void WhenSettingObjectFieldLabel_ShouldHaveAdditionalLabelWithText()
        {
            ObjectField objectField = new ObjectField();
            Handler.InsertLabelInObjectField(objectField, "Test");
            Assert.AreEqual("Test",
                (objectField.hierarchy[0].hierarchy[0].hierarchy[2] as Label)?.text);
        }
        
        [Test]
        public void WhenSettingLabelWithOptionsLabelSet_ShouldUseOptionsLabel()
        {
            const string optionsLabel = "Test";
            Label label = new Label();
            Handler.SetHeaderLabelText(label, null, optionsLabel);
            Assert.AreEqual(
                optionsLabel,
                label.text);
        }

        [Test]
        public void WhenSettingLabelWithoutOptionsLabelSet_ShouldUseListName()
        {
            const string listName = "Test";
            Label label = new Label();
            Handler.SetHeaderLabelText(label, listName, null);
            Assert.AreEqual(
                listName,
                label.text);
        }

        [Test]
        public void WhenSettingLabelAndLabelIsNull_ShouldNotThrowError()
        {
            Handler.SetHeaderLabelText(null, null, null);
        }

        [Test]
        public void WhenCustomStyleSheetNameProvided_ShouldLoadAndAddStyleSheet()
        {
            VisualElement element = new VisualElement();
            Handler.LoadAndAddStyleSheet(element, "TestTemplate", options.TemplateName);
            Assert.AreEqual(1, element.styleSheets.count);
        }

        [Test]
        public void WhenStyleSheetNameIsEqualToTemplateName_ShouldNotAddStyleSheet()
        {
            VisualElement element = new VisualElement();
            Handler.LoadAndAddStyleSheet(element, options.StyleSheetName, options.TemplateName);
            Assert.AreEqual(0, element.styleSheets.count);
        }
        
        [Test]
        public void WhenCustomStyleSheetNameProvidedAndDoesNotExist_ShouldShowWarning()
        {
            VisualElement element = new VisualElement();
            Handler.LoadAndAddStyleSheet(element, "TEST43325416436231", options.TemplateName);
            
            LogAssert.Expect(LogType.Warning, new Regex(".*\n*.*\n*.*\n*.*\n*.*\n*.*\n*.*\n*.*\n*.*"));
        }

        [Test]
        public void WhenSettingTypeOnNull_ShouldNotError()
        {
            Handler.SetTypeOnObjectField(null, typeof(TestHelpers.TestObject));
        }
        
        [Test]
        public void WhenSettingTypeOnObjectField_ShouldSetTypeToTypeProvided()
        {
            ObjectField field = new ObjectField();
            Handler.SetTypeOnObjectField(field, typeof(TestHelpers.TestObject));
            Assert.AreSame(
                typeof(TestHelpers.TestObject),
                field.objectType);
        }
        
        [Test]
        public void WhenCountIsNotZero_ShouldEnableButton()
        {
            Button button = new Button();
            Handler.DisableButtonWhenCountIsZero(button, 1);
            Assert.IsTrue(button.enabledSelf);
        }
      
        [Test]
        public void ShouldDisableButtonWhenCountIsZero()
        {
            Button button = new Button();
            Handler.DisableButtonWhenCountIsZero(button, 0);
            Assert.IsFalse(button.enabledSelf);
        }
        
        [Test]
        public void WhenSettingButtonStateWithNullAsButton_ShouldFailSilently()
        {
            Handler.DisableButtonWhenCountIsZero(null, 0);
            Handler.DisableButtonWhenCountIsZero(null, 1);
        }
    }
}