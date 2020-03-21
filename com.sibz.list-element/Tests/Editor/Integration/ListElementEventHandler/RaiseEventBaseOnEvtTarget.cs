using NUnit.Framework;
using Sibz.ListElement.Events;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Integration.ListElementEventHandlerTests
{
    public class RaiseEventBaseOnEvtTarget
    {
        public class RaiseEventTestEvent : EventBase<RaiseEventTestEvent>
        {
        }

        [Test]
        public void ShouldRaiseEvent()
        {
            VisualElement control = new VisualElement();
            EventRaiserDefinition def = EventRaiserDefinition.Create<RaiseEventTestEvent>(control, null, control);
            control.RegisterCallback<RaiseEventTestEvent>(e =>
            {
                Assert.Pass($"{nameof(RaiseEventTestEvent)} was raised");
            });
            WindowFixture.RootElement.AddAndRemove(control, () =>
                ListElementEventHandler.RaiseEventBaseOnEvtTarget(control, new[] {def}));

            Assert.Fail($"{nameof(RaiseEventTestEvent)} was NOT raised");
        }
    }
}