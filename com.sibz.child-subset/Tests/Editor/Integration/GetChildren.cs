using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sibz.ChildSubset.Tests.Integration
{
    public class GetChildren
    {

        [Test]
        public void WhenRootObjectNull_ShouldThrowArgumentNullException()
        {
            Assert.Catch<ArgumentNullException>(() => ChildSubset.GetChildren(null));
        }

        [Test]
        public void WhenRootObjectHasNoChildren_ShouldReturnEmptyList()
        {
            GameObject go = Object.Instantiate(new GameObject());
            Assert.AreEqual(0, ChildSubset.GetChildren(go).Count());
            Object.DestroyImmediate(go);
        }

        [Test]
        public void WhenRootObjectHasANumberOfChild_ShouldReturnListWithThatCount([Random(1,20,5)] int count)
        {
            GameObject go = Object.Instantiate(new GameObject());

            List<GameObject> children = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                children.Add(Object.Instantiate(new GameObject(), go.transform));
            }
            Assert.AreEqual(count, ChildSubset.GetChildren(go).Count());
            Object.DestroyImmediate(go);
        }

        [Test]
        public void ShouldReturnChildrenRecursivelyByDefault([Random(1,20,5)] int count)
        {
            GameObject go = Object.Instantiate(new GameObject());

            List<GameObject> children = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                children.Add(Object.Instantiate(new GameObject(), go.transform));
                Object.Instantiate(new GameObject(), children[i].transform);
            }
            Assert.AreEqual(count*2, ChildSubset.GetChildren(go).Count());
            Object.DestroyImmediate(go);
        }

        [Test]
        public void WhenRecursiveFalse_ShouldReturnDirectChildrenOnly([Random(1,20,5)] int count)
        {
            GameObject go = Object.Instantiate(new GameObject());

            List<GameObject> children = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                children.Add(Object.Instantiate(new GameObject(), go.transform));
                Object.Instantiate(new GameObject(), children[i].transform);
            }
            Assert.AreEqual(count, ChildSubset.GetChildren(go, null, null, false).Count());
            Object.DestroyImmediate(go);
        }

        [Test]
        public void WhenRootObjectHasANumberOfChildAndIsFiltered_ShouldReturnListWithFilteredCount([Random(10,20,3)] int count, [Random(1, 10, 2)] int filteredCount)
        {
            GameObject go = Object.Instantiate(new GameObject());

            List<GameObject> children = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                children.Add(Object.Instantiate(new GameObject(i<filteredCount?"Test":null), go.transform));
            }
            Assert.AreEqual(filteredCount, ChildSubset.GetChildren(go, (x)=>x.name.StartsWith("Test")).Count());

            Object.DestroyImmediate(go);
        }
    }
}