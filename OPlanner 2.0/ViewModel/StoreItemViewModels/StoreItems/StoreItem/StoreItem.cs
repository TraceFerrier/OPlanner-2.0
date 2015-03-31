using PlannerNameSpace.Model;
using ProductStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace PlannerNameSpace
{
    public enum PersistStates
    {
        Dummy,
        NewUncommitted,
        NewCommitted,
        PersistedToStore,
    }

    public enum ChildState
    {
        NotApplicable,
        HasChildren,
        HasNoChildren,
    }

    public enum CommitErrorStates
    {
        NoError,
        MergingChanges,
        ErrorAccessingAttachmentShare,
    }

    //------------------------------------------------------------------------------------
    /// <summary>
    /// The base class for all items that are backed by a record in the backing store.
    /// </summary>
    //------------------------------------------------------------------------------------
    public abstract partial class StoreItem : BasePropertyChanged
    {
        public static string StatusPropName = StringUtils.GetPropertyName((StoreItem s) => s.Status);

        protected int m_id;
        protected string m_title;
        
        private Dictionary<string, ItemProperty> ItemProperties;
        private AsyncObservableCollection<ItemProperty> m_changedProperties;
        private Dictionary<string, object> LocalProperties;
        private object SyncLockItemProperties;
        private object SyncLockLocalProperties;

        public DocumentAttachmentCollection ItemDocuments;
        public ImageAttachmentCollection ItemImages;
        public StoreItemCollection<StoreItem> SelfList { get; set; }

        public IList AttachedFileNamesToCommit;
        public string AttachedFileNameToRemove { get; set; }
        public StoreChangeAction ChangeAction { get; set; }
        public PersistStates PersistState { get; set; }

        public bool IsNew { get { return PersistState == PersistStates.NewUncommitted || PersistState == PersistStates.NewCommitted; } }
        public bool IsPersisted { get { return PersistState == PersistStates.PersistedToStore; } }
        public bool IsInImmediateSave { get; set; }
        public CommitErrorStates CommitErrorState { get; set; }

        public Datastore Store { get { return StoreID == null ? null : StoreID.Store; } }
        public DatastoreID StoreID { get; set; }
        public DatastoreItem DSItem;
        Guid UncommittedGuid { get; set; }
        public abstract ItemTypeID StoreItemType { get; }
        public abstract string DefaultItemPath { get; }
        public virtual bool IsGlobalItem { get { return false; } }

        public string StoreItemTypeName { get { return StoreItemType.ToString(); } }
        public string StoreChangeTypeName { get { return ChangeAction.ToString(); } }
        public const string FeatureTeamNotAllowedSelection = Constants.c_None;
        static BitmapSource GenericProfileSource = null;
        protected IRepository m_repository;

        public List<ItemProperty> GetItemProperties()
        {
            return ItemProperties.ToList();
        }

        public StoreItem()
        {
            m_id = 0;
            SyncLockItemProperties = new object();
            SyncLockLocalProperties = new object();
            ItemProperties = new Dictionary<string, ItemProperty>();
            m_changedProperties = new AsyncObservableCollection<ItemProperty>();

            ItemDocuments = new DocumentAttachmentCollection(this);
            ItemImages = new ImageAttachmentCollection(this);
            SelfList = new StoreItemCollection<StoreItem>();
            SelfList.Add(this);

            AttachedFileNamesToCommit = null;

            UncommittedGuid = Guid.NewGuid();
            PersistState = PersistStates.Dummy;
            IsInImmediateSave = false;
            CommitErrorState = CommitErrorStates.NoError;
        }

        public void SetRepository(IRepository repository)
        {
            m_repository = repository;
        }

        public AsyncObservableCollection<ItemProperty> ChangedProperties
        {
            get
            {
                return m_changedProperties;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if this property is in the collection of properties that have changed,
        /// and need to be saved to the server.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsPropertyOnChangeList(ItemProperty property)
        {
            return m_changedProperties.Contains(property);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if the given property must be open to read (i.e. this may be an
        /// expensive operation in terms of time to retreive from the server).
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool MustPropertyBeOpenToRead(ItemProperty property)
        {
            if (Store == null)
            {
                return false;
            }

            return Store.MustBeOpenToRead(property.DSPropName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Called after this item has been completely created or loaded, to allow each 
        /// object to do any custom finalization.  Derived classes should always call the base
        /// implementation before doing any other work.
        /// </summary>
        //------------------------------------------------------------------------------------
        protected virtual void FinalizeItem()
        {
            //m_id = ID;
            //m_title = Title;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns true if there are any changes in this item that need to be persisted to
        /// the back-end store.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool IsDirty
        {
            get
            {
                if (ItemDocuments.IsDirty || ItemImages.IsDirty || m_changedProperties.Count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public string Description
        {
            get { return GetStringValue(Datastore.PropNameDescription); }
            set { SetStringValue(Datastore.PropNameDescription, value); }
        }

        public override string ToString()
        {
            return StoreKey + ":" + m_title;
        }

        public static string GetStoreNameFromKey(string key)
        {
            return key.Substring(0, key.IndexOf('|'));
        }

        public static int GetIDFromKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                int id;
                string strID = key.Substring(key.IndexOf('|') + 1);
                if (Int32.TryParse(strID, out id))
                {
                    return id;
                }
            }

            return 0;
        }

        public string StoreKey
        {
            get
            {
                if (PersistState == PersistStates.PersistedToStore)
                {
                    return GetItemKey(StoreID, ID);
                }
                else
                {
                    return UncommittedStoreKey;
                }
            }
        }

        public string UncommittedStoreKey
        {
            get { return UncommittedGuid.ToString(); }
        }

        public static string GetHostItemKey(int ID)
        {
            return GetItemKey(HostItemStore.Instance.StoreID, ID);
        }

        public static string GetItemKey(DatastoreID storeID, int ID)
        {
            return storeID.Name + "|" + ID.ToString();
        }


        public DateTime LastSavedTime { get; set; }

        public string IDQualifiedTitle
        {
            get { return ID.ToString() + ": " + Title; }
        }

        public bool IsAssignedToActive
        {
            get
            {
                return StringUtils.StringsMatch(AssignedTo, StatusValues.Active);
            }
        }

        string m_productTeamAssignedTo = null;
        public string ProductTeamAssignedTo
        {
            get
            {
                if (m_productTeamAssignedTo == null)
                {
                    m_productTeamAssignedTo = AssignedTo;
                    if (!IsAssignedToActive && Planner.Instance.ItemRepository.GetMemberByAlias(m_productTeamAssignedTo) == null)
                    {
                        m_productTeamAssignedTo = Constants.c_ExternalTeam;
                    }
                }

                return m_productTeamAssignedTo;
            }

            set
            {
                string proposedAssignedTo = value;
                if (proposedAssignedTo != Constants.c_ExternalTeam)
                {
                    AssignedTo = proposedAssignedTo;
                }
            }
        }

        public virtual void OnResolution()
        {

        }

        public string ResolvedBy
        {
            get { return GetStringValue(Datastore.PropNameResolvedBy); }
            set { SetStringValue(Datastore.PropNameResolvedBy, value); }
        }

        public bool SubtypeChanged
        {
            get
            {
                return PreviousSubtype != null && !StringUtils.StringsMatch(PreviousSubtype, Subtype);
            }
        }

        public string PreviousSubtype { get; set; }

        public AsyncObservableCollection<AllowedValue> AvailableSubtypes
        {
            get { return PropertyAllowedValues.GetAvailableSubtypes(this); }
        }

        public AsyncObservableCollection<GroupMemberItem> AssignableGroupMembers
        {
            get { return Planner.Instance.ItemRepository.GetAssignableGroupMembers(); }
        }

        public GroupMemberItem AssignedToGroupMember
        {
            get
            {
                return Planner.Instance.ItemRepository.GetMemberByAlias(AssignedTo);
            }

            set
            {
                AssignedTo = value.Alias;
            }
        }

        public bool IsDocumentLoaded(string propName)
        {
            return ItemDocuments.IsDocumentLoaded(propName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of the given property as a document represented by a stream.
        /// </summary>
        //------------------------------------------------------------------------------------
        public MemoryStream GetDocumentValue(string propName)
        {
            return ItemDocuments.GetDocumentValue(propName);
        }

        public async Task<MemoryStream> GetDocumentValueAsync(string propName)
        {
            return await Task.Run(() =>
            {
                return GetDocumentValue(propName);
            });

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the value of the given property as a document represented by the given stream.
        /// </summary>
        //------------------------------------------------------------------------------------
        private void SetDocumentValue(string propName, MemoryStream value)
        {
            ItemDocuments.SetAttachmentValue(propName, value);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the value of the given property as a document represented by a stream.
        /// Note: not used currently - see below.
        /// </summary>
        //------------------------------------------------------------------------------------
        public BitmapSource GetImageValue(string propName, StoreItem itemToNotify = null, [CallerMemberName] string publicPropName = null)
        {
            return ItemImages.GetFileAttachmentImageValue(propName, itemToNotify, publicPropName);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the value of the given property as a document represented by the given stream.
        /// Note: we no longer cache images from active directory, but this may be used in the
        /// future to allow setting of user custom images.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SetImageValue(string propName, MemoryStream value)
        {
            ItemImages.SetAttachmentValue(propName, value);
        }

        public BitmapSource GetActiveDirectoryImageValue(string alias, StoreItem itemToNotify = null, [CallerMemberName] string publicPropName = null)
        {
            BitmapSource source = null;
            if (alias != null)
            {
                source = ItemImages.GetActiveDirectoryImageValue(alias, itemToNotify, publicPropName);
            }

            if (source == null)
            {
                source = GetGenericProfileImage();
            }

            return source;
        }

        public static BitmapSource GetGenericProfileImage()
        {
            if (GenericProfileSource == null)
            {
                var hBitmap = Properties.Resources.GenericProfile.GetHbitmap();
                GenericProfileSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }

            return GenericProfileSource;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Extracts the current document from the given RichTextBox, and saves it in this
        /// item's document property bag, associated with the given propName.  In addition,
        /// the plain text from the box is also stored in the given plainPropName property.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string SaveDocumentFromRichTextBox(string propName, RichTextBox richTextBox)
        {
            TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            MemoryStream mStream = new MemoryStream();
            range.Save(mStream, DataFormats.Rtf);

            SetDocumentValue(propName, mStream);

            return range.Text;
        }

        AsyncObservableCollection<FileAttachment> m_attachedFiles;
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the file names of all the user-attached files for this item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<FileAttachment> AttachedFiles
        {
            get
            {
                if (m_attachedFiles == null)
                {
                    m_attachedFiles = Store.GetAttachedFiles(this);
                }

                return m_attachedFiles;
            }
        }

        public int AttachedFilesCount
        {
            get
            {
                return AttachedFiles.Count;
            }
        }

        public bool IsOneOrMoreFilesAttached
        {
            get
            {
                return AttachedFilesCount > 0;
            }
        }

        public void RefreshAttachedFileInfo()
        {
            m_attachedFiles = null;
            NotifyPropertyChanged(()=> AttachedFiles);
            NotifyPropertyChanged(() => AttachedFilesCount);
        }

        public string GetRefreshedBackingStringValue<T>(Expression<Func<T>> expression)
        {
            return (string)GetRefreshedBackingValue(expression);
        }

        public object GetRefreshedBackingValue<T>(Expression<Func<T>> expression)
        {
            string publicPropName = StringUtils.GetExpressionName(expression);

            if (ItemProperties.ContainsKey(publicPropName))
            {
                ItemProperty itemProperty = ItemProperties[publicPropName];
                itemProperty.ReadFromStore();
                return itemProperty.BackingValue;
            }

            return null;
        }

        public object GetPreviousValue<T>(Expression<Func<T>> expression)
        {
            string publicPropName = StringUtils.GetExpressionName(expression);

            if (ItemProperties.ContainsKey(publicPropName))
            {
                return ItemProperties[publicPropName].PreviousValue;
            }

            return null;
        }

        public string GetPreviousStringValue<T>(Expression<Func<T>> expression)
        {
            return TypeUtils.GetStringValue(GetPreviousValue(expression));
        }

        public int GetPreviousIntValue<T>(Expression<Func<T>> expression)
        {
            return TypeUtils.GetIntValue(GetPreviousValue(expression));
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of all the allowed values for the given Product Studio field, given
        /// the current state of the given item.
        /// </summary>
        //------------------------------------------------------------------------------------
        public AsyncObservableCollection<AllowedValue> GetFieldAllowedValues(string propName)
        {
            return Store.GetFieldAllowedValues(this, propName);
        }

        public virtual ChildState GetChildState()
        {
            return ChildState.NotApplicable;
        }

        public virtual string GetItemStatusText()
        {
            return Status;
        }

        public virtual TrainCommitmentStatusValue GetCommitmentStatus()
        {
            return TrainCommitmentStatusValue.NotCommitted;
        }

        public virtual string GetItemIssueType()
        {
            return null;
        }

        public ContextMenu GetContextMenu(Window ownerWindow)
        {
            ContextMenu menu = new ContextMenu();
            PopulateContextMenu(ownerWindow, menu);

            if (Planner.Instance.IsCurrentUserOPlannerDev())
            {
                AddCopyIDMenu(menu);
            }
            else
            {
                switch (this.StoreItemType)
                {
                    case ItemTypeID.BacklogItem:
                    case ItemTypeID.WorkItem:
                        AddCopyIDMenu(menu);
                        break;
                }
            }

            return menu;
        }

        void AddCopyIDMenu(ContextMenu menu)
        {
            AddContextMenuItem(menu, "Copy ID", "Copy.png", CopyID_Click);
        }

        protected void CopyID_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(ID.ToString());
        }

        public virtual void PopulateContextMenu(Window ownerWindow, ContextMenu menu)
        {

        }

        protected void AddContextMenuItem(ContextMenu menu, string title, string imageName, RoutedEventHandler handler)
        {
            MenuUtils.AddContextMenuItem(menu, title, imageName, handler);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of strings recapping the description change history for the given 
        /// bug.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string ItemDescriptionHistory
        {
            get
            {
                return Store.GetItemDescriptionHistory(this);
            }
        }

        public static string GetEffectiveShipCycle(string shipCycle)
        {
            if (StringUtils.StringsMatch(shipCycle, "Gemini") ||
                StringUtils.StringsMatch(shipCycle, "Gemini - RTrel") ||
                StringUtils.StringsMatch(shipCycle, "Gemini - SPOrel"))
            {
                shipCycle = "Gemini";
            }

            return shipCycle;
        }

        public void BeginSaveImmediate()
        {
            IsInImmediateSave = true;
        }

        public void CancelSaveImmediate()
        {
            IsInImmediateSave = false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Saves this item to the backing store immediately, without sending it to the change
        /// queue.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SaveImmediate()
        {
            Planner.Instance.ItemRepository.SaveItemImmediate(this);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes this item from the in-memory cache, and sends it to the change queue to
        /// be deleted from the backing store during the next commit operation.
        /// </summary>
        //------------------------------------------------------------------------------------
        public virtual void DeleteItem()
        {
            Planner.Instance.ItemRepository.DeleteItem(this);
        }

        public void CloseItem()
        {
            Planner.Instance.ItemRepository.CloseItem(this);
        }

        public void SaveAttachment(Attachment attachment)
        {
            attachment.IsDirty = true;
            Planner.Instance.ItemRepository.SaveAttachment(this);
        }

        public void SaveNewItem()
        {
            Planner.Instance.ItemRepository.NewItemAccepted(this);
        }
    }
}
