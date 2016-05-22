using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CACSLibrary.Silverlight.Maps
{
    [ContentProperty("Children"), StyleTypedProperty(Property = "ItemStyle", StyleTargetType = typeof(VectorItemBase))]
    public class VectorLayer : Control, IMapLayer
    {
        private struct MapState
        {
            public double Zoom;

            public Point Center;
        }

        public class UriSourceFailedEventArgs : EventArgs
        {
            public Exception ErrorException
            {
                get;
                private set;
            }

            public UriSourceFailedEventArgs(Exception error)
            {
                this.ErrorException = error;
            }
        }

        internal const double offsetX = -10000.0;
        internal const double offsetY = -10000.0;
        private ObservableCollection<VectorItemBase> _children;
        private VectorPanel _cnv;
        private ProjectionFast _proj = new ProjectionFast(0.15915494309189535, -0.15915494309189535, 0.5, 0.5, 10000.0, 10000.0);
        private double _minSize = 3.0;
        private List<Rect> _labels = new List<Rect>();
        private bool fullRepaint = true;
        private LabelVisibility _lvis;
        private bool dirty;
        private VectorLayer.MapState _cache = default(VectorLayer.MapState);
        private int _updateCount;
        private bool hasLabels;
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(VectorLayer), new PropertyMetadata(new PropertyChangedCallback(VectorLayer.OnItemTemplateChanged)));
        public static readonly DependencyProperty ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), typeof(VectorLayer), new PropertyMetadata(new PropertyChangedCallback(VectorLayer.OnItemStyleChanged)));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(VectorLayer), new PropertyMetadata(new PropertyChangedCallback(VectorLayer.OnItemsSourceChanged)));
        private CACSMaps _map;
        internal static Rect ClipRect = new Rect(5000.0, 5000.0, 10000.0, 10000.0);
        private bool _needUpdateCollection;
        public event EventHandler<VectorLayer.UriSourceFailedEventArgs> UriSourceFailed;
        public event EventHandler UriSourceLoaded;

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)base.GetValue(VectorLayer.ItemTemplateProperty); }
            set { base.SetValue(VectorLayer.ItemTemplateProperty, value); }
        }

        public Style ItemStyle
        {
            get { return (Style)base.GetValue(VectorLayer.ItemStyleProperty); }
            set { base.SetValue(VectorLayer.ItemStyleProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)base.GetValue(VectorLayer.ItemsSourceProperty); }
            set { base.SetValue(VectorLayer.ItemsSourceProperty, value); }
        }

        public ObservableCollection<VectorItemBase> Children
        {
            get { return this._children; }
        }

        public LabelVisibility LabelVisibility
        {
            get { return this._lvis; }
            set
            {
                this._lvis = value;
                this.fullRepaint = true;
                this.Invalidate();
            }
        }

        public double MinSize
        {
            get { return this._minSize; }
            set
            {
                this._minSize = value;
                this.fullRepaint = true;
                this.Invalidate();
            }
        }

        public CACSMaps ParentMaps
        {
            get { return this._map; }
            set
            {
                if (this._map != value)
                {
                    if (this._map != null)
                    {
                        this._map.ZoomChanged -= new EventHandler<PropertyChangedEventArgs<double>>(this._map_ZoomChanged);
                        this._map.CenterChanged -= new EventHandler<PropertyChangedEventArgs<Point>>(this._map_CenterChanged);
                        this._map.Loaded -= new RoutedEventHandler(this._map_Loaded);
                        this._map.SizeChanged -= new SizeChangedEventHandler(this._map_SizeChanged);
                        this._map.TargetZoomChanged -= new EventHandler<PropertyChangedEventArgs<double>>(this._map_TargetZoomChanged);
                    }
                    this._map = value;
                    if (this._map != null)
                    {
                        this._map.ZoomChanged += new EventHandler<PropertyChangedEventArgs<double>>(this._map_ZoomChanged);
                        this._map.CenterChanged += new EventHandler<PropertyChangedEventArgs<Point>>(this._map_CenterChanged);
                        this._map.Loaded += new RoutedEventHandler(this._map_Loaded);
                        this._map.SizeChanged += new SizeChangedEventHandler(this._map_SizeChanged);
                        this._map.TargetZoomChanged += new EventHandler<PropertyChangedEventArgs<double>>(this._map_TargetZoomChanged);
                        this.InvalidateFull();
                    }
                }
            }
        }

        private int UpdateCount
        {
            get { return this._updateCount; }
            set
            {
                if (value <= 0)
                {
                    if (this._needUpdateCollection)
                    {
                        this._updateCount = 1;
                        this.PerformUpdateCollection(this.ItemsSource, this.ItemTemplate);
                        this._needUpdateCollection = false;
                        this.fullRepaint = true;
                    }
                    this._updateCount = 0;
                    this.Invalidate();
                    return;
                }
                this._updateCount = value;
            }
        }

        public VectorLayer()
        {
            base.DefaultStyleKey = typeof(VectorLayer);
            this._children = new ObservableCollection<VectorItemBase>();
            this._children.CollectionChanged += new NotifyCollectionChangedEventHandler(this._children_CollectionChanged);
            base.Loaded += new RoutedEventHandler(this.VectorLayer_Loaded);
        }

        private void VectorLayer_Loaded(object sender, RoutedEventArgs e)
        {
            this.Invalidate();
        }

        private void OnUriSourceFailed(Exception e)
        {
            if (this.UriSourceFailed != null)
            {
                this.UriSourceFailed.Invoke(this, new VectorLayer.UriSourceFailedEventArgs(e));
            }
        }

        private void OnUriSourceLoaded()
        {
            if (this.UriSourceLoaded != null)
            {
                this.UriSourceLoaded.Invoke(this, EventArgs.Empty);
            }
        }

        public void BeginUpdate()
        {
            this.UpdateCount++;
        }

        public void EndUpdate()
        {
            this.UpdateCount--;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this._cnv != null)
            {
                this._cnv.Children.Clear();
            }
            this._cnv = (base.GetTemplateChild("Canvas") as VectorPanel);
            if (this._cnv == null)
            {
                throw new Exception("Layout canvas is not found in the template");
            }
            this.Invalidate();
        }

        private void _map_TargetZoomChanged(object sender, PropertyChangedEventArgs<double> e)
        {
            this.fullRepaint = true;
        }

        private void _map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateFull();
        }

        private void _map_Loaded(object sender, RoutedEventArgs e)
        {
            this.Invalidate();
        }

        private void _map_CenterChanged(object sender, PropertyChangedEventArgs<Point> e)
        {
            this.Invalidate();
        }

        private void _map_ZoomChanged(object sender, PropertyChangedEventArgs<double> e)
        {
            this.Invalidate();
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.fullRepaint = true;
            if (e.NewItems != null)
            {
                IEnumerator enumerator = e.NewItems.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        VectorItemBase vectorItemBase = (VectorItemBase)enumerator.Current;
                        if (vectorItemBase != null)
                        {
                            vectorItemBase._vl = this;
                            if (vectorItemBase.ReadLocalValue(FrameworkElement.StyleProperty) == DependencyProperty.UnsetValue && this.ItemStyle != null && this.ItemStyle.TargetType != null && this.ItemStyle.TargetType.IsAssignableFrom(vectorItemBase.GetType()))
                            {
                                vectorItemBase.Style = this.ItemStyle;
                            }
                        }
                    }
                }
                finally
                {
                    (enumerator as IDisposable)?.Dispose();
                }
            }
            if (e.OldItems != null)
            {
                IEnumerator enumerator = e.OldItems.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        VectorItemBase vectorItemBase = (VectorItemBase)enumerator.Current;
                        if (vectorItemBase != null)
                        {
                            vectorItemBase._vl = null;
                            if (this._cnv != null)
                            {
                                if (this._cnv.Children.Contains(vectorItemBase))
                                {
                                    this._cnv.Children.Remove(vectorItemBase);
                                }
                                VectorPlacemark vectorPlacemark = vectorItemBase as VectorPlacemark;
                                if (vectorPlacemark != null && vectorPlacemark.LabelUI != null && this._cnv.Children.Contains(vectorPlacemark.LabelUI))
                                {
                                    this._cnv.Children.Remove(vectorPlacemark.LabelUI);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    (enumerator as IDisposable)?.Dispose();
                }
            }
            this.Invalidate();
        }

        private void Update()
        {
            if (this.dirty)
            {
                //UIElementCollection arg_16_0 = this._cnv.Children;
                double zoom = this._map.Zoom;
                Point point = this._map.GeographicToScreen(this._map.Center);
                Point point2 = this._map.GeographicToScreen(this._cache.Center);
                double num = point2.X - point.X;
                double num2 = point2.Y - point.Y;
                if (zoom == this._map.TargetZoom || !this._map.Zooming)
                {
                    if (zoom != this._cache.Zoom || this.fullRepaint || Math.Abs(num) > 0.4 * VectorLayer.ClipRect.Width || Math.Abs(num2) > 0.4 * VectorLayer.ClipRect.Height)
                    {
                        this.FullRepaint();
                    }
                    else
                    {
                        this._cnv.SetTransformation(1.0, 1.0, num, num2);
                    }
                }
                else
                {
                    if (this.hasLabels)
                    {
                        List<UIElement> list = new List<UIElement>();
                        using (IEnumerator<UIElement> enumerator = this._cnv.Children.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                UIElement current = enumerator.Current;
                                if (!(current is VectorItemBase))
                                {
                                    list.Add(current);
                                }
                            }
                        }
                        using (List<UIElement>.Enumerator enumerator2 = list.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                UIElement current2 = enumerator2.Current;
                                this._cnv.Children.Remove(current2);
                            }
                        }
                        this.hasLabels = false;
                    }
                    double num3 = Math.Pow(2.0, zoom) / Math.Pow(2.0, this._cache.Zoom);
                    this._cnv.SetTransformation(num3, num3, num, num2);
                }
                this.dirty = false;
            }
        }

        private static bool Intersect(Rect r1, Rect r2)
        {
            return r1.X <= r2.X + r2.Width && r1.X + r1.Width >= r2.X && r1.Y <= r2.Y + r2.Height && r1.Y + r1.Height >= r2.Y;
        }

        private Point GeoToScreen(Point pt)
        {
            pt = this._map.GeographicToScreen(pt);
            return new Point(pt.X - offsetX, pt.Y - offsetY);
        }

        private void FullRepaint()
        {
            UIElementCollection children = this._cnv.Children;
            double zoom = this._map.Zoom;
            double minsz = this._minSize * this._minSize;
            children.Clear();
            this._labels.Clear();
            this._cache.Zoom = zoom;
            this._cache.Center = this._map.Center;
            PointConverter pc = new PointConverter(this._proj.Project);
            if (this._map.Projection is MercatorProjection)
            {
                this._proj.Init(this._map);
            }
            else
            {
                pc = new PointConverter(this.GeoToScreen);
            }
            IList<VectorItemBase> children2 = this.Children;
            int count = children2.Count;
            Rect clipRect = VectorLayer.ClipRect;
            Point point = this._map.ScreenToGeographic(new Point(VectorLayer.ClipRect.Left + offsetX, VectorLayer.ClipRect.Top + offsetY));
            Point point2 = this._map.ScreenToGeographic(new Point(VectorLayer.ClipRect.Right + offsetX, VectorLayer.ClipRect.Bottom + offsetY));
            Rect r = new Rect(Math.Min(point.X, point.X), Math.Min(point.Y, point2.Y), Math.Abs(point2.X - point.X), Math.Abs(point2.Y - point.Y));
            int i = 0;
            while (i < count)
            {
                VectorItemBase vectorItemBase = children2[i];
                bool flag = vectorItemBase is VectorPlacemark;
                if (flag)
                {
                    vectorItemBase.UpdateShape(pc);
                    Rect screenBounds = vectorItemBase.GetScreenBounds();
                    if (VectorLayer.Intersect(screenBounds, clipRect))
                    {
                        goto IL_1DF;
                    }
                }
                else if (VectorLayer.Intersect(vectorItemBase.Bounds, r))
                {
                    vectorItemBase.GetScreenBounds(pc);
                    goto IL_1DF;
                }
                IL_2E2:
                i++;
                continue;
                IL_1DF:
                if (!vectorItemBase.IsVisible(minsz, zoom))
                {
                    goto IL_2E2;
                }
                if (!flag)
                {
                    vectorItemBase.UpdateShape(pc);
                }
                this._cnv.Children.Add(vectorItemBase);
                VectorPlacemark vectorPlacemark = vectorItemBase as VectorPlacemark;
                if (vectorPlacemark == null || vectorPlacemark.LabelUI == null)
                {
                    goto IL_2E2;
                }
                this.hasLabels = true;
                if (this._lvis == LabelVisibility.Visible)
                {
                    vectorPlacemark.UpdateAutoLabel();
                    this._cnv.Children.Add(vectorPlacemark.LabelUI);
                    goto IL_2E2;
                }
                if (this._lvis != LabelVisibility.AutoHide)
                {
                    goto IL_2E2;
                }
                vectorPlacemark.UpdateAutoLabel();
                Rect labelBounds = vectorPlacemark.LabelBounds;
                bool flag2 = false;
                int count2 = this._labels.Count;
                for (int j = 0; j < count2; j++)
                {
                    Rect rect = this._labels[j];
                    rect.Intersect(labelBounds);
                    if (!rect.IsEmpty)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    this._labels.Add(labelBounds);
                    this._cnv.Children.Add(vectorPlacemark.LabelUI);
                    goto IL_2E2;
                }
                goto IL_2E2;
            }
            this._cnv.ResetTransformation();
            this.fullRepaint = false;
        }

        internal void Invalidate()
        {
            if (this._cnv != null && this._map != null && this._map.ActualWidth > 0.0 && this._updateCount == 0)
            {
                this.dirty = true;
                this.Update();
            }
        }

        internal void InvalidateFull()
        {
            this.fullRepaint = true;
            this.Invalidate();
        }

        private void PerformUpdateCollection(IEnumerable items, DataTemplate itemTemplate)
        {
            this.Children.Clear();
            if (items != null)
            {
                IEnumerable<VectorItemBase> enumerable = items as IEnumerable<VectorItemBase>;
                if (enumerable != null)
                {
                    using (IEnumerator<VectorItemBase> enumerator = enumerable.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            VectorItemBase current = enumerator.Current;
                            this.Children.Add(current);
                        }
                        return;
                    }
                }
                if (itemTemplate != null)
                {
                    VectorItemBase vectorItemBase = itemTemplate.LoadContent() as VectorItemBase;
                    if (vectorItemBase == null)
                    {
                        throw new ArgumentException("The ItemTemplate must define VectorItemBase object");
                    }
                    bool flag = true;
                    IEnumerator enumerator2 = items.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            object current2 = enumerator2.Current;
                            if (!flag)
                            {
                                vectorItemBase = (itemTemplate.LoadContent() as VectorItemBase);
                            }
                            else
                            {
                                flag = false;
                            }
                            vectorItemBase.DataContext = current2;
                            this.Children.Add(vectorItemBase);
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator2 as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                }
            }
        }

        private void UpdateCollection(IEnumerable items, DataTemplate itemTemplate)
        {
            if (this.UpdateCount > 0)
            {
                this._needUpdateCollection = true;
                return;
            }
            this.BeginUpdate();
            this.PerformUpdateCollection(items, itemTemplate);
            this.EndUpdate();
        }

        #region nouse
        //private static void OnUriSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        //{
        //    VectorLayer vectorLayer = (VectorLayer)obj;
        //    vectorLayer.Children.Clear();
        //    if (vectorLayer.UriSource != null)
        //    {
        //        if (!vectorLayer.UriSource.IsAbsoluteUri)
        //        {
        //            Uri uri = vectorLayer.UriSource;
        //            if (!uri.ToString().Contains(";component/"))
        //            {
        //                Assembly assembly = Application.Current.GetType().Assembly;
        //                string text = assembly.FullName.Split(new char[]
        //                {
        //                    ','
        //                })[0];
        //                string text2 = string.Format(CultureInfo.InvariantCulture, "/{0};component/{1}", new object[]
        //                {
        //                    text,
        //                    vectorLayer.UriSource.ToString()
        //                });
        //                uri = new Uri(text2, (UriKind)2);
        //            }
        //            StreamResourceInfo resourceStream = Application.GetResourceStream(uri);
        //            Application.Current.GetType().Assembly.GetManifestResourceNames();
        //            if (resourceStream != null && resourceStream.Stream != null)
        //            {
        //                vectorLayer.ReadFromStream(resourceStream.Stream);
        //                return;
        //            }
        //        }
        //        WebClient webClient = new WebClient();
        //        webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(vectorLayer.wc_OpenReadCompleted);
        //        webClient.OpenReadAsync(vectorLayer.UriSource);
        //    }
        //}

        //private void ReadFromStream(Stream stream)
        //{
        //    if (stream == null)
        //    {
        //        return;
        //    }
        //    using (StreamReader streamReader = new StreamReader(stream))
        //    {
        //        string text = streamReader.ReadToEnd();
        //        if (!string.IsNullOrEmpty(text))
        //        {
        //            List<VectorItemBase> list;
        //            if (text.StartsWith("PK", (StringComparison)5))
        //            {
        //                list = KmzReader.Read(stream);
        //            }
        //            else
        //            {
        //                XElement xElement = XElement.Parse(text);
        //                if (xElement.Element("kml") != null)
        //                {
        //                    list = KmlReader.Read(text);
        //                }
        //                else
        //                {
        //                    list = GeoRssReader.Read(text);
        //                }
        //            }
        //            if (list != null)
        //            {
        //                this.BeginUpdate();
        //                using (List<VectorItemBase>.Enumerator enumerator = list.GetEnumerator())
        //                {
        //                    while (enumerator.MoveNext())
        //                    {
        //                        VectorItemBase current = enumerator.Current;
        //                        this.Children.Add(current);
        //                    }
        //                }
        //                this.EndUpdate();
        //            }
        //        }
        //    }
        //    this.OnUriSourceLoaded();
        //}

        //private void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        //{
        //    if (!e.Cancelled)
        //    {
        //        this.CACSBeginInvoke(delegate
        //        {
        //            if (e.Error == null)
        //            {
        //                this.ReadFromStream(e.Result);
        //                return;
        //            }
        //            this.OnUriSourceFailed(e.Error);
        //        });
        //    }
        //}
        #endregion

        private static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorLayer vectorLayer = (VectorLayer)obj;
            vectorLayer.UpdateCollection(args.NewValue as IEnumerable, vectorLayer.ItemTemplate);
            if (args.OldValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)args.OldValue).CollectionChanged -= new NotifyCollectionChangedEventHandler(vectorLayer.VectorLayer_CollectionChanged);
            }
            if (args.NewValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)args.NewValue).CollectionChanged += new NotifyCollectionChangedEventHandler(vectorLayer.VectorLayer_CollectionChanged);
            }
        }

        private void VectorLayer_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateCollection(this.ItemsSource, this.ItemTemplate);
        }

        private static void OnItemTemplateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorLayer vectorLayer = (VectorLayer)obj;
            vectorLayer.UpdateCollection(vectorLayer.ItemsSource, args.NewValue as DataTemplate);
        }

        private static void OnItemStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorLayer vectorLayer = (VectorLayer)obj;
            int count = vectorLayer.Children.Count;
            for (int i = 0; i < count; i++)
            {
                VectorItemBase vectorItemBase = vectorLayer.Children[i];
                if (vectorItemBase.Style == args.OldValue)
                {
                    Style style = (Style)args.NewValue;
                    if (style.TargetType != null && style.TargetType.IsAssignableFrom(vectorItemBase.GetType()))
                    {
                        vectorItemBase.Style = style;
                    }
                }
            }
            vectorLayer.InvalidateFull();
        }
    }
}
