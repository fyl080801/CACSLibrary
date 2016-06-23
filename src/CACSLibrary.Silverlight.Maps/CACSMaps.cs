using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CACSLibrary.Silverlight.Maps
{
    [ContentProperty(LayersElementName)]
    [TemplatePart(Name = ToolsElementName, Type = typeof(ToolsLayer))]
    [TemplatePart(Name = LayersElementName, Type = typeof(Panel))]

    public class CACSMaps : Control
    {
        internal const double MAXZOOM = 20.0;
        internal const double MINZOOM = 0.0;
        internal const double LAT = 180.0;//85.05113;
        internal const double LONG = 180.0;
        private const double panSpeed = 5.0;
        internal const string LayersElementName = "Layers";
        internal const string ToolsElementName = "Tools";
        private CACSMouseHelper _mouseHelper;
        private ObservableCollection<IMapLayer> _layers = new ObservableCollection<IMapLayer>();
        private bool _zooming;
        private bool _centering;
        private Point _zoomPoint;
        private Point _mousePos;
        private bool _panLeft;
        private bool _panRight;
        private bool _panUp;
        private bool _panDown;
        internal bool _isLoaded;
        internal Panel _elementLayers;
        internal ToolsLayer _elementTools;
        public static readonly DependencyProperty LatProperty = DependencyProperty.Register("Lat", typeof(double), typeof(CACSMaps), new PropertyMetadata(CACSMaps.LAT));
        public static readonly DependencyProperty LongProperty = DependencyProperty.Register("Long", typeof(double), typeof(CACSMaps), new PropertyMetadata(CACSMaps.LONG));
        public static readonly DependencyProperty MaxZoomProperty = DependencyProperty.Register("MaxZoom", typeof(double), typeof(CACSMaps), new PropertyMetadata(CACSMaps.MAXZOOM));
        public static readonly DependencyProperty MinZoomProperty = DependencyProperty.Register("MinZoom", typeof(double), typeof(CACSMaps), new PropertyMetadata(CACSMaps.MINZOOM));
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register("Center", typeof(Point), typeof(CACSMaps), new PropertyMetadata(new PropertyChangedCallback(CACSMaps.OnCenterPropertyChanged)));
        private bool _throwCenterChanged = true;
        public static readonly new DependencyProperty ProjectionProperty = DependencyProperty.Register("Projection", typeof(IMapProjection), typeof(CACSMaps), new PropertyMetadata(new PropertyChangedCallback(CACSMaps.OnProjectionPropertyChanged)));
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(CACSMaps), new PropertyMetadata(new PropertyChangedCallback(CACSMaps.OnZoomPropertyChanged)));
        private bool _throwZoomChanged = true;
        public static readonly DependencyProperty TargetZoomProperty = DependencyProperty.Register("TargetZoom", typeof(double), typeof(CACSMaps), new PropertyMetadata(new PropertyChangedCallback(CACSMaps.OnTargetZoomPropertyChanged)));
        private bool _throwTargetZoomChanged = true;
        public static readonly DependencyProperty ShowToolsProperty = DependencyProperty.Register("ShowTools", typeof(bool), typeof(CACSMaps), new PropertyMetadata(true, new PropertyChangedCallback(CACSMaps.OnShowToolsPropertyChanged)));
        public static readonly DependencyProperty TargetZoomSpeedProperty = DependencyProperty.Register("TargetZoomSpeed", typeof(double), typeof(CACSMaps), new PropertyMetadata(0.3, new PropertyChangedCallback(CACSMaps.OnTargetZoomSpeedPropertyChanged)));
        public static readonly DependencyProperty TargetCenterProperty = DependencyProperty.Register("TargetCenter", typeof(Point), typeof(CACSMaps), new PropertyMetadata(new PropertyChangedCallback(CACSMaps.OnTargetCenterPropertyChanged)));
        private bool _throwTargetCenterChanged = true;
        public static readonly DependencyProperty TargetCenterSpeedProperty = DependencyProperty.Register("TargetCenterSpeed", typeof(double), typeof(CACSMaps), new PropertyMetadata(0.3, new PropertyChangedCallback(CACSMaps.OnTargetCenterSpeedPropertyChanged)));
        public static readonly DependencyProperty IsMouseOverProperty = DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(CACSMaps), new PropertyMetadata(new PropertyChangedCallback(CACSMaps.OnIsMouseOverPropertyChanged)));
        private bool _throwIsMouseOverChanged = true;
        internal static readonly DependencyProperty ForceMouseOverProperty = DependencyProperty.Register("ForceMouseOver", typeof(bool), typeof(CACSMaps), new PropertyMetadata(new PropertyChangedCallback(CACSMaps.OnForceMouseOverPropertyChanged)));
        public event EventHandler<PropertyChangedEventArgs<Point>> CenterChanged;
        public event EventHandler<PropertyChangedEventArgs<double>> ZoomChanged;
        public event EventHandler<PropertyChangedEventArgs<double>> TargetZoomChanged;
        public event EventHandler<PropertyChangedEventArgs<Point>> TargetCenterChanged;
        public event EventHandler<PropertyChangedEventArgs<bool>> IsMouseOverChanged;

        //public double ElementWidth { get; set; }

        //public double ElementHeight { get; set; }

        public double MaxZoom
        {
            get { return (double)base.GetValue(CACSMaps.MaxZoomProperty); }
            set { base.SetValue(CACSMaps.MaxZoomProperty, value); }
        }

        public double MinZoom
        {
            get { return (double)base.GetValue(CACSMaps.MinZoomProperty); }
            set { base.SetValue(CACSMaps.MinZoomProperty, value); }
        }

        public Collection<IMapLayer> Layers
        {
            get { return this._layers; }
        }

        public double Lat
        {
            get { return (double)base.GetValue(CACSMaps.LatProperty); }
            set { base.SetValue(CACSMaps.LatProperty, value); }
        }

        public double Long
        {
            get { return (double)base.GetValue(CACSMaps.LongProperty); }
            set { base.SetValue(CACSMaps.LongProperty, value); }
        }

        public double ViewportWidth
        {
            get { return base.ActualWidth / 512.0 / Math.Pow(2.0, this.Zoom); }
        }

        public double ViewportHeight
        {
            get { return this.ViewportWidth * base.ActualHeight / base.ActualWidth; }
        }

        internal bool Zooming
        {
            get { return this._zooming; }
        }

        public Point Center
        {
            get { return (Point)base.GetValue(CACSMaps.CenterProperty); }
            set { base.SetValue(CACSMaps.CenterProperty, value); }
        }

        public new IMapProjection Projection
        {
            get { return (IMapProjection)base.GetValue(CACSMaps.ProjectionProperty); }
            set { base.SetValue(CACSMaps.ProjectionProperty, value); }
        }

        public double Zoom
        {
            get { return (double)base.GetValue(CACSMaps.ZoomProperty); }
            set { base.SetValue(CACSMaps.ZoomProperty, value); }
        }

        public double TargetZoom
        {
            get { return (double)base.GetValue(CACSMaps.TargetZoomProperty); }
            set { base.SetValue(CACSMaps.TargetZoomProperty, value); }
        }

        public bool ShowTools
        {
            get { return (bool)base.GetValue(CACSMaps.ShowToolsProperty); }
            set { base.SetValue(CACSMaps.ShowToolsProperty, value); }
        }

        public double TargetZoomSpeed
        {
            get { return (double)base.GetValue(CACSMaps.TargetZoomSpeedProperty); }
            set { base.SetValue(CACSMaps.TargetZoomSpeedProperty, value); }
        }

        public Point TargetCenter
        {
            get { return (Point)base.GetValue(CACSMaps.TargetCenterProperty); }
            set { base.SetValue(CACSMaps.TargetCenterProperty, value); }
        }

        public double TargetCenterSpeed
        {
            get { return (double)base.GetValue(CACSMaps.TargetCenterSpeedProperty); }
            set { base.SetValue(CACSMaps.TargetCenterSpeedProperty, value); }
        }

        public bool IsMouseOver
        {
            get { return (bool)base.GetValue(CACSMaps.IsMouseOverProperty); }
            internal set { base.SetValue(CACSMaps.IsMouseOverProperty, value); }
        }

        internal bool ForceMouseOver
        {
            get { return (bool)base.GetValue(CACSMaps.ForceMouseOverProperty); }
            set { base.SetValue(CACSMaps.ForceMouseOverProperty, value); }
        }

        private void SetCustomDefaultValues()
        {
            //LicenseHelper.License(typeof(CACSMap), this);
            this.Projection = new MercatorProjection(0.15915494309189535, -0.15915494309189535, 0.5, 0.5);
            this.Center = default(Point);
            base.IsTabStop = true;
            this._mouseHelper = new CACSMouseHelper(this);
            base.MouseLeftButtonDown += (object s, MouseButtonEventArgs e) =>
            {
                base.Focus();
            };
            this._mouseHelper.MouseDragMove += new EventHandler<MouseDragEventArgs>(this.OnDragMove);
            this._mouseHelper.MouseWheel += new EventHandler<MouseWheelEventArgs>(this.OnMouseWheel);
            base.MouseMove += (object s, MouseEventArgs e) =>
            {
                this._mousePos = e.GetPosition(this);
            };
            this._mouseHelper.MouseDoubleClick += new MouseButtonEventHandler(this.OnDoubleClick);
            base.KeyDown += (object s, KeyEventArgs e) =>
            {
                this.OnKeyDown(new CACSKeyEventArgs(e));
            };
            base.KeyUp += (s, e) =>
            {
                this.OnKeyUp(new CACSKeyEventArgs(e));
            };
            this.BeginMoving();
            base.SizeChanged += (object s, SizeChangedEventArgs e) =>
            {
                this.ApplyClip();
            };
        }

        private void OnBeforeApplyTemplate()
        {
            if (this._elementLayers != null)
            {
                this._elementLayers.Children.Clear();
            }
        }

        private void OnAfterApplyTemplate()
        {
            this.ApplyClip();
        }

        private void ApplyClip()
        {
            if (this._elementLayers != null)
            {
                RectangleGeometry rectGeo = new RectangleGeometry
                {
                    Rect = new Rect(0.0, 0.0, base.ActualWidth, base.ActualHeight)
                };
                this._elementLayers.Clip = rectGeo;
            }
        }

        private void InitializeLayersPart()
        {
            using (IEnumerator<IMapLayer> enumerator = this._layers.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    IMapLayer current = enumerator.Current;
                    this.InitializeLayer(current);
                    this._elementLayers.Children.Add(current as UIElement);
                }
            }
            this._layers.CollectionChanged += new NotifyCollectionChangedEventHandler(this.LayersCollectionChanged);
        }

        private void InitializeToolsPart()
        {
            this._elementTools.ParentMaps = this;
            this.OnShowToolsChanged(false);
        }

        public Point ScreenToGeographic(Point point)
        {
            return this.Projection.Unproject(this.ScreenToLogic(point));
        }

        public Point GeographicToScreen(Point latLong)
        {
            double num = base.ActualWidth / this.ViewportWidth;
            Point center = this.Projection.Project(this.Center);
            Point coodinate = this.Projection.Project(latLong);
            return new Point((coodinate.X - center.X) * num + base.ActualWidth * 0.5, (coodinate.Y - center.Y) * num + base.ActualHeight * 0.5);
        }

        public Point ScreenToLogic(Point point)
        {
            Point center = this.Projection.Project(this.Center);
            double num = this.ViewportWidth / base.ActualWidth;
            return new Point((point.X - base.ActualWidth * 0.5) * num + center.X, (point.Y - base.ActualHeight * 0.5) * num + center.Y);
        }

        public Point LogicToScreen(Point point)
        {
            return this.GeographicToScreen(this.Projection.Unproject(point));
        }

        public static double Distance(Point latLong1, Point latLong2)
        {
            double num = 6371000.0;
            double radX = (latLong2.X - latLong1.X) * ProjectionFast.toRad;
            double radY = (latLong2.Y - latLong1.Y) * ProjectionFast.toRad;
            double num4 = Math.Sin(radY * 0.5) * Math.Sin(radY * 0.5) + Math.Cos(latLong1.Y * ProjectionFast.toRad) * Math.Cos(latLong2.Y * ProjectionFast.toRad) * Math.Sin(radX * 0.5) * Math.Sin(radX * 0.5);
            double num5 = 2.0 * Math.Atan2(Math.Sqrt(num4), Math.Sqrt(1.0 - num4));
            return num * num5;
        }

        private void OnTargetZoomChanged(double oldValue)
        {
            this.TargetZoom = Math.Min(MaxZoom, Math.Max(double.IsNaN(this.TargetZoom) ? MinZoom : this.TargetZoom, MinZoom));
            this._zoomPoint = new Point(0.5, 0.5);
            this._zooming = true;
            this.BeginMoving();
        }

        private void OnTargetCenterChanged(Point oldValue)
        {
            var zoomedLong = this.Long != 0 ? this.Long * this.Zoom : double.MaxValue;
            var zoomedLat = this.Lat != 0 ? this.Lat * this.Zoom : double.MaxValue;
            this.TargetCenter = //this.ClaimCenter(this.TargetCenter);
            new Point(
                Math.Min(zoomedLong, Math.Max(-zoomedLong, this.TargetCenter.X)),
                Math.Min(zoomedLat, Math.Max(-zoomedLat, this.TargetCenter.Y)));
            this._centering = true;
            this.BeginMoving();
        }

        private void OnZoomChanged(double oldValue)
        {
            this.Zoom = Math.Min(MaxZoom, Math.Max(double.IsNaN(this.Zoom) ? MinZoom : this.Zoom, MinZoom));
            if (!this._zooming)
            {
                this.TargetZoom = this.Zoom;
                this._zooming = false;
            }
        }

        private void OnCenterChanged(Point oldValue)
        {
            var zoomedLong = this.Long != 0 ? this.Long * this.Zoom : double.MaxValue;
            var zoomedLat = this.Lat != 0 ? this.Lat * this.Zoom : double.MaxValue;
            this.Center = //this.ClaimCenter(this.Center);
            new Point(
                Math.Min(zoomedLong, Math.Max(-zoomedLong, this.Center.X)),
                Math.Min(zoomedLat, Math.Max(-zoomedLat, this.Center.Y)));
            if (!this._centering)
            {
                this.TargetCenter = this.Center;
                this._centering = false;
            }
        }

        private void OnTargetCenterSpeedChanged(double oldValue)
        {
            this.TargetCenterSpeed = Math.Min(1.0, Math.Max(this.TargetCenterSpeed, 0.0));
        }

        private void OnTargetZoomSpeedChanged(double oldValue)
        {
            this.TargetZoomSpeed = Math.Min(1.0, Math.Max(this.TargetZoomSpeed, 0.0));
        }

        private void OnProjectionChanged(IMapProjection oldValue)
        {
            if (this.Projection == null)
            {
                this.Projection = oldValue;
                throw new ArgumentNullException("Projection");
            }
        }

        private void OnShowToolsChanged(bool oldValue)
        {
            if (this._elementTools != null)
            {
                this._elementTools.Visibility = (Visibility)(this.ShowTools ? 0 : 1);
            }
        }

        private void LayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case (NotifyCollectionChangedAction)0:
                    this.InitializeLayer(e.NewItems[0]);
                    this._elementLayers.Children.Insert(e.NewStartingIndex, e.NewItems[0] as UIElement);
                    return;
                case (NotifyCollectionChangedAction)1:
                    this._elementLayers.Children.RemoveAt(e.OldStartingIndex);
                    this.DetachLayer(e.OldItems[0]);
                    return;
                case (NotifyCollectionChangedAction)2:
                    this.InitializeLayer(e.NewItems[0]);
                    this._elementLayers.Children[e.NewStartingIndex] = e.NewItems[0] as UIElement;
                    this.DetachLayer(e.OldItems[0]);
                    return;
                case (NotifyCollectionChangedAction)3:
                    break;
                case (NotifyCollectionChangedAction)4:
                    using (IEnumerator<UIElement> enumerator = this._elementLayers.Children.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            UIElement current = enumerator.Current;
                            this.DetachLayer(current);
                        }
                    }
                    this._elementLayers.Children.Clear();
                    using (IEnumerator<IMapLayer> enumerator2 = this._layers.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            IMapLayer current2 = enumerator2.Current;
                            this.InitializeLayer(e.NewItems[0]);
                            this._elementLayers.Children.Add(current2 as UIElement);
                        }
                    }
                    break;
                default:
                    return;
            }
        }

        private void InitializeLayer(object layer)
        {
            if (!(layer is UIElement))
            {
                throw new ArgumentException("Map layers must inherit from UIElement.");
            }
            IMapLayer mapLayer = layer as IMapLayer;
            if (mapLayer.ParentMaps != this)
            {
                mapLayer.ParentMaps = this;
            }
        }

        private void DetachLayer(object layer)
        {
            IMapLayer mapLayer = layer as IMapLayer;
            if (mapLayer != null)
            {
                mapLayer.ParentMaps = null;
            }
        }

        internal void OnDragMove(object source, MouseDragEventArgs e)
        {
            double deltaX = e.DeltaX;
            double deltaY = e.DeltaY;
            Point point = this.Projection.Project(this.Center);
            this.Center = this.Projection.Unproject(new Point(point.X - deltaX / base.ActualWidth * this.ViewportWidth, point.Y - deltaY / base.ActualHeight * this.ViewportHeight));
        }

        internal void OnMouseWheel(object source, MouseWheelEventArgs e)
        {
            e.Handled = true;
            this.TargetZoom = Math.Round(this.Zoom + (double)((e.Delta > 0.0) ? 1 : -1));
            this._zoomPoint = this._mousePos;
            if (base.ActualWidth != 0.0)
            {
                this._zoomPoint.X = this._zoomPoint.X / base.ActualWidth;
                this._zoomPoint.Y = this._zoomPoint.Y / base.ActualHeight;
            }
            this._zooming = true;
            this.BeginMoving();
        }

        private void OnDoubleClick(object source, MouseButtonEventArgs e)
        {
            e.Handled = true;
            this.TargetZoom = Math.Round(this.Zoom + 1.0);
            this._zoomPoint = e.GetPosition(this);
            if (base.ActualWidth != 0.0)
            {
                this._zoomPoint.X = this._zoomPoint.X / base.ActualWidth;
                this._zoomPoint.Y = this._zoomPoint.Y / base.ActualHeight;
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
            if (e.Text == "+")
            {
                this.TargetZoom = Math.Round(this.Zoom + 1.0);
                this.BeginMoving();
                return;
            }
            if (e.Text == "-")
            {
                this.TargetZoom = Math.Round(this.Zoom - 1.0);
                this.BeginMoving();
            }
        }

        internal void OnKeyDown(CACSKeyEventArgs e)
        {
            Point point = this.Projection.Project(this.TargetCenter);
            switch (e.Key)
            {
                case Key.PageUp:
                    point.Y = point.Y - this.ViewportHeight;
                    this.TargetCenter = this.Projection.Unproject(point);
                    break;
                case Key.PageDown:
                    point.Y = point.Y + this.ViewportHeight;
                    this.TargetCenter = this.Projection.Unproject(point);
                    break;
                case Key.End:
                    point.X = point.X + this.ViewportWidth;
                    this.TargetCenter = this.Projection.Unproject(point);
                    break;
                case Key.Home:
                    //point.X = point.X - this.ViewportWidth;
                    this.TargetCenter = this.Projection.Unproject(new Point(0.5, 0.5));
                    break;
                case Key.Left:
                    this._panLeft = true;
                    break;
                case Key.Up:
                    this._panUp = true;
                    break;
                case Key.Right:
                    this._panRight = true;
                    break;
                case Key.Down:
                    this._panDown = true;
                    break;
            }
            this.BeginMoving();
        }

        internal void OnKeyUp(CACSKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    this._panLeft = false;
                    return;
                case Key.Up:
                    this._panUp = false;
                    return;
                case Key.Right:
                    this._panRight = false;
                    return;
                case Key.Down:
                    this._panDown = false;
                    return;
                default:
                    return;
            }
        }

        private void BeginMoving()
        {
            CompositionTarget.Rendering -= new EventHandler(this.OnMoveTimer);
            CompositionTarget.Rendering += new EventHandler(this.OnMoveTimer);
        }

        private void OnMoveTimer(object source, EventArgs e)
        {
            if (base.ActualWidth == 0.0)
            {
                return;
            }
            if (this._zooming)
            {
                Point point = this.ScreenToLogic(new Point(this._zoomPoint.X * base.ActualWidth, this._zoomPoint.Y * base.ActualHeight));
                if (Math.Abs(this.Zoom - this.TargetZoom) < 0.001)
                {
                    this.Zoom = this.TargetZoom;
                    this._zooming = false;
                }
                else
                {
                    this.Zoom = this.TargetZoom * this.TargetZoomSpeed + this.Zoom * (1.0 - this.TargetZoomSpeed);
                }
                this.Center = this.Projection.Unproject(new Point(point.X - this.ViewportWidth * (this._zoomPoint.X - 0.5), point.Y - this.ViewportHeight * (this._zoomPoint.Y - 0.5)));
                this._centering = false;
            }
            Point panPoint = default(Point);
            if (this._panLeft)
            {
                panPoint.X = -panSpeed;
            }
            else if (this._panRight)
            {
                panPoint.X = panSpeed;
            }
            if (this._panUp)
            {
                panPoint.Y = -panSpeed;
            }
            else if (this._panDown)
            {
                panPoint.Y = panSpeed;
            }
            if (panPoint != default(Point))
            {
                this.Center = this.ScreenToGeographic(new Point(base.ActualWidth / 2.0 + panPoint.X, base.ActualHeight / 2.0 + panPoint.Y));
                this._centering = false;
            }
            if (this._centering)
            {
                this._zooming = false;
                if (Math.Pow(this.TargetCenter.X - this.Center.X, 2.0) + Math.Pow(this.TargetCenter.Y - this.Center.Y, 2.0) < 1.0)
                {
                    this.Center = this.TargetCenter;
                    this._centering = false;
                }
                else
                {
                    this.Center = new Point(this.TargetCenter.X * this.TargetCenterSpeed + this.Center.X * (1.0 - this.TargetCenterSpeed), this.TargetCenter.Y * this.TargetCenterSpeed + this.Center.Y * (1.0 - this.TargetCenterSpeed));
                }
            }
            if (!this._zooming && !this._centering && !this._panLeft && !this._panRight && !this._panUp && !this._panDown)
            {
                CompositionTarget.Rendering -= new EventHandler(this.OnMoveTimer);
            }
        }

        //private Point ClaimCenter(Point center)
        //{
        //    center = this.Projection.Project(center);
        //    var elementAspectRatio = this.ElementWidth / this.ElementHeight;
        //    var logicalBounds = new Rect(0, 0, 1, 1 / elementAspectRatio);
        //    var viewportOrigin = new Point(center.X - 0.5 * this.ViewportWidth, center.Y - 0.5 * this.ViewportHeight);

        //    var pixelScale = this.ElementWidth / this.ViewportWidth;

        //    var left = (logicalBounds.Left - viewportOrigin.X) * pixelScale;
        //    var right = (logicalBounds.Right - viewportOrigin.X) * pixelScale;
        //    if (right - left < this.ElementWidth)
        //    {
        //        center.X = logicalBounds.Left + 0.5 * logicalBounds.Width;
        //    }
        //    else if (left > 0)
        //    {
        //        viewportOrigin.X = logicalBounds.Left;
        //        center.X = viewportOrigin.X + 0.5 * this.ViewportWidth;
        //    }
        //    else if (right < this.ElementWidth)
        //    {
        //        viewportOrigin.X = logicalBounds.Right - this.ElementWidth / pixelScale;
        //        center.X = viewportOrigin.X + 0.5 * this.ViewportWidth;
        //    }

        //    var top = (logicalBounds.Top - viewportOrigin.Y) * pixelScale;
        //    var bottom = (logicalBounds.Bottom - viewportOrigin.Y) * pixelScale;
        //    if (bottom - top < this.ElementHeight)
        //    {
        //        center.Y = logicalBounds.Top + 0.5 * logicalBounds.Height;
        //    }
        //    else if (top > 0)
        //    {
        //        viewportOrigin.Y = logicalBounds.Top;
        //        center.Y = viewportOrigin.Y + 0.5 * this.ViewportWidth / elementAspectRatio;
        //    }
        //    else if (bottom < this.ElementHeight)
        //    {
        //        viewportOrigin.Y = logicalBounds.Bottom - this.ElementHeight / pixelScale;
        //        center.Y = viewportOrigin.Y + 0.5 * this.ViewportWidth / elementAspectRatio;
        //    }
        //    return this.Projection.Unproject(center);
        //}

        protected void ChangeVisualStateCommon(bool useTransitions)
        {
            if (!base.IsEnabled)
            {
                VisualStateHelper.GoToState(this, "Disabled", useTransitions);
            }
            if (base.IsEnabled)
            {
                VisualStateHelper.GoToState(this, "MouseOver", useTransitions);
            }
            if (base.IsEnabled && !this.IsMouseOver && !this.ForceMouseOver)
            {
                VisualStateHelper.GoToState(this, "Normal", useTransitions);
            }
        }

        private static void OnCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps maps = d as CACSMaps;
            Point oldValue = (Point)e.OldValue;
            maps.OnCenterChanged(oldValue);
            if (maps.CenterChanged != null && maps._throwCenterChanged)
            {
                maps.CenterChanged.Invoke(maps, new PropertyChangedEventArgs<Point>
                {
                    OldValue = (Point)e.OldValue,
                    NewValue = (Point)e.NewValue
                });
            }
        }

        private static void OnProjectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps maps = d as CACSMaps;
            IMapProjection oldValue = (IMapProjection)e.OldValue;
            maps.OnProjectionChanged(oldValue);
        }

        private static void OnZoomPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps maps = d as CACSMaps;
            double oldValue = (double)e.OldValue;
            maps.OnZoomChanged(oldValue);
            if (maps.ZoomChanged != null && maps._throwZoomChanged)
            {
                maps.ZoomChanged.Invoke(maps, new PropertyChangedEventArgs<double>
                {
                    OldValue = (double)e.OldValue,
                    NewValue = (double)e.NewValue
                });
            }
        }

        private static void OnTargetZoomPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps maps = d as CACSMaps;
            double oldValue = (double)e.OldValue;
            maps.OnTargetZoomChanged(oldValue);
            if (maps.TargetZoomChanged != null && maps._throwTargetZoomChanged)
            {
                maps.TargetZoomChanged.Invoke(maps, new PropertyChangedEventArgs<double>
                {
                    OldValue = (double)e.OldValue,
                    NewValue = (double)e.NewValue
                });
            }
        }

        private static void OnShowToolsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps maps = d as CACSMaps;
            bool oldValue = (bool)e.OldValue;
            maps.OnShowToolsChanged(oldValue);
        }

        private static void OnTargetZoomSpeedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps maps = d as CACSMaps;
            double oldValue = (double)e.OldValue;
            maps.OnTargetZoomSpeedChanged(oldValue);
        }

        private static void OnTargetCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps maps = d as CACSMaps;
            Point oldValue = (Point)e.OldValue;
            maps.OnTargetCenterChanged(oldValue);
            if (maps.TargetCenterChanged != null && maps._throwTargetCenterChanged)
            {
                maps.TargetCenterChanged.Invoke(maps, new PropertyChangedEventArgs<Point>
                {
                    OldValue = (Point)e.OldValue,
                    NewValue = (Point)e.NewValue
                });
            }
        }

        private static void OnTargetCenterSpeedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps maps = d as CACSMaps;
            double oldValue = (double)e.OldValue;
            maps.OnTargetCenterSpeedChanged(oldValue);
        }

        private static void OnIsMouseOverPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps maps = d as CACSMaps;
            if (maps.IsMouseOverChanged != null && maps._throwIsMouseOverChanged)
            {
                maps.IsMouseOverChanged.Invoke(maps, new PropertyChangedEventArgs<bool>
                {
                    OldValue = (bool)e.OldValue,
                    NewValue = (bool)e.NewValue
                });
            }
            maps.ChangeVisualStateCommon(true);
        }

        private static void OnForceMouseOverPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps maps = d as CACSMaps;
            maps.ChangeVisualStateCommon(true);
        }

        private void CACSControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;
        }

        private void CACSControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = true;
        }

        public CACSMaps()
        {
            base.DefaultStyleKey = typeof(CACSMaps);
            base.Loaded += (s, e) =>
            {
                this.ChangeVisualStateCommon(false);
            };
            base.IsEnabledChanged += (object s, DependencyPropertyChangedEventArgs a) =>
            {
                this.ChangeVisualStateCommon(true);
            };
            base.MouseEnter += new MouseEventHandler(this.CACSControl_MouseEnter);
            base.MouseLeave += new MouseEventHandler(this.CACSControl_MouseLeave);
            this.SetCustomDefaultValues();
        }

        public override void OnApplyTemplate()
        {
            this.OnBeforeApplyTemplate();
            string empty = string.Empty;
            base.OnApplyTemplate();
            this._isLoaded = true;
            this._elementLayers = this.GetTemplateChild<Panel>(LayersElementName, true, ref empty);
            if (this._elementLayers != null)
            {
                this.InitializeLayersPart();
            }
            this._elementTools = this.GetTemplateChild<ToolsLayer>(ToolsElementName, false, ref empty);
            if (this._elementTools != null)
            {
                this.InitializeToolsPart();
            }
            if (!string.IsNullOrEmpty(empty))
            {
                this._isLoaded = false;
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Template cannot be applied to CACSMaps.\nDetails: {0}", new object[]
                    {
                        empty
                    }));
                }
            }
            else
            {
                this.ChangeVisualStateCommon(false);
                this.OnAfterApplyTemplate();
            }
        }

        private T GetTemplateChild<T>(string childName, bool required, ref string errors) where T : class
        {
            DependencyObject templateChild = base.GetTemplateChild(childName);
            ApplyTemplateHelper.VerifyTemplateChild(typeof(T), "template part", childName, templateChild, required, ref errors);
            return templateChild as T;
        }

        private static T GetTemplateChildResource<T>(FrameworkElement root, string resourceName, bool required, ref string errors) where T : class
        {
            object obj = root.Resources[resourceName];
            ApplyTemplateHelper.VerifyTemplateChild(typeof(T), "resource", resourceName, obj, required, ref errors);
            return obj as T;
        }

        private static Storyboard GetTemplateChildResource(FrameworkElement root, string resourceName, bool required, ref string errors)
        {
            return CACSMaps.GetTemplateChildResource<Storyboard>(root, resourceName, required, ref errors);
        }
    }
}
