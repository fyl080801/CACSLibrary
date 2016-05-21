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
    public class MouseDragEventArgs : EventArgs
    {
        private Point _startPosition;
        private Point _previousPosition;
        private Point _currentPosition;

        internal CACSMouseHelper MouseHelper { get; private set; }

        public double DeltaX
        {
            get { return this._currentPosition.X - this._previousPosition.X; }
        }

        public double DeltaY
        {
            get { return this._currentPosition.Y - this._previousPosition.Y; }
        }

        public double TotalDeltaX
        {
            get { return this._currentPosition.X - this._startPosition.X; }
        }

        public double TotalDeltaY
        {
            get { return this._currentPosition.Y - this._startPosition.Y; }
        }

        public Point StartPosition
        {
            get { return this._startPosition; }
        }

        public Point PreviousPosition
        {
            get { return this._previousPosition; }
        }

        public Point CurrentPosition
        {
            get { return this._currentPosition; }
        }

        internal MouseEventArgs MouseEventArgs { get; set; }

        internal bool IsLostMouseCapture { get; set; }

        internal MouseDragEventArgs(CACSMouseHelper mouseHelper, Point startPosition, Point previousPosition, Point currentPosition)
        {
            this._startPosition = startPosition;
            this._previousPosition = previousPosition;
            this._currentPosition = currentPosition;
            this.MouseHelper = mouseHelper;
        }

        public Point GetPosition(UIElement relativeTo)
        {
            if (this.MouseEventArgs != null)
            {
                return this.MouseEventArgs.GetPosition(relativeTo);
            }
            if (relativeTo == null)
            {
                return this.CurrentPosition;
            }
            return relativeTo.CACSGetRootVisual().CACSTransformToVisual(relativeTo).Transform(this.CurrentPosition);
        }
    }
}
