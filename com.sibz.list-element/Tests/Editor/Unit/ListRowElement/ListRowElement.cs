using System;
using NUnit.Framework;
using Row = Sibz.ListElement.ListRowElement;

namespace Sibz.ListElement.Tests.Unit.ListRowElement
{
    public class Constructor
    {
        [Test]
        public void WhenIndexIsLessThanZero_ShouldThrowArgumentException()
        {
            try
            {
                new Row(-1);
            }
            catch (ArgumentException)
            {
                Assert.Pass();
                return;
            }

            Assert.Fail($"{nameof(ArgumentException)} not thrown.");
        }
    }
}