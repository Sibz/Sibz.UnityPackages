using NUnit.Framework;
using UnityEditor.UIElements;
using UnityEngine;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
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
            ObjectField objectField = new ObjectField {value = new GameObject()};

            if (objectField is null)
            {
                Assert.Fail("Object field is null before method call");
            }

            Handler.SetAddObjectFieldValueToNull(objectField);

            Assert.IsNull(objectField.value);
        }
    }
}