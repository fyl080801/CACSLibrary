using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CACSLibrary.Silverlight.Maps
{
    public class LayerCanvas : Panel
    {
        protected CACSMaps _map;
        public static readonly DependencyProperty CoordinateProperty = DependencyProperty.RegisterAttached("Coordinate", typeof(Point), typeof(LayerCanvas), new PropertyMetadata(new PropertyChangedCallback(LayerCanvas.OnCoordinatePropertyChanged)));
        public static readonly DependencyProperty PinpointProperty = DependencyProperty.RegisterAttached("Pinpoint", typeof(Point), typeof(LayerCanvas), new PropertyMetadata(new PropertyChangedCallback(LayerCanvas.OnPinpointPropertyChanged)));

        public CACSMaps ParentMaps
        {
            get { return this._map; }
            set
            {
                if (this._map != null)
                {
                    this._map.ZoomChanged -= new EventHandler<PropertyChangedEventArgs<double>>(this.OnZoomChanged);
                    this._map.CenterChanged -= new EventHandler<PropertyChangedEventArgs<Point>>(this.OnCenterChanged);
                    this._map.Loaded -= new RoutedEventHandler(this.OnLoaded);
                    this._map.SizeChanged -= new SizeChangedEventHandler(this.OnSizeChanged);
                }
                this._map = value;
                if (this._map != null)
                {
                    this._map.ZoomChanged += new EventHandler<PropertyChangedEventArgs<double>>(this.OnZoomChanged);
                    this._map.CenterChanged += new EventHandler<PropertyChangedEventArgs<Point>>(this.OnCenterChanged);
                    this._map.Loaded += new RoutedEventHandler(this.OnLoaded);
                    this._map.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
                    base.InvalidateArrange();
                }
                this.OnMapChanged();
            }
        }

        protected virtual void OnMapChanged()
        {
        }

        public static Point GetCoordinate(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (Point)element.GetValue(LayerCanvas.CoordinateProperty);
        }

        public static Point GetPinpoint(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (Point)element.GetValue(LayerCanvas.PinpointProperty);
        }

        protected static Point GetPoint(UIElement element, DependencyProperty property)
        {
            Point result;
            if (element.ReadLocalValue(property) != DependencyProperty.UnsetValue)
            {
                result = (Point)element.GetValue(property);
            }
            else
            {
                ContentPresenter reference = element as ContentPresenter;
                if (reference != null && VisualTreeHelper.GetChildrenCount(reference) > 0)
                {
                    result = (Point)VisualTreeHelper.GetChild(reference, 0).GetValue(property);
                }
                else
                {
                    result = default(Point);
                }
            }
            return result;
        }

        private static void Invalidate(DependencyObject element)
        {
            ContentPresenter parent = VisualTreeHelper.GetParent(element) as ContentPresenter;
            if (parent != null)
            {
                LayerCanvas canvas = VisualTreeHelper.GetParent(parent) as LayerCanvas;
                if (canvas != null)
                {
                    canvas.InvalidateArrange();
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in base.Children)
            {
                if (element != null)
                {
                    element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
            }
            return new Size(0.0, 0.0);
        }

        private void OnCenterChanged(object sender, PropertyChangedEventArgs<Point> e)
        {
            base.InvalidateArrange();
        }

        private static void OnCoordinateChanged(DependencyObject element, Point oldValue)
        {
            LayerCanvas.Invalidate(element);
        }

        private static void OnCoordinatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement reference = d as FrameworkElement;
            if (reference != null)
            {
                LayerCanvas parent = VisualTreeHelper.GetParent(reference) as LayerCanvas;
                if (parent != null && !object.Equals(e.OldValue, e.NewValue))
                {
                    parent.InvalidateMeasure();
                }
            }
            LayerCanvas.OnCoordinateChanged(d, (Point)e.OldValue);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.InvalidateArrange();
        }

        private static void OnPinpointChanged(DependencyObject element, Point oldValue)
        {
            LayerCanvas.Invalidate(element);
        }

        private static void OnPinpointPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement reference = d as FrameworkElement;
            if (reference != null)
            {
                LayerCanvas parent = VisualTreeHelper.GetParent(reference) as LayerCanvas;
                if (parent != null && !object.Equals(e.OldValue, e.NewValue))
                {
                    parent.InvalidateMeasure();
                }
            }
            LayerCanvas.OnPinpointChanged(d, (Point)e.OldValue);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.InvalidateArrange();
        }

        private void OnZoomChanged(object sender, PropertyChangedEventArgs<double> e)
        {
            base.InvalidateArrange();
        }

        public static void SetCoordinate(DependencyObject element, Point value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(LayerCanvas.CoordinateProperty, value);
        }

        public static void SetPinpoint(DependencyObject element, Point value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(LayerCanvas.PinpointProperty, value);
        }
    }
}
