// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sibz.ListElement
{
    public class ListOptions
    {
        public const string DefaultTemplateName = "Sibz.ListElement.Template";
        public const string DefaultItemTemplateName = "Sibz.ListElement.ItemTemplate";
        public const string DefaultStyleSheetName = "Sibz.ListElement.Template";

        public string TemplateName { get; set; } = DefaultTemplateName;
        public string ItemTemplateName { get; set; } = DefaultItemTemplateName;
        public string StyleSheetName { get; set; } = DefaultStyleSheetName;
        public string Label { get; set; } = "";
        public bool HidePropertyLabel { get; set; } = true;
        public bool EnableObjectField { get; set; } = true;
        public bool EnableReordering { get; set; } = true;
        public bool EnableDeletions { get; set; } = true;
    }
}