using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CACSLibrary.Silverlight.Maps
{
    internal class ClippingEngine
    {
        private static int CScode(Rect rect, Point pt)
        {
            int num = 0;
            if (pt.X < rect.Left)
            {
                num++;
            }
            else if (pt.X > rect.Right)
            {
                num += 2;
            }
            if (pt.Y > rect.Bottom)
            {
                num += 4;
            }
            else if (pt.Y < rect.Top)
            {
                num += 8;
            }
            return num;
        }

        public static PathFigure[] CreateClippedLines(Point[] pts, Rect r)
        {
            List<PathFigure> list = new List<PathFigure>();
            int num = pts.Length;
            for (int i = 1; i < num; i++)
            {
                Point[] array = ClippingEngine.ClipSegmentCS(r, new Point[]
                {
                    pts[i - 1],
                    pts[i]
                });
                if (array != null)
                {
                    PathFigure pathFigure = new PathFigure();
                    pathFigure.StartPoint = array[0];
                    pathFigure.IsClosed = false;
                    pathFigure.IsFilled = false;
                    pathFigure.Segments = new PathSegmentCollection();
                    PathFigure pathFigure2 = pathFigure;
                    LineSegment lineSegment = new LineSegment();
                    lineSegment.Point = array[1];
                    LineSegment lineSegment2 = lineSegment;
                    pathFigure2.Segments.Add(lineSegment2);
                    list.Add(pathFigure2);
                }
            }
            return list.ToArray();
        }

        public static Point[] ClipSegmentCS(Rect rect, Point[] pts)
        {
            double num = 0.0;
            double num2 = 0.0;
            bool flag = false;
            Point point = pts[0];
            Point point2 = pts[1];
            int num3 = ClippingEngine.CScode(rect, point);
            int num4 = ClippingEngine.CScode(rect, point2);
            double num5 = point2.X - point.X;
            double num6 = point2.Y - point.Y;
            if (num5 != 0.0)
            {
                num2 = num6 / num5;
            }
            else if (num6 == 0.0)
            {
                if (num3 == 0 && num4 == 0)
                {
                    return new Point[]
                    {
                        point,
                        point2
                    };
                }
                return null;
            }
            if (num6 != 0.0)
            {
                num = num5 / num6;
            }
            int num7 = 4;
            while ((num3 & num4) <= 0)
            {
                if (num3 == 0 && num4 == 0)
                {
                    flag = true;
                    break;
                }
                if (num3 == 0)
                {
                    int num8 = num3;
                    num3 = num4;
                    num4 = num8;
                    double num9 = point.X;
                    point.X = point2.X;
                    point2.X = num9;
                    num9 = point.Y;
                    point.Y = point2.Y;
                    point2.Y = num9;
                }
                if ((num3 & 1) > 0)
                {
                    point.Y = point.Y + num2 * (rect.Left - point.X);
                    point.X = rect.Left;
                }
                else if ((num3 & 2) > 0)
                {
                    point.Y = point.Y + num2 * (rect.Right - point.X);
                    point.X = rect.Right;
                }
                else if ((num3 & 4) > 0)
                {
                    point.X = point.X + num * (rect.Bottom - point.Y);
                    point.Y = rect.Bottom;
                }
                else if ((num3 & 8) > 0)
                {
                    point.X = point.X + num * (rect.Top - point.Y);
                    point.Y = rect.Top;
                }
                num3 = ClippingEngine.CScode(rect, point);
                if (--num7 < 0)
                {
                    break;
                }
            }
            if (flag)
            {
                return new Point[]
                {
                    point,
                    point2
                };
            }
            return null;
        }

        private static int CScode(Rect rect, double x, double y)
        {
            int num = 0;
            if (x < rect.Left)
            {
                num++;
            }
            else if (x > rect.Right)
            {
                num += 2;
            }
            if (y > rect.Bottom)
            {
                num += 4;
            }
            else if (y < rect.Top)
            {
                num += 8;
            }
            return num;
        }

        public static bool ClipSegmentCS(Rect rect, ref double x1, ref double y1, ref double x2, ref double y2)
        {
            double num = 0.0;
            double num2 = 0.0;
            bool flag = false;
            double num3 = x1;
            double num4 = y1;
            double num5 = x2;
            double num6 = y2;
            int num7 = ClippingEngine.CScode(rect, num3, num4);
            int num8 = ClippingEngine.CScode(rect, num5, num6);
            double num9 = num5 - num3;
            double num10 = num6 - num4;
            if (num9 != 0.0)
            {
                num2 = num10 / num9;
            }
            else if (num10 == 0.0)
            {
                if (num7 == 0 && num8 == 0)
                {
                    x1 = num3;
                    y1 = num4;
                    x2 = num5;
                    y2 = num6;
                    return true;
                }
                return false;
            }
            if (num10 != 0.0)
            {
                num = num9 / num10;
            }
            int num11 = 4;
            while ((num7 & num8) <= 0)
            {
                if (num7 == 0 && num8 == 0)
                {
                    flag = true;
                    break;
                }
                if (num7 == 0)
                {
                    int num12 = num7;
                    num7 = num8;
                    num8 = num12;
                    double num13 = num3;
                    num3 = num5;
                    num5 = num13;
                    num13 = num4;
                    num4 = num6;
                    num6 = num13;
                }
                if ((num7 & 1) > 0)
                {
                    num4 += num2 * (rect.Left - num3);
                    num3 = rect.Left;
                }
                else if ((num7 & 2) > 0)
                {
                    num4 += num2 * (rect.Right - num3);
                    num3 = rect.Right;
                }
                else if ((num7 & 4) > 0)
                {
                    num3 += num * (rect.Bottom - num4);
                    num4 = rect.Bottom;
                }
                else if ((num7 & 8) > 0)
                {
                    num3 += num * (rect.Top - num4);
                    num4 = rect.Top;
                }
                num7 = ClippingEngine.CScode(rect, num3, num4);
                if (--num11 < 0)
                {
                    break;
                }
            }
            if (flag)
            {
                x1 = num3;
                y1 = num4;
                x2 = num5;
                y2 = num6;
                return true;
            }
            return false;
        }

        public static PointCollection ClipPolygon(PointCollection pts, Rect clip)
        {
            PointCollection pointCollection = null;
            int count = pts.Count;
            if (count >= 2)
            {
                List<Point> list = new List<Point>(count);
                for (int i = 0; i < count; i++)
                {
                    list.Add(pts[i]);
                }
                list = ClippingEngine.ClipPolygonByLine(list, new Point(clip.Left, clip.Top), new Point(clip.Right, clip.Top));
                list = ClippingEngine.ClipPolygonByLine(list, new Point(clip.Right, clip.Top), new Point(clip.Right, clip.Bottom));
                list = ClippingEngine.ClipPolygonByLine(list, new Point(clip.Right, clip.Bottom), new Point(clip.Left, clip.Bottom));
                list = ClippingEngine.ClipPolygonByLine(list, new Point(clip.Left, clip.Bottom), new Point(clip.Left, clip.Top));
                count = list.Count;
                if (count > 0)
                {
                    pointCollection = new PointCollection();
                    for (int j = 0; j < count; j++)
                    {
                        pointCollection.Add(list[j]);
                    }
                }
            }
            return pointCollection;
        }

        private static List<Point> ClipPolygonByLine(List<Point> pts, Point cp1, Point cp2)
        {
            List<Point> list = new List<Point>();
            int count = pts.Count;
            if (count > 0)
            {
                Point point = pts[count - 1];
                for (int i = 0; i < count; i++)
                {
                    Point point2 = pts[i];
                    if (ClippingEngine.Inside(point2, cp1, cp2))
                    {
                        if (ClippingEngine.Inside(point, cp1, cp2))
                        {
                            list.Add(point2);
                        }
                        else
                        {
                            list.Add(ClippingEngine.Intersect(point, point2, cp1, cp2));
                            list.Add(point2);
                        }
                    }
                    else if (ClippingEngine.Inside(point, cp1, cp2))
                    {
                        list.Add(ClippingEngine.Intersect(point, point2, cp1, cp2));
                    }
                    point = point2;
                }
            }
            return list;
        }

        private static bool Inside(Point p, Point cp0, Point cp1)
        {
            return (cp1.X > cp0.X && p.Y >= cp0.Y) || (cp1.X < cp0.X && p.Y <= cp0.Y) || (cp1.Y > cp0.Y && p.X <= cp1.X) || (cp1.Y < cp0.Y && p.X >= cp1.X);
        }

        private static Point Intersect(Point p0, Point p1, Point cp0, Point cp1)
        {
            Point result = default(Point);
            if (cp0.Y == cp1.Y)
            {
                result.Y = cp0.Y;
                result.X = p0.X + (cp0.Y - p0.Y) * (p1.X - p0.X) / (p1.Y - p0.Y);
            }
            else
            {
                result.X = cp0.X;
                result.Y = p0.Y + (cp0.X - p0.X) * (p1.Y - p0.Y) / (p1.X - p0.X);
            }
            return result;
        }
    }
}
