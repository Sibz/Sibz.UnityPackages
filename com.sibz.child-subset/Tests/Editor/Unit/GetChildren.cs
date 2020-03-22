using System;
using NUnit.Framework;

namespace Sibz.ChildSubset.Tests.Unit
{
    public class GetChildren
    {
        [Test]
        public void WhenRootObjectNull_ShouldThrowArgumentNullException()
        {
            Assert.Catch<ArgumentNullException>(() => ChildSubset.GetChildren(null));
        }
    }
}