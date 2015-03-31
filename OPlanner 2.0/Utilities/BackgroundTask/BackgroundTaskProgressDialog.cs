using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class BackgroundTaskProgressDialog : BasePropertyChanged
    {
        private ModernDialog Dialog;
        private bool _isIndeterminate;
        private string _progressDescription;
        private string _progressMessage;
        private bool m_closeRequested;

        public BackgroundTaskProgressDialog()
        {
            Dialog = new ModernDialog();
            Dialog.Content = new BackgroundProgressContent { Margin = new Thickness(3) };
            Dialog.Buttons = new System.Windows.Controls.Button[] { Dialog.CancelButton };
            Dialog.DataContext = this;
            m_closeRequested = false;
            Dialog.Closing += Dialog_Closing;
            Dialog.CancelButton.Click += CancelButton_Click;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (CancelRequested != null)
            {
                CancelRequested(this, EventArgs.Empty);
            }
        }

        void Dialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!m_closeRequested)
            {
                e.Cancel = true;

                if (CancelRequested != null)
                {
                    CancelRequested(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler CancelRequested;

        public string Title
        {
            get
            {
                return Dialog.Title;
            }

            set
            {
                Dialog.Title = value;
            }
        }

        public string ProgressDescription
        {
            get { return _progressDescription; }
            set { _progressDescription = value; NotifyPropertyChangedByName(); }
        }

        public string ProgressMessage
        {
            get { return _progressMessage; }
            set { _progressMessage = value; NotifyPropertyChangedByName(); }
        }

        int _progressValue;
        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }

            set
            {
                _progressValue = value;
                NotifyPropertyChangedByName();
            }
        }

        public void ShowDialog()
        {
            Dialog.ShowDialog();
        }

        public void CloseDialog()
        {
            m_closeRequested = true;
            Dialog.Close();
        }

        public bool IsIndeterminate
        {
            get 
            {
                return _isIndeterminate; 
            }
            
            set 
            {
                _isIndeterminate = value;
                NotifyPropertyChangedByName();
            }
        }

        public bool IsCancelButtonEnabled
        {
            get { return Dialog.CancelButton.IsEnabled; }

            set { Dialog.CancelButton.IsEnabled = value; }
        }


    }
}
