using System.IO;

namespace PlannerNameSpace
{
    public class Attachment
    {
        MemoryStream m_attachmentStream;
        MemoryStream m_backingAttachmentStream;
        bool m_isDirty;

        public bool IsDirty
        {
            get { return m_isDirty; }
            set { m_isDirty = value; }
        }

        public Attachment()
        {
            m_isDirty = false;
        }

        public MemoryStream AttachmentStream
        {
            get { return m_attachmentStream; }

            set
            {
                m_attachmentStream = value;
                if (m_backingAttachmentStream == null)
                {
                    m_backingAttachmentStream = value;
                }
            }
        }

        public MemoryStream BackingAttachmentStream
        {
            get { return m_backingAttachmentStream; }
            set { m_backingAttachmentStream = value; }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called by the host store when a save of the current value of this attachment to
        /// the backing store is completed.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void NotifySaveCompleted()
        {
            IsDirty = false;
            m_backingAttachmentStream = m_attachmentStream;
        }

        public void UndoChanges()
        {
            IsDirty = false;
            m_attachmentStream = m_backingAttachmentStream;
        }
    }
}
