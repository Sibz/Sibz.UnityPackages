using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests
{
    /// <summary>
    /// ListElement renders a list using defaults or provided options dealing with the
    /// interactions that modify the list
    /// </summary>
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class ListElementTests
    {
        private GameObject testGameObject;
        private SerializedObject testSerializedGameObject;

        private VisualElement template;

        [SetUp]
        public void TestSetup()
        {
            template = new VisualElement();

            testGameObject = Object.Instantiate(new GameObject());
            testGameObject.AddComponent<MyTestObject>();

            testSerializedGameObject = new SerializedObject(testGameObject.GetComponent<MyTestObject>());

            VisualTreeAsset vta =
                SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>("Sibz.ListElement.Template");

            vta.CloneTree(template);
        }

        [Test]
        public void InitialisedWhenLoadedFromUxml()
        {
            VisualTreeAsset vta = SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>("ListElementTemplateTest");
            VisualElement testElement = new VisualElement();
            vta.CloneTree(testElement);

            Assert.IsTrue(testElement.Q<ListElement>().IsInitialised);
        }

        [Test]
        public void ShouldBeInitialisedWhenConstructedWithSerializedProperty()
        {
            SerializedProperty prop = testSerializedGameObject.FindProperty(nameof(MyTestObject.myList));
            ListElement listElement = new ListElement(prop);
            Assert.IsTrue(listElement.IsInitialised);
        }

        [Test]
        public void ShouldNotBeInitialisedWhenConstructedWithOutSerializedProperty()
        {
            ListElement listElement = new ListElement();
            Assert.IsFalse(listElement.IsInitialised);
        }

        [Test]
        public void ResetShouldBeCalledWhenSerializedPropertyIsRebound()
        {
            SerializedProperty prop = testSerializedGameObject.FindProperty(nameof(MyTestObject.myList));
            ListElement listElement = new ListElement(prop);
            bool hasReset = false;
            listElement.OnReset += () => hasReset = true;
            listElement.Unbind();
            listElement.BindProperty(prop);
            Assert.IsTrue(hasReset);
        }

        [Test]
        public void ShouldHaveOneArraySizeField()
        {
            SerializedProperty prop = testSerializedGameObject.FindProperty(nameof(MyTestObject.myList));
            ListElement listElement = new ListElement(prop);
            Assert.AreEqual(1,
                listElement.Query<IntegerField>().Where(x => x.bindingPath.Contains("Array.size")).ToList().Count());
        }

        [Test]
        public void ArraySizeShouldBeHidden()
        {
            SerializedProperty prop = testSerializedGameObject.FindProperty(nameof(MyTestObject.myList));
            ListElement listElement = new ListElement(prop);
            IntegerField arraySize = listElement
                .Query<IntegerField>()
                .Where(x => x.bindingPath.Contains("Array.size"))
                .First();
            Assert.IsTrue(arraySize.style.display == DisplayStyle.None);
        }

        [Test]
        public void ShouldContainDefaultTemplateItems()
        {
            SerializedProperty prop = testSerializedGameObject.FindProperty(nameof(MyTestObject.myList));
            ListElement listElement = new ListElement(prop, string.Empty);
            
            listElement.BindProperty(testSerializedGameObject.FindProperty(nameof(MyTestObject.myList)));
            Assert.IsTrue(CheckForDefaultTemplateItems(listElement));
        }

        private bool CheckForDefaultTemplateItems(ListElement listElement)
        {
            return 
                listElement.Q(null, "sibz-list-header") != null
                &&
                listElement.Q(null, "sibz-list-delete-all-confirm") != null
                &&
                listElement.Q(null, "sibz-list-items-section") != null
                ;
        }

        [Test]
        public void ShouldNameLabelSameAsListWhenNoLabelProvided()
        {
            SerializedProperty prop = testSerializedGameObject.FindProperty(nameof(MyTestObject.myList));
            ListElement listElement = new ListElement(prop, string.Empty);
            Label label = listElement.Q<Label>(null, "sibz-list-header-label");
            Assert.AreEqual( ObjectNames.NicifyVariableName(nameof(MyTestObject.myList)), label.text);
        }
        
        [Test]
        public void ShouldNameLabelAsProvided()
        {
            SerializedProperty prop = testSerializedGameObject.FindProperty(nameof(MyTestObject.myList));
            ListElement listElement = new ListElement(prop, "Label");
            Label label = listElement.Q<Label>(null, "sibz-list-header-label");
            Assert.AreEqual( "Label", label.text);
        }

        [Test]
        public void ShouldNameLabelAsProvidedInConfig()
        {
            SerializedProperty prop = testSerializedGameObject.FindProperty(nameof(MyTestObject.myList));
            ListElement listElement = new ListElement(prop, new ListElement.Config() { Label = "Label"});
            Label label = listElement.Q<Label>(null, "sibz-list-header-label");
            Assert.AreEqual( "Label", label.text);
        }

        [Test]
        public void ShouldLoadConfigFromUxml()
        {
            VisualTreeAsset vta = SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>("sibz.list.tests.configtest");
            VisualElement root = new VisualElement();
            vta.CloneTree(root);
            ListElement le = root.Q<ListElement>();
            
            Assert.AreEqual("TestLabel", le.Label);
            // Extra test case to ensure config is applied
            Label label = le.Q<Label>(null, "sibz-list-header-label");
            Assert.IsNotNull(label);
            Assert.AreEqual( "TestLabel", label.text);
        }
    }

    [System.Serializable]
    public class MyTestObject : MonoBehaviour
    {
        public List<string> myList = new List<string>() {"item1", "item2", "item3"};
    }
}