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
    public class LocationCanvas : LayerCanvas
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this._map != null && this._map.ViewportWidth != 0.0)
            {
                double avScale = this._map.ActualWidth / this._map.ViewportWidth;
                Point point = this._map.Projection.Project(this._map.Center);
                double wScale = this._map.ActualWidth / 2.0 - point.X * avScale;
                double hScale = this._map.ActualHeight / 2.0 - point.Y * avScale;
                foreach (UIElement uiElement in base.Children)
                {
                    if (uiElement != null)
                    {
                        Point coordinate = LayerCanvas.GetPoint(uiElement, LayerCanvas.CoordinateProperty);
                        Point pinpoint = LayerCanvas.GetPoint(uiElement, LayerCanvas.PinpointProperty);
                        Point projection = this._map.Projection.Project(coordinate);
                        if (double.IsNaN(projection.X) || double.IsNaN(projection.Y))
                        {
                            projection.X = (projection.Y = -32000.0);
                        }
                        uiElement.Arrange(new Rect(
                            projection.X * avScale + wScale - pinpoint.X,
                            projection.Y * avScale + hScale - pinpoint.Y,
                            uiElement.DesiredSize.Width,
                            uiElement.DesiredSize.Height));
                    }
                }
            }
            return finalSize;
        }
    }
}
