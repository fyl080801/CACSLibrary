using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CACSLibrary.Silverlight
{
    public static class PlatformIndependent
    {
        public static double DPI = 1.3333333333333333;

        public static T CACSLoadXaml<T>(string strXaml) where T : class
        {
            return (T)((object)XamlReader.Load(strXaml));
        }

        public static void CACSBeginInvoke(this FrameworkElement elem, Action action)
        {
            elem.Dispatcher.BeginInvoke(action);
        }

        public static UIElement CACSGetRootVisual(this UIElement element)
        {
            return Application.Current.RootVisual;
        }

        public static FrameworkElement CACSGetLogicalFocusedElement(this FrameworkElement relativeToElement)
        {
            return FocusManager.GetFocusedElement() as FrameworkElement;
        }

        public static FrameworkElement GetKeyboardFocusedElement()
        {
            return FocusManager.GetFocusedElement() as FrameworkElement;
        }

        public static int CACSGetPlatformKeyCode(this KeyEventArgs e)
        {
            return e.PlatformKeyCode;
        }

        public static ControlTemplate AdjustContentControlTemplate(ControlTemplate template)
        {
            return template;
        }
    }
}
