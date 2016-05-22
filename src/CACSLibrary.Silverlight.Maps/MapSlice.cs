using System;

namespace CACSLibrary.Silverlight.Maps
{
    public class MapSlice
    {
        private double _zoom;
        private int _latSlices;
        private int _longSlices;

        public double Zoom
        {
            get { return this._zoom; }
        }

        public int LatSlices
        {
            get { return this._latSlices; }
        }

        public int LongSlices
        {
            get { return this._longSlices; }
        }

        public MapSlice(int latSlices, int longSlices, double zoom)
        {
            this._zoom = zoom;
            this._latSlices = latSlices;
            this._longSlices = longSlices;
        }
    }
}
