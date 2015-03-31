using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace PlannerNameSpace
{
    public class DocumentAttachmentCollection : ItemAttachmentCollection<DocumentAttachment>
    {
        public DocumentAttachmentCollection(StoreItem item)
            : base(item)
        {

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Saves all the documents in this collection as RichTextFileAttachments.
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void SaveAll()
        {
            if (Attachments.Count > 0)
            {
                foreach (KeyValuePair<string, DocumentAttachment> kvp in Attachments)
                {
                    string propName = kvp.Key;
                    Debug.WriteLine("SaveAll: " + propName);
                    DocumentAttachment itemDocument = kvp.Value;
                    if (itemDocument != null)
                    {
                        if (itemDocument.IsDirty)
                        {
                            Item.Store.SaveRichTextFileAttachment(Item.DSItem, propName, itemDocument.AttachmentStream);
                        }
                        else
                        {
                            Debug.WriteLine("SaveAll: document wasn't dirty");
                        }
                    }
                }
            }
        }

        public bool IsDocumentLoaded(string propName)
        {
            return Attachments.ContainsKey(propName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of the given property as a document represented by a stream.
        /// </summary>
        //------------------------------------------------------------------------------------
        public MemoryStream GetDocumentValue(string propName)
        {
            if (!Attachments.ContainsKey(propName))
            {
                MemoryStream mStream = Item.Store.GetRichTextFileAttachmentStream(Item, propName);

                if (mStream != null)
                {
                    Attachments.Add(propName, new DocumentAttachment { AttachmentStream = mStream });
                }
                else
                {
                    return null;
                }
            }

            return Attachments[propName].AttachmentStream;
        }

    }
}
