using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace Sibz.ListElement.Tests
{
    [SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
    public class PropertyModificationHandlerTests
    {
        private GameObject testGameObject;
        private SerializedObject testSerializedGameObject;
        private SerializedProperty property;
        private PropertyModificationHandler handler;
            

        [System.Serializable]
        private class TestObject : MonoBehaviour
        {
            public List<string> myList = new List<string>() {"item1", "item2", "item3"};
        }
        
        [SetUp]
        public void SetUp()
        {
            testGameObject = Object.Instantiate(new GameObject());
            testGameObject.AddComponent<TestObject>();
            testSerializedGameObject = new SerializedObject(testGameObject.GetComponent<TestObject>());
            property = testSerializedGameObject.FindProperty(nameof(TestObject.myList));
            handler = new PropertyModificationHandler(property);
        }
        
        [Test]
        public void ShouldAddItemToList()
        {
            int initialArraySize = property.arraySize;
            handler.Add();
            Assert.AreEqual(initialArraySize+1, property.arraySize);
        }

        [Test]
        public void ShouldClearList()
        {
           handler.Clear();
           Assert.AreEqual(0, property.arraySize);
        }

        [Test]
        public void ShouldDeleteItem()
        {
            int initialArraySize = property.arraySize;
            handler.Remove(0);
            Assert.AreEqual(initialArraySize-1, property.arraySize);
        }

        [Test]
        public void ShouldDeleteCorrectItem()
        {
            int initialArraySize = property.arraySize;
            handler.Remove(0);
            Assert.AreEqual("item3", property.GetArrayElementAtIndex(1).stringValue);
        }

        [Test]
        public void ShouldThrowWhenDeletingOutOfRangeItem()
        {

            bool errorThrown = false;
            try
            {
                handler.Remove(10);
            }
            catch
            {
                errorThrown = true;
            }
            Assert.IsTrue(errorThrown);
        }
        
        [Test]
        public void ShouldMoveItemUp()
        {
            handler.MoveUp(1);
            Assert.AreEqual("item2", property.GetArrayElementAtIndex(0).stringValue);
        }

        [Test]
        public void ShouldSilentlyFailMovingFirstItemUp()
        {
            handler.MoveUp(0);
            Assert.AreEqual("item1", property.GetArrayElementAtIndex(0).stringValue);
        }

        [Test]
        public void ShouldThrowIfMovingUpOutOfRangeItem()
        {
            
            bool errorThrown = false;
            bool errorThrown2 = false;
            try
            {
                handler.MoveUp(10);
            }
            catch
            {
                errorThrown = true;
            }
            try
            {
                handler.MoveUp(-1);
            }
            catch
            {
                errorThrown2 = true;
            }
            Assert.IsTrue(errorThrown && errorThrown2);
        }

        [Test]
        public void ShouldMoveItemDown()
        {
            handler.MoveDown(1);
            Assert.AreEqual("item2", property.GetArrayElementAtIndex(2).stringValue);
        }

        [Test]
        public void ShouldSilentlyFailMovingLastItemDown()
        {
            handler.MoveDown(2);
            Assert.AreEqual("item3", property.GetArrayElementAtIndex(2).stringValue);
        }

        [Test]
        public void ShouldThrowIfMovingItemDownOutOfRangeItem()
        {

            bool errorThrown = false;
            bool errorThrown2 = false;
            try
            {
                handler.MoveDown(10);
            }
            catch
            {
                errorThrown = true;
            }
            try
            {
                handler.MoveDown(-1);
            }
            catch
            {
                errorThrown2 = true;
            }
            Assert.IsTrue(errorThrown && errorThrown2);
        }
    }
}