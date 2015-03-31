using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    class WelcomeDialog : BaseDialog
    {
        WelcomeDialogContent DialogContent;
        public WelcomeDialog()
            : base("Welcome to OPlanner")
        {
            DialogContent = new WelcomeDialogContent();
            Dialog.Content = DialogContent;

            AddButton("Next >").Click += NextButton_Click;
            AddButton("More Info...").Click += MoreInfoButton_Click;
            AddButton(Dialog.CancelButton).Click += CancelButton_Click;
        }

        void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Cancel();
        }

        void MoreInfoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Result = DialogResult.MoreInfo;
        }

        void NextButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Result = DialogResult.Next;
            Close();
        }
    }
}
