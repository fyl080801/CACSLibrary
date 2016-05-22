using System;

namespace CACSLibrary.Silverlight.Maps
{
    public struct LOD
    {
        private double _minSize;
        private double _maxSize;
        private double _minZoom;
        private double _maxZoom;

        public double MinSize
        {
            get { return this._minSize; }
            set { this._minSize = value; }
        }

        public double MaxSize
        {
            get { return this._maxSize; }
            set { this._maxSize = value; }
        }

        public double MinZoom
        {
            get { return this._minZoom; }
            set { this._minZoom = value; }
        }

        public double MaxZoom
        {
            get { return this._maxZoom; }
            set { this._maxZoom = value; }
        }

        internal bool IsDefault
        {
            get
            {
                return this.MinSize == 0.0 && this.MaxSize == 0.0 && this.MinZoom == 0.0 && this.MaxZoom == 0.0;
            }
        }

        public LOD(double minSize, double maxSize, double minZoom, double maxZoom)
        {
            this._minSize = minSize;
            this._maxSize = maxSize;
            this._minZoom = minZoom;
            this._maxZoom = maxZoom;
        }
    }
}
