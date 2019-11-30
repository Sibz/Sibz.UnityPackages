using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sibz.ChildSubset
{
    public class ChildSubsetUpdater
    {
        private readonly ChildSubset m_ChildSubset;

        private System.Type m_FilterIncludeComponentType;
        private System.Type m_FilterExcludeComponentType;

        public enum FilterType
        {
            Include,
            Exclude
        }

        public ChildSubsetUpdater(ChildSubset childSubset)
        {
            m_ChildSubset = childSubset;
        }

        public void UpdateList()
        {
            List<GameObject> updatedChildList = GetChildren();

            if (ChildCountHasChanged(updatedChildList) || ChildListHasChanged(updatedChildList))
            {
                m_ChildSubset.Children.Clear();
                m_ChildSubset.Children.AddRange(updatedChildList);
            }
        }

        public void SetFilter(System.Type componentType, FilterType filterType)
        {
            if (!componentType.IsSubclassOf(typeof(Component)))
            {
                throw new System.ArgumentException("Component type parameter must be a subclass of UnityEnging.Component");
            }

            if (filterType == FilterType.Include)
            {
                m_FilterIncludeComponentType = componentType;
            }
            else if (filterType == FilterType.Exclude)
            {
                m_FilterExcludeComponentType = componentType;
            }
        }

        public List<Component> GetValidFilterOptions()
        {
            return m_ChildSubset.gameObject.GetComponentsInChildren<Component>().ToList();
        }

        public List<GameObject> GetChildren()
        {
            List<GameObject> results = new List<GameObject>();

            foreach (Transform transform in m_ChildSubset.transform)
            {
                if (transform == m_ChildSubset.transform)
                {
                    continue;
                }

                if (m_FilterIncludeComponentType is System.Type && transform.gameObject.GetComponent(m_FilterIncludeComponentType) is null)
                {
                    continue;
                }

                if (m_FilterExcludeComponentType is System.Type && transform.gameObject.GetComponent(m_FilterExcludeComponentType) is Component)
                {
                    continue;
                }

                results.Add(transform.gameObject);
            }

            return results;
        }

        public bool ChildCountHasChanged(List<GameObject> list) => list.Count != m_ChildSubset.Children.Count;

        public bool ChildListHasChanged(List<GameObject> list)
        {
            int length = m_ChildSubset.Children.Count;

            for (int i = 0; i < length; i++)
            {
                if (m_ChildSubset.Children[i] != list[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
}