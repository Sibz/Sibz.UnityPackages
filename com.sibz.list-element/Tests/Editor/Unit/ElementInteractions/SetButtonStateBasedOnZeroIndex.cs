using NUnit.Framework;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractions
{
    public class SetButtonStateBasedOnZeroIndex
    {
        [Test]
        public void WhenIndexIsZero_ShouldSetEnabledFalse()
        {
            Button moveUp = new Button();
            Handler.SetButtonStateBasedOnZeroIndex(moveUp, 0);
            
            Assert.IsFalse(moveUp.enabledSelf);
        }
        
        [Test]
        public void WhenIndexIsNotZero_ShouldSetEnabledTrue()
        {
            Button moveUp = new Button();
            moveUp.SetEnabled(false);
            Handler.SetButtonStateBasedOnZeroIndex(moveUp, 1);
            
            Assert.IsTrue(moveUp.enabledSelf);
        }

        [Test]
        public void WhenButtonIsNull_ShouldNotThrowError()
        {
            Handler.SetButtonStateBasedOnZeroIndex(null, 0);
            
            Assert.Pass();
        }
    }
}