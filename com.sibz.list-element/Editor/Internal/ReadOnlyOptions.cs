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
        public bool EnableRowLabel => BaseOptions.EnableRowLabel;
        public bool EnableObjectField => BaseOptions.EnableObjectField;
        public bool EnableReordering => BaseOptions.EnableReordering;
        public bool EnableDeletions => BaseOptions.EnableDeletions;
        public bool EnableAdditions => BaseOptions.EnableAdditions;
    }
}