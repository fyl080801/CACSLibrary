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

namespace CACSLibrary.Silverlight
{
    public class MouseWheelEventArgs : EventArgs
    {
        private double _delta;
        private Point _position;

        public double Delta
        {
            get { return this._delta; }
        }

        public Point Position
        {
            get { return this._position; }
        }

        public bool Handled { get; set; }

        public MouseWheelEventArgs(double delta, Point position)
        {
            this._delta = delta;
            this._position = position;
        }
    }
}
