using System;
using System.Collections.Generic;
using System.Globalization;
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
    public abstract class VectorItemBase : UserControl
    {
        private const double OptRad = 1.5;
        private Path _path;
        private Geometry _rg;
        internal VectorLayer _vl;
        public static readonly DependencyProperty LodProperty = DependencyProperty.Register("Lod", typeof(LOD), typeof(VectorItemBase), new PropertyMetadata(new PropertyChangedCallback(VectorItemBase.OnLodChanged)));
        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(VectorItemBase), new PropertyMetadata(new PropertyChangedCallback(VectorItemBase.OnFillChanged)));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(VectorItemBase), new PropertyMetadata(new PropertyChangedCallback(VectorItemBase.OnStrokeChanged)));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(VectorItemBase), new PropertyMetadata(1.0, new PropertyChangedCallback(VectorItemBase.OnStrokeThicknessChanged)));
        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(VectorItemBase), new PropertyMetadata(new PropertyChangedCallback(VectorItemBase.OnStrokeDashArrayChanged)));
        public static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(VectorItemBase), new PropertyMetadata(new PropertyChangedCallback(VectorItemBase.OnStrokeStartLineCapChanged)));
        public static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(VectorItemBase), new PropertyMetadata(new PropertyChangedCallback(VectorItemBase.OnStrokeEndLineCapChanged)));
        public static readonly DependencyProperty StrokeDashCapProperty = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(VectorItemBase), new PropertyMetadata(new PropertyChangedCallback(VectorItemBase.OnStrokeDashCapChanged)));
        public static readonly DependencyProperty StrokeLineJoinProperty = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(VectorItemBase), new PropertyMetadata(1, new PropertyChangedCallback(VectorItemBase.OnStrokeLineJoinChanged)));
        public static readonly DependencyProperty StrokeDashOffsetProperty = DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(VectorItemBase), new PropertyMetadata(new PropertyChangedCallback(VectorItemBase.OnStrokeDashOffsetChanged)));
        public static readonly DependencyProperty StrokeMiterLimitProperty = DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(VectorItemBase), new PropertyMetadata(10.0, new PropertyChangedCallback(VectorItemBase.OnStrokeMiterLimitChanged)));

        public LOD Lod
        {
            get { return (LOD)base.GetValue(VectorItemBase.LodProperty); }
            set { base.SetValue(VectorItemBase.LodProperty, value); }
        }

        public Brush Fill
        {
            get { return (Brush)base.GetValue(VectorItemBase.FillProperty); }
            set { base.SetValue(VectorItemBase.FillProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)base.GetValue(VectorItemBase.StrokeProperty); }
            set { base.SetValue(VectorItemBase.StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)base.GetValue(VectorItemBase.StrokeThicknessProperty); }
            set { base.SetValue(VectorItemBase.StrokeThicknessProperty, value); }
        }

        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)base.GetValue(VectorItemBase.StrokeDashArrayProperty); }
            set { base.SetValue(VectorItemBase.StrokeDashArrayProperty, value); }
        }

        public PenLineCap StrokeStartLineCap
        {
            get { return (PenLineCap)base.GetValue(VectorItemBase.StrokeStartLineCapProperty); }
            set { base.SetValue(VectorItemBase.StrokeStartLineCapProperty, value); }
        }

        public PenLineCap StrokeEndLineCap
        {
            get { return (PenLineCap)base.GetValue(VectorItemBase.StrokeEndLineCapProperty); }
            set { base.SetValue(VectorItemBase.StrokeEndLineCapProperty, value); }
        }

        public PenLineCap StrokeDashCap
        {
            get { return (PenLineCap)base.GetValue(VectorItemBase.StrokeDashCapProperty); }
            set { base.SetValue(VectorItemBase.StrokeDashCapProperty, value); }
        }

        public PenLineJoin StrokeLineJoin
        {
            get { return (PenLineJoin)base.GetValue(VectorItemBase.StrokeLineJoinProperty); }
            set { base.SetValue(VectorItemBase.StrokeLineJoinProperty, value); }
        }

        public double StrokeDashOffset
        {
            get { return (double)base.GetValue(VectorItemBase.StrokeDashOffsetProperty); }
            set { base.SetValue(VectorItemBase.StrokeDashOffsetProperty, value); }
        }

        public double StrokeMiterLimit
        {
            get { return (double)base.GetValue(VectorItemBase.StrokeMiterLimitProperty); }
            set { base.SetValue(VectorItemBase.StrokeMiterLimitProperty, value); }
        }

        public virtual Rect Bounds
        {
            get
            {
                if (this.DefiningGeometry != null)
                {
                    return this.DefiningGeometry.Bounds;
                }
                return Rect.Empty;
            }
        }

        internal abstract Geometry DefiningGeometry
        {
            get;
        }

        internal Shape Shape
        {
            get { return base.Content as Shape; }
        }

        private static void OnLodChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        private static void OnFillChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorItemBase vectorItemBase = (VectorItemBase)obj;
            if (vectorItemBase.Shape != null)
            {
                vectorItemBase.Shape.Fill = args.NewValue as Brush;
            }
        }

        private static void OnStrokeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorItemBase vectorItemBase = (VectorItemBase)obj;
            if (vectorItemBase.Shape != null)
            {
                vectorItemBase.Shape.Stroke = args.NewValue as Brush;
            }
        }

        private static void OnStrokeThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorItemBase vectorItemBase = (VectorItemBase)obj;
            if (vectorItemBase.Shape != null)
            {
                vectorItemBase.Shape.StrokeThickness = (double)args.NewValue;
            }
        }

        private static void OnStrokeDashArrayChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorItemBase vectorItemBase = (VectorItemBase)obj;
            if (vectorItemBase.Shape != null)
            {
                vectorItemBase.Shape.StrokeDashArray = args.NewValue as DoubleCollection;
            }
        }

        private static void OnStrokeStartLineCapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorItemBase vectorItemBase = (VectorItemBase)obj;
            if (vectorItemBase.Shape != null)
            {
                vectorItemBase.Shape.StrokeStartLineCap = (PenLineCap)args.NewValue;
            }
        }

        private static void OnStrokeEndLineCapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorItemBase vectorItemBase = (VectorItemBase)obj;
            if (vectorItemBase.Shape != null)
            {
                vectorItemBase.Shape.StrokeEndLineCap = (PenLineCap)args.NewValue;
            }
        }

        private static void OnStrokeDashCapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorItemBase vectorItemBase = (VectorItemBase)obj;
            if (vectorItemBase.Shape != null)
            {
                vectorItemBase.Shape.StrokeDashCap = (PenLineCap)args.NewValue;
            }
        }

        private static void OnStrokeLineJoinChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorItemBase vectorItemBase = (VectorItemBase)obj;
            if (vectorItemBase.Shape != null)
            {
                vectorItemBase.Shape.StrokeLineJoin = (PenLineJoin)args.NewValue;
            }
        }

        private static void OnStrokeDashOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorItemBase vectorItemBase = (VectorItemBase)obj;
            if (vectorItemBase.Shape != null)
            {
                vectorItemBase.Shape.StrokeDashOffset = (double)args.NewValue;
            }
        }

        private static void OnStrokeMiterLimitChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            VectorItemBase vectorItemBase = (VectorItemBase)obj;
            if (vectorItemBase.Shape != null)
            {
                vectorItemBase.Shape.StrokeMiterLimit = (double)args.NewValue;
            }
        }

        protected internal virtual Rect GetScreenBounds()
        {
            Rect result = Rect.Empty;
            if (this._rg != null)
            {
                result = this._rg.Bounds;
            }
            return result;
        }

        internal virtual Rect GetScreenBounds(PointConverter pc)
        {
            return Rect.Empty;
        }

        internal virtual bool IsVisible(double minsz2, double zoom)
        {
            double num = 0.0;
            Rect screenBounds = this.GetScreenBounds();
            if (!screenBounds.IsEmpty)
            {
                num = screenBounds.Width * screenBounds.Width + screenBounds.Height * screenBounds.Height;
            }
            LOD lod = this.Lod;
            if (lod.IsDefault)
            {
                return num >= minsz2;
            }
            bool flag = zoom >= lod.MinZoom && zoom <= lod.MaxZoom;
            if (!flag)
            {
                return false;
            }
            if (lod.MaxSize == 0.0)
            {
                return num >= lod.MinSize * lod.MinSize;
            }
            if (lod.MinSize == 0.0)
            {
                return num <= lod.MaxSize * lod.MaxSize;
            }
            return num >= lod.MinSize * lod.MinSize && num <= lod.MaxSize * lod.MaxSize;
        }

        protected void InitShape(Shape shape)
        {
            shape.Fill = this.Fill;
            shape.Stroke = this.Stroke;
            shape.StrokeThickness = this.StrokeThickness;
            shape.StrokeDashArray = this.StrokeDashArray;
            shape.StrokeStartLineCap = this.StrokeStartLineCap;
            shape.StrokeEndLineCap = this.StrokeEndLineCap;
            shape.StrokeDashCap = this.StrokeDashCap;
            shape.StrokeLineJoin = this.StrokeLineJoin;
            shape.StrokeDashOffset = this.StrokeDashOffset;
            shape.StrokeMiterLimit = this.StrokeMiterLimit;
            base.Content = shape;
        }

        internal virtual Rect UpdateShape(PointConverter pc)
        {
            this._rg = this.GetRenderGeometry(this.DefiningGeometry, pc);
            if (this._path == null)
            {
                this._path = new Path();
                this.InitShape(this._path);
            }
            this._path.Data = this._rg;
            return this.GetScreenBounds();
        }

        private Geometry GetRenderGeometry(Geometry definingGeometry, PointConverter pc)
        {
            Geometry result = null;
            if (definingGeometry is LineGeometry)
            {
                LineGeometry lineGeometry = (LineGeometry)definingGeometry;
                LineGeometry lineGeometry2 = new LineGeometry();
                lineGeometry2.StartPoint = pc(lineGeometry.StartPoint);
                lineGeometry2.EndPoint = pc(lineGeometry.EndPoint);
                LineGeometry lineGeometry3 = lineGeometry2;
                result = lineGeometry3;
            }
            else if (definingGeometry is PathGeometry)
            {
                PathGeometry pathGeometry = (PathGeometry)definingGeometry;
                PathGeometry pathGeometry2 = new PathGeometry();
                pathGeometry2.FillRule = pathGeometry.FillRule;
                PathGeometry pathGeometry3 = pathGeometry2;
                int count = pathGeometry.Figures.Count;
                for (int i = 0; i < count; i++)
                {
                    pathGeometry3.Figures.Add(this.ConvertFigure(pathGeometry.Figures[i], pc));
                }
                result = pathGeometry3;
            }
            else if (definingGeometry is RectangleGeometry)
            {
                RectangleGeometry rectangleGeometry = (RectangleGeometry)definingGeometry;
                RectangleGeometry rectangleGeometry2 = new RectangleGeometry();
                rectangleGeometry2.RadiusX = rectangleGeometry.RadiusX;
                rectangleGeometry2.RadiusY = rectangleGeometry.RadiusY;
                RectangleGeometry rectangleGeometry3 = rectangleGeometry2;
                result = rectangleGeometry3;
            }
            else if (definingGeometry is EllipseGeometry)
            {
                EllipseGeometry ellipseGeometry = (EllipseGeometry)definingGeometry;
                EllipseGeometry ellipseGeometry2 = new EllipseGeometry();
                ellipseGeometry2.Center = pc(ellipseGeometry.Center);
                EllipseGeometry ellipseGeometry3 = ellipseGeometry2;
                result = ellipseGeometry3;
            }
            return result;
        }

        private PathFigure ConvertFigure(PathFigure pf, PointConverter pc)
        {
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = pc(pf.StartPoint);
            pathFigure.IsClosed = pf.IsClosed;
            pathFigure.IsFilled = pf.IsFilled;
            PathSegmentCollection segments = pf.Segments;
            if (segments != null)
            {
                int count = segments.Count;
                for (int i = 0; i < count; i++)
                {
                    pathFigure.Segments.Add(this.ConvertSegment(segments[i], pc));
                }
            }
            return pathFigure;
        }

        private PathSegment ConvertSegment(PathSegment ps, PointConverter pc)
        {
            PolyLineSegment polyLineSegment = ps as PolyLineSegment;
            if (polyLineSegment != null)
            {
                PolyLineSegment polyLineSegment2 = new PolyLineSegment();
                polyLineSegment2.Points = VectorItemBase.ConvertPoints(polyLineSegment.Points, pc);
                return polyLineSegment2;
            }
            LineSegment lineSegment = ps as LineSegment;
            if (lineSegment != null)
            {
                LineSegment lineSegment2 = new LineSegment();
                lineSegment2.Point = pc(lineSegment.Point);
                return lineSegment2;
            }
            BezierSegment bezierSegment = ps as BezierSegment;
            if (bezierSegment != null)
            {
                BezierSegment bezierSegment2 = new BezierSegment();
                bezierSegment2.Point1 = pc(bezierSegment.Point1);
                bezierSegment2.Point2 = pc(bezierSegment.Point2);
                bezierSegment2.Point3 = pc(bezierSegment.Point3);
                return bezierSegment2;
            }
            QuadraticBezierSegment quadraticBezierSegment = ps as QuadraticBezierSegment;
            if (quadraticBezierSegment != null)
            {
                QuadraticBezierSegment quadraticBezierSegment2 = new QuadraticBezierSegment();
                quadraticBezierSegment2.Point1 = pc(quadraticBezierSegment.Point1);
                quadraticBezierSegment2.Point2 = pc(quadraticBezierSegment.Point2);
                return quadraticBezierSegment2;
            }
            PolyBezierSegment polyBezierSegment = ps as PolyBezierSegment;
            if (polyBezierSegment != null)
            {
                PolyBezierSegment polyBezierSegment2 = new PolyBezierSegment();
                polyBezierSegment2.Points = VectorItemBase.ConvertPoints(polyBezierSegment.Points, pc);
                return polyBezierSegment2;
            }
            PolyQuadraticBezierSegment polyQuadraticBezierSegment = ps as PolyQuadraticBezierSegment;
            if (polyQuadraticBezierSegment != null)
            {
                PolyQuadraticBezierSegment polyQuadraticBezierSegment2 = new PolyQuadraticBezierSegment();
                polyQuadraticBezierSegment2.Points = VectorItemBase.ConvertPoints(polyQuadraticBezierSegment.Points, pc);
                return polyQuadraticBezierSegment2;
            }
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "{0} is not supported.", new object[]
            {
                ps
            }));
        }

        internal static PointCollection ConvertPoints(PointCollection pts, PointConverter pc)
        {
            PointCollection pointCollection = null;
            if (pts != null)
            {
                int count = pts.Count;
                pointCollection = new PointCollection();
                Point point = pc(pts[0]);
                pointCollection.Add(point);
                for (int i = 1; i < count; i++)
                {
                    Point point2 = pc(pts[i]);
                    if (Math.Abs(point2.X - point.X) >= 1.5 || Math.Abs(point2.Y - point.Y) >= 1.5)
                    {
                        pointCollection.Add(point2);
                        point = point2;
                    }
                }
            }
            return pointCollection;
        }

        internal static PointCollection ConvertPoints(IEnumerable<Point> pts, PointConverter pc)
        {
            PointCollection pointCollection = null;
            if (pts != null)
            {
                pointCollection = new PointCollection();
                Point point = default(Point);
                bool flag = true;
                using (IEnumerator<Point> enumerator = pts.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Point current = enumerator.Current;
                        if (flag)
                        {
                            point = pc(current);
                            pointCollection.Add(point);
                            flag = false;
                        }
                        else
                        {
                            Point point2 = pc(current);
                            if (Math.Abs(point2.X - point.X) >= 1.5 || Math.Abs(point2.Y - point.Y) >= 1.5)
                            {
                                pointCollection.Add(point2);
                                point = point2;
                            }
                        }
                    }
                }
            }
            return pointCollection;
        }

        internal static Point[] ConvertPointsToArray(IEnumerable<Point> pts, PointConverter pc)
        {
            List<Point> list = null;
            if (pts != null)
            {
                list = new List<Point>();
                bool flag = true;
                Point point = default(Point);
                using (IEnumerator<Point> enumerator = pts.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Point current = enumerator.Current;
                        if (flag)
                        {
                            point = pc(current);
                            list.Add(point);
                            flag = false;
                        }
                        else
                        {
                            Point point2 = pc(current);
                            if (Math.Abs(point2.X - point.X) >= 1.5 || Math.Abs(point2.Y - point.Y) >= 1.5)
                            {
                                list.Add(point2);
                                point = point2;
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }

        protected Rect GetBounds(IEnumerable<Point> pts)
        {
            if (pts != null)
            {
                double num2;
                double num = num2 = 1.7976931348623157E+308;
                double num4;
                double num3 = num4 = -1.7976931348623157E+308;
                bool flag = true;
                using (IEnumerator<Point> enumerator = pts.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Point current = enumerator.Current;
                        flag = false;
                        if (current.X < num2)
                        {
                            num2 = current.X;
                        }
                        if (current.X > num4)
                        {
                            num4 = current.X;
                        }
                        if (current.Y < num)
                        {
                            num = current.Y;
                        }
                        if (current.Y > num3)
                        {
                            num3 = current.Y;
                        }
                    }
                }
                if (!flag)
                {
                    return new Rect(num2, num, num4 - num2, num3 - num);
                }
            }
            return Rect.Empty;
        }
    }
}
