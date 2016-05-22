using System;
using System.ComponentModel;
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
    public class VectorImage : VectorItemBase
    {
        Image _img;
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(VectorImage), new PropertyMetadata(new PropertyChangedCallback(VectorImage.OnSourcePropertyChanged)));

        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VectorImage layer = d as VectorImage;
            ImageSource oldvalue = (ImageSource)e.OldValue;
            if (layer != null)
            {
                layer.OnSourceChanged(oldvalue);
            }
        }

        private void OnSourceChanged(ImageSource oldvalue)
        {
            this.CreateImage();
        }

        [Category("cacs"), TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource Source
        {
            get
            {
                return (ImageSource)base.GetValue(VectorImage.SourceProperty);
            }
            set
            {
                base.SetValue(VectorImage.SourceProperty, value);
            }
        }

        internal override Geometry DefiningGeometry
        {
            get { return null; }
        }

        private void CreateImage()
        {
            _img = new Image()
            {
                Source = this.Source
            };
            base.Content = _img;
        }
    }
}
