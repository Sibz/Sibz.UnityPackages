using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

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
            yield return null;
            testElement.Q<ListElement>().BindProperty(Property);
            Assert.IsTrue(testElement.Q<ListElement>().IsInitialised);
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
            Assert.IsTrue(CheckForDefaultTemplateItems());
        }

        private bool CheckForDefaultTemplateItems()
        {
            return
                ListElement.Controls.HeaderSection != null
                &&
                ListElement.Controls.ClearListConfirmSection != null
                &&
                ListElement.Controls.ItemsSection != null
                &&
                ListElement.Controls.AddSection != null
                ;
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

        [UnityTest]
        public IEnumerator ShouldNameLabelAsProvidedByUxmlAttribute()
        {
            VisualTreeAsset vta =
                SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>("sibz.list.tests.config-test");
            VisualElement root = new VisualElement();
            vta.CloneTree(root);
            ListElement le = root.Q<ListElement>();
            le.BindProperty(Property);
            yield return null;
            Assert.IsNotNull(le.Controls.HeaderLabel);
            Assert.AreEqual("TestLabel", le.Controls.HeaderLabel.text);
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
    }
}