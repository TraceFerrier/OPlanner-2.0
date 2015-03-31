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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlannerNameSpace
{
    public class StartupOptionsDialog : BaseDialog
    {
        public bool ShouldUseClone { get; set; }
        public bool ShouldClearCurrentProductGroup { get; set; }
        public bool ShouldClearUserPreferences { get; set; }
        StartupOptionsDialogContent DialogContent;

        public StartupOptionsDialog()
            : base("OPlanner Startup Options")
        {
            DialogContent = new StartupOptionsDialogContent();
            Dialog.Content = DialogContent;
            ShouldUseClone = false;
            ShouldClearCurrentProductGroup = false;
            ShouldClearUserPreferences = false;

            DialogContent.UserPreferencesPathBox.Text = UserPreferences.GetUserPreferencesFullPath();

            AddButton(Dialog.OkButton).Click += OkButton_Click;
            AddButton(Dialog.CancelButton).Click += CancelButton_Click;
            AddButton("Quit").Click += QuitButton_Click;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ShouldUseClone = DialogContent.UseCloneCheckBox.IsChecked == true;
            ShouldClearCurrentProductGroup = DialogContent.ClearProductGroupCheckBox.IsChecked == true;
            ShouldClearUserPreferences = DialogContent.ClearPreferencesCheckBox.IsChecked == true;
            Result = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResult.Quit;
            Close();
        }
    }
}
