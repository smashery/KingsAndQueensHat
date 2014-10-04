using KingsAndQueensHat.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KingsAndQueensHat.View
{
    /// <summary>
    /// Interaction logic for CancelDialog.xaml
    /// </summary>
    public partial class CancelDialog : Window, ICancelDialog
    {
        private Task _task;
        private System.Threading.CancellationTokenSource _source;

        public CancelDialog(Task task, System.Threading.CancellationTokenSource source)
        {
            InitializeComponent();
            _task = task;
            _source = source;

            // Close the dialog if it completes normally
            task.ContinueWith((t) => this.Dispatcher.Invoke(Close));
        }

        public void ShowUntilCompleteOrCancelled()
        {
            ShowDialog();
        }

        /// <summary>
        /// Closing should terminate the given task, wait for it to terminate, 
        /// then allow the earlier task.ContinueWith to do the magic of closing the window
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            _source.Cancel();
            if (!_task.IsCompleted)
            {
                e.Cancel = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _source.Cancel();
        }
    }
}
