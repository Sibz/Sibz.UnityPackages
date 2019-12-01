using UnityEditor.SceneManagement;
using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    public class ControlManager
    {
        private Label label;
        private Button deleteAll;
        private Button add;
        private Label deleteConfirm;
        private Button yes;
        private Button no;


        public void RefreshFromTree(VisualElement root)
        {
            label = root.Q<Label>("sibz-list-header-label");
            deleteAll = root.Q<Button>("sibz-list-delete-all-button");
            add = root.Q<Button>("sibz-list-add-button");
            deleteConfirm = root.Q<Label>("sibz-list-delete-all-confirm-label");
            yes = root.Q<Button>("sibz-list-delete-all-yes-button");
            no = root.Q<Button>("sibz-list-delete-all-no-button");
        }
    }
}