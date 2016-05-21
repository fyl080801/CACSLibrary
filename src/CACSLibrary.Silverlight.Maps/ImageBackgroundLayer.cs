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
    public class ImageBackgroundLayer : BackgroundLayer
    {
        private Image _image;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;
        private TransformGroup _transformGroup = new TransformGroup();
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(BackgroundLayer), new PropertyMetadata(new PropertyChangedCallback(ImageBackgroundLayer.OnSourcePropertyChanged)));
        public event EventHandler ImageLoaded;

        [Category("cacs"), TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource Source
        {
            get { return (ImageSource)base.GetValue(ImageBackgroundLayer.SourceProperty); }
            set { base.SetValue(ImageBackgroundLayer.SourceProperty, value); }
        }

        internal double ViewportActualWidth
        {
            get { return this._image.ActualWidth; }
        }

        internal double ViewportActualHeight
        {
            get { return this._image.ActualHeight; }
        }

        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageBackgroundLayer layer = d as ImageBackgroundLayer;
            ImageSource oldvalue = (ImageSource)e.OldValue;
            if (layer != null)
            {
                layer.OnSourceChanged(oldvalue);
            }
        }

        protected override void InitializeRootPart()
        {
            this._scaleTransform = new ScaleTransform();
            this._translateTransform = new TranslateTransform();
            this._transformGroup.Children.Add(this._scaleTransform);
            this._transformGroup.Children.Add(this._translateTransform);
            this._elementRoot.RenderTransform = this._transformGroup;
            this.CreateImage();
            base.InitializeRootPart();
        }

        private void OnSourceChanged(ImageSource oldValue)
        {
            this.CreateImage();
            this.ImageLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void CreateImage()
        {
            if (this._elementRoot != null)
            {
                if (this.Source == null)
                {
                    this._elementRoot.Children.Clear();
                }
                else
                {
                    this.CleanImages();
                    Image image = new Image
                    {
                        Source = this.Source
                    };
                    this._image = image;
                    this._elementRoot.Children.Clear();
                    this._elementRoot.Children.Add(this._image);
                    this.CenterChanged();
                    this.ZoomChanged();
                }
            }
        }

        private void CleanImages()
        {
            if (this._image != null)
            {
                this._image.Source = null;
                this._image = null;
            }
        }

        protected override void ZoomChanged()
        {
            if (this._elementRoot != null && base.ParentMaps != null && this.Source != null)
            {
                if (base.ParentMaps.ViewportWidth != 0.0)
                {
                    this._scaleTransform.ScaleX = 1 / base.ParentMaps.ViewportWidth;
                    this._scaleTransform.ScaleY = 1 / base.ParentMaps.ViewportWidth;
                    this.CenterChanged();
                }
            }
        }

        protected override void CenterChanged()
        {
            if (this._elementRoot != null && base.ParentMaps != null && this.Source != null)
            {
                Point point = base.ParentMaps.Projection.Project(base.ParentMaps.Center);
                this.RenderTransformOrigin = point;
                //viewportOrigin.X=viewportOrigin.X - this.ViewportWidth / 2.0);
                //if (base.ActualWidth != 0.0)
                //{
                //    viewportOrigin.Y=viewportOrigin.Y - this.ViewportWidth * (base.ActualHeight / base.ActualWidth) / 2.0);
                //}
                Point newTranslate = new Point(
                    (0.5 - point.X) / base.ParentMaps.ViewportWidth * base.ActualWidth,
                    (0.5 - point.Y) / base.ParentMaps.ViewportHeight * base.ActualHeight);
                this._translateTransform.X = newTranslate.X;
                this._translateTransform.Y = newTranslate.Y;
            }
        }
    }
}
