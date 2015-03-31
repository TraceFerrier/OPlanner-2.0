using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PlannerNameSpace
{
    public abstract class ItemAttachmentCollection<T> where T : Attachment, new()
    {
        protected Dictionary<string, T> Attachments;
        protected StoreItem Item;

        public ItemAttachmentCollection(StoreItem item)
        {
            Attachments = new Dictionary<string, T>();
            Item = item;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the value of the given property as a document represented by the given stream.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetAttachmentValue(string propName, MemoryStream value)
        {
            IsDirty = true;

            if (!Attachments.ContainsKey(propName))
            {
                Attachments.Add(propName, new T());
            }

            Attachment attachment = Attachments[propName];
            if (attachment != null)
            {
                attachment.AttachmentStream = value;
                Item.SaveAttachment(attachment);
            }
        }

        public abstract void SaveAll();

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called by the host store when the save started by SaveAll is successfully
        /// completed.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void NotifySaveCompleted()
        {
            IsDirty = false;

            foreach (KeyValuePair<string, T> kvp in Attachments)
            {
                string propName = kvp.Key;
                T attachment = kvp.Value;
                if (attachment != null)
                {
                    attachment.NotifySaveCompleted();
                }
            }
        }

        public void UndoChanges()
        {
            foreach (KeyValuePair<string, T> kvp in Attachments)
            {
                T attachment = kvp.Value;
                if (attachment != null)
                {
                    attachment.UndoChanges();
                }
            }
        }

        private bool m_isDirty;

        public bool IsDirty
        {
            get { return m_isDirty; }
            set { m_isDirty = value; }
        }

    }
}
