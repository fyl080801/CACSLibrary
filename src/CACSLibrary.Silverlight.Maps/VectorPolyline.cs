using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CACSLibrary.Silverlight.Maps
{
    public class VectorPolyline : VectorItemBase
    {
        private Polyline _pline;
        private Rect _bnds = Rect.Empty;
        private PointConverter _pc;
        private Rect _gbnds = Rect.Empty;
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(VectorPolyline), new PropertyMetadata(new PropertyChangedCallback(VectorPolyline.OnPointsChanged)));
        public static readonly DependencyProperty PointsSourceProperty = DependencyProperty.Register("PointsSource", typeof(IEnumerable<Point>), typeof(VectorPolyline), new PropertyMetadata(new PropertyChangedCallback(VectorPolyline.OnPointsSourceChanged)));
        private Path _path;
        private PathGeometry _pg;

        public PointCollection Points
        {
            get { return (PointCollection)base.GetValue(VectorPolyline.PointsProperty); }
            set { base.SetValue(VectorPolyline.PointsProperty, value); }
        }

        public IEnumerable<Point> PointsSource
        {
            get { return (IEnumerable<Point>)base.GetValue(VectorPolyline.PointsSourceProperty); }
            set { base.SetValue(VectorPolyline.PointsSourceProperty, value); }
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

        protected internal override Rect GetScreenBounds()
        {
            return this._bnds;
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

        internal override Rect UpdateShape(PointConverter pc)
        {
            this._pc = pc;
            if (this._pline == null)
            {
                this._pline = new Polyline();
            }
            if (this._path != null)
            {
                this.ClearShape(this._path);
            }
            base.InitShape(this._pline);
            base.Content = this._pline;
            PointCollection pointCollection = VectorItemBase.ConvertPoints(this.GetPoints(), pc);
            this._bnds = base.GetBounds(pointCollection);
            if (this._bnds.Width > VectorLayer.ClipRect.Width || this._bnds.Height > VectorLayer.ClipRect.Height)
            {
                return this.UpdateClippedShape(pc, VectorLayer.ClipRect);
            }
            this._pline.Points = pointCollection;
            return this._bnds;
        }

        private void ClearShape(Shape shape)
        {
            shape.Fill = null;
            shape.Stroke = null;
            shape.StrokeDashArray = null;
        }

        internal Rect UpdateClippedShape(PointConverter pc, Rect clip)
        {
            if (this._path == null)
            {
                this._path = new Path();
                this._pg = new PathGeometry();
                this._path.Data = this._pg;
            }
            if (this._pline != null)
            {
                this.ClearShape(this._pline);
            }
            base.InitShape(this._path);
            base.Content = this._path;
            Point[] pts = VectorItemBase.ConvertPointsToArray(this.GetPoints(), pc);
            this._pg.Figures.Clear();
            PathFigure[] array = ClippingEngine.CreateClippedLines(pts, clip);
            if (array != null)
            {
                int num = array.Length;
                for (int i = 0; i < num; i++)
                {
                    this._pg.Figures.Add(array[i]);
                }
            }
            this._bnds = this._pg.Bounds;
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
            VectorPolyline vectorPolyline = (VectorPolyline)obj;
            vectorPolyline.UpdateShape();
        }

        private static void OnPointsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorPolyline vectorPolyline = (VectorPolyline)obj;
            if (args.OldValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)args.OldValue).CollectionChanged -= new NotifyCollectionChangedEventHandler(vectorPolyline.PointsChanged);
            }
            vectorPolyline.UpdateShape();
            if (args.NewValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)args.NewValue).CollectionChanged += new NotifyCollectionChangedEventHandler(vectorPolyline.PointsChanged);
            }
        }

        private void PointsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateShape();
        }
    }
}
