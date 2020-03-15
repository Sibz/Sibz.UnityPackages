using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Sibz.ListElement.Tests
{
    [SetUpFixture]
    public class ListElementTestsFixture
    {
        public static TestWindow Window;

        public class TestWindow : EditorWindow
        {
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Window = EditorWindow.GetWindow<TestWindow>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Window.Close();
            Object.DestroyImmediate(Window);
            Window = null;
        }
    }
}