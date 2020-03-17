using System;
using NUnit.Framework;
using Sibz.ListElement.Events;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.EventRaiserDefinition
{
    public class CreateDefinition
    {
        [Test]
        public void WhenControlIsNull_ShouldThrowArgumentNullException()
        {
            try
            {
                Events.EventRaiserDefinition.Create<TestEvent>(null, null, new VisualElement());
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("control", e.ParamName);
                return;
            }
            Assert.Fail("ArgumentNullException not thrown");
        }

        [Test]
        public void WhenTargetNullAndControlIsNotAMemberOfListEvent_ShouldThrowArgumentException()
        {
            try
            {
                Events.EventRaiserDefinition.Create<TestEvent>(new VisualElement());
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("target", e.ParamName);
                return;
            }
            Assert.Fail("ArgumentException not thrown");
        }

        [Test]
        public void WhenDelegateIsNull_ShouldNotThrowError()
        {
            Events.EventRaiserDefinition.Create<TestEvent>(new VisualElement(), null, new VisualElement());
        }
        
        [Test]
        public void ShouldSetControl()
        {
            Events.EventRaiserDefinition def = Events.EventRaiserDefinition.Create<TestEvent>(new VisualElement(), null, new VisualElement());
            Assert.IsNotNull(def.Control);
        }
    }
}