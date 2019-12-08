using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
       // private int templateItemCount; 
/*

       private const string templateXML = "<Sibz.ListElement.ListElement>" +
                                          "<Style src=\"/Assets/ListTest.uss\" />" +
                                          "</Sibz.ListElement.ListElement>";*/
        [SetUp]
        public void TestSetup()
        {
            template = new VisualElement();
            
            testGameObject = Object.Instantiate(new GameObject());
            testGameObject.AddComponent<MyTestObject>();
            
            testSerializedGameObject = new SerializedObject(testGameObject.GetComponent<MyTestObject>());
            
            VisualTreeAsset vta = SingleAssetLoader.SingleAssetLoader.Load<VisualTreeAsset>("Sibz.ListElement.Template");
            
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
    }

    [System.Serializable]
    public class MyTestObject : MonoBehaviour
    {
        public List<string> myList = new List<string>() {"item1", "item2", "item3"};
    }
}