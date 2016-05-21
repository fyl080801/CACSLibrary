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
    public class CACSKeyEventArgs
    {
        private bool _handled;

        public KeyEventArgs Args { get; set; }

        public Key Key { get; set; }

        public int PlatformKeyCode { get; set; }

        public bool Handled
        {
            get { return this._handled; }
            set
            {
                this._handled = value;
                if (this.Args != null)
                {
                    this.Args.Handled = value;
                }
            }
        }

        public CACSKeyEventArgs()
        {
        }

        public CACSKeyEventArgs(KeyEventArgs e)
        {
            this.Key = e.Key;
            this.Handled = e.Handled;
            this.PlatformKeyCode = e.CACSGetPlatformKeyCode();
            this.Args = e;
        }
    }
}
