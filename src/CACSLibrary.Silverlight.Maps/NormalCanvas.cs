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
    public class NormalCanvas : LayerCanvas
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this._map != null && this._map.ViewportWidth != 0.0)
            {
                double avScale = this._map.ActualWidth / this._map.ViewportWidth;
                Point point = this._map.Projection.Project(this._map.Center);
                double wScale = this._map.ActualWidth / 2.0 - point.X * avScale;
                double hScale = this._map.ActualHeight / 2.0 - point.Y * avScale;
                foreach (UIElement element in base.Children)
                {
                    if (element != null)
                    {
                        Point latLong = LayerCanvas.GetPoint(element, LayerCanvas.CoordinateProperty);
                        Point point2 = LayerCanvas.GetPoint(element, LayerCanvas.PinpointProperty);
                        Point point3 = this._map.Projection.Project(latLong);
                        if (double.IsNaN(point3.X) || double.IsNaN(point3.Y))
                        {
                            point3.X = (point3.Y = -32000.0);
                        }
                        element.Arrange(new Rect(point3.X * avScale + wScale - point2.X, point3.Y * avScale + hScale - point2.Y, element.DesiredSize.Width, element.DesiredSize.Height));
                    }
                }
            }
            return finalSize;
        }
    }
}
