using NUnit.Framework;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
{
    public class InsertLabelIntoObjectField
    {
        [Test]
        public void WhenReplacingObjectFieldLabelAndFieldIsNull_ShouldNotThrowError()
        {
            Handler.InsertLabelInObjectField(null, "Test");
        }

        [Test]
        public void WhenReplacingObjectFieldLabelAndFieldIsEmpty_ShouldNotThrowError()
        {
            ObjectField objectField = new ObjectField();
            objectField.hierarchy.RemoveAt(0);
            Handler.InsertLabelInObjectField(objectField, "Test");
        }

        [Test]
        public void WhenSettingObjectFieldLabel_ShouldHaveAdditionalLabelWithText()
        {
            ObjectField objectField = new ObjectField();
            Handler.InsertLabelInObjectField(objectField, "Test");
            Assert.AreEqual("Test",
                (objectField.hierarchy[0].hierarchy[0].hierarchy[2] as Label)?.text);
        }
    }
}