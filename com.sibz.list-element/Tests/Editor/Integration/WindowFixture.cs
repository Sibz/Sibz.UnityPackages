using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Sibz.ListElement.Tests.Integration
{
    [SetUpFixture]
    public class WindowFixture
    {
        public class TestWindow : EditorWindow
        {
        }

        public static TestWindow Window;

        [OneTimeSetUp]
        public void SetUp()
        {
            Window = EditorWindow.GetWindow<TestWindow>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Window.Close();
            Object.DestroyImmediate(Window);
        }
    }
}