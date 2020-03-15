using System.Diagnostics.CodeAnalysis;

namespace Sibz.ListElement
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ListElementOptions : ListElementOptionsInternal
    {
        public new string TemplateName
        {
            get => base.TemplateName;
            set => base.TemplateName = value;
        }

        public new string ItemTemplateName
        {
            get => base.ItemTemplateName;
            set => base.ItemTemplateName = value;
        }

        public new string StyleSheetName
        {
            get => base.StyleSheetName;
            set => base.StyleSheetName = value;
        }

        public new string Label
        {
            get => base.Label;
            set => base.Label = value;
        }

        public new bool HidePropertyLabel
        {
            get => base.HidePropertyLabel;
            set => base.HidePropertyLabel = value;
        }

        public new bool DoNotUseObjectField
        {
            get => base.DoNotUseObjectField;
            set => base.DoNotUseObjectField = value;
        }

        public new bool EnableReordering
        {
            get => base.EnableReordering;
            set => base.EnableReordering = value;
        }

        public new bool EnableDeletions
        {
            get => base.EnableDeletions;
            set => base.EnableDeletions = value;
        }
    }
}