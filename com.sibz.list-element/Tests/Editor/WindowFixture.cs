using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests
{
    [SetUpFixture]
    public abstract class WindowFixture
    {
        public class TestWindow : EditorWindow
        {
        }

        public static TestWindow Window;
        public static VisualElement RootElement => Window.rootVisualElement;

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