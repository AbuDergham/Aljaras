using System.Windows;

namespace Aljaras
{
    public static class Extensions
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(object), typeof(Extensions), new PropertyMetadata(default(string)));
        public static void SetIcon(UIElement element, object value)
        {
            element.SetValue(IconProperty, value);
        }
        public static object GetIcon(UIElement element)
        {
            return (object)element.GetValue(IconProperty);
        }
    }
}
