using NUnit.Framework;
using UnityEditor;
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
            Property = TestHelpers.GetProperty();
            ObjectProperty = TestHelpers.GetProperty(nameof(TestHelpers.TestComponent.myCustomList));
            Handler = new Handler(Property);
            ObjectHandler = new Handler(ObjectProperty);
        }
    }
}