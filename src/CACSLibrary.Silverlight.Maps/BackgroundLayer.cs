using System;
using System.Collections.Generic;
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
    public abstract class BackgroundLayer : Control, IMapLayer
    {
        private struct MapState
        {
            public double Zoom;

            public Point Center;
        }

        private VectorPanel _cnv;
        private CACSMaps _map;
        private bool fullRepaint = true;
        private bool dirty;
        private BackgroundLayer.MapState _cache = default(BackgroundLayer.MapState);

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

        internal void InvalidateFull()
        {
            this.fullRepaint = true;
            this.Invalidate();
        }

        internal void Invalidate()
        {
            if (this._cnv != null && this._map != null && this._map.ActualWidth > 0.0 && this._updateCount == 0)
            {
                this.dirty = true;
                this.Update();
            }
        }

        private void Update()
        {
            if (this.dirty)
            {
                UIElementCollection arg_16_0 = this._cnv.Children;
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
                    double num3 = Math.Pow(2.0, zoom) / Math.Pow(2.0, this._cache.Zoom);
                    this._cnv.SetTransformation(num3, num3, num, num2);
                }
                this.dirty = false;
            }
        }

        private void FullRepaint()
        {
            UIElementCollection children = this._cnv.Children;
            double zoom = this._map.Zoom;
            double minsz = this._minSize * this._minSize;
            children.Clear();
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

        //public CACSMaps ParentMaps
        //{
        //    get { return this._parentMap; }
        //    set
        //    {
        //        SizeChangedEventHandler handler = null;
        //        if (this._parentMap != null)
        //        {
        //            this._parentMap.ZoomChanged -= new EventHandler<PropertyChangedEventArgs<double>>(this.OnZoomChanged);
        //            this._parentMap.CenterChanged -= new EventHandler<PropertyChangedEventArgs<Point>>(this.OnCenterChanged);
        //        }
        //        this._parentMap = value;
        //        if (this._parentMap != null)
        //        {
        //            this._parentMap.ZoomChanged += new EventHandler<PropertyChangedEventArgs<double>>(this.OnZoomChanged);
        //            this._parentMap.CenterChanged += new EventHandler<PropertyChangedEventArgs<Point>>(this.OnCenterChanged);
        //            if (handler == null)
        //            {
        //                handler = delegate (object s, SizeChangedEventArgs e)
        //                {
        //                    this.ZoomChanged();
        //                    this.CenterChanged();
        //                };
        //            }
        //            base.SizeChanged += handler;
        //            this.ZoomChanged();
        //            this.CenterChanged();
        //        }
        //    }
        //}

        //public BackgroundLayer()
        //{
        //    base.DefaultStyleKey = typeof(BackgroundLayer);
        //    this.InitBackLayer();
        //}

        //private void InitBackLayer()
        //{
        //    base.SizeChanged += delegate (object s, SizeChangedEventArgs e)
        //    {
        //        this.ZoomChanged();
        //        this.CenterChanged();
        //    };
        //}

        //protected virtual void InitializeRootPart()
        //{
        //    this.CenterChanged();
        //    this.ZoomChanged();
        //}

        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    this._elementRoot = this.GetTemplateChild<Panel>("Root");
        //    if (this._elementRoot != null)
        //    {
        //        this.InitializeRootPart();
        //    }
        //}

        //protected T GetTemplateChild<T>(string childName) where T : class
        //{
        //    DependencyObject templateChild = base.GetTemplateChild(childName);
        //    return templateChild as T;
        //}

        //protected virtual void OnZoomChanged(object sender, PropertyChangedEventArgs<double> e)
        //{
        //    this.ZoomChanged();
        //}

        //protected virtual void OnCenterChanged(object sender, PropertyChangedEventArgs<Point> e)
        //{
        //    this.CenterChanged();
        //}

        //protected abstract void CenterChanged();

        //protected abstract void ZoomChanged();
    }
}
