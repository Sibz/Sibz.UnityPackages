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
    [CustomEditor(typeof(TestListBehaviour2))]
    public class TestListInspector2 : Editor
    {
        VisualElement m_Root;
        VisualTreeAsset m_VisualTreeAsset;

        TestListBehaviour2 Target => (TestListBehaviour2)target;

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
                m_Root.Add(new ListElement.ListElement(serializedObject.FindProperty(nameof(TestListBehaviour2.MyList)), new ListElement.ListElement.Config() { HidePropertyLabel = true }));
        }
    }
}
