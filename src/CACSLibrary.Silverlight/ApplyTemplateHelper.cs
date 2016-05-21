using System;
using System.Globalization;
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
    public static class ApplyTemplateHelper
    {
        public static void VerifyTemplateChild(Type type, string childType, string childName, object child, bool required, ref string errors)
        {
            if (child == null)
            {
                if (required)
                {
                    errors += string.Format(CultureInfo.InvariantCulture, "\nThe {0} {1} is null!", new object[]
                    {
                        childType,
                        childName
                    });
                    return;
                }
            }
            else if (!type.IsInstanceOfType(child))
            {
                errors += string.Format(CultureInfo.InvariantCulture, "The {0} {1} isn't an instance of {2}!", new object[]
                {
                    childType,
                    childName,
                    type.Name
                });
            }
        }
    }
}
