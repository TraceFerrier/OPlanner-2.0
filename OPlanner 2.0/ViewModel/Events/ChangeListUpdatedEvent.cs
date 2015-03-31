using System;

namespace PlannerNameSpace
{
    public class ChangeListUpdatedEventArgs : EventArgs
    {
        public int ChangesToSave { get; set; }

        public ChangeListUpdatedEventArgs(int changesToSave)
        {
            ChangesToSave = changesToSave;
        }
    }
}
