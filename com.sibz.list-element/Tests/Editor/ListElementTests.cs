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
    public class TestWindow : EditorWindow
    {
    }


    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class ListElementTests
    {
        private ListElement listElement;
        private SerializedProperty property;
        private GameObject testGameObject;
        private SerializedObject testSerializedGameObject;
        private TestWindow testWindow;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testWindow = EditorWindow.GetWindow<TestWindow>();
            testGameObject = Object.Instantiate(new GameObject());
        }

        [SetUp]
        public void TestSetup()
        {
            testGameObject.AddComponent<MyTestObject>();
            testSerializedGameObject = new SerializedObject(testGameObject.GetComponent<MyTestObject>());
            property = testSerializedGameObject.FindProperty(nameof(MyTestObject.myList));
            listElement = new ListElement(property);

            testWindow.rootVisualElement.Add(listElement);
        }

        [TearDown]
        public void TearDown()
        {
            testWindow.rootVisualElement.Clear();
            Object.DestroyImmediate(testGameObject.GetComponent<MyTestObject>());
            testSerializedGameObject = null;
            property = null;
            listElement = null;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            testWindow.Close();
            Object.DestroyImmediate(testWindow);
            Object.DestroyImmediate(testGameObject);
        }

        [UnityTest]
        public IEnumerator ShouldInitialiseWhenLoadedFromUxml()
        {
            VisualTreeAsset vta = SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>("ListElementTemplateTest");
            VisualElement testElement = new VisualElement();
            vta.CloneTree(testElement);

            testElement.Q<ListElement>().BindProperty(property);
            Assert.IsTrue(testElement.Q<ListElement>().IsInitialised);
            yield return null;
        }

        [Test]
        public void ShouldBeInitialisedWhenConstructedWithSerializedProperty()
        {
            Assert.IsTrue(listElement.IsInitialised);
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
            listElement.OnReset += () => hasReset = true;
            listElement.Unbind();
            listElement.BindProperty(property);
            Assert.IsTrue(hasReset);
        }

        [Test]
        public void ShouldHaveOneArraySizeField()
        {
            Assert.AreEqual(1,
                listElement.Query<IntegerField>().Where(x => x.bindingPath.Contains("Array.size")).ToList().Count);
        }

        [Test]
        public void ShouldHideArraySizeField()
        {
            IntegerField arraySize = listElement
                .Query<IntegerField>()
                .Where(x => x.bindingPath.Contains("Array.size"))
                .First();
            Assert.IsTrue(arraySize.style.display == DisplayStyle.None);
        }

        [Test]
        public void ShouldContainDefaultTemplateItems()
        {
            listElement.BindProperty(testSerializedGameObject.FindProperty(nameof(MyTestObject.myList)));
            Assert.IsTrue(CheckForDefaultTemplateItems(listElement));
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
            ListElement testElement = new ListElement(property, string.Empty);
            Label label = testElement.Q<Label>(null, Constants.HeaderLabelClassName);
            Assert.AreEqual(ObjectNames.NicifyVariableName(nameof(MyTestObject.myList)), label.text);
        }

        [Test]
        public void ShouldNameLabelAsProvided()
        {
            ListElement testElement = new ListElement(property, "Label");
            Label label = testElement.Q<Label>(null, Constants.HeaderLabelClassName);
            Assert.AreEqual("Label", label.text);
        }

        [Test]
        public void ShouldNameLabelAsProvidedInConfig()
        {
            ListElement testElement = new ListElement(property, new ListElementOptions {Label = "Label"});
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

            Assert.AreEqual("TestLabel", le.Label);
            Assert.AreEqual("TestTemplate", le.TemplateName);
            Assert.AreEqual("TestTemplate", le.StyleSheetName);
            Assert.AreEqual("TestItemTemplate", le.ItemTemplateName);
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
                new ListElement(property, new ListElementOptions {StyleSheetName = "TestTemplate"});
            Assert.IsTrue(
                testElement.styleSheets.Contains(
                    SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>("TestTemplate")));
        }

        [Test]
        public void ShouldApplyCustomTemplate()
        {
            ListElement testElement =
                new ListElement(property, new ListElementOptions {TemplateName = "TestTemplate"});
            Assert.IsNotNull(
                testElement.Q<VisualElement>("TestTemplateCheck"));
        }

        [Test]
        public void ShouldApplyCustomItemTemplate()
        {
            ListElement testElement =
                new ListElement(property, new ListElementOptions {ItemTemplateName = "TestItemTemplate"});
            Assert.IsNotNull(
                testElement.Q<VisualElement>("TestItemTemplateCheck"));
        }

        [Test]
        public void ShouldCorrectlyDetermineTypeOfListAsString()
        {
            Assert.AreEqual(typeof(string), listElement.ListItemType);
        }

        [Test]
        public void ShouldCorrectlyDetermineTypeOfListAsCustomObject()
        {
            ListElement testElement =
                new ListElement(testSerializedGameObject.FindProperty(nameof(MyTestObject.myCustomList)));
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
            listElement.AddNewItemToList();
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        [Test]
        public void ShouldKeepListInSyncAfterClear()
        {
            listElement.ClearListItems();
            Assert.AreEqual(0, property.arraySize);
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        [Test]
        public void ShouldKeepListInSyncAfterDeleteItem()
        {
            listElement.RemoveItem(1);
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        [Test]
        public void ShouldKeepListInSyncAfterMoveUp()
        {
            listElement.MoveItemUp(1);
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        [Test]
        public void ShouldKeepListInSyncAfterMoveDown()
        {
            listElement.MoveItemDown(1);
            Assert.IsTrue(CheckArrayAndElementsAreInSync());
        }

        private bool CheckArrayAndElementsAreInSync()
        {
            var propFields = listElement.Query<PropertyField>().ToList();
            if (propFields.Count != property.arraySize)
            {
                Debug.LogWarning("Count is not the same");
                return false;
            }

            for (int i = 0; i < property.arraySize; i++)
            {
                if (propFields[i].Q<TextField>().text == property.GetArrayElementAtIndex(i).stringValue)
                {
                    continue;
                }

                Debug.LogWarningFormat(
                    "Item {0} is not the same (textfield:{1} vs array:{2})",
                    i,
                    propFields[i].Q<TextField>().text,
                    property.GetArrayElementAtIndex(i).stringValue);
                return false;
            }

            return true;
        }

        [Test]
        public void ShouldAddHidePropertyLabelStyleSheetIfRequired()
        {
            ListElement testElement = new ListElement(property, new ListElementOptions {HidePropertyLabel = true});

            Assert.IsTrue(testElement.styleSheets.Contains(
                SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>(Constants.HidePropertyLabelStyleSheetName)));
        }

        [Test]
        public void ShouldNotAddHidePropertyLabelStyleSheetIfNotRequired()
        {
            ListElement testElement = new ListElement(property, new ListElementOptions {HidePropertyLabel = false});

            Assert.IsFalse(testElement.styleSheets.Contains(
                SingleAssetLoader.SingleAssetLoader.Load<StyleSheet>(Constants.HidePropertyLabelStyleSheetName)));
        }

        [UnityTest]
        public IEnumerator ShouldHidePropertyLabelsIfRequired()
        {
            yield return null;
            Assert.IsTrue(listElement.Query<PropertyField>().Build().ToList().All(x =>
                x.hierarchy[0].hierarchy[0].resolvedStyle.display == DisplayStyle.None));
        }

        [UnityTest]
        public IEnumerator ShouldNotHidePropertyLabelsIfNotRequired()
        {
            ListElement testElement = new ListElement(property, new ListElementOptions {HidePropertyLabel = false});
            EditorWindow.GetWindow<TestWindow>().rootVisualElement.Add(testElement);
            yield return null;
            Assert.IsFalse(testElement.Query<PropertyField>().Build().ToList().Any(x =>
                x.hierarchy[0].hierarchy[0].resolvedStyle.display == DisplayStyle.None));
        }

        [Test]
        public void ShouldHaveAddButtonForSimpleTypes()
        {
            Assert.AreEqual(
                DisplayStyle.Flex,
                listElement.Q(null, Constants.AddButtonClassName).style.display.value);
            Assert.AreEqual(
                DisplayStyle.None,
                listElement.Q(null, Constants.AddItemObjectField).style.display.value);
        }

        [Test]
        public void ShouldHaveObjectFieldForObjectTypes()
        {
            ListElement testElement =
                new ListElement(testSerializedGameObject.FindProperty(nameof(MyTestObject.myCustomList)));
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
                    testSerializedGameObject.FindProperty(nameof(MyTestObject.myCustomList)),
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
            Assert.IsFalse(listElement.Q(null, Constants.MoveUpButtonClassName).enabledSelf);
        }

        [Test]
        public void ShouldDisableLastMoveDownButton()
        {
            Assert.IsFalse(listElement.Query(null, Constants.MoveDownButtonClassName).Build().Last().enabledSelf);
        }

        [Test]
        public void ShouldDisableOnlyFirstMoveUpButton()
        {
            Assert.AreEqual(1,
                listElement.Query(null, Constants.MoveUpButtonClassName).Build().ToList()
                    .Count(x => x.enabledSelf == false));
        }

        [Test]
        public void ShouldDisableOnlyLastMoveDownButton()
        {
            Assert.AreEqual(1,
                listElement.Query(null, Constants.MoveDownButtonClassName).Build().ToList()
                    .Count(x => x.enabledSelf == false));
        }

        [Test]
        public void ShouldDisableBothReorderButtonsWithOnlyOneItem()
        {
            listElement.ClearListItems();
            listElement.AddNewItemToList();
            Assert.IsFalse(listElement.Q(null, Constants.MoveUpButtonClassName).enabledSelf);
            Assert.IsFalse(listElement.Q(null, Constants.MoveDownButtonClassName).enabledSelf);
        }

        [Test]
        public void ShouldDisableClearListButtonWhenNoItems()
        {
            listElement.ClearListItems();
            Assert.IsFalse(listElement.Q(null, Constants.DeleteAllButtonClassName).enabledSelf);
        }

        [Test]
        public void ShouldNotDisableClearListButtonWhenNoItems()
        {
            Assert.IsTrue(listElement.Q(null, Constants.DeleteAllButtonClassName).enabledSelf);
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