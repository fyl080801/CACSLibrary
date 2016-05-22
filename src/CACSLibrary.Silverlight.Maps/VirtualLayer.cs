using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CACSLibrary.Silverlight.Maps
{
    [TemplatePart(Name = "ItemsHost", Type = typeof(ItemsLayer))]
    public class VirtualLayer : Control, IMapLayer
    {
        private class Region
        {
            public MapSlice Slice { get; set; }

            public int Lat { get; set; }

            public int Long { get; set; }

            public List<object> Elements { get; set; }

            public override bool Equals(object obj)
            {
                VirtualLayer.Region region = obj as VirtualLayer.Region;
                return region != null && (region.Slice == this.Slice && region.Lat == this.Lat) && region.Long == this.Long;
            }

            public override int GetHashCode()
            {
                return this.Lat.GetHashCode() ^ this.Long.GetHashCode() ^ this.Slice.GetHashCode();
            }
        }

        internal const string ItemsHostElementName = "ItemsHost";

        private CACSMaps _maps;

        private ObservableCollection<MapSlice> _slices = new ObservableCollection<MapSlice>();

        private bool _dirtySlices = true;

        private List<VirtualLayer.Region> visibleRegions = new List<VirtualLayer.Region>();

        private IMapVirtualSource _mapItemsSource;

        internal bool _isLoaded;

        internal ItemsLayer _elementItemsHost;

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(VirtualLayer), new PropertyMetadata(new PropertyChangedCallback(VirtualLayer.OnItemTemplatePropertyChanged)));

        public CACSMaps ParentMaps
        {
            get
            {
                return this._maps;
            }
            set
            {
                if (this._maps != null)
                {
                    this._maps.CenterChanged -= new EventHandler<PropertyChangedEventArgs<Point>>(this.OnCenterChanged);
                    this._maps.ZoomChanged -= new EventHandler<PropertyChangedEventArgs<double>>(this.OnZoomChanged);
                    this._maps.SizeChanged -= new SizeChangedEventHandler(this.OnSizeChanged);
                }
                this._maps = value;
                if (this._maps != null)
                {
                    this._maps.CenterChanged += new EventHandler<PropertyChangedEventArgs<Point>>(this.OnCenterChanged);
                    this._maps.ZoomChanged += new EventHandler<PropertyChangedEventArgs<double>>(this.OnZoomChanged);
                    this._maps.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
                    if (this._elementItemsHost != null)
                    {
                        this._elementItemsHost.ParentMaps = this._maps;
                    }
                    this.OnViewChanged();
                }
            }
        }

        public ICollection<MapSlice> Slices
        {
            get
            {
                return this._slices;
            }
        }

        public IMapVirtualSource MapItemsSource
        {
            get
            {
                return this._mapItemsSource;
            }
            set
            {
                this._mapItemsSource = value;
                this.OnViewChanged();
            }
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(VirtualLayer.ItemTemplateProperty);
            }
            set
            {
                base.SetValue(VirtualLayer.ItemTemplateProperty, value);
            }
        }

        private void SetCustomDefaultValues()
        {
            this._slices.CollectionChanged += (object s, NotifyCollectionChangedEventArgs e) =>
            {
                this._dirtySlices = true;
            };
        }

        private void InitializeItemsHostPart()
        {
            if (this._maps != null)
            {
                this._elementItemsHost.ParentMaps = this._maps;
            }
            this._elementItemsHost.ItemTemplate = this.ItemTemplate;
            this.OnViewChanged();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.OnViewChanged();
        }

        private void OnZoomChanged(object sender, PropertyChangedEventArgs<double> e)
        {
            this.OnViewChanged();
        }

        private void OnCenterChanged(object sender, PropertyChangedEventArgs<Point> e)
        {
            this.OnViewChanged();
        }

        public void ForceRefresh()
        {
            if (this._elementItemsHost != null)
            {
                this._elementItemsHost.Items.Clear();
            }
            this.visibleRegions.Clear();
            this.OnViewChanged();
        }

        private void OnItemTemplateChanged(DataTemplate oldValue)
        {
            if (this._elementItemsHost != null)
            {
                this._elementItemsHost.ItemTemplate = this.ItemTemplate;
            }
        }

        private void OnViewChanged()
        {
            if (this.MapItemsSource == null || this._elementItemsHost == null || this._maps == null)
            {
                return;
            }
            this.SortSlices();
            MapSlice mapSlice = Enumerable.FirstOrDefault<MapSlice>(this._slices, (MapSlice s) => s.Zoom <= this._maps.Zoom);
            if (mapSlice == null)
            {
                this._elementItemsHost.Items.Clear();
                this.visibleRegions.Clear();
                return;
            }
            int num = this._slices.IndexOf(mapSlice);
            double maxZoom = (num > 0) ? this._slices[num - 1].Zoom : 20.0;
            Point point;
            Point point2;
            this.GetMapBounds(out point, out point2);
            List<VirtualLayer.Region> list = new List<VirtualLayer.Region>();
            int num2 = (int)(point.X * (double)mapSlice.LongSlices);
            while ((double)num2 < point2.X * (double)mapSlice.LongSlices)
            {
                int num3 = (int)(point.Y * (double)mapSlice.LatSlices);
                while ((double)num3 < point2.Y * (double)mapSlice.LatSlices)
                {
                    list.Add(new VirtualLayer.Region
                    {
                        Slice = mapSlice,
                        Long = num2,
                        Lat = num3,
                        Elements = new List<object>()
                    });
                    num3++;
                }
                num2++;
            }
            using (IEnumerator<object> enumerator = Enumerable.SelectMany<VirtualLayer.Region, object>(Enumerable.Except<VirtualLayer.Region>(this.visibleRegions, list), (VirtualLayer.Region r) => r.Elements).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    this._elementItemsHost.Items.Remove(current);
                }
            }
            IEnumerable<VirtualLayer.Region> enumerable = Enumerable.Except<VirtualLayer.Region>(list, this.visibleRegions);
            this.visibleRegions = Enumerable.ToList<VirtualLayer.Region>(Enumerable.Concat<VirtualLayer.Region>(Enumerable.Intersect<VirtualLayer.Region>(this.visibleRegions, list), enumerable));
            using (IEnumerator<VirtualLayer.Region> enumerator2 = enumerable.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    VirtualLayer.Region current2 = enumerator2.Current;
                    VirtualLayer.Region r = current2;
                    this.MapItemsSource.Request(mapSlice.Zoom, maxZoom, this._maps.Projection.Unproject(new Point((double)r.Long * 1.0 / (double)mapSlice.LongSlices, (double)(r.Lat + 1) * 1.0 / (double)mapSlice.LatSlices)), this._maps.Projection.Unproject(new Point((double)(r.Long + 1) * 1.0 / (double)mapSlice.LongSlices, (double)r.Lat * 1.0 / (double)mapSlice.LatSlices)), delegate (ICollection els)
                    {
                        VirtualLayer.Region region = Enumerable.FirstOrDefault<VirtualLayer.Region>(this.visibleRegions, (VirtualLayer.Region r2) => r.Equals(r2));
                        if (region != null)
                        {
                            IEnumerator enumerator3 = els.GetEnumerator();
                            try
                            {
                                while (enumerator3.MoveNext())
                                {
                                    object current3 = enumerator3.Current;
                                    region.Elements.Add(current3);
                                    this._elementItemsHost.Items.Add(current3);
                                }
                            }
                            finally
                            {
                                IDisposable disposable = enumerator3 as IDisposable;
                                if (disposable != null)
                                {
                                    disposable.Dispose();
                                }
                            }
                        }
                    });
                }
            }
        }

        private void SortSlices()
        {
            if (!this._dirtySlices)
            {
                return;
            }
            List<MapSlice> list = Enumerable.ToList<MapSlice>(Enumerable.OrderByDescending<MapSlice, double>(this._slices, (MapSlice sl) => sl.Zoom));
            this._slices.Clear();
            using (List<MapSlice>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    MapSlice current = enumerator.Current;
                    this._slices.Add(current);
                }
            }
            this.visibleRegions.Clear();
            if (this._elementItemsHost != null)
            {
                this._elementItemsHost.Items.Clear();
            }
            this._dirtySlices = false;
        }

        private void GetMapBounds(out Point lowerLeft, out Point upperRight)
        {
            lowerLeft = this._maps.ScreenToLogic(new Point(0.0, 0.0));
            upperRight = this._maps.ScreenToLogic(new Point(this._maps.ActualWidth, this._maps.ActualHeight));
            lowerLeft.X = Math.Min(1.0, Math.Max(0.0, lowerLeft.X));
            upperRight.X = Math.Min(1.0, Math.Max(0.0, upperRight.X));
            lowerLeft.Y = Math.Min(1.0, Math.Max(0.0, lowerLeft.Y));
            upperRight.Y = Math.Min(1.0, Math.Max(0.0, upperRight.Y));
        }

        private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VirtualLayer virtualLayer = d as VirtualLayer;
            DataTemplate oldValue = (DataTemplate)e.OldValue;
            virtualLayer.OnItemTemplateChanged(oldValue);
        }

        public VirtualLayer()
        {
            base.DefaultStyleKey = typeof(VirtualLayer);
            this.SetCustomDefaultValues();
        }

        public override void OnApplyTemplate()
        {
            string empty = string.Empty;
            base.OnApplyTemplate();
            this._isLoaded = true;
            this._elementItemsHost = this.GetTemplateChild<ItemsLayer>("ItemsHost", true, ref empty);
            if (this._elementItemsHost != null)
            {
                this.InitializeItemsHostPart();
            }
            if (!string.IsNullOrEmpty(empty))
            {
                this._isLoaded = false;
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Template cannot be applied to VirtualLayer.\nDetails: {0}", new object[]
                    {
                        empty
                    }));
                }
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
            return VirtualLayer.GetTemplateChildResource<Storyboard>(root, resourceName, required, ref errors);
        }
    }
}
