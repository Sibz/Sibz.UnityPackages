using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests
{
    public class ButtonBinderTests
    {
        private const string ClassName = "test-class";
        private VisualElement testElement1, testElement2;
        private Button button1, button2;
        private ButtonBinder buttonBinder;

        private static void DummyFunctionToCall()
        {
        }

        [SetUp]
        public void SetUpTestElements()
        {
            testElement1 = new VisualElement();
            button1 = new Button();
            button1.AddToClassList(ClassName);
            testElement1.Add(button1);

            testElement2 = new VisualElement();
            button2 = new Button();
            button2.AddToClassList(ClassName);
            testElement2.Add(button2);

            buttonBinder = new ButtonBinder(ClassName, DummyFunctionToCall);
        }

        [Test]
        public void ShouldBindToGivenFunction()
        {
            buttonBinder.BindToFunction(testElement1);

            Assert.IsTrue(HasFunctionBoundToClicked(button1, nameof(DummyFunctionToCall)));
        }

        [Test]
        public void ShouldBindNewButton()
        {
            buttonBinder.BindToFunction(testElement1);
            buttonBinder.BindToFunction(testElement2);

            Assert.IsTrue(HasFunctionBoundToClicked(button2, nameof(DummyFunctionToCall)));
        }

        [Test]
        public void ShouldUnbindOldButton()
        {
            buttonBinder.BindToFunction(testElement1);
            buttonBinder.BindToFunction(testElement2);

            Assert.IsFalse(HasFunctionBoundToClicked(button1, nameof(DummyFunctionToCall)));
        }

        [Test]
        public void ShouldThrowIfButtonNotFound()
        {
            bool errorThrown = false;
            try
            {
                buttonBinder.BindToFunction(new VisualElement());
            }
            catch
            {
                errorThrown = true;
            }

            Assert.IsTrue(errorThrown);
        }

        private bool HasFunctionBoundToClicked(Button button, string funcName)
        {
            if (button is null)
                throw new ArgumentException("button is null");

            // The actual clicked event is proxied into the m_Clickable field
            object mClickedValue = typeof(Button)
                .GetField("m_Clickable", BindingFlags.Instance | BindingFlags.NonPublic)?
                .GetValue(button);
            
            // Get the private clicked field from the Clickable m_Clickable member on our button
            MulticastDelegate eventDelegate = (MulticastDelegate) typeof(Clickable)
                .GetField("clicked", BindingFlags.Instance | BindingFlags.NonPublic)?
                .GetValue(mClickedValue);

            var delegates = eventDelegate?.GetInvocationList();

            return delegates != null && delegates.Any(dlg => dlg.Method.Name == funcName);
        }
    }
}