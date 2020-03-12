namespace Sibz.ListElement
{
    public class ListElementOptions
    {
        public string TemplateName { get; set; } = Constants.DefaultTemplateName;
        public string ItemTemplateName { get; set; } = Constants.DefaultItemTemplateName;
        public string StyleSheetName { get; set; } = Constants.DefaultStyleSheetName;
        public string Label { get; set; }

        public bool HidePropertyLabel { get; set; }
    }
}