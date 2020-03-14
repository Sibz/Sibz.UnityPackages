namespace Sibz.ListElement
{
    public class ListElementOptionsInternal 
    {
        public string TemplateName { get; internal set; } = Constants.DefaultTemplateName;
        public string ItemTemplateName { get; internal set; } = Constants.DefaultItemTemplateName;
        public string StyleSheetName { get; internal set; } = Constants.DefaultStyleSheetName;
        public string Label { get; internal set; }
        public bool HidePropertyLabel { get; internal set; } = true;
        public bool DoNotUseObjectField { get; internal set; }
    }
}