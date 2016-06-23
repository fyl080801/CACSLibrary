using System;
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
    public class ProjectionFast : IMapProjection
    {
        public const double toRad = 0.017453292519943295;
        public const double toDeg = 57.295779513082323;
        private double _scaleX;
        private double _scaleY;
        private double _offsetX;
        private double _offsetY;
        private double ax;
        private double ay;
        private double bx;
        private double by;
        private double _extraX;
        private double _extraY;
        private double scale = 1.0;
        private double offsetx;
        private double offsety;

        public ProjectionFast(double scaleX, double scaleY, double offsetX, double offsetY, double extraX, double extraY)
        {
            this._scaleX = scaleX;
            this._scaleY = scaleY;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            this._extraX = extraX;
            this._extraY = extraY;
        }

        public void Init(CACSMaps map)
        {
            this.scale = map.ActualWidth / map.ViewportWidth;
            Point point = map.Projection.Project(map.Center);
            this.offsetx = 0.5 * map.ActualWidth - point.X * this.scale;
            this.offsety = 0.5 * map.ActualHeight - point.Y * this.scale;
            this.ax = toRad * this._scaleX * this.scale;
            this.bx = this._offsetX * this.scale + this.offsetx + this._extraX;
            this.ay = 0.5 * this._scaleY * this.scale;
            this.by = this._offsetY * this.scale + this.offsety + this._extraY;
        }

        public Point Project(Point latLong)
        {
            //double num = Math.Sin(latLong.Y * toRad);
            //return new Point(latLong.X * this.ax + this.bx, Math.Log((1.0 + num) / (1.0 - num)) * this.ay + this.by);
            return new Point(latLong.X * this.ax + this.bx, latLong.Y * this.ay + this.by);
        }

        public Point Unproject(Point point)
        {
            //return new Point((point.X - this._offsetX) * toDeg / this._scaleX, Math.Atan(Math.Sinh((point.Y - this._offsetY) / this._scaleY)) * toDeg);
            return new Point((point.X - this._offsetX) * toDeg / this._scaleX, (point.Y - this._offsetY) * toDeg / this._scaleY);
        }
    }
}
