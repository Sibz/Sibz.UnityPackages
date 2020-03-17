using UnityEngine.UIElements;

namespace Sibz.ListElement.UxmlHelpers
{
    public class Util
    {
        public static VisualElement GetRootElement(VisualElement element)
        {
            VisualElement root = element;
            while (root.parent != null)
            {
                root = root.parent;
            }

            return root;
        }
    }
}