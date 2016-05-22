using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public class VectorPolygon : VectorItemBase
    {
        private Polygon _pgon;
        private Rect _bnds = Rect.Empty;
        private PointConverter _pc;
        private Rect _gbnds = Rect.Empty;
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(VectorPolygon), new PropertyMetadata(new PropertyChangedCallback(VectorPolygon.OnPointsChanged)));

        public static readonly DependencyProperty PointsSourceProperty = DependencyProperty.Register("PointsSource", typeof(IEnumerable<Point>), typeof(VectorPolygon), new PropertyMetadata(new PropertyChangedCallback(VectorPolygon.OnPointsSourceChanged)));

        public PointCollection Points
        {
            get { return (PointCollection)base.GetValue(VectorPolygon.PointsProperty); }
            set { base.SetValue(VectorPolygon.PointsProperty, value); }
        }

        public IEnumerable<Point> PointsSource
        {
            get { return (IEnumerable<Point>)base.GetValue(VectorPolygon.PointsSourceProperty); }
            set { base.SetValue(VectorPolygon.PointsSourceProperty, value); }
        }

        public override Rect Bounds
        {
            get { return this._gbnds; }
        }

        internal override Geometry DefiningGeometry
        {
            get { return null; }
        }

        internal IEnumerable<Point> GetPoints()
        {
            if (this.Points != null)
            {
                return this.Points;
            }
            return this.PointsSource;
        }

        internal override Rect GetScreenBounds(PointConverter pc)
        {
            Point pt = new Point(this._gbnds.Left, this._gbnds.Top);
            Point pt2 = new Point(this._gbnds.Right, this._gbnds.Bottom);
            pt = pc(pt);
            pt2 = pc(pt2);
            this._bnds = new Rect(Math.Min(pt.X, pt.X), Math.Min(pt.Y, pt2.Y), Math.Abs(pt2.X - pt.X), Math.Abs(pt2.Y - pt.Y));
            return this._bnds;
        }

        protected internal override Rect GetScreenBounds()
        {
            return this._bnds;
        }

        internal override Rect UpdateShape(PointConverter pc)
        {
            this._pc = pc;
            if (this._pgon == null)
            {
                this._pgon = new Polygon();
                base.InitShape(this._pgon);
            }
            PointCollection pointCollection = VectorItemBase.ConvertPoints(this.GetPoints(), pc);
            this._bnds = base.GetBounds(pointCollection);
            if (this._bnds.Width > VectorLayer.ClipRect.Width || this._bnds.Height > VectorLayer.ClipRect.Height)
            {
                pointCollection = ClippingEngine.ClipPolygon(pointCollection, VectorLayer.ClipRect);
                this._bnds = base.GetBounds(pointCollection);
            }
            this._pgon.Points = pointCollection;
            return this._bnds;
        }

        internal void UpdateShape()
        {
            this._gbnds = base.GetBounds(this.GetPoints());
            if (this._pc != null)
            {
                this.UpdateShape(this._pc);
                if (base.Parent is VectorPanel)
                {
                    ((VectorPanel)base.Parent).InvalidateArrange();
                    return;
                }
            }
            if (this._vl != null)
            {
                this._vl.InvalidateFull();
            }
        }

        private static void OnPointsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorPolygon vectorPolygon = (VectorPolygon)obj;
            vectorPolygon.UpdateShape();
        }

        private static void OnPointsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorPolygon vectorPolygon = (VectorPolygon)obj;
            if (args.OldValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)args.OldValue).CollectionChanged -= new NotifyCollectionChangedEventHandler(vectorPolygon.PointsChanged);
            }
            vectorPolygon.UpdateShape();
            if (args.NewValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)args.NewValue).CollectionChanged += new NotifyCollectionChangedEventHandler(vectorPolygon.PointsChanged);
            }
        }

        private void PointsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateShape();
        }

        public VectorPolygon()
            : base()
        {

        }
    }
}
