using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Sibz.ListElement.Tests.Acceptance
{
    [SetUpFixture]
    public class AcceptanceFixture : WindowFixture
    {
        public static Dictionary<string, ListOptions> OptionSets =
            new Dictionary<string, ListOptions>
            {
                {nameof(ListOptions.EnableReordering), new ListOptions {EnableReordering = false}},
                {nameof(ListOptions.EnableDeletions), new ListOptions {EnableDeletions = false}},
                {nameof(ListOptions.HidePropertyLabel), new ListOptions {HidePropertyLabel = false}},
                {nameof(ListOptions.EnableObjectField), new ListOptions {EnableObjectField = true}},
                {nameof(ListOptions.Label), new ListOptions {Label = TestHelpers.DefaultTestLabel}},
                {
                    nameof(ListOptions.TemplateName),
                    new ListOptions {TemplateName = TestHelpers.DefaultTestTemplateName}
                },
                {
                    nameof(ListOptions.StyleSheetName),
                    new ListOptions {StyleSheetName = TestHelpers.DefaultTestTemplateName}
                },
                {
                    nameof(ListOptions.ItemTemplateName),
                    new ListOptions {ItemTemplateName = TestHelpers.DefaultTestItemTemplateName}
                },
            };

        public static IEnumerable<ListOptions> GetWorkingOptionSet(string optionName)
            => GetWorkingOptionSet(new[] {optionName});
        public static IEnumerable<ListOptions> GetWorkingOptionSet(IEnumerable<string> optionNames, bool exclude = false)
        {
            return OptionSets.Where(x => exclude ? !optionNames.Contains(x.Key) : optionNames.Contains(x.Key))
                .Select(x => x.Value);
        }

        public static IEnumerable<ListOptions> GetWorkingOptionSetExcl(string optionName) =>
            GetWorkingOptionSet(new[] {optionName}, true);

        public static IEnumerable<ListOptions> GetWorkingOptionSetExcl(IEnumerable<string> optionNames)=>
            GetWorkingOptionSet(optionNames, true);
    }
}