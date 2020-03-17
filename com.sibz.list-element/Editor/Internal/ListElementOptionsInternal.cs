namespace Sibz.ListElement.Internal
{
    public class ListElementOptions
    {
        internal const string DefaultTemplateName = "Sibz.ListElement.Template";
        private const string DefaultItemTemplateName = "Sibz.ListElement.ItemTemplate";
        private const string DefaultStyleSheetName = "Sibz.ListElement.Template";
        private const string DefaultHeaderSectionClassName = "header";
        private const string DefaultHeaderLabelClassName = "header-label";
        private const string DefaultItemsSectionClassName = "items";
        private const string DefaultClearListConfirmSectionClassName = "clear-list-confirm-section";
        private const string DefaultClearListButtonClassName = "clear-list";
        private const string DefaultClearListConfirmButtonClassName = "clear-list-confirm";
        private const string DefaultClearListCancelButtonClassName = "clear-list-cancel";
        private const string DefaultMoveItemUpButtonClassName = "move-up";
        private const string DefaultMoveItemDownButtonClassName = "move-down";
        private const string DefaultRemoveItemButtonClassName = "remove";
        private const string DefaultAddItemSectionClassName = "add-section";
        private const string DefaultAddItemButtonClassName = "add-button";
        private const string DefaultAddItemObjectFieldClassName = "add-field";
        
        public string TemplateName { get; internal set; } = DefaultTemplateName;
        public string ItemTemplateName { get; internal set; } = DefaultItemTemplateName;
        public string StyleSheetName { get; internal set; } = DefaultStyleSheetName;
        public string Label { get; internal set; }
        public bool HidePropertyLabel { get; internal set; } = true;
        public bool DoNotUseObjectField { get; internal set; }
        public bool EnableReordering { get; internal set; } = true;
        public bool EnableDeletions { get; internal set; } = true;

        public string HeaderSectionClassName { get; internal set; } = DefaultHeaderSectionClassName;
        public string HeaderLabelClassName { get; internal set; } = DefaultHeaderLabelClassName;
        public string ItemsSectionClassName { get; internal set; } = DefaultItemsSectionClassName;
        public string ClearListConfirmSectionClassName { get; internal set; } = DefaultClearListConfirmSectionClassName;
        public string ClearListButtonClassName { get; internal set; } = DefaultClearListButtonClassName;
        public string ClearListConfirmButtonClassName { get; internal set; } = DefaultClearListConfirmButtonClassName;
        public string ClearListCancelButtonClassName { get; internal set; } = DefaultClearListCancelButtonClassName;
        public string MoveItemUpButtonClassName { get; internal set; } = DefaultMoveItemUpButtonClassName;
        public string MoveItemDownButtonClassName { get; internal set; } = DefaultMoveItemDownButtonClassName;
        public string RemoveItemButtonClassName { get; internal set; } = DefaultRemoveItemButtonClassName;
        public string AddItemSectionClassName { get; internal set; } = DefaultAddItemSectionClassName;
        public string AddItemButtonClassName { get; internal set; } = DefaultAddItemButtonClassName;
        public string AddItemObjectFieldClassName { get; internal set; } = DefaultAddItemObjectFieldClassName;
    }
}