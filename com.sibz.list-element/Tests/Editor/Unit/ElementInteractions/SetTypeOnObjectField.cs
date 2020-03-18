using NUnit.Framework;
using UnityEditor.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
{
    public class SetTypeOnObjectField
    {
        [Test]
        public void WhenSettingTypeOnNull_ShouldNotError()
        {
            Handler.SetTypeOnObjectField(null, typeof(TestHelpers.TestObject));
        }
        
        [Test]
        public void WhenSettingTypeOnObjectField_ShouldSetTypeToTypeProvided()
        {
            ObjectField field = new ObjectField();
            Handler.SetTypeOnObjectField(field, typeof(TestHelpers.TestObject));
            Assert.AreSame(
                typeof(TestHelpers.TestObject),
                field.objectType);
        }
    }
}