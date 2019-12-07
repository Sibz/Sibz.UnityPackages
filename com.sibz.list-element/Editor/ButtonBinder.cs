﻿using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Sibz.ListElement
{
    public class ButtonBinder
    {
        private readonly string className;
        private readonly Action function;
        private Button button;

        public ButtonBinder(string className, Action function)
        {
            this.className = className;
            this.function = function;
        }

        public void BindToFunction(VisualElement root)
        {
            if (button != null)
            {
                button.clicked -= function;
            }

            button = root.Q<Button>(null, className);
            if (button is null)
            {
                throw new ArgumentException($"Element did not contain {nameof(Button)} with classname {className}");
            }

            button.clicked += function;
        }
    }

    public static class ButtonBinderEnumeratorExtensions
    {
        public static void BindButtons(this IEnumerable<ButtonBinder> buttonBinder, VisualElement root)
        {
            foreach (ButtonBinder binder in buttonBinder)
            {
                binder.BindToFunction(root);
            }
        }
    }
}