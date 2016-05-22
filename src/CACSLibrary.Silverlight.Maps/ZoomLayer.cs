using System;
using System.Collections.Specialized;
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
    public class ZoomLayer : ItemsControl, IMapLayer
    {
        private CACSMaps _maps;
        private ZoomCanvas _itemsHost;

        public CACSMaps ParentMaps
        {
            get { return this._maps; }
            set
            {
                this._maps = value;
                this.InitializeItemsHost();
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            FrameworkElement frameworkElement = element as FrameworkElement;
            if (frameworkElement != null)
            {
                this._itemsHost = (VisualTreeHelper.GetParent(frameworkElement) as ZoomCanvas);
                this.InitializeItemsHost();
            }
        }

        private void InitializeItemsHost()
        {
            if (this._itemsHost == null || this._maps == null)
            {
                return;
            }
            this._itemsHost.ParentMaps = this._maps;
        }

        public ZoomLayer()
        {
            base.DefaultStyleKey = typeof(ZoomLayer);
        }
    }
}
