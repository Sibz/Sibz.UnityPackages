using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Sibz.ListElement.Internal;
using UnityEditor;
using UnityEngine.TestTools;
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

        private static readonly IEnumerable<ListOptions> RowLabelWorkingOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(new[] {nameof(ListOptions.EnableRowLabel), nameof(ListOptions.TemplateName), nameof(ListOptions.ItemTemplateName), nameof(ListOptions.StyleSheetName)});

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

        [UnityTest]
        public IEnumerator PropertyLabel_ShouldHaveCorrectVisibility(
            [ValueSource(nameof(RowLabelWorkingOptionSet))] ListOptions options,
            [Values(true,false)] bool option
        )
        {
            options.EnableRowLabel = option;
            DisplayStyle expectedDisplayStyle = option ? DisplayStyle.Flex : DisplayStyle.None;
            listElement = new ListElement(Property, options);
            yield return WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                for (int i = 0; i < listElement.Controls.ItemsSection.childCount; i++)
                {
                    Assert.AreEqual(expectedDisplayStyle,
                        listElement.Controls.Row[i].PropertyFieldLabel.resolvedStyle.display);
                }
                return null;
            });
        }
    }
}