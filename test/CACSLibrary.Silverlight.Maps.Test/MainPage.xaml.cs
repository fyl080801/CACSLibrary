using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CACSLibrary.Silverlight.Maps.Test
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void vl_Loaded(object sender, RoutedEventArgs e)
        {
            //VectorPolygon vec = new VectorPolygon();
            //vec.Points = new PointCollection();
            //vec.Points.Add(new Point(20, 40));
            //vec.Points.Add(new Point(10, 40));
            //vec.Points.Add(new Point(0, 50));
            //vl.Children.Add(vec as VectorItemBase);
        }

        private void map_Loaded(object sender, RoutedEventArgs e)
        {
            map.Zoom = 1;
        }
    }
}
