using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CACSLibrary.Silverlight.Maps
{
    public class VectorPlacemark : VectorItem
    {
        private Path _path;
        private Rect _bnds = Rect.Empty;
        private Rect _lbnds = default(Rect);
        private UIElement _labelUI;
        private PointConverter _pc;
        public static readonly DependencyProperty LabelPositionProperty = DependencyProperty.Register("LabelPosition", typeof(LabelPosition), typeof(VectorPlacemark), new PropertyMetadata(new PropertyChangedCallback(VectorPlacemark.OnLabelChanged)));
        public static readonly DependencyProperty GeoPointProperty = DependencyProperty.Register("GeoPoint", typeof(Point), typeof(VectorPlacemark), new PropertyMetadata(new PropertyChangedCallback(VectorPlacemark.OnGeoPointChanged)));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(VectorPlacemark), new PropertyMetadata(new PropertyChangedCallback(VectorPlacemark.OnLabelChanged)));
        public static readonly DependencyProperty LabelTemplateProperty = DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(VectorPlacemark), new PropertyMetadata(new PropertyChangedCallback(VectorPlacemark.OnLabelChanged)));
        private Border _bdr;
        private TextBlock _tb;
        private bool _autolbl;
        private static readonly DependencyProperty DataContextInternalProperty = DependencyProperty.Register("DataContextInternal", typeof(object), typeof(VectorPlacemark), new PropertyMetadata(null, new PropertyChangedCallback(VectorPlacemark.OnDataContextInternalChanged)));

        public LabelPosition LabelPosition
        {
            get { return (LabelPosition)base.GetValue(VectorPlacemark.LabelPositionProperty); }
            set { base.SetValue(VectorPlacemark.LabelPositionProperty, value); }
        }

        [Obsolete("Please use GeoPoint property instead.")]
        public Point PinPoint
        {
            get { return (Point)base.GetValue(VectorPlacemark.GeoPointProperty); }
            set { base.SetValue(VectorPlacemark.GeoPointProperty, value); }
        }

        public Point GeoPoint
        {
            get { return (Point)base.GetValue(VectorPlacemark.GeoPointProperty); }
            set { base.SetValue(VectorPlacemark.GeoPointProperty, value); }
        }

        public object Label
        {
            get { return base.GetValue(VectorPlacemark.LabelProperty); }
            set { base.SetValue(VectorPlacemark.LabelProperty, value); }
        }

        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)base.GetValue(VectorPlacemark.LabelTemplateProperty); }
            set { base.SetValue(VectorPlacemark.LabelTemplateProperty, value); }
        }

        public override Rect Bounds
        {
            get { return new Rect(this.GeoPoint, default(Size)); }
        }

        internal Rect GeometryBounds
        {
            get { return this._bnds; }
        }

        internal Rect LabelBounds
        {
            get { return this._lbnds; }
        }

        public UIElement LabelUI
        {
            get { return this._labelUI; }
        }

        internal override Geometry DefiningGeometry
        {
            get { return base.Geometry; }
        }

        private static void OnGeoPointChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorPlacemark vectorPlacemark = (VectorPlacemark)obj;
            if (vectorPlacemark._vl != null)
            {
                vectorPlacemark._vl.InvalidateFull();
            }
        }

        private static void OnLabelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorPlacemark vectorPlacemark = (VectorPlacemark)obj;
            vectorPlacemark.UpdateLabel();
            if (vectorPlacemark._vl != null)
            {
                vectorPlacemark._vl.InvalidateFull();
            }
        }

        protected internal override Rect GetScreenBounds()
        {
            return this._bnds;
        }

        internal override bool IsVisible(double minsz2, double zoom)
        {
            if (base.Geometry == null)
            {
                LOD lod = base.Lod;
                bool result = true;
                if (!lod.IsDefault)
                {
                    result = (zoom >= lod.MinZoom && zoom <= lod.MaxZoom);
                }
                return result;
            }
            return base.IsVisible(minsz2, zoom);
        }

        internal override Rect UpdateShape(PointConverter pc)
        {
            this._pc = pc;
            if (this._path == null)
            {
                this._path = new Path();
                this._path.Data = this.DefiningGeometry;
                base.InitShape(this._path);
            }
            Point point = pc(this.GeoPoint);
            Rect rect = default(Rect);
            if (base.Geometry != null)
            {
                rect = base.Geometry.Bounds;
            }
            rect.X = point.X;
            rect.Y = point.Y;
            this._bnds = rect;
            if (this._labelUI != null)
            {
                switch (this.LabelPosition)
                {
                    case LabelPosition.Center:
                        this._lbnds.X = point.X - 0.5 * this._lbnds.Width;
                        this._lbnds.Y = point.Y - 0.5 * this._lbnds.Height;
                        break;
                    case LabelPosition.Left:
                        this._lbnds.X = point.X - this._lbnds.Width;
                        this._lbnds.Y = point.Y - 0.5 * this._lbnds.Height;
                        break;
                    case LabelPosition.Right:
                        this._lbnds.X = point.X;
                        this._lbnds.Y = point.Y - 0.5 * this._lbnds.Height;
                        break;
                    case LabelPosition.Top:
                        this._lbnds.X = point.X - 0.5 * this._lbnds.Width;
                        this._lbnds.Y = point.Y - this._lbnds.Height;
                        break;
                    case LabelPosition.Bottom:
                        this._lbnds.X = point.X - 0.5 * this._lbnds.Width;
                        this._lbnds.Y = point.Y;
                        break;
                }
                Canvas.SetLeft(this._labelUI, this._lbnds.X);
                Canvas.SetTop(this._labelUI, this._lbnds.Y);
            }
            return rect;
        }

        private void CreateAutoLabel(string s)
        {
            if (this._tb == null)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = s;
                this._tb = textBlock;
            }
            this._tb.Text = s;
            if (this._bdr == null)
            {
                Border border = new Border();
                border.Child = this._tb;
                this._bdr = border;
            }
        }

        internal void UpdateAutoLabel()
        {
            if (this._autolbl)
            {
                if (this._tb != null)
                {
                    this._tb.Foreground = base.Foreground;
                    this._tb.FontFamily = base.FontFamily;
                    this._tb.FontSize = base.FontSize;
                    this._tb.FontStretch = base.FontStretch;
                    this._tb.FontStyle = base.FontStyle;
                    this._tb.FontWeight = base.FontWeight;
                }
                if (this._bdr != null)
                {
                    this._bdr.Background = base.Background;
                }
            }
        }

        private void UpdateLabel()
        {
            FrameworkElement frameworkElement = this._labelUI as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.SizeChanged -= new SizeChangedEventHandler(this.fe_SizeChanged);
            }
            this._labelUI = (this.Label as UIElement);
            this._autolbl = false;
            if (this._labelUI == null && this.LabelTemplate != null)
            {
                this._labelUI = (this.LabelTemplate.LoadContent() as UIElement);
            }
            if (this._labelUI == null && this.Label != null)
            {
                string text = this.Label.ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    this._autolbl = true;
                    this.CreateAutoLabel(text);
                    this.UpdateAutoLabel();
                    this._labelUI = this._bdr;
                }
            }
            if (this._labelUI != null)
            {
                FrameworkElement frameworkElement2 = this._labelUI as FrameworkElement;
                if (frameworkElement2 != null)
                {
                    frameworkElement2.DataContext = base.DataContext;
                    frameworkElement2.SizeChanged += new SizeChangedEventHandler(this.fe_SizeChanged);
                }
                this._labelUI.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (this._labelUI.DesiredSize.Width > 0.0 && this._labelUI.DesiredSize.Height > 0.0)
                {
                    this._lbnds.Width = this._labelUI.DesiredSize.Width;
                    this._lbnds.Height = this._labelUI.DesiredSize.Height;
                    return;
                }
                if (frameworkElement2 != null)
                {
                    this._lbnds.Width = frameworkElement2.ActualWidth;
                    this._lbnds.Height = frameworkElement2.ActualHeight;
                }
            }
        }

        private void fe_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement frameworkElement = sender as FrameworkElement;
            this._labelUI.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (this._labelUI.DesiredSize.Width > 0.0 && this._labelUI.DesiredSize.Height > 0.0)
            {
                this._lbnds.Width = this._labelUI.DesiredSize.Width;
                this._lbnds.Height = this._labelUI.DesiredSize.Height;
            }
            else if (frameworkElement != null)
            {
                this._lbnds.Width = frameworkElement.ActualWidth;
                this._lbnds.Height = frameworkElement.ActualHeight;
            }
            if (this._pc != null && base.Parent is VectorPanel)
            {
                this.UpdateShape(this._pc);
                ((VectorPanel)base.Parent).InvalidateArrange();
            }
        }

        public VectorPlacemark()
        {
            base.SetBinding(VectorPlacemark.DataContextInternalProperty, new Binding());
        }

        private static void OnDataContextInternalChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorPlacemark vectorPlacemark = (VectorPlacemark)obj;
            vectorPlacemark.OnDataContextChanged(obj, args);
        }

        protected virtual void OnDataContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            this.UpdateLabel();
        }
    }
}
