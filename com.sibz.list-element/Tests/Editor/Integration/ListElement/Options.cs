using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Sibz.ListElement.Internal;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests.Integration.ListElementTests
{
    public class Options
    {
        private const string TEST_LABEL = "Test";
        private const string TEST_TEMPLATE_NAME = "sibz.list-element.tests.template";
        private const string TEST_ITEM_TEMPLATE_NAME = "sibz.list-element.tests.item-template";
        private const string TEST_CHECK_ELEMENT_NAME = "TestCheckElement";

        private const string
            TEST_TEMPLATE_WTH_OPTIONS_SET_NAME = "sibz.list-element.tests.list-element-test-with-options-set";

        private readonly SerializedProperty property = TestHelpers.GetProperty();

        private readonly ListOptions defaults = new ListOptions();

        private readonly ListOptions testSet = new ListOptions
        {
            Label = TEST_LABEL,
            EnableDeletions = false,
            EnableReordering = false,
            HidePropertyLabel = false,
            DoNotUseObjectField = true,
            TemplateName = TEST_TEMPLATE_NAME,
            StyleSheetName = TEST_TEMPLATE_NAME,
            ItemTemplateName = TEST_ITEM_TEMPLATE_NAME
        };

        private static readonly IEnumerable<PropertyInfo> PropertyInfos = typeof(ListOptions).GetProperties();

        private static List<PropertyInfo> CompareSetsAreEqual(ReadOnlyOptions a, ListOptions b,
            PropertyInfo ignore = null)
            => CompareSetsAreEqual(GetListOptions(a), b, ignore);

        private static List<PropertyInfo> CompareSetsAreEqual(ListOptions a, ListOptions b, PropertyInfo ignore = null)
        {
            List<PropertyInfo> results = new List<PropertyInfo>();
            //Debug.Log("Comparing sets are equal");
            foreach (PropertyInfo propertyInfo in PropertyInfos)
            {
                //Debug.Log(ignore);
                if (propertyInfo != ignore && !ComparePropertiesAreEqual(propertyInfo, a, b))
                {
                    results.Add(propertyInfo);
                }
            }

            return results;
        }

        private static ListOptions GetListOptions(ReadOnlyOptions inOptions)
        {
            return inOptions.GetType()
                       .GetField("BaseOptions", BindingFlags.Instance | BindingFlags.NonPublic)?
                       .GetValue(inOptions) as ListOptions ?? throw new Exception("Could not get BaseOptionsField");
        }

        private static bool ComparePropertiesAreEqual(PropertyInfo propInfo, ReadOnlyOptions obj1, object obj2)
            => ComparePropertiesAreEqual(propInfo, GetListOptions(obj1), obj2);

        private static bool ComparePropertiesAreEqual(PropertyInfo propInfo, object obj1, object obj2)
        {
            object val1 = propInfo.GetValue(obj1);
            object val2 = propInfo.GetValue(obj2);

            //Debug.Log($"{val1}:{val2}");

            return val1 is null && val2 is null || (val1?.Equals(val2) ?? false);
        }

        [Test]
        public void ShouldHaveDefaultOptionsSet([ValueSource(nameof(PropertyInfos))] PropertyInfo propInfo)
        {
            ListElement test = new ListElement();

            if (!ComparePropertiesAreEqual(propInfo, test.Options, defaults))
            {
                Assert.Fail($"Property was not default: {propInfo.Name}");
            }
        }

        [Test]
        public void WhenOneOptionIsSet_ShouldNotModifyOthers([ValueSource(nameof(PropertyInfos))] PropertyInfo propInfo)
        {
            ListOptions testOptions = new ListOptions();
            propInfo.SetValue(testOptions, propInfo.GetValue(testSet));
            ListElement test = new ListElement(TestHelpers.GetProperty(), testOptions);
            var results = CompareSetsAreEqual(test.Options, defaults, propInfo);

            if (results.Count > 0)
            {
                string errorLine = string.Join("\n",
                    results.Select(x =>
                        $"{x.Name} - '{x.GetValue(GetListOptions(test.Options))}' was not default of '{x.GetValue(defaults)}'"));
                Assert.Fail($"Modifying '{propInfo.Name}' & Other fields were modified (or not default):\n" +
                            $"{errorLine}");
            }
        }

        [Test]
        public void ShouldBeSetFromFromUxml()
        {
            VisualElement test = new VisualElement();
            SingleAssetLoader.Load<VisualTreeAsset>(TEST_TEMPLATE_WTH_OPTIONS_SET_NAME).CloneTree(test);

            ListElement listElement = test.Q<ListElement>();
            listElement.BindProperty(TestHelpers.GetProperty());
            //yield return null;
            var results = CompareSetsAreEqual(listElement.Options, testSet);

            if (results.Count > 0)
            {
                string errorLine = string.Join("\n",
                    results.Select(x =>
                        $"{x.Name} was set to '{x.GetValue(GetListOptions(listElement.Options))}', should be '{x.GetValue(testSet)}'"));
                Assert.Fail($"Following options were not set correctly from UXML:\n{errorLine}");
            }
        }

        [Test]
        public void ShouldApplyCustomTemplate()
        {
            ListElement testElement =
                new ListElement(property, new ListOptions {TemplateName = TEST_TEMPLATE_NAME});
            Assert.IsNotNull(
                testElement.Q<VisualElement>(TEST_CHECK_ELEMENT_NAME));
        }

        [Test]
        public void ShouldApplyCustomItemTemplate()
        {
            ListElement testElement =
                new ListElement(property, new ListOptions {ItemTemplateName = TEST_ITEM_TEMPLATE_NAME});
            WindowFixture.RootElement.Add(testElement);
            WindowFixture.RootElement.Remove(testElement);
            Assert.IsNotNull(
                testElement.Q<VisualElement>(TEST_CHECK_ELEMENT_NAME));
        }
    }
}