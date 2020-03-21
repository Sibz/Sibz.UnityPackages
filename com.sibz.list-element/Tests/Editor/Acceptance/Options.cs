using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Sibz.ListElement.Internal;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Acceptance
{
    public class Options
    {
        private static readonly string[] Labels = new[] {string.Empty, TestHelpers.DefaultTestLabel};

        private ListElement listElement;
        private static SerializedProperty Property => TestHelpers.GetProperty();

        private static readonly IEnumerable<ListOptions> LabelWorkingOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(nameof(ListOptions.Label));


        [Test]
        public void Label_ShouldHaveCorrectText(
            [ValueSource(nameof(LabelWorkingOptionSet))] ListOptions options,
            [ValueSource(nameof(Labels))] string label)
        {
            options.Label = label;
            string expectedLabel = string.IsNullOrEmpty(label)?
                ObjectNames.NicifyVariableName(nameof(TestHelpers.TestComponent.myList)) : label;
            listElement = new ListElement(Property, options);
            WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                Assert.AreEqual(expectedLabel, listElement.Controls.HeaderLabel.text);
            });
        }
    }
}