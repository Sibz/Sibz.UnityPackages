using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace Sibz.ListElement.Tests
{
    public class ButtonBinderTests
    {
        private const string ClassName = "test-class";
        private const string ClassName2 = "test-class2";
        private VisualElement testElement1, testElement2;
        private Button button1, button2, button1a;
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
            button1a = new Button();
            button1a.AddToClassList(ClassName2);
            testElement1.Add(button1);
            testElement1.Add(button1a);

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

        [Test]
        public void EnumerableBindShouldBindMultipleButtons()
        {
            var binders = new List<ButtonBinder>
            {
                buttonBinder,
                new ButtonBinder(ClassName2, DummyFunctionToCall)
            };
            binders.BindButtons(testElement1);
            Assert.IsTrue(HasFunctionBoundToClicked(button1, nameof(DummyFunctionToCall)));
            Assert.IsTrue(HasFunctionBoundToClicked(button1a, nameof(DummyFunctionToCall)));
        }

        [Test]
        public void EnumerableBindShouldThrowIfAnyButtonDoesntExist()
        {
            var binders = new List<ButtonBinder>
            {
                buttonBinder,
                new ButtonBinder(ClassName2, DummyFunctionToCall)
            };
            bool errorThrown = false;
            try
            {
                binders.BindButtons(testElement2);
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
            {
                throw new ArgumentException("button is null");
            }

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