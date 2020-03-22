using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Sibz.ChildSubset.Tests.Acceptance
{
    public class ChildSubsetTests
    {
        private GameObject rootObject;
        private List<GameObject> children = new List<GameObject>();

        private const int TOTAL_COUNT = 15;
        private const int SPLIT_COUNT = 5;
        private const int RECURSIVE_COUNT = 5;
        private class TestComponent : MonoBehaviour
        {
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            rootObject = Object.Instantiate(new GameObject());
            for (int i = 0; i <TOTAL_COUNT-RECURSIVE_COUNT; i++)
            {
                 children.Add(Object.Instantiate(new GameObject(), rootObject.transform));
                 if (i < SPLIT_COUNT)
                 {
                     children[i].AddComponent<TestComponent>();
                 }
                 else
                 {
                     children[i].name = $"Test - {TOTAL_COUNT - i}";
                     children.Add(Object.Instantiate(new GameObject(), children[i].transform));
                 }
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Object.DestroyImmediate(rootObject);
        }

        [Test]
        public void WhenNoFilter_ShouldRetrieveAllChildren()
        {
            Assert.AreEqual(TOTAL_COUNT,new ChildSubset(rootObject).Count);
        }

        [Test]
        public void WhenFiltered_ShouldOnlyRetrieveSpecificChildren()
        {
            Assert.IsTrue(new ChildSubset(rootObject) { Filter = (x) => x.name.StartsWith("Test") }.List.All(x=>x.name.StartsWith("Test")));
        }

        [Test]
        public void WhenComponentTypeSpecified_ShouldRetrieveOnlyThoseComponents()
        {
            Assert.AreEqual(SPLIT_COUNT, new ChildSubset<TestComponent>(rootObject).Count);
        }

        [Test]
        public void WhenComponentTypeSpecifiedAndFiltered_ShouldRetrieveOnlyThoseComponents()
        {
            Assert.IsTrue(new ChildSubset<TestComponent>(rootObject) { Filter = (x) => x.gameObject.name.StartsWith("Test") }.List.All(x=>x.gameObject.name.StartsWith("Test")));
        }

        [Test]
        public void WhenOrdered_ShouldOrderListCorrectly([Values(ChildSubset.SortDir.Ascending, ChildSubset.SortDir.Descending)]ChildSubset.SortDir order)
        {
            var unsorted = children.Where(x => !string.IsNullOrEmpty(x.name) && x.name.StartsWith("Test"));
            var sortedList = (order == ChildSubset.SortDir.Descending ? unsorted.OrderByDescending(x=>x.name) :  unsorted.OrderBy(x => x.name)).ToArray();

            ChildSubset cs = new ChildSubset(rootObject)
            {
                Filter = (x) => x.gameObject.name.StartsWith("Test"),
                Sort = x=>x.name,
                SortDirection = order
            };

            var results =cs.List.ToArray();
            Assert.AreEqual(sortedList.Length, results.Length);
            for (int i = 0; i < results.Length; i++)
            {
                Assert.AreSame(sortedList[i], results[i]);
            }
        }

        [Test]
        public void WhenRecursiveSet_shouldGetCorrectAmount([Values(true,false)] bool recursive)
        {
            int expectedCount = recursive ? TOTAL_COUNT : TOTAL_COUNT - RECURSIVE_COUNT;
            Assert.AreEqual(expectedCount, new ChildSubset(rootObject){Recursive = recursive}.Count);
        }

    }
}