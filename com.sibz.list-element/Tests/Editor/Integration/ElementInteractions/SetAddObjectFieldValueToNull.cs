using NUnit.Framework;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Integration.ElementInteractions
{
    public class SetAddObjectFieldValueToNull
    {
        [Test]
        public void ShouldNotNotify()
        {
            // ReSharper disable once HeapView.ClosureAllocation
            bool notified = false;

            void TestCallBack(ChangeEvent<Object> e)
            {
                notified = true;
            }

            ObjectField objectField = new ObjectField();

            objectField.value = new Object();

            objectField.RegisterCallback<ChangeEvent<Object>>(TestCallBack);

            WindowFixture.Window.rootVisualElement.Add(objectField);

            Internal.ElementInteractions.SetAddObjectFieldValueToNull(objectField);

            WindowFixture.Window.rootVisualElement.Remove(objectField);

            Assert.IsFalse(notified);
        }
    }
}