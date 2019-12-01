using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace Sibz.ChildSubset
{
    [AddComponentMenu("Sibz/Child Subset List")]
    [ExecuteAlways]
    [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
    public class ChildSubset : MonoBehaviour
    {
        public List<GameObject> children = new List<GameObject>();
#if UNITY_EDITOR
        public List<Component> FilterComponents => childSubsetUpdater.GetValidFilterOptions();

        private ChildSubsetUpdater childSubsetUpdater;

        private void Update()
        {
            if (HasDirtyContents(gameObject) && childSubsetUpdater.Update())
            {
                EditorUtility.SetDirty(this);
            }
        }

        private static bool HasDirtyContents(GameObject target)
        {
            for (int i = 0; i < target.transform.childCount; i++)
            {
                if (EditorUtility.IsDirty(target.transform.GetChild(i).gameObject)
                    || EditorUtility.IsDirty(target.transform.GetChild(i))
                    || HasDirtyContents(target.transform.GetChild(i).gameObject))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnEnable()
        {
            if (childSubsetUpdater is null)
            {
                childSubsetUpdater = new ChildSubsetUpdater(this);
            }
        }
#endif
    }
}