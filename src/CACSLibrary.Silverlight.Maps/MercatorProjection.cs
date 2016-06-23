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
    public class MercatorProjection : IMapProjection
    {
        private double _scaleX;
        private double _scaleY;
        private double _offsetX;
        private double _offsetY;

        public MercatorProjection(double scaleX, double scaleY, double offsetX, double offsetY)
        {
            this._scaleX = scaleX;
            this._scaleY = scaleY;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
        }

        public Point Project(Point latLong)
        {
            //double num = Math.Sin(latLong.Y * ProjectionFast.toRad);
            //return new Point(latLong.X * ProjectionFast.toRad * this._scaleX + this._offsetX, 0.5 * Math.Log((1.0 + num) / (1.0 - num)) * this._scaleY + this._offsetY);
            return new Point(latLong.X * ProjectionFast.toRad * this._scaleX + this._offsetX, latLong.Y * ProjectionFast.toRad * this._scaleY + this._offsetY);
        }

        public Point Unproject(Point point)
        {
            //return new Point((point.X - this._offsetX) * ProjectionFast.toDeg / this._scaleX, Math.Atan(Math.Sinh((point.Y - this._offsetY) / this._scaleY)) * ProjectionFast.toDeg);
            return new Point((point.X - this._offsetX) * ProjectionFast.toDeg / this._scaleX, (point.Y - this._offsetY) * ProjectionFast.toDeg / this._scaleY);
        }
    }
}
