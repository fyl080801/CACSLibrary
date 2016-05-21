using System;
using System.Windows;
using System.Windows.Media;

namespace CACSLibrary.Silverlight.Maps
{
    public class VectorItem : VectorItemBase
    {
        public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register("Geometry", typeof(Geometry), typeof(VectorItem), new PropertyMetadata(new PropertyChangedCallback(VectorItem.OnGeometryChanged)));

        public Geometry Geometry
        {
            get { return (Geometry)base.GetValue(VectorItem.GeometryProperty); }
            set { base.SetValue(VectorItem.GeometryProperty, value); }
        }

        internal override Geometry DefiningGeometry
        {
            get { return this.Geometry; }
        }

        private static void OnGeometryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }
    }
}
