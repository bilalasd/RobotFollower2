using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace RobotFollowerWPF2
{
    public static class Extension
    {
        public static void SynchronizedInvoke(this ISynchronizeInvoke sync, Action action)
        {
            // If the invoke is not required, then invoke here and get out.
            if (!sync.InvokeRequired)
            {
                // Execute action.
                action();

                // Get out.
                return;
            }

            // Marshal to the required context.
            sync.Invoke(action, new object[] { });
        }
        public static T[] Slice<T>(this T[] source, int index, int length)
        {
            T[] slice = new T[length];
            Array.Copy(source, index, slice, 0, length);
            return slice;
        }

        public static bool ControlInvokeRequired(System.Windows.Controls.Control c, Action a)
        {
            if (c.Dispatcher.CheckAccess())
            {
                c.Dispatcher.BeginInvoke((Action)(delegate { a(); }));
            }
            else return false;

            return true;
        }


    }
}
