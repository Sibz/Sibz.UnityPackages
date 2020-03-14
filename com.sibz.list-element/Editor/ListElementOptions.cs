namespace Sibz.ListElement
{
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
    }
  
}