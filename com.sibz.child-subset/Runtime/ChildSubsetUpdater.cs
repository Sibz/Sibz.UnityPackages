using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Sibz.ChildSubset
{
    [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
    public class ChildSubsetUpdater
    {
        private readonly ChildSubset childSubset;
        private Transform Transform => childSubset.transform;
        private List<GameObject> Children => childSubset.children;

        private System.Type includeComponentTypeFilter;
        private System.Type excludeComponentTypeFilter;

        public ChildSubsetUpdater(ChildSubset childSubset)
        {
            this.childSubset = childSubset;
        }

        public bool Update()
        {
            bool listHasBeenUpdated = false;
            int updateIndex = 0;
            for (int childIndex = 0; childIndex < Transform.childCount; childIndex++)
            {
                GameObject currentChild = Transform.GetChild(childIndex).gameObject;

                if (ValidWithFilterRules(currentChild))
                {
                    listHasBeenUpdated |= AddOrUpdateChildWithIndex(currentChild, updateIndex++);
                }
            }

            if (updateIndex < Children.Count)
            {
                ConcatenateFromIndex(updateIndex);
            }

            return listHasBeenUpdated;
        }

        private bool ValidWithFilterRules(GameObject toValidate)
        {
            return
                (includeComponentTypeFilter == null ||
                 toValidate.GetComponent(excludeComponentTypeFilter)) &&
                (excludeComponentTypeFilter == null ||
                 !toValidate.GetComponent(excludeComponentTypeFilter));
        }

        private bool AddOrUpdateChildWithIndex(GameObject child, int index)
        {
            if (index >= Children.Count)
            {
                Children.Add(child);
            }
            else if (childSubset.children[index] != child)
            {
                Children[index] = child;
            }
            else
            {
                return false;
            }

            return true;
        }

        private void ConcatenateFromIndex(int index)
        {
            int amountToRemove = childSubset.children.Count - index;
            if (amountToRemove > 0)
            {
                childSubset.children.RemoveRange(index, amountToRemove);
            }
        }

        public void SetIncludeFilter(System.Type componentType)
        {
            ThrowIfNotComponentType(componentType);
            includeComponentTypeFilter = componentType;
        }

        public void SetExcludeFilter(System.Type componentType)
        {
            ThrowIfNotComponentType(componentType);
            excludeComponentTypeFilter = componentType;
        }

        private static void ThrowIfNotComponentType(System.Type componentType)
        {
            if (componentType.IsSubclassOf(typeof(Component)))
            {
                return;
            }

            throw new System.ArgumentException(
                "Component type parameter must be a subclass of UnityEngine.Component");
        }

        public List<Component> GetValidFilterOptions()
        {
            return childSubset.GetComponentsInChildren<Component>().ToList();
        }
    }
}