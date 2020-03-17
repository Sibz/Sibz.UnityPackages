using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Handler = Sibz.ListElement.Internal.PropertyModificationHandler;

namespace Sibz.ListElement.Tests.Unit.PropertyModificationHandler
{
    public abstract class PropertyModificationHandlerBase
    {
        protected SerializedProperty Property;
        protected SerializedProperty ObjectProperty;
        protected Handler Handler;
        protected Handler ObjectHandler;
        
        [SetUp]
        public void SetUp()
        {
            GameObject go = new GameObject();
            TestHelpers.TestComponent component = go.AddComponent<TestHelpers.TestComponent>();
            SerializedObject so = new SerializedObject(component);
            Property = so.FindProperty(nameof(TestHelpers.TestComponent.myList));
            ObjectProperty = so.FindProperty(nameof(TestHelpers.TestComponent.myCustomList));
            Handler = new Handler(Property);
            ObjectHandler = new Handler(ObjectProperty);
        }
    }
}