using System;
using System.Windows.Forms;

namespace RealAnggaran
{
    public static class cSaveInvoke
    {
        public delegate void SafeControlInvokeHandler<TControl>(TControl control)
                where TControl : Control;

        public static void SafeControlInvoke<TControl>(this TControl control, SafeControlInvokeHandler<TControl> action)
          where TControl : Control
        {
            if (control != null && control.InvokeRequired)
            {
                IAsyncResult asyncResult = control.BeginInvoke(action, new object[] { control });
                control.EndInvoke(asyncResult);
            }
            else
            {
                action(control);
            }
        }

        public delegate TReturn SafeControlInvokeHandler<TReturn, TControl>(TControl control)
          where TControl : Control;

        public static TReturn SafeControlInvoke<TReturn, TControl>(this TControl control, SafeControlInvokeHandler<TReturn, TControl> action)
          where TControl : Control
        {
            if (control != null && control.InvokeRequired)
            {
                IAsyncResult asyncResult = control.BeginInvoke(action, new object[] { control });
                return (TReturn)control.EndInvoke(asyncResult);
            }
            return action(control);
        }

        public delegate TReturn SafeControlInvokeHandler<TReturn, TControl, TArg>(TControl control, TArg args)
          where TControl : Control;

        public static TReturn SafeControlInvoke<TReturn, TControl, TArg>(this TControl control, TArg args, SafeControlInvokeHandler<TReturn, TControl, TArg> action)
          where TControl : Control
        {
            if (control != null && control.InvokeRequired)
            {
                IAsyncResult asyncResult = control.BeginInvoke(action, new object[] { control, args });
                return (TReturn)control.EndInvoke(asyncResult);
            }
            return action(control, args);
        }
    }
}
