using UnityEngine.UIElements;

namespace Assets.Scripts.Extensions
{
    public static class VisualElementExtensions
    {
        public static T AddClass<T>(this T element, params string[] classes) where T : VisualElement
        {
            foreach (var @class in classes)
            {
                if (string.IsNullOrEmpty(@class)) continue;
                element.AddToClassList(@class);
            }

            return element;
        }

        public static T AddTo<T>(this T child, VisualElement parent) where T : VisualElement
        {
            parent.Add(child);
            return child;
        }

        public static VisualElement CreateChild(this VisualElement parent, params string[] classes)
        {
            var child = new VisualElement();
            child.AssignChild(parent, classes);

            return child;
        }

        public static T CreateChild<T>(this VisualElement parent, params string[] classes)
            where T : VisualElement, new()
        {
            var child = new T();
            child.AssignChild(parent, classes);

            return child;
        }

        public static T WithManipulator<T>(this T element, IManipulator manipulator) where T : VisualElement
        {
            element.AddManipulator(manipulator);
            return element;
        }

        private static void AssignChild<T>(this T child, VisualElement parent, params string[] classes) where T : VisualElement
        {
            child.AddClass(classes).AddTo(parent);
        }
    }
}
