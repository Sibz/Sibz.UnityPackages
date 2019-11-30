using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sibz.ChildSubset
{
    [AddComponentMenu("Sibz/Child Subset List")]
    [ExecuteAlways]
    public class ChildSubset : MonoBehaviour
    {
        public List<GameObject> Children = new List<GameObject>();
#if UNITY_EDITOR
        public List<Component> FilterComponents => m_ChildSubsetUpdater.GetValidFilterOptions();

        private ChildSubsetUpdater m_ChildSubsetUpdater;
#endif

        private void Update()
        {
#if UNITY_EDITOR
            m_ChildSubsetUpdater.UpdateList();
#endif
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (m_ChildSubsetUpdater is null)
            {
                m_ChildSubsetUpdater = new ChildSubsetUpdater(this);
            }
#endif
        }

    }
}
