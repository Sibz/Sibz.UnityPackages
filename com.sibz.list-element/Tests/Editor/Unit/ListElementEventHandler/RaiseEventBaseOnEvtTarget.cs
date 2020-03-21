using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sibz.ListElement.Events;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Unit.ListElementEventHandlerTests
{
    public class RaiseEventBaseOnEvtTarget
    {
        [Test]
        public void WhenTargetIsNull_ShouldThrowArgumentNullException()
        {
            try
            {
                ListElementEventHandler.RaiseEventBaseOnEvtTarget(null, new List<Events.EventRaiserDefinition>());
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("target", e.ParamName);
            }
        }

        [Test]
        public void WhenEventRaisersIsNull_ShouldThrowArgumentNullException()
        {
            try
            {
                ListElementEventHandler.RaiseEventBaseOnEvtTarget(new VisualElement(), null);
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual("eventRaisers", e.ParamName);
            }
        }
    }
}