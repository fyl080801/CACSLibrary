using System;
using System.Net;
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
    }
}
