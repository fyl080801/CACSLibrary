using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CACSLibrary.Silverlight.Maps
{
    public class VectorPanel : Panel
    {
        private static Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        private TranslateTransform tt;
        private ScaleTransform st;
        private TransformGroup tg;

        public VectorPanel()
        {
            this.tg = new TransformGroup();
            this.st = new ScaleTransform();
            this.tt = new TranslateTransform();
            this.tg.Children.Add(this.st);
            this.tg.Children.Add(this.tt);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = base.Children.Count;
            for (int i = 0; i < count; i++)
            {
                UIElement uIElement = base.Children[i];
                if (uIElement is VectorPlacemark)
                {
                    Rect geometryBounds = ((VectorPlacemark)uIElement).GeometryBounds;
                    geometryBounds.Width = uIElement.DesiredSize.Width;
                    geometryBounds.Height = uIElement.DesiredSize.Height;
                    uIElement.Arrange(new Rect(-10000.0 + geometryBounds.X, -10000.0 + geometryBounds.Y, geometryBounds.Width, geometryBounds.Height));
                }
                else if (uIElement is VectorItemBase)
                {
                    uIElement.Arrange(new Rect(-10000.0, -10000.0, uIElement.DesiredSize.Width, uIElement.DesiredSize.Height));
                }
                else
                {
                    uIElement.Arrange(new Rect(-10000.0 + Canvas.GetLeft(uIElement), -10000.0 + Canvas.GetTop(uIElement), uIElement.DesiredSize.Width, uIElement.DesiredSize.Height));
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            int count = base.Children.Count;
            for (int i = 0; i < count; i++)
            {
                UIElement uIElement = base.Children[i];
                uIElement.Measure(VectorPanel.infiniteSize);
            }
            return base.MeasureOverride(availableSize);
        }

        internal void SetTransformation(double scalex, double scaley, double offx, double offy)
        {
            this.st.CenterX = 0.5 * base.ActualWidth;
            this.st.CenterY = 0.5 * base.ActualHeight;
            this.st.ScaleX = scalex;
            this.st.ScaleY = scaley;
            this.tt.X = offx;
            this.tt.Y = offy;
            base.RenderTransform = this.tg;
        }

        internal void ResetTransformation()
        {
            base.RenderTransform = null;
        }
    }
}
