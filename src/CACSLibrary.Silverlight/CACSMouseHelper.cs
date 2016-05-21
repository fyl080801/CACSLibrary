using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
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
    public class CACSMouseHelper : IDisposable
    {
        private const string ERR_MUSTBEWINDOWLESS = "To use this event the Silverlight plug-in must have its 'windowless' parameter set to true.";

        private List<object> _weakHandlers = new List<object>();

        private UIElement _element;

        private UIElement _captureElement;

        private EventHandler<MouseWheelEventArgs> _wheel;

        private MouseEventHandler _contextMenuInvoked;

        private bool _isMouseOver;

        private Point _lastPos;

        private DateTime _lastTime;

        private bool capture;

        private bool dragStarted;

        private Point lastPos = default(Point);

        private Point lastMouseDownPos = default(Point);

        public event MouseEventHandler MouseClick;

        public event MouseButtonEventHandler MouseDoubleClick;

        public event EventHandler<MouseWheelEventArgs> MouseWheel
        {
            add
            {
                this._wheel = (EventHandler<MouseWheelEventArgs>)Delegate.Combine(this._wheel, value);
                this.InitializeWheel();
            }
            remove
            {
                this._wheel = (EventHandler<MouseWheelEventArgs>)Delegate.Remove(this._wheel, value);
            }
        }

        internal event EventHandler<MouseDragEventArgs> MouseDragButtonDown;

        public event EventHandler<MouseDragEventArgs> MouseDragStart;

        public event EventHandler<MouseDragEventArgs> MouseDragMove;

        public event EventHandler<MouseDragEventArgs> MouseDragEnd;

        public event MouseEventHandler ContextMenuInvoked
        {
            add
            {
                this._contextMenuInvoked = (MouseEventHandler)Delegate.Combine(this._contextMenuInvoked, value);
                this.InitializeRightButton();
            }
            remove
            {
                this._contextMenuInvoked = (MouseEventHandler)Delegate.Remove(this._contextMenuInvoked, value);
            }
        }

        internal bool IsMouseCaptured
        {
            get
            {
                return this.capture;
            }
        }

        private T WeakHandler<T>(T t)
        {
            this._weakHandlers.Add(t);
            return t;
        }

        public CACSMouseHelper(UIElement element) : this(element, element)
        {
        }

        internal CACSMouseHelper(UIElement element, UIElement captureElement)
        {
            this._element = element;
            this._captureElement = captureElement;
            this._element.MouseEnter += new MouseEventHandler(this._element_MouseEnter);
            this._element.MouseLeave += new MouseEventHandler(this._element_MouseLeave);
            this.InitializeClick();
            this.InitializeDoubleClick();
            this.InitializeRightButton();
            this.InitializeDrag();
        }

        public void Dispose()
        {
            if (this._element != null)
            {
                this._element.MouseEnter -= new MouseEventHandler(this._element_MouseEnter);
                this._element.MouseLeave -= new MouseEventHandler(this._element_MouseLeave);
                this.FinalizeClick();
                this.FinalizeDoubleClick();
                this.FinalizeRightButton();
                this.FinalizeDrag();
            }
        }

        internal static void AddEventHandler(UIElement element, string eventName, Delegate handler)
        {
            EventInfo @event = element.GetType().GetEvent(eventName);
            @event.AddEventHandler(element, handler);
        }

        internal static void RemoveEventHandler(UIElement element, string eventName, Delegate handler)
        {
            EventInfo @event = element.GetType().GetEvent(eventName);
            @event.RemoveEventHandler(element, handler);
        }

        private void _element_MouseEnter(object sender, MouseEventArgs e)
        {
            this._isMouseOver = true;
        }

        private void _element_MouseLeave(object sender, MouseEventArgs e)
        {
            this._isMouseOver = false;
        }

        private void InitializeDoubleClick()
        {
            this._lastPos = default(Point);
            this._lastTime = default(DateTime);
            CACSMouseHelper.AddEventHandler(this._element, "MouseLeftButtonDown", new MouseButtonEventHandler(this.DoubleClickMouseLeftButtonDown));
        }

        private void FinalizeDoubleClick()
        {
            CACSMouseHelper.RemoveEventHandler(this._element, "MouseLeftButtonDown", new MouseButtonEventHandler(this.DoubleClickMouseLeftButtonDown));
        }

        private void DoubleClickMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.MouseDoubleClick != null)
            {
                DateTime now = DateTime.Now;
                Point position = e.GetPosition(this._element);
                if (now.Subtract(this._lastTime).TotalMilliseconds <= 400.0)
                {
                    double num = this._lastPos.X - position.X;
                    double num2 = this._lastPos.Y - position.Y;
                    if (num * num + num2 * num2 <= 4.0)
                    {
                        this.MouseDoubleClick.Invoke(this._element, e);
                        return;
                    }
                }
                this._lastTime = now;
                this._lastPos = position;
            }
        }

        private void InitializeClick()
        {
            this._lastPos = default(Point);
            CACSMouseHelper.AddEventHandler(this._element, "MouseLeftButtonDown", new MouseButtonEventHandler(this.ClickMouseLeftButtonDown));
            CACSMouseHelper.AddEventHandler(this._element, "MouseLeftButtonUp", new MouseButtonEventHandler(this.ClickMouseLeftButtonUp));
        }

        private void FinalizeClick()
        {
            CACSMouseHelper.RemoveEventHandler(this._element, "MouseLeftButtonDown", new MouseButtonEventHandler(this.ClickMouseLeftButtonDown));
            CACSMouseHelper.RemoveEventHandler(this._element, "MouseLeftButtonUp", new MouseButtonEventHandler(this.ClickMouseLeftButtonUp));
        }

        private void ClickMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.MouseClick != null)
            {
                this._lastPos = e.GetPosition(this._element);
            }
        }

        private void ClickMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.MouseClick != null)
            {
                DateTime arg_0D_0 = DateTime.Now;
                Point position = e.GetPosition(this._element);
                double num = this._lastPos.X - position.X;
                double num2 = this._lastPos.Y - position.Y;
                if (num * num + num2 * num2 <= 4.0)
                {
                    this.MouseClick.Invoke(this._element, e);
                }
            }
        }

        private void InitializeDrag()
        {
            this.capture = false;
            this.dragStarted = false;
            this.lastPos = default(Point);
            this.lastMouseDownPos = default(Point);
            CACSMouseHelper.AddEventHandler(this._element, "MouseLeftButtonDown", new MouseButtonEventHandler(this.DragMouseLeftButtonDown));
        }

        private void FinalizeDrag()
        {
            CACSMouseHelper.RemoveEventHandler(this._element, "MouseLeftButtonDown", new MouseButtonEventHandler(this.DragMouseLeftButtonDown));
            this.ReleaseMouseCapture();
        }

        internal void ReleaseMouseCapture()
        {
            if (!this.capture)
            {
                return;
            }
            this.capture = false;
            this.dragStarted = false;
            CACSMouseHelper.RemoveEventHandler(this._captureElement, "MouseMove", new MouseEventHandler(this.DragMouseMove));
            CACSMouseHelper.RemoveEventHandler(this._captureElement, "MouseLeftButtonUp", new MouseButtonEventHandler(this.DragMouseLeftButtonUp));
            CACSMouseHelper.RemoveEventHandler(this._captureElement, "LostMouseCapture", new MouseEventHandler(this.DragLostMouseCapture));
            this._captureElement.ReleaseMouseCapture();
        }

        private void DragMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.MouseDragStart != null || this.MouseDragMove != null || this.MouseDragEnd != null)
            {
                this.lastPos = e.GetPosition(Application.Current.RootVisual);
                this.lastMouseDownPos = this.lastPos;
                this.TryCaptureMouse(e);
            }
        }

        internal bool TryCaptureMouse(MouseButtonEventArgs e = null)
        {
            if (!this._captureElement.TryCaptureMouse())
            {
                return false;
            }
            this.capture = true;
            CACSMouseHelper.AddEventHandler(this._captureElement, "MouseMove", new MouseEventHandler(this.DragMouseMove));
            CACSMouseHelper.AddEventHandler(this._captureElement, "MouseLeftButtonUp", new MouseButtonEventHandler(this.DragMouseLeftButtonUp));
            CACSMouseHelper.AddEventHandler(this._captureElement, "LostMouseCapture", new MouseEventHandler(this.DragLostMouseCapture));
            if (this.MouseDragButtonDown != null && e != null)
            {
                this.MouseDragButtonDown.Invoke(this._element, new MouseDragEventArgs(this, this.lastMouseDownPos, this.lastPos, this.lastPos)
                {
                    MouseEventArgs = e
                });
            }
            return true;
        }

        private void DragMouseMove(object sender, MouseEventArgs e)
        {
            if (this.MouseDragStart != null || this.MouseDragMove != null || this.MouseDragEnd != null)
            {
                if (!this.dragStarted)
                {
                    this.dragStarted = true;
                    if (this.MouseDragStart != null)
                    {
                        this.MouseDragStart.Invoke(this._element, new MouseDragEventArgs(this, this.lastMouseDownPos, this.lastPos, this.lastPos)
                        {
                            MouseEventArgs = e
                        });
                    }
                }
                if (!this.capture)
                {
                    return;
                }
                Point position = e.GetPosition(null);
                if (this.MouseDragMove != null)
                {
                    this.MouseDragMove.Invoke(this._element, new MouseDragEventArgs(this, this.lastMouseDownPos, this.lastPos, position)
                    {
                        MouseEventArgs = e
                    });
                }
                this.lastPos = position;
            }
        }

        private void DragMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.DragEnd(e, false);
        }

        private void DragLostMouseCapture(object sender, MouseEventArgs e)
        {
            this.DragEnd(e, true);
        }

        private void DragEnd(MouseEventArgs e, bool isLostMouseCapture)
        {
            bool flag = this.dragStarted;
            this.ReleaseMouseCapture();
            if (flag && this.MouseDragEnd != null)
            {
                this.MouseDragEnd.Invoke(this._element, new MouseDragEventArgs(this, this.lastMouseDownPos, this.lastPos, this.lastPos)
                {
                    MouseEventArgs = e,
                    IsLostMouseCapture = isLostMouseCapture
                });
            }
        }

        private void InitializeWheel()
        {
            this._element.MouseWheel += (s, e) =>
            {
                if (this._isMouseOver && this._wheel != null)
                {
                    MouseWheelEventArgs mouseWheelEventArgs = new MouseWheelEventArgs((double)(e.Delta / 120), e.GetPosition(this._element.CACSGetRootVisual()));
                    mouseWheelEventArgs.Handled = e.Handled;
                    this._wheel.Invoke(this._element, mouseWheelEventArgs);
                    e.Handled = mouseWheelEventArgs.Handled;
                }
            };
        }

        private void InitializeRightButton()
        {
            this._element.MouseRightButtonDown += new MouseButtonEventHandler(this.OnContextMenuInvoked);
        }

        private void FinalizeRightButton()
        {
            this._element.MouseRightButtonDown -= new MouseButtonEventHandler(this.OnContextMenuInvoked);
        }

        private void OnContextMenuInvoked(object sender, MouseButtonEventArgs e)
        {
            if (this._isMouseOver && this._contextMenuInvoked != null)
            {
                this._contextMenuInvoked.Invoke(this._element, e);
                e.Handled = true;
            }
        }
    }
}
