using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlannerNameSpace
{
    public class ApplicationWelcome
    {
        public DialogResult Result;
        public ProductGroupItem SelectedProductGroupItem { get; set; }

        public ApplicationWelcome()
        {

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Begins the Welcome sequence that will enable the user to open an existing product
        /// group, or create a new group for their team.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void BeginWelcome()
        {
            WelcomeDialog welcomeDialog = new WelcomeDialog();

            welcomeDialog.ShowDialog();
            Result = welcomeDialog.Result;
            if (Result == DialogResult.Next)
            {
                SelectProductGroupDialog selectProductGroupDialog = new SelectProductGroupDialog();
                selectProductGroupDialog.ShowDialog();
                Result = selectProductGroupDialog.Result;

                if (Result == DialogResult.Open)
                {
                    SelectedProductGroupItem = selectProductGroupDialog.SelectedProductGroupItem;
                }
            }

        }
    }
}
