using NUnit.Framework;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
{
    public class SetButtonStateBasedOnBeingLastPositionInArray
    {
        [Test]
        public void WhenIndexIsLastPosition_ShouldSetEnabledFalse()
        {
            Button moveDown = new Button();
            Handler.SetButtonStateBasedOnBeingLastPositionInArray(moveDown, 1, 2);
            
            Assert.IsFalse(moveDown.enabledSelf);
            
        }
        
        [Test]
        public void WhenIndexIsNotLastPosition_ShouldSetEnabledTrue()
        {
            Button moveDown = new Button();
            moveDown.SetEnabled(false);
            Handler.SetButtonStateBasedOnBeingLastPositionInArray(moveDown, 0, 2);
            
            Assert.IsTrue(moveDown.enabledSelf);
        }

        [Test]
        public void WhenButtonIsNull_ShouldNotThrowError()
        {
            Handler.SetButtonStateBasedOnBeingLastPositionInArray(null, 1, 2);
            
            Assert.Pass();
        }
    }
}