namespace Sibz.ListElement.Internal
{
    public class ReadOnlyOptions
    {
        internal readonly ListOptions BaseOptions;

        public ReadOnlyOptions(ListOptions options)
        {
            BaseOptions = options;
        }

        public string TemplateName => BaseOptions.TemplateName;
        public string ItemTemplateName => BaseOptions.ItemTemplateName;
        public string StyleSheetName => BaseOptions.StyleSheetName;
        public string Label => BaseOptions.Label;
        public bool HidePropertyLabel => BaseOptions.HidePropertyLabel;
        public bool DoNotUseObjectField => BaseOptions.DoNotUseObjectField;
        public bool EnableReordering => BaseOptions.EnableReordering;
        public bool EnableDeletions => BaseOptions.EnableDeletions;
    }
}