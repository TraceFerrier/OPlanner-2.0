using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PlannerNameSpace
{
    public enum SuccessResult
    {
        Open,
        Done,
    }

    public enum ProductGroupStages
    {
        NotStarted,
        ShowCreateProductGroupDialog,
        ShowEditProductGroupDialog,
        EditProductGroupDialogActive,
        CreateProductGroupDialogActive,
        GroupMemberDiscoveryCompleted,
        CreatingProductGroupItem,
        CreatingProductGroupItemCompleted,
        CreateDiscoveredGroupMembersCompleted,
        CommittingDiscoveredGroupMembers,
        CommittingDiscoveredGroupMembersCompleted,
        UpdateExistingProductGroupCompleted,
        RebuildingProductGroupMembership,
        ProductGroupMembershipRebuildComplete,
        EditProductGroupCompleted,
        EndOfCycle,
    }

    public class SelectProductGroupDialog : BaseDialog
    {
        public ProductGroupItem SelectedProductGroupItem { get; set; }
        public ProductGroupItem NewProductGroupItem { get; set; }
        DispatcherTimer UpdateTimer { get; set; }
        ProductGroupStages ProductGroupStage { get; set; }
        bool CreateProductGroupOperationInProgress { get; set; }
        bool ExistingProductGroupAliasesChanged { get; set; }
        
        SelectProductGroupContent DialogContent;
        Button OpenButton;
        Button EditButton;

        public SelectProductGroupDialog() : base ("Select Product Group")
        {
            DialogContent = new SelectProductGroupContent();
            Dialog.Content = DialogContent;

            ExistingProductGroupAliasesChanged = false;
            CreateProductGroupOperationInProgress = false;
            ProductGroupStage = ProductGroupStages.NotStarted;

            EnsureProductGroupGridItems();

            DialogContent.ProductGroupGrid.SelectionChanged += ProductGroupGrid_SelectionChanged;
            Planner.Instance.ItemRepository.GroupMemberDiscoveryComplete += GroupMemberManager_GroupMemberDiscoveryComplete;
            Planner.Instance.ItemRepository.RebuildGroupMembershipComplete += GroupMemberManager_RebuildGroupMembershipComplete;
            Planner.Instance.ItemRepository.CreateDiscoveredGroupMembersComplete += GroupMemberManager_CreateDiscoveredGroupMembersComplete;
            Planner.StoreCommitComplete += Handle_StoreCommitComplete;

            OpenButton = AddButton("Open");
            OpenButton.Click += OpenButton_Click;

            EditButton = AddButton("Edit...");
            EditButton.Click += EditButton_Click;

            AddButton("New Product Group...").Click += NewButton_Click;
            AddButton("Done").Click += CancelButton_Click;

            UpdateUI();

            UpdateTimer = new DispatcherTimer();
            UpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Start();

        }

        void EnsureProductGroupGridItems()
        {
            //StoreItemCollection<ProductGroupItem> allGroups = Planner.Instance.ItemRepository.ProductGroupItems;
            //StoreItemCollection<ProductGroupItem> compatibleGroups = new StoreItemCollection<ProductGroupItem>();
            //foreach (ProductGroupItem productGroup in allGroups)
            //{
            //    if (productGroup.IsCompatibleWithCurrentStore)
            //    {
            //        compatibleGroups.Add(productGroup);
            //    }
            //}

            //compatibleGroups.Sort((x, y) => y.LastChangedDate.CompareTo(x.LastChangedDate));
            //DialogContent.ProductGroupGrid.ItemsSource = compatibleGroups;
        }

        StoreItemCollection<ProductGroupItem> m_productGroupItems = null;
        public StoreItemCollection<ProductGroupItem> ProductGroupItems
        {
            get
            {
                if (m_productGroupItems == null)
                {
                    StoreItemCollection<ProductGroupItem> allGroups = Planner.Instance.ItemRepository.ProductGroupItems;
                    m_productGroupItems = new StoreItemCollection<ProductGroupItem>();
                    foreach (ProductGroupItem productGroup in allGroups)
                    {
                        if (productGroup.IsCompatibleWithCurrentStore)
                        {
                            m_productGroupItems.Add(productGroup);
                        }
                    }

                    m_productGroupItems.Sort((x, y) => y.LastChangedDate.CompareTo(x.LastChangedDate));
                }

                return m_productGroupItems;
            }
        }

        void Handle_StoreCommitComplete(object sender, StoreCommitCompleteEventArgs e)
        {
            CreateProductGroupOperationInProgress = false;
            if (ProductGroupStage == ProductGroupStages.CreatingProductGroupItem)
            {
                ProductGroupStage = ProductGroupStages.CreatingProductGroupItemCompleted;
            }
            else if (ProductGroupStage == ProductGroupStages.CommittingDiscoveredGroupMembers)
            {
                ProductGroupStage = ProductGroupStages.CommittingDiscoveredGroupMembersCompleted;
            }
            else if (ProductGroupStage == ProductGroupStages.EditProductGroupDialogActive)
            {
                ProductGroupStage = ProductGroupStages.UpdateExistingProductGroupCompleted;
            }
            else if (ProductGroupStage == ProductGroupStages.ProductGroupMembershipRebuildComplete)
            {
                ProductGroupStage = ProductGroupStages.EditProductGroupCompleted;
            }
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTimer.Stop();
            Cancel();
        }

        void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SelectedProductGroupItem.IsCompatibleWithCurrentStore)
            {
                UserMessage.Show("The product group you selected is not compatible with the current Store (" + HostItemStore.Instance.StoreName + "). Please pick another group.");
                return;
            }

            UpdateTimer.Stop();
            Result = DialogResult.Open;
            Close();
        }

        void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupStage = ProductGroupStages.ShowCreateProductGroupDialog;
            CreateProductGroupOperationInProgress = false;
        }

        void GroupMemberManager_GroupMemberDiscoveryComplete()
        {
            CreateProductGroupOperationInProgress = false;
            ProductGroupStage = ProductGroupStages.GroupMemberDiscoveryCompleted;
        }

        void GroupMemberManager_CreateDiscoveredGroupMembersComplete()
        {
            CreateProductGroupOperationInProgress = false;
            ProductGroupStage = ProductGroupStages.CreateDiscoveredGroupMembersCompleted;
        }

        void GroupMemberManager_RebuildGroupMembershipComplete()
        {
            CreateProductGroupOperationInProgress = false;
            ProductGroupStage = ProductGroupStages.ProductGroupMembershipRebuildComplete;
        }

        void UpdateTimer_Tick(object sender, System.EventArgs e)
        {
            if (!CreateProductGroupOperationInProgress)
            {
                // Show ProductGroupEditor with a newly created product group
                if (ProductGroupStage == ProductGroupStages.ShowCreateProductGroupDialog)
                {
                    CreateProductGroupOperationInProgress = true;
                    ProductGroupStage = ProductGroupStages.CreateProductGroupDialogActive;

                    if (NewProductGroupItem == null)
                    {
                        NewProductGroupItem = ScheduleStore.Instance.CreateStoreItem<ProductGroupItem>(ItemTypeID.ProductGroup);
                    }

                    //ProductGroupEditor dialog = new ProductGroupEditor(this, NewProductGroupItem);
                    //dialog.ShowDialog();

                    //if (dialog.WelcomeState == PlannerNameSpace.WelcomeState.Ok)
                    //{
                    //    Planner.Instance.ItemRepository.DiscoverGroupMembers(NewProductGroupItem);
                    //}
                }

                // Show ProductGroupEditor to edit an existing group
                else if (ProductGroupStage == ProductGroupStages.ShowEditProductGroupDialog)
                {
                    CreateProductGroupOperationInProgress = true;
                    ProductGroupStage = ProductGroupStages.EditProductGroupDialogActive;

                    //ProductGroupEditor dialog = new ProductGroupEditor(this, SelectedProductGroupItem);
                    //dialog.ShowDialog();

                    //if (dialog.WelcomeState == PlannerNameSpace.WelcomeState.Ok)
                    //{
                    //    SelectedProductGroupItem.BeginSaveImmediate();
                    //    ExistingProductGroupAliasesChanged = dialog.AliasesChanged;
                    //    SelectedProductGroupItem.SaveImmediate();
                    //}
                    //else
                    //{
                    //    CreateProductGroupOperationInProgress = false;
                    //}
                }

                // The discovery of members for a new product group has completed
                else if (ProductGroupStage == ProductGroupStages.GroupMemberDiscoveryCompleted)
                {
                    AsyncObservableCollection<MemberDescriptor> discoveredGroupMembers = Planner.Instance.ItemRepository.DiscoveredGroupMembers;
                    //ConfirmNewProductGroupDialog dialog = new ConfirmNewProductGroupDialog(this, NewProductGroupItem, discoveredGroupMembers);
                    //dialog.ShowDialog();

                    //if (dialog.DialogConfirmed)
                    //{
                    //    CreateProductGroupOperationInProgress = true;
                    //    NewProductGroupItem.HostItemStoreName = HostItemStore.Instance.StoreName;
                    //    NewProductGroupItem.SaveNewItem();
                    //    ProductGroupStage = ProductGroupStages.CreatingProductGroupItem;
                    //    Planner.Instance.ItemRepository.CommitChanges(true);
                    //}
                    //else
                    //{
                    //    ProductGroupStage = ProductGroupStages.ShowCreateProductGroupDialog;
                    //    CreateProductGroupOperationInProgress = false;
                    //}
                }

                // The commit of the new ProductGroupItem has completed.
                else if (ProductGroupStage == ProductGroupStages.CreatingProductGroupItemCompleted)
                {
                    NewProductGroupItem.ParentProductGroupKey = NewProductGroupItem.StoreKey;
                    EnsureProductGroupGridItems();
                    Planner.Instance.ItemRepository.CreateDiscoveredGroupMembers(NewProductGroupItem.StoreKey);
                }

                // The creation of previously discovered members for a new product group has completed.
                else if (ProductGroupStage == ProductGroupStages.CreateDiscoveredGroupMembersCompleted)
                {
                    // Now commit all the newly discovered members
                    CreateProductGroupOperationInProgress = true;
                    ProductGroupStage = ProductGroupStages.CommittingDiscoveredGroupMembers;
                    Planner.Instance.ItemRepository.CommitChanges(true);
                }

                // The creation of a new product group is fully completed
                else if (ProductGroupStage == ProductGroupStages.CommittingDiscoveredGroupMembersCompleted)
                {
                    CreateProductGroupOperationInProgress = true;
                    ProductGroupStage = ProductGroupStages.EndOfCycle;

                    //NewProductGroupSuccessfulDialog dialog = new NewProductGroupSuccessfulDialog(this, NewProductGroupItem);
                    //dialog.ShowDialog();

                    //if (dialog.SuccessResult == SuccessResult.Open)
                    //{
                    //    SelectedProductGroupItem = NewProductGroupItem;
                    //    WelcomeState = PlannerNameSpace.WelcomeState.Open;
                    //    Dialog.Close();
                    //}
                }

                else if (ProductGroupStage == ProductGroupStages.UpdateExistingProductGroupCompleted)
                {
                    ProductGroupStage = ProductGroupStages.RebuildingProductGroupMembership;
                    if (ExistingProductGroupAliasesChanged)
                    {
                        CreateProductGroupOperationInProgress = true;
                        Planner.Instance.ItemRepository.RebuildProductGroupMembership(SelectedProductGroupItem);
                    }
                    else
                    {
                        ProductGroupStage = ProductGroupStages.EditProductGroupCompleted;
                    }
                }

                else if (ProductGroupStage == ProductGroupStages.ProductGroupMembershipRebuildComplete)
                {
                    CreateProductGroupOperationInProgress = true;
                    Planner.Instance.ItemRepository.CommitChanges(true);

                }

                else if (ProductGroupStage == ProductGroupStages.EditProductGroupCompleted)
                {
                    ProductGroupStage = ProductGroupStages.EndOfCycle;
                    if (ExistingProductGroupAliasesChanged && SelectedProductGroupItem == Planner.Instance.ItemRepository.CurrentProductGroup && Planner.Instance.IsStartupComplete)
                    {
                        Result = DialogResult.Restart;
                        Dialog.Close();
                    }
                }
            }
        }

        void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupStage = ProductGroupStages.ShowEditProductGroupDialog;
        }

        void ProductGroupGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProductGroupItem = DialogContent.ProductGroupGrid.SelectedItem as ProductGroupItem;
            UpdateUI();
        }

        void UpdateUI()
        {
            if (SelectedProductGroupItem != null && SelectedProductGroupItem != Planner.Instance.ItemRepository.CurrentProductGroup)
            {
                OpenButton.IsEnabled = true;
            }
            else
            {
                OpenButton.IsEnabled = false;
            }

            EditButton.IsEnabled = SelectedProductGroupItem == null ? false : true;
        }
    }
}
