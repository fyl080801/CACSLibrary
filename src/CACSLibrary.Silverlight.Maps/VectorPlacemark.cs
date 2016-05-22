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

namespace CACSLibrary.Silverlight.Maps
{
    public class VectorPlacemark : VectorItemBase
    {
        internal override Geometry DefiningGeometry
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal void UpdateAutoLabel()
        {
            throw new NotImplementedException();
        }

        public TextBlock LabelUI
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Rect LabelBounds
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Rect GeometryBounds
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
