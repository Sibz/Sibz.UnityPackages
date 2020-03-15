namespace Sibz.ListElement
{
    public class ListElementOptionsInternal
    {
        internal const string DefaultTemplateName = "Sibz.ListElement.Template";
        private const string DefaultItemTemplateName = "Sibz.ListElement.ItemTemplate";
        private const string DefaultStyleSheetName = "Sibz.ListElement.Template";
        public const string HidePropertyLabelStyleSheetName = "Sibz.ListElement.Hide-Property-Label";
        private const string DefaultHeaderSectionClassName = "sibz-list-header";
        private const string DefaultHeaderLabelClassName = "sibz-list-header-label";
        private const string DefaultItemsSectionClassName = "sibz-list-items-section";
        private const string DefaultClearListConfirmSectionClassName = "sibz-list-delete-all-confirm";
        private const string DefaultClearListButtonClassName = "sibz-list-delete-all-button";
        private const string DefaultClearListConfirmButtonClassName = "sibz-list-delete-confirm-yes";
        private const string DefaultClearListCancelButtonClassName = "sibz-list-delete-confirm-no";
        private const string DefaultMoveItemUpButtonClassName = "sibz-list-move-up-button";
        private const string DefaultMoveItemDownButtonClassName = "sibz-list-move-down-button";
        private const string DefaultRemoveItemButtonClassName = "sibz-list-delete-item-button";
        private const string DefaultAddItemSectionClassName = "sibz-list-add-section";
        private const string DefaultAddItemButtonClassName = "sibz-list-add-button";
        private const string DefaultAddItemObjectFieldClassName = "sibz-list-add-item-object-field";

        public string TemplateName { get; internal set; } = DefaultTemplateName;
        public string ItemTemplateName { get; internal set; } = DefaultItemTemplateName;
        public string StyleSheetName { get; internal set; } = DefaultStyleSheetName;
        public string Label { get; internal set; }
        public bool HidePropertyLabel { get; internal set; } = true;
        public bool DoNotUseObjectField { get; internal set; }

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