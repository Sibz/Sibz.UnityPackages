﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Acceptance
{
    public class Options
    {
        private static readonly string[] Labels = {string.Empty, TestHelpers.DefaultTestLabel};

        private ListElement listElement;
        private static SerializedProperty Property => TestHelpers.GetProperty();

        private static readonly IEnumerable<ListOptions> LabelWorkingOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(nameof(ListOptions.Label));

        private static readonly IEnumerable<ListOptions> RowLabelWorkingOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(new[]
            {
                nameof(ListOptions.EnableRowLabel), nameof(ListOptions.TemplateName),
                nameof(ListOptions.ItemTemplateName), nameof(ListOptions.StyleSheetName)
            });

        private static readonly IEnumerable<ListOptions> EnableModifyOptionSet =
            AcceptanceFixture.GetWorkingOptionSetExcl(new[]
            {
                nameof(ListOptions.EnableModify), nameof(ListOptions.TemplateName),
                nameof(ListOptions.ItemTemplateName), nameof(ListOptions.StyleSheetName)
            });

        [Test]
        public void Label_ShouldHaveCorrectText([ValueSource(nameof(Labels))] string label)
        {
            string expectedLabel = string.IsNullOrEmpty(label)
                ? ObjectNames.NicifyVariableName(nameof(TestHelpers.TestComponent.myList))
                : label;
            listElement = new ListElement(Property, new ListOptions(){ Label = label});
            WindowFixture.RootElement.AddAndRemove(listElement,
                () => { Assert.AreEqual(expectedLabel, listElement.Controls.HeaderLabel.text); });
        }

        [UnityTest]
        public IEnumerator PropertyLabel_ShouldHaveCorrectVisibility([Values(true, false)] bool option)
        {
            DisplayStyle expectedDisplayStyle = option ? DisplayStyle.Flex : DisplayStyle.None;
            listElement = new ListElement(Property, new ListOptions{EnableRowLabel = option});
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

        [UnityTest]
        public IEnumerator EnableModify_ShouldSetPropertyFieldStateBasedOnOption([Values(true, false)] bool option)
        {
            listElement = new ListElement(Property, new ListOptions{ EnableModify = option });
            yield return WindowFixture.RootElement.AddAndRemove(listElement, () =>
            {
                for (int i = 0; i < listElement.Controls.ItemsSection.childCount; i++)
                {
                    Assert.IsTrue(listElement.Controls.Row[i].PropertyField.enabledSelf == option);
                }

                return null;
            });
        }
    }
}