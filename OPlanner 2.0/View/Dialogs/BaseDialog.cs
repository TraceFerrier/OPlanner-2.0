using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PlannerNameSpace
{
    public enum DialogResult
    {
        OK,
        Cancel,
        Quit,
        Next,
        Open,
        Restart,
        MoreInfo,
        NotSet,
    }

    public abstract class BaseDialog
    {
        public DialogResult Result { get; set; }

        protected List<Button> Buttons;
        protected ModernDialog Dialog;

        public BaseDialog(string Title)
        {
            Buttons = new List<Button>();
            Result = DialogResult.NotSet;
            Dialog = new ModernDialog { SizeToContent = SizeToContent.WidthAndHeight, Title = Title, MaxWidth=1000, MaxHeight = 1000 };
            Dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Dialog.DataContext = this;
        }

        protected Button AddButton(Button button)
        {
            Buttons.Add(button);
            return button;
        }

        protected Button AddButton(string content)
        {
            Button button = new Button { Content = content, MinHeight = 21, MinWidth = 65, Margin = new Thickness(4, 0, 0, 0) };
            AddButton(button);
            return button;
        }

        public virtual void ShowDialog()
        {
            Dialog.Buttons = Buttons;
            Dialog.ShowDialog();
        }

        public virtual void Close()
        {
            Dialog.Close();
        }

        public virtual void Cancel()
        {
            Result = DialogResult.Cancel;
            Close();
        }
    }
}
