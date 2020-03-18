using NUnit.Framework;
using UnityEngine.UIElements;
using Handler = Sibz.ListElement.Internal.ElementInteractions;

namespace Sibz.ListElement.Tests.Unit.ElementInteractionsTests
{
    public class SetHeaderLabelText
    {
        [Test]
        public void WhenOptionsLabelSet_ShouldUseOptionsLabel()
        {
            const string optionsLabel = "Test";
            Label label = new Label();
            Handler.SetHeaderLabelText(label, null, optionsLabel);
            Assert.AreEqual(
                optionsLabel,
                label.text);
        }

        [Test]
        public void WhenOptionsLabelNotSet_ShouldUseListName()
        {
            const string listName = "Test";
            Label label = new Label();
            Handler.SetHeaderLabelText(label, listName, null);
            Assert.AreEqual(
                listName,
                label.text);
        }

        [Test]
        public void WhenLabelIsNull_ShouldNotThrowError()
        {
            Handler.SetHeaderLabelText(null, null, null);
        }
    }
}