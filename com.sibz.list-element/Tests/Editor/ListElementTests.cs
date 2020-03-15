using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Sibz.ListElement.Tests
{
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class ListElementTests : ListElementTestBase
    {
        [UnityTest]
        public IEnumerator ShouldInitialiseWhenLoadedFromUxml()
        {
            VisualTreeAsset vta = SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>("ListElementTemplateTest");
            VisualElement testElement = new VisualElement();
            vta.CloneTree(testElement);

            testElement.Q<ListElement>().BindProperty(Property);
            Assert.IsTrue(testElement.Q<ListElement>().IsInitialised);
            yield return null;
        }

        [Test]
        public void ShouldBeInitialisedWhenConstructedWithSerializedProperty()
        {
            Assert.IsTrue(ListElement.IsInitialised);
        }

        [Test]
        public void ShouldNotBeInitialisedWhenConstructedWithOutSerializedProperty()
        {
            ListElement testListElement = new ListElement();
            Assert.IsFalse(testListElement.IsInitialised);
        }

        [Test]
        public void ShouldCallResetWhenSerializedPropertyIsRebound()
        {
            bool hasReset = false;
            ListElement.OnReset += () => hasReset = true;
            ListElement.Unbind();
            ListElement.BindProperty(Property);
            Assert.IsTrue(hasReset);
        }

        [Test]
        public void ShouldHaveOneArraySizeField()
        {
            Assert.AreEqual(1,
                ListElement.Query<IntegerField>().Where(x => x.bindingPath.Contains("Array.size")).ToList().Count);
        }

        [Test]
        public void ShouldHideArraySizeField()
        {
            IntegerField arraySize = ListElement
                .Query<IntegerField>()
                .Where(x => x.bindingPath.Contains("Array.size"))
                .First();
            Assert.IsTrue(arraySize.style.display == DisplayStyle.None);
        }

        [Test]
        public void ShouldContainDefaultTemplateItems()
        {
            ListElement.BindProperty(TestSerializedGameObject.FindProperty(nameof(MyTestObject.myList)));
            Assert.IsTrue(CheckForDefaultTemplateItems(ListElement));
        }

        private static bool CheckForDefaultTemplateItems(VisualElement testElement)
        {
            return
                testElement.Q(null, Constants.HeaderSectionClassName) != null
                &&
                testElement.Q(null, Constants.DeleteConfirmSectionClassName) != null
                &&
                testElement.Q(null, Constants.ItemSectionClassName) != null
                ;
        }

        [Test]
        public void ShouldNameLabelSameAsListWhenNoLabelProvided()
        {
            ListElement testElement = new ListElement(string.Empty, Property);
            Label label = testElement.Q<Label>(null, Constants.HeaderLabelClassName);
            Assert.AreEqual(ObjectNames.NicifyVariableName(nameof(MyTestObject.myList)), label.text);
        }

        [Test]
        public void ShouldNameLabelAsProvided()
        {
            ListElement testElement = new ListElement("Label", Property);
            Label label = testElement.Q<Label>(null, Constants.HeaderLabelClassName);
            Assert.AreEqual("Label", label.text);
        }

        [Test]
        public void ShouldNameLabelAsProvidedInConfig()
        {
            ListElement testElement = new ListElement(Property, new ListElementOptions {Label = "Label"});
            Label label = testElement.Q<Label>(null, Constants.HeaderLabelClassName);
            Assert.AreEqual("Label", label.text);
        }

        [Test]
        public void ShouldLoadConfigFromUxml()
        {
            VisualTreeAsset vta =
                SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>("sibz.list.tests.config-test");
            VisualElement root = new VisualElement();
            vta.CloneTree(root);
            ListElement le = root.Q<ListElement>();

            Assert.AreEqual("TestLabel", le.Options.Label);
            Assert.AreEqual("TestTemplate", le.Options.TemplateName);
            Assert.AreEqual("TestTemplate", le.Options.StyleSheetName);
            Assert.AreEqual("TestItemTemplate", le.Options.ItemTemplateName);
        }

        [Test]
        public void ShouldNameLabelAsProvidedByUxmlAttribute()
        {
            VisualTreeAsset vta =
                SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>("sibz.list.tests.config-test");
            VisualElement root = new VisualElement();
            vta.CloneTree(root);
            ListElement le = root.Q<ListElement>();

            Label label = le.Q<Label>(null, Constants.HeaderLabelClassName);
            Assert.IsNotNull(label);
            Assert.AreEqual("TestLabel", label.text);
        }

        [Test]
        public void ShouldApplyCustomStylesheet()
        {
            ListElement testElement =
                new ListElement(Property, new ListElementOptions {StyleSheetName = "TestTemplate"});
            Assert.IsTrue(
                testElement.styleSheets.Contains(
                    SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>("TestTemplate")));
        }

        [Test]
        public void ShouldApplyCustomTemplate()
        {
            ListElement testElement =
                new ListElement(Property, new ListElementOptions {TemplateName = "TestTemplate"});
            Assert.IsNotNull(
                testElement.Q<VisualElement>("TestTemplateCheck"));
        }

        [Test]
        public void ShouldApplyCustomItemTemplate()
        {
            ListElement testElement =
                new ListElement(Property, new ListElementOptions {ItemTemplateName = "TestItemTemplate"});
            Assert.IsNotNull(
                testElement.Q<VisualElement>("TestItemTemplateCheck"));
        }

        [Test]
        public void ShouldCorrectlyDetermineTypeOfListAsString()
        {
            Assert.AreEqual(typeof(string), ListElement.ListItemType);
        }

        [Test]
        public void ShouldCorrectlyDetermineTypeOfListAsCustomObject()
        {
            ListElement testElement =
                new ListElement(TestSerializedGameObject.FindProperty(nameof(MyTestObject.myCustomList)));
            Assert.AreEqual(typeof(CustomObject), testElement.ListItemType);
        }

        [Test]
        public void ShouldPopulateList()
        {
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        [Test]
        public void ShouldKeepListInSyncAfterAddNew()
        {
            ListElement.AddNewItemToList();
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        [Test]
        public void ShouldKeepListInSyncAfterClear()
        {
            ListElement.ClearListItems();
            Assert.AreEqual(0, Property.arraySize);
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        [Test]
        public void ShouldKeepListInSyncAfterDeleteItem()
        {
            ListElement.RemoveItem(1);
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        [Test]
        public void ShouldKeepListInSyncAfterMoveUp()
        {
            ListElement.MoveItemUp(1);
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        [Test]
        public void ShouldKeepListInSyncAfterMoveDown()
        {
            ListElement.MoveItemDown(1);
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        private bool CheckArrayAndElementsAreInSync()
        {
            var propFields = ListElement.Query<PropertyField>().ToList();
            if (propFields.Count != Property.arraySize)
            {
                Debug.LogWarning("Count is not the same");
                return false;
            }

            for (int i = 0; i < Property.arraySize; i++)
            {
                if (propFields[i].Q<TextField>().text == Property.GetArrayElementAtIndex(i).stringValue)
                {
                    continue;
                }

                Debug.LogWarningFormat(
                    "Item {0} is not the same (textfield:{1} vs array:{2})",
                    i,
                    propFields[i].Q<TextField>().text,
                    Property.GetArrayElementAtIndex(i).stringValue);
                return false;
            }

            return true;
        }

        [Test]
        public void ShouldAddHidePropertyLabelStyleSheetIfRequired()
        {
            ListElement testElement = new ListElement(Property, new ListElementOptions {HidePropertyLabel = true});

            Assert.IsTrue(testElement.styleSheets.Contains(
                SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>(Constants.HidePropertyLabelStyleSheetName)));
        }

        [Test]
        public void ShouldNotAddHidePropertyLabelStyleSheetIfNotRequired()
        {
            ListElement testElement = new ListElement(Property, new ListElementOptions {HidePropertyLabel = false});

            Assert.IsFalse(testElement.styleSheets.Contains(
                SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>(Constants.HidePropertyLabelStyleSheetName)));
        }

        [UnityTest]
        public IEnumerator ShouldHidePropertyLabelsIfRequired()
        {
            yield return null;
            Assert.IsTrue(ListElement.Query<PropertyField>().Build().ToList().All(x =>
                x.hierarchy[0].hierarchy[0].resolvedStyle.display == DisplayStyle.None));
        }

        [UnityTest]
        public IEnumerator ShouldNotHidePropertyLabelsIfNotRequired()
        {
            ListElement testElement = new ListElement(Property, new ListElementOptions {HidePropertyLabel = false});
            TestWindow.rootVisualElement.Add(testElement);
            yield return null;
            Assert.IsFalse(testElement.Query<PropertyField>().Build().ToList().Any(x =>
                x.hierarchy[0].hierarchy[0].resolvedStyle.display == DisplayStyle.None));
            TestWindow.rootVisualElement.Remove(testElement);
        }

        [Test]
        public void ShouldHaveAddButtonForSimpleTypes()
        {
            Assert.AreEqual(
                DisplayStyle.Flex,
                ListElement.Q(null, Constants.AddButtonClassName).style.display.value);
            Assert.AreEqual(
                DisplayStyle.None,
                ListElement.Q(null, Constants.AddItemObjectField).style.display.value);
        }

        [Test]
        public void ShouldHaveObjectFieldForObjectTypes()
        {
            ListElement testElement =
                new ListElement(TestSerializedGameObject.FindProperty(nameof(MyTestObject.myCustomList)));
            Assert.AreEqual(
                DisplayStyle.None,
                testElement.Q(null, Constants.AddButtonClassName).style.display.value);
            Assert.AreEqual(
                DisplayStyle.Flex,
                testElement.Q(null, Constants.AddItemObjectField).style.display.value);
        }

        [Test]
        public void ShouldNotUseObjectFieldIfSpecified()
        {
            ListElement testElement =
                new ListElement(
                    TestSerializedGameObject.FindProperty(nameof(MyTestObject.myCustomList)),
                    new ListElementOptions {DoNotUseObjectField = true});

            Assert.AreEqual(
                DisplayStyle.Flex,
                testElement.Q(null, Constants.AddButtonClassName).style.display.value);
            Assert.AreEqual(
                DisplayStyle.None,
                testElement.Q(null, Constants.AddItemObjectField).style.display.value);
        }

        [Test]
        public void ShouldDisableFirstMoveUpButton()
        {
            Assert.IsFalse(ListElement.Q(null, Constants.MoveUpButtonClassName).enabledSelf);
        }

        [Test]
        public void ShouldDisableLastMoveDownButton()
        {
            Assert.IsFalse(ListElement.Query(null, Constants.MoveDownButtonClassName).Build().Last().enabledSelf);
        }

        [Test]
        public void ShouldDisableOnlyFirstMoveUpButton()
        {
            Assert.AreEqual(1,
                ListElement.Query(null, Constants.MoveUpButtonClassName).Build().ToList()
                    .Count(x => x.enabledSelf == false));
        }

        [Test]
        public void ShouldDisableOnlyLastMoveDownButton()
        {
            Assert.AreEqual(1,
                ListElement.Query(null, Constants.MoveDownButtonClassName).Build().ToList()
                    .Count(x => x.enabledSelf == false));
        }

        [Test]
        public void ShouldDisableBothReorderButtonsWithOnlyOneItem()
        {
            ListElement.ClearListItems();
            ListElement.AddNewItemToList();
            Assert.IsFalse(ListElement.Q(null, Constants.MoveUpButtonClassName).enabledSelf);
            Assert.IsFalse(ListElement.Q(null, Constants.MoveDownButtonClassName).enabledSelf);
        }

        [Test]
        public void ShouldDisableClearListButtonWhenNoItems()
        {
            ListElement.ClearListItems();
            Assert.IsFalse(ListElement.Q(null, Constants.DeleteAllButtonClassName).enabledSelf);
        }

        [Test]
        public void ShouldNotDisableClearListButtonWhenNoItems()
        {
            Assert.IsTrue(ListElement.Q(null, Constants.DeleteAllButtonClassName).enabledSelf);
        }

        [Serializable]
        public class MyTestObject : MonoBehaviour
        {
            public List<CustomObject> myCustomList = new List<CustomObject> {new CustomObject()};
            public List<string> myList = new List<string> {"item1", "item2", "item3"};
        }
    }


    public class CustomObject : Object
    {
    }
}