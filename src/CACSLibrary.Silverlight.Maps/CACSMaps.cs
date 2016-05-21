using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
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

namespace CACSLibrary.Silverlight.Maps
{
    [ContentProperty(LayersElementName), TemplatePart(Name = ToolsElementName, Type = typeof(ToolsLayer)), TemplatePart(Name = LayersElementName, Type = typeof(Panel)), TemplatePart(Name = BackgroundElementName, Type = typeof(ContentControl)), TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]

    public class CACSMaps : Control
    {
        public const double MaxZoom = 20.0;
        public const double MinZoom = 0.0;
        public const double MaxLat = 85.05113;
        public const double MinLat = -85.05113;
        public const double MaxLong = 180.0;
        public const double MinLong = -180.0;
        private const double panSpeed = 5.0;
        internal const string LayersElementName = "Layers";
        internal const string BackgroundElementName = "BackLayer";
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
        internal ContentControl _backContent;
        internal ToolsLayer _elementTools;
        public static readonly DependencyProperty BackLayerProperty = DependencyProperty.Register(BackgroundElementName, typeof(BackgroundLayer), typeof(CACSMaps), new PropertyMetadata(new PropertyChangedCallback(CACSMaps.BackLayerPropertyChanged)));
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
        public static readonly DependencyProperty FocusCuesVisibilityProperty = DependencyProperty.Register("FocusCuesVisibility", typeof(Visibility), typeof(CACSMaps), new PropertyMetadata((Visibility)0));
        public event EventHandler<PropertyChangedEventArgs<Point>> CenterChanged;
        public event EventHandler<PropertyChangedEventArgs<double>> ZoomChanged;
        public event EventHandler<PropertyChangedEventArgs<double>> TargetZoomChanged;
        public event EventHandler<PropertyChangedEventArgs<Point>> TargetCenterChanged;
        public event EventHandler<PropertyChangedEventArgs<bool>> IsMouseOverChanged;
        public event EventHandler BackLayerChanged;

        [Category("cacs")]
        public BackgroundLayer BackLayer
        {
            get { return (BackgroundLayer)base.GetValue(CACSMaps.BackLayerProperty); }
            set { base.SetValue(CACSMaps.BackLayerProperty, value); }
        }

        public Collection<IMapLayer> Layers
        {
            get { return this._layers; }
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

        public Visibility FocusCuesVisibility
        {
            get { return (Visibility)base.GetValue(CACSMaps.FocusCuesVisibilityProperty); }
            set { base.SetValue(CACSMaps.FocusCuesVisibilityProperty, value); }
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
            if (this._elementLayers != null && this.BackLayer != null)
            {
                Rect rect = new Rect(0.0, 0.0, base.ActualWidth, base.ActualHeight);
                UIElement backLayer = this.BackLayer;
                RectangleGeometry rectangleGeometry = new RectangleGeometry();
                rectangleGeometry.Rect = rect;
                backLayer.Clip = rectangleGeometry;
                UIElement elementLayer = this._elementLayers;
                RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
                rectangleGeometry2.Rect = rect;
                elementLayer.Clip = rectangleGeometry2;
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

        private void InitializeBackPart()
        {
            if (this.BackLayer != null)
            {
                this.BackLayer.ParentMaps = this;
                this._backContent.Content = this.BackLayer;
                this._isLoaded = true;
            }
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
            Point point = this.Projection.Project(this.Center);
            Point point2 = this.Projection.Project(latLong);
            return new Point((point2.X - point.X) * num + base.ActualWidth * 0.5, (point2.Y - point.Y) * num + base.ActualHeight * 0.5);
        }

        public Point ScreenToLogic(Point point)
        {
            Point point2 = this.Projection.Project(this.Center);
            double num = this.ViewportWidth / base.ActualWidth;
            return new Point((point.X - base.ActualWidth * 0.5) * num + point2.X, (point.Y - base.ActualHeight * 0.5) * num + point2.Y);
        }

        public Point LogicToScreen(Point point)
        {
            return this.GeographicToScreen(this.Projection.Unproject(point));
        }

        public static double Distance(Point latLong1, Point latLong2)
        {
            double num = 6371000.0;
            double num2 = (latLong2.Y - latLong1.Y) * ProjectionFast.toRad;
            double num3 = (latLong2.X - latLong1.X) * ProjectionFast.toRad;
            double num4 = Math.Sin(num2 * 0.5) * Math.Sin(num2 * 0.5) + Math.Cos(latLong1.Y * ProjectionFast.toRad) * Math.Cos(latLong2.Y * ProjectionFast.toRad) * Math.Sin(num3 * 0.5) * Math.Sin(num3 * 0.5);
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
            this.TargetCenter = new Point(Math.Min(MaxLong, Math.Max(MinLong, this.TargetCenter.X)), Math.Min(MaxLat, Math.Max(MinLat, this.TargetCenter.Y)));
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
            this.Center = new Point(Math.Min(MaxLong, Math.Max(MinLong, this.Center.X)), Math.Min(MaxLat, Math.Max(MinLat, this.Center.Y)));
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
                case (Key)10:
                    point.Y = point.Y - this.ViewportHeight;
                    this.TargetCenter = this.Projection.Unproject(point);
                    break;
                case (Key)11:
                    point.Y = point.Y + this.ViewportHeight;
                    this.TargetCenter = this.Projection.Unproject(point);
                    break;
                case (Key)12:
                    point.X = point.X + this.ViewportWidth;
                    this.TargetCenter = this.Projection.Unproject(point);
                    break;
                case (Key)13:
                    point.X = point.X - this.ViewportWidth;
                    this.TargetCenter = this.Projection.Unproject(point);
                    break;
                case (Key)14:
                    this._panLeft = true;
                    break;
                case (Key)15:
                    this._panUp = true;
                    break;
                case (Key)16:
                    this._panRight = true;
                    break;
                case (Key)17:
                    this._panDown = true;
                    break;
            }
            this.BeginMoving();
        }

        internal void OnKeyUp(CACSKeyEventArgs e)
        {
            switch (e.Key)
            {
                case (Key)14:
                    this._panLeft = false;
                    return;
                case (Key)15:
                    this._panUp = false;
                    return;
                case (Key)16:
                    this._panRight = false;
                    return;
                case (Key)17:
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
            Point point2 = default(Point);
            if (this._panLeft)
            {
                point2.X = -panSpeed;
            }
            else if (this._panRight)
            {
                point2.X = panSpeed;
            }
            if (this._panUp)
            {
                point2.Y = -panSpeed;
            }
            else if (this._panDown)
            {
                point2.Y = panSpeed;
            }
            if (point2 != default(Point))
            {
                this.Center = this.ScreenToGeographic(new Point(base.ActualWidth / 2.0 + point2.X, base.ActualHeight / 2.0 + point2.Y));
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
            CACSMaps cacsMaps = d as CACSMaps;
            Point oldValue = (Point)e.OldValue;
            cacsMaps.OnCenterChanged(oldValue);
            if (cacsMaps.CenterChanged != null && cacsMaps._throwCenterChanged)
            {
                cacsMaps.CenterChanged.Invoke(cacsMaps, new PropertyChangedEventArgs<Point>
                {
                    OldValue = (Point)e.OldValue,
                    NewValue = (Point)e.NewValue
                });
            }
        }

        private static void OnProjectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps cacsMaps = d as CACSMaps;
            IMapProjection oldValue = (IMapProjection)e.OldValue;
            cacsMaps.OnProjectionChanged(oldValue);
        }

        private static void OnZoomPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps cacsMaps = d as CACSMaps;
            double oldValue = (double)e.OldValue;
            cacsMaps.OnZoomChanged(oldValue);
            if (cacsMaps.ZoomChanged != null && cacsMaps._throwZoomChanged)
            {
                cacsMaps.ZoomChanged.Invoke(cacsMaps, new PropertyChangedEventArgs<double>
                {
                    OldValue = (double)e.OldValue,
                    NewValue = (double)e.NewValue
                });
            }
        }

        private static void OnTargetZoomPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps cacsMaps = d as CACSMaps;
            double oldValue = (double)e.OldValue;
            cacsMaps.OnTargetZoomChanged(oldValue);
            if (cacsMaps.TargetZoomChanged != null && cacsMaps._throwTargetZoomChanged)
            {
                cacsMaps.TargetZoomChanged.Invoke(cacsMaps, new PropertyChangedEventArgs<double>
                {
                    OldValue = (double)e.OldValue,
                    NewValue = (double)e.NewValue
                });
            }
        }

        private static void OnShowToolsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps cacsMaps = d as CACSMaps;
            bool oldValue = (bool)e.OldValue;
            cacsMaps.OnShowToolsChanged(oldValue);
        }

        private static void OnTargetZoomSpeedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps cacsMaps = d as CACSMaps;
            double oldValue = (double)e.OldValue;
            cacsMaps.OnTargetZoomSpeedChanged(oldValue);
        }

        private static void OnTargetCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps cacsMaps = d as CACSMaps;
            Point oldValue = (Point)e.OldValue;
            cacsMaps.OnTargetCenterChanged(oldValue);
            if (cacsMaps.TargetCenterChanged != null && cacsMaps._throwTargetCenterChanged)
            {
                cacsMaps.TargetCenterChanged.Invoke(cacsMaps, new PropertyChangedEventArgs<Point>
                {
                    OldValue = (Point)e.OldValue,
                    NewValue = (Point)e.NewValue
                });
            }
        }

        private static void OnTargetCenterSpeedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps cacsMaps = d as CACSMaps;
            double oldValue = (double)e.OldValue;
            cacsMaps.OnTargetCenterSpeedChanged(oldValue);
        }

        private static void OnIsMouseOverPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps cacsMaps = d as CACSMaps;
            if (cacsMaps.IsMouseOverChanged != null && cacsMaps._throwIsMouseOverChanged)
            {
                cacsMaps.IsMouseOverChanged.Invoke(cacsMaps, new PropertyChangedEventArgs<bool>
                {
                    OldValue = (bool)e.OldValue,
                    NewValue = (bool)e.NewValue
                });
            }
            cacsMaps.ChangeVisualStateCommon(true);
        }

        private static void OnForceMouseOverPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps cacsMaps = d as CACSMaps;
            cacsMaps.ChangeVisualStateCommon(true);
        }

        private void OnBackLayerChanged()
        {
            if (this.BackLayer != null)
            {
                this.TargetCenter = new Point(0.0, 0.0);
                this._isLoaded = true;
            }
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
            this._backContent = this.GetTemplateChild<ContentControl>(BackgroundElementName, false, ref empty);
            if (this._backContent != null)
            {
                this.InitializeBackPart();
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

        private static void BackLayerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CACSMaps sender = d as CACSMaps;
            if (sender.BackLayerChanged != null)
            {
                sender.BackLayerChanged(sender, EventArgs.Empty);
            }
            sender.OnBackLayerChanged();
        }
    }
}
