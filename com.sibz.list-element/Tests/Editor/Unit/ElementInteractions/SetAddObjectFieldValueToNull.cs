using NUnit.Framework;
using UnityEditor.UIElements;
using UnityEngine;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractionsTests
{
    public class SetAddObjectFieldValueToNull
    {
        [Test]
        public void WhenElementIsNull_ShouldFailSilently()
        {
            Handler.SetAddObjectFieldValueToNull(null);
        }

        [Test]
        public void ShouldSetToNull()
        {
            ObjectField objectField = new ObjectField
                {value = ScriptableObject.CreateInstance<TestHelpers.TestComponent>()};

            if (objectField is null)
            {
                Assert.Fail("Object field is null before method call");
            }

            Handler.SetAddObjectFieldValueToNull(objectField);

            Assert.IsNull(objectField.value);
        }
    }
}