

using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sibz.UXMLList
{
    public abstract partial class ListElementsFactoryBase
    {
        #region Controls
        public ControlsClass Controls { get; protected set; }
        public class ControlsClass
        {
            protected ListElementsFactoryBase m_Base;
            public VisualElement HeaderSection => GetOrCreateUsingNested<VisualElement>(nameof(HeaderSection));
            public Label HeaderLabel => GetOrCreateUsingNested<Label>(nameof(HeaderLabel));
            public Button AddButton => GetOrCreateUsingNested<Button>(nameof(AddButton));
            public Button DeleteAllButton => GetOrCreateUsingNested<Button>(nameof(DeleteAllButton));
            public VisualElement DeleteAllConfirmSection => GetOrCreateUsingNested<VisualElement>(nameof(DeleteAllConfirmSection));
            public Label DeleteAllConfirmLabel => GetOrCreateUsingNested<Label>(nameof(DeleteAllConfirmLabel));
            public Button DeleteAllYesButton => GetOrCreateUsingNested<Button>(nameof(DeleteAllYesButton));
            public Button DeleteAllNoButton => GetOrCreateUsingNested<Button>(nameof(DeleteAllNoButton));
            public VisualElement ItemsSection => GetOrCreateUsingNested<VisualElement>(nameof(ItemsSection));

            public PropertyField ItemPropertyField => CreateUsingNested<PropertyField>(nameof(ItemPropertyField));
            public Button NewMoveUpButton => CreateUsingNested<Button>(nameof(NewMoveUpButton));
            public Button NewMoveDownButton => CreateUsingNested<Button>(nameof(NewMoveDownButton));
            public Button NewDeleteItemButton => CreateUsingNested<Button>(nameof(NewDeleteItemButton));

            private readonly System.Reflection.MethodInfo GetOrCreateMethod_INFO;
            private readonly System.Reflection.MethodInfo CreateMethod_INFO;
            private readonly System.Type[] NestedTypes;

            public ControlsClass(ListElementsFactoryBase baseFactory)
            {
                m_Base = baseFactory;
                NestedTypes = m_Base.GetType().GetNestedTypes().ToArray();
                GetOrCreateMethod_INFO = typeof(ListElementsFactoryBase).GetMethod(nameof(ListElementsFactoryBase.GetOrCreateElement), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                CreateMethod_INFO = typeof(ListElementsFactoryBase).GetMethod(nameof(ListElementsFactoryBase.CreateElement), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            }

            public T GetOrCreateUsingNested<T>(string name) where T : VisualElement, new()
            {
                var t = typeof(T);
                var nestedType = NestedTypes.Where(x => x.Name == name && (x.IsSubclassOf(t) || x.IsSubclassOf(t.BaseType))).FirstOrDefault();
                if (nestedType != null)
                {
                    return GetOrCreateMethod_INFO.MakeGenericMethod(nestedType).Invoke(m_Base, new object[1] { name }) as T;
                }
                return m_Base.GetOrCreateElement<T>(name);
            }

            public T CreateUsingNested<T>(string name) where T : VisualElement, new()
            {
                var t = typeof(T);
                var nestedType = NestedTypes.Where(x => x.Name == name && (x.IsSubclassOf(t) || x.IsSubclassOf(t.BaseType))).FirstOrDefault();
                if (nestedType != null)
                {
                    return CreateMethod_INFO.MakeGenericMethod(nestedType).Invoke(m_Base, new object[0]) as T;
                }
                return m_Base.CreateElement<T>();
            }
        }
        #endregion
    }
}
