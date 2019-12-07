using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    [CustomEditor(typeof(TestListBehaviour))]
    public class TestListInspector : Editor
    {
        VisualElement m_Root;
        VisualTreeAsset m_VisualTreeAsset;

        TestListBehaviour Target => (TestListBehaviour)target;

        private const string
            UXML_FILTER = "ListTest t:VisualTreeAsset";

        public override VisualElement CreateInspectorGUI()
        {
            m_Root.Bind(serializedObject);

            //m_Root.Q<Button>("but").clicked += () => Target.SecondList.Add(DateTime.Now.ToLocalTime().ToString());
           // m_Root.Q<Button>("but2").clicked += () => Target.TheList.Add(new TestItem() { Test = DateTime.Now.ToLocalTime().ToString() });
            return m_Root;
        }


        public void OnEnable()
        {
            m_Root = new VisualElement();
            m_Root.Clear();
            // Import UXML

            var x = AssetDatabase.FindAssets(UXML_FILTER);

            if (x.Length > 0)
            {
                m_VisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(x[0]));
            }

            if (m_VisualTreeAsset is VisualTreeAsset)
            {
                m_VisualTreeAsset.CloneTree(m_Root);
            }
            else
            {
                Debug.LogWarning($"{nameof(TestListInspector)}: Unable to load {UXML_FILTER}");
            }
        }
    }
}
