using System;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.EventRaiserDefinition
{
    public class CreateRaiseEvent
    {
        [Test]
        public void WhenEventTypeIsNull_ShouldThrowError()
        {
            try
            {
                Events.EventRaiserDefinition.CreateRaiseEvent(null, new VisualElement(), (e) => { });
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(e.ParamName, "eventType");
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void WhenEventTypeIsNotEventBase_ShouldThrowError()
        {
            try
            {
                Events.EventRaiserDefinition.CreateRaiseEvent(typeof(string), new VisualElement(), (e) => { });
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(e.ParamName, "eventType");
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void WhenTargetIsNull_ShouldThrowError()
        {
            try
            {
                Events.EventRaiserDefinition.CreateRaiseEvent(typeof(TestEvent), null, (e) => { });
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(e.ParamName, "target");
                return;
            }

            Assert.Fail();
        }

        [Test]
        public void ShouldSetTarget()
        {
            EventBase raiseEvent =
                Events.EventRaiserDefinition.CreateRaiseEvent(typeof(TestEvent), new VisualElement(), (e) => { });
            Assert.IsNotNull(raiseEvent.target);
        }

        [Test]
        public void ShouldInvokeDelegate()
        {
            bool invoked = false;
            Events.EventRaiserDefinition.CreateRaiseEvent(typeof(TestEvent), new VisualElement(),
                (e) => invoked = true);
            Assert.IsTrue(invoked);
        }
    }
}