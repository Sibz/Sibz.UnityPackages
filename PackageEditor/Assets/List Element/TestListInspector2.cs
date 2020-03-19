using Sibz.ListElement;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    [CustomEditor(typeof(TestListBehaviour2))]
    public class TestListInspector2 : Editor
    {
        private VisualElement m_Root;
        private VisualTreeAsset m_VisualTreeAsset;

        private TestListBehaviour2 Target => (TestListBehaviour2) target;

        public override VisualElement CreateInspectorGUI()
        {
            m_Root.Bind(serializedObject);

            //m_Root.Q<Button>("but").clicked += () => Target.SecondList.Add(DateTime.Now.ToLocalTime().ToString());
            //m_Root.Q<Button>("but2").clicked += () => Target.TheList.Add(new TestItem() { Test = DateTime.Now.ToLocalTime().ToString() });
            return m_Root;
        }

        public void OnEnable()
        {
            if (m_Root is null)
            {
                m_Root = new VisualElement();
            }

            m_Root.Clear();
            m_Root.Bind(serializedObject);
            SingleAssetLoader.Load<VisualTreeAsset>("ListTest").CloneTree(m_Root);
            m_Root.Add(new ListElement.ListElement(serializedObject.FindProperty(nameof(TestListBehaviour2.MyList)),
                new ListOptions {HidePropertyLabel = true}));
            m_Root.Add(new ListElement.ListElement(
                serializedObject.FindProperty(nameof(TestListBehaviour2.MyObjectList)),
                new ListOptions {HidePropertyLabel = true}));
        }
    }
}