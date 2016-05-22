using System;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CACSLibrary.Silverlight
{
    public static class Extensions
    {
        public static bool TryCaptureMouse(this UIElement element)
        {
            return element.CaptureMouse();
        }

        public static GeneralTransform CACSTransformToVisual(this UIElement element, UIElement visual)
        {
            GeneralTransform result;
            try
            {
                result = element.TransformToVisual(visual);
            }
            catch
            {
                MatrixTransform matrixTransform = new MatrixTransform();
                matrixTransform.Matrix = Matrix.Identity;
                result = matrixTransform;
            }
            return result;
        }

        public static T New<T>(this Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(new Type[0]);
            if (constructor == null)
            {
                throw new InvalidOperationException(string.Format("Cannot find a default constructor for type {0}", type.FullName));
            }
            return (T)((object)constructor.Invoke(new object[0]));
        }

        public static T New<T>(this Type type, Action<T> initializers)
        {
            T t = type.New<T>();
            if (initializers != null)
            {
                initializers.Invoke(t);
            }
            return t;
        }

        public static double Distance(this Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2.0) + Math.Pow(p2.Y - p1.Y, 2.0));
        }

        internal static DependencyObject GetVisualOrLogicalParent(this DependencyObject obj)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            if (parent != null)
            {
                return parent;
            }
            FrameworkElement frameworkElement = obj as FrameworkElement;
            if (frameworkElement == null)
            {
                return null;
            }
            return frameworkElement.Parent;
        }

        internal static bool IsVisualOrLogicalParentOf(this DependencyObject parent, DependencyObject descendant)
        {
            while (descendant != null)
            {
                if (descendant == parent)
                {
                    return true;
                }
                descendant = descendant.GetVisualOrLogicalParent();
            }
            return false;
        }
    }
}
